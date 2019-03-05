////////////////////////////////////////////////////////////////////////////
// <copyright file="PhraseSpeakAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.PhraseSpeakAgent
{
    /// <summary>
    /// Functional agent that displays a list of phrases that the
    /// user can pick from and convert to speech.  The list of phrases
    /// comes from the abbreviations file.  It selects only those
    /// abbreviations whose mode is "Speak" and displays them in a list.
    /// </summary>
    [DescriptorAttribute("BAE2CB86-5D8D-4285-A33C-A32DB93AE311",
                        "Text-to-speech Phrase Manager",
                        "PhraseSpeakAgent",
                        "Select phrases from a list of preferred phrases and convert them to speech")]
    public class PhraseSpeakAgent : FunctionalAgentBase
    {
        /// <summary>
        /// The abbreviations form
        /// </summary>
        private PhraseSpeakScanner _phraseSpeakScanner;

        /// <summary>
        /// To prevent reentrancy in PhraseSpeakFormEvtDone()
        /// </summary>
        private volatile bool _inDone;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public PhraseSpeakAgent()
        {
            Instance = this;
        }

        /// <summary>
        /// For the event raised to check if a widget needs to be enabled
        /// </summary>
        /// <param name="arg">event args</param>
        internal delegate void CheckCommandEnabledDelegate(CommandEnabledArg arg);

        /// <summary>
        /// Raised to check if a widget needs to be enabled
        /// </summary>
        internal event CheckCommandEnabledDelegate EvtCommandEnabled;

        /// <summary>
        /// Returns the instance of this agent
        /// </summary>
        public static PhraseSpeakAgent Instance { get; private set; }

        /// <summary>
        /// Get/sets whether "Search" be enabled or not
        /// </summary>
        public bool EnableSearch { get; set; }

        /// <summary>
        /// Gets or sets whether to edit the phrase list
        /// </summary>
        public bool PhraseListEdit { get; set; }

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
        /// Creates the Phrases scanner, subscribes to events
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Activate()
        {
            bool retVal = true;

            ExitCode = CompletionCode.None;
            IsClosing = false;

            if (PhraseListEdit)
            {
                var dlg = Context.AppPanelManager.CreatePanel("PhraseListEditForm");
                IsActive = true;

                Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentPanel(), dlg as IPanel);

                IsClosing = true;
                IsActive = false;

                Close();
            }
            else
            {
                _phraseSpeakScanner = Context.AppPanelManager.CreatePanel("PhraseSpeakScanner") as PhraseSpeakScanner;
                if (_phraseSpeakScanner != null)
                {
                    _phraseSpeakScanner.ShowSearchButton = EnableSearch;

                    subscribeToEvents();

                    IsActive = true;

                    Context.AppPanelManager.ShowDialog(_phraseSpeakScanner);
                }
                else
                {
                    retVal = false;
                }
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
                case "CmdCursorScanner":
                    arg.Enabled = true;
                    break;

                default:
                    if (EvtCommandEnabled != null)
                    {
                        EvtCommandEnabled(arg);
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
                default:
                    Log.Debug(command);
                    if (_phraseSpeakScanner != null)
                    {
                        _phraseSpeakScanner.OnRunCommand(command, ref handled);
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
            return new PhraseSpeakTextControlAgent(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Closes the Phrases scanner
        /// </summary>
        private void closeScanner()
        {
            if (_phraseSpeakScanner != null)
            {
                Windows.CloseForm(_phraseSpeakScanner);
                _phraseSpeakScanner = null;
            }
        }

        /// <summary>
        /// Handler for when the Phrases scanner is closing.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void PhraseSpeakFormClosing(object sender, FormClosingEventArgs e)
        {
            unsubscribeFromEvents();
        }

        /// <summary>
        /// Event handler for event raised when the user
        /// wants to quit
        /// </summary>
        /// <param name="flag">Quit if false</param>
        private void PhraseSpeakFormEvtDone(bool flag)
        {
            if (_inDone)
            {
                return;
            }

            _inDone = true;

            if (!flag)
            {
                quit();
            }

            _inDone = false;
        }

        /// <summary>
        /// Event handler to display the alphabet scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void PhraseSpeakFormEvtShowScanner(object sender, EventArgs e)
        {
            if (_phraseSpeakScanner != null)
            {
                var arg = new PanelRequestEventArgs(PanelClasses.AlphabetMinimal,
                                                    WindowActivityMonitor.GetForegroundWindowInfo())
                {
                    TargetPanel = _phraseSpeakScanner,
                    RequestArg = _phraseSpeakScanner,
                    UseCurrentScreenAsParent = true
                };
                showPanel(this, arg);
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

            if (_phraseSpeakScanner == null)
            {
                return;
            }

            if (showConfirmDialog)
            {
                quit = Confirm(R.GetString("CloseQ"));
            }

            if (quit)
            {
                IsClosing = true;
                IsActive = false;

                closeScanner();
                Close();
            }
        }

        /// <summary>
        /// Subscribes to events
        /// </summary>
        private void subscribeToEvents()
        {
            if (_phraseSpeakScanner != null)
            {
                _phraseSpeakScanner.EvtDone += PhraseSpeakFormEvtDone;
                _phraseSpeakScanner.FormClosing += PhraseSpeakFormClosing;
                _phraseSpeakScanner.EvtShowScanner += PhraseSpeakFormEvtShowScanner;
            }
        }

        /// <summary>
        /// Unsubscribe events
        /// </summary>
        private void unsubscribeFromEvents()
        {
            if (_phraseSpeakScanner != null)
            {
                _phraseSpeakScanner.EvtDone -= PhraseSpeakFormEvtDone;
                _phraseSpeakScanner.FormClosing -= PhraseSpeakFormClosing;
                _phraseSpeakScanner.EvtShowScanner -= PhraseSpeakFormEvtShowScanner;
            }
        }
    }
}