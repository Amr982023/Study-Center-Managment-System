#nullable disable
using System.ComponentModel;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation
{
    public partial class UserDialog : Form
    {
        private readonly IUserService _userService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int? UserId { get; set; }

        private StyledTextBox _txtFirst;
        private StyledTextBox _txtMid;
        private StyledTextBox _txtLast;
        private StyledTextBox _txtPhone;
        private StyledTextBox _txtUsername;
        private StyledTextBox _txtEmail;
        private StyledTextBox _txtPassword;
        private StyledComboBox _cmbGender;
        private StyledComboBox _cmbPermission;
        private RoundedButton _btnSave;
        private GhostButton _btnCancel;
        private Label _lblError;

        public UserDialog(IUserService userService)
        {
            _userService = userService;
            InitUI();
        }

        private void InitUI()
        {
            Text = UserId.HasValue ? "Edit User" : "Add User";
            Size = new Size(520, 560);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label { Text = UserId.HasValue ? "Edit User" : "New User", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 18) };
            var div = new Panel { Height = 1, Width = 462, BackColor = AppTheme.Border, Location = new Point(24, 54) };

            int c1 = 24, c2 = 272;
            int r1 = 70, r2 = 146, r3 = 222, r4 = 298, r5 = 374;

            var l1 = L("First Name *", c1, r1); _txtFirst = TB(c1, r1, 200);
            var l2 = L("Middle Name", c2, r1); _txtMid = TB(c2, r1, 200);
            var l3 = L("Last Name *", c1, r2); _txtLast = TB(c1, r2, 200);
            var l4 = L("Phone *", c2, r2); _txtPhone = TB(c2, r2, 200);
            var l5 = L("Username *", c1, r3); _txtUsername = TB(c1, r3, 200);
            var l6 = L("Email", c2, r3); _txtEmail = TB(c2, r3, 200);
            var l7 = L("Password *", c1, r4); _txtPassword = TB(c1, r4, 200); _txtPassword.UseSystemPasswordChar = true;

            var l8 = L("Gender", c2, r4);
            _cmbGender = new StyledComboBox { Width = 200, Location = new Point(c2, r4 + 22) };
            _cmbGender.Items.AddRange(new object[] { "Male", "Female" });
            _cmbGender.SelectedIndex = 0;

            var l9 = L("Permission *", c1, r5);
            _cmbPermission = new StyledComboBox { Width = 200, Location = new Point(c1, r5 + 22) };
            _cmbPermission.Items.AddRange(new object[] { "Admin", "Staff", "Viewer" });
            _cmbPermission.SelectedIndex = 1;

            _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 462, Height = 18, Location = new Point(24, 458) };

            _btnSave = new RoundedButton { Text = UserId.HasValue ? "Save Changes" : "Create User", Width = 160, Height = AppTheme.ButtonHeight, Location = new Point(24, 480) };
            _btnSave.Click += async (s, e) => await SaveAsync();

            _btnCancel = new GhostButton { Text = "Cancel", Width = 100, Height = AppTheme.ButtonHeight, Location = new Point(196, 480) };
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblTitle, div, l1, _txtFirst, l2, _txtMid, l3, _txtLast, l4, _txtPhone, l5, _txtUsername, l6, _txtEmail, l7, _txtPassword, l8, _cmbGender, l9, _cmbPermission, _lblError, _btnSave, _btnCancel });
        }

        private async Task SaveAsync()
        {
            _lblError.Text = "";
            if (string.IsNullOrWhiteSpace(_txtFirst.Text) || string.IsNullOrWhiteSpace(_txtLast.Text) ||
                string.IsNullOrWhiteSpace(_txtUsername.Text) || string.IsNullOrWhiteSpace(_txtPassword.Text))
            { _lblError.Text = "Fill all required fields (*)."; return; }

            _btnSave.Enabled = false;
            var r = await _userService.CreateAsync(
                _txtFirst.Text.Trim(), _txtLast.Text.Trim(),
                _txtPhone.Text.Trim(), _cmbGender.SelectedItem?.ToString() ?? "Male",
                _txtUsername.Text.Trim(), _txtEmail.Text.Trim(),
                _cmbPermission.SelectedItem?.ToString() ?? "Staff",
                _txtPassword.Text,
                string.IsNullOrWhiteSpace(_txtMid.Text) ? null : _txtMid.Text.Trim());
            _btnSave.Enabled = true;

            if (r.IsSuccess) { DialogResult = DialogResult.OK; Close(); }
            else _lblError.Text = r.ErrorMessage;
        }

        private static StyledTextBox TB(int x, int y, int w) => new StyledTextBox { Width = w, Height = AppTheme.InputHeight, Location = new Point(x, y + 22) };
        private static Label L(string t, int x, int y) => new Label { Text = t, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(x, y) };
    }


}
