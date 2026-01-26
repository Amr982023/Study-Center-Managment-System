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
    public class ExamRepository
     : GenericRepository<Exam>, IExam
    {
        public ExamRepository(CenterDbContext context)
            : base(context)
        {
        }

        
     
    
        public async Task<IEnumerable<Exam>> GetByGroupAsync(int groupId)
        {
            return await _dbSet
                .Where(e => EF.Property<int>(e, "GroupId") == groupId)
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }
    }

}
