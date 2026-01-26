using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class StudentRegistration : BaseEntity<int>
    {
        public Student Student { get; private set; }
        public ClassSession ClassSession { get; private set; }
        public DateTime RegistrationTime { get; private set; }

        private StudentRegistration() { }

        private StudentRegistration(Student student, ClassSession session)
        {
            Student = student;
            ClassSession = session;
            RegistrationTime = DateTime.UtcNow;
        }

        public static Result<StudentRegistration> Create(
            Student student,
            ClassSession session)
            => Result<StudentRegistration>.Success(
                new StudentRegistration(student, session));
    }

}
