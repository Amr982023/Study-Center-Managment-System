using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Repos;
using Domain.Models;
using Infrastructure.Repository.Generic;

namespace Infrastructure.Repository
{
    public class ExamStatusRepository : GenericRepository<ExamStatus>, IExamStatusRepository
    {
        public ExamStatusRepository(CenterDbContext context) : base(context)
        {
        }
    }
}
