using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class ClassSession : BaseEntity<int>
    {
        public int SessionNumber { get; private set; }
        public Group Group { get; private set; }
        public DateTime SessionDateTime { get; private set; }
        public SessionStatus Status { get; private set; }

        private ClassSession() { }

        private ClassSession(Group group, int number, DateTime date, SessionStatus status)
        {
            Group = group;
            SessionNumber = number;
            SessionDateTime = date;
            Status = status;
        }

        public static Result<ClassSession> Create(Group group, int number, DateTime date, SessionStatus status)
        {
            if (date < DateTime.Now)
                return Result<ClassSession>.Failure("Session date invalid");

            return Result<ClassSession>.Success(
                new ClassSession(group, number, date, status));
        }
    }

}
