#nullable disable
using System.Data;
using Application.Services;
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation.UserControls
{
    public class UsersPage : UserControl
    {
        private readonly IUserService _userService;

        private StyledDataGridView _grid;
        private StyledTextBox _txtSearch;
        private RoundedButton _btnAdd;
        private RoundedButton _btnEdit;
        private DangerButton _btnDelete;
        private Label _lblCount;

        public UsersPage(IUserService userService)
        {
            _userService = userService;
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;
            BuildUI();
            _ = LoadAsync();
        }

        private void BuildUI()
        {
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.Transparent, Padding = new Padding(0, 0, 0, 8) };
            var lblTitle = new SectionLabel { Text = "System Users", Location = new Point(0, 0) };
            _lblCount = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.TextMuted, BackColor = Color.Transparent, AutoSize = true, Location = new Point(0, 36) };
            var toolbar = new Panel { Height = 48, BackColor = Color.Transparent, Location = new Point(0, 60) };
            _txtSearch = new StyledTextBox { Width = 260, Height = AppTheme.InputHeight, Placeholder = "🔍  Search users...", Location = new Point(0, 5) };
            _txtSearch.Inner.TextChanged += async (s, e) => await ApplyFilterAsync();

            _btnAdd = new RoundedButton { Text = "+ Add User", Width = 130, Height = AppTheme.ButtonHeight, Location = new Point(276, 5) };
            _btnAdd.Click += (s, e) => OpenDialog(null);
            toolbar.Controls.AddRange(new Control[] { _txtSearch, _btnAdd });

            var tableCard = new CardPanel { Dock = DockStyle.Fill };
            _grid = new StyledDataGridView { Dock = DockStyle.Fill };
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "Id", FillWeight = 8 },
                new DataGridViewTextBoxColumn { HeaderText = "Username", Name = "UserName", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "First Name", Name = "FirstName", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Last Name", Name = "LastName", FillWeight = 20 },
                new DataGridViewTextBoxColumn { HeaderText = "Email", Name = "Email", FillWeight = 25 },
                new DataGridViewTextBoxColumn { HeaderText = "Permission", Name = "Permission", FillWeight = 15 }
            );
            _grid.SelectionChanged += (s, e) => UpdateButtons();
            tableCard.Controls.Add(_grid);

            var actionPanel = new Panel { Height = 56, Dock = DockStyle.Bottom, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };
            _btnEdit = new RoundedButton { Text = "✏️  Edit", Width = 110, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(0, 0) };
            _btnEdit.Click += (s, e) => OpenDialog(GetSelectedId());

            _btnDelete = new DangerButton { Text = "🗑  Delete", Width = 110, Height = AppTheme.ButtonHeight, Enabled = false, Location = new Point(120, 0) };
            _btnDelete.Click += async (s, e) => await DeleteAsync();
            actionPanel.Controls.AddRange(new Control[] { _btnEdit, _btnDelete });

            headerPanel.Controls.AddRange(new Control[] { lblTitle, _lblCount, toolbar });
            // DOCK ORDER: add Fill last, Top second-to-last, Bottom first
            Controls.Add(tableCard);
            Controls.Add(headerPanel);
            Controls.Add(actionPanel);
        }

        private async Task LoadAsync()
        {
            var r = await _userService.GetAllAsync();
            if (!r.IsSuccess) return;
            PopulateGrid(r.Value);
        }

        private void PopulateGrid(System.Collections.Generic.IEnumerable<User> users)
        {
            _grid.Rows.Clear();
            int cnt = 0;
            foreach (var u in users) { _grid.Rows.Add(u.Id, u.UserName, u.FirstName, u.LastName, u.Email, u.Permission); cnt++; }
            _lblCount.Text = $"{cnt} user(s)";
            UpdateButtons();
        }

        private async Task ApplyFilterAsync()
        {
            var r = await _userService.GetAllAsync();
            if (!r.IsSuccess) return;
            var search = _txtSearch.Text.ToLower();
            PopulateGrid(string.IsNullOrWhiteSpace(search)
                ? r.Value
                : r.Value.Where(u => u.UserName.ToLower().Contains(search) || u.FirstName.ToLower().Contains(search) || u.LastName.ToLower().Contains(search)));
        }

        private void UpdateButtons()
        {
            bool sel = _grid.SelectedRows.Count > 0;
            _btnEdit.Enabled = sel;
            _btnDelete.Enabled = sel && GetSelectedId() != AppSession.CurrentUser?.Id;
        }

        private int? GetSelectedId() => _grid.SelectedRows.Count > 0 ? (int?)_grid.SelectedRows[0].Cells["Id"].Value : null;

        private void OpenDialog(int? id)
        {
            var dlg = Program.ServiceLocator.Resolve<UserDialog>();
            dlg.UserId = id;
            if (dlg.ShowDialog() == DialogResult.OK) _ = LoadAsync();
        }

        private async Task DeleteAsync()
        {
            if (GetSelectedId() is not int id) return;
            if (id == AppSession.CurrentUser?.Id) { MessageBox.Show("You cannot delete your own account.", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("Delete this user?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            var r = await _userService.DeleteAsync(id);
            if (r.IsSuccess) await LoadAsync();
            else MessageBox.Show(r.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
