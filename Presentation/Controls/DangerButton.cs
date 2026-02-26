using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation.Theme;

namespace Presentation.Controls
{

    public class DangerButton : RoundedButton
    {
        public DangerButton()
        {
            NormalColor = AppTheme.Danger;
            HoverColor = Color.FromArgb(255, 80, 80);
            BackColor = AppTheme.Danger;
        }
    }

}
