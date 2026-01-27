using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IStudentGroupAggregationRepository
     : IGeneric<StudentGroupAggregation>
    {
        Task<bool> ExistsWithSameGroupAsync(int studentId, int groupId);
        Task<bool> HasAnyEnrollmentAsync(int studentId);

        Task<bool> ExistsWithSameSubjectAsync(int studentId, int subjectId);

        Task<IEnumerable<StudentGroupAggregation>> GetByStudentAsync(int studentId);

        Task<IEnumerable<StudentGroupAggregation>> GetByGroupAsync(int groupId);

        Task RemoveAsync(int studentId, int groupId);

        Task RemoveAllAsync(int studentId);
    }

}
