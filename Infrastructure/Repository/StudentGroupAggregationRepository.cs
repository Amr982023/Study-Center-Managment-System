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

        public async Task<bool> ExistsWithSameGroupAsync(int studentId, int groupId)
        {
            return await _dbSet.AnyAsync(sga =>
                EF.Property<int>(sga, "StudentId") == studentId &&
                EF.Property<int>(sga, "GroupId") == groupId);
        }

        public async Task<bool> ExistsWithSameSubjectAsync(
        int studentId,
        int subjectId)
        {
            return await _context.StudentGroupAggregations
                .AnyAsync(sga =>
                    sga.StudentId == studentId &&
                    sga.Group.SubjectGrade.SubjectId == subjectId);
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

        public async Task<bool> HasAnyEnrollmentAsync(int studentId)
        {
            return await _context.StudentGroupAggregations
        .AnyAsync(x => x.StudentId == studentId);
        }


        public async Task RemoveAsync(int studentId, int groupId)
        {
            var entity = await _context.StudentGroupAggregations
                .FirstOrDefaultAsync(x =>
                    x.StudentId == studentId &&
                    x.GroupId == groupId);

            if (entity != null)
                _context.StudentGroupAggregations.Remove(entity);
        }

        public async Task RemoveAllAsync(int studentId)
        {
            var entities = await _context.StudentGroupAggregations
                .Where(x => x.StudentId == studentId)
                .ToListAsync();

            _context.StudentGroupAggregations.RemoveRange(entities);
        }

    }

}
