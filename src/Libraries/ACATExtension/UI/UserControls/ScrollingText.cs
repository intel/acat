using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.UserControls
{
    [DesignerCategory("Code")]
    public partial class ScrollingText : UserControl
    {
        private readonly Timer _timer;
        private int _textX;
        private int _textWidth;

        private string _displayText = "Scrolling Text";
        private int _scrollSpeed = 2;

        public ScrollingText()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            _timer = new Timer { Interval = 30 }; // ~33 FPS
            _timer.Tick += Timer_Tick;

            // Recompute measurements when things that affect size change
            FontChanged += (s, e) => UpdateTextMetrics();
            Resize += (s, e) => UpdateTextMetrics();
            VisibleChanged += (s, e) => UpdateTimerState();
            HandleCreated += (s, e) => UpdateTimerState();

            // Don't start timer here; handle creation & visibility determine it.
        }

        private bool IsDesignTime =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode;

        private void UpdateTimerState()
        {
            if (IsDesignTime) { _timer.Enabled = false; return; }
            _timer.Enabled = Visible && IsHandleCreated;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_textWidth <= 0 || Width <= 0) return;

            _textX -= Math.Max(1, _scrollSpeed);
            if (_textX < -_textWidth)
                _textX = Width;

            Invalidate();
        }

        private void UpdateTextMetrics()
        {
            if (string.IsNullOrEmpty(_displayText) || Font == null)
            {
                _textWidth = 0;
                _textX = Width;
                return;
            }

            // Use TextRenderer for WinForms measurement; NoPadding tends to be visually correct
            _textWidth = TextRenderer.MeasureText(_displayText, Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding | TextFormatFlags.SingleLine).Width;

            // If we don't have a sensible _textX yet, start at right edge
            if (_textX == 0 || _textX > Width)
                _textX = Width;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(BackColor);

            if (string.IsNullOrEmpty(_displayText))
                return;

            var textSize = TextRenderer.MeasureText(_displayText, Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);
            int y = (Height - textSize.Height) / 2;

            TextRenderer.DrawText(e.Graphics, _displayText, Font, new Point(_textX, Math.Max(0, y)), ForeColor,
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
        }

        #region Properties

        [Browsable(true)]
        [Category("Appearance")]
        [Description("The text to scroll.")]
        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value ?? string.Empty;
                UpdateTextMetrics();
            }
        }

        [Browsable(true)]
        [Category("Behavior")]
        [Description("Pixels per timer tick to move the text.")]
        public int ScrollSpeed
        {
            get => _scrollSpeed;
            set => _scrollSpeed = Math.Max(1, value);
        }

        #endregion

        // Designer file (InitializeComponent) remains unchanged from the earlier version.
    }
}