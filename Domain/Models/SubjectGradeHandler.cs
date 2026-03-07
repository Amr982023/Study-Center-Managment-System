using System;
using Domain.Common;

namespace Domain.Models
{
    public class SubjectGradeHandler : BaseEntity<int>
    {
        public int SubjectId { get; private set; }
        public Subject Subject { get; private set; }

        public int GradeId { get; private set; }
        public Grade Grade { get; private set; }

        public decimal SessionFees { get; private set; }

        private SubjectGradeHandler() { }

        private SubjectGradeHandler(Subject subject, Grade grade, decimal fees)
        {
            Subject = subject;
            SubjectId = subject.Id;

            Grade = grade;
            GradeId = grade.Id;

            SessionFees = fees;
        }

        public static Result<SubjectGradeHandler> Create(
            Subject subject,
            Grade grade,
            decimal fees)
        {
            if (subject == null)
                return Result<SubjectGradeHandler>.Failure("Subject required");

            if (grade == null)
                return Result<SubjectGradeHandler>.Failure("Grade required");

            if (fees <= 0)
                return Result<SubjectGradeHandler>.Failure("Invalid fees");

            return Result<SubjectGradeHandler>.Success(
                new SubjectGradeHandler(subject, grade, fees));
        }
    }
}