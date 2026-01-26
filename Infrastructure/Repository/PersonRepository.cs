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
    public class PersonRepository
     : GenericRepository<Person>, IPerson
    {
        public PersonRepository(CenterDbContext context)
            : base(context)
        {
        }

       
        public async Task<Person?> GetByPhoneAsync(string phone)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.PersonalPhone == phone);
        }

        
        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            return await _dbSet
                .AnyAsync(p => p.PersonalPhone == phone);
        }
    }

}
