////////////////////////////////////////////////////////////////////////////
// <copyright file="IOutlookInspector.cs" company="Intel Corporation">
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
    /// Interface to support different versions of Outlook.  Has methods
    /// to inspect the active window/ control elements and notify the
    /// agent which Outlook window/control has focus.
    /// Implement this interface for each version of Outlook.
    /// </summary>
    public interface IOutlookInspector
    {
        /// <summary>
        /// Identify the top level window in Outlook
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>Type of the window</returns>
        OutlookWindowTypes IdentifyWindow(WindowActivityMonitorInfo monitorInfo,
            ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window the details window that is
        /// displayed when the user clicks on a name in the
        /// address book search results
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        bool IsAddressBookDetailsWindow(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Is the active window the Outlook Address book window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        bool IsAddressBookWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window the Appointement scheduling window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        bool IsAppointmentSchedulingWindow(WindowActivityMonitorInfo monitorInfo,
            ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window the Outlook calendar window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        bool IsCalendarWindow(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        bool IsContactsWindow(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Is the active window a new or existing open email  window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="isNewMessage">true if this is a new email window</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        bool IsEmailMsgWindow(WindowActivityMonitorInfo monitorInfo, ref bool isNewMessage,
            ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window the email inbox window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        bool IsInboxWindow(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Is the active window the "Contacts" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        bool IsNotesWindow(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Is the active window an open Appointment window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        bool IsOpenAppointmentWindow(WindowActivityMonitorInfo monitorInfo,
            ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window an open "Contact" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        bool IsOpenContactWindow(WindowActivityMonitorInfo monitorInfo,
            ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window an open Outlook "Note" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        bool IsOpenNoteWindow(WindowActivityMonitorInfo monitorInfo, ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window an open Task window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <param name="subType">The focused Outlook window control element </param>
        /// <returns>true if it is</returns>
        bool IsOpenTaskWindow(WindowActivityMonitorInfo monitorInfo,
            ref OutlookControlSubType subType);

        /// <summary>
        /// Is the active window the "Tasks" window?
        /// </summary>
        /// <param name="monitorInfo">Active window info</param>
        /// <returns>true if it is</returns>
        bool IsTasksWindow(WindowActivityMonitorInfo monitorInfo);
    }
}