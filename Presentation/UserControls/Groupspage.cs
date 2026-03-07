#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
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
        private CancellationTokenSource _filterCts;

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
            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 60), Width = 1000 };

            _txtSearch = new StyledTextBox
            {
                Width = 260,
                Height = AppTheme.InputHeight,
                Placeholder = "🔍  Search groups...",
                Location = new Point(0, 5)
            };
            _txtSearch.Inner.TextChanged += (s, e) => ScheduleFilter();

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
            _grid.AutoGenerateColumns = false;
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", DataPropertyName = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Group Name", Name = "Name", DataPropertyName = "Name", FillWeight = 22 },
                new DataGridViewTextBoxColumn { HeaderText = "Subject", Name = "Subject", DataPropertyName = "Subject", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Grade", Name = "Grade", DataPropertyName = "Grade", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Fees/Session", Name = "Fees", DataPropertyName = "Fees", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "First Session", Name = "First", DataPropertyName = "First", FillWeight = 20 }
            );
            _grid.SelectionChanged += (s, e) => UpdateButtons();
            tableCard.Controls.Add(_grid);

            var actionPanel = new Panel { Height = 56, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };

            _btnEdit = new RoundedButton { Text = "✏️  Edit", Width = 110, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(0, 0) };
            _btnEdit.Click += (s, e) => OpenGroupDialog(GetSelectedId());

            _btnEnroll = new RoundedButton { Text = "👨‍🎓  Enroll", Width = 120, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(120, 0), NormalColor = AppTheme.Info };
            _btnEnroll.Click += (s, e) => OpenEnrollDialog();

            _btnSchedule = new RoundedButton { Text = "📅  Schedule", Width = 130, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(250, 0), NormalColor = AppTheme.Success };
            _btnSchedule.Click += (s, e) => OpenScheduleDialog();

            _btnDelete = new DangerButton { Text = "🗑  Delete", Width = 110, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(390, 0) };
            _btnDelete.Click += async (s, e) => await DeleteAsync();

            actionPanel.Controls.AddRange(new Control[] { _btnEdit, _btnEnroll, _btnSchedule, _btnDelete });
            headerPanel.Controls.AddRange(new Control[] { lblTitle, _lblCount, toolbar });

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
            var list = groups.ToList();

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Subject", typeof(string));
            dt.Columns.Add("Grade", typeof(string));
            dt.Columns.Add("Fees", typeof(string));
            dt.Columns.Add("First", typeof(string));

            foreach (var g in list)
                dt.Rows.Add(
                    g.Id,
                    g.Name,
                    g.SubjectGrade?.Subject?.Name ?? "-",
                    g.SubjectGrade?.Grade?.Name ?? "-",
                    g.SubjectGrade?.SessionFees.ToString("C") ?? "-",
                    g.FirstSessionDate.ToString("MMM dd, yyyy"));

            _grid.DataSource = null;
            _grid.DataSource = dt;

            _lblCount.Text = $"{list.Count} group(s)";
            UpdateButtons();
        }

        private void ScheduleFilter()
        {
            _filterCts?.Cancel();
            _filterCts?.Dispose();
            _filterCts = new CancellationTokenSource();
            var token = _filterCts.Token;
            _ = Task.Delay(300, token).ContinueWith(async t =>
            {
                if (t.IsCanceled) return;
                await ApplyFilterAsync(token);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task ApplyFilterAsync(CancellationToken token = default)
        {
            var r = await _groupService.GetAllAsync();
            if (token.IsCancellationRequested || !r.IsSuccess) return;

            var search = _txtSearch.Text.ToLower();
            var filtered = string.IsNullOrWhiteSpace(search)
                ? r.Value
                : r.Value.Where(g =>
                    g.Name.ToLower().Contains(search) ||
                    (g.SubjectGrade?.Subject?.Name?.ToLower().Contains(search) ?? false));

            if (token.IsCancellationRequested) return;
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

        private int? GetSelectedId()
        {
            if (_grid.SelectedRows.Count == 0) return null;
            var drv = _grid.SelectedRows[0].DataBoundItem as DataRowView;
            return drv != null ? (int?)drv["Id"] : null;
        }

        private void OpenGroupDialog(int? id)
        {
            var dlg = Program.ServiceLocator.Resolve<GroupDialog>();
            dlg.GroupId = id;
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
                _ = LoadAsync();
        }

        private void OpenEnrollDialog()
        {
            if (GetSelectedId() is not int id) return;
            var dlg = Program.ServiceLocator.Resolve<EnrollStudentDialog>();
            dlg.GroupId = id;
            dlg.ShowDialog(FindForm());
        }

        private void OpenScheduleDialog()
        {
            if (GetSelectedId() is not int id) return;
            var dlg = Program.ServiceLocator.Resolve<GroupScheduleDialog>();
            dlg.GroupId = id;
            dlg.ShowDialog(FindForm());
        }

        private async Task DeleteAsync()
        {
            if (GetSelectedId() is not int id) return;
            var drv = _grid.SelectedRows[0].DataBoundItem as DataRowView;
            var name = drv?["Name"]?.ToString();
            if (MessageBox.Show($"Delete group '{name}'?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var r = await _groupService.DeleteAsync(id);
            if (r.IsSuccess) await LoadAsync();
            else MessageBox.Show(r.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}