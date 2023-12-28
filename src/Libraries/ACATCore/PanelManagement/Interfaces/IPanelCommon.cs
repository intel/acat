////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.WidgetManagement;
using System;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Returns common properties of all panels -
    /// scanners, menus and dialogs
    /// </summary>
    public interface IPanelCommon
    {
        /// <summary>
        /// Gets the Animation Manager object
        /// </summary>
        AnimationManager AnimationManager { get; }

        /// <summary>
        /// Gets the Panel config ID
        /// </summary>
        Guid ConfigId { get; }

        /// <summary>
        /// Gets the display mode of the panel
        /// </summary>
        DisplayModeTypes DisplayMode { get; }

        /// <summary>
        /// Gets the object that manages the size and position
        /// of the panel
        /// </summary>
        ScannerPositionSizeController PositionSizeController { get; }

        ScannerPositionSizeController2 PositionSizeController2 { get; }

        /// <summary>
        /// Gets the widget that reprensents the form
        /// </summary>
        Widget RootWidget { get; }

        /// <summary>
        /// Gets the WidgetManager objet
        /// </summary>
        WidgetManager WidgetManager { get; }

        /// <summary>
        /// Check to see if a command should be
        /// enabled or not. This depends on the context.   The arg parameter
        /// contains the widget/command object in question.  For instance, if the
        /// talk window is empty, the "Clear talk window" button should be disabled.
        /// </summary>
        /// <param name="arg">Argument</param>
        void CheckCommandEnabled(CommandEnabledArg arg);
    }
}