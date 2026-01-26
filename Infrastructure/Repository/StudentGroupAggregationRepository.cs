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
    public class StudentGroupAggregationRepository
    : GenericRepository<StudentGroupAggregation>,
      IStudentGroupAggregationRepository
    {
        public StudentGroupAggregationRepository(CenterDbContext context)
            : base(context)
        {
        }

        public async Task<bool> ExistsAsync(int studentId, int groupId)
        {
            return await _dbSet.AnyAsync(sga =>
                EF.Property<int>(sga, "StudentId") == studentId &&
                EF.Property<int>(sga, "GroupId") == groupId);
        }

        
        public async Task<IEnumerable<StudentGroupAggregation>> GetByStudentAsync(int studentId)
        {
            return await _dbSet
                .Include(sga => sga.Group)
                .Where(sga => EF.Property<int>(sga, "StudentId") == studentId)
                .OrderByDescending(sga => sga.Id)
                .ToListAsync();
        }

        
        public async Task<IEnumerable<StudentGroupAggregation>> GetByGroupAsync(int groupId)
        {
            return await _dbSet
                .Include(sga => sga.Student)
                .Where(sga => EF.Property<int>(sga, "GroupId") == groupId)
                .OrderBy(sga => sga.Id)
                .ToListAsync();
        }
    }

}
