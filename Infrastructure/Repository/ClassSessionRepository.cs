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
    public class ClassSessionRepository
    : GenericRepository<ClassSession>, IClassSession
    {
        public ClassSessionRepository(CenterDbContext context)
            : base(context)
        {
        }

   
        public async Task<IEnumerable<ClassSession>> GetUpcomingAsync(DateTime from)
        {
            return await _dbSet
                .Where(s => s.SessionDateTime >= from)
                .OrderBy(s => s.SessionDateTime)
                .ToListAsync();
        }

        public async Task<ClassSession?> GetWithGroupAsync(int sessionId)
        {
            return await _dbSet
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }
    }

}
