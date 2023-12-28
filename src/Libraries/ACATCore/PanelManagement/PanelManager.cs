////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// How is a panel being displayed?
    /// </summary>
    public enum DisplayModeTypes
    {
        None,

        /// <summary>
        /// As a normal window
        /// </summary>
        Normal,

        /// <summary>
        /// As a modal dialog
        /// </summary>
        Dialog,

        /// <summary>
        /// As a popup window.  Similar to Dialog except
        /// the parent scanner may alter its behavior in
        /// the OnPause handler
        /// </summary>
        Popup
    }

    /// <summary>
    /// Manages display of scanners.  On startup, walks the
    /// extension directories and loads all the scanners, dialogs
    /// and contextual menus and maintains them in a cache.
    /// ACAT follows the stack model to display scanners, similar to
    /// how Android handles activities. When a scanner is displayed,
    /// the parent scanner is 'paused' (hidden). When the scanner is
    /// closed, the parent scanner is 'resumed' (displayed).
    /// </summary>
    public class PanelManager : IDisposable
    {
        /// <summary>
        /// The root directory under ACAT from where the scanners/dialog/menus
        /// are loaded
        /// </summary>
        public static String UiRootDir = "UI";

        /// <summary>
        /// Singleton instance of PanelManager
        /// </summary>
        private static PanelManager _instance;

        /// <summary>
        /// Represents the stack of panels
        /// </summary>
        private readonly Stack<PanelStack> _stack = new Stack<PanelStack>();

        /// <summary>
        /// Is calibration in progress?
        /// </summary>
        private bool _actuatorCalibrationInProgress;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes an instance of the PanelManager
        /// </summary>
        public PanelManager()
        {
            Context.AppAgentMgr.EvtPanelRequest += AppAgent_EvtPanelRequest;
            Context.AppAgentMgr.EvtFocusChanged += AppAgent_EvtFocusChanged;
            Context.EvtCultureChanged += Context_EvtCultureChanged;
            ScannerCommon.EvtScannerShow += ScannerCommon_EvtScannerShow;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

            getTopOfStack();
        }

        public delegate void AlphabetScannerWidthChanged(int width);

        /// <summary>
        /// Deleagate for notification of start of calibration by an actuator
        /// </summary>
        /// <param name="args">Calibration notifaction object</param>
        public delegate void CalibrationStartNotify(ActuatorManagement.CalibrationNotifyEventArgs args);

        public event AlphabetScannerWidthChanged EvtAlphabetScannerWidthChanged;

        /// <summary>
        /// Inovked when the application quits
        /// </summary>
        public event EventHandler EvtAppQuit;

        /// <summary>
        /// Event raised to indicate end of calibration
        /// </summary>
        public event EventHandler EvtCalibrationEndNotify;

        /// <summary>
        /// Event raised to indicate start of calibration
        /// </summary>
        public event CalibrationStartNotify EvtCalibrationStartNotify;

        /// <summary>
        /// Event raised when the desktop size or the resolution changes
        /// </summary>
        public event EventHandler EvtDisplaySettingsChanged;

        /// <summary>
        /// Event raised just before panel is displayed
        /// </summary>
        public event PanelPreShow EvtPanelPreShow;

        /// <summary>
        /// Raised when a scanner is closed
        /// </summary>
        public event ScannerClose EvtScannerClosed;

        /// <summary>
        /// Raised when a scanner is shown
        /// </summary>
        public event ScannerShow EvtScannerShow;

        /// <summary>
        /// Raised on startup when the PanelManager is enumerating
        /// forms that reside in the extension dirs.  The event subscriber
        /// should add scanner types that are not located in the extension
        /// directories.
        /// </summary>
        public event EventHandler EvtStartupAddForms;

        public event EventHandler EvtStartupAddUserControls;

        /// <summary>
        /// Returns the singleton instance of the PanelManager
        /// </summary>
        public static PanelManager Instance
        {
            get { return _instance ?? (_instance = new PanelManager()); }
        }

        /// <summary>
        /// Returns the currently visible Form object
        /// </summary>
        public Form CurrentForm
        {
            get
            {
                return (_stack.Count > 0) ? _stack.Peek().CurrentForm : null;
            }
        }

        /// <summary>
        /// Gets the display mode of the panel that is currently displayed
        /// </summary>
        public DisplayModeTypes PanelDisplayMode
        {
            get { return getTopOfStack().PanelDisplayMode; }
        }

        /// <summary>
        /// Gets the panel that is about to be shown.  Call
        /// this from the OnPause handler to see which panel is going
        /// to be displayed. Has non-null value JUST before the panel
        /// is shown, null all other times
        /// </summary>
        public IPanel PreShowPanel
        {
            get { return getTopOfStack().PreShowPanel; }
        }

        /// <summary>
        /// Gets the display mode of the panel that is about to
        /// be shown
        /// </summary>
        public DisplayModeTypes PreShowPanelDisplayMode
        {
            get { return getTopOfStack().PreShowPanelDisplayMode; }
        }

        /// <summary>
        /// Add the form of the specified type to the form cache.
        /// </summary>
        /// <param name="type">the .NET type</param>
        public void AddFormToCache(Type type)
        {
            var guid = PanelConfigMap.GetFormId(type);
            PanelConfigMap.AddFormToCache(guid, type);
        }

        /// <summary>
        /// Clears all the entries in the stack, closes all
        /// the panels in each stack and then creates an empty stack
        /// </summary>
        public void ClearStack()
        {
            while (_stack.Count > 0)
            {
                var panelStack = _stack.Pop();

                panelStack.CloseCurrentPanel();

                panelStack.EvtScannerClosed -= panelStack_EvtScannerClosed;
            }

            if (_stack.Count == 0)
            {
                _stack.Push(createPanelStack());
            }
        }

        /// <summary>
        /// Closes the current form that is active.  The current
        /// Form is the one that is topmost on the stack, the one
        /// that is currently active and visible.
        /// </summary>
        public void CloseCurrentForm()
        {
            if (_stack.Count > 0)
            {
                _stack.Peek().CloseCurrentForm();
            }
        }

        /// <summary>
        /// Closes the current panel.  The current panel
        /// need NOT be the one that is currently active
        /// and visible (that is the 'current form').
        /// The current panel is the ancestor of 'current form'.
        /// </summary>
        public void CloseCurrentPanel()
        {
            Log.Debug();

            if (_stack.Count > 0)
            {
                _stack.Peek().CloseCurrentPanel();
            }
        }

        /// <summary>
        /// Closes the topmost stack entry
        /// </summary>
        public void CloseStack()
        {
            if (_stack.Count > 0)
            {
                var panelStack = _stack.Pop();

                panelStack.CloseCurrentPanel();

                panelStack.EvtScannerClosed -= panelStack_EvtScannerClosed;

                if (_stack.Count > 0 && !_actuatorCalibrationInProgress)
                {
                    panelStack = getTopOfStack();

                    panelStack.Resume();
                }
            }

            if (_stack.Count == 0)
            {
                _stack.Push(createPanelStack());
            }
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(String panelClass)
        {
            return getTopOfStack().CreatePanel(panelClass);
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="title">title of the panel</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(String panelClass, String title)
        {
            return getTopOfStack().CreatePanel(panelClass, title);
        }

        /// <summary>
        /// Creates a panel with the specified panel title and startup args
        /// </summary>
        /// <param name="panelTitle">Title for the panel</param>
        /// <param name="startupArg">startup arguments for the panel</param>
        /// <returns></returns>
        public Form CreatePanel(String panelTitle, StartupArg startupArg)
        {
            return getTopOfStack().CreatePanel(startupArg.PanelClass, panelTitle, startupArg);
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="panelTitle">panel title</param>
        /// <param name="startupArg">statrtup arg for the panel</param>
        /// <returns>the form for the panel</returns>

        public Form CreatePanel(String panelClass, String panelTitle, StartupArg startupArg)
        {
            return getTopOfStack().CreatePanel(panelClass, panelTitle, startupArg);
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="panelTitle">panel title</param>
        /// <param name="winHandle">target window handle</param>
        /// <param name="focusedElement">currently focused element</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(
            ref String panelClass,
            String panelTitle,
            IntPtr winHandle,
            AutomationElement focusedElement)
        {
            return getTopOfStack().CreatePanel(ref panelClass, panelTitle, winHandle, focusedElement);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns the currently visible panel Form
        /// </summary>
        /// <returns>form</returns>
        public IPanel GetCurrentForm()
        {
            return (_stack.Count > 0) ? _stack.Peek().GetCurrentForm() : null;
        }

        /// <summary>
        /// Returns the current panel.
        /// The current panel is the the last scanner that was
        /// not shown as a dialog.  Note that the Current Panel scanner may
        /// not be visible as it may have created child scanners as dialogs.
        /// </summary>
        /// <returns>The active panel</returns>
        public IPanel GetCurrentPanel()
        {
            return (_stack.Count > 0) ? _stack.Peek().GetCurrentPanel() : null;
        }

        /// <summary>
        /// Return the panel name of the currently
        /// active panel
        /// </summary>
        /// <returns>the name</returns>
        public String GetCurrentPanelName()
        {
            return (_stack.Count > 0) ? _stack.Peek().GetCurrentPanelName() : String.Empty;
        }

        /// <summary>
        /// Performs initialization. Walks the extension
        /// dirs and caches the Types of all the scanner/dialogs and menus.
        /// The Type will be used to create an instance using .NET
        /// relection.
        /// </summary>
        /// <param name="extensionDirs">extension dirs to walk</param>
        /// <returns>true on success</returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            PanelConfigMap.Reset();

            var retVal = PanelConfigMap.Load(extensionDirs);
            if(!retVal)
                return false;

            retVal = UserControlConfigMap.Load(extensionDirs);
            if (!retVal)
                return false;

            PanelConfigMap.Load(Preferences.ApplicationAssembly);

            if (EvtStartupAddForms != null)
            {
                EvtStartupAddForms(this, new EventArgs());
            }

            EvtStartupAddUserControls?.Invoke(this, new EventArgs());

            PanelConfigMap.CleanupOrphans();

            UserControlConfigMap.CleanupOrphans();

            if (!String.IsNullOrEmpty(CoreGlobals.AppPreferences.PreferredPanelConfigNames))
            {
                PanelConfigMap.SetDefaultPanelConfig(CoreGlobals.AppPreferences.PreferredPanelConfigNames.Trim());
            }

            Context.AppActuatorManager.EvtCalibrationStartNotify += AppActuatorManager_EvtCalibrationStartNotify;
            Context.AppActuatorManager.EvtCalibrationEndNotify += AppActuatorManager_EvtCalibrationEndNotify;
            return retVal;
        }

        /// <summary>
        /// Returns true if the current panel class is the one
        /// specified =
        /// </summary>
        /// <param name="panelClass">panelclass to check for</param>
        /// <returns>true if it is</returns>
        public bool IsCurrentPanelClass(String panelClass)
        {
            return String.Compare(panelClass, Context.AppPanelManager.GetCurrentPanelName(), true) == 0;
        }

        /// <summary>
        /// Pauses current stack and creates and pushes
        /// a new panelStack entry
        /// </summary>
        public void NewStack()
        {
            if (_stack.Count > 0)
            {
                var panelStack = _stack.Peek();
                panelStack.Pause();
            }

            _stack.Push(createPanelStack());
        }

        /// <summary>
        /// Pause panel change requests.  This means any requests
        /// in the future to change panels from the
        /// Agent Manager will not be honored. Call this to
        /// keep the current scanner locked.
        /// </summary>
        public void PausePanelChangeRequests()
        {
            Context.AppAgentMgr.PausePanelChangeRequests();
        }

        /// <summary>
        /// Resumes previously paused panel change requests. This means
        /// any requests in the future to change panels from the
        /// Agent Manager will be honored.
        /// </summary>
        public void ResumePanelChangeRequests()
        {
            Context.AppAgentMgr.ResumePanelChangeRequests();
        }

        /// <summary>
        /// Displays the panel. Parent is the panel
        /// making the call. Also Pauses the parent
        /// It will be Resumed when the 'panel' is closed.
        /// </summary>
        /// <param name="parent">The parent panel</param>
        /// <param name="panel">the panel to show</param>
        /// <returns>true on success</returns>
        public bool Show(IPanel parent, IPanel panel)
        {
            return getTopOfStack().Show(parent, panel);
        }

        /// <summary>
        /// Displays the panel
        /// </summary>
        /// <param name="form">panel to display</param>
        /// <returns></returns>
        public bool Show(IPanel form)
        {
            return getTopOfStack().Show(form);
        }

        /// <summary>
        /// Shows the specified panel as a dialog. If there
        /// is a scanner currently active, it uses that scanner
        /// as the parent of the dialog
        /// </summary>
        /// <param name="panel">panel to show</param>
        /// <returns>true on success</returns>
        public bool ShowDialog(IPanel panel)
        {
            return getTopOfStack().ShowDialog(panel);
        }

        /// <summary>
        /// Show panel as a popup with the parent as the
        /// parent form
        /// </summary>
        /// <param name="parent">the parent form</param>
        /// <param name="panel">panel to show as dialog</param>
        /// <returns>true on success</returns>
        public bool ShowDialog(IPanel parent, IPanel panel)
        {
            return getTopOfStack().ShowDialog(parent, panel);
        }

        /// <summary>
        /// Displays the panel as a popup
        /// </summary>
        /// <param name="form">panel to display</param>
        /// <returns>true on success</returns>
        public bool ShowPopup(IPanel panel)
        {
            return getTopOfStack().ShowPopup(panel);
        }

        /// <summary>
        /// Displays the panel as a popup. Parent is the panel
        /// making the call. Also Pauses the parent
        /// It will be Resumed when the 'panel' is closed.
        /// </summary>
        /// <param name="parent">The parent panel</param>
        /// <param name="panel">the panel to show</param>
        /// <returns>true on success</returns>
        public bool ShowPopup(IPanel parent, IPanel panel)
        {
            return getTopOfStack().ShowPopup(parent, panel);
        }

        /// <summary>
        /// Raises the event before panel is displayed
        /// </summary>
        /// <param name="arg"></param>
        internal void NotifyPanelPreShow(PanelPreShowEventArg arg)
        {
            if (EvtPanelPreShow != null)
            {
                EvtPanelPreShow(this, arg);
            }
        }

        /// <summary>
        /// Notify subscribers that application is quiting
        /// </summary>
        internal void NotifyQuitApplication()
        {
            if (EvtAppQuit != null)
            {
                EvtAppQuit(_instance, new EventArgs());
            }
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                Context.EvtCultureChanged -= Context_EvtCultureChanged;

                Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;

                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        private void AppActuatorManager_EvtCalibrationEndNotify(object sender, EventArgs e)
        {
            _actuatorCalibrationInProgress = false;

            Log.Debug("Resuming WindowActivityMonitor");

            WindowActivityMonitor.Resume();

            var panelStack = _stack.Peek();

            if (panelStack.IsPaused)
            {
                panelStack.Resume();
                EvtCalibrationEndNotify?.Invoke(this, e);
            }

            // this is only for ACAT App
            //EnumWindows.RestoreFocusToTopWindowOnDesktop();

            //WindowActivityMonitor.GetActiveWindowAsync();
        }

        private void AppActuatorManager_EvtCalibrationStartNotify(ActuatorManagement.CalibrationNotifyEventArgs args)
        {
            _actuatorCalibrationInProgress = true;

            Log.Debug("Pausing WindowActivityMonitor");

            WindowActivityMonitor.Pause();

            var panelStack = _stack.Peek();

            if (!panelStack.IsPaused)
            {
                panelStack.Pause();
                EvtCalibrationStartNotify?.Invoke(args);
            }
        }

        /// <summary>
        /// Foreground window focus changed.  Let the active
        /// scanner know about this
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppAgent_EvtFocusChanged(object sender, FocusChangedEventArgs e)
        {
            var panel = getTopOfStack().GetCurrentPanel();
            if (panel is IScannerPanel)
            {
                ((IScannerPanel)panel).OnFocusChanged(e.WindowActivityInfo);
            }
        }

        /// <summary>
        /// Event handler for request to display a scanner. The
        /// arg parameter contains information about which scanner
        /// to display
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        private void AppAgent_EvtPanelRequest(object sender, PanelRequestEventArgs arg)
        {
            getTopOfStack().AppAgent_EvtPanelRequest(sender, arg);
        }

        /// <summary>
        /// Event handler for when the default culture chnages
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        private void Context_EvtCultureChanged(object sender, CultureChangedEventArg arg)
        {
            getTopOfStack().CurrentForm.Invoke(new MethodInvoker(delegate
            {
                ClearStack();
                PanelConfigMap.Reset();
                Init(Context.ExtensionDirs);
            }));
        }

        /// <summary>
        /// Creates a new panelstack object
        /// </summary>
        /// <returns>created object</returns>
        private PanelStack createPanelStack()
        {
            var panelStack = new PanelStack();
            panelStack.EvtScannerClosed += panelStack_EvtScannerClosed;
            return panelStack;
        }

        /// <summary>
        /// Returns the top of stack in the stack of panels
        /// </summary>
        /// <returns>PanelStack object</returns>
        private PanelStack getTopOfStack()
        {
            PanelStack panelStack;

            if (_stack.Count == 0)
            {
                panelStack = createPanelStack();

                _stack.Push(panelStack);
            }
            else
            {
                panelStack = _stack.Peek();
            }

            return panelStack;
        }

        /// <summary>
        /// Event handler for when a scanner closes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void panelStack_EvtScannerClosed(object sender, ScannerCloseEventArg arg)
        {
            if (EvtScannerClosed != null)
            {
                EvtScannerClosed(sender, arg);
            }
        }

        /// <summary>
        /// Handler for the event that is raised to indicate
        /// that a scanner was just shown. Notify subscribers
        /// about this
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void ScannerCommon_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (arg.Scanner.PanelClass == "Alphabet")
            {
                Windows.WidestScannerWidth = arg.Scanner.Form.Width;
                if (EvtAlphabetScannerWidthChanged != null)
                {
                    EvtAlphabetScannerWidthChanged(arg.Scanner.Form.Width);
                }
            }

            if (EvtScannerShow != null)
            {
                EvtScannerShow(sender, arg);
            }
        }

        /// <summary>
        /// Display resolution changed.  Raise the event
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            Log.Debug("Display Resolution changed. Working area is " + Screen.PrimaryScreen.WorkingArea);

            if (EvtDisplaySettingsChanged != null)
            {
                EvtDisplaySettingsChanged(sender, e);
            }
        }
    }
}