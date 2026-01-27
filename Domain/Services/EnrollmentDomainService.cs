using System;
using System.Collections;
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
        public Result<StudentGroupAggregation> Enroll(Student student, Group groupToJoin, bool canJoin,
            bool alreadyEnrolledSameGroup, bool alreadyEnrolledSameSubject)
        {
            if (!canJoin)
                return Result<StudentGroupAggregation>
                    .Failure("Student grade does not match group grade");

            if (alreadyEnrolledSameGroup)
                return Result<StudentGroupAggregation>
                    .Failure("Student already enrolled in this group");

            if (alreadyEnrolledSameSubject)
                return Result<StudentGroupAggregation>
                    .Failure("Student already enrolled in a group with the same subject");

            return StudentGroupAggregation.Create(
                student,
                groupToJoin);
        }

        public Result<bool> Disenroll(bool isEnrolled)
        {
            if (!isEnrolled)
                return Result<bool>.Failure("Student is not enrolled in this group");

            return Result<bool>.Success(true);
        }

        public Result<bool> DisenrollAll(bool hasAnyEnrollment)
        {
            if (!hasAnyEnrollment)
                return Result<bool>.Failure("Student has no enrollments");

            return Result<bool>.Success(true);

        }
    }
}
