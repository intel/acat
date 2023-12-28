////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.WidgetManagement;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// All ACAT Dialogs must implement this interface
    /// </summary>
    public interface IDialogPanel : IPanel
    {
        /// <summary>
        /// When a button on the dialog is actuated.  Any widget
        /// that derives from ButtonWidgetBase is considered a button.
        /// </summary>
        /// <param name="widget"></param>
        void OnButtonActuated(Widget widget);

        /// <summary>
        /// A command handler.  If the dialog doesn't recongnize
        /// the command, it sets 'handled' to false.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handled"></param>
        void OnRunCommand(string command, ref bool handled);
    }
}