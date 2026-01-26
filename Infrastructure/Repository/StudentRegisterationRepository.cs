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
    public class StudentRegistrationRepository
    : GenericRepository<StudentRegistration>, IStudentRegistration
    {
        public StudentRegistrationRepository(CenterDbContext context)
            : base(context)
        {
        }

        
        public async Task<bool> ExistsAsync(int studentId, int sessionId)
        {
            return await _dbSet
                .AnyAsync(r =>
                    EF.Property<int>(r, "StudentId") == studentId &&
                    EF.Property<int>(r, "ClassSessionId") == sessionId);
        }

        
        public async Task<IEnumerable<StudentRegistration>> GetByStudentAsync(int studentId)
        {
            return await _dbSet
                .Include(r => r.ClassSession)
                .Where(r => EF.Property<int>(r, "StudentId") == studentId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }
    }

}
