////////////////////////////////////////////////////////////////////////////
// <copyright file="LectureManagerAgent.cs" company="Intel Corporation">
//
// Copyright (c) 2014 Intel Corporation 
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
using ACAT.Lib.Core.ScreenManagement;
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
#endregion

namespace ACAT.Extensions.Hawking.FunctionalAgents.LectureManager
{
    [DescriptorAttribute("A921931B-DEAD-4820-8A73-F37E5A96E919", "Lecture Manager Agent", "Agent for Lecture Manager")]
    internal class LectureManagerAgent : FunctionalAgentBase
    {
        private const String _title = "Lecture Mgr";
        private const String SpeakMenuPanel = "LectureManagerSpeakMenu";
        private const String SpeakAllMenuPanel = "LectureManagerSpeakAllMenu";
        private static LectureManagerMainForm _lectureMgrForm;
        private readonly String[] _supportedFeatures = { "ContextualMenu" };
        private LectureManagerTextControlAgent textInterface = new LectureManagerTextControlAgent();
        private bool _menuShown = false;

        public LectureManagerAgent()
        {
        }

        public bool LoadFromFile { get; set; }

        public String LectureFile { get; set; }

        public String LectureText { get; set; }

        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug("OnFocus: " + monitorInfo.ToString());

            base.OnFocusChanged(monitorInfo, ref handled);

            setTextInterface(textInterface);

            if (!_menuShown)
            {
                var arg = new PanelRequestEventArgs(PanelClasses.None, monitorInfo);
                handled = getPanel(arg);

                if (handled)
                {
                    _menuShown = true;
                    showPanel(this, arg);
                }
            }
        }

        public override bool Activate()
        {
            ExitCode = CompletionCode.None;
            LectureManagerMainForm form = new LectureManagerMainForm();
            form.FormClosing += new FormClosingEventHandler(form_FormClosing);
            form.LoadFromFile = LoadFromFile;
            form.LectureFile = LectureFile;
            form.LectureText = LectureText;
            _lectureMgrForm = form;
            Windows.ShowForm(form);
            return true;
        }

        public override bool OnRequestClose()
        {
            return false;
        }

        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            PanelRequestEventArgs arg = new PanelRequestEventArgs(PanelClasses.None, monitorInfo);

            getPanel(arg);

            showPanel(this, arg);
        }

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
                            if (_lectureMgrForm.Mode == LectureManagerMainForm.SpeechMode.All)
                            {
                                var arg2 = new PanelRequestEventArgs(SpeakAllMenuPanel, _title, WindowActivityMonitor.GetForegroundWindowInfo());
                                arg2.UseCurrentScreenAsParent = true;
                                showPanel(this, arg2);
                            }
                            else
                            {
                                var arg2 = new PanelRequestEventArgs(SpeakMenuPanel, _title, WindowActivityMonitor.GetForegroundWindowInfo());
                                arg2.UseCurrentScreenAsParent = true;
                                showPanel(this, arg2);
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

                        Close();
                    }

                    break;

                case "ToggleMode":
                    PanelRequestEventArgs arg21 = new PanelRequestEventArgs("LectureManagerToggleModeMenu", _title, WindowActivityMonitor.GetForegroundWindowInfo());
                    arg21.UseCurrentScreenAsParent = true;
                    showPanel(this, arg21);
                    break;

                case "NavigateMenu":
                    PanelRequestEventArgs arg1 = new PanelRequestEventArgs("LectureManagerNavigationMenu", _title, WindowActivityMonitor.GetForegroundWindowInfo());
                    arg1.UseCurrentScreenAsParent = true;
                    showPanel(this, arg1);
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

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
                                        String s = "\u0127";
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
                base.checkWidgetEnabled(_supportedFeatures, arg);
            }
        }

        internal static bool Confirm(String prompt)
        {
            return DialogUtils.ConfirmScannerNarrow(ScreenManager.Instance.GetCurrentPanel(), prompt, _title);
        }

        private void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            _menuShown = false;
            _lectureMgrForm = null;
        }
        
        private bool getPanel(PanelRequestEventArgs arg)
        {
            arg.PanelClass = "LectureManagerContextMenuSimple";
            arg.Title = "Lecture Mgr";
            return true;
        }

        private bool isMainFormActive()
        {
            return _lectureMgrForm != null && _lectureMgrForm.FormLoaded;
        }

        private void closeCurrentPanel()
        {
            Form currentPanel = ScreenManager.Instance.GetCurrentPanel() as Form;
            Windows.CloseForm(currentPanel);
        }

        private bool isContextMenuText(Widget widget)
        {
            return widget.Name.Contains("ItemText");
        }

        private bool isContextMenuIcon(Widget widget)
        {
            return widget.Name.Contains("ItemIcon");
        }
    }
}
