using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repos;
using Domain.Models;
using Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class UserRepository
     : GenericRepository<User>, IUser
    {
        public UserRepository(CenterDbContext context)
            : base(context)
        {
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

       
        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<bool> IsUserNameTakenAsync(string userName)
        {
            return await _dbSet
                .AnyAsync(u => u.UserName == userName);
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email == email);
        }

      
        public async Task<User?> GetWithPersonAsync(int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }



}
