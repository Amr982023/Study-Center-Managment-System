using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;
using System.ComponentModel;

namespace Presentation
{
    public class AttendanceDialog : Form
    {
        private readonly IStudentRegistrationService _regService;
        private readonly IStudentGroupAggregationService _enrollService;
        private readonly IClassSessionService _sessionService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SessionId { get; set; }

        private StyledDataGridView _grid;
        private Label _lblInfo;
        private Label _lblError;
        private RoundedButton _btnMark;
        private DangerButton _btnRemove;

        public AttendanceDialog(IStudentRegistrationService regService, IStudentGroupAggregationService enrollService, IClassSessionService sessionService)
        {
            _regService = regService;
            _enrollService = enrollService;
            _sessionService = sessionService;
            InitUI();
        }

        private void InitUI()
        {
            Text = "Session Attendance";
            Size = new Size(560, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label { Text = "Mark Attendance", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 18) };
            _lblInfo = new Label { Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 52) };

            var card = new CardPanel { Location = new Point(24, 80), Width = 500, Height = 300 };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "SId", FillWeight = 12 },
                new DataGridViewTextBoxColumn { HeaderText = "Code", Name = "Code", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Name", Name = "Name", FillWeight = 45 },
                new DataGridViewTextBoxColumn { HeaderText = "Attended", Name = "Present", FillWeight = 20 }
            );
            card.Controls.Add(_grid);

            _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 500, Height = 18, Location = new Point(24, 392) };

            _btnMark = new RoundedButton { Text = "✔ Mark Present", Width = 150, Height = AppTheme.ButtonHeight, Location = new Point(24, 414), Enabled = false };
            _btnMark.Click += async (s, e) => await MarkAsync();

            _btnRemove = new DangerButton { Text = "✘ Remove", Width = 110, Height = AppTheme.ButtonHeight, Location = new Point(184, 414), Enabled = false };

            _grid.SelectionChanged += (s, e) =>
            {
                _btnMark.Enabled = _grid.SelectedRows.Count > 0;
                _btnRemove.Enabled = _grid.SelectedRows.Count > 0;
            };

            Controls.AddRange(new Control[] { lblTitle, _lblInfo, card, _lblError, _btnMark, _btnRemove });
            Load += async (s, e) => await OnLoadAsync();
        }

        private async Task OnLoadAsync()
        {
            var session = await _sessionService.GetWithGroupAsync(SessionId);
            if (!session.IsSuccess) return;

            _lblInfo.Text = $"Session #{session.Value.SessionNumber} — {session.Value.SessionDateTime:MMM dd, yyyy  hh:mm tt}  |  Group: {session.Value.Group?.Name}";

            var enrolled = await _enrollService.GetByGroupAsync(session.Value.Group.Id);
            _grid.Rows.Clear();
            if (enrolled.IsSuccess)
            {
                foreach (var e in enrolled.Value)
                {
                    var exists = await _regService.ExistsAsync(e.StudentId, SessionId);
                    bool present = exists.IsSuccess && exists.Value;
                    _grid.Rows.Add(e.StudentId, e.Student?.Code, $"{e.Student?.FirstName} {e.Student?.LastName}", present ? "✔ Yes" : "—");
                }
            }
        }

        private async Task MarkAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var sid = (int)_grid.SelectedRows[0].Cells["SId"].Value;
            var r = await _regService.RegisterAsync(sid, SessionId);
            if (r.IsSuccess) await OnLoadAsync();
            else _lblError.Text = r.ErrorMessage;
        }
    }
}

