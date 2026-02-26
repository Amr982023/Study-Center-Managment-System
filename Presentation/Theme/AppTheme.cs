using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Theme
{
    public static class AppTheme
    {
        // ── Brand Colors ──────────────────────────────────
        // #274B59  Blue Dianne  — primary background / sidebar
        // #F08A04  Tangerine    — accent, active states, titles
        public static readonly Color BlueDianne = Color.FromArgb(39, 75, 89);
        public static readonly Color BlueDialneDark = Color.FromArgb(25, 52, 63);   // darker variant for sidebar
        public static readonly Color BlueDianneLight = Color.FromArgb(52, 98, 116);   // lighter variant for cards/topbar
        public static readonly Color Tangerine = Color.FromArgb(240, 138, 4);
        public static readonly Color TangerineHover = Color.FromArgb(255, 158, 30);
        public static readonly Color TangerineDim = Color.FromArgb(80, 240, 138, 4); // semi-transparent for hover bg

        // ── Backgrounds ──────────────────────────────────
        public static readonly Color SidebarBg = BlueDialneDark;              // #19343F
        public static readonly Color MainBg = Color.FromArgb(20, 45, 55); // between dark & BlueDianne
        public static readonly Color CardBg = BlueDianne;                  // #274B59
        public static readonly Color CardHover = BlueDianneLight;
        public static readonly Color InputBg = Color.FromArgb(25, 55, 66);
        public static readonly Color TableRowAlt = Color.FromArgb(22, 50, 60);
        public static readonly Color TopbarBg = BlueDialneDark;

        // ── Accent = Tangerine ────────────────────────────
        public static readonly Color Accent = Tangerine;
        public static readonly Color AccentHover = TangerineHover;
        public static readonly Color AccentLight = Color.FromArgb(255, 185, 80);  // light tangerine
        public static readonly Color AccentGlow = TangerineDim;

        // ── Nav active state ──────────────────────────────
        public static readonly Color NavActiveBg = Color.FromArgb(60, 240, 138, 4);  // tangerine tint bg
        public static readonly Color NavActiveText = Tangerine;
        public static readonly Color NavHoverBg = Color.FromArgb(30, 240, 138, 4);

        // ── Semantic ─────────────────────────────────────
        public static readonly Color Success = Color.FromArgb(52, 211, 153);
        public static readonly Color Warning = Color.FromArgb(251, 191, 36);
        public static readonly Color Danger = Color.FromArgb(248, 113, 113);
        public static readonly Color Info = Color.FromArgb(56, 189, 248);

        // ── Text — all white-based for readability ────────
        public static readonly Color TextPrimary = Color.FromArgb(255, 255, 255);   // pure white
        public static readonly Color TextSecondary = Color.FromArgb(200, 220, 228);   // soft white-blue
        public static readonly Color TextMuted = Color.FromArgb(140, 175, 190);   // muted teal-white
        public static readonly Color TextOnAccent = Color.White;

        // ── Borders ──────────────────────────────────────
        public static readonly Color Border = Color.FromArgb(52, 98, 116);    // BlueDianneLight
        public static readonly Color BorderFocus = Tangerine;

        // ── Fonts — bigger & more readable ───────────────
        public static readonly Font FontTitle = new Font("Segoe UI", 22f, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 14f, FontStyle.Regular);
        public static readonly Font FontH2 = new Font("Segoe UI", 17f, FontStyle.Bold);
        public static readonly Font FontH3 = new Font("Segoe UI", 14f, FontStyle.Bold);
        public static readonly Font FontLabel = new Font("Segoe UI", 11f, FontStyle.Regular);
        public static readonly Font FontLabelBold = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontInput = new Font("Segoe UI", 11f, FontStyle.Regular);
        public static readonly Font FontButton = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontNav = new Font("Segoe UI", 12f, FontStyle.Regular);
        public static readonly Font FontNavBold = new Font("Segoe UI", 12f, FontStyle.Bold);
        public static readonly Font FontSmall = new Font("Segoe UI", 10f, FontStyle.Regular);

        // ── Sizes ─────────────────────────────────────────
        public const int SidebarWidth = 250;
        public const int NavItemHeight = 52;
        public const int InputHeight = 40;
        public const int ButtonHeight = 40;
        public const int CardRadius = 12;
        public const int InputRadius = 8;
        public const int ButtonRadius = 8;
        public const int TopbarHeight = 62;
    }
}

