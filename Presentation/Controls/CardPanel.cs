using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(1, 1, Width - 2, Height - 2);
            using var path = RoundedPath(rect, CornerRadius);
            using var bg = new SolidBrush(CardColor);
            e.Graphics.FillPath(bg, path);
            if (ShowBorder)
            {
                using var pen = new Pen(AppTheme.Border, 1f);
                e.Graphics.DrawPath(pen, path);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) { }

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
