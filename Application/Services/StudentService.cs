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
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _uow;

        public StudentService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<Student>> CreateAsync(
            string firstName, string lastName, string phone, string gender,
            string code, string guardianPhone, int gradeId, string? midName = null)
        {
            if (await _uow.Students.ExistsByCodeAsync(code))
                return Result<Student>.Failure($"Student code '{code}' already exists.");

            if (await _uow.Students.AnyAsync(s => s.PersonalPhone == phone))
                return Result<Student>.Failure("Phone already in use by a student.");

            if (await _uow.Users.AnyAsync(u => u.PersonalPhone == phone))
                return Result<Student>.Failure("Phone already in use by a user.");

            var grade = await _uow.Grades.GetByIdAsync(gradeId);
            if (grade is null)
                return Result<Student>.Failure("Grade not found.");

            var result = Student.Create(firstName, lastName, phone, gender,
                                        code, guardianPhone, grade, midName);
            if (!result.IsSuccess)
                return Result<Student>.Failure(result.ErrorMessage!);

            await _uow.Students.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<Student>.Success(result.Value!);
        }

        public async Task<Result<Student>> GetByIdAsync(int id)
        {
            var student = await _uow.Students.GetByIdAsync(id);
            return student is null
                ? Result<Student>.Failure("Student not found.")
                : Result<Student>.Success(student);
        }

        public async Task<Result<Student>> GetByCodeAsync(string code)
        {
            var student = await _uow.Students.GetByCodeAsync(code);
            return student is null
                ? Result<Student>.Failure("Student not found.")
                : Result<Student>.Success(student);
        }

        public async Task<Result<Student>> GetByPhoneAsync(string phone)
        {
            var student = await _uow.Students.FirstOrDefaultAsync(s => s.PersonalPhone == phone);
            return student is null
                ? Result<Student>.Failure("No student found with this phone number.")
                : Result<Student>.Success(student);
        }

        public async Task<Result<Student>> GetWithRegistrationsAsync(int id)
        {
            var student = await _uow.Students.GetWithRegistrationsAsync(id);
            return student is null
                ? Result<Student>.Failure("Student not found.")
                : Result<Student>.Success(student);
        }

        public async Task<Result<Student>> GetWithGradeAsync(int id)
        {
            var student = await _uow.Students.GetWithGradeAsync(id);
            return student is null
                ? Result<Student>.Failure("Student not found.")
                : Result<Student>.Success(student);
        }

        public async Task<Result<IEnumerable<Student>>> GetAllAsync()
        {
            var students = await _uow.Students.GetAllAsync();
            return Result<IEnumerable<Student>>.Success(students);
        }

        public async Task<Result<IEnumerable<Student>>> GetByGradeAsync(int gradeId)
        {
            var grade = await _uow.Grades.GetByIdAsync(gradeId);
            if (grade is null)
                return Result<IEnumerable<Student>>.Failure("Grade not found.");

            var students = await _uow.Students.GetByGradeAsync(gradeId);
            return Result<IEnumerable<Student>>.Success(students);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var student = await _uow.Students.GetByIdAsync(id);
            if (student is null)
                return Result<bool>.Failure("Student not found.");

            await _uow.StudentGroupAggregations.RemoveAllAsync(id);
            await _uow.Students.DeleteAsync(student);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
    }
