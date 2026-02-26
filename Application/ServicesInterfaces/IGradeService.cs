using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IGradeService
    {
        Task<Result<Grade>> CreateAsync(string name);
        Task<Result<Grade>> GetByIdAsync(int id);
        Task<Result<Grade>> GetByNameAsync(string name);
        Task<Result<IEnumerable<Grade>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}
