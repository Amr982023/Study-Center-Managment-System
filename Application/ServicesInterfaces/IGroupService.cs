using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IGroupService
    {
        Task<Result<Group>> CreateAsync(string name, int subjectGradeHandlerId, DateTime firstSessionDate);
        Task<Result<Group>> GetByIdAsync(int id);
        Task<Result<Group>> GetWithSessionsAsync(int id);
        Task<Result<Group>> GetWithSubjectGradeDetailsAsync(int id);
        Task<Result<IEnumerable<Group>>> GetAllAsync();
        Task<Result<IEnumerable<Group>>> GetByGradeAsync(int gradeId);
        Task<Result<IEnumerable<Group>>> GetByStudentWithDetailsAsync(int studentId);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
