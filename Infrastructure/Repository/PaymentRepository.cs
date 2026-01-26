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
    public class PaymentRepository
    : GenericRepository<Payment>, IPayment
    {
        public PaymentRepository(CenterDbContext context)
            : base(context)
        {
        }

        
        public async Task<decimal> GetTotalPaidAsync(int studentId, int month)
        {
            return await _dbSet
                .Where(p =>
                    EF.Property<int>(p, "StudentId") == studentId &&
                    p.Month == month)
                .SumAsync(p => p.Amount);
        }

        
        public async Task<IEnumerable<Payment>> GetByStudentAsync(int studentId)
        {
            return await _dbSet
                .Where(p => EF.Property<int>(p, "StudentId") == studentId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }

}
