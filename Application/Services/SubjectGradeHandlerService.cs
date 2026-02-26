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
    public class SubjectGradeHandlerService : ISubjectGradeHandlerService
    {
        private readonly IUnitOfWork _uow;

        public SubjectGradeHandlerService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<SubjectGradeHandler>> CreateAsync(int subjectId, int gradeId, decimal fees)
        {
            var subject = await _uow.Subjects.GetByIdAsync(subjectId);
            if (subject is null)
                return Result<SubjectGradeHandler>.Failure("Subject not found.");

            var grade = await _uow.Grades.GetByIdAsync(gradeId);
            if (grade is null)
                return Result<SubjectGradeHandler>.Failure("Grade not found.");

            var existing = await _uow.SubjectGradeHandlers.GetAsync(subjectId, gradeId);
            if (existing is not null)
                return Result<SubjectGradeHandler>.Failure("This subject-grade combination already exists.");

            var result = SubjectGradeHandler.Create(subject, grade, fees);
            if (!result.IsSuccess)
                return Result<SubjectGradeHandler>.Failure(result.ErrorMessage!);

            await _uow.SubjectGradeHandlers.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<SubjectGradeHandler>.Success(result.Value!);
        }

        public async Task<Result<SubjectGradeHandler>> GetByIdAsync(int id)
        {
            var handler = await _uow.SubjectGradeHandlers.GetByIdAsync(id);
            return handler is null
                ? Result<SubjectGradeHandler>.Failure("SubjectGradeHandler not found.")
                : Result<SubjectGradeHandler>.Success(handler);
        }

        public async Task<Result<SubjectGradeHandler>> GetAsync(int subjectId, int gradeId)
        {
            var handler = await _uow.SubjectGradeHandlers.GetAsync(subjectId, gradeId);
            return handler is null
                ? Result<SubjectGradeHandler>.Failure("SubjectGradeHandler not found.")
                : Result<SubjectGradeHandler>.Success(handler);
        }

        public async Task<Result<IEnumerable<SubjectGradeHandler>>> GetAllAsync()
        {
            var handlers = await _uow.SubjectGradeHandlers.GetAllAsync();
            return Result<IEnumerable<SubjectGradeHandler>>.Success(handlers);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var handler = await _uow.SubjectGradeHandlers.GetByIdAsync(id);
            if (handler is null)
                return Result<bool>.Failure("SubjectGradeHandler not found.");

            await _uow.SubjectGradeHandlers.DeleteAsync(handler);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
