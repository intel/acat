////////////////////////////////////////////////////////////////////////////
// <copyright file="ACATAgentBase.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Permissions;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.TalkWindowManagement;
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Lib.Extension.AppAgents.ACATApp
{
    /// <summary>
    /// The Application agent for the executing assembly.
    /// </summary>
    public class ACATAgentBase : AgentBase
    {
        /// <summary>
        /// Title for the talk window contextual menu
        /// </summary>
        private const string ContextMenuTitle = "Talk Window";

        /// <summary>
        /// Name of the executing assembly
        /// </summary>
        private readonly String _currentProcessName;

        /// <summary>
        /// Has the scanner for the talk window been displayed?
        /// </summary>
        private bool _talkWindowScannerShown;

        /// <summary>
        /// The text control agent responsbile for handling
        /// editing and caret movement functions
        /// </summary>
        private TextControlAgentBase _textInterface;

        /// <summary>
        /// Does the text-to-speech engine support escape
        /// sequences for control? These can be typed into
        /// the talk window and sent directly to the speech engine
        /// </summary>
        private bool _ttsEngineSupportsSpeechControl;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ACATAgentBase()
        {
            _currentProcessName = Process.GetCurrentProcess().ProcessName.ToLower();

            Context.AppTalkWindowManager.EvtTalkWindowVisibilityChanged += AppTalkWindowManager_EvtTalkWindowVisibilityChanged;
            Context.AppTalkWindowManager.EvtTalkWindowClosed += AppTalkWindowManager_EvtTalkWindowClosed;
        }

        /// <summary>
        /// Gets the list of process supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(_currentProcessName) }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            arg.Handled = true;
            switch (arg.Widget.SubClass)
            {
                case "ClearTalkWindowText":
                    arg.Enabled = !Context.AppTalkWindowManager.IsTalkWindowEmpty();
                    break;

                case "ContextualMenu":
                case "FileBrowserToggle":
                    arg.Enabled = true;
                    break;

                case "SpeechControl":
                    if (_ttsEngineSupportsSpeechControl)
                    {
                        String word;
                        TextControlAgent.GetWordAtCaret(out word);
                        object ret = Context.AppTTSManager.ActiveEngine.GetInvoker().InvokeExtensionMethod("IsValidSpeechControlSequence", word);
                        if (ret is bool)
                        {
                            arg.Enabled = (bool)ret;
                        }
                    }

                    break;

                case "QuickSearch":
                case "GoogleSearch":
                case "WikiSearch":
                    arg.Enabled = !String.IsNullOrEmpty(getTalkWindowSelectedTextOrPara().Trim());
                    break;

                default:
                    arg.Handled = false;
                    break;
            }
        }

        /// <summary>
        /// Request for a contextual menu came in. If talk window is visible
        /// display the contextual menu for it.
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            if (Context.AppTalkWindowManager.IsTalkWindowVisible)
            {
                String panel = "TalkWindowContextMenu";
                showPanel(this, new PanelRequestEventArgs(panel, ContextMenuTitle, monitorInfo));
            }
        }

        /// <summary>
        /// Invoked when the foreground window focus changes. If the foreground
        /// window is the talk window, create the Text control agent for it and
        /// display the Alphabet scanner
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            _ttsEngineSupportsSpeechControl = false;

            if (Context.AppTalkWindowManager.IsTalkWindowVisible &&
                    (monitorInfo.IsNewFocusedElement || monitorInfo.IsNewWindow))
            {
                var invoker = Context.AppTTSManager.ActiveEngine.GetInvoker();
                _ttsEngineSupportsSpeechControl = invoker.SupportsMethod("SpeechControl") &&
                                                    invoker.SupportsMethod("IsValidSpeechControlSequence");

                var automationElement = getTalkTextWinAutomationElement();
                if (automationElement != null)
                {
                    disposeTextInterface();
                    createTalkWindowTextInterface(monitorInfo.FgHwnd, automationElement);
                    if (!_talkWindowScannerShown)
                    {
                        _talkWindowScannerShown = true;
                        showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                    }
                }
            }

            handled = true;
        }

        /// <summary>
        /// Focus shifted to an app not supported by this agent.
        /// Release resources
        /// </summary>
        public override void OnFocusLost()
        {
            disposeTextInterface();
        }

        /// <summary>
        /// Invoked to run a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="commandArg">Optional arguments for the command</param>
        /// <param name="handled">set this to true if handled</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;

            Log.Debug(command);
            switch (command)
            {
                case "TalkWindowZoomIn":
                    Context.AppTalkWindowManager.ZoomIn();
                    break;

                case "TalkWindowZoomOut":
                    Context.AppTalkWindowManager.ZoomOut();
                    break;

                case "TalkWindowZoomDefault":
                    Context.AppTalkWindowManager.ZoomDefault();
                    break;

                case "TalkWindowSaveZoom":
                    if (DialogUtils.ConfirmScanner("Save Font Setting?"))
                    {
                        Context.AppTalkWindowManager.SaveFont();
                    }

                    break;

                case "ClearTalkWindowText":
                    if (Context.AppTalkWindowManager.IsTalkWindowActive)
                    {
                        String text = Context.AppTalkWindowManager.TalkWindowText;
                        if (!String.IsNullOrEmpty(text))
                        {
                            if (DialogUtils.ConfirmScanner("Clear Talk Window?"))
                            {
                                Context.AppTalkWindowManager.Clear();
                            }
                        }
                    }

                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Returns the text from the talk window.  The text
        /// is either one that is currently selected, or the
        /// current paragraph
        /// </summary>
        /// <returns>text</returns>
        protected String getTalkWindowSelectedTextOrPara()
        {
            String retVal = String.Empty;

            try
            {
                if (Context.AppTalkWindowManager.IsTalkWindowVisible)
                {
                    using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                    {
                        retVal = context.TextAgent().GetSelectedText().Trim();
                        if (String.IsNullOrEmpty(retVal))
                        {
                            context.TextAgent().GetParagraphAtCaret(out retVal);
                            retVal = retVal.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return retVal;
        }

        /// <summary>
        /// Invoked on disposal. Cleanup
        /// </summary>
        protected override void OnDispose()
        {
            disposeTextInterface();
        }

        /// <summary>
        /// Event handler for when text or caret position in the text
        /// window changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _textInterface_EvtTextChanged(object sender, TextChangedEventArgs e)
        {
            triggerTextChanged(e.TextInterface);
        }

        /// <summary>
        /// Talk window was closed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void AppTalkWindowManager_EvtTalkWindowClosed(object sender, EventArgs e)
        {
            _talkWindowScannerShown = false;
        }

        /// <summary>
        /// Visibility of talk window changed.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppTalkWindowManager_EvtTalkWindowVisibilityChanged(object sender,
                                                TalkWindowVisibilityChangedEventArgs e)
        {
            if (!e.Visible)
            {
                _talkWindowScannerShown = false;
            }
        }

        /// <summary>
        /// Creates the text interface for the Talk window
        /// </summary>
        /// <param name="handle">Handle to the talk window</param>
        /// <param name="automationElement">talk window automation element</param>
        private void createTalkWindowTextInterface(IntPtr handle, AutomationElement automationElement)
        {
            disposeTextInterface();

            bool handled = false;
            _textInterface = new TalkWindowEditControlTextAgent(handle, automationElement, ref handled);
            _textInterface.EvtTextChanged += _textInterface_EvtTextChanged;
            setTextInterface(_textInterface);
            triggerTextChanged(_textInterface);
        }

        /// <summary>
        /// Disposes text interface
        /// </summary>
        private void disposeTextInterface()
        {
            if (_textInterface != null)
            {
                Log.Debug("Disposing old text interface");
                _textInterface.EvtTextChanged -= _textInterface_EvtTextChanged;
                _textInterface.Dispose();
                _textInterface = null;
                setTextInterface();
            }
        }

        /// <summary>
        /// Gets the automation element of the talk text window
        /// </summary>
        /// <returns>the automatoin element</returns>
        private AutomationElement getTalkTextWinAutomationElement()
        {
            AutomationElement retVal = null;
            TalkWindowManager.Instance.GetTalkWindow().Invoke(new MethodInvoker(delegate
            {
                var talkWindowTextBox = TalkWindowManager.Instance.TalkWindowTextBox;
                retVal = AutomationElement.FromHandle(talkWindowTextBox.Handle);
            }));
            return retVal;
        }
    }
}