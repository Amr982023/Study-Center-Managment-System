using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models;

namespace Application.ServicesInterfaces
{
    public interface IEnrollmentAppService
    {
        Task<Result<StudentGroupAggregation>> EnrollStudentAsync(int studentId,int groupId);
        Task<Result<bool>> DisenrollAsync(int studentId, int groupId);
        Task<Result<bool>> DisenrollAllAsync(int studentId);
    }

}
