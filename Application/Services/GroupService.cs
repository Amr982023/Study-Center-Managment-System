using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces.UOW;
using Domain.Models;

namespace Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _uow;
        public GroupService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<Group>> CreateAsync(string name, int subjectGradeHandlerId, DateTime firstSessionDate)
        {
            var handler = await _uow.SubjectGradeHandlers.GetByIdAsync(subjectGradeHandlerId);
            if (handler is null)
                return Result<Group>.Failure("Subject/Grade handler not found.");

            var result = Group.Create(name, handler, firstSessionDate);
            if (!result.IsSuccess)
                return Result<Group>.Failure(result.ErrorMessage!);

            await _uow.Groups.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<Group>.Success(result.Value!);
        }

        public async Task<Result<Group>> UpdateAsync(int id, string name, int subjectGradeHandlerId, DateTime firstSessionDate)
        {
            var group = await _uow.Groups.GetByIdAsync(id);
            if (group is null)
                return Result<Group>.Failure("Group not found.");

            var handler = await _uow.SubjectGradeHandlers.GetByIdAsync(subjectGradeHandlerId);
            if (handler is null)
                return Result<Group>.Failure("Subject/Grade handler not found.");

            group.Update(name, handler, firstSessionDate);
            await _uow.Groups.UpdateAsync(group);
            await _uow.SaveChangesAsync();
            return Result<Group>.Success(group);
        }

        public async Task<Result<Group>> GetByIdAsync(int id)
        {
            var group = await _uow.Groups.GetByIdAsync(id);
            return group is null
                ? Result<Group>.Failure("Group not found.")
                : Result<Group>.Success(group);
        }

        public async Task<Result<Group>> GetWithSessionsAsync(int id)
        {
            var group = await _uow.Groups.GetWithSessionsAsync(id);
            return group is null
                ? Result<Group>.Failure("Group not found.")
                : Result<Group>.Success(group);
        }

        public async Task<Result<Group>> GetWithSubjectGradeDetailsAsync(int id)
        {
            var group = await _uow.Groups.GetWithSubjectGradeDetailsAsync(id);
            return group is null
                ? Result<Group>.Failure("Group not found.")
                : Result<Group>.Success(group);
        }

        public async Task<Result<IEnumerable<Group>>> GetAllAsync()
        {
            var groups = await _uow.Groups.GetAllAsync();
            return Result<IEnumerable<Group>>.Success(groups);
        }

        public async Task<Result<IEnumerable<Group>>> GetByGradeAsync(int gradeId)
        {
            var groups = await _uow.Groups.GetByGradeAsync(gradeId);
            return Result<IEnumerable<Group>>.Success(groups);
        }

        public async Task<Result<IEnumerable<Group>>> GetByStudentWithDetailsAsync(int studentId)
        {
            var groups = await _uow.Groups.GetByStudentWithDetailsAsync(studentId);
            return Result<IEnumerable<Group>>.Success(groups);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var group = await _uow.Groups.GetByIdAsync(id);
            if (group is null)
                return Result<bool>.Failure("Group not found.");

            await _uow.Groups.DeleteAsync(group);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}