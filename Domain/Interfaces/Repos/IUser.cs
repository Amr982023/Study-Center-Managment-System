using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IUser : IGeneric<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUserNameAsync(string userName);

        Task<bool> IsUserNameTakenAsync(string userName);

        Task<bool> IsEmailTakenAsync(string Email);

        Task<User?> GetWithPersonAsync(int userId);
    }

}
