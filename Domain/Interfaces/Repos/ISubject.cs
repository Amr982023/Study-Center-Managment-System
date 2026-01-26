using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface ISubjectRepository : IGeneric<Subject>
    {
        Task<Subject?> GetByNameAsync(string name);
    }

}
