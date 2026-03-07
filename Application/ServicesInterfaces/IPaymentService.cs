using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IPaymentService
    {
        Task<Result<Payment>> CreateAsync(int studentId, int performedByUserId, decimal amount, int month);
        Task<Result<Payment>> GetByIdAsync(int id);
        Task<Result<IEnumerable<Payment>>> GetByStudentAsync(int studentId);
        Task<Result<decimal>> GetTotalPaidAsync(int studentId, int month);
        Task<Result<bool>> DeleteAsync(int id);

        /// <summary>Returns all students who have NO payment recorded for the given month.</summary>
        Task<Result<IEnumerable<Student>>> GetUnpaidStudentsAsync(int month);
    }
}