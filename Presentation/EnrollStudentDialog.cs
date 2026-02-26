#nullable disable
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;
using System.ComponentModel;

public class EnrollStudentDialog : Form
{
    private readonly IStudentGroupAggregationService _enrollService;
    private readonly IStudentService _studentService;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int GroupId { get; set; }

    private StyledComboBox _cmbStudent;
    private StyledDataGridView _gridEnrolled;
    private RoundedButton _btnEnroll;
    private DangerButton _btnUnenroll;
    private Label _lblError;

    public EnrollStudentDialog(
        IStudentGroupAggregationService enrollService,
        IStudentService studentService)
    {
        _enrollService = enrollService;
        _studentService = studentService;
        InitUI();
    }

    private void InitUI()
    {
        Text = "Manage Group Enrollment";
        Size = new Size(560, 480);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.CardBg;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var lblTitle = new Label
        {
            Text = "Enroll Students",
            Font = AppTheme.FontH2,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(24, 18)
        };

        var l1 = MakeLabel("Select Student", 24, 60);
        _cmbStudent = new StyledComboBox { Width = 380, Location = new Point(24, 80) };
        _cmbStudent.SelectedIndexChanged += async (s, e) => await Task.CompletedTask;

        _btnEnroll = new RoundedButton
        {
            Text = "Enroll",
            Width = 90,
            Height = AppTheme.ButtonHeight,
            Location = new Point(416, 80)
        };
        _btnEnroll.Click += async (s, e) => await EnrollAsync();

        var l2 = MakeLabel("Currently Enrolled", 24, 130);
        var card = new CardPanel { Location = new Point(24, 152), Width = 500, Height = 240 };

        _gridEnrolled = new StyledDataGridView { Dock = DockStyle.Fill };
        _gridEnrolled.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "SId", FillWeight = 15 },
            new DataGridViewTextBoxColumn { HeaderText = "Code", Name = "Code", FillWeight = 20 },
            new DataGridViewTextBoxColumn { HeaderText = "Name", Name = "Name", FillWeight = 50 }
        );
        card.Controls.Add(_gridEnrolled);

        _btnUnenroll = new DangerButton
        {
            Text = "Remove",
            Width = 100,
            Height = AppTheme.ButtonHeight,
            Location = new Point(24, 404),
            Enabled = false
        };
        _btnUnenroll.Click += async (s, e) => await UnenrollAsync();
        _gridEnrolled.SelectionChanged += (s, e) =>
            _btnUnenroll.Enabled = _gridEnrolled.SelectedRows.Count > 0;

        _lblError = new Label
        {
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.Danger,
            BackColor = Color.Transparent,
            AutoSize = false,
            Width = 500,
            Height = 18,
            Location = new Point(24, 388)
        };

        Controls.AddRange(new Control[]
        {
                lblTitle, l1, _cmbStudent, _btnEnroll, l2, card, _lblError, _btnUnenroll
        });

        Load += async (s, e) => await OnLoadAsync();
    }

    private async Task OnLoadAsync()
    {
        var sr = await _studentService.GetAllAsync();
        if (sr.IsSuccess)
        {
            foreach (var s in sr.Value)
                _cmbStudent.Items.Add(s);
            _cmbStudent.DisplayMember = "FullName";
            if (_cmbStudent.Items.Count > 0)
                _cmbStudent.SelectedIndex = 0;
        }
        await RefreshEnrolledAsync();
    }

    private async Task RefreshEnrolledAsync()
    {
        var r = await _enrollService.GetByGroupAsync(GroupId);
        _gridEnrolled.Rows.Clear();
        if (r.IsSuccess)
            foreach (var e in r.Value)
                _gridEnrolled.Rows.Add(
                    e.Student?.Id,
                    e.Student?.Code,
                    $"{e.Student?.FirstName} {e.Student?.LastName}");
    }

    private async Task EnrollAsync()
    {
        _lblError.Text = "";
        if (_cmbStudent.SelectedItem is not Student s) return;

        var r = await _enrollService.EnrollAsync(s.Id, GroupId);
        if (r.IsSuccess) await RefreshEnrolledAsync();
        else _lblError.Text = r.ErrorMessage;
    }

    private async Task UnenrollAsync()
    {
        if (_gridEnrolled.SelectedRows.Count == 0) return;
        var sid = (int)_gridEnrolled.SelectedRows[0].Cells["SId"].Value;

        var r = await _enrollService.UnenrollAsync(sid, GroupId);
        if (r.IsSuccess) await RefreshEnrolledAsync();
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
