#nullable disable
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.Email;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public class PaymentsPage : UserControl
    {
        private readonly IPaymentService _paymentService;
        private readonly IStudentService _studentService;
        private readonly EmailNotificationService _emailService;

        private StyledDataGridView _grid;
        private StyledTextBox _txtSearch;
        private StyledComboBox _cmbMonth;
        private RoundedButton _btnAdd;
        private RoundedButton _btnSendReminders;
        private DangerButton _btnDelete;
        private Label _lblCount;
        private Label _lblTotal;

        public PaymentsPage(IPaymentService paymentService, IStudentService studentService,
                            IUserService userService, EmailNotificationService emailService)
        {
            _paymentService = paymentService;
            _studentService = studentService;
            _emailService = emailService;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
            _ = LoadAsync();
        }

        private void BuildUI()
        {
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 158, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 8) };
            var lblTitle = new SectionLabel { Text = "Payments", Location = new Point(0, 0) };
            _lblCount = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, BackColor = Color.Transparent, AutoSize = true, Location = new Point(0, 36) };

            var summaryCard = new CardPanel { Location = new Point(0, 80), Width = 300, Height = 70 };
            summaryCard.Controls.Add(new Label { Text = "Total This Month", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 8) });
            _lblTotal = new Label { Text = "—", Font = new System.Drawing.Font("Segoe UI", 20f, System.Drawing.FontStyle.Bold), ForeColor = AppTheme.Success, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 32) };
            summaryCard.Controls.Add(_lblTotal);

            var toolbar = new Panel { Height = 100, BackColor = Color.Transparent, Location = new Point(350, 90), Width = 1000 };
            _txtSearch = new StyledTextBox { Width = 220, Height = AppTheme.InputHeight, Placeholder = "🔍  Search student...", Location = new Point(0, 5) };
            _txtSearch.Inner.TextChanged += async (s, e) => await LoadAsync();

            _cmbMonth = new StyledComboBox { Width = 140, Location = new Point(236, 10) };
            for (int i = 1; i <= 12; i++) _cmbMonth.Items.Add(new MonthItem(i));
            _cmbMonth.DisplayMember = "Name";
            _cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
            _cmbMonth.SelectedIndexChanged += async (s, e) => await LoadAsync();

            _btnAdd = new RoundedButton { Text = "+ New Payment", Width = 150, Height = AppTheme.ButtonHeight, Location = new Point(392, 5) };
            _btnAdd.Click += (s, e) => OpenPaymentDialog();

            _btnSendReminders = new RoundedButton
            {
                Text = "📧  Send Reminders",
                Width = 160,
                Height = AppTheme.ButtonHeight,
                Location = new Point(556, 5),
                NormalColor = AppTheme.Info
            };
            _btnSendReminders.Click += async (s, e) => await SendRemindersAsync();

            toolbar.Controls.AddRange(new Control[] { _txtSearch, _cmbMonth, _btnAdd, _btnSendReminders });

            var tableCard = new CardPanel { Dock = DockStyle.Fill };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Student", Name = "Student", FillWeight = 28 },
                new DataGridViewTextBoxColumn { HeaderText = "Amount", Name = "Amount", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Month", Name = "Month", FillWeight = 12 },
                new DataGridViewTextBoxColumn { HeaderText = "Date", Name = "Date", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Performed By", Name = "User", FillWeight = 17 }
            );
            _grid.SelectionChanged += (s, e) => _btnDelete.Enabled = _grid.SelectedRows.Count > 0;
            tableCard.Controls.Add(_grid);

            var actionPanel = new Panel { Height = 56, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };
            _btnDelete = new DangerButton { Text = "🗑  Delete Payment", Width = 160, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(0, 7) };
            _btnDelete.Click += async (s, e) => await DeleteAsync();
            actionPanel.Controls.Add(_btnDelete);

            headerPanel.Controls.AddRange(new Control[] { lblTitle, _lblCount, summaryCard, toolbar });
            Controls.Add(tableCard);
            Controls.Add(headerPanel);
            Controls.Add(actionPanel);
        }

        private async Task LoadAsync()
        {
            var studentsResult = await _studentService.GetAllAsync();
            if (!studentsResult.IsSuccess) return;

            var search = _txtSearch.Text.ToLower();
            var students = studentsResult.Value.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                students = students.Where(s => $"{s.FirstName} {s.LastName}".ToLower().Contains(search) || s.Code.ToLower().Contains(search));

            int selectedMonth = (_cmbMonth.SelectedItem as MonthItem)?.Month ?? DateTime.Now.Month;

            _grid.Rows.Clear();
            decimal total = 0;
            int cnt = 0;

            foreach (var student in students)
            {
                var pr = await _paymentService.GetByStudentAsync(student.Id);
                if (!pr.IsSuccess) continue;
                foreach (var p in pr.Value.Where(p => p.Month == selectedMonth))
                {
                    _grid.Rows.Add(p.Id, $"{student.FirstName} {student.LastName}", p.Amount.ToString("C"), new DateTime(2000, p.Month, 1).ToString("MMMM"), p.DateTime.ToString("MMM dd, yyyy"), p.PerformedBy?.UserName ?? "-");
                    total += p.Amount;
                    cnt++;
                }
            }

            _lblCount.Text = $"{cnt} payment(s)";
            _lblTotal.Text = total.ToString("C");
        }

        private void OpenPaymentDialog()
        {
            var dlg = Program.ServiceLocator.Resolve<PaymentDialog>();
            if (dlg.ShowDialog() == DialogResult.OK) _ = LoadAsync();
        }

        private async Task SendRemindersAsync()
        {
            int month = (_cmbMonth.SelectedItem as MonthItem)?.Month ?? DateTime.Now.Month;
            string monthName = new DateTime(2000, month, 1).ToString("MMMM");

            var confirm = MessageBox.Show(
                $"Send fee reminders to all students who haven't paid for {monthName}?",
                "Send Reminders", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            _btnSendReminders.Enabled = false;
            var unpaid = await _paymentService.GetUnpaidStudentsAsync(month);
            if (!unpaid.IsSuccess) { _btnSendReminders.Enabled = true; return; }

            var (sent, skipped, errors) = await _emailService.SendMonthlyRemindersAsync(unpaid.Value, month);
            _btnSendReminders.Enabled = true;

            string msg = $"Reminders sent: {sent}\nSkipped (no email): {skipped}";
            if (errors.Count > 0) msg += $"\nFailed: {errors.Count}";
            MessageBox.Show(msg, "Reminders Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task DeleteAsync()
        {
            if (_grid.SelectedRows.Count == 0) return;
            var id = (int)_grid.SelectedRows[0].Cells["Id"].Value;
            if (MessageBox.Show("Delete this payment record?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            var r = await _paymentService.DeleteAsync(id);
            if (r.IsSuccess) await LoadAsync();
            else MessageBox.Show(r.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private class MonthItem
        {
            public int Month { get; }
            public string Name { get; }
            public MonthItem(int m) { Month = m; Name = new DateTime(2000, m, 1).ToString("MMMM"); }
            public override string ToString() => Name;
        }
    }
}