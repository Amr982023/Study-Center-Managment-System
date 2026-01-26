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
    public class GradeRepository
    : GenericRepository<Grade>, IGrade
    {
        public GradeRepository(CenterDbContext context)
            : base(context)
        {
        }
        public async Task<Grade?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(g => g.Name == name);
        }
    }
}
