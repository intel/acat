////////////////////////////////////////////////////////////////////////////
// <copyright file="AbbreviationsAgent.cs" company="Intel Corporation">
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
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
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
    /// Functional agent that manages the abbreviations scanner
    /// which displays a list of abbreviations and the user
    /// can add, delete or modify abbreviations.
    /// </summary>
    [DescriptorAttribute("4AD7D574-9C8F-4ED7-9152-F836F210F68E", "Abbreviations Agent", "UI to create/edit/delete abbreviations")]
    public class AbbreviationsAgent : FunctionalAgentBase
    {
        /// <summary>
        /// this is the list of widgets that hold the abbreviations in the abbreviations
        /// scanner.
        /// </summary>
        private readonly String[] _supportedFeatures =
        {
            "Select_1",
            "Select_2",
            "Select_3",
            "Select_4",
            "Select_5",
            "Select_6",
            "Select_7",
            "Select_8",
            "Select_9",
            "Select_10",
        };

        /// <summary>
        /// The abbreviation selected for editing
        /// </summary>
        private Abbreviation _abbreviationSelected;

        /// <summary>
        /// Confirms whether the user wants to delete or
        /// edit the abbreviation
        /// </summary>
        private Form _editDeleteConfirmScanner;

        /// <summary>
        /// has the edit/delete menu shown?  show it only
        /// if this is false
        /// </summary>
        private bool _editDeleteMenuShown;

        /// <summary>
        /// The abbreviations form
        /// </summary>
        private AbbreviationsScanner _form;

        /// <summary>
        /// Has the scanner been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// Get confirmation from the user
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>if the user said yes</returns>
        public static bool Confirm(String prompt)
        {
            return DialogUtils.ConfirmScanner(PanelManager.Instance.GetCurrentPanel(), prompt);
        }

        /// <summary>
        /// Invoked when the Functional agent is activated.  This is
        /// the entry point for the functional agent.
        /// Creates the abbreviations scanner, subscribe to events
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Activate()
        {
            bool retVal = true;

            _scannerShown = false;
            ExitCode = CompletionCode.None;
            _form = Context.AppPanelManager.CreatePanel("AbbreviationsScanner") as AbbreviationsScanner;
            if (_form != null)
            {
                _form.EvtDone += _form_EvtDone;
                _form.EvtAddAbbreviation += _form_EvtAddAbbreviation;
                _form.EvtEditAbbreviation += _form_EvtEditAbbreviation;
                _form.FormClosing += _form_FormClosing;
                Context.AppPanelManager.ShowDialog(_form);
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            switch (arg.Widget.SubClass)
            {
                case "Back":
                case "DeletePreviousWord":
                    if (_form != null && Windows.GetVisible(_form))
                    {
                        arg.Enabled = !_form.IsFilterEmpty();
                    }
                    else
                    {
                        arg.Enabled = true;
                    }

                    arg.Handled = true;
                    return;

                case "clearText":
                    if (_form != null && Windows.GetVisible(_form))
                    {
                        arg.Enabled = !_form.IsFilterEmpty();
                    }
                    else
                    {
                        arg.Enabled = false;
                    }

                    arg.Handled = true;
                    return;

                case "FileBrowserToggle":
                    arg.Handled = true;
                    arg.Enabled = false;
                    return;
            }

            checkWidgetEnabled(_supportedFeatures, arg);
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug("OnFocus: " + monitorInfo.ToString());

            base.OnFocusChanged(monitorInfo, ref handled);

            var handle = IntPtr.Zero;
            if (_form != null)
            {
                _form.Invoke(new MethodInvoker(delegate()
                    {
                        handle = _form.Handle;
                    }));
            }

            // if the abbreviations scanner is up, display the
            // alphabet scanner
            if (handle != IntPtr.Zero && monitorInfo.FgHwnd == handle)
            {
                if (!_scannerShown)
                {
                    var arg = new PanelRequestEventArgs(PanelClasses.AlphabetMinimal, monitorInfo)
                    {
                        RequestArg = _form,
                        TargetPanel = _form,
                        UseCurrentScreenAsParent = true
                    };
                    showPanel(this, arg);
                    _scannerShown = true;
                }
            }

            handled = true;
        }

        /// <summary>
        /// A request came in to close the agent. We MUST
        /// quit if this call is ever made
        /// </summary>
        /// <returns>true on success</returns>
        public override bool OnRequestClose()
        {
            quit(false);
            return true;
        }

        /// <summary>
        /// Invoked when there is a request to run a command. This
        /// could as a result of the user activating a button on the
        /// scanner and there is a command associated with the button
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="commandArg">any optional arguments</param>
        /// <param name="handled">was this handled?</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                case "clearText":
                    _form.ClearFilter();
                    break;

                case "CancelEditDelAbbreviation":
                    closeEditDeleteConfirmScanner();
                    break;

                case "EditAbbreviation":
                    closeEditDeleteConfirmScanner();
                    if (_abbreviationSelected != null)
                    {
                        editAbbreviation(_abbreviationSelected);
                    }

                    break;

                case "DeleteAbbreviation":
                    if (_abbreviationSelected != null)
                    {
                        if (DialogUtils.ConfirmScanner("Delete " + _abbreviationSelected.Mnemonic + "?"))
                        {
                            deleteAbbreviation(_abbreviationSelected);
                            _abbreviationSelected = null;
                            closeEditDeleteConfirmScanner();
                        }
                    }

                    break;

                default:
                    Log.Debug(command);
                    if (_form != null)
                    {
                        _form.OnRunCommand(command, ref handled);
                    }

                    break;
            }
        }

        /// <summary>
        /// Creates a text control agent object
        /// </summary>
        /// <param name="handle">handle of the control in focus</param>
        /// <param name="focusedElement">the automation object associated with the control</param>
        /// <param name="handled">was this handled?</param>
        /// <returns>object created</returns>
        protected override TextControlAgentBase createEditControlTextInterface(IntPtr handle, 
                                                                                AutomationElement focusedElement, 
                                                                                ref bool handled)
        {
            return new AbbreviationsTextControlAgent(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Event handler to add an abbreviation
        /// </summary>
        /// <param name="abbr">abbreviation to add</param>
        private void _form_EvtAddAbbreviation(string abbr)
        {
            addAbbreviation(new Abbreviation(abbr, String.Empty, Abbreviation.AbbreviationMode.Speak));
        }

        /// <summary>
        /// Event handler for event raised when the user
        /// wants to quit
        /// </summary>
        /// <param name="flag">Quit if false</param>
        private void _form_EvtDone(bool flag)
        {
            if (!flag)
            {
                quit();
            }
        }

        /// <summary>
        /// User wants to edit an abbreviation. Get confirmation to see
        /// if the user want to edit or delete
        /// </summary>
        /// <param name="abbr">Abbreviation to be edited</param>
        private void _form_EvtEditAbbreviation(Abbreviation abbr)
        {
            if (!_editDeleteMenuShown)
            {
                _abbreviationSelected = abbr;
                _editDeleteConfirmScanner = Context.AppPanelManager.CreatePanel("AbbreviationEditDeleteConfirm", "Abbreviation");
                if (_editDeleteConfirmScanner != null)
                {
                    _editDeleteMenuShown = true;
                    IPanel panel = _editDeleteConfirmScanner as IPanel;
                    Context.AppPanelManager.Show(Context.AppPanelManager.GetCurrentPanel(), panel);
                }
            }
        }

        /// <summary>
        /// Handler for when the abbr scanner is closing. Close
        /// the alphabet scanner as well
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            _form.EvtDone -= _form_EvtDone;
            _form.EvtAddAbbreviation -= _form_EvtAddAbbreviation;
            _form.EvtEditAbbreviation -= _form_EvtEditAbbreviation;

            if (Context.AppPanelManager.GetCurrentPanel() != null)
            {
                Windows.CloseForm(Context.AppPanelManager.GetCurrentPanel() as Form);
            }
        }

        /// <summary>
        /// Adds a new abbreviation to the list, saves
        /// and reloads the abbreviation list
        /// </summary>
        /// <param name="abbr">abbreviation to add</param>
        private void addAbbreviation(Abbreviation abbr)
        {
            Windows.SetVisible(_form, false);

            var operation = new AbbrOperation { InputAbbreviation = abbr, Add = true };

            editOrAddAbbreviation(operation);

            if (!operation.Cancel)
            {
                Context.AppAbbreviations.Add(operation.OutputAbbreviation);
                Context.AppAbbreviations.Save();
                Context.AppAbbreviations.Load();
            }

            _form.LoadAbbreviations();

            Windows.SetVisible(_form, true);
        }

        /// <summary>
        /// Closes the scanner that confirms whether the
        /// user wants to delete or edit an abbreviation
        /// </summary>
        private void closeEditDeleteConfirmScanner()
        {
            _editDeleteMenuShown = false;

            if (_editDeleteConfirmScanner != null)
            {
                Windows.CloseForm(_editDeleteConfirmScanner);
                _editDeleteConfirmScanner = null;
            }
        }

        /// <summary>
        /// Closes the abbreviations scanner
        /// </summary>
        private void closeScanner()
        {
            if (_form != null)
            {
                Windows.CloseForm(_form);
                _form = null;
                _scannerShown = false;
            }
        }

        /// <summary>
        /// Deletes an abbreviation from the list and reloads
        /// the list
        /// </summary>
        /// <param name="abbr">abbreviation to delete</param>
        private void deleteAbbreviation(Abbreviation abbr)
        {
            Context.AppAbbreviations.Remove(abbr.Mnemonic);
            Context.AppAbbreviations.Save();
            Context.AppAbbreviations.Load();
            _form.LoadAbbreviations();
        }

        /// <summary>
        /// Edits an existing abbreviation in the list of
        /// abbreviations and then saves and reloads the list
        /// </summary>
        /// <param name="abbr">abbreviation to be edited</param>
        private void editAbbreviation(Abbreviation abbr)
        {
            Windows.SetVisible(_form, false);

            var operation = new AbbrOperation { InputAbbreviation = abbr, Add = false };

            editOrAddAbbreviation(operation);
            if (!operation.Cancel)
            {
                if (operation.Delete)
                {
                    Context.AppAbbreviations.Remove(abbr.Mnemonic);
                }
                else
                {
                    if (!Context.AppAbbreviations.Exists(operation.OutputAbbreviation.Mnemonic))
                    {
                        Context.AppAbbreviations.Add(operation.OutputAbbreviation);
                    }
                    else
                    {
                        Context.AppAbbreviations.Update(operation.OutputAbbreviation);
                    }
                }

                Context.AppAbbreviations.Save();
                Context.AppAbbreviations.Load();
            }

            _form.LoadAbbreviations();
            Windows.SetVisible(_form, true);
        }

        /// <summary>
        /// Displays the abbreviation dialog box that allows
        /// the user to either edit or delete an abbreviation
        /// </summary>
        /// <param name="operation">type of operation - edit/add/delete</param>
        private void editOrAddAbbreviation(AbbrOperation operation)
        {
            var dlg = Context.AppPanelManager.CreatePanel("AbbreviationEditorForm");
            if (dlg == null)
            {
                return;
            }

            var invoker = (dlg as IExtension).GetInvoker();
            invoker.SetValue("InputAbbreviation", operation.InputAbbreviation);
            invoker.SetValue("Add", operation.Add);

            _form.Pause();
            Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentPanel(), dlg as IPanel);
            _form.Resume();

            bool? canceled = invoker.GetBoolValue("Cancel");
            var outputAbbreviation = invoker.GetValue("OutputAbbreviation") as Abbreviation;
            if (outputAbbreviation != null)
            {
                bool? deleted = invoker.GetBoolValue("Delete");
                if (deleted != null)
                {
                    operation.Cancel = canceled.Value;
                    operation.Delete = deleted.Value;
                }

                operation.OutputAbbreviation = outputAbbreviation;
            }
        }

        /// <summary>
        /// User wants to quit. Show confirmation and quit
        /// if user chooses to
        /// </summary>
        /// <param name="showConfirmDialog"></param>
        private void quit(bool showConfirmDialog = true)
        {
            bool quit = true;

            if (_form == null)
            {
                return;
            }

            if (showConfirmDialog)
            {
                quit = DialogUtils.ConfirmScanner("Close and Exit?");
            }

            if (quit)
            {
                closeScanner();
                Close();
            }
        }
    }

    /// <summary>
    /// Type of operation to perform on the abbreviation. This
    /// object is used as the data object
    /// </summary>
    internal class AbbrOperation
    {
        public bool Add;
        public bool Cancel = true;
        public bool Delete;
        public Abbreviation InputAbbreviation;
        public Abbreviation OutputAbbreviation;
    }
}