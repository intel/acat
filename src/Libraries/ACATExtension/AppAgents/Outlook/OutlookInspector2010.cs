////////////////////////////////////////////////////////////////////////////
// <copyright file="OutlookInspector2010.cs" company="Intel Corporation">
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
using System.Windows.Automation;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Utility;
using System.Diagnostics.CodeAnalysis;

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
    /// Interface to support 2010 version of Outlook.  Has methods
    /// to inspect the active window/ control elements and notify the
    /// agent which Outlook window/control has focus
    /// </summary>
    internal class OutlookInspector2010 : IOutlookInspector
    {
        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public OutlookWindowTypes IdentifyWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            var windowType = OutlookWindowTypes.Unknown;

            bool isNewEmail = false;

            subType = OutlookControlSubType.Unknown;

            if (IsEmailMsgWindow(monitorInfo, ref isNewEmail, ref subType))
            {
                windowType = (isNewEmail) ? OutlookWindowTypes.NewEmailMessage : OutlookWindowTypes.EmailMessage;
            }
            else if (IsTasksWindow(monitorInfo))
            {
                windowType = OutlookWindowTypes.Tasks;
            }
            else if (IsNotesWindow(monitorInfo))
            {
                windowType = OutlookWindowTypes.Notes;
            }
            else if (IsContactsWindow(monitorInfo))
            {
                windowType = OutlookWindowTypes.Contacts;
            }
            else if (IsCalendarWindow(monitorInfo))
            {
                windowType = OutlookWindowTypes.Calendar;
            }
            else if (IsInboxWindow(monitorInfo))
            {
                windowType = OutlookWindowTypes.Inbox;
            }
            else if (IsOpenNoteWindow(monitorInfo, ref subType))
            {
                windowType = OutlookWindowTypes.OpenNote;
            }
            else if (IsOpenAppointmentWindow(monitorInfo, ref subType))
            {
                windowType = OutlookWindowTypes.OpenAppointment;
            }
            else if (IsAppointmentSchedulingWindow(monitorInfo, ref subType))
            {
                windowType = OutlookWindowTypes.AppointmentScheduling;
            }
            else if (IsOpenContactWindow(monitorInfo, ref subType))
            {
                windowType = OutlookWindowTypes.OpenContact;
            }
            else if (IsOpenTaskWindow(monitorInfo, ref subType))
            {
                windowType = OutlookWindowTypes.OpenTask;
            }
            else if (IsAddressBookWindow(monitorInfo, ref subType))
            {
                windowType = OutlookWindowTypes.AddressBook;
            }
            else if (IsAddressBookDetailsWindow(monitorInfo))
            {
                windowType = OutlookWindowTypes.AddressBookDetails;
            }

            return windowType;
        }

        /// <summary>
        /// Is the active window the details window that is 
        /// displayed when the user clicks on a name in the
        /// address book search results
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsAddressBookDetailsWindow(WindowActivityMonitorInfo monitorInfo)
        {
            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);

            var element = AgentUtils.FindElementByAutomationId(windowElement, "Edit", "ControlType.Edit", "2002", "First:") ??
                            AgentUtils.FindElementByAutomationId(windowElement, "Edit", "ControlType.Edit", "2006", "Last:") ??
                            AgentUtils.FindElementByAutomationId(windowElement, "Button", "ControlType.Button", "108", "Add to Contacts");


            return element != null;

        }

        /// <summary>
        /// Is the active window the Outlook Address book window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsAddressBookWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);

            var retVal = (windowElement != null) && (monitorInfo.Title.StartsWith("Address Book") &&
                                                      AgentUtils.IsElementByAutomationId(windowElement, "OutexABCLS",
                                                          "ControlType.Window", String.Empty));

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Edit", "101", "Search:"))
                {
                    subType = OutlookControlSubType.AddressBookSearchField;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Is the active window the Appointement scheduling window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsAppointmentSchedulingWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            Log.Debug();

            subType = OutlookControlSubType.Unknown;

            bool retVal = true;

            if (monitorInfo.FocusedElement == null)
            {
                return false;
            }

            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);
            var element = (AgentUtils.FindElementByAutomationId(windowElement, "SUPERGRID", ControlType.List, "4542", "All Attendees") ??
                           AgentUtils.FindElementByAutomationId(windowElement, "Button", ControlType.Button, "4370", "Add Attendees...")) ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RIchEdit20WPT", ControlType.Edit, "4098", "Meeting Start Date:");

            retVal = (element != null);

            Log.Debug("Returning " + retVal);

            return retVal;
        }

        /// <summary>
        /// Is the active window the Outlook calendar window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsCalendarWindow(WindowActivityMonitorInfo monitorInfo)
        {
            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);

            var element = AgentUtils.FindElementByAutomationId(windowElement, "AfxWndW", ControlType.Text, "");

            if (element != null)
            {
                Log.Debug("&&&&&& " + element.Current.Name);
                return element.Current.Name.StartsWith("Calendar ");
            }

            return false;
        }

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsContactsWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return (monitorInfo.Title.StartsWith("Contacts - ") && monitorInfo.Title.EndsWith("Microsoft Outlook"));
        }

        /// <summary>
        /// Is the active window a new or existing open email  window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="isNewMessage">true if this is a new email window</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsEmailMsgWindow(WindowActivityMonitorInfo monitorInfo, ref bool isNewMessage, ref OutlookControlSubType subType)
        {
            Log.Debug();

            isNewMessage = false;

            subType = OutlookControlSubType.Unknown;

            bool retVal = true;

            if (monitorInfo.FocusedElement == null)
            {
                return false;
            }

            if (!monitorInfo.Title.Contains(" - Message (HTML)") &&
                !monitorInfo.Title.Contains(" - Message (Plain Text)") &&
                !monitorInfo.Title.Contains(" - Message (Rich Text)"))
            {
                Log.Debug("Title: " + monitorInfo.Title);
                Log.Debug("****  TITLE DOES NOT MATCH.  RETURNING ");
                return false;
            }

            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);
            var element = AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Text, "4099") ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4099");

            if (element != null)
            {
                var patterns = element.GetSupportedPatterns();
                foreach (var pattern in patterns)
                {
                    if (String.Compare(pattern.ProgrammaticName, "ValuePatternIdentifiers.Pattern", true) == 0)
                    {
                        bool obj = (bool)element.GetCurrentPropertyValue(ValuePatternIdentifiers.IsReadOnlyProperty);

                        isNewMessage = !obj;

                        if (isNewMessage)
                        {
                            if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "Button",
                                                                        "ControlType.Button", "4352"))
                            {
                                subType = OutlookControlSubType.NewEmailToButton;
                            }
                            else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "Button",
                                                                        "ControlType.Button", "4353"))
                            {
                                subType = OutlookControlSubType.NewEmailCCButton;
                            }
                            else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "Button",
                                                                        "ControlType.Button", "4256"))
                            {
                                subType = OutlookControlSubType.NewEmailCCButton;
                            }
                            else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Edit", "4101"))
                            {
                                subType = OutlookControlSubType.NewEmailSubjectField;
                            }
                            else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "_WwG", "ControlType.Pane", "", "Message"))
                            {
                                subType = OutlookControlSubType.NewEmailMessageBodyField;
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Debug("Could not find element");
                retVal = false;
            }

            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Is the active window the email inbox window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsInboxWindow(WindowActivityMonitorInfo monitorInfo)
        {
            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);

            var element = AgentUtils.FindElementByAutomationId(windowElement, "SuperGrid", ControlType.Table, "4704");

            return element != null;
        }

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsNotesWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return (monitorInfo.Title.StartsWith("Notes - ") && monitorInfo.Title.EndsWith("Microsoft Outlook"));
        }

        /// <summary>
        /// Is the active window an open Appointment window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenAppointmentWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            Log.Debug();

            subType = OutlookControlSubType.Unknown;

            bool retVal = true;

            if (monitorInfo.FocusedElement == null)
            {
                return false;
            }

            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);
            var element = AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.ComboBox, "4098", "Start Date:") ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4100", "Subject:");

            retVal = (element != null);

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Edit", "4100", "Subject:"))
                {
                    subType = OutlookControlSubType.AppointmentSubjectField;
                }
                else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "_WwG", "ControlType.Pane", "", "Message"))
                {
                    subType = OutlookControlSubType.AppointmentMessageBodyField;
                }
            }

            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Is the active window an open "Contact" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenContactWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            Log.Debug();

            subType = OutlookControlSubType.Unknown;

            bool retVal = true;

            if (monitorInfo.FocusedElement == null)
            {
                return false;
            }

            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);
            var element = (AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4481", "Company:") ??
                           AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4480", "Job Title:")) ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4096", "FullName");

            retVal = (element != null);

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "_WwG", "ControlType.Pane", "", "Message"))
                {
                    subType = OutlookControlSubType.ContactMessageField;
                }
            }

            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Is the active window an open Outlook "Note" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenNoteWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            bool retVal = false;

            subType = OutlookControlSubType.Unknown;

            var element = AgentUtils.GetElementOrAncestorByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Edit", "4159");
            if (element != null)
            {
                var name = element.Current.Name;
                retVal = (name == "Note text");
            }

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Edit", "4159"))
                {
                    subType = OutlookControlSubType.OpenNoteMessageBodyField;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Is the active window an open "Task" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenTaskWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            Log.Debug();

            subType = OutlookControlSubType.Unknown;

            bool retVal = true;

            if (monitorInfo.FocusedElement == null)
            {
                return false;
            }

            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);

            if (windowElement == null)
            {
                return false;
            }

            var element = (AgentUtils.FindElementByAutomationId(windowElement, "REComboBox20W", ControlType.ComboBox, "4481", "Status:") ??
                           AgentUtils.FindElementByAutomationId(windowElement, "REComboBox20W", ControlType.ComboBox, "4480", "Priority:")) ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4112", "% Complete:");

            retVal = (element != null);

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Edit", "4097", "Subject:"))
                {
                    subType = OutlookControlSubType.TaskSubjectField;
                }
                else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "_WwG", "ControlType.Pane", "", "Message"))
                {
                    subType = OutlookControlSubType.TaskMessageBodyField;
                }
            }

            Log.Debug("Returning " + retVal);
            return retVal;
        }

        /// <summary>
        /// Is the active window the Task window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsTasksWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return ((monitorInfo.Title.StartsWith("Tasks - ") || monitorInfo.Title.StartsWith("To-Do List")) && monitorInfo.Title.EndsWith("Microsoft Outlook"));
        }
    }
}