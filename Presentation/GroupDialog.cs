#nullable disable
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.ServicesInterfaces;
using Domain.Common;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

public class GroupDialog : Form
{
    private readonly IGroupService _groupService;
    private readonly ISubjectGradeHandlerService _handlerService;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int? GroupId { get; set; }

    private StyledTextBox _txtName;
    private StyledComboBox _cmbHandler;
    private DateTimePicker _dtpFirst;
    private RoundedButton _btnSave;
    private GhostButton _btnCancel;
    private Label _lblTitle;
    private Label _lblError;

    public GroupDialog(IGroupService groupService, ISubjectGradeHandlerService handlerService)
    {
        _groupService = groupService;
        _handlerService = handlerService;

        BackColor = AppTheme.CardBg;

        InitUI();
        Load += async (s, e) => await OnLoadAsync();
    }

    private void InitUI()
    {
        Text = "Group";
        Size = new Size(440, 400);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        _lblTitle = new Label
        {
            Text = "Group Details",
            Font = AppTheme.FontH2,
            ForeColor = AppTheme.TextPrimary,
            AutoSize = true,
            Location = new Point(24, 18)
        };

        var l1 = MakeLabel("Group Name *", 24, 64);
        _txtName = new StyledTextBox { Width = 380, Height = AppTheme.InputHeight, Location = new Point(24, 84) };

        var l2 = MakeLabel("Subject / Grade *", 24, 134);
        _cmbHandler = new StyledComboBox { Width = 380, Location = new Point(24, 154) };

        var l3 = MakeLabel("First Session Date *", 24, 200);
        _dtpFirst = new DateTimePicker
        {
            Width = 380,
            Location = new Point(24, 220),
            Format = DateTimePickerFormat.Short,
            MinDate = DateTime.Today
        };

        _lblError = new Label
        {
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.Danger,
            AutoSize = false,
            Width = 380,
            Height = 18,
            Location = new Point(24, 260)
        };

        _btnSave = new RoundedButton
        {
            Text = "Add Group",
            Width = 160,
            Height = AppTheme.ButtonHeight,
            Location = new Point(24, 300)
        };
        _btnSave.Click += async (s, e) => await SaveAsync();

        _btnCancel = new GhostButton
        {
            Text = "Cancel",
            Width = 120,
            Height = AppTheme.ButtonHeight,
            Location = new Point(200, 300)
        };
        _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { _lblTitle, l1, _txtName, l2, _cmbHandler, l3, _dtpFirst, _lblError, _btnSave, _btnCancel });
    }

    private async Task OnLoadAsync()
    {
        // Update title and button now that GroupId is set
        bool isEdit = GroupId.HasValue;
        Text = isEdit ? "Edit Group" : "Add Group";
        _lblTitle.Text = isEdit ? "Edit Group" : "Group Details";
        _btnSave.Text = isEdit ? "Save Changes" : "Add Group";

        // Load handlers
        var r = await _handlerService.GetAllAsync();
        if (r.IsSuccess)
        {
            _cmbHandler.Items.Clear();
            foreach (var h in r.Value) _cmbHandler.Items.Add(new HandlerItem(h));
            if (_cmbHandler.Items.Count > 0) _cmbHandler.SelectedIndex = 0;
        }

        // Populate fields if editing
        if (isEdit)
        {
            _dtpFirst.MinDate = DateTimePicker.MinimumDateTime; // allow past dates when editing
            var gr = await _groupService.GetByIdAsync(GroupId.Value);
            if (gr.IsSuccess)
            {
                var g = gr.Value;
                _txtName.Text = g.Name;
                _dtpFirst.Value = g.FirstSessionDate;

                // Select matching handler in combobox
                foreach (var item in _cmbHandler.Items)
                    if (item is HandlerItem hi && hi.Handler.Id == g.SubjectGradeHandlerId)
                    { _cmbHandler.SelectedItem = item; break; }
            }
        }
    }

    private sealed class HandlerItem
    {
        public SubjectGradeHandler Handler { get; }
        public HandlerItem(SubjectGradeHandler h) => Handler = h;
        public override string ToString() =>
            $"{Handler.Subject?.Name ?? "?"} – {Handler.Grade?.Name ?? "?"} ({Handler.SessionFees:C})";
    }

    private async Task SaveAsync()
    {
        _lblError.Text = "";

        if (string.IsNullOrWhiteSpace(_txtName.Text))
        { _lblError.Text = "Group name required."; return; }

        if (_cmbHandler.SelectedItem is not HandlerItem hi)
        { _lblError.Text = "Select a subject/grade."; return; }

        _btnSave.Enabled = false;

        Result<Group> r;
        if (GroupId.HasValue)
            r = await _groupService.UpdateAsync(GroupId.Value, _txtName.Text.Trim(), hi.Handler.Id, _dtpFirst.Value);
        else
            r = await _groupService.CreateAsync(_txtName.Text.Trim(), hi.Handler.Id, _dtpFirst.Value);

        _btnSave.Enabled = true;

        if (r.IsSuccess) { DialogResult = DialogResult.OK; Close(); }
        else _lblError.Text = r.ErrorMessage;
    }

    private static Label MakeLabel(string t, int x, int y) => new Label
    {
        Text = t,
        Font = AppTheme.FontLabelBold,
        ForeColor = AppTheme.TextSecondary,
        AutoSize = true,
        Location = new Point(x, y)
    };
}