using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Group : BaseEntity<int>
    {
        public string Name { get; private set; }
        public int SubjectGradeHandlerId { get; private set; }
        public SubjectGradeHandler SubjectGrade { get; private set; }
        public DateTime FirstSessionDate { get; private set; }

        private readonly List<ClassSession> _sessions = new();
        public IReadOnlyCollection<ClassSession> Sessions => _sessions;

        private Group() { }

        private Group(string name, SubjectGradeHandler handler, DateTime date)
        {
            Name = name;
            SubjectGrade = handler;
            FirstSessionDate = date;
        }

        public static Result<Group> Create(
            string name,
            SubjectGradeHandler handler,
            DateTime firstSession)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Group>.Failure("Group name required");

            return Result<Group>.Success(
                new Group(name, handler, firstSession));
        }
    }

}
