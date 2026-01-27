using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Interfaces.Repos;
using Domain.Models;
using Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repository
{
    public class StudentRepository
    : GenericRepository<Student>, IStudent
    {
        public StudentRepository(CenterDbContext context) : base(context) { }

        public async Task<Student?> GetByCodeAsync(string code)
            => await _dbSet
                .Include(s => s.Person)
                .FirstOrDefaultAsync(s => s.Code == code);

        public async Task<IEnumerable<Student>> GetByGradeAsync(int gradeId)
            => await _dbSet
                .Where(s => EF.Property<int>(s, "GradeId") == gradeId)
        .ToListAsync();

        public async Task<bool> ExistsByCodeAsync(string code)
            => await _dbSet.AnyAsync(s => s.Code == code);

        public async Task<Student?> GetWithRegistrationsAsync(int studentId)
            => await _dbSet
                .Include("_registrations")
                .FirstOrDefaultAsync(s => s.Id == studentId);

        public async Task<Student?> GetWithGradeAsync(int studentId)
          => await _dbSet
              .Include("Grade")
              .FirstOrDefaultAsync(s => s.Id == studentId);

        public async Task<bool> CanJoinAsync(int studentId, int groupId)
        {
            return await _context.Students
            .AnyAsync(s =>
                s.Id == studentId &&
                _context.Groups.Any(g =>
                    g.Id == groupId &&
                    g.SubjectGrade.GradeId == s.Grade.Id));
        }
    }

}
