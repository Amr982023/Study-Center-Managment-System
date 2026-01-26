using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Domain.Services
{
    public class EnrollmentDomainService
    {
        public Result<StudentGroupAggregation> Enroll(
            Student student,
            Group groupToJoin,
            IEnumerable<Group> currentStudentGroups)
        {
            if (currentStudentGroups.Any(g => g.Id == groupToJoin.Id))
                return Result<StudentGroupAggregation>
                    .Failure("Student already enrolled in this group");

            if (currentStudentGroups.Any(g =>
                g.SubjectGrade.Subject.Id ==
                groupToJoin.SubjectGrade.Subject.Id))
                return Result<StudentGroupAggregation>
                    .Failure("Student already enrolled in a group with the same subject");

            var sga =  new StudentGroupAggregation
            {
                Student = student,
                Group = groupToJoin
            };

            return Result<StudentGroupAggregation>.Success(sga);
        }
    }
}
