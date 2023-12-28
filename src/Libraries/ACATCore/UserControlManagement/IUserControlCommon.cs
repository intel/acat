////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.WidgetManagement;

namespace ACAT.Lib.Core.UserControlManagement
{
    /// <summary>
    /// Returns common properties of UserControls
    /// </summary>
    public interface IUserControlCommon
    {
        /// <summary>
        /// Gets the Animation Manager object
        /// </summary>
        AnimationManager2 AnimationManager { get; }

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

        void Close();

        void Dispose();
    }
}