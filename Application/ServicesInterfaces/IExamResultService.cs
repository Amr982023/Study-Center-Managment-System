using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IExamResultService
    {
        Task<Result<ExamResult>> CreateAsync(int examId, int studentId, int statusId, int resultScore, bool exceptFullMark);
        Task<Result<ExamResult>> GetByIdAsync(int id);
        Task<Result<ExamResult>> GetAsync(int examId, int studentId);
        Task<Result<IEnumerable<ExamResult>>> GetByStudentAsync(int studentId);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
