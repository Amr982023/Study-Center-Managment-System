#nullable disable
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation
{
    public partial class PaymentDialog : Form
    {
        private readonly IPaymentService _paymentService;
        private readonly IStudentService _studentService;

        private StyledComboBox _cmbStudent;
        private StyledComboBox _cmbMonth;
        private StyledTextBox _txtAmount;
        private RoundedButton _btnSave;
        private GhostButton _btnCancel;
        private Label _lblError;
        private Label _lblHistory;
        private StyledDataGridView _gridHistory;

        public PaymentDialog(IPaymentService paymentService, IStudentService studentService)
        {
            _paymentService = paymentService;
            _studentService = studentService;
            InitUI();
        }

        private void InitUI()
        {
            Text = "Record Payment";
            Size = new Size(480, 520);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label { Text = "New Payment", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 18) };

            var l1 = Lbl("Student *", 24, 60);
            _cmbStudent = new StyledComboBox { Width = 420, Location = new Point(24, 80) };
            _cmbStudent.SelectedIndexChanged += async (s, e) => await LoadHistoryAsync();

            var l2 = Lbl("Month *", 24, 124);
            _cmbMonth = new StyledComboBox { Width = 200, Location = new Point(24, 144) };
            for (int i = 1; i <= 12; i++) _cmbMonth.Items.Add(new DateTime(2000, i, 1).ToString("MMMM"));
            _cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

            var l3 = Lbl("Amount (EGP) *", 240, 124);
            _txtAmount = new StyledTextBox { Width = 200, Height = AppTheme.InputHeight, Location = new Point(240, 144), Placeholder = "0.00" };

            _lblHistory = Lbl("Payment History", 24, 196);

            var card = new CardPanel { Location = new Point(24, 216), Width = 420, Height = 180 };
            _gridHistory = new StyledDataGridView { Dock = DockStyle.Fill };
            _gridHistory.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "Month", Name = "Month", FillWeight = 30 },
                new DataGridViewTextBoxColumn { HeaderText = "Amount", Name = "Amount", FillWeight = 30 },
                new DataGridViewTextBoxColumn { HeaderText = "Date", Name = "Date", FillWeight = 40 }
            );
            card.Controls.Add(_gridHistory);

            _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 420, Height = 18, Location = new Point(24, 408) };

            _btnSave = new RoundedButton { Text = "Record Payment", Width = 160, Height = AppTheme.ButtonHeight, Location = new Point(24, 430) };
            _btnSave.Click += async (s, e) => await SaveAsync();

            _btnCancel = new GhostButton { Text = "Cancel", Width = 100, Height = AppTheme.ButtonHeight, Location = new Point(196, 430) };
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblTitle, l1, _cmbStudent, l2, _cmbMonth, l3, _txtAmount, _lblHistory, card, _lblError, _btnSave, _btnCancel });

            Load += async (s, e) =>
            {
                var r = await _studentService.GetAllAsync();
                if (r.IsSuccess)
                {
                    foreach (var st in r.Value) _cmbStudent.Items.Add(st);
                    _cmbStudent.DisplayMember = "FullName";
                    if (_cmbStudent.Items.Count > 0) _cmbStudent.SelectedIndex = 0;
                }
            };
        }

        private async Task LoadHistoryAsync()
        {
            if (_cmbStudent.SelectedItem is not Student s) return;
            var r = await _paymentService.GetByStudentAsync(s.Id);
            _gridHistory.Rows.Clear();
            if (r.IsSuccess)
                foreach (var p in r.Value)
                    _gridHistory.Rows.Add(new DateTime(2000, p.Month, 1).ToString("MMMM"), p.Amount.ToString("C"), p.DateTime.ToString("MMM dd"));
        }

        private async Task SaveAsync()
        {
            _lblError.Text = "";
            if (_cmbStudent.SelectedItem is not Student s) { _lblError.Text = "Select a student."; return; }
            if (!decimal.TryParse(_txtAmount.Text, out decimal amount) || amount <= 0) { _lblError.Text = "Enter a valid amount."; return; }
            int month = _cmbMonth.SelectedIndex + 1;

            _btnSave.Enabled = false;
            var r = await _paymentService.CreateAsync(s.Id, AppSession.CurrentUser.Id, amount, month);
            _btnSave.Enabled = true;

            if (r.IsSuccess) { DialogResult = DialogResult.OK; Close(); }
            else _lblError.Text = r.ErrorMessage;
        }

        private static Label Lbl(string t, int x, int y) => new Label { Text = t, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(x, y) };
    }
}



