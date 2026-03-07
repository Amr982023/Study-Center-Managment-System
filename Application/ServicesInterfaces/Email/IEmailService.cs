using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServicesInterfaces.Email
{
    public interface IEmailService
    {
        Task SendMessageAsync(string toEmail, string Message);
    }
}
