using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IStudentGroupAggregationService
    {
        Task<Result<StudentGroupAggregation>> EnrollAsync(int studentId, int groupId);
        Task<Result<IEnumerable<StudentGroupAggregation>>> GetByStudentAsync(int studentId);
        Task<Result<IEnumerable<StudentGroupAggregation>>> GetByGroupAsync(int groupId);
        Task<Result<bool>> UnenrollAsync(int studentId, int groupId);
        Task<Result<bool>> UnenrollAllAsync(int studentId);
    }
}
