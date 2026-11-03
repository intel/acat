////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.PanelManagement.PanelConfig;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Core.PanelManagement
{
    /// <summary>
    /// Interface for PanelManager to support dependency injection.
    /// Manages display of scanners.
    /// </summary>
    public interface IPanelManager : IDisposable
    {
        /// <summary>
        /// Event raised when the alphabet scanner width changes
        /// </summary>
        event PanelManager.AlphabetScannerWidthChanged EvtAlphabetScannerWidthChanged;

        /// <summary>
        /// Event raised when the application quits
        /// </summary>
        event EventHandler EvtAppQuit;

        /// <summary>
        /// Event raised to indicate end of calibration
        /// </summary>
        event EventHandler EvtCalibrationEndNotify;

        /// <summary>
        /// Event raised to indicate start of calibration
        /// </summary>
        event PanelManager.CalibrationStartNotify EvtCalibrationStartNotify;

        /// <summary>
        /// Event raised when the desktop size or the resolution changes
        /// </summary>
        event EventHandler EvtDisplaySettingsChanged;

        /// <summary>
        /// Event raised just before panel is displayed
        /// </summary>
        event PanelPreShow EvtPanelPreShow;

        /// <summary>
        /// Raised when a scanner is closed
        /// </summary>
        event ScannerClose EvtScannerClosed;

        /// <summary>
        /// Raised when a scanner is shown
        /// </summary>
        event ScannerShow EvtScannerShow;

        /// <summary>
        /// Raised on startup when the PanelManager is enumerating forms
        /// </summary>
        event EventHandler EvtStartupAddForms;

        /// <summary>
        /// Raised on startup when the PanelManager is enumerating user controls
        /// </summary>
        event EventHandler EvtStartupAddUserControls;

        /// <summary>
        /// Returns the currently visible Form object
        /// </summary>
        Form CurrentForm { get; }

        /// <summary>
        /// Gets the display mode of the panel that is currently displayed
        /// </summary>
        DisplayModeTypes PanelDisplayMode { get; }

        /// <summary>
        /// Gets the panel that is about to be shown
        /// </summary>
        IPanel PreShowPanel { get; }

        /// <summary>
        /// Gets the display mode of the panel that is about to be shown
        /// </summary>
        DisplayModeTypes PreShowPanelDisplayMode { get; }

        /// <summary>
        /// Add the form of the specified type to the form cache
        /// </summary>
        /// <param name="type">the .NET type</param>
        void AddFormToCache(Type type);

        /// <summary>
        /// Clears all the entries in the stack
        /// </summary>
        void ClearStack();

        /// <summary>
        /// Closes the current form that is active
        /// </summary>
        void CloseCurrentForm();

        /// <summary>
        /// Closes the current panel
        /// </summary>
        void CloseCurrentPanel();

        /// <summary>
        /// Closes the topmost stack entry
        /// </summary>
        void CloseStack();

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <returns>the form for the panel</returns>
        Form CreatePanel(String panelClass);

        /// <summary>
        /// Creates the panel from configuration
        /// </summary>
        /// <param name="panelConfig">panel configuration</param>
        /// <param name="title">title of the panel</param>
        /// <returns>the form for the panel</returns>
        Form CreatePanelFromConfig(PanelConfigMapEntry panelConfig, string title);

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="title">title of the panel</param>
        /// <returns>the form for the panel</returns>
        Form CreatePanel(String panelClass, String title);

        /// <summary>
        /// Creates a panel with the specified panel title and startup args
        /// </summary>
        /// <param name="panelTitle">Title for the panel</param>
        /// <param name="startupArg">startup arguments for the panel</param>
        /// <returns>the form for the panel</returns>
        Form CreatePanel(String panelTitle, StartupArg startupArg);

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="panelTitle">panel title</param>
        /// <param name="startupArg">startup arg for the panel</param>
        /// <returns>the form for the panel</returns>
        Form CreatePanel(String panelClass, String panelTitle, StartupArg startupArg);

        /// <summary>
        /// Returns the currently visible panel Form
        /// </summary>
        /// <returns>form</returns>
        IPanel GetCurrentForm();

        /// <summary>
        /// Returns the current panel
        /// </summary>
        /// <returns>The active panel</returns>
        IPanel GetCurrentPanel();

        /// <summary>
        /// Return the panel name of the currently active panel
        /// </summary>
        /// <returns>the name</returns>
        String GetCurrentPanelName();

        /// <summary>
        /// Performs initialization
        /// </summary>
        /// <param name="extensionDirs">extension dirs to walk</param>
        /// <returns>true on success</returns>
        bool Init(IEnumerable<string> extensionDirs);

        /// <summary>
        /// Returns true if the current panel class is the one specified
        /// </summary>
        /// <param name="panelClass">panelclass to check for</param>
        /// <returns>true if it is</returns>
        bool IsCurrentPanelClass(String panelClass);

        /// <summary>
        /// Pauses current stack and creates and pushes a new panelStack entry
        /// </summary>
        void NewStack();

        /// <summary>
        /// Pause panel change requests
        /// </summary>
        void PausePanelChangeRequests();

        /// <summary>
        /// Resumes previously paused panel change requests
        /// </summary>
        void ResumePanelChangeRequests();

        /// <summary>
        /// Displays the panel
        /// </summary>
        /// <param name="parent">The parent panel</param>
        /// <param name="panel">the panel to show</param>
        /// <returns>true on success</returns>
        bool Show(IPanel parent, IPanel panel);

        /// <summary>
        /// Displays the panel
        /// </summary>
        /// <param name="form">panel to display</param>
        /// <returns>true on success</returns>
        bool Show(IPanel form);

        /// <summary>
        /// Shows the specified panel as a dialog
        /// </summary>
        /// <param name="panel">panel to show</param>
        /// <returns>true on success</returns>
        bool ShowDialog(IPanel panel);

        /// <summary>
        /// Show panel as a dialog with the parent
        /// </summary>
        /// <param name="parent">the parent form</param>
        /// <param name="panel">panel to show as dialog</param>
        /// <returns>true on success</returns>
        bool ShowDialog(IPanel parent, IPanel panel);

        /// <summary>
        /// Displays the panel as a popup
        /// </summary>
        /// <param name="panel">panel to display</param>
        /// <returns>true on success</returns>
        bool ShowPopup(IPanel panel);

        /// <summary>
        /// Displays the panel as a popup
        /// </summary>
        /// <param name="parent">The parent panel</param>
        /// <param name="panel">the panel to show</param>
        /// <returns>true on success</returns>
        bool ShowPopup(IPanel parent, IPanel panel);
    }
}
