#nullable disable
using System.ComponentModel;
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation
{
    public partial class ExamDialog : Form
    {
        private readonly IExamService _examService;
        private readonly IGroupService _groupService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int? ExamId { get; set; }

        private StyledTextBox _txtName;
        private StyledTextBox _txtFullMark;
        private StyledComboBox _cmbGroup;
        private RoundedButton _btnSave;
        private GhostButton _btnCancel;
        private Label _lblError;

        public ExamDialog(IExamService examService, IGroupService groupService)
        {
            _examService = examService;
            _groupService = groupService;
            InitUI();
        }

        private void InitUI()
        {
            Text = "Exam Details";
            Size = new Size(420, 340);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label { Text = "Exam Details", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 18) };

            var l1 = Lbl("Exam Name *", 24, 60);
            _txtName = new StyledTextBox { Width = 360, Height = AppTheme.InputHeight, Location = new Point(24, 80) };

            var l2 = Lbl("Group *", 24, 130);
            _cmbGroup = new StyledComboBox { Width = 360, Location = new Point(24, 150) };

            var l3 = Lbl("Full Mark *", 24, 196);
            _txtFullMark = new StyledTextBox { Width = 360, Height = AppTheme.InputHeight, Location = new Point(24, 216) };

            _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 360, Height = 18, Location = new Point(24, 262) };

            _btnSave = new RoundedButton { Text = "Save", Width = 140, Height = AppTheme.ButtonHeight, Location = new Point(24, 284) };
            _btnSave.Click += async (s, e) => await SaveAsync();

            _btnCancel = new GhostButton { Text = "Cancel", Width = 100, Height = AppTheme.ButtonHeight, Location = new Point(176, 284) };
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblTitle, l1, _txtName, l2, _cmbGroup, l3, _txtFullMark, _lblError, _btnSave, _btnCancel });
            Load += async (s, e) =>
            {
                var gr = await _groupService.GetAllAsync();
                if (gr.IsSuccess) { foreach (var g in gr.Value) _cmbGroup.Items.Add(g); _cmbGroup.DisplayMember = "Name"; if (_cmbGroup.Items.Count > 0) _cmbGroup.SelectedIndex = 0; }
            };
        }

        private async Task SaveAsync()
        {
            _lblError.Text = "";
            if (string.IsNullOrWhiteSpace(_txtName.Text)) { _lblError.Text = "Exam name required."; return; }
            if (_cmbGroup.SelectedItem is not Group g) { _lblError.Text = "Select a group."; return; }
            if (!int.TryParse(_txtFullMark.Text, out int fm) || fm <= 0) { _lblError.Text = "Valid full mark required."; return; }

            _btnSave.Enabled = false;
            var r = await _examService.CreateAsync(g.Id, _txtName.Text.Trim(), fm);
            _btnSave.Enabled = true;

            if (r.IsSuccess) { DialogResult = DialogResult.OK; Close(); }
            else _lblError.Text = r.ErrorMessage;
        }

        private static Label Lbl(string t, int x, int y) => new Label { Text = t, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(x, y) };
    }
}
