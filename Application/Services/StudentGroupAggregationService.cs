using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces.UOW;
using Domain.Models;

namespace Application.Services
{
    public class StudentGroupAggregationService:IStudentGroupAggregationService
    {
        private readonly IUnitOfWork _uow;

        public StudentGroupAggregationService(IUnitOfWork uow) => _uow = uow;

        /// <summary>
        /// Enroll a student into a group. Validates:
        /// - Student and Group exist
        /// - Not already enrolled in this group
        /// - Not already enrolled in another group with the same subject (conflict check)
        /// - Student grade matches group's grade
        /// </summary>
        public async Task<Result<StudentGroupAggregation>> EnrollAsync(int studentId, int groupId)
        {
            var student = await _uow.Students.GetWithGradeAsync(studentId);
            if (student is null)
                return Result<StudentGroupAggregation>.Failure("Student not found.");

            var group = await _uow.Groups.GetWithSubjectGradeDetailsAsync(groupId);
            if (group is null)
                return Result<StudentGroupAggregation>.Failure("Group not found.");

            // Check if student grade matches group grade
            if (student.Grade.Id != group.SubjectGrade.Grade.Id)
                return Result<StudentGroupAggregation>.Failure(
                    "Student grade does not match group grade.");

            // Check duplicate enrollment in same group
            if (await _uow.StudentGroupAggregations.ExistsWithSameGroupAsync(studentId, groupId))
                return Result<StudentGroupAggregation>.Failure(
                    "Student is already enrolled in this group.");

            // Check same subject conflict
            if (await _uow.StudentGroupAggregations.ExistsWithSameSubjectAsync(
                    studentId, group.SubjectGrade.Subject.Id))
                return Result<StudentGroupAggregation>.Failure(
                    "Student is already enrolled in a group for this subject.");

            var result = StudentGroupAggregation.Create(student, group);
            if (!result.IsSuccess)
                return Result<StudentGroupAggregation>.Failure(result.ErrorMessage!);

            await _uow.StudentGroupAggregations.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<StudentGroupAggregation>.Success(result.Value!);
        }

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

        public async Task<Result<bool>> UnenrollAsync(int studentId, int groupId)
        {
            if (!await _uow.StudentGroupAggregations.ExistsWithSameGroupAsync(studentId, groupId))
                return Result<bool>.Failure("Enrollment not found.");

            await _uow.StudentGroupAggregations.RemoveAsync(studentId, groupId);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UnenrollAllAsync(int studentId)
        {
            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<bool>.Failure("Student not found.");

            await _uow.StudentGroupAggregations.RemoveAllAsync(studentId);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }

}
