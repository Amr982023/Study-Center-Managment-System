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
    public class GroupRepository
     : GenericRepository<Group>, IGroup
    {
        public GroupRepository(CenterDbContext context)
            : base(context)
        {
        }

      
        public async Task<Group?> GetWithSessionsAsync(int groupId)
        {
            return await _dbSet
                .Include("_sessions")             
                .FirstOrDefaultAsync(g => g.Id == groupId);
        }

      
        public async Task<IEnumerable<Group>> GetByGradeAsync(int subjectGradeHandlerId)
        {
            return await _dbSet
                .Where(g =>
                    EF.Property<int>(g, "SubjectGradeHandlerId") == subjectGradeHandlerId)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }
    }

}
