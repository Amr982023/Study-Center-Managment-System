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
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _uow;

        public GradeService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<Grade>> CreateAsync(string name)
        {
            var existing = await _uow.Grades.GetByNameAsync(name);
            if (existing is not null)
                return Result<Grade>.Failure($"Grade '{name}' already exists.");

            var result = Grade.Create(name);
            if (!result.IsSuccess)
                return Result<Grade>.Failure(result.ErrorMessage!);

            await _uow.Grades.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<Grade>.Success(result.Value!);
        }

        public async Task<Result<Grade>> GetByIdAsync(int id)
        {
            var grade = await _uow.Grades.GetByIdAsync(id);
            return grade is null
                ? Result<Grade>.Failure("Grade not found.")
                : Result<Grade>.Success(grade);
        }

        public async Task<Result<IEnumerable<Grade>>> GetAllAsync()
        {
            var grades = await _uow.Grades.GetAllAsync();
            return Result<IEnumerable<Grade>>.Success(grades);
        }

        public async Task<Result<Grade>> GetByNameAsync(string name)
        {
            var grade = await _uow.Grades.GetByNameAsync(name);
            return grade is null
                ? Result<Grade>.Failure("Grade not found.")
                : Result<Grade>.Success(grade);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var grade = await _uow.Grades.GetByIdAsync(id);
            if (grade is null)
                return Result<bool>.Failure("Grade not found.");

            await _uow.Grades.DeleteAsync(grade);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
