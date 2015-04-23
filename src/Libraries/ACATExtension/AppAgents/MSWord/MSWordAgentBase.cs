////////////////////////////////////////////////////////////////////////////
// <copyright file="MSWordAgentBase.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
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

namespace ACAT.Lib.Extension.AppAgents.MSWord
{
    /// <summary>
    /// Base class for the Agent for Microsoft Word
    /// </summary>
    public class MSWordAgentBase : AgentBase
    {
        /// <summary>
        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// Is word installed on this computer?
        /// </summary>
        protected bool isWordInstalled;

        /// <summary>
        /// List of features supported by this agent. These
        /// widgets will be enabled in the scanners.
        /// </summary>
        protected String[] supportedFeatures =
        {
            "NewFile",
            "OpenFile",
            "OpenRecentFile",
            "SaveFileAs",
            "Find",
            "ContextualMenu",
            "ZoomIn",
            "ZoomOut",
            "ZoomFit"
        };

        /// <summary>
        /// Class name of the MS Word main window
        /// </summary>
        private const string DocClassName = "_WwG";

        /// <summary>
        /// Name of the ms word process
        /// </summary>
        private const String MSWordProcessName = "winword";

        /// <summary>
        /// Title of the contextual menu
        /// </summary>
        private const String ScannerTitle = "MS Word";

        /// <summary>
        /// Has the scanner been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// The text control agent for ms word
        /// </summary>
        private TextControlAgentBase _textInterface;

        /// <summary>
        /// Word prediction context handle used to add contents
        /// of the MS Word window for temporary learning.
        /// </summary>
        private int _wordPredictionContext;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MSWordAgentBase()
        {
            _textInterface = null;
            isWordInstalled = (Type.GetTypeFromProgID("Word.Application") != null);
        }

        /// <summary>
        /// Returns list of processes supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(MSWordProcessName) }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            checkWidgetEnabled(supportedFeatures, arg);
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showPanel(this, new PanelRequestEventArgs("MSWordContextMenu", ScannerTitle, monitorInfo));
        }

        /// <summary>
        /// Invoked when the foreground window focus changes. Display the
        /// scanner depending on the context. Also, if this is a new window that has
        /// come into focus, add its contents to the word prediction temporary batch model for more
        /// contextual prediction of words
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            if (String.Compare(monitorInfo.FocusedElement.Current.ClassName, DocClassName, true) == 0)
            {
                createMSWordTextInterface();
                if (monitorInfo.IsNewFocusedElement)
                {
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                }

                if (monitorInfo.IsNewWindow)
                {
                    loadWordPredictionContext();
                }
            }
            else if (autoSwitchScanners)
            {
                if (isRecentDocuments(monitorInfo.FocusedElement))
                {
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.DialogContextMenu, ScannerTitle, monitorInfo));
                }
                else if (String.Compare(monitorInfo.FocusedElement.Current.ClassName, "NetUIToolWindow") == 0)
                {
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.MenuContextMenu, ScannerTitle, monitorInfo));
                }
                else
                {
                    handled = false;
                    return;
                }
            }
            else if (!_scannerShown)
            {
                showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                _scannerShown = true;
            }

            handled = true;
        }

        /// <summary>
        /// Focus shifted to another app.  This agent is
        /// getting deactivated.
        /// </summary>
        public override void OnFocusLost()
        {
            unloadWordPredictionContext();
            _scannerShown = false;
        }

        /// <summary>
        /// Invoked to run a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="commandArg">Optional arguments for the command</param>
        /// <param name="handled">set this to true if handled</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "MenuContextClose":
                    AgentManager.Instance.Keyboard.Send(Keys.Escape);
                    AgentManager.Instance.Keyboard.Send(Keys.Escape);
                    AgentManager.Instance.Keyboard.Send(Keys.Escape);
                    break;

                case "SwitchAppWindow":
                    DialogUtils.ShowTaskSwitcher(MSWordProcessName);
                    break;

                case "CmdZoomIn":
                    if (_textInterface is MSWordTextControlAgent)
                    {
                        var textAgent = _textInterface as MSWordTextControlAgent;
                        textAgent.ZoomIn();
                    }

                    break;

                case "CmdZoomOut":
                    if (_textInterface is MSWordTextControlAgent)
                    {
                        var textAgent = _textInterface as MSWordTextControlAgent;
                        textAgent.ZoomOut();
                    }

                    break;

                case "CmdZoomFit":
                    if (_textInterface is MSWordTextControlAgent)
                    {
                        var textAgent = _textInterface as MSWordTextControlAgent;
                        textAgent.ZoomFit();
                    }

                    break;

                case "OpenRecentFile":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F, Keys.R);
                    AgentManager.Instance.Keyboard.Send(Keys.Tab);
                    break;

                case "SaveAs":
                    AgentManager.Instance.Keyboard.Send(Keys.F12);
                    break;

                case "SaveFile":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.S);
                    //DialogUtils.Toast("Saved");
                    break;

                case "MSWordLectureManager":
                    if (_textInterface is MSWordTextControlAgent)
                    {
                        var textAgent = (MSWordTextControlAgent)_textInterface;
                        var text = textAgent.GetTextFromDocument();
                        if (String.IsNullOrEmpty(text.Trim()))
                        {
                            DialogUtils.ShowTimedDialog(PanelManager.Instance.GetCurrentPanel() as Form,
                                                        "Lecture Manager", "Document is empty");
                            break;
                        }

                        if (DialogUtils.ConfirmScanner("Load this document into Lecture Manager?"))
                        {
                            launchLectureManager(textAgent.GetFileName(), text);
                        }
                    }

                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Creates the text control interface for MS Word
        /// </summary>
        protected void createMSWordTextInterface()
        {
            if (_textInterface == null)
            {
                _textInterface = new MSWordTextControlAgent();
                setTextInterface(_textInterface);
                _textInterface.EvtTextChanged += _textInterface_EvtTextChanged;
            }
        }

        /// <summary>
        /// Returns if the currently focused element is the "Recent
        /// Documents" window
        /// </summary>
        /// <param name="focusedElement">focused element</param>
        /// <returns>true if it is</returns>
        protected bool isRecentDocuments(AutomationElement focusedElement)
        {
            return AgentUtils.IsElementOrAncestorByName(focusedElement,
                                                        "NetUIFullpageUIWindow",
                                                        "ControlType.Pane", "");
        }

        /// <summary>
        /// Launches lecture manager with text from the MS Word window. This
        /// function returns only after lecture manager has exited
        /// </summary>
        /// <returns>task</returns>
        protected async void launchLectureManager(String fileName, String text)
        {
            IApplicationAgent agent = Context.AppAgentMgr.GetAgentByName("Lecture Manager Agent");
            if (agent != null)
            {
                IExtension extension = agent;
                extension.GetInvoker().SetValue("LoadFromFile", false);
                if (!String.IsNullOrEmpty(fileName))
                {
                    extension.GetInvoker().SetValue("LectureFile", fileName);
                }

                extension.GetInvoker().SetValue("LectureText", text);

                await Context.AppAgentMgr.ActivateAgent(agent as IFunctionalAgent);
            }
        }

        /// <summary>
        /// Dispose allocated resources
        /// </summary>
        protected override void OnDispose()
        {
            Log.Debug();
            disposeTextInterface();
        }

        /// <summary>
        /// Event handler for when text or caret position in
        /// the MS Word window has changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _textInterface_EvtTextChanged(object sender, TextChangedEventArgs e)
        {
            triggerTextChanged(e.TextInterface);
        }

        /// <summary>
        /// Disposes off the text interface
        /// </summary>
        private void disposeTextInterface()
        {
            if (_textInterface != null)
            {
                _textInterface.EvtTextChanged -= _textInterface_EvtTextChanged;
                _textInterface.Dispose();
                _textInterface = null;
                setTextInterface();
            }
        }

        /// <summary>
        /// Loads the text from the active word document to the
        /// word prediction engine as temporary context for better
        /// word prediction results, that are more relevant to the active
        /// document
        /// </summary>
        private void loadWordPredictionContext()
        {
            var textAgent = _textInterface as MSWordTextControlAgent;
            if (textAgent != null)
            {
                var text = textAgent.GetTextFromDocument(false);
                _wordPredictionContext = Context.AppWordPredictionManager.ActiveWordPredictor.LoadContext(text);
            }
        }

        /// <summary>
        /// Unloaded previously loaded context from the word predictor
        /// </summary>
        private void unloadWordPredictionContext()
        {
            Context.AppWordPredictionManager.ActiveWordPredictor.UnloadContext(_wordPredictionContext);           
        }

    }
}