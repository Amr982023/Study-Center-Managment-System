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
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _uow;

        public SubjectService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<Subject>> CreateAsync(string name)
        {
            var existing = await _uow.Subjects.GetByNameAsync(name);
            if (existing is not null)
                return Result<Subject>.Failure($"Subject '{name}' already exists.");

            var result = Subject.Create(name);
            if (!result.IsSuccess)
                return Result<Subject>.Failure(result.ErrorMessage!);

            await _uow.Subjects.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<Subject>.Success(result.Value!);
        }

        public async Task<Result<Subject>> GetByIdAsync(int id)
        {
            var subject = await _uow.Subjects.GetByIdAsync(id);
            return subject is null
                ? Result<Subject>.Failure("Subject not found.")
                : Result<Subject>.Success(subject);
        }

        public async Task<Result<IEnumerable<Subject>>> GetAllAsync()
        {
            var subjects = await _uow.Subjects.GetAllAsync();
            return Result<IEnumerable<Subject>>.Success(subjects);
        }

        public async Task<Result<Subject>> GetByNameAsync(string name)
        {
            var subject = await _uow.Subjects.GetByNameAsync(name);
            return subject is null
                ? Result<Subject>.Failure("Subject not found.")
                : Result<Subject>.Success(subject);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var subject = await _uow.Subjects.GetByIdAsync(id);
            if (subject is null)
                return Result<bool>.Failure("Subject not found.");

            await _uow.Subjects.DeleteAsync(subject);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
