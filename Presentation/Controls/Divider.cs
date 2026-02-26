using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class Divider : Panel
    {
        public Divider()
        {
            Height = 1;
            BackColor = AppTheme.Border;
            Dock = DockStyle.Top;
        }
    }

}
