using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IGroupScheduleService
    {
        Task<Result<GroupSchedule>> CreateAsync(int groupId, DayOfWeek day, TimeSpan time);
        Task<Result<GroupSchedule>> GetByIdAsync(int id);
        Task<Result<IEnumerable<GroupSchedule>>> GetByGroupAsync(int groupId);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
