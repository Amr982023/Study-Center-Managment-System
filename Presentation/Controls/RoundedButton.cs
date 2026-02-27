using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Windows.Forms;
using Presentation.Theme;
using System.ComponentModel;

namespace Presentation.Controls
{
    public class RoundedButton : Button
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = AppTheme.ButtonRadius;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color NormalColor { get; set; } = AppTheme.Tangerine;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverColor { get; set; } = AppTheme.TangerineHover;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color TextColor { get; set; } = Color.White;

        private bool _isHovered;
        private bool _isPressed;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = AppTheme.Tangerine;
            ForeColor = Color.White;
            Font = AppTheme.FontButton;
            Cursor = Cursors.Hand;
            Height = AppTheme.ButtonHeight;
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
        }

        // Remove the WS_BORDER and WS_EX_CLIENTEDGE window styles that
        // cause the OS to draw a sharp rectangle border underneath our custom paint.
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_BORDER = 0x00800000;
                const int WS_EX_CLIENTEDGE = 0x00000200;
                var cp = base.CreateParams;
                cp.Style &= ~WS_BORDER;
                cp.ExStyle &= ~WS_EX_CLIENTEDGE;
                return cp;
            }
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        // Suppress default background so no sharp rect flickers behind our shape
        protected override void OnPaintBackground(PaintEventArgs e) { }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Fill entire client rect with parent background color first to
            // erase any corner pixels left by the OS
            using var parentBg = new SolidBrush(Parent?.BackColor ?? Color.Transparent);
            e.Graphics.FillRectangle(parentBg, ClientRectangle);

            Color fill = _isPressed ? Color.FromArgb(200, 110, 0)
                       : _isHovered ? HoverColor
                       : NormalColor;

            using var path = RoundRect(ClientRectangle, CornerRadius);
            using var brush = new SolidBrush(fill);
            e.Graphics.FillPath(brush, path);

            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var tb = new SolidBrush(TextColor);
            e.Graphics.DrawString(Text, Font, tb, ClientRectangle, sf);
        }

        protected static GraphicsPath RoundRect(Rectangle r, int rad)
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
