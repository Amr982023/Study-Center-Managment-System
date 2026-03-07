using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Settings
{
    public class EmailSettings
    {
        public string FromAddress { get; set; }
        public string DisplayName { get; set; }
        public string AppPassword { get; set; }
        public string Subject { get; set; }
    }
}
