using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Student : BaseEntity<int>
    {
        public Person Person { get; private set; }
        public string Code { get; private set; }
        public string GuardianPhone { get; private set; }
        public Grade Grade { get; private set; }


        private readonly List<StudentRegistration> _registrations = new();
        public IReadOnlyCollection<StudentRegistration> Registrations => _registrations;

        private Student() { }

        private Student(Person person, string code, string guardianPhone, Grade grade)
        {
            Person = person;
            Code = code;
            GuardianPhone = guardianPhone;
            Grade = grade;
        }

        public static Result<Student> Create(
            Person person,
            string code,
            string guardianPhone,
            Grade grade)
        {
            if (person == null)
                return Result<Student>.Failure("Person required");

            if (string.IsNullOrWhiteSpace(code))
                return Result<Student>.Failure("Code required");

            return Result<Student>.Success(
                new Student(person, code, guardianPhone, grade));
        }

        public Result<StudentRegistration> Register(ClassSession session)
        {
            if (_registrations.Any(r => r.ClassSession == session))
                return Result<StudentRegistration>.Failure("Already registered");

            var reg = StudentRegistration.Create(this, session);
            _registrations.Add(reg.Value);
            return reg;
        }

        public bool CanJoin(Group group)
        => group.SubjectGrade.Grade.Id == Grade.Id;

        public Result<StudentGroupAggregation> Enroll(Group GroupToJoin, IEnumerable<StudentGroupAggregation> GroupsOfStudent)
        {
            if (GroupsOfStudent.Any(SGA => SGA.Group.Id == GroupToJoin.Id && SGA.Student.Id == this.Id))
            {
                return Result<StudentGroupAggregation>.Failure("Student already enrolled in this group");

            }
            else
            {
                var SubjectID = GroupToJoin.SubjectGrade.Subject.Id;

                foreach (var group in GroupsOfStudent)
                {
                    if (group.Group.SubjectGrade.Subject.Id == SubjectID)
                    {
                        return Result<StudentGroupAggregation>.Failure("Student already enrolled in a group with the same subject");
                    }
                }


                StudentGroupAggregation studentGroupAggregation = new StudentGroupAggregation { Student = this, Group = GroupToJoin };

                return Result<StudentGroupAggregation>.Success(studentGroupAggregation);
            }
        }
    }

}
