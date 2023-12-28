////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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