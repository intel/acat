////////////////////////////////////////////////////////////////////////////
// <copyright file="LectureManagerAgent.cs" company="Intel Corporation">
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
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
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

namespace ACAT.Extensions.Hawking.FunctionalAgents.LectureManager
{
    /// <summary>
    /// Functional agent for Lecture manager.  User can load a
    /// text or a word document in the lecture manager window and then
    /// 'speak' the document (deliver the lecture).  User can pace the
    /// lecture either by senetence or by para.
    /// </summary>
    [DescriptorAttribute("A921931B-DEAD-4820-8A73-F37E5A96E919", "Lecture Manager Agent", "Agent for Lecture Manager")]
    internal class LectureManagerAgent : FunctionalAgentBase
    {
        /// <summary>
        /// Contextual menu shown while speaking the entire lecture without
        /// pausing
        /// </summary>
        private const String SpeakAllMenuPanel = "LectureManagerSpeakAllMenu";

        /// <summary>
        /// Contextual menu shown while speaking by sentence or para
        /// </summary>
        private const String SpeakMenuPanel = "LectureManagerSpeakMenu";

        /// <summary>
        /// Title of the contextual menu
        /// </summary>
        private const String Title = "Lecture Mgr";

        /// <summary>
        /// Main window of the lecture manager that has the text of the lecture
        /// </summary>
        private static LectureManagerMainForm _lectureMgrForm;

        /// <summary>
        /// We need a contextual menu
        /// </summary>
        private readonly String[] _supportedFeatures = { "ContextualMenu" };

        /// <summary>
        /// The text control agent
        /// </summary>
        private readonly LectureManagerTextControlAgent textInterface = new LectureManagerTextControlAgent();

        /// <summary>
        /// Has the contextual menu for lecture manager been shown yet?
        /// </summary>
        private bool _menuShown;

        /// <summary>
        /// We always position the contextual menu for the lecture manager in
        /// the top right corner of the display.  Lets remember the scanner's
        /// previous default position so we can restore it after the user
        /// exits lecture manager
        /// </summary>
        private Windows.WindowPosition _prevScannerPosition;

        /// <summary>
        /// Gets or sets Name of the file to load the lecture from
        /// </summary>
        public String LectureFile { get; set; }

        /// <summary>
        /// Gets or sets the text of the lecture if not
        /// loading from a file
        /// </summary>
        public String LectureText { get; set; }

        /// <summary>
        /// Gets or sets whether the text is loaded from
        ///  afile or not
        /// </summary>
        public bool LoadFromFile { get; set; }

        /// <summary>
        /// Invoked when the Functional agent is activated.  This is
        /// the entry point.
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Activate()
        {
            _menuShown = false;
            ExitCode = CompletionCode.None;
            _prevScannerPosition = Context.AppWindowPosition;
            Context.AppWindowPosition = Windows.WindowPosition.TopRight;
            var form = new LectureManagerMainForm
            {
                LoadFromFile = this.LoadFromFile,
                LectureFile = this.LectureFile,
                LectureText = this.LectureText
            };

            form.FormClosing += form_FormClosing;

            _lectureMgrForm = form;
            Windows.ShowForm(form);

            return true;
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            if (arg.Widget.SubClass.Equals("Speak", StringComparison.InvariantCultureIgnoreCase))
            {
                if (isMainFormActive())
                {
                    if (_lectureMgrForm.FileLoaded)
                    {
                        if (_lectureMgrForm.Mode == LectureManagerMainForm.SpeechMode.All)
                        {
                            if (_lectureMgrForm.Speaking)
                            {
                                if (isContextMenuText(arg.Widget))
                                {
                                    if (arg.Widget.GetText() != "Pause")
                                    {
                                        arg.Widget.SetText("Pause");
                                    }
                                }
                                else if (isContextMenuIcon(arg.Widget))
                                {
                                    String text = arg.Widget.GetText();
                                    if (text[0] != 0x127)
                                    {
                                        const string s = "\u0127";
                                        arg.Widget.SetText(s);
                                    }
                                }
                            }
                            else if (arg.Widget.GetText() != "Next")
                            {
                                if (isContextMenuText(arg.Widget))
                                {
                                    arg.Widget.SetText("Next");
                                }
                                else if (isContextMenuIcon(arg.Widget))
                                {
                                    arg.Widget.SetText("F");
                                }
                            }

                            arg.Enabled = true;
                            arg.Handled = true;
                        }
                        else
                        {
                            arg.Handled = true;
                            arg.Enabled = !_lectureMgrForm.Speaking;
                        }
                    }
                }
            }

            if (arg.Widget.SubClass.Equals("SpeakMenu", StringComparison.InvariantCultureIgnoreCase))
            {
                if (isMainFormActive())
                {
                    arg.Enabled = _lectureMgrForm.FileLoaded;
                    arg.Handled = true;
                }
            }
            else
            {
                checkWidgetEnabled(_supportedFeatures, arg);
            }
        }

        /// <summary>
        /// Invoked when there is a request to display a contextual
        /// menu
        /// </summary>
        /// <param name="monitorInfo">foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            var arg = new PanelRequestEventArgs(PanelClasses.None, monitorInfo);

            getLectureManagerPanel(arg);

            showPanel(this, arg);
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

            setTextInterface(textInterface);

            if (_menuShown)
            {
                return;
            }

            var arg = new PanelRequestEventArgs(PanelClasses.None, monitorInfo);
            handled = getLectureManagerPanel(arg);

            Log.Debug("handled: " + handled);

            if (handled)
            {
                _menuShown = true;
                Log.Debug("calling showpanel for : " + arg.PanelClass);

                showPanel(this, arg);
            }
        }

        /// <summary>
        /// A request came in to close the agent. We MUST
        /// quit if this call is ever made
        /// </summary>
        /// <returns>true on success</returns>
        public override bool OnRequestClose()
        {
            return false;
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
                case "SpeakMenu":
                    if (isMainFormActive())
                    {
                        if (Confirm("Speak now?"))
                        {
                            PanelRequestEventArgs speakArg;
                            if (_lectureMgrForm.Mode == LectureManagerMainForm.SpeechMode.All)
                            {
                                speakArg = new PanelRequestEventArgs(
                                                        SpeakAllMenuPanel, 
                                                        Title,
                                                        WindowActivityMonitor.GetForegroundWindowInfo())
                                {
                                    UseCurrentScreenAsParent = true
                                };

                                showPanel(this, speakArg);
                            }
                            else
                            {
                                speakArg = new PanelRequestEventArgs(
                                                    SpeakMenuPanel, 
                                                    Title,
                                                    WindowActivityMonitor.GetForegroundWindowInfo())
                                {
                                    UseCurrentScreenAsParent = true
                                };

                                showPanel(this, speakArg);
                            }
                        }
                    }

                    break;

                case "LMTopOfDoc":
                    if (isMainFormActive())
                    {
                        _lectureMgrForm.GoToTop();
                    }

                    break;

                case "LMForward":
                    if (isMainFormActive())
                    {
                        _lectureMgrForm.NavigateForward();
                    }

                    break;

                case "LMBackward":
                    if (isMainFormActive())
                    {
                        _lectureMgrForm.NavigateBackward();
                    }

                    break;

                case "SpeechModeParagraph":
                    if (isMainFormActive())
                    {
                        if (Confirm("Set Paragraph Mode?"))
                        {
                            _lectureMgrForm.Mode = LectureManagerMainForm.SpeechMode.Paragraph;
                            closeCurrentPanel();
                        }
                    }

                    break;

                case "SpeechModeSentence":
                    if (isMainFormActive())
                    {
                        if (Confirm("Set Sentence Mode?"))
                        {
                            _lectureMgrForm.Mode = LectureManagerMainForm.SpeechMode.Sentence;
                            closeCurrentPanel();
                        }
                    }

                    break;

                case "SpeechModeAll":
                    if (isMainFormActive())
                    {
                        if (Confirm("Set All?"))
                        {
                            _lectureMgrForm.Mode = LectureManagerMainForm.SpeechMode.All;
                            closeCurrentPanel();
                        }
                    }

                    break;

                case "SpeakNext":
                    if (isMainFormActive())
                    {
                        _lectureMgrForm.ProcessSpeakNext();
                    }

                    break;

                case "SpeakAll":
                    if (isMainFormActive())
                    {
                        if (_lectureMgrForm.Speaking)
                        {
                            _lectureMgrForm.PauseSpeaking();
                        }
                        else
                        {
                            _lectureMgrForm.ProcessReadAllSpeech();
                        }
                    }

                    break;

                case "leaveSpeak":
                    if (Confirm("Speaking. Leave?"))
                    {
                        closeCurrentPanel();
                        if (isMainFormActive())
                        {
                            _lectureMgrForm.StopIfSpeaking();
                        }
                    }

                    break;

                case "exitLectureManager":
                    if (Confirm("Exit Lecture Manager?"))
                    {
                        closeCurrentPanel();
                        if (_lectureMgrForm != null)
                        {
                            Windows.CloseForm(_lectureMgrForm);
                            _lectureMgrForm = null;
                        }
                        Context.AppWindowPosition = _prevScannerPosition;
                        Close();
                        Log.Debug("setting _menushown to false ");

                        _menuShown = false;
                    }

                    break;

                case "ToggleMode":
                    var toggleModeArg = new PanelRequestEventArgs("LectureManagerToggleModeMenu", Title,
                                    WindowActivityMonitor.GetForegroundWindowInfo()) { UseCurrentScreenAsParent = true };
                    showPanel(this, toggleModeArg);
                    break;

                case "NavigateMenu":
                    var navigateMenuArg = new PanelRequestEventArgs("LectureManagerNavigationMenu", Title,
                                            WindowActivityMonitor.GetForegroundWindowInfo()) { UseCurrentScreenAsParent = true };
                    showPanel(this, navigateMenuArg);
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Gets confirmation from the user
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>true on yes</returns>
        internal static bool Confirm(String prompt)
        {
            return DialogUtils.ConfirmScannerNarrow(PanelManager.Instance.GetCurrentPanel(), prompt, Title);
        }

        /// <summary>
        /// Closes the current scanner
        /// </summary>
        private void closeCurrentPanel()
        {
            Form currentPanel = PanelManager.Instance.GetCurrentPanel() as Form;
            Windows.CloseForm(currentPanel);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log.Debug("setting _menushown to false ");

            _menuShown = false;
            _lectureMgrForm = null;
        }

        /// <summary>
        /// Depending on the context, returns the appropriate scanner
        /// to activate
        /// </summary>
        /// <param name="arg">contextual info</param>
        /// <returns>true on success</returns>
        private bool getLectureManagerPanel(PanelRequestEventArgs arg)
        {
            arg.PanelClass = "LectureManagerContextMenuSimple";
            arg.Title = "Lecture Mgr";
            return true;
        }

        /// <summary>
        /// Checks if the specified widget is a context menu
        /// icon widget
        /// </summary>
        /// <param name="widget">widget</param>
        /// <returns>true if it is</returns>
        private bool isContextMenuIcon(Widget widget)
        {
            return widget.Name.Contains("ItemIcon");
        }

        /// <summary>
        /// Checks if the specified widget is a context menu
        /// text widget
        /// </summary>
        /// <param name="widget">widget</param>
        /// <returns>true if it is</returns>
        private bool isContextMenuText(Widget widget)
        {
            return widget.Name.Contains("ItemText");
        }

        /// <summary>
        /// Returns if the lecture manager form is active
        /// </summary>
        /// <returns>true if it is</returns>
        private bool isMainFormActive()
        {
            return _lectureMgrForm != null && _lectureMgrForm.FormLoaded;
        }
    }
}