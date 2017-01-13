////////////////////////////////////////////////////////////////////////////
// <copyright file="AbbreviationsAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.AbbreviationsAgent
{
    /// <summary>
    /// Functional agent that manages the abbreviations scanner
    /// which displays a list of abbreviations.   The user
    /// can add, delete or modify abbreviations.
    /// </summary>
    [DescriptorAttribute("4AD7D574-9C8F-4ED7-9152-F836F210F68E",
                            "Abbreviations Editor",
                            "AbbreviationsAgent",
                            "Creates/edits/deletes abbreviations")]
    public class AbbreviationsAgent : FunctionalAgentBase
    {
        /// <summary>
        /// The abbreviation selected for editing
        /// </summary>
        private Abbreviation _abbreviationSelected;

        /// <summary>
        /// The abbreviations form
        /// </summary>
        private AbbreviationsScanner _abbrForm;

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
        /// Get confirmation from the user
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>if the user said yes</returns>
        public static bool Confirm(String prompt)
        {
            return DialogUtils.ConfirmScanner(PanelManager.Instance.GetCurrentForm(), prompt);
        }

        /// <summary>
        /// Invoked when the Functional agent is activated.  This is
        /// the entry point for the functional agent.
        /// Creates the abbreviations scanner and displays
        /// it.  Subscribes to events.
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Activate()
        {
            bool retVal = true;

            ExitCode = CompletionCode.None;
            IsClosing = false;

            _abbrForm = Context.AppPanelManager.CreatePanel("AbbreviationsScanner") as AbbreviationsScanner;
            if (_abbrForm != null)
            {
                subscribeToEvents();

                IsActive = true;

                Context.AppPanelManager.ShowDialog(_abbrForm);
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
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            arg.Handled = true;

            switch (arg.Command)
            {
                case "CmdPunctuationScanner":
                case "CmdNumberScanner":
                    arg.Enabled = true;
                    break;

                default:

                    if (_abbrForm != null && !Windows.GetVisible(_abbrForm))
                    {
                        arg.Handled = false;
                        return;
                    }

                    if (_abbrForm != null && Windows.GetVisible(_abbrForm))
                    {
                        _abbrForm.CheckCommandEnabled(arg);
                    }

                    if (!arg.Handled)
                    {
                        arg.Enabled = false;
                        arg.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            if (IsClosing)
            {
                Log.Debug("IsClosing is true.  Will not handle the focus change");
                return;
            }

            base.OnFocusChanged(monitorInfo, ref handled);

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
                        if (Confirm(String.Format(R.GetString("DeleteAbbr"), _abbreviationSelected.Mnemonic)))
                        {
                            deleteAbbreviation(_abbreviationSelected);
                            _abbreviationSelected = null;
                            closeEditDeleteConfirmScanner();
                        }
                    }

                    break;

                default:
                    Log.Debug(command);
                    if (_abbrForm != null)
                    {
                        _abbrForm.OnRunCommand(command, ref handled);
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
        /// Handler for when the abbr scanner is closing.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void AbbrFormAbbrFormClosing(object sender, FormClosingEventArgs e)
        {
            unsubscribeFromEvents();
        }

        /// <summary>
        /// Event handler to add an abbreviation
        /// </summary>
        /// <param name="abbr">abbreviation to add</param>
        private void AbbrFormEvtAddAbbreviation(string abbr)
        {
            addAbbreviation(new Abbreviation(abbr, String.Empty, Abbreviation.AbbreviationMode.Speak));
        }

        /// <summary>
        /// Event handler for event raised when the user
        /// wants to quit
        /// </summary>
        /// <param name="dontQuit">Don't quit if true</param>
        private void AbbrFormEvtDone(bool dontQuit)
        {
            // don't you love double negatives?
            if (!dontQuit)
            {
                quit();
            }
        }

        /// <summary>
        /// User wants to edit an abbreviation. Get confirmation to see
        /// if the user want to edit or delete
        /// </summary>
        /// <param name="abbr">Abbreviation to be edited</param>
        private void AbbrFormEvtEditAbbreviation(Abbreviation abbr)
        {
            if (!_editDeleteMenuShown)
            {
                _abbreviationSelected = abbr;
                _editDeleteConfirmScanner = Context.AppPanelManager.CreatePanel("AbbreviationEditDeleteConfirm", abbr.Mnemonic);
                if (_editDeleteConfirmScanner != null)
                {
                    _editDeleteMenuShown = true;
                    IPanel panel = _editDeleteConfirmScanner as IPanel;
                    Context.AppPanelManager.Show(Context.AppPanelManager.GetCurrentForm(), panel);
                }
            }
        }

        /// <summary>
        /// Event handler to display the alphabet scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AbbrFormEvtShowScanner(object sender, EventArgs e)
        {
            if (_abbrForm != null)
            {
                var arg = new PanelRequestEventArgs(PanelClasses.AlphabetMinimal,
                                                    WindowActivityMonitor.GetForegroundWindowInfo())
                {
                    TargetPanel = _abbrForm,
                    RequestArg = _abbrForm,
                    UseCurrentScreenAsParent = true
                };
                showPanel(this, arg);
            }
        }

        /// <summary>
        /// Adds a new abbreviation to the list, saves
        /// and reloads the abbreviation list
        /// </summary>
        /// <param name="abbr">abbreviation to add</param>
        private void addAbbreviation(Abbreviation abbr)
        {
            Windows.SetVisible(_abbrForm, false);

            var operation = new AbbrOperation { InputAbbreviation = abbr, Add = true };

            editOrAddAbbreviation(operation);

            if (!operation.Cancel)
            {
                Context.AppAbbreviationsManager.Abbreviations.Add(operation.OutputAbbreviation);
                Context.AppAbbreviationsManager.Abbreviations.Save();
                Context.AppAbbreviationsManager.Abbreviations.Load();
            }

            _abbrForm.LoadAbbreviations();

            Windows.SetVisible(_abbrForm, true);
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
            if (_abbrForm != null)
            {
                Windows.CloseForm(_abbrForm);
                _abbrForm = null;
            }
        }

        /// <summary>
        /// Deletes an abbreviation from the list and reloads
        /// the list
        /// </summary>
        /// <param name="abbr">abbreviation to delete</param>
        private void deleteAbbreviation(Abbreviation abbr)
        {
            Context.AppAbbreviationsManager.Abbreviations.Remove(abbr.Mnemonic);
            Context.AppAbbreviationsManager.Abbreviations.Save();
            Context.AppAbbreviationsManager.Abbreviations.Load();
            _abbrForm.LoadAbbreviations();
        }

        /// <summary>
        /// Edits an existing abbreviation in the list of
        /// abbreviations and then saves and reloads the list
        /// </summary>
        /// <param name="abbr">abbreviation to be edited</param>
        private void editAbbreviation(Abbreviation abbr)
        {
            Windows.SetVisible(_abbrForm, false);

            var operation = new AbbrOperation { InputAbbreviation = abbr, Add = false };

            editOrAddAbbreviation(operation);
            if (!operation.Cancel)
            {
                if (operation.Delete)
                {
                    Context.AppAbbreviationsManager.Abbreviations.Remove(abbr.Mnemonic);
                }
                else
                {
                    if (!Context.AppAbbreviationsManager.Abbreviations.Exists(operation.OutputAbbreviation.Mnemonic))
                    {
                        Context.AppAbbreviationsManager.Abbreviations.Add(operation.OutputAbbreviation);
                    }
                    else
                    {
                        Context.AppAbbreviationsManager.Abbreviations.Update(operation.OutputAbbreviation);
                    }
                }

                Context.AppAbbreviationsManager.Abbreviations.Save();
                Context.AppAbbreviationsManager.Abbreviations.Load();
            }

            _abbrForm.LoadAbbreviations();
            Windows.SetVisible(_abbrForm, true);
        }

        /// <summary>
        /// Displays the abbreviation dialog box that allows
        /// the user to either edit or delete an abbreviation
        /// </summary>
        /// <param name="operation">type of operation - edit/add/delete</param>
        private void editOrAddAbbreviation(AbbrOperation operation)
        {
            var dlg = Context.AppPanelManager.CreatePanel("AbbreviationEditorForm");
            if (!(dlg is IExtension))
            {
                return;
            }

            var invoker = (dlg as IExtension).GetInvoker();
            invoker.SetValue("InputAbbreviation", operation.InputAbbreviation);
            invoker.SetValue("Add", operation.Add);

            _abbrForm.Pause();
            Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentPanel(), dlg as IPanel);
            _abbrForm.Resume();

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

            if (_abbrForm == null)
            {
                return;
            }

            if (showConfirmDialog)
            {
                quit = Confirm(R.GetString("CloseQ"));
            }

            if (quit)
            {
                IsActive = false;
                IsClosing = true;

                closeScanner();
                Close();
            }
        }

        /// <summary>
        /// Subscribes to events
        /// </summary>
        private void subscribeToEvents()
        {
            if (_abbrForm != null)
            {
                _abbrForm.EvtDone += AbbrFormEvtDone;
                _abbrForm.EvtAddAbbreviation += AbbrFormEvtAddAbbreviation;
                _abbrForm.EvtEditAbbreviation += AbbrFormEvtEditAbbreviation;
                _abbrForm.FormClosing += AbbrFormAbbrFormClosing;
                _abbrForm.EvtShowScanner += AbbrFormEvtShowScanner;
            }
        }

        /// <summary>
        /// Unsubscribe events
        /// </summary>
        private void unsubscribeFromEvents()
        {
            if (_abbrForm != null)
            {
                _abbrForm.EvtDone -= AbbrFormEvtDone;
                _abbrForm.EvtAddAbbreviation -= AbbrFormEvtAddAbbreviation;
                _abbrForm.EvtEditAbbreviation -= AbbrFormEvtEditAbbreviation;
                _abbrForm.FormClosing -= AbbrFormAbbrFormClosing;
                _abbrForm.EvtShowScanner -= AbbrFormEvtShowScanner;
            }
        }
    }

    /// <summary>
    /// Type of operation to perform on the abbreviation. This
    /// object is used as the meta-data object
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