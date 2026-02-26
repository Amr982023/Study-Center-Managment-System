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
    public class StatusBadge : Label
    {
        public enum BadgeType { Success, Warning, Danger, Info, Default }

        private BadgeType _type = BadgeType.Default;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BadgeType Type
        {
            get => _type;
            set
            {
                _type = value;
                BackColor = value switch
                {
                    BadgeType.Success => Color.FromArgb(40, 52, 211, 153),
                    BadgeType.Warning => Color.FromArgb(40, 240, 138, 4),
                    BadgeType.Danger => Color.FromArgb(40, 248, 113, 113),
                    BadgeType.Info => Color.FromArgb(40, 56, 189, 248),
                    _ => Color.FromArgb(40, 100, 140, 160),
                };
                ForeColor = value switch
                {
                    BadgeType.Success => AppTheme.Success,
                    BadgeType.Warning => AppTheme.Tangerine,
                    BadgeType.Danger => AppTheme.Danger,
                    BadgeType.Info => AppTheme.Info,
                    _ => AppTheme.TextSecondary,
                };
            }
        }

        public StatusBadge()
        {
            Font = AppTheme.FontSmall;
            AutoSize = true;
            Padding = new Padding(8, 3, 8, 3);
            Type = BadgeType.Default;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var bg = new SolidBrush(BackColor);
            using var path = Pill(ClientRectangle);
            e.Graphics.FillPath(bg, path);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var tb = new SolidBrush(ForeColor);
            e.Graphics.DrawString(Text, Font, tb, ClientRectangle, sf);
        }

        private static GraphicsPath Pill(Rectangle r)
        {
            int rad = r.Height / 2;
            var p = new GraphicsPath(); int d = rad * 2;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure(); return p;
        }
    }
}
