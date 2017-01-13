////////////////////////////////////////////////////////////////////////////
// <copyright file="PhraseEditorForm.cs" company="Intel Corporation">
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

using ACAT.ACATResources;
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Extension;
using System;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.PhraseSpeakAgent
{
    /// <summary>
    /// Dialog box form to enable the user to edit phrase
    /// </summary>
    [DescriptorAttribute("158493C6-C195-4482-83F5-231BE4CB897C",
                            "PhraseEditorForm",
                            "Phrase Editor")]
    public partial class PhraseEditorForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// Extension invoker object to invoke properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Aspect ratio of form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// The dialog common object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Has first call to OnClientSizeChanged been made?
        /// </summary>
        private bool _firstClientChangedCall = true;

        /// <summary>
        /// Text of phrase on entry
        /// </summary>
        private string _initialText;

        /// <summary>
        /// Did something change?
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Makes sure this window stays active and keeps focus
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// The widget representingthe OK button
        /// </summary>
        private Widget buttonOKWidget;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PhraseEditorForm()
        {
            InitializeComponent();
            _invoker = new ExtensionInvoker(this);
            Load += PhraseEditorForm_Load;
            FormClosing += PhraseEditorForm_FormClosing;
        }

        /// <summary>
        /// Gets or sets whether the user canceled out
        /// </summary>
        public Boolean Cancel { get; set; }

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
        /// Gets or sets the input phrase that has to be added/edited
        /// </summary>
        public Phrase Phrase { get; set; }

        /// <summary>
        /// Gets the sync object used for synchronization
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the style of the form
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return DialogCommon.SetFormStyles(base.CreateParams); }
        }

        /// <summary>
        /// Returns the extension invoker
        /// </summary>
        /// <returns>the invoker</returns>
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

            Cancel = false;

            return _dialogCommon.Initialize(startupArg);
        }

        /// <summary>
        /// Invoked when the user selects a widget
        /// in the dialog such as a button or a text box.
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            String value = widget.Value;

            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            Invoke(new MethodInvoker(delegate
            {
                switch (value)
                {
                    case "buttonOK":
                        finish();
                        break;

                    case "buttonCancel":
                        cancel();
                        break;

                    case "labelFavoriteCheckBox":
                        _isDirty = true;
                        this.Phrase.Favorite = !this.Phrase.Favorite;
                        (PanelCommon.RootWidget.Finder.FindChild(labelFavoriteCheckBox.Name) as CheckBoxWidget).SetState(Phrase.Favorite);
                        break;
                }
            }));
        }

        /// <summary>
        /// Pauses the animation
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
        }

        /// <summary>
        /// Invoked when there is a need to run a command
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="handled">was it handled</param>
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
        /// Client size changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            if (_firstClientChangedCall)
            {
                _designTimeAspectRatio = (float)ClientSize.Height / ClientSize.Width;
                _firstClientChangedCall = false;
            }
        }

        /// <summary>
        /// Windows procedure
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// User wants to cancel out of the dialog box
        /// </summary>
        private void cancel()
        {
            Log.Debug();

            var quit = true;
            if (_isDirty || hasTextChanged())
            {
                quit = DialogUtils.Confirm(R.GetString("CancelAndQuit"));
            }

            if (quit)
            {
                Invoke(new MethodInvoker(delegate
                {
                    Cancel = true;
                    Windows.CloseForm(this);
                }));
            }
        }

        /// <summary>
        /// User is done. Confirm, perform validation and check
        /// that everything is OK and then quit.
        /// </summary>
        private void finish()
        {
            var quit = true;

            if (_isDirty || hasTextChanged())
            {
                quit = DialogUtils.Confirm(R.GetString("SaveAndQuit"));
            }

            if (quit)
            {
                Phrase.Text = Windows.GetText(textBoxPhrase);
                Cancel = false;
                Windows.CloseForm(this);
            }
        }

        /// <summary>
        /// Returns true if the text has changed in the text box
        /// </summary>
        /// <returns>true if it has</returns>
        private bool hasTextChanged()
        {
            return String.Compare(_initialText, textBoxPhrase.Text) != 0;
        }

        /// <summary>
        /// Initializes the state of all the widgets in the
        /// dialog box.
        /// </summary>
        private void initWidgetSettings()
        {
            var rootWidget = PanelCommon.RootWidget;

            if (Phrase == null)
            {
                Phrase = new Phrase();
            }

            textBoxPhrase.Text = Phrase.Text;
            _initialText = Phrase.Text;

            textBoxPhrase.SelectionStart = 0;
            textBoxPhrase.SelectionLength = 0;
            textBoxPhrase.ScrollToCaret();

            (rootWidget.Finder.FindChild(labelFavoriteCheckBox.Name) as CheckBoxWidget).SetState(Phrase.Favorite);
        }

        /// <summary>
        /// Event handler to check if a widget needs to be enabled
        /// </summary>
        /// <param name="arg">event arg</param>
        private void Instance_EvtCommandEnabled(Lib.Core.AgentManagement.CommandEnabledArg arg)
        {
            arg.Handled = true;

            switch (arg.Command)
            {
                case "CmdDeletePrevWord":
                case "CmdPrevChar":
                case "CmdNextChar":
                case "CmdNextLine":
                case "CmdPrevLine":
                case "CmdNextWord":
                case "CmdPrevWord":
                case "CmdCursorScanner":
                    arg.Handled = true;
                    arg.Enabled = true;
                    break;

                default:
                    arg.Handled = false;
                    break;
            }
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void PhraseEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            PhraseSpeakAgent.Instance.EvtCommandEnabled -= Instance_EvtCommandEnabled;

            _windowActiveWatchdog.Dispose();

            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Perform initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void PhraseEditorForm_Load(object sender, EventArgs e)
        {
            Text = R.GetString("PhraseEditor");
            labelPhrase.Text = R.GetString("Phrase") + ":";

            float currentAspectRatio = (float)ClientSize.Height / ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ClientSize = new System.Drawing.Size(ClientSize.Width, (int)(_designTimeAspectRatio * ClientSize.Width));
            }

            PhraseSpeakAgent.Instance.EvtCommandEnabled += Instance_EvtCommandEnabled;
            textBoxPhrase.TextChanged += TextBoxPhrase_TextChanged;

            textBoxPhrase.KeyDown += TextBoxPhrase_KeyDown;
            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            initWidgetSettings();

            _dialogCommon.OnLoad();

            buttonOKWidget = _dialogCommon.RootWidget.Finder.FindChild(buttonOK.Handle);

            updateUI();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Key down event.  Disable return key
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TextBoxPhrase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Event handler for when the text changes in the text box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TextBoxPhrase_TextChanged(object sender, EventArgs e)
        {
            updateUI();
        }

        /// <summary>
        /// Updates the state of the OK button
        /// </summary>
        private void updateUI()
        {
            if (buttonOKWidget != null)
            {
                buttonOKWidget.Enabled = !String.IsNullOrEmpty(textBoxPhrase.Text.Trim());
            }
        }
    }
}