using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IMessageService
    {
        Task<Result<Message>> CreateAsync(int messageTypeId, string text);
        Task<Result<Message>> GetByIdAsync(int id);
        Task<Result<IEnumerable<Message>>> GetAllAsync();
        Task<Result<IEnumerable<Message>>> GetByTypeAsync(int messageTypeId);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
