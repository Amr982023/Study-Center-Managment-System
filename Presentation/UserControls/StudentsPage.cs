#nullable disable
using System;
using System.Drawing;
using System.Linq;
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

        private StyledTextBox _txtSearch;
        private StyledComboBox _cmbGradeFilter;
        private RoundedButton _btnAdd;
        private StyledDataGridView _grid;
        private RoundedButton _btnEdit;
        private DangerButton _btnDelete;
        private Label _lblCount;

        public StudentsPage(IStudentService studentService, IGradeService gradeService)
        {
            _studentService = studentService;
            _gradeService = gradeService;
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
            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 60) };

            _txtSearch = new StyledTextBox { Width = 260, Height = AppTheme.InputHeight, Placeholder = "🔍  Search by name or code...", Location = new Point(0, 5) };
            _txtSearch.Inner.TextChanged += async (s, e) => await ApplyFilterAsync();

            _cmbGradeFilter = new StyledComboBox { Width = 150, Location = new Point(275, 5) };
            _cmbGradeFilter.SelectedIndexChanged += async (s, e) => await ApplyFilterAsync();

            _btnAdd = new RoundedButton { Text = "+ Add Student", Width = 140, Height = AppTheme.ButtonHeight, Location = new Point(440, 5) };
            _btnAdd.Click += (s, e) => OpenStudentDialog(null);

            toolbar.Controls.AddRange(new Control[] { _txtSearch, _cmbGradeFilter, _btnAdd });

            var tableCard = new CardPanel { Dock = DockStyle.Fill };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Code", Name = "Code", FillWeight = 12 },
                new DataGridViewTextBoxColumn { HeaderText = "First Name", Name = "FirstName", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Last Name", Name = "LastName", FillWeight = 18 },
                new DataGridViewTextBoxColumn { HeaderText = "Grade", Name = "Grade", FillWeight = 14 },
                new DataGridViewTextBoxColumn { HeaderText = "Phone", Name = "Phone", FillWeight = 16 },
                new DataGridViewTextBoxColumn { HeaderText = "Guardian Ph.", Name = "Guardian", FillWeight = 16 }
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
            // DOCK ORDER: add Fill last, Top second-to-last, Bottom first
            Controls.Add(tableCard);
            Controls.Add(headerPanel);
            Controls.Add(actionPanel);
        }

        private async Task LoadAsync()
        {
            var gradesResult = await _gradeService.GetAllAsync();
            if (gradesResult.IsSuccess)
            {
                _cmbGradeFilter.Items.Clear();
                _cmbGradeFilter.Items.Add("All Grades");
                foreach (var g in gradesResult.Value) _cmbGradeFilter.Items.Add(g);
                _cmbGradeFilter.DisplayMember = "Name";
                _cmbGradeFilter.SelectedIndex = 0;
            }
            await RefreshGridAsync();
        }

        private async Task RefreshGridAsync()
        {
            var result = await _studentService.GetAllAsync();
            if (!result.IsSuccess) { MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            PopulateGrid(result.Value);
        }

        private void PopulateGrid(System.Collections.Generic.IEnumerable<Student> students)
        {
            _grid.Rows.Clear();
            int count = 0;
            foreach (var s in students)
            {
                _grid.Rows.Add(s.Id, s.Code, s.FirstName, s.LastName, s.Grade?.Name ?? "-", s.PersonalPhone, s.GuardianPhone);
                count++;
            }
            _lblCount.Text = $"{count} student(s) found";
            UpdateActionButtons();
        }

        private async Task ApplyFilterAsync()
        {
            var result = await _studentService.GetAllAsync();
            if (!result.IsSuccess) return;

            var students = result.Value.AsEnumerable();
            if (_cmbGradeFilter.SelectedItem is Grade grade)
                students = students.Where(s => s.Grade?.Id == grade.Id);

            var search = _txtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                students = students.Where(s =>
                    s.FirstName.ToLower().Contains(search) ||
                    s.LastName.ToLower().Contains(search) ||
                    s.Code.ToLower().Contains(search));

            PopulateGrid(students);
        }

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
            if (dlg.ShowDialog() == DialogResult.OK) _ = RefreshGridAsync();
        }

        private void EditSelected()
        {
            if (_grid.SelectedRows.Count == 0) return;
            OpenStudentDialog((int)_grid.SelectedRows[0].Cells["Id"].Value);
        }

        private async Task DeleteSelectedAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var id = (int)_grid.SelectedRows[0].Cells["Id"].Value;
            var name = $"{_grid.SelectedRows[0].Cells["FirstName"].Value} {_grid.SelectedRows[0].Cells["LastName"].Value}";

            if (MessageBox.Show($"Delete student '{name}'? This will also remove all group enrollments.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var result = await _studentService.DeleteAsync(id);
            if (result.IsSuccess) await RefreshGridAsync();
            else MessageBox.Show(result.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}