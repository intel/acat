////////////////////////////////////////////////////////////////////////////
// <copyright file="OutlookInspector2013.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Automation;
using ACAT.ACATResources;

namespace ACAT.Lib.Extension.AppAgents.Outlook
{
    /// <summary>
    /// Interface to support 2013 version of Outlook.  Has methods
    /// to inspect the active window/ control elements and notify the
    /// agent which Outlook window/control has focus
    /// </summary>
    internal class OutlookInspector2013 : IOutlookInspector
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

            Log.Debug(windowType.ToString());
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

            var element = AgentUtils.FindElementByAutomationId(windowElement, "Edit", "ControlType.Edit", "2002", R.GetString2("Outlook2013AddrBookFirstNameFieldLabel")) ??
                            AgentUtils.FindElementByAutomationId(windowElement, "Edit", "ControlType.Edit", "2006", R.GetString2("Outlook2013AddrBookLastNameFieldLabel")) ??
                            AgentUtils.FindElementByAutomationId(windowElement, "Button", "ControlType.Button", "108", R.GetString2("Outlook2013AddrBookAddToContactsButtonText"));

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

            var retVal = (windowElement != null) && (monitorInfo.Title.ToLower().StartsWith(R.GetString2("Outlook2013TitleAddressBook").ToLower()) &&
                                                      AgentUtils.IsElementByAutomationId(windowElement, "OutexABCLS",
                                                          "ControlType.Window", String.Empty));

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Edit", "101", R.GetString2("Outlook2013AddrBookSearchFieldLabel")))
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
            var element = (AgentUtils.FindElementByAutomationId(windowElement, "OutlookGrid", ControlType.List, "4542") ??
                           AgentUtils.FindElementByAutomationId(windowElement, "Button", ControlType.Button, "4370")) ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4098");

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
            return (monitorInfo.Title.ToLower().StartsWith(R.GetString2("Outlook2013TitleCalendarPrefix").ToLower()) &&
                        monitorInfo.Title.ToLower().EndsWith(R.GetString2("Outlook2013TitleSuffix").ToLower()));
        }

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsContactsWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return (monitorInfo.Title.ToLower().StartsWith(R.GetString2("Outlook2013TitleContactsPrefix").ToLower()) && 
                        monitorInfo.Title.ToLower().EndsWith(R.GetString2("Outlook2013TitleSuffix").ToLower()));
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

            var title = monitorInfo.Title.ToLower();
            if (!title.Contains(R.GetString2("Outlook2013EmailTitleSuffixHTML").ToLower()) &&
                !title.Contains(R.GetString2("Outlook2013EmailTitleSuffixPlainText").ToLower()) &&
                !title.Contains(R.GetString2("Outlook2013EmailTitleSuffixRichText").ToLower()))
            {
                Log.Debug("Title: " + monitorInfo.Title);
                Log.Debug("TITLE DOES NOT MATCH.  RETURNING ");
                return false;
            }

            var windowElement = AutomationElement.FromHandle(monitorInfo.FgHwnd);
            var element = AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Document, "4099");
            var toButton = AgentUtils.FindElementByAutomationId(windowElement, "Button", ControlType.Button, "4352");

            if (element != null)
            {
                var patterns = element.GetSupportedPatterns();
                foreach (var pattern in patterns)
                {
                    if (String.Compare(pattern.ProgrammaticName, "ValuePatternIdentifiers.Pattern", true) == 0)
                    {
                        bool obj = (bool)element.GetCurrentPropertyValue(ValuePatternIdentifiers.IsReadOnlyProperty);

                        isNewMessage = !obj || (toButton != null);

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
                            else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "_WwG", "ControlType.Document", ""))
                            {
                                subType = OutlookControlSubType.NewEmailMessageBodyField;
                            }

                            Log.Debug(subType.ToString());
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
            return (monitorInfo.Title.ToLower().StartsWith(R.GetString2("Outlook2013TitleNotesPrefix").ToLower()) && 
                monitorInfo.Title.ToLower().EndsWith(R.GetString2("Outlook2013TitleSuffix").ToLower()));
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
            var element = AgentUtils.FindElementByAutomationId(windowElement, "REComboBox20W", ControlType.ComboBox, "4100") ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Document, "4100");

            retVal = (element != null);

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Document", "4100"))
                {
                    subType = OutlookControlSubType.AppointmentSubjectField;
                }
                else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "_WwG", "ControlType.Document", ""))
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
            var element = (AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4481", R.GetString2("Outlook2013OpenContactCompanyFieldLabel")) ??
                           AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4480", R.GetString2("Outlook2013OpenContactJobTitleFieldLabel"))) ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4096", R.GetString2("Outlook2013OpenContactFullNameFieldLabel"));

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

            var element = AgentUtils.GetElementOrAncestorByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Document", "4159");
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

            var element = (AgentUtils.FindElementByAutomationId(windowElement, "REComboBox20W", ControlType.ComboBox, "4481", R.GetString2("Outlook2013OpenTaskStatusFieldLabel")) ??
                           AgentUtils.FindElementByAutomationId(windowElement, "REComboBox20W", ControlType.ComboBox, "4480", R.GetString2("Outlook2013OpenTaskPriorityFieldLabel"))) ??
                          AgentUtils.FindElementByAutomationId(windowElement, "RichEdit20WPT", ControlType.Edit, "4112", R.GetString2("Outlook2013OpenTaskPercentCompleteFieldLabel"));

            retVal = (element != null);

            if (retVal)
            {
                if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "RichEdit20WPT", "ControlType.Document", "4097"))
                {
                    subType = OutlookControlSubType.TaskSubjectField;
                }
                else if (AgentUtils.IsElementByAutomationId(monitorInfo.FocusedElement, "_WwG", "ControlType.Document", ""))
                {
                    subType = OutlookControlSubType.TaskMessageBodyField;
                }

                Log.Debug(subType.ToString());
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
            var title = monitorInfo.Title.ToLower();
            return (title.StartsWith(R.GetString2("Outlook2010TitleTasksPrefix").ToLower()) || 
                    title.StartsWith(R.GetString2("Outlook2013TitleTodoPrefix").ToLower())) && 
                        title.EndsWith(R.GetString2("Outlook2013TitleSuffix").ToLower());
        }
    }
}