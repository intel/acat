////////////////////////////////////////////////////////////////////////////
// <copyright file="YesNoDialogForm.cs" company="Intel Corporation">
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
using System.Security.Permissions;
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
    /// Displays a dialog box with a prompt and
    /// yes no buttons. Can be used for user confirmaton
    /// </summary>
    [DescriptorAttribute("285CB072-737D-4EE1-B866-F574BA633401", "YesNoDialogForm",
                        "Yes/No Dialog")]
    public partial class YesNoDialogForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// Provides access to methods and properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public YesNoDialogForm()
        {
            InitializeComponent();

            _invoker = new ExtensionInvoker(this);

            Choice = false;

            TextButton1 = "Yes";
            TextButton2 = "No";
            Caption = String.Empty;
            TitleBar = "ACAT";

            Init();
        }

        /// <summary>
        /// Gets or sets the caption
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Gets the choice the user made. True
        /// if yes (or if the first button was selected)
        /// </summary>
        public bool Choice { get; set; }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Gets / set the text for the first text
        /// button. Typically this is a "Yes"
        /// </summary>
        public String TextButton1 { get; set; }

        /// <summary>
        /// Gets / sets the text for the second text
        /// button. Typically this is a "No"
        /// </summary>
        public String TextButton2 { get; set; }

        /// <summary>
        /// Gets or sets the title for the dialog box
        /// </summary>
        public String TitleBar { get; set; }

        /// <summary>
        /// Sets the form style
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                var createParams = base.CreateParams;
                createParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_NOACTIVATE;
                return createParams;
            }
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns></returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Initialzes the dialog
        /// </summary>
        /// <returns>true on success</returns>
        public bool Init()
        {
            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            Load += Form_Load;
            FormClosing += Form_Closing;

            return true;
        }

        /// <summary>
        /// Triggered when a widget is actuated
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
                case "@button1":
                    Choice = true;
                    break;

                case "@button2":
                    Choice = false;
                    break;
            }

            Windows.CloseForm(this);
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnResume()
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handled"></param>
        public void OnRunCommand(string command, ref bool handled)
        {
            switch (command)
            {
                default:
                    handled = false;
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            if (_dialogCommon != null)
            {
                _dialogCommon.HandleWndProc(m);
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        private void Form_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();

            initialize();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }

        /// <summary>
        /// Initialize the scanner
        /// </summary>
        /// <returns></returns>
        private bool initialize()
        {
            Windows.SetText(labelCaption, Caption);
            Windows.SetText(Button1, TextButton1);
            Windows.SetText(Button2, TextButton2);

            return true;
        }
    }
}