using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Interfaces.UOW;
using Domain.Models;

namespace Application.Services
{
    public class GroupScheduleService : IGroupScheduleService
    {
        private readonly IUnitOfWork _uow;

        public GroupScheduleService(IUnitOfWork uow) => _uow = uow;

        public async Task<Result<GroupSchedule>> CreateAsync(int groupId, DayOfWeek day, TimeSpan time)
        {
            var group = await _uow.Groups.GetByIdAsync(groupId);
            if (group is null)
                return Result<GroupSchedule>.Failure("Group not found.");

            // Prevent duplicate day schedule for same group
            var existing = await _uow.GroupSchedules.GetByGroupAsync(groupId);
            if (existing.Any(s => s.Day == day))
                return Result<GroupSchedule>.Failure($"Group already has a schedule on {day}.");

            var result = GroupSchedule.Create(group, day, time);
            if (!result.IsSuccess)
                return Result<GroupSchedule>.Failure(result.ErrorMessage!);

            await _uow.GroupSchedules.AddAsync(result.Value!);
            await _uow.SaveChangesAsync();
            return Result<GroupSchedule>.Success(result.Value!);
        }

        public async Task<Result<GroupSchedule>> GetByIdAsync(int id)
        {
            var schedule = await _uow.GroupSchedules.GetByIdAsync(id);
            return schedule is null
                ? Result<GroupSchedule>.Failure("Schedule not found.")
                : Result<GroupSchedule>.Success(schedule);
        }

        public async Task<Result<IEnumerable<GroupSchedule>>> GetByGroupAsync(int groupId)
        {
            var group = await _uow.Groups.GetByIdAsync(groupId);
            if (group is null)
                return Result<IEnumerable<GroupSchedule>>.Failure("Group not found.");

            var schedules = await _uow.GroupSchedules.GetByGroupAsync(groupId);
            return Result<IEnumerable<GroupSchedule>>.Success(schedules);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var schedule = await _uow.GroupSchedules.GetByIdAsync(id);
            if (schedule is null)
                return Result<bool>.Failure("Schedule not found.");

            await _uow.GroupSchedules.DeleteAsync(schedule);
            await _uow.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
