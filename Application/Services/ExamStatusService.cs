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
    public class ExamStatusService : IExamStatusService
    {
        private readonly IUnitOfWork _uow;

        public ExamStatusService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<ExamStatus>> CreateAsync(string name)
        {
            var existing = await _uow.ExamStatuses.AnyAsync(s => s.Name == name);
            if (existing)
                return Result<ExamStatus>.Failure($"Exam status '{name}' already exists.");

            var result = ExamStatus.Create(name);
            if (!result.IsSuccess)
                return Result<ExamStatus>.Failure(result.ErrorMessage!);

            await _uow.ExamStatuses.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<ExamStatus>.Success(result.Value!);
        }

        public async Task<Result<ExamStatus>> GetByIdAsync(int id)
        {
            var status = await _uow.ExamStatuses.GetByIdAsync(id);
            return status is null
                ? Result<ExamStatus>.Failure("Exam status not found.")
                : Result<ExamStatus>.Success(status);
        }

        public async Task<Result<IEnumerable<ExamStatus>>> GetAllAsync()
        {
            var statuses = await _uow.ExamStatuses.GetAllAsync();
            return Result<IEnumerable<ExamStatus>>.Success(statuses);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var status = await _uow.ExamStatuses.GetByIdAsync(id);
            if (status is null)
                return Result<bool>.Failure("Exam status not found.");

            await _uow.ExamStatuses.DeleteAsync(status);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
