#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.Email;
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public partial class ExamsPage : UserControl
    {
        private readonly IExamService _examService;
        private readonly IGroupService _groupService;
        private readonly IStudentGroupAggregationService _enrollService;
        private readonly EmailNotificationService _emailService;

        private StyledDataGridView _grid;
        private StyledComboBox _cmbGroup;
        private RoundedButton _btnAdd;
        private RoundedButton _btnResults;
        private RoundedButton _btnSendReminder;
        private DangerButton _btnDelete;
        private Label _lblCount;

        public ExamsPage(IExamService examService, IGroupService groupService,
                         IStudentGroupAggregationService enrollService,
                         EmailNotificationService emailService)
        {
            _examService = examService;
            _groupService = groupService;
            _enrollService = enrollService;
            _emailService = emailService;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
            _ = LoadAsync();
        }

        private void BuildUI()
        {
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 8) };
            var lblTitle = new SectionLabel { Text = "Exams", Location = new Point(0, 0) };
            _lblCount = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, BackColor = Color.Transparent, AutoSize = true, Location = new Point(0, 36) };

            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 60), Width = 1000 };
            _cmbGroup = new StyledComboBox { Width = 200, Location = new Point(0, 11) };
            _cmbGroup.SelectedIndexChanged += async (s, e) => await LoadAsync();

            _btnAdd = new RoundedButton { Text = "+ Add Exam", Width = 130, Height = AppTheme.ButtonHeight, Location = new Point(216, 5) };
            _btnAdd.Click += (s, e) => OpenExamDialog(null);

            toolbar.Controls.AddRange(new Control[] { _cmbGroup, _btnAdd });

            var tableCard = new CardPanel { Dock = DockStyle.Fill };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Exam Name", Name = "Name", FillWeight = 30 },
                new DataGridViewTextBoxColumn { HeaderText = "Group", Name = "Group", FillWeight = 25 },
                new DataGridViewTextBoxColumn { HeaderText = "Full Mark", Name = "FullMark", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Date", Name = "Date", FillWeight = 22 }
            );
            _grid.SelectionChanged += (s, e) => UpdateButtons();
            tableCard.Controls.Add(_grid);

            var actionPanel = new Panel { Height = 56, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };

            _btnResults = new RoundedButton { Text = "📊  Results", Width = 130, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(0, 0), NormalColor = AppTheme.Info };
            _btnResults.Click += (s, e) => OpenResultsDialog();

            _btnSendReminder = new RoundedButton { Text = "📧  Remind Students", Width = 160, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(140, 0), NormalColor = AppTheme.Warning };
            _btnSendReminder.Click += async (s, e) => await SendExamReminderAsync();

            _btnDelete = new DangerButton { Text = "🗑  Delete", Width = 110, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(310, 0) };
            _btnDelete.Click += async (s, e) => await DeleteAsync();

            actionPanel.Controls.AddRange(new Control[] { _btnResults, _btnSendReminder, _btnDelete });

            headerPanel.Controls.AddRange(new Control[] { lblTitle, _lblCount, toolbar });
            Controls.Add(tableCard);
            Controls.Add(headerPanel);
            Controls.Add(actionPanel);
        }

        private async Task LoadAsync()
        {
            if (_cmbGroup.Items.Count == 0)
            {
                var gr = await _groupService.GetAllAsync();
                if (gr.IsSuccess)
                {
                    _cmbGroup.Items.Add("All Groups");
                    foreach (var g in gr.Value) _cmbGroup.Items.Add(g);
                    _cmbGroup.DisplayMember = "Name";
                    _cmbGroup.SelectedIndex = 0;
                }
            }

            IEnumerable<Exam> exams;
            if (_cmbGroup.SelectedItem is Group grp)
            {
                var r = await _examService.GetByGroupAsync(grp.Id);
                if (!r.IsSuccess) return;
                exams = r.Value;
            }
            else
            {
                var r = await _examService.GetAllAsync();
                if (!r.IsSuccess) return;
                exams = r.Value;
            }

            _grid.Rows.Clear();
            int cnt = 0;
            foreach (var e in exams)
            {
                _grid.Rows.Add(e.Id, e.Name, e.Group?.Name ?? "-", e.FullMark, e.ExamDate.ToString("MMM dd, yyyy  hh:mm tt"));
                cnt++;
            }
            _lblCount.Text = $"{cnt} exam(s)";
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            bool sel = _grid.SelectedRows.Count > 0;
            _btnResults.Enabled = sel;
            _btnSendReminder.Enabled = sel;
            _btnDelete.Enabled = sel;
        }

        private int? GetSelectedId() => _grid.SelectedRows.Count > 0
            ? (int?)_grid.SelectedRows[0].Cells["Id"].Value : null;

        private void OpenExamDialog(int? id)
        {
            var dlg = Program.ServiceLocator.Resolve<ExamDialog>();
            dlg.ExamId = id;
            if (dlg.ShowDialog() == DialogResult.OK) _ = LoadAsync();
        }

        private void OpenResultsDialog()
        {
            if (GetSelectedId() is not int id) return;
            var dlg = Program.ServiceLocator.Resolve<ExamResultsDialog>();
            dlg.ExamId = id;
            dlg.ShowDialog();
        }

        private async Task SendExamReminderAsync()
        {
            if (GetSelectedId() is not int id) return;

            var exam = await _examService.GetByIdAsync(id);
            if (!exam.IsSuccess) return;

            string confirm = $"Send exam reminder to all students enrolled in \"{exam.Value.Name}\"?\n\nExam Date: {exam.Value.ExamDate:MMM dd, yyyy  hh:mm tt}";
            if (MessageBox.Show(confirm, "Send Reminder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            _btnSendReminder.Enabled = false;
            var enrolled = await _enrollService.GetByGroupAsync(exam.Value.Group.Id);
            if (!enrolled.IsSuccess) { _btnSendReminder.Enabled = true; return; }

            var students = enrolled.Value.Select(e => e.Student).ToList();
            var (sent, skipped, errors) = await _emailService.SendExamRemindersAsync(students, exam.Value);
            _btnSendReminder.Enabled = true;

            string msg = $"Reminders sent: {sent}\nSkipped (no email): {skipped}";
            if (errors.Count > 0) msg += $"\nFailed: {errors.Count}";
            MessageBox.Show(msg, "Reminders Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task DeleteAsync()
        {
            if (GetSelectedId() is not int id) return;
            if (MessageBox.Show("Delete this exam and all its results?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            var r = await _examService.DeleteAsync(id);
            if (r.IsSuccess) await LoadAsync();
            else MessageBox.Show(r.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}