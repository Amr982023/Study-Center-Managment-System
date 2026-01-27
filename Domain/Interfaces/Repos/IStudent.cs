using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IStudent : IGeneric<Student>
    {
        Task<Student?> GetByCodeAsync(string code);

        Task<IEnumerable<Student>> GetByGradeAsync(int gradeId);

        Task<bool> ExistsByCodeAsync(string code);

        Task<bool> CanJoinAsync(int studentId, int groupId);

        Task<Student?> GetWithRegistrationsAsync(int studentId);

        Task<Student?> GetWithGradeAsync(int studentId);
    }

}
