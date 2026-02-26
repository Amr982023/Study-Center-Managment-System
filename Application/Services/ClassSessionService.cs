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
    public class ClassSessionService : IClassSessionService
    {
        private readonly IUnitOfWork _uow;

        public ClassSessionService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<ClassSession>> CreateAsync(
            int groupId, int sessionNumber, DateTime sessionDateTime, int statusId)
        {
            var group = await _uow.Groups.GetByIdAsync(groupId);
            if (group is null)
                return Result<ClassSession>.Failure("Group not found.");

            var status = await _uow.SessionStatuses.GetByIdAsync(statusId);
            if (status is null)
                return Result<ClassSession>.Failure("Session status not found.");

            var result = ClassSession.Create(group, sessionNumber, sessionDateTime, status);
            if (!result.IsSuccess)
                return Result<ClassSession>.Failure(result.ErrorMessage!);

            await _uow.ClassSessions.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<ClassSession>.Success(result.Value!);
        }

        public async Task<Result<ClassSession>> GetByIdAsync(int id)
        {
            var session = await _uow.ClassSessions.GetByIdAsync(id);
            return session is null
                ? Result<ClassSession>.Failure("Class session not found.")
                : Result<ClassSession>.Success(session);
        }

        public async Task<Result<ClassSession>> GetWithGroupAsync(int id)
        {
            var session = await _uow.ClassSessions.GetWithGroupAsync(id);
            return session is null
                ? Result<ClassSession>.Failure("Class session not found.")
                : Result<ClassSession>.Success(session);
        }

        public async Task<Result<IEnumerable<ClassSession>>> GetAllAsync()
        {
            var sessions = await _uow.ClassSessions.GetAllAsync();
            return Result<IEnumerable<ClassSession>>.Success(sessions);
        }

        public async Task<Result<IEnumerable<ClassSession>>> GetUpcomingAsync(DateTime from)
        {
            var sessions = await _uow.ClassSessions.GetUpcomingAsync(from);
            return Result<IEnumerable<ClassSession>>.Success(sessions);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var session = await _uow.ClassSessions.GetByIdAsync(id);
            if (session is null)
                return Result<bool>.Failure("Class session not found.");

            await _uow.ClassSessions.DeleteAsync(session);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
