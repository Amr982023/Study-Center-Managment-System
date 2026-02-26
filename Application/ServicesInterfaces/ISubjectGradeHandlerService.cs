using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface ISubjectGradeHandlerService
    {
        Task<Result<SubjectGradeHandler>> CreateAsync(int subjectId, int gradeId, decimal fees);
        Task<Result<SubjectGradeHandler>> GetByIdAsync(int id);
        Task<Result<SubjectGradeHandler>> GetAsync(int subjectId, int gradeId);
        Task<Result<IEnumerable<SubjectGradeHandler>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}
