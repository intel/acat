////////////////////////////////////////////////////////////////////////////
// <copyright file="AbbreviationEditorForm.cs" company="Intel Corporation">
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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Extension;

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

namespace ACAT.Extensions.Hawking.FunctionalAgents.Abbreviations
{
    /// <summary>
    /// Dialog box form to enable the user to edit an
    /// abbreviation. This includes the abbreviation, its expansion
    /// and the expansion mode
    /// </summary>
    [DescriptorAttribute("5D03D10B-48B4-412D-9442-C93E65D96BA6", "AbbreviationEditorForm", "Abbreviations Editor")]
    public partial class AbbreviationEditorForm : Form, IDialogPanel, IExtension
    {
        /// <summary>
        /// Extension invoker object to invoke properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// the dialog common object
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
            init();
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
        /// Gets or sets the input abbreviation that has
        /// to be edited
        /// </summary>
        public Abbreviation InputAbbreviation { get; set; }

        /// <summary>
        /// Gets the final version of the abbreviation after the
        /// user has made the necessary changes
        /// </summary>
        public Abbreviation OutputAbbreviation { get; private set; }

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

            Invoke(new MethodInvoker(delegate()
            {
                switch (value)
                {
                    case "valButtonFinished":
                        finish();
                        break;

                    case "valButtonUndo":
                        undo();
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
        /// Win procedure
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
                Windows.SetText(labelTitle, "Add Abbreviation");
                var widget = _dialogCommon.GetRootWidget().Finder.FindChild("lblDelete");
                if (widget != null)
                {
                    widget.Enabled = false;
                }
            }
            else
            {
                Windows.SetText(labelTitle, "Edit/Delete Abbreviation");
            }

            _windowActiveWatchdog = new WindowActiveWatchdog(this);

            initWidgetSettings();

            _dialogCommon.OnLoad();

            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }

        /// <summary>
        /// User wants to cancel out of the dialog box
        /// </summary>
        private void cancel()
        {
            Log.Debug();

            if (DialogUtils.Confirm("Cancel and Quit?"))
            {
                Invoke(new MethodInvoker(delegate()
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
                DialogUtils.ShowTimedDialog(this, "Error", "Please enter an abbreviation!");
            }
            else if (String.IsNullOrEmpty(tbExpansion.Text.Trim()))
            {
                DialogUtils.ShowTimedDialog(this, "Error", "Expansion is empty");
            }
            else
            {
                bool saveAndQuit = false;
                bool confirm = false;

                if (Add)
                {
                    // a new abbreviation is beein added
                    if (Context.AppAbbreviations.Exists(abbr))
                    {
                        DialogUtils.ShowTimedDialog(
                                        Context.AppPanelManager.GetCurrentPanel() as Form,
                                        "Add", 
                                        "Can't Save. Abbr for '" + abbr + "' already exists");
                    }
                    else
                    {
                        saveAndQuit = true;
                        confirm = true;
                    }
                }
                else if (Context.AppAbbreviations.Exists(abbr) &&
                        String.Compare(abbr, InputAbbreviation.Mnemonic.Trim(), true) != 0)
                {
                    if (DialogUtils.Confirm("You are changing existing abbr " + tbAbbreviation.Text + " . Continue?"))
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
                        quit = DialogUtils.Confirm("Save Abbreviation and Quit?");
                    }

                    if (quit)
                    {
                        OutputAbbreviation = new Abbreviation(tbAbbreviation.Text, tbExpansion.Text, _mode);
                        _dialogCommon.GetRootWidget();
                        Cancel = false;
                        Windows.CloseForm(this);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void init()
        {
            _dialogCommon = new DialogCommon(this);

            Add = false;
            Delete = false;
            OutputAbbreviation = new Abbreviation(String.Empty, String.Empty, Abbreviation.AbbreviationMode.Speak);
            Cancel = false;

            if (!_dialogCommon.Initialize())
            {
                Log.Debug("Initialization error");
            }

            Load += AbbreviationsEditorForm_Load;
            FormClosing += AbbreviationsEditorForm_FormClosing;
        }

        /// <summary>
        /// Initializes the state of all the widgets in the
        /// dialog box.
        /// </summary>
        private void initWidgetSettings()
        {
            Widget rootWidget = _dialogCommon.GetRootWidget();

            _mode = InputAbbreviation.Mode;
            WidgetUtils.SetCheckBoxWidgetState(
                                rootWidget, 
                                pbTypeSpoken.Name, 
                                _mode == Abbreviation.AbbreviationMode.Speak || _mode == Abbreviation.AbbreviationMode.None);

            WidgetUtils.SetCheckBoxWidgetState(
                                rootWidget, 
                                pbTypeWritten.Name, _mode == Abbreviation.AbbreviationMode.Write);

            Windows.SetText(tbAbbreviation, InputAbbreviation.Mnemonic);
            tbAbbreviation.Select(tbAbbreviation.Text.Length, 0);
            Windows.SetText(tbExpansion, Regex.Replace(InputAbbreviation.Expansion, "\n", "\r\n"));
            tbExpansion.Select(tbExpansion.Text.Length, 0);
        }

        /// <summary>
        /// Sets the appropriate radio button depending on the
        /// abbreviation mode
        /// </summary>
        /// <param name="widgetName">name of the widget</param>
        /// <param name="choice">choice made</param>
        private void radioSetAbbreviationMode(String widgetName, Boolean choice)
        {
            Widget rootWidget = _dialogCommon.GetRootWidget();

            WidgetUtils.SetCheckBoxWidgetState(rootWidget, pbTypeSpoken.Name, false);
            WidgetUtils.SetCheckBoxWidgetState(rootWidget, pbTypeWritten.Name, false);

            WidgetUtils.SetCheckBoxWidgetState(rootWidget, widgetName, choice);
        }

        /// <summary>
        /// Undo any changes and restore original state
        /// </summary>
        private void undo()
        {
            if (DialogUtils.Confirm(this, "Undo Change?"))
            {
                initWidgetSettings();
            }
        }
    }
}