////////////////////////////////////////////////////////////////////////////
// <copyright file="DefaultOutlookInspector.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;
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

namespace ACAT.Lib.Extension.AppAgents.Outlook
{
    /// <summary>
    /// A 'null' class for unsupported versions of Outlook
    /// </summary>
    internal class DefaultOutlookInspector : IOutlookInspector
    {
        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public OutlookWindowTypes IdentifyWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return OutlookWindowTypes.Unknown;
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
            return false;
        }

        /// <summary>
        /// Is the active window the Outlook Address book window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsAddressBookWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return false;
        }

        /// <summary>
        /// Is the active window the Appointement scheduling window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsAppointmentSchedulingWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return false;
        }

        /// <summary>
        /// Is the active window the Outlook calendar window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsCalendarWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return false;
        }

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsContactsWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return false;
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
            return false;
        }

        /// <summary>
        /// Is the active window the email inbox window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsInboxWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return false;
        }

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        public bool IsNotesWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return false;
        }

        /// <summary>
        /// Is the active window an open Appointment window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenAppointmentWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return false;
        }

        /// <summary>
        /// Is the active window an open "Contact" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenContactWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return false;
        }

        /// <summary>
        /// Is the active window an open Outlook "Note" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenNoteWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return false;
        }

        /// <summary>
        /// Is the active window an open "Task" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsOpenTaskWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType)
        {
            return false;
        }

        /// <summary>
        /// Is the active window the Task window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        public bool IsTasksWindow(WindowActivityMonitorInfo monitorInfo)
        {
            return false;
        }
    }
}