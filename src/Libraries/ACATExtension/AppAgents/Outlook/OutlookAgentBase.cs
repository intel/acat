////////////////////////////////////////////////////////////////////////////
// <copyright file="OutlookAgentBase.cs" company="Intel Corporation">
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
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;

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

namespace ACAT.Lib.Extension.AppAgents.Outlook
{
    /// <summary>
    /// The various fields of interest in Outlook
    /// </summary>
    public enum OutlookControlSubType
    {
        Unknown,
        NewEmailToButton,
        NewEmailCCButton,
        NewEmailSendButton,
        NewEmailSubjectField,
        NewEmailMessageBodyField,
        AppointmentSubjectField,
        AppointmentMessageBodyField,
        ContactMessageField,
        TaskSubjectField,
        TaskMessageBodyField,
        OpenNoteMessageBodyField,
        AddressBookSearchField
    }

    /// <summary>
    /// The various windows of interest in Outlook
    /// </summary>
    public enum OutlookWindowTypes
    {
        Unknown,
        EmailMessage,
        NewEmailMessage,
        Inbox,
        Calendar,
        Tasks,
        Contacts,
        Notes,
        OpenNote,
        OpenAppointment,
        AppointmentScheduling,
        OpenContact,
        OpenTask,
        AddressBook,
        AddressBookDetails
    }

    /// <summary>
    /// Base class for the Outlook email application.  Tracks the
    /// currently focused window/field in Outlook and displays the
    /// appropriate contextual menu to enable the user to interact
    /// with Outlook.
    /// Has the ability to support multiple versions of Outlook,
    /// currently only supports Outlook 2010.
    /// </summary>
    public class OutlookAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// The window element (eg button, edit box) that has focus
        /// </summary>
        public OutlookControlSubType outlookControlSubType = OutlookControlSubType.Unknown;

        /// <summary>
        /// The window type that has focus - eg, inbox, tasks, contacts, open email etc
        /// </summary>
        public OutlookWindowTypes outlookWindowType = OutlookWindowTypes.Unknown;

        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// The ControlType of the element that currently has focus
        /// </summary>
        protected ControlType controlType = ControlType.Custom;

        /// <summary>
        /// Inspector object that interfaces with the version of Outlook
        /// installed on the machine
        /// </summary>
        protected IOutlookInspector outlookInspector;

        /// <summary>
        /// Contextual menu title
        /// </summary>
        private const String PanelTitle = "Outlook";

        /// <summary>
        /// Name of the outlook process
        /// </summary>
        private const String ProcessName = "outlook";

        /// <summary>
        /// Name of the file to attach to the email
        /// </summary>
        private string _fileAttachment = String.Empty;

        /// <summary>
        /// Has the scanner for Outlook been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// Initializes an instance of hte class
        /// </summary>
        public OutlookAgentBase()
        {
            outlookInspector = createOutlookInspector();
        }

        /// <summary>
        /// List of processes this agent supports.  This one only
        /// supports Outlook
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(ProcessName) }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            switch (arg.Widget.SubClass)
            {
                case "SwitchTo":
                    arg.Enabled = isTopLevelWindow(outlookWindowType);
                    arg.Handled = true;
                    break;

                case "ApptAppointment":
                    arg.Enabled = (outlookWindowType != OutlookWindowTypes.OpenAppointment);
                    arg.Handled = true;
                    break;

                case "ApptScheduling":
                    arg.Enabled = (outlookWindowType != OutlookWindowTypes.AppointmentScheduling);
                    arg.Handled = true;
                    break;
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
                String panel;
                String title = PanelTitle;

                switch (outlookWindowType)
                {
                    case OutlookWindowTypes.OpenNote:
                        panel = "OutlookOpenNoteContextMenu";
                        break;

                    default:
                        panel = getContextualMenuForWindow(outlookWindowType, ref title);
                        break;
                }

                showPanel(this, new PanelRequestEventArgs(panel, title, monitorInfo));
            }
            else
            {
                showPanel(this, new PanelRequestEventArgs("OutlookContextMenu", PanelTitle, monitorInfo));
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

            outlookWindowType = identifyWindow(monitorInfo, ref outlookControlSubType);

            controlType = monitorInfo.FocusedElement.Current.ControlType;

            Log.Debug("OutlookWindowType: " + outlookWindowType + ", subType: " + outlookControlSubType);

            String title = PanelTitle;

            base.OnFocusChanged(monitorInfo, ref handled);

            if (autoSwitchScanners)
            {
                var panel = getContextualMenuForWindow(outlookWindowType, ref title);
                showPanel(this, new PanelRequestEventArgs(panel, title, monitorInfo));
            }
            else
            {
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
        /// Invoked to run a command from all the Outlook contextual menus
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
                case "GotoInbox":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D1);
                    break;

                case "ShowFolders":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Y);
                    break;

                case "GotoCalendar":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D2);
                    break;

                case "GotoContacts":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D3);
                    break;

                case "GotoTasks":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D4);
                    break;

                case "GotoNotes":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D5);
                    break;

                case "DeleteItem":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D);
                    break;

                case "NewItem":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.N);
                    break;

                case "OpenItem":
                    AgentManager.Instance.Keyboard.Send(Keys.Enter);
                    break;

                case "Down":
                    AgentManager.Instance.Keyboard.Send(Keys.Down);
                    break;

                case "Up":
                    AgentManager.Instance.Keyboard.Send(Keys.Up);
                    break;

                case "Right":
                    AgentManager.Instance.Keyboard.Send(Keys.Right);
                    break;

                case "Left":
                    AgentManager.Instance.Keyboard.Send(Keys.Left);
                    break;

                case "EmailReply":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.R);
                    break;

                case "EmailReplyAll":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.R);
                    break;

                case "EmailForward":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.F);
                    break;

                case "EmailBrowseDelete":
                case "EmailInboxDelete":
                    if (DialogUtils.ConfirmScanner("Confirm Delete?"))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D);
                    }

                    break;

                case "EmailSelectField":
                    if (!actuateStandardControl(controlType))
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        showPanel(this, new PanelRequestEventArgs("Alphabet", monitorInfo));
                    }

                    break;

                case "NextField":
                    gotoNextField();
                    break;

                case "PreviousField":
                    gotoPreviousField();
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
                        AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.S);
                    }

                    break;

                case "EmailBrowseNext":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.OemPeriod);
                    break;

                case "EmailBrowsePrevious":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Oemcomma);
                    break;

                case "EmailClose":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F4);
                    break;

                case "ApptSelectField":
                    if (!actuateStandardControl(controlType))
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        showPanel(this, new PanelRequestEventArgs("Alphabet", monitorInfo));
                    }

                    break;

                case "ApptPreviousField":
                    gotoPreviousField();
                    break;

                case "ApptNextField":
                    gotoNextField();
                    break;

                case "ApptAttendees":
                    AgentManager.Instance.Keyboard.Send(Keys.F10);
                    AgentManager.Instance.Keyboard.Send(Keys.H);
                    AgentManager.Instance.Keyboard.Send(Keys.U);
                    break;

                case "ApptAppointment":
                    AgentManager.Instance.Keyboard.Send(Keys.F10);
                    AgentManager.Instance.Keyboard.Send(Keys.H);
                    AgentManager.Instance.Keyboard.Send(Keys.P);
                    AgentManager.Instance.Keyboard.Send(Keys.P);
                    break;

                case "ApptSend":
                    if (DialogUtils.ConfirmScanner("Confirm Send?"))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.S);
                    }

                    break;

                case "ApptDelete":
                    if (DialogUtils.ConfirmScanner("Confirm Delete?"))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.F10);
                        AgentManager.Instance.Keyboard.Send(Keys.H);
                        AgentManager.Instance.Keyboard.Send(Keys.D);
                    }

                    break;

                case "ApptClose":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F4);
                    break;

                case "ContactSelectField":
                    if (!actuateStandardControl(controlType))
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        showPanel(this, new PanelRequestEventArgs("Alphabet", monitorInfo));
                    }

                    break;

                case "ContactPreviousField":
                    gotoPreviousField();
                    break;

                case "ContactNextField":
                    gotoNextField();
                    break;

                case "ContactSave":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.S);
                    break;

                case "ContactDelete":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D);
                    break;

                case "ContactClose":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F4);
                    break;

                case "TaskSelectField":
                    if (!actuateStandardControl(controlType))
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        showPanel(this, new PanelRequestEventArgs("Alphabet", monitorInfo));
                    }

                    break;

                case "TaskPreviousField":
                    gotoPreviousField();
                    break;

                case "TaskNextField":
                    gotoNextField();
                    break;

                case "TaskSave":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.S);
                    break;

                case "TaskDelete":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D);
                    break;

                case "TaskClose":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F4);
                    break;

                case "Top":
                    AgentManager.Instance.Keyboard.Send(Keys.Home);
                    break;

                case "SwitchTo":
                    showPanel(this, new PanelRequestEventArgs("OutlookMailBoxesContextMenu",
                                                                "Switch To",
                                                                WindowActivityMonitor.GetForegroundWindowInfo(),
                                                                true));
                    break;

                case "EmailAction":
                    showPanel(this, new PanelRequestEventArgs("OutlookEmailActionContextMenu",
                                                            PanelTitle,
                                                            WindowActivityMonitor.GetForegroundWindowInfo(),
                                                            true));
                    break;

                case "AddressBook":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.B);
                    break;

                case "AddressBookSelectField":
                    if (!actuateStandardControl(controlType))
                    {
                        showPanel(this, new PanelRequestEventArgs("Alphabet", WindowActivityMonitor.GetForegroundWindowInfo()));
                    }

                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Actuates a standard windows control such as a button,
        /// checkbox etc.
        /// </summary>
        /// <param name="ctrlType">Type of the control</param>
        /// <returns>true if the element is a windows control</returns>
        protected bool actuateStandardControl(ControlType ctrlType)
        {
            bool handled = true;

            if (ctrlType.Equals(ControlType.Button) ||
                ctrlType.Equals(ControlType.CheckBox) ||
                ctrlType.Equals(ControlType.RadioButton))
            {
                AgentManager.Instance.Keyboard.Send(Keys.Space);
            }
            else if (ctrlType.Equals(ControlType.ListItem))
            {
                AgentManager.Instance.Keyboard.Send(Keys.Enter);
            }
            else
            {
                handled = false;
            }

            return handled;
        }

        /// <summary>
        /// Attaches a file to the current email being composed.
        /// Launches the file browser to get the file to attach  from the user.
        /// Sends a command to bring up the Outlook "Insert File" dialog.
        /// Inserts the filename into the file name field in the dialog
        /// </summary>
        protected async void attachFile()
        {
            _fileAttachment = String.Empty;
            await getFileToAttach();

            if (String.IsNullOrEmpty(_fileAttachment))
            {
                return;
            }

            Thread.Sleep(500);
            EnumWindows.RestoreFocusToTopWindow();
            Thread.Sleep(500);

            AgentManager.Instance.Keyboard.Send(Keys.F10);
            AgentManager.Instance.Keyboard.Send(Keys.H);
            AgentManager.Instance.Keyboard.Send(Keys.A);
            AgentManager.Instance.Keyboard.Send(Keys.F);

            Thread.Sleep(1000);

            for (int ii = 0; ii < 10; ii++)
            {
                var info1 = WindowActivityMonitor.GetForegroundWindowInfo();
                if (info1.Title == "Insert File")
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

            return new OutlookAgentTextInterface(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Creates a keylogger interface for all those edit controls that
        /// don't support the TextPattern.  Depending on the type of the field
        /// we enable/disable abbreviations, smart punctuations, learning and
        /// spell check
        /// </summary>
        /// <param name="handle">handle of the window</param>
        /// <param name="editTextElement">edit control automation element</param>
        /// <returns>the keylogger object</returns>
        protected override TextControlAgentBase createKeyLoggerTextInterface(
                                                                            IntPtr handle,
                                                                            AutomationElement editTextElement)
        {
            Log.Debug("subtype = " + outlookControlSubType);

            TextControlAgentBase textInterface;

            if (isMessageBodyField(outlookControlSubType))
            {
                Log.Debug("creating outlookagentkeylogger with learn, spell and abbr" + outlookControlSubType);

                textInterface = new OutlookAgentKeyLoggerTextInterface();
            }
            else if (isSubjectField(outlookControlSubType))
            {
                Log.Debug("iSSubjectfield creating outlookagentkeylogger WITHOUT learn");
                textInterface = new OutlookAgentKeyLoggerTextInterface(false);
            }
            else
            {
                Log.Debug("creating outlookagentkeylogger WITHOUT learn, spellcheck and smart puncutationss");

                textInterface = new OutlookAgentKeyLoggerTextInterface(false, true, false, false);
            }

            return textInterface;
        }

        /// <summary>
        /// Returns the name of the contextual menu for the window/element
        /// currently in focus.
        /// </summary>
        /// <param name="windowType">Type of window</param>
        /// <param name="title">returns the title</param>
        /// <returns>name of the panel</returns>
        protected String getContextualMenuForWindow(OutlookWindowTypes windowType, ref String title)
        {
            Log.Debug("windowType: " + windowType);

            String scannerName;
            title = PanelTitle;

            switch (windowType)
            {
                case OutlookWindowTypes.EmailMessage:
                    scannerName = "OutlookBrowseEmailContextMenu";
                    title = "Mail";
                    break;

                case OutlookWindowTypes.NewEmailMessage:
                    scannerName = "OutlookNewEmailContextMenu";
                    title = "Mail";
                    break;

                case OutlookWindowTypes.Inbox:
                    scannerName = "OutlookContextMenu";
                    title = "Mail";
                    break;

                case OutlookWindowTypes.Calendar:
                    scannerName = "OutlookContextMenu";
                    title = "Calendar";
                    break;

                case OutlookWindowTypes.Tasks:
                    scannerName = "OutlookContextMenu";
                    title = "Tasks";
                    break;

                case OutlookWindowTypes.Contacts:
                    scannerName = "OutlookContextMenu";
                    title = "Contacts";
                    break;

                case OutlookWindowTypes.Notes:
                    scannerName = "OutlookContextMenu";
                    title = "Notes";
                    break;

                case OutlookWindowTypes.OpenNote:
                    scannerName = PanelClasses.Alphabet;
                    title = "Note";
                    break;

                case OutlookWindowTypes.AppointmentScheduling:
                case OutlookWindowTypes.OpenAppointment:
                    scannerName = "OutlookOpenAppointmentContextMenu";
                    title = "Appointment";
                    break;

                case OutlookWindowTypes.OpenContact:
                    scannerName = "OutlookOpenContactContextMenu";
                    title = "Contact";
                    break;

                case OutlookWindowTypes.OpenTask:
                    scannerName = "OutlookOpenTaskContextMenu";
                    title = "Task";
                    break;

                case OutlookWindowTypes.AddressBookDetails:
                case OutlookWindowTypes.AddressBook:
                    scannerName = "OutlookAddressBookContextMenu";
                    title = "Addr. Book";
                    break;

                default:
                    scannerName = "OutlookContextMenu";
                    break;
            }

            return scannerName;
        }

        /// <summary>
        /// Returns the outlook version install on the client machine.  14.0 is
        /// Outlook 2010
        /// </summary>
        /// <returns>Outlook version, -1 if not installed</returns>
        protected int getOutlookVersion()
        {
            RegistryKey key = null;

            try
            {
                key = Registry.ClassesRoot.OpenSubKey("Outlook.Application\\CurVer", false);
                if (key != null)
                {
                    var version = key.GetValue("", "Outlook.Application.9").ToString();
                    key.Close();

                    var position = version.LastIndexOf(".");
                    if (position >= 0)
                    {
                        version = version.Remove(0, position + 1);
                        return Convert.ToInt32(version);
                    }
                }
            }
            catch
            {
                if (key != null)
                {
                    key.Close();
                }
            }

            return -1;
        }

        /// <summary>
        /// Tabs to the next field in the window
        /// </summary>
        protected void gotoNextField()
        {
            AgentManager.Instance.Keyboard.Send(Keys.Tab);
        }

        /// <summary>
        /// Tabs to the previous field in the window
        /// </summary>
        protected void gotoPreviousField()
        {
            AgentManager.Instance.Keyboard.ExtendedKeyDown(Keys.RShiftKey);
            Thread.Sleep(10);
            AgentManager.Instance.Keyboard.Send(Keys.Tab);
            Thread.Sleep(10);
            AgentManager.Instance.Keyboard.ExtendedKeyUp(Keys.RShiftKey);
        }

        /// <summary>
        /// Identify the top level window in Outlook
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>Type of the window</returns>
        protected virtual OutlookWindowTypes identifyWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return outlookInspector.IdentifyWindow(monitorInfo, ref subType);
        }

        /// <summary>
        /// Is the active window the Appointement scheduling window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        protected virtual bool isAppointmentSchedulingWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return outlookInspector.IsAppointmentSchedulingWindow(monitorInfo, ref subType);
        }

        /// <summary>
        /// Is the active window the Outlook calendar window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        protected virtual bool isCalendarWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return outlookInspector.IsCalendarWindow(monitorInfo);
        }

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        protected virtual bool isContactsWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return outlookInspector.IsContactsWindow(monitorInfo);
        }

        /// <summary>
        /// Is the active window a new or existing open email  window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="isNewMessage">true if this is a new email window</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        protected virtual bool isEmailMsgWindow(WindowActivityMonitorInfo monitorInfo, ref bool isNewMessage, ref OutlookControlSubType subType)
        {
            return outlookInspector.IsEmailMsgWindow(monitorInfo, ref isNewMessage, ref subType);
        }

        /// <summary>
        /// Is the active window the email inbox window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        protected virtual bool isInboxWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return outlookInspector.IsInboxWindow(monitorInfo);
        }

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        protected virtual bool isNotesWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return outlookInspector.IsNotesWindow(monitorInfo);
        }

        /// <summary>
        /// Is the active window an open Appointment window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        protected virtual bool isOpenAppointmentWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return outlookInspector.IsOpenAppointmentWindow(monitorInfo, ref subType);
        }

        /// <summary>
        /// Is the active window an open "Contact" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        protected virtual bool isOpenContactWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return outlookInspector.IsOpenContactWindow(monitorInfo, ref subType);
        }

        /// <summary>
        /// Is the active window an open Outlook "Note" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        protected virtual bool isOpenNoteWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return outlookInspector.IsOpenNoteWindow(monitorInfo, ref subType);
        }

        protected virtual bool isOpenTaskWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return outlookInspector.IsOpenTaskWindow(monitorInfo, ref subType);
        }

        protected bool IsOutlookInstalled()
        {
            return (Type.GetTypeFromProgID("Outlook.Application", false) != null);
        }

        /// <summary>
        /// Is the active window an open Task window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        protected bool isTasksWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return outlookInspector.IsTasksWindow(monitorInfo);
        }

        /// <summary>
        /// Creates the outlook inspector depending on the version
        /// of Outlook installed on the client computer. Returns
        /// default inspector if outlook is not installed or is the
        /// unsupported version.
        /// </summary>
        private IOutlookInspector createOutlookInspector()
        {
            int version = -1;
            IOutlookInspector inspector;

            if (IsOutlookInstalled())
            {
                version = getOutlookVersion();
            }

            switch (version)
            {
                case 14:  // Outlook 2010
                    inspector = new OutlookInspector2010();
                    break;

                default:
                    inspector = new DefaultOutlookInspector();
                    break;
            }

            return inspector;
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
            fileBrowserAgent.GetInvoker().SetValue("IncludeFileExtensions", new[] { "*.", "txt", "doc", "docx" });
            fileBrowserAgent.GetInvoker().SetValue("ActionVerb", "Attach");

            Log.Debug("Calling ActivateAgent");
            await Context.AppAgentMgr.ActivateAgent(fileBrowserAgent as IFunctionalAgent);
            Log.Debug("Returned from ActivateAgent");
            _fileAttachment = fileBrowserAgent.GetInvoker().GetStringValue("SelectedFile");
        }

        /// <summary>
        /// Checks if the specified window element is a message body field
        /// </summary>
        /// <param name="subType">window element</param>
        /// <returns>true if it is</returns>
        private bool isMessageBodyField(OutlookControlSubType subType)
        {
            return subType == OutlookControlSubType.TaskMessageBodyField ||
                   subType == OutlookControlSubType.ContactMessageField ||
                   subType == OutlookControlSubType.NewEmailMessageBodyField ||
                   subType == OutlookControlSubType.AppointmentMessageBodyField;
        }

        /// <summary>
        /// Checks if the specified window element is a 'Subject' field
        /// </summary>
        /// <param name="subType">window element</param>
        /// <returns>true if it is</returns>
        private bool isSubjectField(OutlookControlSubType subType)
        {
            return subType == OutlookControlSubType.TaskSubjectField ||
                   subType == OutlookControlSubType.NewEmailSubjectField ||
                   subType == OutlookControlSubType.AppointmentSubjectField;
        }

        /// <summary>
        /// Checks if the specified window is a top-level window in Outlook
        /// </summary>
        /// <param name="windowType">window type</param>
        /// <returns>true if it is</returns>
        private bool isTopLevelWindow(OutlookWindowTypes windowType)
        {
            return (windowType == OutlookWindowTypes.Tasks || windowType == OutlookWindowTypes.Notes ||
                    windowType == OutlookWindowTypes.Inbox || windowType == OutlookWindowTypes.Contacts ||
                    windowType == OutlookWindowTypes.Calendar);
        }
    }
}