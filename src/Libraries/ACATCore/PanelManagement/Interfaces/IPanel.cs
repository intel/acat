////////////////////////////////////////////////////////////////////////////
// <copyright file="IPanel.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.Utility;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// ALL panels in ACAT - Scanners, Dialogs, Menus
    /// and Contextual Menus must implement this.
    /// </summary>
    public interface IPanel
    {
        /// <summary>
        /// Gets the Descriptor for the panel. Descriptor
        /// contains name and ID of the panel
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Gets the PanelCommon interface that contains properties
        /// common to all panels
        /// </summary>
        IPanelCommon PanelCommon { get; }

        /// <summary>
        /// Used for synchronization
        /// </summary>
        SyncLock SyncObj { get; }

        /// <summary>
        /// Performs initialization.  This is invoked by
        /// the Panel manager after instantiating the panel, but
        /// before the panel is shown.
        /// </summary>
        /// <param name="initArg"></param>
        /// <returns></returns>
        bool Initialize(StartupArg initArg);

        /// <summary>
        /// Invoked when the panel is being deactivated because control
        /// is going to be shifted to another panel.  Typically in
        /// the OnPause handler, animation is paused the panel form is hidden
        /// </summary>
        void OnPause();

        /// <summary>
        /// Invoked when a paused panel is re-activated.  Typically in
        /// the OnResume handler, the panel form is displayed and animation
        /// is resumed.
        /// </summary>
        void OnResume();
    }
}