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

namespace Presentation
{
    public partial class ExamResultsDialog : Form
    {
        private readonly IExamResultService _resultService;
        private readonly IExamStatusService _statusService;
        private readonly IStudentGroupAggregationService _enrollService;
        private readonly IExamService _examService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ExamId { get; set; }

        private StyledDataGridView _grid;
        private StyledComboBox _cmbStudent;
        private StyledComboBox _cmbStatus;
        private StyledTextBox _txtScore;
        private CheckBox _chkExceed;
        private RoundedButton _btnAdd;
        private DangerButton _btnDelete;
        private Label _lblInfo;
        private Label _lblError;

        public ExamResultsDialog(IExamResultService resultService, IExamStatusService statusService, IStudentGroupAggregationService enrollService, IExamService examService)
        {
            _resultService = resultService;
            _statusService = statusService;
            _enrollService = enrollService;
            _examService = examService;
            InitUI();
        }

        private void InitUI()
        {
            Text = "Exam Results";
            Size = new Size(680, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label { Text = "Exam Results", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 18) };
            _lblInfo = new Label { Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 52) };

            int y = 80;
            Controls.Add(Lbl("Student", 24, y)); _cmbStudent = new StyledComboBox { Width = 200, Location = new Point(24, y + 22) }; Controls.Add(_cmbStudent);
            Controls.Add(Lbl("Score", 234, y)); _txtScore = new StyledTextBox { Width = 100, Height = AppTheme.InputHeight, Location = new Point(234, y + 22) }; Controls.Add(_txtScore);
            Controls.Add(Lbl("Status", 354, y)); _cmbStatus = new StyledComboBox { Width = 160, Location = new Point(354, y + 22) }; Controls.Add(_cmbStatus);

            _chkExceed = new CheckBox { Text = "Exceed Mark", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(524, y + 28) };
            Controls.Add(_chkExceed);

            _btnAdd = new RoundedButton { Text = "+ Add", Width = 80, Height = AppTheme.ButtonHeight, Location = new Point(24, y + 68) };
            _btnAdd.Click += async (s, e) => await AddResultAsync();
            Controls.Add(_btnAdd);

            _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 620, Height = 18, Location = new Point(24, y + 110) };
            Controls.Add(_lblError);

            var card = new CardPanel { Location = new Point(24, y + 132), Width = 620, Height = 300 };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 10 },
                new DataGridViewTextBoxColumn { HeaderText = "Student", Name = "Student", FillWeight = 35 },
                new DataGridViewTextBoxColumn { HeaderText = "Score", Name = "Score", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "/ Mark", Name = "Mark", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Status", Name = "Status", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "%", Name = "Pct", FillWeight = 10 }
            );
            card.Controls.Add(_grid);

            _btnDelete = new DangerButton { Text = "Remove Result", Width = 140, Height = AppTheme.ButtonHeight, Location = new Point(24, y + 444), Enabled = false };
            _btnDelete.Click += async (s, e) => await DeleteAsync();
            _grid.SelectionChanged += (s, e) => _btnDelete.Enabled = _grid.SelectedRows.Count > 0;

            Controls.AddRange(new Control[] { lblTitle, _lblInfo, card, _btnDelete });
            Load += async (s, e) => await OnLoadAsync();
        }

        private async Task OnLoadAsync()
        {
            var exam = await _examService.GetByIdAsync(ExamId);
            if (!exam.IsSuccess) return;

            _lblInfo.Text = $"{exam.Value.Name}  |  Full Mark: {exam.Value.FullMark}  |  Group: {exam.Value.Group?.Name}";

            var enrolled = await _enrollService.GetByGroupAsync(exam.Value.Group.Id);
            if (enrolled.IsSuccess)
            {
                _cmbStudent.Items.Clear();
                foreach (var e in enrolled.Value) _cmbStudent.Items.Add(e.Student);
                _cmbStudent.DisplayMember = "FullName";
                if (_cmbStudent.Items.Count > 0) _cmbStudent.SelectedIndex = 0;
            }

            var statuses = await _statusService.GetAllAsync();
            if (statuses.IsSuccess)
            {
                _cmbStatus.Items.Clear();
                foreach (var s in statuses.Value) _cmbStatus.Items.Add(s);
                _cmbStatus.DisplayMember = "Name";
                if (_cmbStatus.Items.Count > 0) _cmbStatus.SelectedIndex = 0;
            }

            await RefreshGridAsync();
        }

        private async Task RefreshGridAsync()
        {
            var exam = await _examService.GetByIdAsync(ExamId);
            if (!exam.IsSuccess) return;
            int fullMark = exam.Value.FullMark;

            var enrolled = await _enrollService.GetByGroupAsync(exam.Value.Group.Id);
            _grid.Rows.Clear();
            if (!enrolled.IsSuccess) return;

            foreach (var e in enrolled.Value)
            {
                var res = await _resultService.GetAsync(ExamId, e.StudentId);
                if (res.IsSuccess && res.Value != null)
                {
                    double pct = fullMark > 0 ? Math.Round((double)res.Value.Result / fullMark * 100, 1) : 0;
                    _grid.Rows.Add(res.Value.Id, $"{e.Student?.FirstName} {e.Student?.LastName}", res.Value.Result, fullMark, res.Value.Status?.Name, $"{pct}%");
                }
            }
        }

        private async Task AddResultAsync()
        {
            _lblError.Text = "";
            if (_cmbStudent.SelectedItem is not Student s) { _lblError.Text = "Select a student."; return; }
            if (!int.TryParse(_txtScore.Text, out int score)) { _lblError.Text = "Enter a valid score."; return; }
            if (_cmbStatus.SelectedItem is not ExamStatus st) { _lblError.Text = "Select a status."; return; }

            var r = await _resultService.CreateAsync(ExamId, s.Id, st.Id, score, _chkExceed.Checked);
            if (r.IsSuccess) await RefreshGridAsync();
            else _lblError.Text = r.ErrorMessage;
        }

        private async Task DeleteAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var id = (int)_grid.SelectedRows[0].Cells["Id"].Value;
            await _resultService.DeleteAsync(id);
            await RefreshGridAsync();
        }

        private static Label Lbl(string t, int x, int y) => new Label { Text = t, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(x, y) };
    }
}

