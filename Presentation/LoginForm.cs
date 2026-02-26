#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.Forms
{
    public class LoginForm : Form
    {
        private readonly IUserService _userService;

        private CardPanel _card;
        private StyledTextBox _txtUsername;
        private StyledTextBox _txtPassword;
        private RoundedButton _btnLogin;
        private Label _lblError;
        private CheckBox _chkShowPassword;

        public LoginForm(IUserService userService)
        {
            _userService = userService;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Login — Center Management";
            Size = new Size(480, 600);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = AppTheme.MainBg;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Font = AppTheme.FontLabel;

            _card = new CardPanel
            {
                Width = 380,
                Height = 460,
                CardColor = AppTheme.CardBg,
                CornerRadius = 16,
                Location = new Point((480 - 380) / 2, 70)
            };

            var iconLabel = new Label
            {
                Text = "🎓",
                Font = new Font("Segoe UI Emoji", 36f),
                ForeColor = AppTheme.AccentLight,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(165, 32)
            };

            var lblAppName = new Label
            {
                Text = "Center Management",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = AppTheme.TextPrimary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(72, 90)
            };

            var lblSubtitle = new Label
            {
                Text = "Sign in to continue",
                Font = AppTheme.FontLabel,
                ForeColor = AppTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(138, 122)
            };

            var lblUsername = new Label
            {
                Text = "Username",
                Font = AppTheme.FontLabelBold,
                ForeColor = AppTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(30, 162)
            };

            _txtUsername = new StyledTextBox
            {
                Width = 320,
                Height = AppTheme.InputHeight,
                Placeholder = "Enter your username",
                Location = new Point(30, 182)
            };

            var lblPassword = new Label
            {
                Text = "Password",
                Font = AppTheme.FontLabelBold,
                ForeColor = AppTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(30, 238)
            };

            _txtPassword = new StyledTextBox
            {
                Width = 320,
                Height = AppTheme.InputHeight,
                Placeholder = "Enter your password",
                UseSystemPasswordChar = true,
                Location = new Point(30, 258)
            };

            _chkShowPassword = new CheckBox
            {
                Text = "Show password",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(30, 308),
                Cursor = Cursors.Hand
            };
            _chkShowPassword.CheckedChanged += (s, e) =>
                _txtPassword.UseSystemPasswordChar = !_chkShowPassword.Checked;

            _lblError = new Label
            {
                Text = "",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.Danger,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = 320,
                Height = 20,
                Location = new Point(30, 335),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _btnLogin = new RoundedButton
            {
                Text = "Sign In",
                Width = 320,
                Height = 44,
                Location = new Point(30, 362)
            };
            _btnLogin.Click += async (s, e) => await HandleLoginAsync();

            var lblVersion = new Label
            {
                Text = "v1.0.0",
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextMuted,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(30, 418)
            };

            _card.Controls.AddRange(new Control[]
            {
                iconLabel, lblAppName, lblSubtitle,
                lblUsername, _txtUsername,
                lblPassword, _txtPassword,
                _chkShowPassword, _lblError,
                _btnLogin, lblVersion
            });

            Controls.Add(_card);

            _txtPassword.Inner.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) _btnLogin.PerformClick();
            };
            _txtUsername.Inner.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) _txtPassword.Inner.Focus();
            };
        }

        private async Task HandleLoginAsync()
        {
            var username = _txtUsername.Text.Trim();
            var password = _txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _lblError.Text = "Please enter both username and password.";
                return;
            }

            _btnLogin.Enabled = false;
            _btnLogin.Text = "Signing in...";
            _lblError.Text = "";

            try
            {
                var result = await _userService.AuthenticateAsync(username, password);

                if (result.IsSuccess)
                {
                    AppSession.Login(result.Value);
                    var main = Program.ServiceLocator.Resolve<MainShell>();
                    main.Show();
                    Hide();
                }
                else
                {
                    _lblError.Text = result.ErrorMessage ?? "Invalid credentials.";
                }
            }
            catch
            {
                _lblError.Text = "An error occurred. Please try again.";
            }
            finally
            {
                _btnLogin.Enabled = true;
                _btnLogin.Text = "Sign In";
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var brush = new LinearGradientBrush(
                ClientRectangle,
                AppTheme.MainBg,
                Color.FromArgb(20, 30, 50),
                LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}