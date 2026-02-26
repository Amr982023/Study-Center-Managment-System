using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IExamService
    {
        Task<Result<Exam>> CreateAsync(int groupId, string name, int fullMark);
        Task<Result<Exam>> GetByIdAsync(int id);
        Task<Result<IEnumerable<Exam>>> GetAllAsync();
        Task<Result<IEnumerable<Exam>>> GetByGroupAsync(int groupId);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
