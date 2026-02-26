#nullable disable
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.Forms
{
    public class DashboardPage : UserControl
    {
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly IExamService _examService;

        private StatCard _cardStudents;
        private StatCard _cardGroups;
        private StatCard _cardPayments;
        private StatCard _cardExams;
        private StyledDataGridView _gridRecent;
        private CardPanel _tableCard;

        public DashboardPage(
            IStudentService studentService,
            IGroupService groupService,
            IExamService examService)
        {
            _studentService = studentService;
            _groupService = groupService;
            _examService = examService;

            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
            _ = LoadDataAsync();
        }

        private void BuildUI()
        {
            var lblWelcome = new Label
            {
                Text = $"Welcome back, {AppSession.CurrentUser?.FirstName ?? "User"} 👋",
                Font = AppTheme.FontTitle,
                ForeColor = AppTheme.TextPrimary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(0, 0)
            };

            var lblSub = new Label
            {
                Text = $"Here's what's happening — {DateTime.Now:MMMM dd, yyyy}",
                Font = AppTheme.FontLabel,
                ForeColor = AppTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(0, 38)
            };

            _cardStudents = new StatCard { Title = "Total Students", Value = "...", Icon = "👨‍🎓", Location = new Point(0, 80), Width = 200 };
            _cardGroups = new StatCard { Title = "Active Groups", Value = "...", Icon = "📚", Location = new Point(216, 80), Width = 200 };
            _cardPayments = new StatCard { Title = "Month", Value = "...", Icon = "💳", Location = new Point(432, 80), Width = 200 };
            _cardExams = new StatCard { Title = "Total Exams", Value = "...", Icon = "📝", Location = new Point(648, 80), Width = 200 };

            var lblRecent = new Label
            {
                Text = "Recent Students",
                Font = AppTheme.FontH3,
                ForeColor = AppTheme.TextPrimary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(0, 220)
            };

            _tableCard = new CardPanel { Location = new Point(0, 252), Width = 860, Height = 320 };

            _gridRecent = new StyledDataGridView { Dock = DockStyle.Fill };
            _gridRecent.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "Code", Name = "Code", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "First Name", Name = "FirstName", FillWeight = 25 },
                new DataGridViewTextBoxColumn { HeaderText = "Last Name", Name = "LastName", FillWeight = 25 },
                new DataGridViewTextBoxColumn { HeaderText = "Grade", Name = "Grade", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Phone", Name = "Phone", FillWeight = 25 }
            );
            _tableCard.Controls.Add(_gridRecent);

            Controls.AddRange(new Control[]
            {
                lblWelcome, lblSub,
                _cardStudents, _cardGroups, _cardPayments, _cardExams,
                lblRecent, _tableCard
            });

            Resize += (s, e) => LayoutCards();
        }

        private void LayoutCards()
        {
            int cardW = Math.Max(160, (Width - 60) / 4);
            int gap = 20;
            _cardStudents.Width = cardW;
            _cardGroups.Width = cardW;
            _cardPayments.Width = cardW;
            _cardExams.Width = cardW;
            _cardStudents.Location = new Point(0, 80);
            _cardGroups.Location = new Point(cardW + gap, 80);
            _cardPayments.Location = new Point((cardW + gap) * 2, 80);
            _cardExams.Location = new Point((cardW + gap) * 3, 80);
            _tableCard.Width = Width;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var students = await _studentService.GetAllAsync();
                var groups = await _groupService.GetAllAsync();
                var exams = await _examService.GetAllAsync();

                if (students.IsSuccess)
                {
                    _cardStudents.Value = students.Value.Count().ToString();
                    _gridRecent.Rows.Clear();
                    int count = 0;
                    foreach (var s in students.Value)
                    {
                        if (count++ >= 10) break;
                        _gridRecent.Rows.Add(s.Code, s.FirstName, s.LastName, s.Grade?.Name ?? "-", s.PersonalPhone);
                    }
                }
                if (groups.IsSuccess) _cardGroups.Value = groups.Value.Count().ToString();
                if (exams.IsSuccess) _cardExams.Value = exams.Value.Count().ToString();

                _cardPayments.Value = DateTime.Now.ToString("MMM");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}