using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IExamResult : IGeneric<ExamResult>
    {
        Task<IEnumerable<ExamResult>> GetByStudentAsync(int studentId);

        Task<ExamResult?> GetAsync(int examId, int studentId);
    }

}
