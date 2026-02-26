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
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _uow;

        public ExamService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<Exam>> CreateAsync(int groupId, string name, int fullMark)
        {
            var group = await _uow.Groups.GetByIdAsync(groupId);
            if (group is null)
                return Result<Exam>.Failure("Group not found.");

            var result = Exam.Create(group, name, fullMark);
            if (!result.IsSuccess)
                return Result<Exam>.Failure(result.ErrorMessage!);

            await _uow.Exams.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<Exam>.Success(result.Value!);
        }

        public async Task<Result<Exam>> GetByIdAsync(int id)
        {
            var exam = await _uow.Exams.GetByIdAsync(id);
            return exam is null
                ? Result<Exam>.Failure("Exam not found.")
                : Result<Exam>.Success(exam);
        }

        public async Task<Result<IEnumerable<Exam>>> GetAllAsync()
        {
            var exams = await _uow.Exams.GetAllAsync();
            return Result<IEnumerable<Exam>>.Success(exams);
        }

        public async Task<Result<IEnumerable<Exam>>> GetByGroupAsync(int groupId)
        {
            var group = await _uow.Groups.GetByIdAsync(groupId);
            if (group is null)
                return Result<IEnumerable<Exam>>.Failure("Group not found.");

            var exams = await _uow.Exams.GetByGroupAsync(groupId);
            return Result<IEnumerable<Exam>>.Success(exams);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var exam = await _uow.Exams.GetByIdAsync(id);
            if (exam is null)
                return Result<bool>.Failure("Exam not found.");

            await _uow.Exams.DeleteAsync(exam);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }

}
