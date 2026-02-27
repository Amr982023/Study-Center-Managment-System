using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces;
using Domain.Interfaces.UOW;
using Domain.Models;
using Domain.Services;

namespace Application.Services;

public class StudentGroupAggregationService : IStudentGroupAggregationService
{
    private readonly IUnitOfWork _uow;
    private readonly EnrollmentDomainService _domain;

    public StudentGroupAggregationService(IUnitOfWork uow, EnrollmentDomainService domain)
    {
        _uow = uow;
        _domain = domain;
    }

    // ── Enroll ────────────────────────────────────────────────────────────────
    /// <summary>
    /// Validates and persists a new enrollment.
    /// All business rules live in EnrollmentDomainService.
    /// </summary>
    public async Task<Result<StudentGroupAggregation>> EnrollAsync(int studentId, int groupId)
    {
        // 1. Load aggregates
        var student = await _uow.Students.GetWithGradeAsync(studentId);
        if (student is null)
            return Result<StudentGroupAggregation>.Failure("Student not found.");

        var group = await _uow.Groups.GetWithSubjectGradeDetailsAsync(groupId);
        if (group is null)
            return Result<StudentGroupAggregation>.Failure("Group not found.");

        // 2. Gather flags for domain service
        bool canJoin = student.Grade?.Id == group.SubjectGrade?.Grade?.Id;

        bool alreadyEnrolledSameGroup =
            await _uow.StudentGroupAggregations.ExistsWithSameGroupAsync(studentId, groupId);

        bool alreadyEnrolledSameSubject =
            await _uow.StudentGroupAggregations.ExistsWithSameSubjectAsync(
                studentId, group.SubjectGrade?.Subject?.Id ?? 0);

        // 3. Delegate all validation + creation to domain service
        var result = _domain.Enroll(student, group, canJoin,
                                     alreadyEnrolledSameGroup,
                                     alreadyEnrolledSameSubject);

        if (!result.IsSuccess)
            return Result<StudentGroupAggregation>.Failure(result.ErrorMessage!);

        // 4. Persist
        await _uow.StudentGroupAggregations.AddAsync(result.Value!);
        await _uow.SaveChangesAsync();
        return Result<StudentGroupAggregation>.Success(result.Value!);
    }

    // ── Queries ───────────────────────────────────────────────────────────────
    public async Task<Result<IEnumerable<StudentGroupAggregation>>> GetByStudentAsync(int studentId)
    {
        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student is null)
            return Result<IEnumerable<StudentGroupAggregation>>.Failure("Student not found.");

        var enrollments = await _uow.StudentGroupAggregations.GetByStudentAsync(studentId);
        return Result<IEnumerable<StudentGroupAggregation>>.Success(enrollments);
    }

    public async Task<Result<IEnumerable<StudentGroupAggregation>>> GetByGroupAsync(int groupId)
    {
        var group = await _uow.Groups.GetByIdAsync(groupId);
        if (group is null)
            return Result<IEnumerable<StudentGroupAggregation>>.Failure("Group not found.");

        var enrollments = await _uow.StudentGroupAggregations.GetByGroupAsync(groupId);
        return Result<IEnumerable<StudentGroupAggregation>>.Success(enrollments);
    }

    // ── Unenroll ──────────────────────────────────────────────────────────────
    public async Task<Result<bool>> UnenrollAsync(int studentId, int groupId)
    {
        bool isEnrolled =
            await _uow.StudentGroupAggregations.ExistsWithSameGroupAsync(studentId, groupId);

        // Domain service validates the rule
        var check = _domain.Disenroll(isEnrolled);
        if (!check.IsSuccess)
            return Result<bool>.Failure(check.ErrorMessage!);

        await _uow.StudentGroupAggregations.RemoveAsync(studentId, groupId);
        await _uow.SaveChangesAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UnenrollAllAsync(int studentId)
    {
        var student = await _uow.Students.GetByIdAsync(studentId);
        if (student is null)
            return Result<bool>.Failure("Student not found.");

        bool hasAny =
            (await _uow.StudentGroupAggregations.GetByStudentAsync(studentId)).Any();

        // Domain service validates the rule
        var check = _domain.DisenrollAll(hasAny);
        if (!check.IsSuccess)
            return Result<bool>.Failure(check.ErrorMessage!);

        await _uow.StudentGroupAggregations.RemoveAllAsync(studentId);
        await _uow.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}