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
    public class ExamResultService : IExamResultService
    {
        private readonly IUnitOfWork _uow;

        public ExamResultService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<ExamResult>> CreateAsync(
            int examId, int studentId, int statusId, int resultScore, bool exceptFullMark)
        {
            var exam = await _uow.Exams.GetByIdAsync(examId);
            if (exam is null)
                return Result<ExamResult>.Failure("Exam not found.");

            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<ExamResult>.Failure("Student not found.");

            var status = await _uow.ExamStatuses.GetByIdAsync(statusId);
            if (status is null)
                return Result<ExamResult>.Failure("Exam status not found.");

            // Prevent duplicate result for same exam+student
            var existing = await _uow.ExamResults.GetAsync(examId, studentId);
            if (existing is not null)
                return Result<ExamResult>.Failure(
                    "An exam result already exists for this student and exam.");

            // Validate score doesn't exceed full mark (unless exceptFullMark)
            if (!exceptFullMark && resultScore > exam.FullMark)
                return Result<ExamResult>.Failure(
                    $"Result ({resultScore}) exceeds full mark ({exam.FullMark}).");

            var result = ExamResult.Create(exam, student, status, resultScore, exceptFullMark);
            if (!result.IsSuccess)
                return Result<ExamResult>.Failure(result.ErrorMessage!);

            await _uow.ExamResults.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<ExamResult>.Success(result.Value!);
        }

        public async Task<Result<ExamResult>> GetByIdAsync(int id)
        {
            var examResult = await _uow.ExamResults.GetByIdAsync(id);
            return examResult is null
                ? Result<ExamResult>.Failure("Exam result not found.")
                : Result<ExamResult>.Success(examResult);
        }

        public async Task<Result<ExamResult>> GetAsync(int examId, int studentId)
        {
            var examResult = await _uow.ExamResults.GetAsync(examId, studentId);
            return examResult is null
                ? Result<ExamResult>.Failure("Exam result not found.")
                : Result<ExamResult>.Success(examResult);
        }

        public async Task<Result<IEnumerable<ExamResult>>> GetByStudentAsync(int studentId)
        {
            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<IEnumerable<ExamResult>>.Failure("Student not found.");

            var results = await _uow.ExamResults.GetByStudentAsync(studentId);
            return Result<IEnumerable<ExamResult>>.Success(results);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var examResult = await _uow.ExamResults.GetByIdAsync(id);
            if (examResult is null)
                return Result<bool>.Failure("Exam result not found.");

            await _uow.ExamResults.DeleteAsync(examResult);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
