// <copyright file="WindowMoveResizeScannerForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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

using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

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
    [DescriptorAttribute("AB688D22-7302-48C8-92A0-1F47EE38147E",
                        "WindowMoveResizeScannerForm",
                        "Window Move/Resize Dialog")]
    public partial class WindowMoveResizeScannerForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// Used to invoke methods and properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The dialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WindowMoveResizeScannerForm()
        {
            InitializeComponent();

            _invoker = new ExtensionInvoker(this);

            Title.Hide();

            Load += WindowMoveResizeScannerForm_Load;
            FormClosing += WindowMoveResizeScannerForm_FormClosing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Perform move operation on target window
        /// </summary>
        public bool MoveWindow { get; set; }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _dialogCommon; } }

        /// <summary>
        /// Perform resize operation on target window
        /// </summary>
        public bool ResizeWindow { get; set; }

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
        /// Don't steal focus
        /// </summary>
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns>The invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Intitializes the class
        /// </summary>
        /// <param name="startupArg">startup param</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _dialogCommon = new DialogCommon(this);

            return _dialogCommon.Initialize(startupArg);
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

            Invoke(new MethodInvoker(delegate
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
        /// Sends commands to the target window to enable moving/
        /// resizing
        /// </summary>
        private void setMoveResizeMode()
        {
            Context.AppAgentMgr.Keyboard.Send(Keys.LMenu, Keys.Space);
            Thread.Sleep(500);

            if (MoveWindow)
            {
                Context.AppAgentMgr.Keyboard.Send(Keys.M);
            }
            else if (ResizeWindow)
            {
                Context.AppAgentMgr.Keyboard.Send(Keys.S);
            }
        }

        /// <summary>
        /// Form is closing. Releases resources
        /// </summary>
        private void WindowMoveResizeScannerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        private void WindowMoveResizeScannerForm_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();
            EnumWindows.RestoreFocusToTopWindow(Handle.ToInt32());
            setMoveResizeMode();
            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }
    }
}