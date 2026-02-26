using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class SectionLabel : Label
    {
        public SectionLabel()
        {
            Font = AppTheme.FontH2;
            ForeColor = Color.White;
            BackColor = Color.Transparent;
            AutoSize = true;
        }
    }
}
