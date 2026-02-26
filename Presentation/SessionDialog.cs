#nullable disable
using System.ComponentModel;
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

namespace Presentation
{
    public partial class SessionDialog : Form
    {
        private readonly IClassSessionService _sessionService;
        private readonly IGroupService _groupService;
        private readonly ISessionStatusService _statusService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int? SessionId { get; set; }

        private StyledComboBox _cmbGroup;
        private StyledComboBox _cmbStatus;
        private StyledTextBox _txtNumber;
        private DateTimePicker _dtpDate;
        private RoundedButton _btnSave;
        private GhostButton _btnCancel;
        private Label _lblError;

        public SessionDialog(IClassSessionService sessionService, IGroupService groupService, ISessionStatusService statusService)
        {
            _sessionService = sessionService;
            _groupService = groupService;
            _statusService = statusService;
            InitUI();
        }

        private void InitUI()
        {
            Text = "Class Session";
            Size = new Size(440, 400);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label { Text = "Class Session", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 18) };

            var l1 = Lbl("Group *", 24, 60);
            _cmbGroup = new StyledComboBox { Width = 380, Location = new Point(24, 80) };

            var l2 = Lbl("Session Number *", 24, 126);
            _txtNumber = new StyledTextBox { Width = 180, Height = AppTheme.InputHeight, Location = new Point(24, 146) };

            var l3 = Lbl("Date & Time *", 24, 192);
            _dtpDate = new DateTimePicker { Width = 380, Location = new Point(24, 212), Format = DateTimePickerFormat.Custom, CustomFormat = "MMM dd, yyyy  hh:mm tt", MinDate = DateTime.Today };

            var l4 = Lbl("Status *", 24, 258);
            _cmbStatus = new StyledComboBox { Width = 380, Location = new Point(24, 278) };

            _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 380, Height = 18, Location = new Point(24, 314) };

            _btnSave = new RoundedButton { Text = "Save", Width = 140, Height = AppTheme.ButtonHeight, Location = new Point(24, 336) };
            _btnSave.Click += async (s, e) => await SaveAsync();

            _btnCancel = new GhostButton { Text = "Cancel", Width = 100, Height = AppTheme.ButtonHeight, Location = new Point(176, 336) };
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblTitle, l1, _cmbGroup, l2, _txtNumber, l3, _dtpDate, l4, _cmbStatus, _lblError, _btnSave, _btnCancel });
            Load += async (s, e) => await OnLoadAsync();
        }

        private async Task OnLoadAsync()
        {
            var gr = await _groupService.GetAllAsync();
            if (gr.IsSuccess) { foreach (var g in gr.Value) _cmbGroup.Items.Add(g); _cmbGroup.DisplayMember = "Name"; if (_cmbGroup.Items.Count > 0) _cmbGroup.SelectedIndex = 0; }

            var sr = await _statusService.GetAllAsync();
            if (sr.IsSuccess) { foreach (var s in sr.Value) _cmbStatus.Items.Add(s); _cmbStatus.DisplayMember = "Name"; if (_cmbStatus.Items.Count > 0) _cmbStatus.SelectedIndex = 0; }
        }

        private async Task SaveAsync()
        {
            _lblError.Text = "";
            if (_cmbGroup.SelectedItem is not Group g) { _lblError.Text = "Select a group."; return; }
            if (!int.TryParse(_txtNumber.Text, out int num) || num < 1) { _lblError.Text = "Valid session number required."; return; }
            if (_cmbStatus.SelectedItem is not SessionStatus st) { _lblError.Text = "Select a status."; return; }

            _btnSave.Enabled = false;
            var r = await _sessionService.CreateAsync(g.Id, num, _dtpDate.Value, st.Id);
            _btnSave.Enabled = true;

            if (r.IsSuccess) { DialogResult = DialogResult.OK; Close(); }
            else _lblError.Text = r.ErrorMessage;
        }

        private static Label Lbl(string t, int x, int y) => new Label { Text = t, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(x, y) };
    }

}
