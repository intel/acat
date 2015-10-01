////////////////////////////////////////////////////////////////////////////
// <copyright file="IE8Elements.cs" company="Intel Corporation">
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
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;

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

namespace ACAT.Lib.Extension.AppAgents.InternetExplorer
{
    /// <summary>
    /// Determines which control in the IE Browser currently has focus
    /// E.g.  Is it the address bar?  Is it the Find window?
    /// This is applicable only to IE8
    /// </summary>
    public class IE8Elements : IInternetExplorerElements
    {
        /// <summary>
        /// Email a link to the current page in the browser
        /// </summary>
        public void EmailPageAsLink()
        {
            AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.P);
            Thread.Sleep(1000);
            AgentManager.Instance.Keyboard.Send(Keys.L);
        }

        /// <summary>
        /// Is the focused element the Address textbox?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        public bool IsAddressWindow(AutomationElement focusedElement)
        {
            return focusedElement != null &&
                    String.Compare(focusedElement.Current.ClassName, "Edit", true) == 0 &&
                    String.Compare(focusedElement.Current.ControlType.ProgrammaticName, "ControlType.Edit", true) == 0 &&
                    String.Compare(focusedElement.Current.Name, "Address", true) == 0;
        }

        /// <summary>
        /// Is the focused element the Favorites window?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        public bool IsFavoritesWindow(AutomationElement focusedElement)
        {
            return focusedElement != null &&
                AgentUtils.IsAncestorByName(focusedElement, "SysTreeView32", "ControlType.Tree", "Favorites");
        }

        /// <summary>
        /// Is the focused element the Feeds window?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        public bool IsFeedsWindow(AutomationElement focusedElement)
        {
            return focusedElement != null &&
                AgentUtils.IsAncestorByName(focusedElement, "SysTreeView32", "ControlType.Tree", "Feeds");
        }

        /// <summary>
        /// Is the focused element the Find textbox?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        public bool IsFindControl(AutomationElement focusedElement)
        {
            return focusedElement != null &&
                String.Compare(focusedElement.Current.ClassName, "Edit", true) == 0 &&
                String.Compare(focusedElement.Current.ControlType.ProgrammaticName, "ControlType.Edit", true) == 0 &&
                AgentUtils.IsAncestorByName(focusedElement, "FindBarClass", "ControlType.Pane", "");
        }

        /// <summary>
        /// Is the focused element the History window?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        public bool IsHistoryWindow(AutomationElement focusedElement)
        {
            return focusedElement != null &&
                AgentUtils.IsAncestorByName(focusedElement, "SysTreeView32", "ControlType.Tree", "History");
        }

        /// <summary>
        /// Is the focused element the search textbox?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        public bool IsSearchControl(AutomationElement focusedElement)
        {
            return focusedElement != null &&
                String.Compare(focusedElement.Current.ClassName, "Edit", true) == 0 &&
                String.Compare(focusedElement.Current.ControlType.ProgrammaticName, "ControlType.Edit", true) == 0 &&
                AgentUtils.IsAncestorByName(focusedElement, "Search Control", "ControlType.Pane", "");
        }
    }
}