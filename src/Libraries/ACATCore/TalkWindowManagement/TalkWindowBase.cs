////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowBase.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.TalkWindowManagement
{
    [DescriptorAttribute("97D0FFE5-88C1-4185-9DF4-8BAE3F905BAF", "TalkWindowBase", "Base class for Talk Window")]
    public class TalkWindowBase : Form, ITalkWindow
    {
        /// <summary>
        /// By how much to increment/decrememt the font
        /// </summary>
        protected const float FontSizeDelta = 2.0f;

        /// <summary>
        /// maximum font size
        /// </summary>
        protected const float MaxFontSize = 72.0f;

        /// <summary>
        ///  minimum font size
        /// </summary>
        protected const float MinFontSize = 6.0f;

        /// <summary>
        /// Default date format to display in talk window status bar
        /// </summary>
        protected String defaultDateFormat = "ddd, MMM d, yyyy";

        /// <summary>
        /// Default font size of the text box
        /// </summary>
        protected float defaultFontSize;

        /// <summary>
        /// Default time format to display in talk window status bar
        /// </summary>
        protected String defaultTimeFormat = "h:mm:ss tt";

        /// <summary>
        /// Date format to display in talk window status bar
        /// </summary>
        protected String displayDateFormat = String.Empty;

        /// <summary>
        /// Time format to display in talk window status bar
        /// </summary>
        protected String displayTimeFormat = String.Empty;

        /// <summary>
        /// Should date/time be displayed?
        /// </summary>
        protected bool enableDateTimeDisplay = false;

        /// <summary>
        /// Synchronization object
        /// </summary>
        private readonly SyncLock _syncObj;

        /// <summary>
        /// TalkWindow must have a text box.  This is the one into
        /// which the user types to communicate
        /// </summary>
        private Control _talkWindowTextBox;

        /// <summary>
        /// Used to update the time display
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TalkWindowBase()
        {
            _syncObj = new SyncLock();

            Load += TalkWindowBase_Load;
            FormClosing += TalkWindowBase_FormClosing;
        }

        /// <summary>
        /// This event is raised when the talk window wants to
        /// close. The event subscriber is responsbile for closing
        /// the talk window
        /// </summary>
        public event EventHandler EvtRequestCloseTalkWindow;

        /// <summary>
        /// Raised when the font size changes
        /// </summary>
        public event EventHandler EvtTalkWindowFontChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets the size of the font
        /// </summary>
        public virtual float FontSize { get; set; }

        /// <summary>
        /// Gets the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncObj; }
        }

        /// <summary>
        /// Gets the talk window form
        /// </summary>
        public virtual Form TalkWindowForm
        {
            get { return this; }
        }

        /// <summary>
        /// Gets or sets the text in the talk window
        /// </summary>
        public virtual String TalkWindowText
        {
            get
            {
                String retVal = String.Empty;
                if (TalkWindowTextBox != null && TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
                {
                    TalkWindowForm.Invoke(new MethodInvoker(delegate
                    {
                        retVal = TalkWindowTextBox.Text;
                    }));
                }

                return retVal;
            }

            set
            {
                if (TalkWindowTextBox != null && TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
                {
                    TalkWindowForm.Invoke(new MethodInvoker(delegate
                    {
                        var tb = TalkWindowTextBox as TextBoxBase;
                        tb.Text = value;
                        if (String.IsNullOrEmpty(value))
                        {
                            tb.SelectionStart = 0;
                            tb.SelectionLength = 0;
                            tb.ScrollToCaret();
                        }
                    }));
                }
            }
        }

        /// <summary>
        /// Gets or sets the control that is the text box in
        /// the talk window
        /// </summary>
        public virtual Control TalkWindowTextBox
        {
            get
            {
                return _talkWindowTextBox;
            }

            protected set
            {
                _talkWindowTextBox = value;
                defaultFontSize = value.Font.Size;
                FontSize = defaultFontSize;
                _talkWindowTextBox.KeyDown += textBox_KeyDown;
            }
        }

        /// <summary>
        /// Gets or sets the control (typically a Label) that
        /// displays the current date/time.  Return null if
        /// the talk window does not have one.
        /// </summary>
        protected Control dateTimeControl { get; set; }

        /// <summary>
        ///  Centers the talk window in the screen
        /// </summary>
        public virtual void Center()
        {
            if (IsHandleCreated)
            {
                Invoke(new MethodInvoker(CenterToScreen));
            }
        }

        /// <summary>
        /// Clears the text
        /// </summary>
        public virtual void Clear()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                var tb = (TextBoxBase)TalkWindowTextBox;
                TalkWindowForm.Invoke(new MethodInvoker(tb.Clear));
            }
        }

        /// <summary>
        /// Copies text to clipboard
        /// </summary>
        public virtual void Copy()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                var tb = (TextBoxBase)TalkWindowTextBox;
                TalkWindowForm.Invoke(new MethodInvoker(tb.Copy));
            }
        }

        /// <summary>
        /// Cuts text to clipboard
        /// </summary>
        public virtual void Cut()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                var tb = (TextBoxBase)TalkWindowTextBox;
                TalkWindowForm.Invoke(new MethodInvoker(tb.Cut));
            }
        }

        /// <summary>
        /// Pause the talk window
        /// </summary>
        public virtual void OnPause()
        {
        }

        /// <summary>
        /// Invoked when the position of the talk window changes
        /// </summary>
        public virtual void OnPositionChanged()
        {
        }

        /// <summary>
        /// Resume the talk window
        /// </summary>
        public virtual void OnResume()
        {
        }

        /// <summary>
        /// Pastes text from clipboard into the talk window
        /// </summary>
        public virtual void Paste()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                var tb = (TextBoxBase)TalkWindowTextBox;
                TalkWindowForm.Invoke(new MethodInvoker(tb.Paste));
            }
        }

        /// <summary>
        /// Selects all the text
        /// </summary>
        public virtual void SelectAll()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                var tb = (TextBoxBase)TalkWindowTextBox;
                TalkWindowForm.Invoke(new MethodInvoker(tb.SelectAll));
            }
        }

        /// <summary>
        /// Restores default font size
        /// </summary>
        public virtual void ZoomDefault()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                Invoke(new MethodInvoker(delegate
                {
                    if (setFont(defaultFontSize))
                    {
                        notifyFontChanged();
                    }
                }));
            }
        }

        /// <summary>
        /// Increases the font size in the talk window by a step
        /// </summary>
        public virtual void ZoomIn()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                TalkWindowForm.Invoke(new MethodInvoker(delegate
                {
                    if (TalkWindowTextBox.Font.Size + FontSizeDelta <= MaxFontSize)
                    {
                        if (setFont(TalkWindowTextBox.Font.Size + FontSizeDelta))
                        {
                            notifyFontChanged();
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// Decreases the font size in the talk window by a step
        /// </summary>
        public virtual void ZoomOut()
        {
            if (TalkWindowForm != null && TalkWindowTextBox is TextBoxBase)
            {
                TalkWindowForm.Invoke(new MethodInvoker(delegate
                {
                    if (TalkWindowTextBox.Font.Size - FontSizeDelta >= MinFontSize)
                    {
                        if (setFont(TalkWindowTextBox.Font.Size - FontSizeDelta))
                        {
                            notifyFontChanged();
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// Raises event that font has changed
        /// </summary>
        protected virtual void notifyFontChanged()
        {
            if (EvtTalkWindowFontChanged != null)
            {
                EvtTalkWindowFontChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises event that the talk window wants to close
        /// </summary>
        protected virtual void notifyRequestClose()
        {
            if (EvtRequestCloseTalkWindow != null)
            {
                EvtRequestCloseTalkWindow(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Talk window was actived. Make it the topmost window
        /// </summary>
        /// <param name="e">event arg</param>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            Windows.SetTopMost(this);
            //TopMost = false;
            //TopMost = true;
            BringToFront();
        }

        /// <summary>
        /// Invoked when the form is closing
        /// </summary>
        /// <param name="e">event arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _syncObj.Status = SyncLock.StatusValues.Closing;
            stopTimer();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Called when the window is shown
        /// </summary>
        /// <param name="e">event arg</param>
        protected override void OnShown(EventArgs e)
        {
            Log.Debug();

            base.OnShown(e);
            if (_talkWindowTextBox != null)
            {
                ActiveControl = _talkWindowTextBox;
                _talkWindowTextBox.Select();
            }
        }

        /// <summary>
        /// Sets the Theme/color of the talk window
        /// </summary>
        protected virtual void setColors()
        {
            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.TalkWindowSchemeName);
            _talkWindowTextBox.BackColor = colorScheme.Background;
            _talkWindowTextBox.ForeColor = colorScheme.Foreground;
        }

        /// <summary>
        /// Changes the size of the font in the talk window
        /// </summary>
        /// <param name="size">size of the font</param>
        /// <returns>true on success</returns>
        protected virtual bool setFont(float size)
        {
            if (size == 0.0f)
            {
                size = defaultFontSize;
            }

            if (size >= MinFontSize && size <= MaxFontSize)
            {
                var font = TalkWindowTextBox.Font;

                var newFont = new Font(font.FontFamily, size, font.Style);
                TalkWindowTextBox.Font = newFont;

                FontSize = size;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Formats and displays the current date/time in the status bar
        /// </summary>
        protected virtual void updateDateTime(String dateFormat, string timeFormat)
        {
            if (dateTimeControl == null)
            {
                return;
            }

            var dateTime = String.Empty;
            try
            {
                if (!String.IsNullOrEmpty(dateFormat))
                {
                    dateTime = DateTime.Today.ToString(dateFormat);
                }
            }
            catch
            {
                dateTime = DateTime.Today.ToString(defaultDateFormat);
            }

            try
            {
                if (!String.IsNullOrEmpty(timeFormat))
                {
                    if (!String.IsNullOrEmpty(dateTime))
                    {
                        dateTime += ". ";
                    }

                    dateTime += DateTime.Now.ToString(timeFormat);
                }
            }
            catch
            {
                if (!String.IsNullOrEmpty(dateTime))
                {
                    dateTime += ", ";
                }

                dateTime += DateTime.Now.ToString(defaultTimeFormat);
            }

            if (!String.IsNullOrEmpty(dateTime))
            {
                Windows.SetText(dateTimeControl, dateTime);
            }
        }

        /// <summary>
        /// Time tick function.  Update the time display on the status bar
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateDateTime(displayDateFormat, displayTimeFormat);
        }

        /// <summary>
        /// Starts the timer to update the time display
        /// </summary>
        private void startTimer()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = 1000;
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        private void stopTimer()
        {
            if (_timer != null)
            {
                _timer.Elapsed -= _timer_Elapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Form is closing.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TalkWindowBase_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        /// <summary>
        /// Perform initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TalkWindowBase_Load(object sender, EventArgs e)
        {
            if (enableDateTimeDisplay)
            {
                updateDateTime(displayDateFormat, displayTimeFormat);
                startTimer();
            }
        }

        /// <summary>
        /// Handle shortcuts.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                SelectAll();
            }
            else if (e.Control & e.KeyCode == Keys.Back)
            {
                SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
            }
        }
    }
}