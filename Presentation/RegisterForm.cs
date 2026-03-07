#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.ServicesInterfaces;
using Application.Validation;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.Forms
{
    public class RegisterForm : Form
    {
        private readonly IUserService _userService;

        private StyledTextBox _txtFirst;
        private StyledTextBox _txtLast;
        private StyledTextBox _txtPhone;
        private StyledTextBox _txtEmail;
        private StyledTextBox _txtUsername;
        private StyledTextBox _txtPassword;
        private StyledTextBox _txtConfirm;
        private RoundedButton _btnRegister;
        private Label _lblError;

        public RegisterForm(IUserService userService)
        {
            _userService = userService;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Setup — Create Admin Account";
            Size = new Size(500, 860);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = AppTheme.BlueDialneDark;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = AppTheme.FontLabel;

            var card = new CardPanel
            {
                Width = 420,
                Height = 740,
                CardColor = AppTheme.BlueDianne,
                CornerRadius = 16,
                ShowBorder = true,
                Location = new Point((500 - 420) / 2, 50)
            };

            var accentStrip = new Panel
            {
                Width = 420,
                Height = 4,
                Location = new Point(0, 0),
                BackColor = AppTheme.Tangerine
            };
            card.Controls.Add(accentStrip);

            var iconLbl = new Label { Text = "🎓", Font = new Font("Segoe UI Emoji", 34f), ForeColor = AppTheme.Tangerine, BackColor = Color.Transparent, AutoSize = true, Location = new Point(186, 22) };
            var title = new Label { Text = "First-Time Setup", Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = true, Location = new Point(130, 80) };
            var sub = new Label { Text = "Create your administrator account", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(100, 108) };
            var div = new Panel { Width = 360, Height = 1, BackColor = AppTheme.Border, Location = new Point(30, 138) };

            int y = 150;
            Label L(string t) { var l = new Label { Text = t, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(30, y) }; y += 20; return l; }
            StyledTextBox TB(string ph = "", bool pw = false) { var tb = new StyledTextBox { Width = 360, Height = AppTheme.InputHeight, Placeholder = ph, UseSystemPasswordChar = pw, Location = new Point(30, y) }; y += 52; return tb; }

            var l1 = L("First Name *"); _txtFirst = TB("Ahmed");
            var l2 = L("Last Name *"); _txtLast = TB("Mohamed");
            var l3 = L("Phone *"); _txtPhone = TB("01xxxxxxxxx");
            var l4 = L("Email *"); _txtEmail = TB("admin@example.com");
            var l5 = L("Username *"); _txtUsername = TB("admin");
            var l6 = L("Password *"); _txtPassword = TB("", true);
            var l7 = L("Confirm Password *"); _txtConfirm = TB("", true);

            _lblError = new Label { Text = "", Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 360, Height = 22, Location = new Point(30, y), TextAlign = ContentAlignment.MiddleCenter };
            y += 26;

            _btnRegister = new RoundedButton { Text = "Create Account", Width = 360, Height = 46, Location = new Point(30, y), Font = new Font("Segoe UI", 12f, FontStyle.Bold) };
            _btnRegister.Click += async (s, e) => await HandleRegisterAsync();

            card.Controls.AddRange(new Control[]
            {
                accentStrip, iconLbl, title, sub, div,
                l1, _txtFirst, l2, _txtLast, l3, _txtPhone,
                l4, _txtEmail, l5, _txtUsername,
                l6, _txtPassword, l7, _txtConfirm,
                _lblError, _btnRegister
            });

            Controls.Add(card);

            _txtPassword.Inner.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) _txtConfirm.Inner.Focus(); };
            _txtConfirm.Inner.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) _btnRegister.PerformClick(); };
        }

        private async Task HandleRegisterAsync()
        {
            _lblError.Text = "";

            var first = _txtFirst.Text.Trim();
            var last = _txtLast.Text.Trim();
            var phone = _txtPhone.Text.Trim();
            var email = _txtEmail.Text.Trim();
            var username = _txtUsername.Text.Trim();
            var password = _txtPassword.Text;
            var confirm = _txtConfirm.Text;

            // Required fields
            var reqErr = Validator.ValidateAllRequired(
                (first, "First name"),
                (last, "Last name"),
                (phone, "Phone"),
                (email, "Email"),
                (username, "Username"),
                (password, "Password"));
            if (reqErr != null) { _lblError.Text = reqErr; return; }

            // Format validation
            var phoneErr = Validator.ValidatePhone(phone);
            if (phoneErr != null) { _lblError.Text = phoneErr; return; }

            var emailErr = Validator.ValidateEmail(email);
            if (emailErr != null) { _lblError.Text = emailErr; return; }

            var passErr = Validator.ValidatePassword(password, confirm);
            if (passErr != null) { _lblError.Text = passErr; return; }

            _btnRegister.Enabled = false;
            _btnRegister.Text = "Creating...";

            try
            {
                // Pass positionally — avoids named-param mismatch with interface
                var result = await _userService.CreateAsync(
                    first, last, phone, "Male",
                    username, email, "Admin", password);

                if (result.IsSuccess)
                {
                    // Auto-login after register — go straight to main shell
                    AppSession.Login(result.Value);
                    Program.ServiceLocator.Resolve<MainShell>().Show();
                    Close();
                }
                else
                {
                    _lblError.Text = result.ErrorMessage ?? "Failed to create account.";
                }
            }
            catch (Exception ex)
            {
                _lblError.Text = $"Error: {ex.Message}";
            }
            finally
            {
                _btnRegister.Enabled = true;
                _btnRegister.Text = "Create Account";
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var brush = new LinearGradientBrush(ClientRectangle,
                AppTheme.BlueDialneDark, Color.FromArgb(15, 35, 44), LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}