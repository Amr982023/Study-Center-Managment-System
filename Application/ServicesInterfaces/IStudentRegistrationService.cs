using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IStudentRegistrationService
    {
        Task<Result<StudentRegistration>> RegisterAsync(int studentId, int sessionId);
        Task<Result<IEnumerable<StudentRegistration>>> GetByStudentAsync(int studentId);
        Task<Result<bool>> ExistsAsync(int studentId, int sessionId);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
