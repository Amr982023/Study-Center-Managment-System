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
    public class SubjectGradeHandlerRepository
     : GenericRepository<SubjectGradeHandler>, ISubjectGradeHandlerRepository
    {
        public SubjectGradeHandlerRepository(CenterDbContext context)
            : base(context)
        {
        }

        public override async Task<IEnumerable<SubjectGradeHandler>> GetAllAsync()
        {
            return await _dbSet
                .Include(sgh => sgh.Subject)
                .Include(sgh => sgh.Grade)
                .ToListAsync();
        }

        public async Task<SubjectGradeHandler?> GetAsync(int subjectId, int gradeId)
        {
            return await _dbSet
                .Include(sgh => sgh.Subject)
                .Include(sgh => sgh.Grade)
                .FirstOrDefaultAsync(sgh =>
                    EF.Property<int>(sgh, "SubjectId") == subjectId &&
                    EF.Property<int>(sgh, "GradeId") == gradeId);
        }
    }

}
