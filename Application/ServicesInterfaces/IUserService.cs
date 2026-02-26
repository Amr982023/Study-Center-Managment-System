using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IUserService
    {
     
        Task<Result<User>> CreateAsync(
  string firstName, string lastName, string phone, string gender,
  string userName, string email, string permission, string hashedPassword,
  string? midName = null);
        Task<Result<User>> GetByIdAsync(int id);
        Task<Result<User>> GetWithPersonAsync(int id);
        Task<Result<User>> GetByEmailAsync(string email);
        Task<Result<User>> GetByUserNameAsync(string userName);
        Task<Result<IEnumerable<User>>> GetAllAsync();
        Task<Result<User>> AuthenticateAsync(string userName, string hashedPassword);
        Task<Result<bool>> UpdatePermissionAsync(int userId, string permission);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
