#nullable disable
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application.ServicesInterfaces;
using Presentation.Controls;
using Presentation.Theme;
using System.ComponentModel;
using Domain.Models;

namespace Presentation.Forms
{
    public class StudentDialog : Form
    {
        private readonly IStudentService _studentService;
        private readonly IGradeService _gradeService;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int? StudentId { get; set; }

        private StyledTextBox _txtFirstName;
        private StyledTextBox _txtMidName;
        private StyledTextBox _txtLastName;
        private StyledTextBox _txtPhone;
        private StyledTextBox _txtGuardianPhone;
        private StyledTextBox _txtCode;
        private StyledComboBox _cmbGender;
        private StyledComboBox _cmbGrade;
        private RoundedButton _btnSave;
        private GhostButton _btnCancel;
        private Label _lblError;

        public StudentDialog(IStudentService studentService, IGradeService gradeService)
        {
            _studentService = studentService;
            _gradeService = gradeService;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = StudentId.HasValue ? "Edit Student" : "Add New Student";
            Size = new Size(520, 520);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = AppTheme.CardBg;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblTitle = new Label { Text = StudentId.HasValue ? "Edit Student" : "Add New Student", Font = AppTheme.FontH2, ForeColor = AppTheme.TextPrimary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(24, 20) };
            var divider = new Panel { Height = 1, Width = 472, BackColor = AppTheme.Border, Location = new Point(24, 56) };

            int c1 = 24, c2 = 272;
            int r1 = 74, r2 = 150, r3 = 226, r4 = 302;

            var l1 = L("First Name *", c1, r1); _txtFirstName = TB(c1, r1, 200);
            var l2 = L("Middle Name", c2, r1); _txtMidName = TB(c2, r1, 200);
            var l3 = L("Last Name *", c1, r2); _txtLastName = TB(c1, r2, 200);
            var l4 = L("Student Code *", c2, r2); _txtCode = TB(c2, r2, 200);
            var l5 = L("Personal Phone *", c1, r3); _txtPhone = TB(c1, r3, 200);
            var l6 = L("Guardian Phone", c2, r3); _txtGuardianPhone = TB(c2, r3, 200);

            var l7 = L("Grade *", c1, r4);
            _cmbGrade = new StyledComboBox { Width = 200, Location = new Point(c1, r4 + 22) };

            var l8 = L("Gender *", c2, r4);
            _cmbGender = new StyledComboBox { Width = 200, Location = new Point(c2, r4 + 22) };
            _cmbGender.Items.AddRange(new object[] { "Male", "Female" });
            _cmbGender.SelectedIndex = 0;

            _lblError = new Label { Font = AppTheme.FontSmall, ForeColor = AppTheme.Danger, BackColor = Color.Transparent, AutoSize = false, Width = 460, Height = 18, Location = new Point(24, 390), TextAlign = ContentAlignment.MiddleLeft };

            _btnSave = new RoundedButton { Text = StudentId.HasValue ? "Save Changes" : "Add Student", Width = 160, Height = AppTheme.ButtonHeight, Location = new Point(24, 416) };
            _btnSave.Click += async (s, e) => await SaveAsync();

            _btnCancel = new GhostButton { Text = "Cancel", Width = 110, Height = AppTheme.ButtonHeight, Location = new Point(196, 416) };
            _btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblTitle, divider, l1, _txtFirstName, l2, _txtMidName, l3, _txtLastName, l4, _txtCode, l5, _txtPhone, l6, _txtGuardianPhone, l7, _cmbGrade, l8, _cmbGender, _lblError, _btnSave, _btnCancel });

            Load += async (s, e) => await OnLoadAsync();
        }

        private async Task OnLoadAsync()
        {
            var grades = await _gradeService.GetAllAsync();
            if (grades.IsSuccess)
            {
                _cmbGrade.Items.Clear();
                foreach (var g in grades.Value) _cmbGrade.Items.Add(g);
                _cmbGrade.DisplayMember = "Name";
                if (_cmbGrade.Items.Count > 0) _cmbGrade.SelectedIndex = 0;
            }

            if (StudentId.HasValue)
            {
                var r = await _studentService.GetByIdAsync(StudentId.Value);
                if (r.IsSuccess)
                {
                    var s = r.Value;
                    _txtFirstName.Text = s.FirstName;
                    _txtMidName.Text = s.MidName ?? "";
                    _txtLastName.Text = s.LastName;
                    _txtCode.Text = s.Code;
                    _txtPhone.Text = s.PersonalPhone;
                    _txtGuardianPhone.Text = s.GuardianPhone;
                    _cmbGender.SelectedItem = s.Gender;
                    foreach (var item in _cmbGrade.Items)
                        if (item is Grade g && g.Id == s.Grade?.Id) { _cmbGrade.SelectedItem = g; break; }
                }
            }
        }

        private async Task SaveAsync()
        {
            _lblError.Text = "";
            if (string.IsNullOrWhiteSpace(_txtFirstName.Text) || string.IsNullOrWhiteSpace(_txtLastName.Text) ||
                string.IsNullOrWhiteSpace(_txtCode.Text) || string.IsNullOrWhiteSpace(_txtPhone.Text))
            { _lblError.Text = "Please fill in all required fields (*)."; return; }

            if (_cmbGrade.SelectedItem is not Grade grade)
            { _lblError.Text = "Please select a grade."; return; }

            _btnSave.Enabled = false;
            try
            {
                if (StudentId.HasValue)
                {
                    _lblError.Text = "Edit not yet wired — implement UpdateAsync in service.";
                    return;
                }

                var r = await _studentService.CreateAsync(
                    _txtFirstName.Text.Trim(), _txtLastName.Text.Trim(),
                    _txtPhone.Text.Trim(), _cmbGender.SelectedItem?.ToString() ?? "Male",
                    _txtCode.Text.Trim(), _txtGuardianPhone.Text.Trim(), grade.Id,
                    string.IsNullOrWhiteSpace(_txtMidName.Text) ? null : _txtMidName.Text.Trim());

                if (!r.IsSuccess) { _lblError.Text = r.ErrorMessage; return; }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { _lblError.Text = "Unexpected error: " + ex.Message; }
            finally { _btnSave.Enabled = true; }
        }

        private static StyledTextBox TB(int x, int y, int w) =>
            new StyledTextBox { Width = w, Height = AppTheme.InputHeight, Location = new Point(x, y + 22) };

        private static Label L(string text, int x, int y) =>
            new Label { Text = text, Font = AppTheme.FontLabelBold, ForeColor = AppTheme.TextSecondary, BackColor = Color.Transparent, AutoSize = true, Location = new Point(x, y) };
    }
}