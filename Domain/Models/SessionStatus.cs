using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class SessionStatus : BaseEntity<int>
    {
        public string Name { get; private set; }

        private SessionStatus() { }

        private SessionStatus(string name)
        {
            Name = name;
        }

        public static Result<SessionStatus> Create(string name)
            => string.IsNullOrWhiteSpace(name)
                ? Result<SessionStatus>.Failure("Session status name is required")
                : Result<SessionStatus>.Success(new SessionStatus(name));
    }

}
