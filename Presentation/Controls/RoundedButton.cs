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
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = AppTheme.FontButton;
            Cursor = Cursors.Hand;
            Height = AppTheme.ButtonHeight;
            // NO ControlStyles.Opaque — we need transparent corners
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
        }

        // Strip OS border/edge styles that draw a sharp rectangle under our shape
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

        // Paint parent content into our background first so corners are truly transparent
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            PaintTransparentBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Color fill = _isPressed ? Color.FromArgb(200, 110, 0)
                       : _isHovered ? HoverColor
                       : NormalColor;

            using var path = RoundRect(ClientRectangle, CornerRadius);
            using var brush = new SolidBrush(fill);
            g.FillPath(brush, path);

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            using var tb = new SolidBrush(Enabled ? TextColor : Color.FromArgb(140, TextColor));
            g.DrawString(Text, Font, tb, (RectangleF)ClientRectangle, sf);
        }

        // Fills the corners with the parent's background so rounded edges are invisible.
        // Simple and stack-safe: just sample the parent background — no recursion.
        protected void PaintTransparentBackground(PaintEventArgs e)
        {
            if (Parent == null) return;
            using var bg = new SolidBrush(Parent.BackColor);
            e.Graphics.FillRectangle(bg, ClientRectangle);
        }

        protected static GraphicsPath RoundRect(Rectangle r, int rad)
        {
            var p = new GraphicsPath();
            int d = rad * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }


}
