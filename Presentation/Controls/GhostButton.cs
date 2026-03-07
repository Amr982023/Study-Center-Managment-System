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
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            NormalColor = AppTheme.CardBg;
            HoverColor = Color.FromArgb(20, 240, 138, 4);
            TextColor = AppTheme.Tangerine;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            PaintTransparentBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.Clear(Parent.BackColor);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            float borderWidth = 1.4f;

            var borderRect = new RectangleF(
                borderWidth,
                borderWidth,
                Width - borderWidth * 2,
                Height - borderWidth * 2
            );

            var fillRect = new RectangleF(
                borderWidth + 1,
                borderWidth + 1,
                Width - (borderWidth + 1) * 2,
                Height - (borderWidth + 1) * 2
            );

            using var borderPath = RoundedRect(borderRect, CornerRadius);
            using var fillPath = RoundedRect(fillRect, CornerRadius - 1);

            using var fillBrush = new SolidBrush(_hovered ? HoverColor : AppTheme.CardBg);
            using var borderPen = new Pen(AppTheme.Tangerine, borderWidth)
            {
                Alignment = PenAlignment.Inset
            };

            g.FillPath(fillBrush, fillPath);
            g.DrawPath(borderPen, borderPath);

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                ClientRectangle,
                TextColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine
            );
        }

        private GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            var path = new GraphicsPath();
            float d = radius * 2;

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}