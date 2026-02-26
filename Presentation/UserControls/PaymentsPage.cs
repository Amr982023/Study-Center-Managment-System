#nullable disable
using System.Data;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public class PaymentsPage : UserControl
    {
        private readonly IPaymentService _paymentService;
        private readonly IStudentService _studentService;

        private StyledDataGridView _grid;
        private StyledTextBox _txtSearch;
        private StyledComboBox _cmbMonth;
        private RoundedButton _btnAdd;
        private DangerButton _btnDelete;
        private Label _lblCount;
        private Label _lblTotal;

        public PaymentsPage(IPaymentService paymentService, IStudentService studentService, IUserService userService)
        {
            _paymentService = paymentService;
            _studentService = studentService;
            // userService intentionally not stored — payments use AppSession.CurrentUser
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

            var summaryCard = new CardPanel { Location = new Point(0, 60), Width = 300, Height = 70 };
            summaryCard.Controls.Add(new Label { Text = "Total This Month", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 12) });
            _lblTotal = new Label { Text = "—", Font = new Font("Segoe UI", 20f, FontStyle.Bold), ForeColor = AppTheme.Success, BackColor = Color.Transparent, AutoSize = true, Location = new Point(16, 32) };
            summaryCard.Controls.Add(_lblTotal);

            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 142) };
            _txtSearch = new StyledTextBox { Width = 220, Height = AppTheme.InputHeight, Placeholder = "🔍  Search student...", Location = new Point(0, 5) };
            _txtSearch.Inner.TextChanged += async (s, e) => await LoadAsync();

            _cmbMonth = new StyledComboBox { Width = 140, Location = new Point(236, 5) };
            for (int i = 1; i <= 12; i++) _cmbMonth.Items.Add(new MonthItem(i));
            _cmbMonth.DisplayMember = "Name";
            _cmbMonth.SelectedIndex = DateTime.Now.Month - 1;
            _cmbMonth.SelectedIndexChanged += async (s, e) => await LoadAsync();

            _btnAdd = new RoundedButton { Text = "+ New Payment", Width = 150, Height = AppTheme.ButtonHeight, Location = new Point(392, 5) };
            _btnAdd.Click += (s, e) => OpenPaymentDialog();

            toolbar.Controls.AddRange(new Control[] { _txtSearch, _cmbMonth, _btnAdd });

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
            _btnDelete = new DangerButton { Text = "🗑  Delete Payment", Width = 160, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(0, 0) };
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
