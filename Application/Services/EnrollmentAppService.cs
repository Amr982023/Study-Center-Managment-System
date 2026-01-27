using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces.UOW;
using Domain.Models;
using Domain.Services;

namespace Application.Services
{
    public class EnrollmentAppService : IEnrollmentAppService
    {
        private readonly IUnitOfWork _uow;
        private readonly EnrollmentDomainService _domainService;

        public EnrollmentAppService(
            IUnitOfWork uow,
            EnrollmentDomainService domainService)
        {
            _uow = uow;
            _domainService = domainService;
        }

        public async Task<Result<StudentGroupAggregation>> EnrollStudentAsync(int studentId,int groupId)
        {
            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<StudentGroupAggregation>.Failure("Student not found");

            var group = await _uow.Groups.GetWithSubjectGradeDetailsAsync(groupId);
            if (group is null)
                return Result<StudentGroupAggregation>.Failure("Group not found");

            bool canJoin =
                await _uow.Students
                    .CanJoinAsync(studentId, groupId);

            bool sameGroup =
                await _uow.StudentGroupAggregations
                    .ExistsWithSameGroupAsync(studentId, groupId);

            bool sameSubject =
                await _uow.StudentGroupAggregations
                    .ExistsWithSameSubjectAsync(
                        studentId,
                        group.SubjectGrade.SubjectId);

            var result = _domainService.Enroll(
                student,
                group,
                canJoin,
                sameGroup,
                sameSubject);

            if (!result.IsSuccess)
                return result;

            await _uow.StudentGroupAggregations.AddAsync(result.Value);
            await _uow.SaveChangesAsync();

            return result;
        }

        public async Task<Result<bool>> DisenrollAsync(int studentId, int groupId)
        {
            bool isEnrolled =
                await _uow.StudentGroupAggregations
                    .ExistsWithSameGroupAsync(studentId, groupId);

            var decision = _domainService.Disenroll(isEnrolled);
            if (!decision.IsSuccess)
                return decision;

            await _uow.StudentGroupAggregations.RemoveAsync(studentId, groupId);
            await _uow.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DisenrollAllAsync(int studentId)
        {
            bool hasAny =
                await _uow.StudentGroupAggregations
                    .HasAnyEnrollmentAsync(studentId);

            var decision = _domainService.DisenrollAll(hasAny);
            if (!decision.IsSuccess)
                return decision;

            await _uow.StudentGroupAggregations.RemoveAllAsync(studentId);
            await _uow.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

    }

}
