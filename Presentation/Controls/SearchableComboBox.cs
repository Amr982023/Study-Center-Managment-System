#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Presentation.Theme;
using System.ComponentModel;

namespace Presentation.Controls
{
    /// <summary>
    /// A searchable dropdown: StyledTextBox on top + floating ListBox popup below.
    /// Usage:
    ///   var cmb = new SearchableComboBox();
    ///   cmb.SetItems(list, item => item.ToString());
    ///   cmb.SelectedItemChanged += (s, e) => { var item = cmb.SelectedItem; };
    /// </summary>
    public class SearchableComboBox : Panel
    {
        // ── Events ────────────────────────────────────────────────────────────
        public event EventHandler SelectedItemChanged;

        // ── Private fields ────────────────────────────────────────────────────
        private readonly StyledTextBox _txtSearch;
        private readonly ListBox _listBox;
        private readonly Form _popup;

        private List<object> _allItems = new();
        private Func<object, string> _displayFunc = o => o?.ToString() ?? "";
        private object _selectedItem;
        private bool _suppressEvents;

        // ── Public API ────────────────────────────────────────────────────────
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        // ── Public API ────────────────────────────────────────────────────────
        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                _suppressEvents = true;
                _txtSearch.Text = value == null ? "" : _displayFunc(value);
                _suppressEvents = false;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PlaceholderText
        {
            get => _txtSearch.Placeholder;
            set => _txtSearch.Placeholder = value;
        }

        /// <summary>Load items into the control.</summary>
        public void SetItems<T>(IEnumerable<T> items, Func<T, string> display = null)
        {
            _allItems = items.Cast<object>().ToList();
            _displayFunc = display != null
                ? o => display((T)o)
                : o => o?.ToString() ?? "";

            _selectedItem = null;
            _txtSearch.Text = "";
        }

        /// <summary>Clear all items and reset.</summary>
        public void Clear()
        {
            _allItems.Clear();
            _selectedItem = null;
            _txtSearch.Text = "";
            _listBox.Items.Clear();
        }

        // ── Constructor ───────────────────────────────────────────────────────
        public SearchableComboBox()
        {
            Height = AppTheme.InputHeight;
            BackColor = Color.Transparent;

            // ── Search box ────────────────────────────────────────────────────
            _txtSearch = new StyledTextBox
            {
                Dock = DockStyle.Fill,
                Placeholder = "Search...",
            };
            _txtSearch.Inner.TextChanged += OnSearchTextChanged;
            _txtSearch.Inner.KeyDown += OnKeyDown;
            _txtSearch.Inner.LostFocus += (s, e) =>
            {
                // Only hide if focus didn't move to the popup or listbox
                var t = new System.Windows.Forms.Timer { Interval = 200 };
                t.Tick += (ts, te) =>
                {
                    t.Stop(); t.Dispose();
                    var focused = Form.ActiveForm;
                    if (focused == null || focused == _popup) return;
                    HidePopup();
                };
                t.Start();
            };

            Controls.Add(_txtSearch);

            // ── Floating popup listbox ────────────────────────────────────────
            _listBox = new ListBox
            {
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.FontInput,
                BorderStyle = BorderStyle.None,
                SelectionMode = SelectionMode.One,
                ItemHeight = 28,
            };
            _listBox.MouseClick += OnListBoxClick;
            _listBox.KeyDown += OnKeyDown;

            // Borderless popup form to host the listbox
            _popup = new NoActivateForm
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                BackColor = AppTheme.Border,
                Padding = new Padding(1),
            };
            _popup.Controls.Add(_listBox);
            _listBox.Dock = DockStyle.Fill;
        }

        // ── Search logic ──────────────────────────────────────────────────────
        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            if (_suppressEvents) return;

            // If user clears text — reset selection
            if (string.IsNullOrWhiteSpace(_txtSearch.Text))
            {
                _selectedItem = null;
                SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            }

            RefreshList();
            ShowPopup();
            // Always keep focus in the search box while typing
            _txtSearch.Inner.Focus();
        }

        private void RefreshList()
        {
            var query = _txtSearch.Text.Trim().ToLower();
            var filtered = string.IsNullOrWhiteSpace(query)
                ? _allItems
                : _allItems.Where(i => _displayFunc(i).ToLower().Contains(query)).ToList();

            _listBox.BeginUpdate();
            _listBox.Items.Clear();
            foreach (var item in filtered)
                _listBox.Items.Add(item);
            _listBox.EndUpdate();

            // Auto-select first item for keyboard nav
            if (_listBox.Items.Count > 0)
                _listBox.SelectedIndex = 0;
        }

        // ── Popup show/hide ───────────────────────────────────────────────────
        private void ShowPopup()
        {
            if (_listBox.Items.Count == 0) { HidePopup(); return; }

            var screen = PointToScreen(new Point(0, Height));
            int popupH = Math.Min(_listBox.Items.Count * 28 + 4, 200);

            _popup.Location = screen;
            _popup.Size = new Size(Width, popupH);

            if (!_popup.Visible)
            {
                _popup.Show(FindForm());
                _txtSearch.Inner.Focus();   // reclaim focus immediately after Show
            }
        }

        private void HidePopup()
        {
            if (_popup.Visible) _popup.Hide();
        }

        // ── Selection ─────────────────────────────────────────────────────────
        private void OnListBoxClick(object sender, MouseEventArgs e)
        {
            SelectCurrent();
        }

        private void SelectCurrent()
        {
            if (_listBox.SelectedItem == null) return;

            _selectedItem = _listBox.SelectedItem;
            _suppressEvents = true;
            _txtSearch.Text = _displayFunc(_selectedItem);
            _suppressEvents = false;

            HidePopup();
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            _txtSearch.Inner.Focus();
        }

        // ── Keyboard navigation ───────────────────────────────────────────────
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (!_popup.Visible) { RefreshList(); ShowPopup(); }
                    else if (_listBox.Items.Count > 0)
                        _listBox.SelectedIndex = Math.Min(_listBox.SelectedIndex + 1, _listBox.Items.Count - 1);
                    e.Handled = true;
                    break;

                case Keys.Up:
                    if (_listBox.Items.Count > 0)
                        _listBox.SelectedIndex = Math.Max(_listBox.SelectedIndex - 1, 0);
                    e.Handled = true;
                    break;

                case Keys.Enter:
                    SelectCurrent();
                    e.Handled = true;
                    break;

                case Keys.Escape:
                    HidePopup();
                    e.Handled = true;
                    break;
            }
        }

        // ── Draw drop arrow ───────────────────────────────────────────────────
        protected override void OnPaint(PaintEventArgs e) { }   // StyledTextBox handles its own paint

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_txtSearch == null) return;
            _txtSearch.Size = new Size(Width, Height);
        }
    }

    /// <summary>Popup form that never steals focus from the search textbox.</summary>
    internal class NoActivateForm : Form
    {
        protected override bool ShowWithoutActivation => true;
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                return cp;
            }
        }
    }
}