////////////////////////////////////////////////////////////////////////////
// <copyright file="AbbreviationEditorForm.cs" company="Intel Corporation">
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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.AbbreviationsAgent
{
    /// <summary>
    /// Dialog box form to enable the user to edit an
    /// abbreviation. This includes the abbreviation, its expansion
    /// and the expansion mode
    /// </summary>
    [DescriptorAttribute("5D03D10B-48B4-412D-9442-C93E65D96BA6",
                            "AbbreviationEditorForm",
                            "Abbreviations Editor")]
    public partial class AbbreviationEditorForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// Extension invoker object to invoke properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The dialog common object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Mode:Spoken or written
        /// </summary>
        private Abbreviation.AbbreviationMode _mode;

        /// <summary>
        /// Makes sure this window stays active and keeps focus
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AbbreviationEditorForm()
        {
            InitializeComponent();
            _invoker = new ExtensionInvoker(this);
            Load += AbbreviationsEditorForm_Load;
            FormClosing += AbbreviationsEditorForm_FormClosing;
        }

        /// <summary>
        /// Gets or sets the operation. Are we adding a
        /// new abbr?
        /// </summary>
        public bool Add { get; set; }

        /// <summary>
        /// Gets or sets whether the user canceled out
        /// </summary>
        public Boolean Cancel { get; set; }

        /// <summary>
        /// Gets or sets whether we are deleting an
        /// abbreviation
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets the input abbreviation that has to be edited
        /// </summary>
        public Abbreviation InputAbbreviation { get; set; }

        /// <summary>
        /// Gets the final version of the abbreviation after the
        /// user has made the necessary changes
        /// </summary>
        public Abbreviation OutputAbbreviation { get; private set; }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _dialogCommon; } }

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

            Add = false;
            Delete = false;
            OutputAbbreviation = new Abbreviation(String.Empty, String.Empty, Abbreviation.AbbreviationMode.Speak);
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
                    case "valButtonFinished":
                        finish();
                        break;

                    case "valButtonCancel":
                        cancel();
                        break;

                    case "valCheckWritten":
                        _mode = Abbreviation.AbbreviationMode.Write;
                        radioSetAbbreviationMode(pbTypeWritten.Name, true);
                        break;

                    case "valCheckSpoken":
                        _mode = Abbreviation.AbbreviationMode.Speak;
                        radioSetAbbreviationMode(pbTypeSpoken.Name, true);
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
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void AbbreviationsEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _windowActiveWatchdog.Dispose();

            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Perform initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AbbreviationsEditorForm_Load(object sender, EventArgs e)
        {
            if (Add)
            {
                Windows.SetText(labelTitle, R.GetString("AddAbbreviation"));
                var widget = PanelCommon.RootWidget.Finder.FindChild("lblDelete");
                if (widget != null)
                {
                    widget.Enabled = false;
                }
            }
            else
            {
                Windows.SetText(labelTitle, R.GetString("EditAbbreviation"));
            }

            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            initWidgetSettings();

            _dialogCommon.OnLoad();

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// User wants to cancel out of the dialog box
        /// </summary>
        private void cancel()
        {
            Log.Debug();

            if (DialogUtils.Confirm(R.GetString("CancelAndQuit")))
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
            var abbr = tbAbbreviation.Text.Trim();
            if (String.IsNullOrEmpty(abbr))
            {
                showTimedDialog(R.GetString("Error"), R.GetString("PleaseEnterAnAbbreviation"));
                return;
            }

            if (String.IsNullOrEmpty(tbExpansion.Text.Trim()))
            {
                showTimedDialog(R.GetString("Error"), R.GetString("ExpansionIsEmpty"));
                return;
            }

            bool saveAndQuit = false;
            bool confirm = false;

            if (Add)
            {
                // a new abbreviation is being added
                if (Context.AppAbbreviationsManager.Abbreviations.Exists(abbr))
                {
                    showTimedDialog(R.GetString("Add"), String.Format(R.GetString("CantSaveAbbreviation"), abbr));
                }
                else
                {
                    saveAndQuit = true;
                    confirm = true;
                }
            }
            else if (Context.AppAbbreviationsManager.Abbreviations.Exists(abbr) &&
                    String.Compare(abbr, InputAbbreviation.Mnemonic.Trim(), true) != 0)
            {
                if (DialogUtils.Confirm(String.Format(R.GetString("YouAreChangingExistingAbbr"), tbAbbreviation.Text)))
                {
                    saveAndQuit = true;
                }
            }
            else
            {
                saveAndQuit = true;
                confirm = true;
            }

            if (saveAndQuit)
            {
                bool quit = true;

                if (confirm)
                {
                    quit = DialogUtils.Confirm(R.GetString("SaveAndQuit"));
                }

                if (quit)
                {
                    OutputAbbreviation = new Abbreviation(tbAbbreviation.Text, tbExpansion.Text, _mode);
                    Cancel = false;
                    Windows.CloseForm(this);
                }
            }
        }

        /// <summary>
        /// Initializes the state of all the widgets in the
        /// dialog box.
        /// </summary>
        private void initWidgetSettings()
        {
            var rootWidget = PanelCommon.RootWidget;

            _mode = InputAbbreviation.Mode;
            (rootWidget.Finder.FindChild(pbTypeSpoken.Name) as CheckBoxWidget).SetState(_mode == Abbreviation.AbbreviationMode.Speak ||
                                                                                                _mode == Abbreviation.AbbreviationMode.None);

            (rootWidget.Finder.FindChild(pbTypeWritten.Name) as CheckBoxWidget).SetState(_mode == Abbreviation.AbbreviationMode.Write);

            Windows.SetText(tbAbbreviation, InputAbbreviation.Mnemonic);
            tbAbbreviation.Select(tbAbbreviation.Text.Length, 0);
            Windows.SetText(tbExpansion, Regex.Replace(InputAbbreviation.Expansion, "\n", "\r\n"));
            tbExpansion.Select(tbExpansion.Text.Length, 0);
        }

        /// <summary>
        /// Sets the appropriate radio button for the mode depending on the
        /// abbreviation mode
        /// </summary>
        /// <param name="widgetName">name of the widget</param>
        /// <param name="choice">choice made</param>
        private void radioSetAbbreviationMode(String widgetName, Boolean choice)
        {
            var rootWidget = PanelCommon.RootWidget;

            (rootWidget.Finder.FindChild(pbTypeSpoken.Name) as CheckBoxWidget).SetState(false);
            (rootWidget.Finder.FindChild(pbTypeWritten.Name) as CheckBoxWidget).SetState(false);
            (rootWidget.Finder.FindChild(widgetName) as CheckBoxWidget).SetState(choice);
        }

        /// <summary>
        /// Displays a timed dialog with the title and message
        /// </summary>
        /// <param name="title">title of the dialog</param>
        /// <param name="message">message</param>
        private void showTimedDialog(String title, String message)
        {
            _windowActiveWatchdog.Pause();
            DialogUtils.ShowTimedDialog(this, title, message);
            _windowActiveWatchdog.Resume();
        }
    }
}