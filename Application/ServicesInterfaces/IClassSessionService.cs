using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IClassSessionService
    {
        Task<Result<ClassSession>> CreateAsync(int groupId, int sessionNumber, DateTime sessionDateTime, int statusId);
        Task<Result<ClassSession>> GetByIdAsync(int id);
        Task<Result<ClassSession>> GetWithGroupAsync(int id);
        Task<Result<IEnumerable<ClassSession>>> GetAllAsync();
        Task<Result<IEnumerable<ClassSession>>> GetUpcomingAsync(DateTime from);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
