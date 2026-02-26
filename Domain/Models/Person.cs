using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public abstract class Person : BaseEntity<int>
    {
        public string FirstName { get; protected set; }
        public string? MidName { get; protected set; }
        public string LastName { get; protected set; }
        public string PersonalPhone { get; protected set; }
        public string Gender { get; protected set; }

        protected Person() { } // For ORM

        protected Person(string first, string last, string phone, string gender, string? mid)
        {
            FirstName = first;
            LastName = last;
            PersonalPhone = phone;
            Gender = gender;
            MidName = mid;
        }
    }

}
