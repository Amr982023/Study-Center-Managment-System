using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class User : Person
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string Permission { get; private set; }
        public string HashedPassword { get; private set; }
        public string FullName => $"{FirstName} {LastName}";

        private User() { }

        private User(string first, string last, string phone, string gender, string? mid,
                     string userName, string email, string permission, string hashedPassword)
            : base(first, last, phone, gender, mid)
        {
            UserName = userName;
            Email = email;
            Permission = permission;
            HashedPassword = hashedPassword;
        }

        public static Result<User> Create(
            string firstName, string lastName, string phone, string gender,
            string userName, string email, string permission, string hashedPassword,
            string? midName = null)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result<User>.Failure("First name required.");
            if (string.IsNullOrWhiteSpace(userName))
                return Result<User>.Failure("Username required.");

            return Result<User>.Success(
                new User(firstName, lastName, phone, gender, midName,
                         userName, email, permission, hashedPassword));
        }
    }


}
