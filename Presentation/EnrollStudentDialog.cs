#nullable disable
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.Email;
using Application.ServicesInterfaces;
using Domain.Models;
using Presentation.Controls;
using Presentation.Theme;

public class EnrollStudentDialog : Form
{
    private readonly IStudentGroupAggregationService _enrollService;
    private readonly IStudentService _studentService;
    private readonly IGroupService _groupService;
    private readonly EmailNotificationService _emailService;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int GroupId { get; set; }

    private SearchableComboBox _cmbStudent;
    private StyledDataGridView _gridEnrolled;
    private RoundedButton _btnEnroll;
    private DangerButton _btnUnenroll;
    private Label _lblError;

    public EnrollStudentDialog(
        IStudentGroupAggregationService enrollService,
        IStudentService studentService,
        IGroupService groupService,
        EmailNotificationService emailService)
    {
        _enrollService = enrollService;
        _studentService = studentService;
        _groupService = groupService;
        _emailService = emailService;
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

        var lblTitle = new Label { Text = "Enroll Students", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 18) };

        var l1 = MakeLabel("Select Student", 24, 60);
        _cmbStudent = new SearchableComboBox { Width = 380, Height = AppTheme.InputHeight, Location = new Point(24, 80), PlaceholderText = "Search student..." };

        _btnEnroll = new RoundedButton { Text = "Enroll", Width = 90, Height = AppTheme.ButtonHeight, Location = new Point(416, 80) };
        _btnEnroll.Click += async (s, e) => await EnrollAsync();
        _cmbStudent.SelectedItemChanged += (s, e) => { };

        var l2 = MakeLabel("Currently Enrolled", 24, 130);
        var card = new CardPanel { Location = new Point(24, 152), Width = 500, Height = 240 };

        _gridEnrolled = new StyledDataGridView { Dock = DockStyle.Fill };
        _gridEnrolled.AutoGenerateColumns = false;
        _gridEnrolled.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "ID", Name = "SId", DataPropertyName = "SId", FillWeight = 15 },
            new DataGridViewTextBoxColumn { HeaderText = "Code", Name = "Code", DataPropertyName = "Code", FillWeight = 20 },
            new DataGridViewTextBoxColumn { HeaderText = "Name", Name = "Name", DataPropertyName = "Name", FillWeight = 50 }
        );
        card.Controls.Add(_gridEnrolled);

        _btnUnenroll = new DangerButton { Text = "Remove", Width = 100, Height = AppTheme.ButtonHeight, Location = new Point(24, 404), Enabled = false };
        _btnUnenroll.Click += async (s, e) => await UnenrollAsync();
        _gridEnrolled.SelectionChanged += (s, e) => _btnUnenroll.Enabled = _gridEnrolled.SelectedRows.Count > 0;

        _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 500, Height = 18, Location = new Point(24, 388) };

        Controls.AddRange(new Control[] { lblTitle, l1, _cmbStudent, _btnEnroll, l2, card, _lblError, _btnUnenroll });
        Load += async (s, e) => await OnLoadAsync();
    }

    private async Task OnLoadAsync()
    {
        var sr = await _studentService.GetAllAsync();
        if (sr.IsSuccess)
        {
            _cmbStudent.SetItems(
                sr.Value.Select(s => new StudentItem(s)),
                item => item.ToString());
        }
        await RefreshEnrolledAsync();
    }

    private sealed class StudentItem
    {
        public Student Student { get; }
        public StudentItem(Student s) => Student = s;
        public override string ToString() => $"{Student.FirstName} {Student.LastName} ({Student.Code})";
    }

    private async Task RefreshEnrolledAsync()
    {
        var r = await _enrollService.GetByGroupAsync(GroupId);
        if (!r.IsSuccess) return;

        var dt = new DataTable();
        dt.Columns.Add("SId", typeof(int));
        dt.Columns.Add("Code", typeof(string));
        dt.Columns.Add("Name", typeof(string));

        foreach (var e in r.Value)
            dt.Rows.Add(e.Student?.Id, e.Student?.Code, $"{e.Student?.FirstName} {e.Student?.LastName}");

        _gridEnrolled.DataSource = null;
        _gridEnrolled.DataSource = dt;
    }

    private async Task EnrollAsync()
    {
        _lblError.Text = "";
        if (_cmbStudent.SelectedItem is not StudentItem si) { _lblError.Text = "Select a student."; return; }

        var r = await _enrollService.EnrollAsync(si.Student.Id, GroupId);
        if (!r.IsSuccess) { _lblError.Text = r.ErrorMessage; return; }

        // Send enrollment confirmation — fetch group for the template
        var gr = await _groupService.GetByIdAsync(GroupId);
        if (gr.IsSuccess)
        {
            var (sent, error) = await _emailService.SendEnrollmentConfirmationAsync(si.Student, gr.Value);
            if (!sent && !string.IsNullOrWhiteSpace(error) && error != "Student has no email address.")
                _lblError.Text = $"Enrolled. Email failed: {error}";
        }

        await RefreshEnrolledAsync();
    }

    private async Task UnenrollAsync()
    {
        if (_gridEnrolled.SelectedRows.Count == 0) return;
        var drv = _gridEnrolled.SelectedRows[0].DataBoundItem as DataRowView;
        var sid = (int)drv["SId"];

        var r = await _enrollService.UnenrollAsync(sid, GroupId);
        if (r.IsSuccess) await RefreshEnrolledAsync();
        else _lblError.Text = r.ErrorMessage;
    }

    private static Label MakeLabel(string t, int x, int y) =>
        new Label { Text = t, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(x, y) };
}