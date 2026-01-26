using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Grade : BaseEntity<int>
    {
        public string Name { get; private set; }

        private Grade() { }

        private Grade(string name) => Name = name;

        public static Result<Grade> Create(string name)
            => string.IsNullOrWhiteSpace(name)
                ? Result<Grade>.Failure("Grade name required")
                : Result<Grade>.Success(new Grade(name));
    }

}
