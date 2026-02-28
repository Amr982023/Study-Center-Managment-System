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
            ReadOnly = false;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToResizeRows = false;
            Font = AppTheme.FontLabel;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ScrollBars = ScrollBars.Vertical;

            // Header
            ColumnHeadersDefaultCellStyle.BackColor = AppTheme.BlueDianneLight;
            ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.Tangerine;
            ColumnHeadersDefaultCellStyle.Font = AppTheme.FontLabelBold;
            ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
            ColumnHeadersDefaultCellStyle.SelectionBackColor = AppTheme.BlueDianneLight;
            ColumnHeadersHeight = 44;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            DefaultCellStyle.BackColor = AppTheme.CardBg;
            DefaultCellStyle.ForeColor = Color.White;
            DefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
            RowTemplate.Height = 46;

            DefaultCellStyle.SelectionBackColor = AppTheme.BlueDialneDark;
            DefaultCellStyle.SelectionForeColor = Color.White;

            AlternatingRowsDefaultCellStyle.BackColor = AppTheme.CardBg;
            AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
            AlternatingRowsDefaultCellStyle.SelectionBackColor = AppTheme.BlueDialneDark;
            AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
        }
    }

}
