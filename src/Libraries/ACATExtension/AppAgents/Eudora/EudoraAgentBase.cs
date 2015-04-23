////////////////////////////////////////////////////////////////////////////
// <copyright file="EudoraAgentBase.cs" company="Intel Corporation">
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
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

namespace ACAT.Lib.Extension.AppAgents.Eudora
{
    public class EudoraAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// Used to check which widgets should be enabled
        /// </summary>
        private readonly String[] _supportedFeatures = { "ContextualMenu" };

        /// <summary>
        /// Name of the file to attach to the email
        /// </summary>
        private string _fileAttachment = String.Empty;

        /// <summary>
        /// Has the scanner for eudora been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// List of processes this agent supports.  This one only
        /// supports Eudora
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo("eudora") }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            if (String.Compare(arg.Widget.SubClass, "EmailAttachFile", true) == 0)
            {
                WindowActivityMonitorInfo info = WindowActivityMonitor.GetForegroundWindowInfo();
                arg.Enabled = (isMailComposeMessageWindow(info.FocusedElement) ||
                               isMailComposeWindow(info.FocusedElement));
                arg.Handled = true;
            }
            else
            {
                checkWidgetEnabled(_supportedFeatures, arg);
            }
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            if (autoSwitchScanners)
            {
                var panel = isMailComposeWindow(monitorInfo.FocusedElement) ? 
                    "EudoraMailComposeContextMenu" : 
                    getPanelNameForWindow(monitorInfo);

                showPanel(this, new PanelRequestEventArgs(panel, "Eudora", monitorInfo));
            }
            else
            {
                showPanel(this, new PanelRequestEventArgs("EudoraContextMenuSimple", "Eudora", monitorInfo));
            }
        }

        /// <summary>
        /// Invoked when the foreground window focus changes. Display the
        /// scanner depending on the context.
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();
            base.OnFocusChanged(monitorInfo, ref handled);

            if (autoSwitchScanners)
            {
                var panel = getPanelNameForWindow(monitorInfo);
                showPanel(this,
                            !String.IsNullOrEmpty(panel) ? 
                            new PanelRequestEventArgs(panel, "Eudora", monitorInfo): 
                            new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
            }
            else
            {
                // always show alphabet scanner
                if (monitorInfo.IsNewWindow)
                {
                    setEudoraWindowSize();
                }

                if (!_scannerShown)
                {
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                    _scannerShown = true;
                }
            }

            handled = true;
        }

        /// <summary>
        /// Focus shifted to another app.  This agent is
        /// getting deactivated.
        /// </summary>
        public override void OnFocusLost()
        {
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
            Log.Debug(command);
            switch (command)
            {
                case "EmailOpenOrInbox":
                    handleCmdEmailOpenOrInbox();
                    break;

                case "EmailNew":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.N);
                    break;

                case "EmailOpen":
                    AgentManager.Instance.Keyboard.Send(Keys.Enter);
                    break;

                case "EmailNext":
                    AgentManager.Instance.Keyboard.Send(Keys.Down);
                    break;

                case "EmailPrevious":
                    AgentManager.Instance.Keyboard.Send(Keys.Up);
                    break;

                case "EmailNextSimple":
                    handleCmdEmailNextSimple();
                    break;

                case "EmailPreviousSimple":
                    handleCmdEmailPreviousSimple();
                    break;

                case "EmailReply":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.R);
                    break;

                case "EmailReplyAll":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.R);
                    break;

                case "EmailForward":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.M, Keys.F);
                    break;

                case "EmailBrowseDelete":
                    if (DialogUtils.ConfirmScanner("Confirm Delete?"))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D);
                    }

                    break;

                case "EmailInboxDelete":
                    if (DialogUtils.ConfirmScanner("Confirm Delete?"))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.Delete);
                    }

                    break;

                case "EmailEnterText":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        showPanel(this, new PanelRequestEventArgs("Alphabet", monitorInfo));
                        break;
                    }

                case "EmailNextField":
                    handleCmdEmailNextField();
                    break;

                case "EmailPreviousField":
                    handleCmdEmailPreviousField();
                    break;

                case "EmailAttachFile":
                    attachFile();
                    break;

                case "EmailAttachments":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.A);
                    break;

                case "EmailSend":
                    if (DialogUtils.ConfirmScanner("Confirm Send?"))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.E);
                    }

                    break;

                case "EmailBrowseNext":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Down);
                    break;

                case "EmailBrowsePrevious":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Up);
                    break;

                case "EmailClose":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.W);
                    break;

                case "EmailInbox":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D1);
                    break;

                case "EmailOutbox":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D0);
                    break;

                case "EmailJunkbox":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.X);
                    AgentManager.Instance.Keyboard.Send(Keys.J);
                    break;

                case "EmailTrashbox":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.X);
                    AgentManager.Instance.Keyboard.Send(Keys.T);
                    break;

                case "Top":
                    AgentManager.Instance.Keyboard.Send(Keys.Home);
                    break;

                case "MailBoxes":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var arg = new PanelRequestEventArgs("EudoraMailBoxesContextMenu", "Eudora", monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };
                        showPanel(this, arg);
                    }

                    break;

                case "EmailAction":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var arg = new PanelRequestEventArgs("EudoraMailActionContextMenu", "Eudora", monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };
                        showPanel(this, arg);
                    }

                    break;

                case "EmailAddToContacts":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.K);
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Attaches a file to the current email being composed.  The currently
        /// focused window has to be the mail compose window.  Sends a
        /// Ctrl+H command to.  Launches the file browser to get the file to attach
        /// from the user.  Then inserts the filename into the attachment field
        /// in the mail compose window
        /// </summary>
        protected async void attachFile()
        {
            var info = WindowActivityMonitor.GetForegroundWindowInfo();
            if (!isMailComposeMessageWindow(info.FocusedElement) &&
                !isMailComposeWindow(info.FocusedElement))
            {
                Log.Debug("Wrong window");
                return;
            }

            _fileAttachment = String.Empty;
            await getFileToAttach();

            if (String.IsNullOrEmpty(_fileAttachment))
            {
                return;
            }

            Thread.Sleep(500);
            EnumWindows.RestoreFocusToTopWindow();
            Thread.Sleep(500);

            SendKeys.SendWait("^h");
            Thread.Sleep(1000);

            for (int ii = 0; ii < 10; ii++)
            {
                var info1 = WindowActivityMonitor.GetForegroundWindowInfo();
                if (info1.Title == "Attach File")
                {
                    Log.Debug("YES!  Found Attach file window");
                    var automationElement = AgentUtils.GetElementOrAncestorByAutomationId(
                                                                    info1.FocusedElement,
                                                                    "Edit",
                                                                    "ControlType.Edit",
                                                                    "1148");
                    if (automationElement != null)
                    {
                        Log.Debug("element is not null");
                        AgentUtils.InsertTextIntoElement(automationElement, _fileAttachment);
                        SendKeys.Send("{ENTER}");
                    }
                    else
                    {
                        Log.Debug("element is null");
                    }

                    break;
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Creates an edit control text agent to manipulate text
        /// in the target text control that's currently active.
        /// the 'handled' param is set to true if it was
        /// handled successfully.
        /// </summary>
        /// <param name="handle">handle to the active window</param>
        /// <param name="focusedElement">the active text control</param>
        /// <param name="handled">true if handled</param>
        /// <returns>edit text control agent object</returns>
        protected override TextControlAgentBase createEditControlTextInterface(
                                                                            IntPtr handle,
                                                                            AutomationElement focusedElement,
                                                                            ref bool handled)
        {
            Log.Debug();

            if (isSubjectField(focusedElement))
            {
                Log.Debug("subject or message body");
                return base.createEditControlTextInterface(handle, focusedElement, ref handled);
            }

            Log.Debug("Neither subject nor message ");
            return new EudoraAgentTextInterface(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Creates a key logger text interface which uses a shadow
        /// text box to manipulate text in the active window. Key
        /// logger is used for text controls which are NOT edit controls.
        /// We can't track the cursor or scrape text from the control
        /// </summary>
        /// <param name="handle">handle to the active window</param>
        /// <param name="editTextElement">the active text control</param>
        /// <returns>Key logger text agent</returns>

        protected override TextControlAgentBase createKeyLoggerTextInterface(
                                                                            IntPtr handle,
                                                                            AutomationElement editTextElement)
        {
            Log.Debug();
            if (isMessageBodyField(editTextElement))
            {
                Log.Debug("Create custom keylogger");
                return new EudoraAgentKeyLoggerTextInterface();
            }

            Log.Debug("Not message body field");

            return base.createKeyLoggerTextInterface(handle, editTextElement);
        }

        /// <summary>
        /// Depending on which Eudora window currently
        /// has focus, returns the name of the scanner/menu appropriate for
        /// the window.  The caller can then display the scanner.
        /// </summary>
        /// <param name="monitorInfo">active window info</param>
        /// <returns>name of scanner</returns>
        protected String getPanelNameForWindow(WindowActivityMonitorInfo monitorInfo)
        {
            Log.Debug("FocusedElement: ClassName: " + monitorInfo.FocusedElement.Current.ClassName +
                        ", controlType:  " + monitorInfo.FocusedElement.Current.ControlType.ProgrammaticName);
            String scannerName;

            if (isInboxWindow(monitorInfo.FocusedElement))
            {
                Log.Debug("Returning EudoraInboxContextMenu");
                scannerName = "EudoraInboxContextMenu";
            }
            else if (isMailComposeWindow(monitorInfo.FocusedElement))
            {
                Log.Debug("Mailbox window");
                scannerName = "EudoraMailComposeContextMenu";
            }
            else if (isBrowseEmailWindow(monitorInfo.FocusedElement))
            {
                scannerName = "EudoraBrowseMailContextMenu";
            }
            else if (isMailComposeMessageWindow(monitorInfo.FocusedElement))
            {
                Log.Debug("MailMessage window");
                scannerName = PanelClasses.Alphabet;
            }
            else
            {
                Log.Debug("Not a recognized control in Eudora.");
                scannerName = "EudoraMailBoxesContextMenu";
            }

            return scannerName;
        }

        /// <summary>
        /// Checks if the specified element is the attachment field
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isAttachmentField(AutomationElement element)
        {
            return AgentUtils.IsElementOrAncestorByAutomationId(element,
                                                                "Edit",
                                                                "ControlType.Document",
                                                                "5055");
        }

        /// <summary>
        /// Checks if the specified element is the BCC field
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isBCCField(AutomationElement element)
        {
            return AgentUtils.IsElementOrAncestorByAutomationId(element,
                                                                "Edit",
                                                                "ControlType.Document",
                                                                "5054");
        }

        /// <summary>
        /// Checks if the specified element representing the Eudora
        /// window is the Browse email window
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isBrowseEmailWindow(AutomationElement element)
        {
            Log.Debug();
            bool retVal = element != null &&
                            (AgentUtils.IsElementOrAncestor(element, "Internet Explorer_Server", "ControlType.Pane") ||
                            AgentUtils.IsElementOrAncestor(element, "EudoraEdit", "ControlType.Pane"));
            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Checks if the specified element is the CC field
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isCCField(AutomationElement element)
        {
            return AgentUtils.IsElementOrAncestorByAutomationId(element,
                                                                "Edit",
                                                                "ControlType.Document",
                                                                "5053");
        }

        /// <summary>
        /// Checks if the specified element representing the Eudora
        /// window is the Inbox
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isInboxWindow(AutomationElement element)
        {
            Log.Debug();
            bool retVal = element != null &&
                            AgentUtils.IsAncestorByAutomationId(element,
                                                                "ListBox",
                                                                "ControlType.List",
                                                                "1159");
            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Checks if the specified element representing the Eudora
        /// window is the mail composition window
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isMailComposeMessageWindow(AutomationElement element)
        {
            Log.Debug();
            bool retVal = element != null &&
                            AgentUtils.IsElementOrAncestorByAutomationId(element,
                                                                        "EudoraEdit",
                                                                        "ControlType.Pane",
                                                                        "59664");

            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Checks if the specified element representing the Eudora
        /// window is the mail compose window
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isMailComposeWindow(AutomationElement element)
        {
            Log.Debug();
            bool retVal = element != null &&
                            AgentUtils.IsAncestorByAutomationId(element,
                                                                "AfxMDIFrame70",
                                                                "ControlType.Pane",
                                                                "59648");

            if (retVal)
            {
                Log.Debug("Checking if sibling is to fieild");
                retVal = AgentUtils.IsSiblingByAutomationId(element, "Edit", "ControlType.Document", "5050");
                Log.Debug("isSibling returned " + retVal);
            }

            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Checks if the specified element is the message body field
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isMessageBodyField(AutomationElement element)
        {
            return AgentUtils.IsElementOrAncestorByAutomationId(element,
                                                                "EudoraEdit",
                                                                "ControlType.Pane",
                                                                "59664");
        }

        /// <summary>
        /// Checks if the specified element is the subject field
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isSubjectField(AutomationElement element)
        {
            return AgentUtils.IsElementOrAncestorByAutomationId(element,
                                                                "Edit",
                                                                "ControlType.Document",
                                                                "5052");
        }

        /// <summary>
        /// Checks if the specified element is the TO field
        /// </summary>
        /// <param name="element">the automation element</param>
        /// <returns>true if it is</returns>
        protected bool isToField(AutomationElement element)
        {
            return AgentUtils.IsElementOrAncestorByAutomationId(element,
                                                                "Edit",
                                                                "ControlType.Document",
                                                                "5050");
        }

        /// <summary>
        /// Partially maximize the Eudora window
        /// </summary>
        protected void setEudoraWindowSize()
        {
            var processes = Process.GetProcessesByName("eudora");
            if (processes.Length > 0)
            {
                // yes. Eudora is running
                var eudoraProcess = processes[0];
                var mainWindow = eudoraProcess.MainWindowHandle;
                if (mainWindow != IntPtr.Zero)
                {
                    Windows.SetWindowSizePercent(mainWindow,
                                                Context.AppWindowPosition,
                                                Common.AppPreferences.WindowMaximizeSizePercent);
                }
            }
        }

        /// <summary>
        /// Get the list of file extensions to exclude
        /// </summary>
        /// <returns>list, emtpy if nothing to exclude</returns>
        private String[] getExcludeExtensions()
        {
            return (!String.IsNullOrEmpty(Common.AppPreferences.FileBrowserExcludeFileExtensions)) ?
                                Common.AppPreferences.FileBrowserExcludeFileExtensions.Split(';') :
                                new String[] { };
        }

        /// <summary>
        /// Activates the file browser agent to get the
        /// name of the file to attach
        /// </summary>
        /// <returns>the task</returns>
        private async Task getFileToAttach()
        {
            IApplicationAgent fileBrowserAgent = Context.AppAgentMgr.GetAgentByName("FileBrowser Agent");
            if (fileBrowserAgent == null)
            {
                return;
            }

            fileBrowserAgent.GetInvoker().SetValue("AutoLaunchFile", false);
            fileBrowserAgent.GetInvoker().SetValue("SelectActionOpen", true);
            fileBrowserAgent.GetInvoker().SetValue("Folders", Common.AppPreferences.GetFavoriteFolders());//.FavoriteFolders.Split(';'));
            fileBrowserAgent.GetInvoker().SetValue("IncludeFileExtensions", new[] { "*.", "txt", "doc", "docx" });
            fileBrowserAgent.GetInvoker().SetValue("ExcludeFileExtensions", getExcludeExtensions());
            fileBrowserAgent.GetInvoker().SetValue("ActionVerb", "Attach");

            Log.Debug("Calling ActivateAgent");
            await Context.AppAgentMgr.ActivateAgent(fileBrowserAgent as IFunctionalAgent);
            Log.Debug("Returned from ActivateAgent");
            _fileAttachment = fileBrowserAgent.GetInvoker().GetStringValue("SelectedFile");
        }

        /// <summary>
        /// Tabs to the next field in an email window
        /// </summary>
        private void handleCmdEmailNextField()
        {
            var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
            if (!isMailComposeMessageWindow(monitorInfo.FocusedElement))
            {
                AgentManager.Instance.Keyboard.Send(Keys.Tab);
            }
        }

        /// <summary>
        /// Tabs to the next field if in an email, or to the
        /// next mail item if the user is browsing email
        /// </summary>
        private void handleCmdEmailNextSimple()
        {
            var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
            if (isMailComposeWindow(monitorInfo.FocusedElement) ||
                isMailComposeMessageWindow(monitorInfo.FocusedElement))
            {
                if (isToField(monitorInfo.FocusedElement) ||
                    isSubjectField(monitorInfo.FocusedElement) ||
                    isCCField(monitorInfo.FocusedElement) ||
                    isBCCField(monitorInfo.FocusedElement) ||
                    isAttachmentField(monitorInfo.FocusedElement))
                {
                    AgentManager.Instance.Keyboard.Send(Keys.Tab);
                }
            }
            else if (isBrowseEmailWindow(monitorInfo.FocusedElement))
            {
                AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Down);
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(Keys.Down);
            }
        }

        /// <summary>
        /// Opens the email or goes to the inbox
        /// </summary>
        private void handleCmdEmailOpenOrInbox()
        {
            WindowActivityMonitorInfo monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
            if (isInboxWindow(monitorInfo.FocusedElement))
            {
                AgentManager.Instance.Keyboard.Send(Keys.Enter);
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D1);
            }
        }

        /// <summary>
        /// Tabs to the previous field
        /// </summary>
        private void handleCmdEmailPreviousField()
        {
            AgentManager.Instance.Keyboard.ExtendedKeyDown(Keys.RShiftKey);
            Thread.Sleep(10);
            AgentManager.Instance.Keyboard.Send(Keys.Tab);
            Thread.Sleep(10);
            AgentManager.Instance.Keyboard.ExtendedKeyUp(Keys.RShiftKey);
        }

        /// <summary>
        /// Tabs to the previous field if inside an email. If
        /// the user is browsing emails, goes to the previous email
        /// </summary>
        private void handleCmdEmailPreviousSimple()
        {
            var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
            if (isMailComposeWindow(monitorInfo.FocusedElement) ||
                isMailComposeMessageWindow(monitorInfo.FocusedElement))
            {
                if (isToField(monitorInfo.FocusedElement) ||
                    isSubjectField(monitorInfo.FocusedElement) ||
                    isCCField(monitorInfo.FocusedElement) ||
                    isBCCField(monitorInfo.FocusedElement) ||
                    isMessageBodyField(monitorInfo.FocusedElement) ||
                    isAttachmentField(monitorInfo.FocusedElement))
                {
                    AgentManager.Instance.Keyboard.ExtendedKeyDown(Keys.RShiftKey);
                    Thread.Sleep(10);
                    AgentManager.Instance.Keyboard.Send(Keys.Tab);
                    Thread.Sleep(10);
                    AgentManager.Instance.Keyboard.ExtendedKeyUp(Keys.RShiftKey);
                }
            }
            else if (isBrowseEmailWindow(monitorInfo.FocusedElement))
            {
                AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Up);
            }
            else
            {
                AgentManager.Instance.Keyboard.Send(Keys.Up);
            }
        }
    }
}