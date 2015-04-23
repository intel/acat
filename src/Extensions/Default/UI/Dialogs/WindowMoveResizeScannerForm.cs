////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowMoveResizeScannerForm.cs" company="Intel Corporation">
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
    /// This dialog enables the user to move or resize
    /// an application window. The caller has to first
    /// send a alt-space to the window which brings up the
    /// control menu. The caller must then send a 'm' or an
    /// 's' to move or resize the window.  Then using arrow
    /// keys, the user can point to which edge of the window
    /// to resize or move and then continue pressing the arrow
    /// key to move or resize the window.  This form enables
    /// the user to send the arrow key strokes to the
    /// application window so it can be moved or resized.
    /// </summary>
    [DescriptorAttribute("AB688D22-7302-48C8-92A0-1F47EE38147E", "WindowMoveResizeScannerForm",
                        "Window Move/Resize Dialog")]
    public partial class WindowMoveResizeScannerForm : Form, IDialogPanel
    {
        /// <summary>
        /// The dialogCommon object
        /// </summary>
        private readonly DialogCommon _dialogCommon;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WindowMoveResizeScannerForm()
        {
            InitializeComponent();

            _dialogCommon = new DialogCommon(this);
            Title.Hide();

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            Load += ResizeScannerScreen_Load;
            FormClosing += ResizeScannerScreen_FormClosing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the sync object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Sets the form style
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                var createParams = base.CreateParams;

                const int WS_SYSMENU = 0x80000;
                createParams.Style &= ~WS_SYSMENU;
                createParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_NOACTIVATE;

                return createParams;
            }
        }

        /// <summary>
        /// Triggered when a widget is actuated
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        ///
        public void OnButtonActuated(Widget widget)
        {
            Log.Debug("**Actuate** " + widget.UIControl.Name + " Value: " + widget.Value);

            var value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                Log.Debug("OnButtonActuated() -- received actuation from empty widget!");
                return;
            }

            Invoke(new MethodInvoker(delegate()
            {
                switch (value)
                {
                    case "goBack":
                        Context.AppAgentMgr.Keyboard.Send(Keys.Escape);
                        Windows.CloseForm(this);
                        break;

                    case "Up":
                        Context.AppAgentMgr.Keyboard.Send(Keys.Up);
                        break;

                    case "Down":
                        Context.AppAgentMgr.Keyboard.Send(Keys.Down);
                        break;

                    case "Left":
                        Context.AppAgentMgr.Keyboard.Send(Keys.Left);
                        break;

                    case "Right":
                        Context.AppAgentMgr.Keyboard.Send(Keys.Right);
                        break;

                    case "Done":
                        Context.AppAgentMgr.Keyboard.Send(Keys.Enter);
                        Windows.CloseForm(this);
                        break;

                    default:
                        Log.Debug("OnButtonActuated() -- unhandled widget actuation!");
                        break;
                }
            }));
        }

        /// <summary>
        /// Pauses the scanner
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resumes paused scanner
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
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

        /// <summary>
        /// Form is closing. release resources
        /// </summary>
        /// <param name="e">closing arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        private void ResizeScannerScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        private void ResizeScannerScreen_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();
            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }
    }
}