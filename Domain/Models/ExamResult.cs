using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class ExamResult : BaseEntity<int>
    {
        public Exam Exam { get; private set; }
        public Student Student { get; private set; }
        public ExamStatus Status { get; private set; }
        public int Result { get; private set; }
        public bool ExceptFullMark { get; private set; }

        private ExamResult() { } // ORM

        private ExamResult(
            Exam exam,
            Student student,
            ExamStatus status,
            int result,
            bool exceptFullMark)
        {
            Exam = exam;
            Student = student;
            Status = status;
            Result = result;
            ExceptFullMark = exceptFullMark;
        }

        public static Result<ExamResult> Create(
            Exam exam,
            Student student,
            ExamStatus status,
            int result,
            bool exceptFullMark)
        {
            if (exam == null || student == null || status == null)
                return Result<ExamResult>.Failure("Invalid exam result data");

            if (result < 0)
                return Result<ExamResult>.Failure("Result cannot be negative");

            return Result<ExamResult>.Success(
                new ExamResult(exam, student, status, result, exceptFullMark));
        }
    }

}
