#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Forms;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public class StudentsPage : UserControl
    {
        private readonly IStudentService _studentService;
        private readonly IGradeService _gradeService;
        private readonly ISubjectGradeHandlerService _handlerService;
        private readonly IGroupService _groupService;
        private readonly IStudentGroupAggregationService _enrollService;

        private StyledTextBox _txtSearch;
        private StyledComboBox _cmbGradeFilter;
        private StyledComboBox _cmbSubjectFilter;
        private StyledComboBox _cmbGroupFilter;
        private RoundedButton _btnAdd;
        private StyledDataGridView _grid;
        private RoundedButton _btnEdit;
        private DangerButton _btnDelete;
        private Label _lblCount;

        private CancellationTokenSource _filterCts;
        private bool _loading;

        // cached data for dependent filters
        private List<SubjectGradeHandler> _allHandlers = new();
        private List<Group> _allGroups = new();

        public StudentsPage(IStudentService studentService, IGradeService gradeService,
                            ISubjectGradeHandlerService handlerService, IGroupService groupService,
                            IStudentGroupAggregationService enrollService)
        {
            _studentService = studentService;
            _gradeService = gradeService;
            _handlerService = handlerService;
            _groupService = groupService;
            _enrollService = enrollService;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
            _ = LoadAsync();
        }

        private void BuildUI()
        {
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 8) };
            var lblTitle = new SectionLabel { Text = "Students", Location = new Point(0, 0) };
            _lblCount = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, BackColor = Color.Transparent, AutoSize = true, Location = new Point(0, 36) };

            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 60), Width = 1200 };

            _txtSearch = new StyledTextBox { Width = 220, Height = AppTheme.InputHeight, Placeholder = "🔍  Search by name or code...", Location = new Point(0, 5) };
            _txtSearch.Inner.TextChanged += (s, e) => ScheduleFilter();

            _cmbGradeFilter = new StyledComboBox { Width = 140, Location = new Point(230, 5) };
            _cmbGradeFilter.SelectedIndexChanged += (s, e) => OnGradeChanged();

            _cmbSubjectFilter = new StyledComboBox { Width = 150, Location = new Point(380, 5) };
            _cmbSubjectFilter.SelectedIndexChanged += (s, e) => OnSubjectChanged();

            _cmbGroupFilter = new StyledComboBox { Width = 160, Location = new Point(540, 5) };
            _cmbGroupFilter.SelectedIndexChanged += (s, e) => ScheduleFilter();

            _btnAdd = new RoundedButton { Text = "+ Add Student", Width = 140, Height = AppTheme.ButtonHeight, Location = new Point(712, 5) };
            _btnAdd.Click += (s, e) => OpenStudentDialog(null);

            toolbar.Controls.AddRange(new Control[] { _txtSearch, _cmbGradeFilter, _cmbSubjectFilter, _cmbGroupFilter, _btnAdd });

            var tableCard = new CardPanel { Dock = DockStyle.Fill };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.AutoGenerateColumns = false;
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", DataPropertyName = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Code", Name = "Code", DataPropertyName = "Code", FillWeight = 12 },
                new DataGridViewTextBoxColumn { HeaderText = "First Name", Name = "FirstName", DataPropertyName = "FirstName", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Last Name", Name = "LastName", DataPropertyName = "LastName", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Grade", Name = "Grade", DataPropertyName = "Grade", FillWeight = 14 },
                new DataGridViewTextBoxColumn { HeaderText = "Phone", Name = "Phone", DataPropertyName = "Phone", FillWeight = 16 },
                new DataGridViewTextBoxColumn { HeaderText = "Guardian Ph.", Name = "Guardian", DataPropertyName = "Guardian", FillWeight = 16 }
            );
            _grid.SelectionChanged += (s, e) => UpdateActionButtons();
            tableCard.Controls.Add(_grid);

            var actionPanel = new Panel { Height = 56, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };
            _btnEdit = new RoundedButton { Text = "✏️  Edit", Width = 120, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(0, 0) };
            _btnEdit.Click += (s, e) => EditSelected();
            _btnDelete = new DangerButton { Text = "🗑  Delete", Width = 120, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(130, 0) };
            _btnDelete.Click += async (s, e) => await DeleteSelectedAsync();
            actionPanel.Controls.AddRange(new Control[] { _btnEdit, _btnDelete });

            headerPanel.Controls.AddRange(new Control[] { lblTitle, _lblCount, toolbar });
            Controls.Add(tableCard);
            Controls.Add(headerPanel);
            Controls.Add(actionPanel);
        }

        // ── Initial load ──────────────────────────────────────────────────────
        private async Task LoadAsync()
        {
            _loading = true;

            // Load grades
            var gradesResult = await _gradeService.GetAllAsync();
            if (gradesResult.IsSuccess)
            {
                _cmbGradeFilter.Items.Clear();
                _cmbGradeFilter.Items.Add("All Grades");
                foreach (var g in gradesResult.Value) _cmbGradeFilter.Items.Add(g);
                _cmbGradeFilter.DisplayMember = "Name";
            }

            // Cache all handlers and groups
            var handlersResult = await _handlerService.GetAllAsync();
            if (handlersResult.IsSuccess) _allHandlers = handlersResult.Value.ToList();

            var groupsResult = await _groupService.GetAllAsync();
            if (groupsResult.IsSuccess) _allGroups = groupsResult.Value.ToList();

            // Init subject + group combos with "All" options
            ResetSubjectFilter(null);
            ResetGroupFilter(null);

            _loading = false;
            _cmbGradeFilter.SelectedIndex = 0;
        }

        // ── Dependent filter cascade ──────────────────────────────────────────
        private void OnGradeChanged()
        {
            if (_loading) return;
            var grade = _cmbGradeFilter.SelectedItem as Grade;
            ResetSubjectFilter(grade);
            ResetGroupFilter(null);
            ScheduleFilter();
        }

        private void OnSubjectChanged()
        {
            if (_loading) return;
            var grade = _cmbGradeFilter.SelectedItem as Grade;
            var subject = _cmbSubjectFilter.SelectedItem as Subject;
            ResetGroupFilter(subject == null ? null : (grade, subject));
            ScheduleFilter();
        }

        private void ResetSubjectFilter(Grade grade)
        {
            _loading = true;
            _cmbSubjectFilter.Items.Clear();
            _cmbSubjectFilter.Items.Add("All Subjects");

            var handlers = grade == null
                ? _allHandlers
                : _allHandlers.Where(h => h.GradeId == grade.Id).ToList();

            // distinct subjects from handlers
            var subjects = handlers
                .Where(h => h.Subject != null)
                .Select(h => h.Subject)
                .GroupBy(s => s.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var s in subjects) _cmbSubjectFilter.Items.Add(s);
            _cmbSubjectFilter.DisplayMember = "Name";
            _cmbSubjectFilter.SelectedIndex = 0;
            _loading = false;
        }

        private void ResetGroupFilter((Grade grade, Subject subject)? filter)
        {
            _loading = true;
            _cmbGroupFilter.Items.Clear();
            _cmbGroupFilter.Items.Add("All Groups");

            var groups = _allGroups.AsEnumerable();
            if (filter.HasValue)
            {
                groups = groups.Where(g =>
                    g.SubjectGrade?.GradeId == filter.Value.grade.Id &&
                    g.SubjectGrade?.SubjectId == filter.Value.subject.Id);
            }
            else if (_cmbGradeFilter.SelectedItem is Grade grade)
            {
                groups = groups.Where(g => g.SubjectGrade?.GradeId == grade.Id);
            }

            foreach (var g in groups) _cmbGroupFilter.Items.Add(g);
            _cmbGroupFilter.DisplayMember = "Name";
            _cmbGroupFilter.SelectedIndex = 0;
            _loading = false;
        }

        // ── Grid population ───────────────────────────────────────────────────
        private async Task RefreshGridAsync()
        {
            var result = await _studentService.GetAllAsync();
            if (!result.IsSuccess) { MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            await PopulateGridAsync(result.Value);
        }

        private async Task PopulateGridAsync(IEnumerable<Student> students)
        {
            var list = students.ToList();

            // If a group is selected, filter to enrolled students only
            if (_cmbGroupFilter.SelectedItem is Group selectedGroup)
            {
                var enrolled = await _enrollService.GetByGroupAsync(selectedGroup.Id);
                if (enrolled.IsSuccess)
                {
                    var enrolledIds = enrolled.Value.Select(e => e.StudentId).ToHashSet();
                    list = list.Where(s => enrolledIds.Contains(s.Id)).ToList();
                }
            }

            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("Grade", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Guardian", typeof(string));

            foreach (var s in list)
                dt.Rows.Add(s.Id, s.Code, s.FirstName, s.LastName,
                            s.Grade?.Name ?? "-", s.PersonalPhone, s.GuardianPhone);

            _grid.DataSource = null;
            _grid.DataSource = dt;
            _lblCount.Text = $"{list.Count} student(s) found";
            UpdateActionButtons();
        }

        // ── Debounced filter ──────────────────────────────────────────────────
        private void ScheduleFilter()
        {
            if (_loading) return;
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
            var result = await _studentService.GetAllAsync();
            if (token.IsCancellationRequested || !result.IsSuccess) return;

            var students = result.Value.AsEnumerable();

            // Grade filter
            if (_cmbGradeFilter.SelectedItem is Grade grade)
                students = students.Where(s => s.Grade?.Id == grade.Id);

            // Search filter
            var search = _txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                students = students.Where(s =>
                    s.FirstName.ToLower().Contains(search) ||
                    s.LastName.ToLower().Contains(search) ||
                    s.Code.ToLower().Contains(search));

            if (token.IsCancellationRequested) return;
            await PopulateGridAsync(students);
        }

        // ── Actions ───────────────────────────────────────────────────────────
        private void UpdateActionButtons()
        {
            bool has = _grid.SelectedRows.Count > 0;
            _btnEdit.Enabled = has;
            _btnDelete.Enabled = has;
        }

        private void OpenStudentDialog(int? id)
        {
            var dlg = Program.ServiceLocator.Resolve<StudentDialog>();
            dlg.StudentId = id;
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK) _ = RefreshGridAsync();
        }

        private void EditSelected()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var row = (_grid.SelectedRows[0].DataBoundItem as DataRowView)?["Id"];
            if (row != null) OpenStudentDialog((int)row);
        }

        private async Task DeleteSelectedAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var drv = _grid.SelectedRows[0].DataBoundItem as DataRowView;
            var id = (int)drv["Id"];
            var name = $"{drv["FirstName"]} {drv["LastName"]}";

            if (MessageBox.Show($"Delete student '{name}'? This will also remove all group enrollments.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var result = await _studentService.DeleteAsync(id);
            if (result.IsSuccess) await RefreshGridAsync();
            else MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}