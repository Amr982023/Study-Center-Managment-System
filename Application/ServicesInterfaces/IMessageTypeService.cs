using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IMessageTypeService
    {
        Task<Result<MessageType>> CreateAsync(string name);
        Task<Result<MessageType>> GetByIdAsync(int id);
        Task<Result<IEnumerable<MessageType>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}
