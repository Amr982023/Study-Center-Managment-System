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

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int GradeId { get; set; }

    private StyledDataGridView _gridAvailable;
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
        Size = new Size(900, 600);
        MinimumSize = new Size(700, 500);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = AppTheme.CardBg;
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = false;

        var lblTitle = new Label
        {
            Text = "Manage Group Enrollment",
            Font = AppTheme.FontH2,
            ForeColor = AppTheme.TextPrimary,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(24, 16)
        };

        _lblError = new Label
        {
            Font = AppTheme.FontSmall,
            ForeColor = AppTheme.Danger,
            BackColor = Color.Transparent,
            AutoSize = false,
            Height = 20,
            Dock = DockStyle.Bottom,
            Padding = new Padding(8, 0, 0, 0)
        };

        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 50, 16, 8),
            ColumnCount = 3,
            RowCount = 1
        };
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47f));
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120f));
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47f));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        // ── Left panel ───────────────────────────────────────────
        var leftPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
        leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var lblAvailable = MakeLabel("Available Students", 0, 0);
        lblAvailable.Dock = DockStyle.Fill;

        var cardLeft = new CardPanel { Dock = DockStyle.Fill };
        _gridAvailable = BuildGrid("AId", "ACode", "AName");
        cardLeft.Controls.Add(_gridAvailable);

        leftPanel.Controls.Add(lblAvailable, 0, 0);
        leftPanel.Controls.Add(cardLeft, 0, 1);

        // ── Middle buttons ───────────────────────────────────────
        var middlePanel = new Panel { Dock = DockStyle.Fill };

        _btnEnroll = new RoundedButton
        {
            Text = "Enroll →",
            Width = 100,
            Height = AppTheme.ButtonHeight,
            Enabled = false,
            Anchor = AnchorStyles.None
        };

        _btnUnenroll = new DangerButton
        {
            Text = "← Remove",
            Width = 100,
            Height = AppTheme.ButtonHeight,
            Enabled = false,
            Anchor = AnchorStyles.None
        };

        middlePanel.Controls.AddRange(new Control[] { _btnEnroll, _btnUnenroll });
        middlePanel.Resize += (s, e) =>
        {
            int cx = middlePanel.Width / 2;
            int cy = middlePanel.Height / 2;
            _btnEnroll.Location = new Point(cx - _btnEnroll.Width / 2, cy - _btnEnroll.Height - 6);
            _btnUnenroll.Location = new Point(cx - _btnUnenroll.Width / 2, cy + 6);
        };

        // ── Right panel ──────────────────────────────────────────
        var rightPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
        rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var lblEnrolled = MakeLabel("Currently Enrolled", 0, 0);
        lblEnrolled.Dock = DockStyle.Fill;

        var cardRight = new CardPanel { Dock = DockStyle.Fill };
        _gridEnrolled = BuildGrid("EId", "ECode", "EName");
        cardRight.Controls.Add(_gridEnrolled);

        rightPanel.Controls.Add(lblEnrolled, 0, 0);
        rightPanel.Controls.Add(cardRight, 0, 1);

        _btnEnroll.Click += async (s, e) => await EnrollSelectedAsync();
        _btnUnenroll.Click += async (s, e) => await UnenrollSelectedAsync();

        mainPanel.Controls.Add(leftPanel, 0, 0);
        mainPanel.Controls.Add(middlePanel, 1, 0);
        mainPanel.Controls.Add(rightPanel, 2, 0);

        Controls.AddRange(new Control[] { lblTitle, _lblError, mainPanel });

        Load += async (s, e) => await OnLoadAsync();
    }

    // ── Grid builder (instance method) ───────────────────────────
    private StyledDataGridView BuildGrid(string idCol, string codeCol, string nameCol)
    {
        var grid = new StyledDataGridView
        {
            Dock = DockStyle.Fill,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false
        };

        // ✅ ReadOnly = false so the checkbox column can be interacted with
        var chkCol = new DataGridViewCheckBoxColumn
        {
            Name = "Chk",
            HeaderText = "✓",
            Width = 36,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            FillWeight = 1,
            ReadOnly = false
        };

        grid.Columns.Add(chkCol);
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", Name = idCol, FillWeight = 15, ReadOnly = true });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Code", Name = codeCol, FillWeight = 25, ReadOnly = true });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", Name = nameCol, FillWeight = 60, ReadOnly = true });

        // ✅ Commit immediately on first click — no double click needed
        grid.CurrentCellDirtyStateChanged += (s, e) =>
        {
            if (grid.CurrentCell is DataGridViewCheckBoxCell)
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };

        // ✅ After commit, update button states
        grid.CellValueChanged += (s, e) =>
        {
            if (e.ColumnIndex == 0)
                UpdateButtonStates();
        };

        // ✅ Header checkbox click = select all / deselect all
        grid.CellMouseClick += (s, e) =>
        {
            if (e.ColumnIndex == 0 && e.RowIndex == -1)
            {
                bool newVal = !GetAllChecked(grid);
                foreach (DataGridViewRow row in grid.Rows)
                    row.Cells["Chk"].Value = newVal;

                grid.EndEdit();
                grid.Invalidate();
                UpdateButtonStates();
            }
        };

        return grid;
    }

    private void UpdateButtonStates()
    {
        _btnEnroll.Enabled = HasCheckedRows(_gridAvailable);
        _btnUnenroll.Enabled = HasCheckedRows(_gridEnrolled);
    }

    private static bool GetAllChecked(StyledDataGridView grid) =>
        grid.Rows.Count > 0 &&
        grid.Rows.Cast<DataGridViewRow>().All(r => r.Cells["Chk"].Value is true);

    private static bool HasCheckedRows(StyledDataGridView grid) =>
        grid.Rows.Cast<DataGridViewRow>().Any(r => r.Cells["Chk"].Value is true);

    private static IEnumerable<DataGridViewRow> GetCheckedRows(StyledDataGridView grid) =>
        grid.Rows.Cast<DataGridViewRow>().Where(r => r.Cells["Chk"].Value is true);

    // ── Load & refresh ───────────────────────────────────────────
    private async Task OnLoadAsync() => await RefreshBothGridsAsync();

    private async Task RefreshBothGridsAsync()
    {
        _lblError.Text = "";
        _gridAvailable.Rows.Clear();
        _gridEnrolled.Rows.Clear();

        var allStudentsResult = await _studentService.GetByGradeAsync(GradeId);
        var enrolledResult = await _enrollService.GetByGroupAsync(GroupId);

        if (!allStudentsResult.IsSuccess) { _lblError.Text = allStudentsResult.ErrorMessage; return; }
        if (!enrolledResult.IsSuccess) { _lblError.Text = enrolledResult.ErrorMessage; return; }

        var enrolledIds = enrolledResult.Value
            .Where(e => e.Student != null)
            .Select(e => e.Student!.Id)
            .ToHashSet();

        foreach (var s in allStudentsResult.Value)
        {
            var row = new object[] { false, s.Id, s.Code, $"{s.FirstName} {s.LastName}" };

            if (enrolledIds.Contains(s.Id))
                _gridEnrolled.Rows.Add(row);
            else
                _gridAvailable.Rows.Add(row);
        }

        _btnEnroll.Enabled = false;
        _btnUnenroll.Enabled = false;
    }

    private async Task EnrollSelectedAsync()
    {
        _lblError.Text = "";
        var rows = GetCheckedRows(_gridAvailable).ToList();

        foreach (var row in rows)
        {
            var sid = (int)row.Cells["AId"].Value;
            var r = await _enrollService.EnrollAsync(sid, GroupId);
            if (!r.IsSuccess) { _lblError.Text = r.ErrorMessage; break; }
        }

        await RefreshBothGridsAsync();
    }

    private async Task UnenrollSelectedAsync()
    {
        _lblError.Text = "";
        var rows = GetCheckedRows(_gridEnrolled).ToList();

        foreach (var row in rows)
        {
            var sid = (int)row.Cells["EId"].Value;
            var r = await _enrollService.UnenrollAsync(sid, GroupId);
            if (!r.IsSuccess) { _lblError.Text = r.ErrorMessage; break; }
        }

        await RefreshBothGridsAsync();
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