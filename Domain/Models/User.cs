using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class User : BaseEntity<int>
    {
        public Person Person { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string Permission { get; private set; }
        public string HashedPassword { get; private set; }

        private User() { }

        private User(Person person, string userName, string permission, string hashedPassword)
        {
            Person = person;
            UserName = userName;
            Permission = permission;
            HashedPassword = hashedPassword;
        }

        public static Result<User> Create(
            Person person,
            string userName,
            string permission,
            string hashedPassword)
        {
            if (person == null)
                return Result<User>.Failure("Person required");

            if (string.IsNullOrWhiteSpace(userName))
                return Result<User>.Failure("Username required");

            return Result<User>.Success(
                new User(person, userName, permission, hashedPassword));
        }
    }


}
