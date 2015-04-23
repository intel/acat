////////////////////////////////////////////////////////////////////////////
// <copyright file="TimedDialogForm.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// A message box equivalent to display a message. But
    /// it stays up only for a few seconds and then closes.
    /// Can be used to display error messages,
    /// The form itself doesn't have a timer. It is controlled
    /// by the animation xml file.  An alternate xml file
    /// can be used where the timer is not specified.
    /// </summary>
    [DescriptorAttribute("22C1D68A-574F-4512-A0C3-579EA9AADD61", "TimedDialogForm", "Timed Dialog")]
    public partial class TimedDialogForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private readonly DialogCommon _dialogCommon;

        /// <summary>
        /// Enables access to the class methods and properties.
        /// </summary>
        private ExtensionInvoker _invoker;

        /// <summary>
        /// Message to display in the vox
        /// </summary>
        private String _message = String.Empty;

        /// <summary>
        /// Whether to show the OK button
        /// </summary>
        private bool _showButton;

        /// <summary>
        /// Title of the dialog box
        /// </summary>
        private String _titleText = String.Empty;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TimedDialogForm()
        {
            InitializeComponent();

            _invoker = new ExtensionInvoker(this);

            _dialogCommon = new DialogCommon(this);
            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            ShowButton = true;
            Text = _titleText;
            MessageText = _message;

            Load += Form_Load;
            FormClosing += Form_Closing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets the prompt to be displayed
        /// </summary>
        public String MessageText
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
                Windows.SetText(labelMessage, value);
            }
        }

        /// <summary>
        /// Gets or sets whether to display the OK button
        /// </summary>
        public bool ShowButton
        {
            get
            {
                return _showButton;
            }

            set
            {
                _showButton = value;
                buttonOK.Visible = _showButton;
            }
        }

        /// <summary>
        /// Gets synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Gets or sets the title of the form
        /// </summary>
        public String TitleText
        {
            get
            {
                return _titleText;
            }

            set
            {
                _titleText = value;
                Windows.SetText(this, value);
            }
        }

        /// <summary>
        /// Gets the form style
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return DialogCommon.SetFormStyles(Windows.SetFormStyles(base.CreateParams)); }
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns>invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Triggered when a widget is actuated.
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            var value = widget.Value;
            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            Log.Debug("**Actuate** " + widget.Name + " Value: " + value);

            switch (value)
            {
                case "ok":
                    Windows.CloseForm(this);
                    break;
            }
        }

        /// <summary>
        /// Pause the scanner
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resume paused scanner
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
        }

        /// <summary>
        /// Handle the command. There is only the OK
        /// button that we have to handle
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <param name="handled">was it handled?</param>
        public void OnRunCommand(string command, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                case "@ok":
                    Windows.CloseAsync(this);
                    break;

                default:
                    handled = false;
                    break;
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="e">closing argument</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">event args</param>
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Initialize
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">event args</param>
        private void Form_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }
    }
}