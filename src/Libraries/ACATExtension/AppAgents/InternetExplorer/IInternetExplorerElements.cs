////////////////////////////////////////////////////////////////////////////
// <copyright file="IInternetExplorerElements.cs" company="Intel Corporation">
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

using System.Windows.Automation;

namespace ACAT.Lib.Extension.AppAgents.InternetExplorer
{
    /// <summary>
    /// Interface to determine which control in the IE Browser currently has focus -
    /// address bar?  search box?  favorites window?
    /// Implement this interface for each specific version of IE
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