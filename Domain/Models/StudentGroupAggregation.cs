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
        public Group Group { get; set; }
        public Student Student { get; set; }
        
    }
}
