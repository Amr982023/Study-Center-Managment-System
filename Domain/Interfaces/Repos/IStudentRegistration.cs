using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IStudentRegistration
     : IGeneric<StudentRegistration>
    {
        Task<bool> ExistsAsync(int studentId, int sessionId);

        Task<IEnumerable<StudentRegistration>> GetByStudentAsync(int studentId);
    }

}
