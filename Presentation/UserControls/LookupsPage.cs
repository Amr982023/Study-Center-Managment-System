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
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public class LookupsPage : UserControl
    {
        private readonly IGradeService _gradeService;
        private readonly ISubjectService _subjectService;
        private readonly ISessionStatusService _sessionStatusService;
        private readonly IExamStatusService _examStatusService;

        public LookupsPage(
            IGradeService gradeService,
            ISubjectService subjectService,
            ISessionStatusService sessionStatusService,
            IExamStatusService examStatusService,
            IMessageTypeService messageTypeService)
        {
            _gradeService = gradeService;
            _subjectService = subjectService;
            _sessionStatusService = sessionStatusService;
            _examStatusService = examStatusService;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
        }

        private void BuildUI()
        {
            var lblTitle = new SectionLabel { Text = "Lookups & Settings", Location = new Point(0, 0) };
            var lblSub = new Label { Text = "Manage grades, subjects, and status lists", Font = AppTheme.FontLabel, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(0, 36) };

            var tabs = new TabControl
            {
                Location = new Point(0, 68),
                Width = 900,
                Height = 500,
                Font = AppTheme.FontLabel,
                DrawMode = TabDrawMode.OwnerDrawFixed,
                ItemSize = new Size(160, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            tabs.DrawItem += (s, e) =>
            {
                bool active = e.Index == tabs.SelectedIndex;
                using var bgBrush = new System.Drawing.SolidBrush(active ? AppTheme.CardBg : AppTheme.SidebarBg);
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
                var sf = new System.Drawing.StringFormat { Alignment = System.Drawing.StringAlignment.Center, LineAlignment = System.Drawing.StringAlignment.Center };
                using var fgBrush = new System.Drawing.SolidBrush(active ? AppTheme.Tangerine : AppTheme.TextSecondary);
                e.Graphics.DrawString(tabs.TabPages[e.Index].Text, active ? AppTheme.FontLabelBold : AppTheme.FontLabel, fgBrush, e.Bounds, sf);
            };

            tabs.TabPages.Add(BuildTab("Grades", LoadGrades, AddGrade, DeleteGrade));
            tabs.TabPages.Add(BuildTab("Subjects", LoadSubjects, AddSubject, DeleteSubject));
            tabs.TabPages.Add(BuildTab("Session Statuses", LoadSessionStatuses, AddSessionStatus, DeleteSessionStatus));
            tabs.TabPages.Add(BuildTab("Exam Statuses", LoadExamStatuses, AddExamStatus, DeleteExamStatus));

            Controls.AddRange(new Control[] { lblTitle, lblSub, tabs });
        }

        // ── Loaders ───────────────────────────────────────
        private async Task<(bool ok, System.Collections.Generic.IEnumerable<(int Id, string Name)> items, string error)> LoadGrades()
        {
            var r = await _gradeService.GetAllAsync();
            return r.IsSuccess
                ? (true, r.Value.Select(x => (x.Id, x.Name)), null)
                : (false, System.Linq.Enumerable.Empty<(int, string)>(), r.ErrorMessage);
        }

        private async Task<(bool ok, System.Collections.Generic.IEnumerable<(int Id, string Name)> items, string error)> LoadSubjects()
        {
            var r = await _subjectService.GetAllAsync();
            return r.IsSuccess
                ? (true, r.Value.Select(x => (x.Id, x.Name)), null)
                : (false, System.Linq.Enumerable.Empty<(int, string)>(), r.ErrorMessage);
        }

        private async Task<(bool ok, System.Collections.Generic.IEnumerable<(int Id, string Name)> items, string error)> LoadSessionStatuses()
        {
            var r = await _sessionStatusService.GetAllAsync();
            return r.IsSuccess
                ? (true, r.Value.Select(x => (x.Id, x.Name)), null)
                : (false, System.Linq.Enumerable.Empty<(int, string)>(), r.ErrorMessage);
        }

        private async Task<(bool ok, System.Collections.Generic.IEnumerable<(int Id, string Name)> items, string error)> LoadExamStatuses()
        {
            var r = await _examStatusService.GetAllAsync();
            return r.IsSuccess
                ? (true, r.Value.Select(x => (x.Id, x.Name)), null)
                : (false, System.Linq.Enumerable.Empty<(int, string)>(), r.ErrorMessage);
        }

        // ── Adders — return (bool ok, string error) ───────
        private async Task<(bool ok, string error)> AddGrade(string name)
        {
            var r = await _gradeService.CreateAsync(name);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        private async Task<(bool ok, string error)> AddSubject(string name)
        {
            var r = await _subjectService.CreateAsync(name);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        private async Task<(bool ok, string error)> AddSessionStatus(string name)
        {
            var r = await _sessionStatusService.CreateAsync(name);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        private async Task<(bool ok, string error)> AddExamStatus(string name)
        {
            var r = await _examStatusService.CreateAsync(name);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        // ── Deleters — return (bool ok, string error) ─────
        private async Task<(bool ok, string error)> DeleteGrade(int id)
        {
            var r = await _gradeService.DeleteAsync(id);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        private async Task<(bool ok, string error)> DeleteSubject(int id)
        {
            var r = await _subjectService.DeleteAsync(id);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        private async Task<(bool ok, string error)> DeleteSessionStatus(int id)
        {
            var r = await _sessionStatusService.DeleteAsync(id);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        private async Task<(bool ok, string error)> DeleteExamStatus(int id)
        {
            var r = await _examStatusService.DeleteAsync(id);
            return (r.IsSuccess, r.IsSuccess ? null : r.ErrorMessage);
        }

        // ── Tab builder — takes simple async delegates ─────
        private TabPage BuildTab(
            string title,
            Func<Task<(bool ok, System.Collections.Generic.IEnumerable<(int Id, string Name)> items, string error)>> loadFn,
            Func<string, Task<(bool ok, string error)>> addFn,
            Func<int, Task<(bool ok, string error)>> deleteFn)
        {
            var page = new TabPage(title) { BackColor = AppTheme.MainBg, Padding = new Padding(16) };
            var card = new CardPanel { Dock = DockStyle.Fill };
            var inner = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.Transparent };

            // Toolbar
            var toolbar = new Panel { Height = 64, Dock = DockStyle.Top, BackColor = Color.Transparent };
            var txtNew = new StyledTextBox { Width = 280, Height = AppTheme.InputHeight, Placeholder = $"New {title.TrimEnd('s')}...", Location = new Point(0, 4) };
            var btnAdd = new RoundedButton { Text = "+ Add", Width = 90, Height = AppTheme.ButtonHeight, Location = new Point(296, 4) };
            var lblErr = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 500, Height = 18, Location = new Point(0, 48) };
            toolbar.Controls.AddRange(new Control[] { txtNew, btnAdd, lblErr });

            // Grid
            var grid = new StyledDataGridView { Dock = DockStyle.Fill };
            grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 15 },
                new DataGridViewTextBoxColumn { HeaderText = "Name", Name = "Name", FillWeight = 85 }
            );

            // Delete button
            var btnDel = new DangerButton { Text = "🗑  Delete", Width = 120, Height = AppTheme.ButtonHeight, Dock = DockStyle.Bottom, Enabled = false };
            grid.SelectionChanged += (s, e) => btnDel.Enabled = grid.SelectedRows.Count > 0;

            // Refresh helper
            async Task Refresh()
            {
                var (ok, items, err) = await loadFn();
                grid.Rows.Clear();
                if (ok)
                    foreach (var (id, name) in items)
                        grid.Rows.Add(id, name);
                else
                    lblErr.Text = err ?? "Failed to load.";
            }

            // Add handler
            btnAdd.Click += async (s, e) =>
            {
                lblErr.Text = "";
                if (string.IsNullOrWhiteSpace(txtNew.Text)) { lblErr.Text = "Name cannot be empty."; return; }
                var (ok, err) = await addFn(txtNew.Text.Trim());
                if (ok) { txtNew.Text = ""; await Refresh(); }
                else lblErr.Text = err ?? "Failed to add.";
            };

            // Delete handler
            btnDel.Click += async (s, e) =>
            {
                if (grid.SelectedRows.Count == 0) return;
                if (MessageBox.Show("Delete this item?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                var id = (int)grid.SelectedRows[0].Cells["Id"].Value;
                var (ok, err) = await deleteFn(id);
                if (ok) await Refresh();
                else lblErr.Text = err ?? "Failed to delete.";
            };

            txtNew.Inner.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) btnAdd.PerformClick(); };

            inner.Controls.AddRange(new Control[] { toolbar, grid, btnDel });
            card.Controls.Add(inner);
            page.Controls.Add(card);

            // Load when tab becomes visible
            page.Enter += async (s, e) => await Refresh();
            _ = Refresh();

            return page;
        }
    }
}

