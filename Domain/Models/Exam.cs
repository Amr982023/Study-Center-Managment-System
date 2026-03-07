using System;
using Domain.Common;

namespace Domain.Models
{
    public class Exam : BaseEntity<int>
    {
        public Group Group { get; private set; }
        public string Name { get; private set; }
        public int FullMark { get; private set; }
        public DateTime DateTime { get; private set; }   // creation timestamp
        public DateTime ExamDate { get; private set; }   // scheduled exam date/time

        private Exam() { }

        private Exam(Group group, string name, int fullMark, DateTime examDate)
        {
            Group = group;
            Name = name;
            FullMark = fullMark;
            DateTime = DateTime.UtcNow;
            ExamDate = examDate;
        }

        public static Result<Exam> Create(Group group, string name, int fullMark, DateTime examDate)
        {
            if (fullMark <= 0)
                return Result<Exam>.Failure("Invalid full mark.");
            if (examDate < DateTime.UtcNow.Date)
                return Result<Exam>.Failure("Exam date cannot be in the past.");

            return Result<Exam>.Success(new Exam(group, name, fullMark, examDate));
        }
    }
}