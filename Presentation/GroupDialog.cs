#nullable disable
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;
using System.ComponentModel;
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
    private Label _lblError;

    public GroupDialog(IGroupService groupService, ISubjectGradeHandlerService handlerService)
    {
        _groupService = groupService;
        _handlerService = handlerService;
        InitUI();
    }

    private void InitUI()
    {
        Text = GroupId.HasValue ? "Edit Group" : "Add Group";
        Size = new Size(440, 360);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.CardBg;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var lblTitle = new Label
        {
            Text = "Group Details",
            Font = AppTheme.FontH2,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
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
            BackColor = Color.Transparent,
            AutoSize = false,
            Width = 380,
            Height = 18,
            Location = new Point(24, 262)
        };

        _btnSave = new RoundedButton
        {
            Text = GroupId.HasValue ? "Save Changes" : "Add Group",
            Width = 160,
            Height = AppTheme.ButtonHeight,
            Location = new Point(24, 284)
        };
        _btnSave.Click += async (s, e) => await SaveAsync();

        _btnCancel = new GhostButton
        {
            Text = "Cancel",
            Width = 100,
            Height = AppTheme.ButtonHeight,
            Location = new Point(196, 284)
        };
        _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[]
        {
                lblTitle, l1, _txtName, l2, _cmbHandler, l3, _dtpFirst, _lblError, _btnSave, _btnCancel
        });

        Load += async (s, e) => await OnLoadAsync();
    }

    private async Task OnLoadAsync()
    {
        var r = await _handlerService.GetAllAsync();
        if (r.IsSuccess)
        {
            foreach (var h in r.Value)
                _cmbHandler.Items.Add(h);
            _cmbHandler.DisplayMember = "Name";
            if (_cmbHandler.Items.Count > 0)
                _cmbHandler.SelectedIndex = 0;
        }
    }

    private async Task SaveAsync()
    {
        _lblError.Text = "";

        if (string.IsNullOrWhiteSpace(_txtName.Text))
        { _lblError.Text = "Group name required."; return; }

        if (_cmbHandler.SelectedItem is not SubjectGradeHandler h)
        { _lblError.Text = "Select a subject/grade."; return; }

        _btnSave.Enabled = false;
        var r = await _groupService.CreateAsync(_txtName.Text.Trim(), h.Id, _dtpFirst.Value);
        _btnSave.Enabled = true;

        if (r.IsSuccess) { DialogResult = DialogResult.OK; Close(); }
        else _lblError.Text = r.ErrorMessage;
    }

    private static Label MakeLabel(string t, int x, int y) => new Label
    {
        Text = t,
        Font = AppTheme.FontLabelBold,
        ForeColor = AppTheme.TextSecondary,
        BackColor = Color.Transparent,
        AutoSize = true,
        Location = new Point(x, y)
    };
}

