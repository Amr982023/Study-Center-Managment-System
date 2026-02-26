using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Student : Person
    {
        public string Code { get; private set; }
        public string GuardianPhone { get; private set; }
        public string FullName => $"{FirstName} {LastName}";
        public Grade Grade { get; private set; }
        private readonly List<StudentRegistration> _registrations = new();
        public IReadOnlyCollection<StudentRegistration> Registrations => _registrations;

        private Student() { }

        private Student(string first, string last, string phone, string gender, string? mid,
                        string code, string guardianPhone, Grade grade)
            : base(first, last, phone, gender, mid)
        {
            Code = code;
            GuardianPhone = guardianPhone;
            Grade = grade;
        }

        public static Result<Student> Create(
            string firstName, string lastName, string phone, string gender,
            string code, string guardianPhone, Grade grade, string? midName = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result<Student>.Failure("First name required.");
            if (string.IsNullOrWhiteSpace(code))
                return Result<Student>.Failure("Code required.");

            return Result<Student>.Success(
                new Student(firstName, lastName, phone, gender, midName, code, guardianPhone, grade));
        }

        public Result<StudentRegistration> Register(ClassSession session)
        {
            if (_registrations.Any(r => r.ClassSession == session))
                return Result<StudentRegistration>.Failure("Already registered.");
            var reg = StudentRegistration.Create(this, session);
            _registrations.Add(reg.Value!);
            return reg;
        }
    }

}
