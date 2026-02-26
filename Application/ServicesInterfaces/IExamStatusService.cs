using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IExamStatusService
    {
        Task<Result<ExamStatus>> CreateAsync(string name);
        Task<Result<ExamStatus>> GetByIdAsync(int id);
        Task<Result<IEnumerable<ExamStatus>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}
