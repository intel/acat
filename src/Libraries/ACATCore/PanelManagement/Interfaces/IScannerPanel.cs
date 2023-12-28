////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// All scanners must implement the IScannerPanel interface.  Examples
    /// of scanners are the Alphabet scanner, the Cursor navigation scanner etc.
    /// (Dialogs and Menus are not scanners)
    /// </summary>
    public interface IScannerPanel : IPanel
    {
        RunCommandDispatcher CommandDispatcher { get; }

        /// <summary>
        /// Gets the scanner Window form
        /// </summary>
        Form Form { get; }

        /// <summary>
        /// Return the panel type
        /// </summary>
        /// <returns></returns>
        String PanelClass { get; }

        /// <summary>
        /// Gets the ScannerCommon object.  Every scanner
        /// must create an instance of this object
        /// </summary>
        ScannerCommon ScannerCommon { get; }

        /// <summary>
        /// Gets the text controller object. Every scanner
        /// must create an instance of this object
        /// </summary>
        ITextController TextController { get; }

        /// <summary>
        /// Invoked to check if a widget on a scanner needs to
        /// be enabled or not.  This depends on the context.
        /// </summary>
        /// <param name="arg">Contextual information</param>
        /// <returns>true on success</returns>
        bool CheckCommandEnabled(CommandEnabledArg arg);

        /// <summary>
        /// Invoked when the focus changes in the current
        /// foreground application
        /// </summary>
        /// <param name="monitorInfo"></param>
        void OnFocusChanged(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// Invoked when a request for a new scanner panel is made.  Return
        /// true to de-activate this panel and activate the new panel.
        /// Return false to dissallow changing of panels.
        /// </summary>
        /// <param name="eventArg"></param>
        /// <returns></returns>
        bool OnQueryPanelChange(PanelRequestEventArgs eventArg);

        /// <summary>
        /// A widget was actuated by the user either clicking on it
        /// or by using one of the input switches
        /// </summary>
        /// <param name="widget">the source widget</param>
        /// <param name="handled">set to true if handled</param>
        void OnWidgetActuated(WidgetActuatedEventArgs widgetActuatedEvent, ref bool handled);

        /// <summary>
        /// Inovked when the scanner is being used as an input
        /// panel to a control in a dialog box.  E.g, the
        /// alphabet scanner may be used to enter text in an edit
        /// control
        /// </summary>
        /// <param name="parent">parent form</param>
        /// <param name="widget">widget</param>
        void SetTargetControl(Form parent, Widget widget);
    }
}