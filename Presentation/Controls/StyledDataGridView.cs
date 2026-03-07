using System;
using System.Windows.Forms;
using System.Drawing;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class StyledDataGridView : DataGridView
    {
        public StyledDataGridView()
        {
            // Enable double buffering via reflection — WinForms blocks direct access
            typeof(DataGridView).GetProperty("DoubleBuffered",
    System.Reflection.BindingFlags.NonPublic |
    System.Reflection.BindingFlags.Instance)
    ?.SetValue(this, true);

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

            ColumnHeadersDefaultCellStyle.BackColor = AppTheme.BlueDialneDark;
            ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.Tangerine;
            ColumnHeadersDefaultCellStyle.Font = AppTheme.FontLabelBold;
            ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
            ColumnHeadersDefaultCellStyle.SelectionBackColor = AppTheme.BlueDialneDark;
            ColumnHeadersHeight = 44;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            DefaultCellStyle.BackColor = AppTheme.CardBg;
            DefaultCellStyle.ForeColor = Color.White;
            DefaultCellStyle.SelectionBackColor = AppTheme.Tangerine;
            DefaultCellStyle.SelectionForeColor = Color.White;
            DefaultCellStyle.Padding = new Padding(12, 0, 0, 0);
            RowTemplate.Height = 46;

            AlternatingRowsDefaultCellStyle.BackColor = AppTheme.TableRowAlt;
            AlternatingRowsDefaultCellStyle.ForeColor = Color.White;
            AlternatingRowsDefaultCellStyle.SelectionBackColor = AppTheme.Tangerine;
            AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
        }
    }
}