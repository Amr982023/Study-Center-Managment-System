using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class StyledComboBox : ComboBox
    {
        public StyledComboBox()
        {
            FlatStyle = FlatStyle.Flat;
            BackColor = AppTheme.InputBg;
            ForeColor = Color.White;
            Font = AppTheme.FontInput;
            Height = AppTheme.InputHeight;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }

}
