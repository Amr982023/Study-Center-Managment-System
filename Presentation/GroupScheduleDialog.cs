using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation
{
    public partial class GroupScheduleDialog : Form
    {
        private readonly IGroupScheduleService _scheduleService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int GroupId { get; set; }

        private StyledComboBox _cmbDay;
        private DateTimePicker _dtpTime;
        private RoundedButton _btnAdd;
        private DangerButton _btnDelete;
        private StyledDataGridView _grid;
        private Label _lblError;

        public GroupScheduleDialog(IGroupScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
            InitUI();
        }

        private void InitUI()
        {
            Text = "Group Schedule";
            Size = new Size(480, 420);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label
            {
                Text = "Manage Schedule",
                Font = AppTheme.FontH2,
                ForeColor = AppTheme.TextPrimary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(24, 18)
            };

            var l1 = MakeLabel("Day of Week", 24, 60);
            _cmbDay = new StyledComboBox { Width = 180, Location = new Point(24, 80) };
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                _cmbDay.Items.Add(day);
            _cmbDay.SelectedIndex = 0;

            var l2 = MakeLabel("Time", 220, 60);
            _dtpTime = new DateTimePicker
            {
                Width = 140,
                Location = new Point(220, 80),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true
            };

            _btnAdd = new RoundedButton
            {
                Text = "+ Add",
                Width = 90,
                Height = AppTheme.ButtonHeight,
                Location = new Point(374, 80)
            };
            _btnAdd.Click += async (s, e) => await AddScheduleAsync();

            var card = new CardPanel { Location = new Point(24, 134), Width = 420, Height = 200 };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Day", Name = "Day", FillWeight = 40 },
                new DataGridViewTextBoxColumn { HeaderText = "Time", Name = "Time", FillWeight = 40 }
            );
            card.Controls.Add(_grid);

            _btnDelete = new DangerButton
            {
                Text = "Remove Slot",
                Width = 120,
                Height = AppTheme.ButtonHeight,
                Location = new Point(24, 346),
                Enabled = false
            };
            _btnDelete.Click += async (s, e) => await DeleteAsync();
            _grid.SelectionChanged += (s, e) =>
                _btnDelete.Enabled = _grid.SelectedRows.Count > 0;

            _lblError = new Label
            {
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.Danger,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = 420,
                Height = 18,
                Location = new Point(24, 330)
            };

            Controls.AddRange(new Control[]
            {
                lblTitle, l1, _cmbDay, l2, _dtpTime, _btnAdd, card, _lblError, _btnDelete
            });

            Load += async (s, e) => await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            var r = await _scheduleService.GetByGroupAsync(GroupId);
            _grid.Rows.Clear();
            if (r.IsSuccess)
                foreach (var s in r.Value)
                    _grid.Rows.Add(s.Id, s.Day.ToString(), s.Time.ToString(@"hh\:mm"));
        }

        private async Task AddScheduleAsync()
        {
            _lblError.Text = "";
            if (_cmbDay.SelectedItem is not DayOfWeek day) return;
            var r = await _scheduleService.CreateAsync(GroupId, day, _dtpTime.Value.TimeOfDay);
            if (r.IsSuccess) await RefreshAsync();
            else _lblError.Text = r.ErrorMessage;
        }

        private async Task DeleteAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var id = (int)_grid.SelectedRows[0].Cells["Id"].Value;
            await _scheduleService.DeleteAsync(id);
            await RefreshAsync();
        }

        private static Label MakeLabel(string t, int x, int y) => new Label
        {
            Text = t,
            Font = AppTheme.FontLabelBold,
            ForeColor = AppTheme.TextSecondary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(x, y)
        };
    }
}

