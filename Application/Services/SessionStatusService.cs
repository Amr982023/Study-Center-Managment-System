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
    public class SessionStatusService : ISessionStatusService
    {
        private readonly IUnitOfWork _uow;

        public SessionStatusService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<SessionStatus>> CreateAsync(string name)
        {
            var existing = await _uow.SessionStatuses.AnyAsync(s => s.Name == name);
            if (existing)
                return Result<SessionStatus>.Failure($"Session status '{name}' already exists.");

            var result = SessionStatus.Create(name);
            if (!result.IsSuccess)
                return Result<SessionStatus>.Failure(result.ErrorMessage!);

            await _uow.SessionStatuses.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<SessionStatus>.Success(result.Value!);
        }

        public async Task<Result<SessionStatus>> GetByIdAsync(int id)
        {
            var status = await _uow.SessionStatuses.GetByIdAsync(id);
            return status is null
                ? Result<SessionStatus>.Failure("Session status not found.")
                : Result<SessionStatus>.Success(status);
        }

        public async Task<Result<IEnumerable<SessionStatus>>> GetAllAsync()
        {
            var statuses = await _uow.SessionStatuses.GetAllAsync();
            return Result<IEnumerable<SessionStatus>>.Success(statuses);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var status = await _uow.SessionStatuses.GetByIdAsync(id);
            if (status is null)
                return Result<bool>.Failure("Session status not found.");

            await _uow.SessionStatuses.DeleteAsync(status);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
