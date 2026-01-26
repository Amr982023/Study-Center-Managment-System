using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class ExamStatus : BaseEntity<int>
    {
        public string Name { get; private set; }

        private ExamStatus() { }

        private ExamStatus(string name)
        {
            Name = name;
        }

        public static Result<ExamStatus> Create(string name)
            => string.IsNullOrWhiteSpace(name)
                ? Result<ExamStatus>.Failure("Exam status name is required")
                : Result<ExamStatus>.Success(new ExamStatus(name));
    }

}
