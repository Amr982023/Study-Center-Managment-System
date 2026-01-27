using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IGroup : IGeneric<Group>
    {
        Task<Group?> GetWithSessionsAsync(int groupId);
        Task<Group?> GetWithSubjectGradeDetailsAsync(int groupId);

        Task<IEnumerable<Group>> GetByGradeAsync(int gradeId);

        Task<IEnumerable<Group>> GetByStudentWithDetailsAsync(int studentId);
    }

}
