using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface ISessionStatusService
    {
        Task<Result<SessionStatus>> CreateAsync(string name);
        Task<Result<SessionStatus>> GetByIdAsync(int id);
        Task<Result<IEnumerable<SessionStatus>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}
