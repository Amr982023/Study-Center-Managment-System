using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces.UOW;
using Domain.Models;

namespace Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;

        public PaymentService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<Payment>> CreateAsync(
            int studentId, int performedByUserId, decimal amount, int month)
        {
            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<Payment>.Failure("Student not found.");

            var user = await _uow.Users.GetByIdAsync(performedByUserId);
            if (user is null)
                return Result<Payment>.Failure("User (cashier) not found.");

            if (month < 1 || month > 12)
                return Result<Payment>.Failure("Invalid month value.");

            var result = Payment.Create(student, user, amount, month);
            if (!result.IsSuccess)
                return Result<Payment>.Failure(result.ErrorMessage!);

            await _uow.Payments.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<Payment>.Success(result.Value!);
        }

        public async Task<Result<Payment>> GetByIdAsync(int id)
        {
            var payment = await _uow.Payments.GetByIdAsync(id);
            return payment is null
                ? Result<Payment>.Failure("Payment not found.")
                : Result<Payment>.Success(payment);
        }

        public async Task<Result<IEnumerable<Payment>>> GetByStudentAsync(int studentId)
        {
            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<IEnumerable<Payment>>.Failure("Student not found.");

            var payments = await _uow.Payments.GetByStudentAsync(studentId);
            return Result<IEnumerable<Payment>>.Success(payments);
        }

        public async Task<Result<decimal>> GetTotalPaidAsync(int studentId, int month)
        {
            var student = await _uow.Students.GetByIdAsync(studentId);
            if (student is null)
                return Result<decimal>.Failure("Student not found.");

            var total = await _uow.Payments.GetTotalPaidAsync(studentId, month);
            return Result<decimal>.Success(total);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var payment = await _uow.Payments.GetByIdAsync(id);
            if (payment is null)
                return Result<bool>.Failure("Payment not found.");

            await _uow.Payments.DeleteAsync(payment);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
