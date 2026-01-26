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
    public class ExamResultRepository
    : GenericRepository<ExamResult>, IExamResult
    {
        public ExamResultRepository(CenterDbContext context)
            : base(context)
        {
        }

        
        public async Task<IEnumerable<ExamResult>> GetByStudentAsync(int studentId)
        {
            return await _dbSet
                .Include(r => r.Exam)
                .Include(r => r.Status)
                .Where(r => EF.Property<int>(r, "StudentId") == studentId)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
        }

      
        public async Task<ExamResult?> GetAsync(int examId, int studentId)
        {
            return await _dbSet
                .Include(r => r.Status)
                .FirstOrDefaultAsync(r =>
                    EF.Property<int>(r, "ExamId") == examId &&
                    EF.Property<int>(r, "StudentId") == studentId);
        }
    }

}
