using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation.Theme;

namespace Presentation.Controls
{

        public class StyledDataGridView : DataGridView
        {
            public StyledDataGridView()
            {
                BackgroundColor = AppTheme.CardBg;
                BorderStyle = BorderStyle.None;
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                EnableHeadersVisualStyles = false;
                GridColor = AppTheme.Border;
                RowHeadersVisible = false;
                SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                MultiSelect = false;
                ReadOnly = true;
                AllowUserToAddRows = false;
                AllowUserToDeleteRows = false;
                AllowUserToResizeRows = false;
                Font = AppTheme.FontLabel;
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                ScrollBars = ScrollBars.Vertical;

                // Header — tangerine accent
                ColumnHeadersDefaultCellStyle.BackColor = AppTheme.BlueDialneDark;
                ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.Tangerine;
                ColumnHeadersDefaultCellStyle.Font = AppTheme.FontLabelBold;
                ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
                ColumnHeadersDefaultCellStyle.SelectionBackColor = AppTheme.BlueDialneDark;
                ColumnHeadersHeight = 44;
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                // Rows
                DefaultCellStyle.BackColor = AppTheme.CardBg;
                DefaultCellStyle.ForeColor = Color.White;
                DefaultCellStyle.SelectionBackColor = AppTheme.NavActiveBg;
                DefaultCellStyle.SelectionForeColor = Color.White;
                DefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
                RowTemplate.Height = 46;

                AlternatingRowsDefaultCellStyle.BackColor = AppTheme.TableRowAlt;
                AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
                AlternatingRowsDefaultCellStyle.SelectionBackColor = AppTheme.NavActiveBg;
                AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
            }
        }
    
}
