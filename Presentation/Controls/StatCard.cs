using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presentation.Theme;

namespace Presentation.Controls
{
    public class StatCard : CardPanel
    {
        private readonly Label _lblValue;
        private readonly Label _lblTitle;
        private readonly Label _lblIcon;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title { get => _lblTitle.Text; set => _lblTitle.Text = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Value { get => _lblValue.Text; set => _lblValue.Text = value; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Icon { get => _lblIcon.Text; set => _lblIcon.Text = value; }

        public StatCard()
        {
            Height = 116;
            CardColor = AppTheme.BlueDianne;
            CornerRadius = 12;
            ShowBorder = true;

            _lblIcon = new Label
            {
                Font = new Font("Segoe UI Emoji", 26f),
                ForeColor = AppTheme.Tangerine,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(16, 18)
            };

            _lblValue = new Label
            {
                Font = new Font("Segoe UI", 28f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(16, 16)
            };

            _lblTitle = new Label
            {
                Font = AppTheme.FontLabel,
                ForeColor = AppTheme.TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(16, 64)
            };

            Controls.AddRange(new Control[] { _lblIcon, _lblValue, _lblTitle });
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_lblIcon == null) return;
            _lblIcon.Location = new Point(Width - 54, 16);
            _lblValue.Location = new Point(16, 18);
            _lblTitle.Location = new Point(16, 66);
        }
    }
}

