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
        Task<bool> ExistsAsync(int studentId, int groupId);

        Task<IEnumerable<StudentGroupAggregation>> GetByStudentAsync(int studentId);

        Task<IEnumerable<StudentGroupAggregation>> GetByGroupAsync(int groupId);
    }

}
