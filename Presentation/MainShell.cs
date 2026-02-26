#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Presentation.Controls;
using Presentation.Theme;
using Presentation.UserControls;

namespace Presentation.Forms
{
    public class MainShell : Form
    {
        private Panel _sidebar;
        private Panel _topbar;
        private Panel _contentArea;
        private Panel _activeNavItem;
        private Control _currentPage;
        private readonly List<(Panel panel, string page)> _navItems = new();

        public MainShell()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Center Management System";
            Size = new Size(1280, 800);
            MinimumSize = new Size(1024, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = AppTheme.MainBg;
            Font = AppTheme.FontLabel;
            FormBorderStyle = FormBorderStyle.Sizable;

            // ── CRITICAL: Dock order must be Fill → Top → Left
            // WinForms processes Dock in reverse Controls-add order:
            // Add Fill first, then Top, then Left — so Left occupies left,
            // Top takes remaining top, Fill takes whatever is left.
            BuildContentArea();   // Dock = Fill   (add first)
            BuildTopbar();        // Dock = Top    (add second)
            BuildSidebar();       // Dock = Left   (add last)

            Controls.Add(_contentArea);
            Controls.Add(_topbar);
            Controls.Add(_sidebar);

            NavigateTo("Dashboard");
        }

        // ─────────────────────────────────────────────────
        //  Sidebar
        // ─────────────────────────────────────────────────
        private void BuildSidebar()
        {
            _sidebar = new Panel
            {
                Width = AppTheme.SidebarWidth,
                Dock = DockStyle.Left,
                BackColor = AppTheme.SidebarBg,
                Padding = new Padding(0)
            };

            // Logo strip
            var logoPanel = new Panel { Height = 78, Dock = DockStyle.Top, BackColor = AppTheme.BlueDialneDark };
            var logoIcon = new Label { Text = "🎓", Font = new Font("Segoe UI Emoji", 20f), ForeColor = AppTheme.Tangerine, BackColor = Color.Transparent, AutoSize = true, Location = new Point(14, 20) };
            var logoName = new Label { Text = "CenterPro", Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = false, Size = new Size(178, 28), Location = new Point(48, 26) };
            logoPanel.Controls.AddRange(new Control[] { logoIcon, logoName });

            var topSep = new Panel { Height = 2, Dock = DockStyle.Top, BackColor = AppTheme.Tangerine };

            // Nav items container — scrollable if many items
            var navContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = false
            };

            var navDefs = new[]
            {
                ("Dashboard",       "🏠", "Dashboard"),
                ("Students",        "👨‍🎓", "Students"),
                ("Groups",          "📚", "Groups"),
                ("Sessions",        "📅", "Sessions"),
                ("Exams",           "📝", "Exams"),
                ("Payments",        "💳", "Payments"),
                ("Users",           "👤", "Users"),
                ("Grades & Subjects","🏷", "Lookups"),
            };

            int yOffset = 12;
            foreach (var (label, icon, page) in navDefs)
            {
                var item = CreateNavItem(icon, label, page);
                item.Location = new Point(0, yOffset);
                navContainer.Controls.Add(item);
                _navItems.Add((item, page));
                yOffset += AppTheme.NavItemHeight + 3;
            }

            // Bottom: user info + logout
            var bottomSep = new Panel { Height = 1, Dock = DockStyle.Bottom, BackColor = AppTheme.Border };

            var logoutBtn = new Panel { Height = 52, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Cursor = Cursors.Hand };
            var logoutIcon = new Label { Text = "⏻", Font = new Font("Segoe UI", 13f), ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = true, Location = new Point(18, 16), Cursor = Cursors.Hand };
            var logoutText = new Label { Text = "Sign Out", Font = AppTheme.FontNavBold, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = true, Location = new Point(46, 16), Cursor = Cursors.Hand };
            logoutBtn.Controls.AddRange(new Control[] { logoutIcon, logoutText });
            logoutBtn.Click += Logout;
            logoutIcon.Click += Logout;
            logoutText.Click += Logout;
            logoutBtn.MouseEnter += (s, e) => logoutBtn.BackColor = Color.FromArgb(30, 248, 113, 113);
            logoutBtn.MouseLeave += (s, e) => logoutBtn.BackColor = Color.Transparent;

            var userSep = new Panel { Height = 1, Dock = DockStyle.Bottom, BackColor = AppTheme.Border };

            var userPanel = new Panel { Height = 68, Dock = DockStyle.Bottom, BackColor = AppTheme.BlueDialneDark };
            var userAvatar = new Panel { Size = new Size(38, 38), Location = new Point(12, 15), BackColor = AppTheme.Tangerine };
            userAvatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var b = new SolidBrush(AppTheme.Tangerine);
                e.Graphics.FillEllipse(b, new Rectangle(0, 0, 37, 37));
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var tb = new SolidBrush(Color.White);
                string initials = AppSession.CurrentUser != null
                    ? $"{AppSession.CurrentUser.FirstName?[0]}{AppSession.CurrentUser.LastName?[0]}"
                    : "?";
                e.Graphics.DrawString(initials, new Font("Segoe UI", 12f, FontStyle.Bold), tb, new Rectangle(0, 0, 37, 37), sf);
            };
            var lblUserName = new Label { Text = AppSession.CurrentUser?.UserName ?? "User", Font = AppTheme.FontLabelBold, ForeColor = Color.White, BackColor = Color.Transparent, AutoSize = true, Location = new Point(56, 14) };
            var lblUserRole = new Label { Text = AppSession.CurrentUser?.Permission ?? "Staff", Font = AppTheme.FontSmall, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(56, 36) };
            userPanel.Controls.AddRange(new Control[] { userAvatar, lblUserName, lblUserRole });

            _sidebar.Controls.AddRange(new Control[] { navContainer, topSep, logoPanel, logoutBtn, bottomSep, userSep, userPanel });
        }

        private Panel CreateNavItem(string icon, string label, string page)
        {
            var panel = new Panel { Width = AppTheme.SidebarWidth, Height = AppTheme.NavItemHeight, BackColor = Color.Transparent, Cursor = Cursors.Hand };

            // Left accent bar (shown when active)
            var accentBar = new Panel { Width = 4, Height = AppTheme.NavItemHeight, Location = new Point(0, 0), BackColor = Color.Transparent };

            var lblIcon = new Label { Text = icon, Font = new Font("Segoe UI Emoji", 14f), ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(18, 14), Cursor = Cursors.Hand };
            var lblText = new Label { Text = label, Font = AppTheme.FontNav, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(50, 15), Cursor = Cursors.Hand };

            panel.Controls.AddRange(new Control[] { accentBar, lblIcon, lblText });

            void SetActive(bool active)
            {
                panel.BackColor = active ? AppTheme.NavActiveBg : Color.Transparent;
                accentBar.BackColor = active ? AppTheme.Tangerine : Color.Transparent;
                lblIcon.ForeColor = active ? AppTheme.Tangerine : AppTheme.TextSecondary;
                lblText.ForeColor = active ? Color.White : AppTheme.TextSecondary;
                lblText.Font = active ? AppTheme.FontNavBold : AppTheme.FontNav;
            }

            void OnEnter(object s, EventArgs e) { if (panel != _activeNavItem) panel.BackColor = AppTheme.NavHoverBg; }
            void OnLeave(object s, EventArgs e) { if (panel != _activeNavItem) panel.BackColor = Color.Transparent; }
            void OnClick(object s, EventArgs e) => NavigateTo(page);

            panel.Tag = (Action<bool>)SetActive;

            panel.MouseEnter += OnEnter; lblIcon.MouseEnter += OnEnter; lblText.MouseEnter += OnEnter;
            panel.MouseLeave += OnLeave; lblIcon.MouseLeave += OnLeave; lblText.MouseLeave += OnLeave;
            panel.Click += OnClick; lblIcon.Click += OnClick; lblText.Click += OnClick;

            return panel;
        }

        // ─────────────────────────────────────────────────
        //  Topbar
        // ─────────────────────────────────────────────────
        private void BuildTopbar()
        {
            _topbar = new Panel
            {
                Height = AppTheme.TopbarHeight,
                Dock = DockStyle.Top,
                BackColor = AppTheme.TopbarBg,
                Padding = new Padding(0)
            };

            var leftAccent = new Panel { Width = 4, Height = AppTheme.TopbarHeight, Location = new Point(0, 0), BackColor = AppTheme.Tangerine, Dock = DockStyle.Left };

            var lblPageTitle = new Label
            {
                Name = "lblPageTitle",
                Text = "Dashboard",
                Font = AppTheme.FontH3,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(20, 18)
            };

            var lblDate = new Label
            {
                Text = DateTime.Now.ToString("dddd, MMMM dd yyyy"),
                Font = AppTheme.FontSmall,
                ForeColor = AppTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Location = new Point(900, 22)
            };

            var bottomLine = new Panel { Height = 1, Dock = DockStyle.Bottom, BackColor = AppTheme.Border };

            _topbar.Controls.AddRange(new Control[] { leftAccent, lblPageTitle, lblDate, bottomLine });

            // Reposition date label on resize
            _topbar.Resize += (s, e) =>
            {
                lblDate.Location = new Point(_topbar.Width - lblDate.Width - 24, 22);
            };
        }

        // ─────────────────────────────────────────────────
        //  Content Area
        // ─────────────────────────────────────────────────
        private void BuildContentArea()
        {
            _contentArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = AppTheme.MainBg,
                Padding = new Padding(28, 24, 28, 24)
            };
        }

        // ─────────────────────────────────────────────────
        //  Navigation
        // ─────────────────────────────────────────────────
        public void NavigateTo(string page)
        {
            // Update topbar title
            if (_topbar.Controls["lblPageTitle"] is Label lbl)
                lbl.Text = page switch
                {
                    "Lookups" => "Grades & Subjects",
                    _ => page
                };

            // Update nav item states
            foreach (var (panel, p) in _navItems)
            {
                bool active = p == page;
                if (panel.Tag is Action<bool> setActive)
                    setActive(active);
                if (active)
                    _activeNavItem = panel;
            }

            // Swap page
            _currentPage?.Dispose();
            _contentArea.Controls.Clear();

            Control newPage = page switch
            {
                "Dashboard" => Program.ServiceLocator.Resolve<DashboardPage>(),
                "Students" => Program.ServiceLocator.Resolve<StudentsPage>(),
                "Groups" => Program.ServiceLocator.Resolve<GroupsPage>(),
                "Sessions" => Program.ServiceLocator.Resolve<SessionsPage>(),
                "Exams" => Program.ServiceLocator.Resolve<ExamsPage>(),
                "Payments" => Program.ServiceLocator.Resolve<PaymentsPage>(),
                "Users" => Program.ServiceLocator.Resolve<UsersPage>(),
                "Lookups" => Program.ServiceLocator.Resolve<LookupsPage>(),
                _ => null
            };

            if (newPage != null)
            {
                newPage.Dock = DockStyle.Fill;
                _contentArea.Controls.Add(newPage);
                _currentPage = newPage;
            }
        }

        private void Logout(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to sign out?", "Sign Out",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            AppSession.Logout();
            Program.ServiceLocator.Resolve<LoginForm>().Show();
            Close();
        }
    }
}