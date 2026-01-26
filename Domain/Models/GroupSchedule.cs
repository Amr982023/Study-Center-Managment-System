using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;

namespace Domain.Models
{
    public class GroupSchedule : BaseEntity<int>
    {
        public Group Group { get; private set; }
        public DayOfWeek Day { get; private set; }
        public TimeSpan Time { get; private set; }

        private GroupSchedule() { } // ORM

        private GroupSchedule(Group group, DayOfWeek day, TimeSpan time)
        {
            Group = group;
            Day = day;
            Time = time;
        }

        public static Result<GroupSchedule> Create(
            Group group,
            DayOfWeek day,
            TimeSpan time)
        {
            if (group == null)
                return Result<GroupSchedule>.Failure("Group is required");

            return Result<GroupSchedule>.Success(
                new GroupSchedule(group, day, time));
        }
    }

}
