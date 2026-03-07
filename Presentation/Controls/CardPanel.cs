using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class CardPanel : Panel
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CornerRadius { get; set; } = AppTheme.CardRadius;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color CardColor { get; set; } = AppTheme.CardBg;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowBorder { get; set; } = true;

        public CardPanel()
        {
            BackColor = Color.Transparent;
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.ResizeRedraw,
                true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {
                using var path = RoundedPath(new Rectangle(0, 0, Width, Height), CornerRadius);
                Region = new Region(path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Paint parent background into corners so rounded edges look transparent
            if (Parent != null)
            {
                using var bg = new SolidBrush(Parent.BackColor);
                e.Graphics.FillRectangle(bg, ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighSpeed; // faster compositing
            g.InterpolationMode = InterpolationMode.Low;

            var rect = new Rectangle(1, 1, Width - 2, Height - 2);
            using var path = RoundedPath(rect, CornerRadius);
            using var bg = new SolidBrush(CardColor);
            g.FillPath(bg, path);

            if (ShowBorder)
            {
                using var pen = new Pen(AppTheme.Border, 1f);
                g.DrawPath(pen, path);
            }
        }

        // CRITICAL: don't propagate invalidation into children (the grid)
        // This stops CardPanel repaints from triggering grid repaints mid-update
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
        }

        private static GraphicsPath RoundedPath(Rectangle r, int rad)
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