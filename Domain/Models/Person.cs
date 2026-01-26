using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class Person : BaseEntity<int>
    {
        public required string FirstName { get;  set; }
        public string? MidName { get; private set; }
        public string LastName { get; private set; }
        public required string PersonalPhone { get;  set; }
        public string Gender { get; private set; }

        private Person(string first, string last, string phone, string gender, string? mid)
        {
           
            FirstName = first;
            LastName = last;
            PersonalPhone = phone;
            Gender = gender;
            MidName = mid;
        }

    }

}
