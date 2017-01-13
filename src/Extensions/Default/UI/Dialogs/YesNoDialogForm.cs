////////////////////////////////////////////////////////////////////////////
// <copyright file="YesNoDialogForm.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// A dialog which has a prompt and expects
    /// a YES/NO response.  Has a blank spaces between the yes
    /// and no to give user time to react
    /// </summary>
    [DescriptorAttribute("640B79F7-0574-45A5-9A8C-50F87C62B08A",
                            "YesNoDialogForm",
                            "Yes/No Dialog")]
    public partial class YesNoDialogForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// Provides access to methods and properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The keyboard actuator
        /// </summary>
        private readonly KeyboardActuator _keyboardActuator;

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

            Caption = String.Empty;
            TitleBar = "ACAT";

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyDown += keyboardActuator_EvtKeyDown;
            }

            Load += Form_Load;
            FormClosing += Form_Closing;
        }

        /// <summary>
        /// Gets or sets the caption
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Gets the choice the user made. True
        /// if the user selected "Yes"
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
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _dialogCommon; } }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Gets or sets the title bar
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
                return DialogCommon.SetFormStyles(createParams);
            }
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
        /// Initializes the dialog
        /// </summary>
        /// <param name="startupArg">startup info</param>
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
                case "@CmdNo":
                    Choice = false;
                    break;

                case "@CmdYes":
                    Choice = true;
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

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _dialogCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m">windows message</param>
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
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            _keyboardActuator.EvtKeyDown -= keyboardActuator_EvtKeyDown;
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();

            initialize();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Initializes the form
        /// </summary>
        /// <returns>true</returns>
        private bool initialize()
        {
            Windows.SetText(labelCaption, Caption);
            Windows.SetText(this, TitleBar);

            return true;
        }

        /// <summary>
        /// Keydown event handler.  Handles y n and escape
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="keyEventArgs">event args</param>
        private void keyboardActuator_EvtKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Escape || keyEventArgs.KeyCode == Keys.N)
            {
                Choice = false;
                Close();
            }
            else if (keyEventArgs.KeyCode == Keys.Y)
            {
                Choice = true;
                Close();
            }
        }
    }
}