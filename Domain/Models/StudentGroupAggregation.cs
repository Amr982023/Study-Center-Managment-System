using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class StudentGroupAggregation : BaseEntity<int>
    {
        public int StudentId { get; private set; }
        public Student Student { get; private set; }

        public int GroupId { get; private set; }
        public Group Group { get; private set; }

        private StudentGroupAggregation() { }

        private StudentGroupAggregation(int studentId, int groupId)
        {
            StudentId = studentId;
            GroupId = groupId;
        }

        public static Result<StudentGroupAggregation> Create(
            Student student,
            Group group)
        {
            if (student == null || group == null)
                return Result<StudentGroupAggregation>
                    .Failure("Invalid student or group");

            return Result<StudentGroupAggregation>.Success(
                new StudentGroupAggregation(
                    student.Id,
                    group.Id));
        }
    }

}
