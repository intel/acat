////////////////////////////////////////////////////////////////////////////
// <copyright file="IInternetExplorerElements.cs" company="Intel Corporation">
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
using System.Windows.Automation;

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
    /// Base class to determines which control in the IE Browser currently has focus
    /// Derive a class from this for each specific version of the browser
    /// </summary>
    public interface IInternetExplorerElements
    {
        /// <summary>
        /// Email a link to the current page in the browser
        /// </summary>
        void EmailPageAsLink();

        /// <summary>
        /// Is the focused element the Address textbox?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        bool IsAddressWindow(AutomationElement focusedElement);

        /// <summary>
        /// Is the focused element the Favorites window?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        bool IsFavoritesWindow(AutomationElement focusedElement);

        /// <summary>
        /// Is the focused element the Feeds window?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        bool IsFeedsWindow(AutomationElement focusedElement);

        /// <summary>
        /// Is the focused element the Find textbox?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        bool IsFindControl(AutomationElement focusedElement);

        /// <summary>
        /// Is the focused element the History window?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        bool IsHistoryWindow(AutomationElement focusedElement);

        /// <summary>
        /// Is the focused element the search textbox?
        /// </summary>
        /// <param name="focusedElement">The control in focus</param>
        /// <returns>true if it is</returns>
        bool IsSearchControl(AutomationElement focusedElement);
    }
}