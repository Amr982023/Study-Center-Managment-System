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
    public class StudentRegistrationService : IStudentRegistrationService
    {
        private readonly IUnitOfWork _uow;

        public StudentRegistrationService(IUnitOfWork uow) => _uow = uow;

        /// <summary>
        /// Register a student for a specific class session.
        /// Validates that the student is enrolled in the session's group.
        /// </summary>
        public async Task<Result<StudentRegistration>> RegisterAsync(int studentId, int sessionId)
        {
            // Check duplicate registration
            if (await _uow.StudentRegistrations.ExistsAsync(studentId, sessionId))
                return Result<StudentRegistration>.Failure(
                    "Student is already registered for this session.");

            var student = await _uow.Students.GetWithRegistrationsAsync(studentId);
            if (student is null)
                return Result<StudentRegistration>.Failure("Student not found.");

            var session = await _uow.ClassSessions.GetWithGroupAsync(sessionId);
            if (session is null)
                return Result<StudentRegistration>.Failure("Class session not found.");

            // Validate student is enrolled in the group
            bool canJoin = await _uow.Students.CanJoinAsync(studentId, session.Group.Id);
            if (!canJoin)
                return Result<StudentRegistration>.Failure(
                    "Student is not enrolled in this session's group.");

            var result = student.Register(session);
            if (!result.IsSuccess)
                return Result<StudentRegistration>.Failure(result.ErrorMessage!);

            await _uow.StudentRegistrations.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<StudentRegistration>.Success(result.Value!);
        }

        public async Task<Result<IEnumerable<StudentRegistration>>> GetByStudentAsync(int studentId)
        {
            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<IEnumerable<StudentRegistration>>.Failure("Student not found.");

            var registrations = await _uow.StudentRegistrations.GetByStudentAsync(studentId);
            return Result<IEnumerable<StudentRegistration>>.Success(registrations);
        }

        public async Task<Result<bool>> ExistsAsync(int studentId, int sessionId)
        {
            var exists = await _uow.StudentRegistrations.ExistsAsync(studentId, sessionId);
            return Result<bool>.Success(exists);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var reg = await _uow.StudentRegistrations.GetByIdAsync(id);
            if (reg is null)
                return Result<bool>.Failure("Registration not found.");

            await _uow.StudentRegistrations.DeleteAsync(reg);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
