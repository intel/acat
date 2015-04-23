////////////////////////////////////////////////////////////////////////////
// <copyright file="TimedMessageBox.cs" company="Intel Corporation">
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
using System.Windows.Forms;

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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Form for a message box with an OK button.  Uses
    /// a timer and when the timer expires, automatically
    /// closes the box.  Useful for error messages or informational
    /// messages
    /// </summary>
    public partial class TimedMessageBox : Form
    {
        /// <summary>
        /// How long to keep the box up?
        /// </summary>
        private const int _timeout = 6;

        /// <summary>
        /// Closing message
        /// </summary>
        private const String _timerMsg = "This message box will close in {0} seconds";

        /// <summary>
        /// Message to display
        /// </summary>
        private readonly String _message;

        /// <summary>
        /// Used to keep update the info on the form
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// Time remaining
        /// </summary>
        private int _timeleft;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="message">Nessage to display</param>
        private TimedMessageBox(String message)
        {
            InitializeComponent();

            _message = message;
            _timer = new Timer();
            Load += TimedMessageBox_Load;
        }

        /// <summary>
        /// Customize look and feel
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_SYSMENU = 0x80000;
                var cp = base.CreateParams;
                cp.Style &= ~WS_SYSMENU;
                return cp;
            }
        }

        /// <summary>
        /// Shows the message box
        /// </summary>
        /// <param name="message"></param>
        public static void Show(String message)
        {
            var box = new TimedMessageBox(message);
            box.CenterToScreen();
            box.ShowDialog();
        }

        /// <summary>
        /// Copy the message to clipboard
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonCopyClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_message);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            Close();
        }

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TimedMessageBox_Load(object sender, EventArgs e)
        {
            _timeleft = _timeout;
            labelMessage.Text = _message;
            _timer.Interval = 1000;
            _timer.Tick += timer_Tick;
            updateTimerMessage();
            _timer.Start();
        }

        /// <summary>
        /// Timer tick event.  Update form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            updateTimerMessage();
        }

        /// <summary>
        /// Updates info on the form
        /// </summary>
        private void updateTimerMessage()
        {
            labelTimer.Text = String.Format(_timerMsg, _timeleft);
            _timeleft -= 1;
            if (_timeleft < 0)
            {
                _timer.Stop();
                Close();
            }
        }

        /// <summary>
        /// User clicked on the OK button.  Close the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
    }
}