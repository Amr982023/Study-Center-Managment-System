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
    public class SubjectRepository
     : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(CenterDbContext context)
            : base(context)
        {
        }

      
        public async Task<Subject?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Name == name);
        }
    }

}
