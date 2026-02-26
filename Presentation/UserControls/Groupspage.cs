#nullable disable
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
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public class GroupsPage : UserControl
    {
        private readonly IGroupService _groupService;
        private readonly ISubjectGradeHandlerService _handlerService;
        private readonly IStudentGroupAggregationService _enrollService;

        private StyledDataGridView _grid;
        private RoundedButton _btnAdd;
        private RoundedButton _btnEdit;
        private RoundedButton _btnEnroll;
        private RoundedButton _btnSchedule;
        private DangerButton _btnDelete;
        private StyledTextBox _txtSearch;
        private Label _lblCount;

        public GroupsPage(
            IGroupService groupService,
            ISubjectGradeHandlerService handlerService,
            IStudentGroupAggregationService enrollService)
        {
            _groupService = groupService;
            _handlerService = handlerService;
            _enrollService = enrollService;

            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
            _ = LoadAsync();
        }

        private void BuildUI()
        {
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 8) };
            var lblTitle = new SectionLabel { Text = "Groups", Location = new Point(0, 0) };
            _lblCount = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, BackColor = Color.Transparent, AutoSize = true, Location = new Point(0, 36) };
            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 60) };

            _txtSearch = new StyledTextBox
            {
                Width = 260,
                Height = AppTheme.InputHeight,
                Placeholder = "🔍  Search groups...",
                Location = new Point(0, 5)
            };
            _txtSearch.Inner.TextChanged += async (s, e) => await ApplyFilterAsync();

            _btnAdd = new RoundedButton
            {
                Text = "+ Add Group",
                Width = 130,
                Height = AppTheme.ButtonHeight,
                Location = new Point(275, 5)
            };
            _btnAdd.Click += (s, e) => OpenGroupDialog(null);

            toolbar.Controls.AddRange(new Control[] { _txtSearch, _btnAdd });

            var tableCard = new CardPanel { Dock = DockStyle.Fill };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Group Name", Name = "Name", FillWeight = 22 },
                new DataGridViewTextBoxColumn { HeaderText = "Subject", Name = "Subject", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Grade", Name = "Grade", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Fees/Session", Name = "Fees", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "First Session", Name = "First", FillWeight = 20 }
            );
            _grid.SelectionChanged += (s, e) => UpdateButtons();
            tableCard.Controls.Add(_grid);

            var actionPanel = new Panel { Height = 56, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };

            _btnEdit = new RoundedButton
            {
                Text = "✏️  Edit",
                Width = 110,
                Height = AppTheme.ButtonHeight,
                Enabled = false,
                Location = new Point(0, 0)
            };
            _btnEdit.Click += (s, e) => OpenGroupDialog(GetSelectedId());

            _btnEnroll = new RoundedButton
            {
                Text = "👨‍🎓  Enroll",
                Width = 120,
                Height = AppTheme.ButtonHeight,
                Enabled = false,
                Location = new Point(120, 0),
                NormalColor = AppTheme.Info
            };
            _btnEnroll.Click += (s, e) => OpenEnrollDialog();

            _btnSchedule = new RoundedButton
            {
                Text = "📅  Schedule",
                Width = 130,
                Height = AppTheme.ButtonHeight,
                Enabled = false,
                Location = new Point(250, 0),
                NormalColor = AppTheme.Success
            };
            _btnSchedule.Click += (s, e) => OpenScheduleDialog();

            _btnDelete = new DangerButton
            {
                Text = "🗑  Delete",
                Width = 110,
                Height = AppTheme.ButtonHeight,
                Enabled = false,
                Location = new Point(390, 0)
            };
            _btnDelete.Click += async (s, e) => await DeleteAsync();

            actionPanel.Controls.AddRange(new Control[] { _btnEdit, _btnEnroll, _btnSchedule, _btnDelete });

            headerPanel.Controls.AddRange(new Control[] { lblTitle, _lblCount, toolbar });
            // DOCK ORDER: add Fill last, Top second-to-last, Bottom first
            Controls.Add(tableCard);
            Controls.Add(headerPanel);
            Controls.Add(actionPanel);
        }

        private async Task LoadAsync()
        {
            var result = await _groupService.GetAllAsync();
            if (!result.IsSuccess) return;
            PopulateGrid(result.Value);
        }

        private void PopulateGrid(IEnumerable<Group> groups)
        {
            _grid.Rows.Clear();
            int cnt = 0;
            foreach (var g in groups)
            {
                _grid.Rows.Add(
                    g.Id, g.Name,
                    g.SubjectGrade?.Subject?.Name ?? "-",
                    g.SubjectGrade?.Grade?.Name ?? "-",
                    g.SubjectGrade?.SessionFees.ToString("C") ?? "-",
                    g.FirstSessionDate.ToString("MMM dd, yyyy"));
                cnt++;
            }
            _lblCount.Text = $"{cnt} group(s)";
            UpdateButtons();
        }

        private async Task ApplyFilterAsync()
        {
            var r = await _groupService.GetAllAsync();
            if (!r.IsSuccess) return;
            var search = _txtSearch.Text.ToLower();
            var filtered = string.IsNullOrWhiteSpace(search)
                ? r.Value
                : r.Value.Where(g =>
                    g.Name.ToLower().Contains(search) ||
                    (g.SubjectGrade?.Subject?.Name?.ToLower().Contains(search) ?? false));
            PopulateGrid(filtered);
        }

        private void UpdateButtons()
        {
            bool sel = _grid.SelectedRows.Count > 0;
            _btnEdit.Enabled = sel;
            _btnDelete.Enabled = sel;
            _btnEnroll.Enabled = sel;
            _btnSchedule.Enabled = sel;
        }

        private int? GetSelectedId() =>
            _grid.SelectedRows.Count > 0
                ? (int?)_grid.SelectedRows[0].Cells["Id"].Value
                : null;

        private void OpenGroupDialog(int? id)
        {
            var dlg = Program.ServiceLocator.Resolve<GroupDialog>();
            dlg.GroupId = id;
            if (dlg.ShowDialog() == DialogResult.OK)
                _ = LoadAsync();
        }

        private void OpenEnrollDialog()
        {
            if (GetSelectedId() is not int id) return;
            var dlg = Program.ServiceLocator.Resolve<EnrollStudentDialog>();
            dlg.GroupId = id;
            dlg.ShowDialog();
        }

        private void OpenScheduleDialog()
        {
            if (GetSelectedId() is not int id) return;
            var dlg = Program.ServiceLocator.Resolve<GroupScheduleDialog>();
            dlg.GroupId = id;
            dlg.ShowDialog();
        }

        private async Task DeleteAsync()
        {
            if (GetSelectedId() is not int id) return;
            var name = _grid.SelectedRows[0].Cells["Name"].Value?.ToString();
            if (MessageBox.Show($"Delete group '{name}'?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var r = await _groupService.DeleteAsync(id);
            if (r.IsSuccess) await LoadAsync();
            else MessageBox.Show(r.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
