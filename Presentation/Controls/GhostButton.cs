using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class GhostButton : RoundedButton
    {
        private bool _hovered;

        public GhostButton()
        {
            NormalColor = Color.Transparent;
            HoverColor = Color.FromArgb(30, 240, 138, 4);
            TextColor = AppTheme.Tangerine;
        }

        protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hovered = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Parent?.BackColor ?? BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = RoundRect(ClientRectangle, CornerRadius);
            using var fillBrush = new SolidBrush(_hovered ? HoverColor : NormalColor);
            using var borderPen = new Pen(AppTheme.Tangerine, 1.5f);
            e.Graphics.FillPath(fillBrush, path);
            e.Graphics.DrawPath(borderPen, path);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var tb = new SolidBrush(TextColor);
            e.Graphics.DrawString(Text, Font, tb, ClientRectangle, sf);
        }
    }
}