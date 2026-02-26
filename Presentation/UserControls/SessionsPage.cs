#nullable disable
using System.Data;
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public class SessionsPage : UserControl
    {
        private readonly IClassSessionService _sessionService;
        private readonly IGroupService _groupService;

        private StyledDataGridView _grid;
        private StyledComboBox _cmbGroupFilter;
        private RoundedButton _btnAdd;
        private RoundedButton _btnEdit;
        private RoundedButton _btnRegister;
        private DangerButton _btnDelete;
        private Label _lblCount;

        public SessionsPage(IClassSessionService sessionService, IGroupService groupService, ISessionStatusService statusService)
        {
            _sessionService = sessionService;
            _groupService = groupService;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
            _ = LoadAsync();
        }

        private void BuildUI()
        {
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 8) };
            var lblTitle = new SectionLabel { Text = "Class Sessions", Location = new Point(0, 0) };
            _lblCount = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, BackColor = Color.Transparent, AutoSize = true, Location = new Point(0, 36) };
            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 60) };
            _cmbGroupFilter = new StyledComboBox { Width = 200, Location = new Point(0, 5) };
            _cmbGroupFilter.SelectedIndexChanged += async (s, e) => await LoadAsync();

            _btnAdd = new RoundedButton { Text = "+ Add Session", Width = 140, Height = AppTheme.ButtonHeight, Location = new Point(216, 5) };
            _btnAdd.Click += (s, e) => OpenDialog(null);

            toolbar.Controls.AddRange(new Control[] { _cmbGroupFilter, _btnAdd });

            var tableCard = new CardPanel { Dock = DockStyle.Fill };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "#", Name = "Num", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Group", Name = "Group", FillWeight = 22 },
                new DataGridViewTextBoxColumn { HeaderText = "Date & Time", Name = "Date", FillWeight = 25 },
                new DataGridViewTextBoxColumn { HeaderText = "Status", Name = "Status", FillWeight = 18 }
            );
            _grid.SelectionChanged += (s, e) => UpdateButtons();
            tableCard.Controls.Add(_grid);

            var actionPanel = new Panel { Height = 56, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };
            _btnEdit = new RoundedButton { Text = "✏️  Edit", Width = 110, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(0, 0) };
            _btnEdit.Click += (s, e) => OpenDialog(GetSelectedId());

            _btnRegister = new RoundedButton { Text = "📋  Attendance", Width = 140, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(120, 0), NormalColor = AppTheme.Success };
            _btnRegister.Click += (s, e) => OpenAttendanceDialog();

            _btnDelete = new DangerButton { Text = "🗑  Delete", Width = 110, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(270, 0) };
            _btnDelete.Click += async (s, e) => await DeleteAsync();

            actionPanel.Controls.AddRange(new Control[] { _btnEdit, _btnRegister, _btnDelete });

            headerPanel.Controls.AddRange(new Control[] { lblTitle, _lblCount, toolbar });
            // DOCK ORDER: add Fill last, Top second-to-last, Bottom first
            Controls.Add(tableCard);
            Controls.Add(headerPanel);
            Controls.Add(actionPanel);
        }

        private async Task LoadAsync()
        {
            if (_cmbGroupFilter.Items.Count == 0)
            {
                var gr = await _groupService.GetAllAsync();
                if (gr.IsSuccess)
                {
                    _cmbGroupFilter.Items.Add("All Groups");
                    foreach (var g in gr.Value) _cmbGroupFilter.Items.Add(g);
                    _cmbGroupFilter.DisplayMember = "Name";
                    _cmbGroupFilter.SelectedIndex = 0;
                }
            }

            var result = await _sessionService.GetAllAsync();
            if (!result.IsSuccess) return;

            var sessions = result.Value.AsEnumerable();
            if (_cmbGroupFilter.SelectedItem is Group grp)
                sessions = sessions.Where(s => s.Group?.Id == grp.Id);

            _grid.Rows.Clear();
            int cnt = 0;
            foreach (var s in sessions.OrderBy(x => x.SessionDateTime))
            {
                _grid.Rows.Add(s.Id, s.SessionNumber, s.Group?.Name ?? "-", s.SessionDateTime.ToString("MMM dd, yyyy  hh:mm tt"), s.Status?.Name ?? "-");
                cnt++;
            }
            _lblCount.Text = $"{cnt} session(s)";
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            bool sel = _grid.SelectedRows.Count > 0;
            _btnEdit.Enabled = sel;
            _btnDelete.Enabled = sel;
            _btnRegister.Enabled = sel;
        }

        private int? GetSelectedId() => _grid.SelectedRows.Count > 0 ? (int?)_grid.SelectedRows[0].Cells["Id"].Value : null;

        private void OpenDialog(int? id)
        {
            var dlg = Program.ServiceLocator.Resolve<SessionDialog>();
            dlg.SessionId = id;
            if (dlg.ShowDialog() == DialogResult.OK) _ = LoadAsync();
        }

        private void OpenAttendanceDialog()
        {
            if (GetSelectedId() is not int id) return;
            var dlg = Program.ServiceLocator.Resolve<AttendanceDialog>();
            dlg.SessionId = id;
            dlg.ShowDialog();
        }

        private async Task DeleteAsync()
        {
            if (GetSelectedId() is not int id) return;
            if (MessageBox.Show("Delete this session?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            var r = await _sessionService.DeleteAsync(id);
            if (r.IsSuccess) await LoadAsync();
            else MessageBox.Show(r.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
