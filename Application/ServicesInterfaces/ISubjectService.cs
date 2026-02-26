using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface ISubjectService
    {
        Task<Result<Subject>> CreateAsync(string name);
        Task<Result<Subject>> GetByIdAsync(int id);
        Task<Result<Subject>> GetByNameAsync(string name);
        Task<Result<IEnumerable<Subject>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}
