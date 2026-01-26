using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Exam : BaseEntity<int>
    {
        public Group Group { get; private set; }
        public string Name { get; private set; }
        public int FullMark { get; private set; }
        public DateTime DateTime { get; private set; }

        private Exam() { }

        private Exam(Group group, string name, int fullMark)
        {
            Group = group;
            Name = name;
            FullMark = fullMark;
            DateTime = DateTime.UtcNow;
        }

        public static Result<Exam> Create(Group group, string name, int fullMark)
        {
            if (fullMark <= 0)
                return Result<Exam>.Failure("Invalid full mark");

            return Result<Exam>.Success(
                new Exam(group, name, fullMark));
        }
    }

}
