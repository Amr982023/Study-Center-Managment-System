using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IStudentService
    {
        Task<Result<Student>> CreateAsync(
            string firstName, string lastName, string phone, string gender,
            string code, string guardianPhone, int gradeId, string? midName = null);
        Task<Result<Student>> GetByIdAsync(int id);
        Task<Result<Student>> GetByPhoneAsync(string phone);
        Task<Result<Student>> GetByCodeAsync(string code);
        Task<Result<Student>> GetWithRegistrationsAsync(int id);
        Task<Result<Student>> GetWithGradeAsync(int id);
        Task<Result<IEnumerable<Student>>> GetAllAsync();
        Task<Result<IEnumerable<Student>>> GetByGradeAsync(int gradeId);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
