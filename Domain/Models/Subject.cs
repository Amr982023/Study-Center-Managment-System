using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Subject : BaseEntity<int>
    {
        public string Name { get; private set; }

        private Subject() { }
        private Subject(string name) => Name = name;

        public static Result<Subject> Create(string name)
            => string.IsNullOrWhiteSpace(name)
                ? Result<Subject>.Failure("Subject name required")
                : Result<Subject>.Success(new Subject(name));
    }

}
