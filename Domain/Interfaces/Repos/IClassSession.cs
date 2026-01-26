using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IClassSession : IGeneric<ClassSession>
    {
        Task<IEnumerable<ClassSession>> GetUpcomingAsync(DateTime from);

        Task<ClassSession?> GetWithGroupAsync(int sessionId);
    }

}
