using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Payment : BaseEntity<int>
    {
        public Student Student { get; private set; }
        public User PerformedBy { get; private set; }
        public decimal Amount { get; private set; }
        public int Month { get; private set; }
        public DateTime DateTime { get; private set; }

        private Payment() { }

        private Payment(Student student, User user, decimal amount, int month)
        {
            Student = student;
            PerformedBy = user;
            Amount = amount;
            Month = month;
            DateTime = DateTime.UtcNow;
        }

        public static Result<Payment> Create(
            Student student,
            User user,
            decimal amount,
            int month)
        {
            if (amount <= 0)
                return Result<Payment>.Failure("Invalid amount");

            return Result<Payment>.Success(
                new Payment(student, user, amount, month));
        }
    }

}
