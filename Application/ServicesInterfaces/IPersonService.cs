using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IPersonService
    {
        Task<Result<Person>> CreateAsync(string firstName, string lastName, string phone, string gender, string? midName = null);
        Task<Result<Person>> GetByIdAsync(int id);
        Task<Result<Person>> GetByPhoneAsync(string phone);
        Task<Result<bool>> ExistsByPhoneAsync(string phone);
        Task<Result<IEnumerable<Person>>> GetAllAsync();
        Task<Result<bool>> DeleteAsync(int id);
    }
}
