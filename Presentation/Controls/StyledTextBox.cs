using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class StyledTextBox : Panel
    {
        private const int PAD = 12;   // horizontal inset so text stays inside rounded corners
        private const int RAD = 8;    // corner radius — must match AppTheme.InputRadius

        // Initialized inline so Inner is never null, even when properties are set
        // via object initializers before the constructor body runs.
        public TextBox Inner { get; } = new TextBox
        {
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(25, 55, 66),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 11f),
            // NO Dock — we position manually so it stays inside the rounded corners
        };

        private string _placeholder = "";
        private bool _isFocused;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Placeholder
        {
            get => _placeholder;
            set { _placeholder = value; if (Inner != null) Inner.PlaceholderText = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get => Inner?.Text ?? "";
            set { if (Inner != null) Inner.Text = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseSystemPasswordChar
        {
            get => Inner?.UseSystemPasswordChar ?? false;
            set { if (Inner != null) Inner.UseSystemPasswordChar = value; }
        }

        public StyledTextBox()
        {
            Height = AppTheme.InputHeight;
            BackColor = Color.Transparent;   // panel itself transparent — we draw in OnPaint

            Inner.BackColor = AppTheme.InputBg;
            Inner.Font = AppTheme.FontInput;

            Inner.GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
            Inner.LostFocus += (s, e) => { _isFocused = false; Invalidate(); };

            Controls.Add(Inner);
            PositionInner();

            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        }

        private void PositionInner()
        {
            // Keep inner textbox inset from edges so it never overlaps the rounded corners
            int innerH = Inner.PreferredHeight;
            Inner.SetBounds(PAD, (Height - innerH) / 2, Width - PAD * 2, innerH);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            PositionInner();

            // Clip the panel to the rounded rect so child controls can't bleed outside
            using var path = RoundedPath(new Rectangle(0, 0, Width, Height), RAD);
            Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Fill background
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedPath(rect, RAD);
            using var bg = new SolidBrush(AppTheme.InputBg);
            e.Graphics.FillPath(bg, path);

            // Border — tangerine when focused, subtle when not
            Color borderColor = _isFocused ? AppTheme.Tangerine : AppTheme.Border;
            float borderWidth = _isFocused ? 2f : 1.5f;
            using var pen = new Pen(borderColor, borderWidth);
            e.Graphics.DrawPath(pen, path);
        }

        private static GraphicsPath RoundedPath(Rectangle r, int rad)
        {
            var p = new GraphicsPath(); int d = rad * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure(); return p;
        }
    }


}