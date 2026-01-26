using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces.Generic;
using Domain.Models;

namespace Domain.Interfaces.Repos
{
    public interface IPerson : IGeneric<Person>
    {
        Task<Person?> GetByPhoneAsync(string phone);

        Task<bool> ExistsByPhoneAsync(string phone);
    }

}
