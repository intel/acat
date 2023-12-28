////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// This is a helper class for all scanners.  It contains functions
    /// that are common across all scanners and using this avoids duplication
    /// on code, consistency in how events are handled and makes it easier
    /// for the developer to add new scanners to ACAT
    /// The scanner form should contain a field that returns a ScannerCommon
    /// object and call functions in this class at the appropriate points in the
    /// form.  The documentation for this class contains info on where each
    /// of the functions need to be invoked.
    /// </summary>
    public class ScannerCommon2 : IDisposable, IPanelCommon
    {
        /// <summary>
        /// Status bar for the scanner form
        /// </summary>
        public readonly StatusBar StatusBarControl = new StatusBar();

        /// <summary>
        /// Used for trapping mouse_activate events
        /// </summary>
        private const int WM_MOUSEACTIVATE = 0x21;

        /// <summary>
        /// Interface to IScannerCommon. All scanner should implement this
        /// interface
        /// </summary>
        private readonly IScannerPanel _scannerPanel;

        /// <summary>
        /// used for synchronization
        /// </summary>
        private readonly SyncLock _syncLock;

        /// <summary>
        /// The animation manager object.  Manages all animations
        /// </summary>
        private AnimationManager _animationManager;

        /// <summary>
        /// Aspect ratio of the form at design time
        /// </summary>
        private float _designTimeAspectRatio = 0.0f;

        /// <summary>
        /// Has the scanner been shown as a modal dialog?
        /// </summary>
        private bool _dialogMode;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Remember if this is the first call to the OnClientSizeChanged handler
        /// </summary>
        private bool _firstCallToClientSizeChanged = true;

        /// <summary>
        /// is the scanner made invisible due to calibration of actuator?
        /// </summary>
        private bool _hideScannerOnCalibration;

        /// <summary>
        /// Tracks scanner idle time.  If the user hasn't activated
        /// the scanner in this time, the scanner makes itself invisible
        /// (behavior is configurable)
        /// </summary>
        private System.Timers.Timer _idleTimer;

        /// <summary>
        /// Is the scanner paused?
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// is scanner in preview mode?
        /// </summary>
        private bool _previewMode;

        /// <summary>
        /// The widget represnting the window
        /// </summary>
        private Widget _rootWidget;

        /// <summary>
        /// Is the scanner opacity != 1.0?
        /// </summary>
        private bool _scannerFaded;

        /// <summary>
        /// The status bar object for the scanner
        /// </summary>
        private ScannerStatusBar _scannerStatusBar;

        private UserControlManager _userControlManager;

        /// <summary>
        /// The widget manager object.  Maintains a list of all
        /// widgets in this window
        /// </summary>
        private WidgetManager _widgetManager;

        /// <summary>
        /// Watchdog object that makes sure that the scanner
        /// is not overlapped by another window
        /// </summary>
        private WindowOverlapWatchdog _windowOverlapWatchdog;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="iScannerPanel">The scanner Form object</param>
        public ScannerCommon2(IScannerPanel iScannerPanel)
        {
            ScannerForm = iScannerPanel.Form;

            StartupArg = null;
            _scannerPanel = iScannerPanel;
            ScannerForm.ShowInTaskbar = false;
            PositionSizeController2 = new ScannerPositionSizeController2(this);
            TextController = new TextController();
            HideScannerOnIdle = CoreGlobals.AppPreferences.HideScannerOnIdle;
            _syncLock = new SyncLock();

            _userControlManager = new UserControlManager(iScannerPanel, TextController);
        }

        /// <summary>
        /// Raised when the scanner is shown
        /// </summary>
        public static event ScannerShow EvtScannerShow;

        /// <summary>
        /// What to do the parent scanner in the OnPause() handler
        /// </summary>
        public enum PauseDisplayMode
        {
            /// <summary>
            /// Don't do anything. Keep the scanner active
            /// </summary>
            None,

            /// <summary>
            /// Perform the default behavior
            /// </summary>
            Default,

            /// <summary>
            /// Hide the parent scanner
            /// </summary>
            HideScanner,

            /// <summary>
            /// Fade the parent scanner
            /// </summary>
            FadeScanner,
        }

        /// <summary>
        /// Gets the widget that was actuated as a result of one
        /// of the actuator switches trigerring
        /// </summary>
        public Widget ActuatedWidget { get; private set; }

        /// <summary>
        /// Gets the Animation Manager object
        /// </summary>
        public AnimationManager AnimationManager
        { get { return _animationManager; } }

        /// <summary>
        /// Gets the panel config id for this scanner
        /// </summary>
        public Guid ConfigId { get; private set; }

        /// <summary>
        /// Gets or sets the dialog mode of the scanner.  If TRUE,
        /// the scanner is being used to interact with a dialog panel
        /// in ACAT.  Its important to know this as certanin functions
        /// are not avaliable if DialogMode is true.  Eg, user cannot
        /// activate the talk window.
        ///
        /// </summary>
        public bool DialogMode { get; private set; }

        /// <summary>
        /// Gets the display mode of the panel
        /// </summary>
        public DisplayModeTypes DisplayMode { get; private set; }

        /// <summary>
        /// Gets/sets whether to hide the scanner after the scanner goes
        /// to the idle state
        /// </summary>
        public bool HideScannerOnIdle { get; set; }

        /// <summary>
        /// Gets the paused state of the form
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
        }

        /// <summary>
        /// Helps with managing the position and size of the scanner panel
        /// </summary>
        public ScannerPositionSizeController PositionSizeController { get; private set; }

        public ScannerPositionSizeController2 PositionSizeController2 { get; private set; }

        /// <summary>
        /// Gets or sets the preview mode of the scanner.  This mode
        /// is used when the user is previewing the look and feel
        /// of the scanner (scaled up, scaled down, where the scanner
        /// should be docked etc)
        /// </summary>
        public bool PreviewMode
        {
            get
            {
                return _previewMode;
            }

            set
            {
                _previewMode = value;
                if (_windowOverlapWatchdog != null)
                {
                    if (_previewMode)
                    {
                        Windows.SetTopMost(ScannerForm, false);
                        _windowOverlapWatchdog.Pause();
                    }
                    else
                    {
                        _windowOverlapWatchdog.Resume();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the widget that reprensents the form
        /// </summary>
        public Widget RootWidget
        { get { return _rootWidget; } }

        /// <summary>
        /// Gets the parent scanner form
        /// </summary>
        public Form ScannerForm { get; private set; }

        /// <summary>
        /// Gets the arguments that were sent to the form during
        /// initialization
        /// </summary>
        public StartupArg StartupArg { get; private set; }

        /// <summary>
        /// Gets or sets the scanner status bar object
        /// </summary>
        public ScannerStatusBar StatusBar
        {
            get { return _scannerStatusBar; }

            set
            {
                _scannerStatusBar = value;
                if (StatusBarController == null)
                {
                    StatusBarController = new StatusBarController(value);
                }
                else
                {
                    StatusBarController.StatusBar = value;
                }
            }
        }

        /// <summary>
        /// Helps with managing the scanner status bar
        /// </summary>
        public StatusBarController StatusBarController { get; private set; }

        /// <summary>
        /// Gets the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncLock; }
        }

        /// <summary>
        /// Helps with auto completing words, handling abbreviations,
        /// spell check etc.
        /// </summary>
        public TextController TextController { get; private set; }

        public UserControlManager UserControlManager
        {
            get
            {
                return _userControlManager;
            }
        }

        /// <summary>
        /// Gets the WidgetManager objet
        /// </summary>
        public WidgetManager WidgetManager
        { get { return _widgetManager; } }

        /// <summary>
        /// Autocompletes the partially entered word at the
        /// caret position
        /// </summary>
        /// <param name="word">The completed word</param>
        public void AutoCompleteWord(String word)
        {
            TextController.AutoCompleteWord(word);
        }

        /// <summary>
        /// Check to see if a command should be enabled or not.
        /// This depends on the context.   The arg parameter
        /// contains the widget/command object in question.
        /// </summary>
        /// <param name="arg">Argument</param>
        public void CheckCommandEnabled(CommandEnabledArg arg)
        {
            if (_syncLock.IsClosing())
            {
                return;
            }

            _scannerPanel.CheckCommandEnabled(arg);

            if (!arg.Handled)
            {
                Context.AppAgentMgr.CheckCommandEnabled(arg);
            }
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
        /// Fades the scanner with parital opacity and pauses watchdog
        /// </summary>
        public void FadeScanner()
        {
            _windowOverlapWatchdog?.Pause();

            //Windows.SetOpacity(ScannerForm, 0.7f);
            _scannerFaded = true;
        }

        /// <summary>
        /// If the user clicked anywhere on the form, pause the animation.
        /// Call this function in the WndProc override in the form
        /// </summary>
        /// <param name="m"></param>
        /// <returns>true if handled</returns>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public bool HandleWndProc(Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xfff0;
                if (command == SC_MOVE)
                {
                    return true;
                }
            }
            else if (m.Msg == WM_MOUSEACTIVATE)
            {
                var control = Control.FromHandle(m.HWnd);
                if (control != null && (control == ScannerForm || ScannerForm.Contains(control)))
                {
                    _userControlManager.StopTopLevelAnimation();
                }
            }

            return false;
        }

        /// <summary>
        /// Hides the scanner
        /// </summary>
        public void HideScanner()
        {
            if (ScannerForm != null)
            {
                Windows.SetVisible(ScannerForm, false);
            }
        }

        /// <summary>
        /// Performs initialization.  Reads the config file for the form, creates
        /// the animation manager, widget manager, loads in  all the widgets,
        /// subscribes to events
        /// Call this function in the Initialize() function in the scanner.
        /// </summary>
        /// <param name="startupArg"></param>
        /// <returns></returns>
        public bool Initialize(StartupArg startupArg)
        {
            Log.Debug("Entered from Initialize");

            Context.AppPanelManager.EvtPanelPreShow += AppPanelManager_EvtPanelPreShow;

            StartupArg = startupArg;

            StatusBarController = new StatusBarController();

            ScannerForm.TopMost = true;

            ScannerForm.MaximizeBox = false;

            Windows.ShowWindowBorder(ScannerForm, true, ScannerForm.Text);

            Windows.EvtWindowPositionChanged += Windows_EvtWindowPositionChanged;

            ScannerForm.SizeChanged += ScannerForm_SizeChanged;

            CoreGlobals.AppPreferences.EvtPreferencesChanged += AppPreferences_EvtPreferencesChanged;

            ScannerForm.Shown += form_Shown;
            ScannerForm.VisibleChanged += form_VisibleChanged;

            _dialogMode = startupArg.DialogMode;

            var panelConfigMapEntry = PanelConfigMap.GetPanelConfigMapEntry(startupArg.PanelClass);
            if (panelConfigMapEntry == null) // did not find the panel
            {
                return false;
            }

            ConfigId = panelConfigMapEntry.ConfigId;

            bool retVal = initWidgetManager(panelConfigMapEntry);

            if (retVal)
            {
                retVal = initAnimationManager(panelConfigMapEntry);
            }

            if (retVal && HideScannerOnIdle)
            {
                createIdleTimer();
            }

            if (retVal)
            {
                PositionSizeController2.Initialize();

                PositionSizeController2.AutoSetPosition();

                _windowOverlapWatchdog = new WindowOverlapWatchdog(ScannerForm);

                WindowActivityMonitor.EvtWindowMonitorHeartbeat += WindowActivityMonitor_EvtWindowMonitorHeartbeat;

                _userControlManager.Initialize();
            }

            Context.AppPanelManager.EvtCalibrationStartNotify += AppPanelManager_EvtCalibrationStartNotify;
            Context.AppPanelManager.EvtCalibrationEndNotify += AppPanelManager_EvtCalibrationEndNotify;

            Log.Debug("Returning from Initialize " + retVal);

            return retVal;
        }

        /// <summary>
        /// Call this in the form's OnClientChanged handler.
        /// Client size changed.  Resize the form to maintain aspect ratio
        /// If the app is DPI aware, the form doesn't scale properly.  The
        /// vertical gets squeezed.  Let's store the design time aspect
        /// ratio and then use it later in the OnLoad to restore the aspect
        /// ratio.
        /// </summary>
        /// <param name="e">event args</param>
        public void OnClientSizeChanged()
        {
            if (_firstCallToClientSizeChanged)
            {
                _designTimeAspectRatio = (float)ScannerForm.ClientSize.Height / ScannerForm.ClientSize.Width;
                _firstCallToClientSizeChanged = false;
            }
        }

        /// <summary>
        /// Call this in the OnClose event handler for the form.
        /// Releases resources
        /// </summary>
        public void OnClosing()
        {
            try
            {
                _animationManager.Dispose();

                _rootWidget.Dispose();

                if (Context.AppQuit)
                {
                    Windows.ShowTaskbar();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Focus changed in the foreground application. Set widget
        /// enabled states
        /// </summary>
        /// <param name="monitorInfo">info about the active application</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            setWidgetEnabledStates(monitorInfo);
        }

        /// <summary>
        /// Call this in the OnFormClosing override in the form. Stops animation,
        /// unsubscribes from events
        /// </summary>
        /// <param name="e">args</param>
        public void OnFormClosing(FormClosingEventArgs e)
        {
            var buttonList = new List<Widget>();
            _rootWidget.Finder.FindAllChildren(buttonList);

            foreach (var widget in buttonList)
            {
                if (widget is IButtonWidget)
                {
                    widget.EvtActuated -= widget_EvtActuated;
                }
            }

            if (_animationManager != null)
            {
                _animationManager.EvtPlayerStateChanged -= animationManager_EvtPlayerStateChanged;
            }

            _userControlManager.OnClosing();

            Context.AppPanelManager.EvtPanelPreShow -= AppPanelManager_EvtPanelPreShow;

            CoreGlobals.AppPreferences.EvtPreferencesChanged -= AppPreferences_EvtPreferencesChanged;

            Log.Debug(ScannerForm.Name + ", _syncObj.Status: " + _syncLock.Status + ", hashcode: " + _syncLock.GetHashCode());

            if (_syncLock.Status != SyncLock.StatusValues.None)
            {
                Log.Debug(ScannerForm.Name + ", _syncObj.Status: " + _syncLock.Status + ", form already closed.  returning");
                return;
            }

            Log.Debug(ScannerForm.Name + ", _syncObj.Status: " + _syncLock.Status + ", Will continue closing");

            Log.Debug("Setting _syncLock.Status to CLOSING " + ScannerForm.Name);
            _syncLock.Status = SyncLock.StatusValues.Closing;

            if (_animationManager != null)
            {
                Log.Debug("Before animationmangoer.stop");
                _animationManager.Stop();
                Log.Debug("After animationmangoer.stop");
            }

            Log.Debug("Unsubscribe to EvtHeartbeat for " + ScannerForm.Name);
            WindowActivityMonitor.EvtWindowMonitorHeartbeat -= WindowActivityMonitor_EvtWindowMonitorHeartbeat;
            Log.Debug("Unsubscribe to EvtHeartbeat DONE for " + ScannerForm.Name);

            PositionSizeController2.OnClosing();

            TextController.OnClosing();

            unsubscribeEvents();

            Context.AppPanelManager.EvtCalibrationStartNotify -= AppPanelManager_EvtCalibrationStartNotify;
            Context.AppPanelManager.EvtCalibrationEndNotify -= AppPanelManager_EvtCalibrationEndNotify;

            Log.Debug("Exiting FormClosing for " + ScannerForm.Name);
        }

        /// <summary>
        /// Call this function in the OnLoad event handler for the form.
        /// Initializates the controllers, sets scanner position
        /// </summary>
        /// <param name="resetTalkWindowPosition"></param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnLoad()
        {
            restoreAspectRatio();

            PositionSizeController2.OnLoad();

            TextController.OnLoad();

            PositionSizeController2.ScaleForm();

            Windows.SetWindowPosition(ScannerForm, Context.AppWindowPosition);

            updateStatusBar();

            subscribeToEvents();

            setWidgetEnabledStates(WindowActivityMonitor.GetForegroundWindowInfo());

            Context.AppPanelManager.EvtDisplaySettingsChanged += AppPanelManager_EvtDisplaySettingsChanged;

            _userControlManager.StartTopLevelAnimation();
        }

        /// <summary>
        /// Call this in the OnPause hander in the form.
        /// </summary>
        public void OnPause(PauseDisplayMode mode = PauseDisplayMode.Default)
        {
            Log.Debug("CALIBTEST_isPaused: " + _isPaused);
            if (_isPaused)
            {
                return;
            }

            _isPaused = true;

            try
            {
                if (DialogMode)
                {
                    Log.Debug("CALIBTEST Dialog mode is true. Returning");
                    return;
                }

                Log.Debug("CALIBTEST Pausing animation manager");
                AnimationManager.Pause();
                Log.Debug("CALIBTEST calling setDisplayStateOnpause");
                setDisplayStateOnPause(mode);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Call this in the OnResume event handler in the form
        /// </summary>
        public void OnResume()
        {
            Log.Debug("CALIBTEST Scannercommon2 OnResume. is_paused: " + _isPaused);
            if (!_isPaused)
            {
                return;
            }

            _isPaused = false;

            try
            {
                PositionSizeController2.ScaleForm();

                Log.Debug("CALIBTEST Scannercommon2 Showing scanner");
                ShowScanner();

                Log.Debug("CALIBTEST Calling Animationmanager resume");
                AnimationManager.Resume();

                updateStatusBar();

                if (!DialogMode)
                {
                    PositionSizeController2.SetPositionAndNotify();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        public void PauseOverlapWatchdog()
        {
            _windowOverlapWatchdog?.Pause();
        }

        /// <summary>
        /// Resizes form to fit the height of the desktop if it
        /// exceeds the size
        /// </summary>
        /// <param name="form">the form</param>
        public void ResizeToFitDesktop(Form form)
        {
            int desktopHeight = Screen.PrimaryScreen.WorkingArea.Height;
            if (form.Height > desktopHeight)
            {
                float ratio = ((float)desktopHeight / form.Height);

                form.Top = 0;
                form.Height = desktopHeight;
                form.Width = (int)((float)form.Width * ratio);
            }
        }

        public void ResumeOverlapWatchdog()
        {
            _windowOverlapWatchdog?.Resume();
        }

        /// <summary>
        /// Creates the status bar for the form to display the status of the
        /// Ctrl, Alt and Shift keys
        /// </summary>
        public void SetStatusBar(StatusStrip statusStrip)
        {
            ToolStripStatusLabel toolStripAltStatus = null;
            ToolStripStatusLabel toolStripCtrlStatus = null;
            ToolStripStatusLabel toolStripShiftStatus = null;

            statusStrip.SizingGrip = false;

            if (statusStrip.Items.Count >= 1 && statusStrip.Items[0] is ToolStripStatusLabel)
            {
                toolStripShiftStatus = statusStrip.Items[0] as ToolStripStatusLabel;
            }

            if (statusStrip.Items.Count >= 2 && statusStrip.Items[1] is ToolStripStatusLabel)
            {
                toolStripCtrlStatus = statusStrip.Items[1] as ToolStripStatusLabel;
            }

            if (statusStrip.Items.Count >= 3 && statusStrip.Items[2] is ToolStripStatusLabel)
            {
                toolStripAltStatus = statusStrip.Items[2] as ToolStripStatusLabel;
            }

            var statusbar = new ScannerStatusBar
            {
                AltStatus = toolStripAltStatus,
                CtrlStatus = toolStripCtrlStatus,
                ShiftStatus = toolStripShiftStatus,
            };

            StatusBar = statusbar;
        }

        /// <summary>
        /// Displays the scanner. Makes it the topmost window
        /// </summary>
        public void ShowScanner()
        {
            if (ScannerForm != null)
            {
                Windows.SetVisible(ScannerForm, true);
                Windows.SetTopMost(ScannerForm);

                if (_scannerFaded)
                {
                    Windows.SetOpacity(ScannerForm, 1.0f);
                    _windowOverlapWatchdog?.Resume();
                    _scannerFaded = false;
                }
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

                if (disposing)
                {
                    Context.AppPanelManager.EvtDisplaySettingsChanged -= AppPanelManager_EvtDisplaySettingsChanged;

                    _windowOverlapWatchdog?.Dispose();

                    // dispose all managed resources.
                    if (_widgetManager != null)
                    {
                        _widgetManager.Dispose();
                    }

                    if (_idleTimer != null)
                    {
                        _idleTimer.Elapsed -= idleTimer_Elapsed;
                        _idleTimer.Dispose();
                    }

                    if (_animationManager != null)
                    {
                        _animationManager.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        /// <summary>
        /// User actuated a button on the form.  Handles the event.
        /// </summary>
        /// <param name="widget">Widget to acutate</param>
        private void actuateButton(Widget widget)
        {
            ActuatedWidget = null;

            if (_isPaused ||
                widget is WordListItemWidget ||
                String.IsNullOrEmpty(widget.Value) ||
                !(widget is IButtonWidget))
            {
                return;
            }

            var button = (IButtonWidget)widget;

            ActuatedWidget = widget;

            if (widget.IsCommand)
            {
                runCommand(widget.Command);
            }
            else if (button.GetWidgetAttribute().IsVirtualKey)
            {
                actuateVirtualKey(button.GetWidgetAttribute(), widget.Value);
            }
            else if (!Context.AppAgentMgr.TextChangedNotifications.OnHold())
            {
                if (widget.Value.Length > 1)
                {
                    CoreGlobals.Stopwatch1.Reset();
                    CoreGlobals.Stopwatch1.Start();
                    Context.AppAgentMgr.TextChangedNotifications.Hold();
                    SendKeys.SendWait(widget.Value + " ");
                    Context.AppAgentMgr.TextChangedNotifications.Release();
                    CoreGlobals.Stopwatch1.Stop();
                    Log.Debug("TimeElapsed 1: " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);
                }
                else
                {
                    CoreGlobals.Stopwatch1.Reset();
                    CoreGlobals.Stopwatch1.Start();

                    actuateKey(button.GetWidgetAttribute(), widget.Value[0]);

                    CoreGlobals.Stopwatch1.Stop();
                    Log.Debug("TimeElapsed 2 : " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);
                }
            }

            ActuatedWidget = null;
        }

        /// <summary>
        /// Handles actuation of a keyboard key (eg a-z, punctuations etc). Modifiers
        /// are optionally shift, ctrl, alt etc
        /// </summary>
        /// <param name="modifiers">modifier keys</param>
        /// <param name="value">String to send</param>
        private void actuateKey(WidgetAttribute widgetAttribute, char value)
        {
            Log.Debug(value.ToString());
            if (!TextController.HandlePunctuation(widgetAttribute.Modifiers, value))
            {
                if ((KeyStateTracker.IsShiftOn() || KeyStateTracker.IsCapsLockOn()) &&
                    !String.IsNullOrEmpty(widgetAttribute.ShiftValue))
                {
                    TextController.HandleAlphaNumericChar(widgetAttribute.ShiftValue[0]);
                }
                else
                {
                    TextController.HandleAlphaNumericChar(widgetAttribute.Modifiers, value);
                }
            }
        }

        /// <summary>
        /// Handles actuation of a button whose "virtualkey" attribute is set to
        /// true in the WidgetAtribute for the button in the panel config file.
        /// This means the key is a string representation of the Keys enum.
        /// </summary>
        /// <param name="widgetAttribute">widgetattribute of the button</param>
        /// <param name="value">value of the key</param>
        private void actuateVirtualKey(WidgetAttribute widgetAttribute, String value)
        {
            Log.Debug("VirtualKey: " + value);

            Keys key = TextController.MapVirtualKey(value);
            if (key == Keys.Escape && _dialogMode)
            {
                ScannerForm.Close();
            }
            else
            {
                TextController.HandleVirtualKey(widgetAttribute.Modifiers, value);
            }
        }

        /// <summary>
        /// The play state of the animation manager changed.  If the
        /// scanner is not running, starts the idle timer if configured)
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void animationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            if (!HideScannerOnIdle || _isPaused || PreviewMode)
            {
                return;
            }

            try
            {
                var newState = _animationManager.GetPlayerState();
                Log.Debug(ScannerForm.Name + ": PlayerState changed from " + e.OldState + " to " + newState);
                switch (newState)
                {
                    case PlayerState.Timeout:
                        _idleTimer.Start();
                        break;

                    case PlayerState.Running:
                        _idleTimer.Stop();
                        Windows.SetOpacity(ScannerForm, 1.0);
                        Windows.SetVisible(ScannerForm, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Tests whether the point x,y lies within the bounds of the scanner
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        private bool AppAgent_EvtScannerHitTest(int x, int y)
        {
            bool retVal = false;
            if (ScannerForm != null)
            {
                retVal = new Rectangle(ScannerForm.Location.X,
                                        ScannerForm.Location.Y,
                                        ScannerForm.Width,
                                        ScannerForm.Height).Contains(x, y);
            }

            return retVal;
        }

        /// <summary>
        /// Event handler for the event that was raised to indicated that
        /// the text has changed in the currently active applicatoin window.
        /// This is raised by the application agents.  The event is also
        /// triggered if the cursor position changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppAgent_EvtTextChanged(object sender, EventArgs e)
        {
            Log.Debug("Enter " + Kernel32Interop.GetCurrentThreadId());
            try
            {
                if (Windows.GetVisible(ScannerForm) &&
                    !(ScannerForm is MenuPanelBase) &&
                    Context.AppAgentMgr.CurrentEditingMode == EditingMode.TextEntry)
                {
                    bool abbreviationDetected = false;

                    using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                    {
                        if (!DialogMode)
                        {
                            if (context.TextAgent().ExpandAbbreviations())
                            {
                                abbreviationDetected = checkAndExpandAbbreviation();
                            }
                        }

                        if (!abbreviationDetected && !context.TextAgent().SupportsSpellCheck())
                        {
                            Log.Debug("Calling spellccheck " + Kernel32Interop.GetCurrentThreadId());
                            TextController.SpellCheck();
                            Log.Debug("Returned from spellccheck " + Kernel32Interop.GetCurrentThreadId());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            Log.Debug("Leave " + Kernel32Interop.GetCurrentThreadId());
        }

        /// <summary>
        /// Event handler for end of calibration
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppPanelManager_EvtCalibrationEndNotify(object sender, EventArgs e)
        {
            Log.Debug("CALIBTEST Calibration end notify for " + ScannerForm.Name);

            if ((Context.AppPanelManager.GetCurrentForm() as Form) != ScannerForm)
            {
                Log.Debug("CALIBTESTForm is not the current form. returning " + ScannerForm.Name + " CurrentForm is " + (Context.AppPanelManager.GetCurrentForm() as Form).Name);
                return;
            }

            _windowOverlapWatchdog?.Resume();

            if (_hideScannerOnCalibration)
            {
                Windows.SetVisible(ScannerForm, true);
                _hideScannerOnCalibration = false;
            }
            else
            {
                Log.Debug("CALIBTESTCalling OnResume for scanner");
                (ScannerForm as IScannerPanel).OnResume();
            }
        }

        /// <summary>
        /// Event handler for beginning of calibration
        /// </summary>
        /// <param name="args">event args</param>
        private void AppPanelManager_EvtCalibrationStartNotify(ActuatorManagement.CalibrationNotifyEventArgs args)
        {
            Log.Debug("CALIBTEST ScannerCommon2: Calibration start notify for " + ScannerForm.Name);

            if ((Context.AppPanelManager.GetCurrentForm() as Form) != ScannerForm)
            {
                Log.Debug("CALIBTEST Form is not the current form. returning " + ScannerForm.Name + " CurrentForm is " + (Context.AppPanelManager.GetCurrentForm() as Form).Name);
                return;
            }

            if (ScannerForm != null && Windows.GetVisible(ScannerForm))
            {
                _windowOverlapWatchdog?.Pause();
                if (args.HideScanner)
                {
                    Windows.SetVisible(ScannerForm, false);
                    _hideScannerOnCalibration = true;
                }
                else
                {
                    Log.Debug("CALIBTEST Calling onPause for " + ScannerForm.Name);

                    (ScannerForm as IScannerPanel).OnPause();
                }
            }
        }

        /// <summary>
        /// Event handler for when the display settings change (connect to a monitor etc)
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppPanelManager_EvtDisplaySettingsChanged(object sender, EventArgs e)
        {
            if (ScannerForm.Visible)
            {
                PositionSizeController2.ScaleForm();
                PositionSizeController2.SetPositionAndNotify();
            }
        }

        /// <summary>
        /// Event handler for JUST before the panel is displayed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        private void AppPanelManager_EvtPanelPreShow(object sender, PanelPreShowEventArg arg)
        {
            if (arg.Panel == _scannerPanel)
            {
                DisplayMode = arg.DisplayMode;
            }
        }

        /// <summary>
        /// Event handler for when preferences change
        /// </summary>
        private void AppPreferences_EvtPreferencesChanged()
        {
            HideScannerOnIdle = CoreGlobals.AppPreferences.HideScannerOnIdle;
        }

        /// Checks if the word at the caret represents an abbreviation
        /// and if it requires expansion, does so
        /// </summary>
        /// <returns>true if abbr was expanded successfully</returns>
        private bool checkAndExpandAbbreviation()
        {
            bool retVal = false;

            Abbreviation abbr = TextController.CheckAndReplaceAbbreviation(ref retVal);
            if (!retVal && abbr != null && abbr.Mode == Abbreviation.AbbreviationMode.Speak)
            {
                retVal = true;
                textToSpeech(abbr.Expansion);
            }

            return retVal;
        }

        /// <summary>
        /// Creates the timer to track scanner idle time
        /// </summary>
        private void createIdleTimer()
        {
            _idleTimer = new System.Timers.Timer();
            _idleTimer.Elapsed += idleTimer_Elapsed;
            _idleTimer.Interval = CoreGlobals.AppPreferences.HideOnIdleTimeout;
        }

        /// <summary>
        /// Event handler for form Shown event. The scanner form
        /// was displayed. Position it in its place
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void form_Shown(object sender, EventArgs e)
        {
            Log.Debug("Shown " + ScannerForm.Name);
            Windows.SetTopMost(ScannerForm);
        }

        /// <summary>
        /// Form visibility changed. Handle it
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void form_VisibleChanged(object sender, EventArgs e)
        {
            Log.Debug("Form Visibility for " + ScannerForm.Name + ":  " + ScannerForm.Visible);

            if (ScannerForm.Visible)
            {
                Context.AppAgentMgr.EvtScannerHitTest += AppAgent_EvtScannerHitTest;

                // the following two lines are crucial.  Since the form style has
                // WS_EX_COMPOSITED for smooth rendering, there is a bug in
                // windows where if the visibility of the form changes, the form
                // doesn't display.  Everything is there, but the form itself doesn't
                // paint.  To reproduce the bug:
                // Display the alphabet scanner
                // Change the desktop resolution
                // Go to the main menu (or any other scanner)
                // Go back to the alphabet scanner.  You will see only the border
                // with nothing inside.  Changing the size of the scanner fixes this.
                ScannerForm.Height += 1;
                ScannerForm.Height -= 1;

                if (_scannerPanel.PanelClass == "Alphabet")
                {
                    Log.Debug("form_visibleChanged " + ScannerForm.Width);
                }
                notifyScannerShow();
            }
            else
            {
                Context.AppAgentMgr.EvtScannerHitTest -= AppAgent_EvtScannerHitTest;
            }
        }

        /// <summary>
        /// User did not interact with the scanner for 'idletime'.
        /// Makes it fade away.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void idleTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _idleTimer.Stop();

            if (_animationManager.GetPlayerState() == PlayerState.Timeout)
            {
                Windows.FadeOut(ScannerForm);
            }
        }

        /// <summary>
        /// Creates the animation manager and initialize it  Load
        /// all the animations from the config file.
        /// </summary>
        /// <param name="configFileName">name of the animation file</param>
        /// <returns></returns>
        private bool initAnimationManager(PanelConfigMapEntry panelConfigMapEntry)
        {
            bool retVal = true;

            _animationManager = new AnimationManager();
            if (_animationManager.Init(panelConfigMapEntry, _rootWidget))
            {
                _animationManager.EvtPlayerStateChanged += animationManager_EvtPlayerStateChanged;
            }
            else
            {
                Log.Error("Error initializing animation manager");
                _animationManager = null;
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Create the widget manager. Load all the widgets
        /// from the config file
        /// </summary>
        /// <param name="panelConfigMapEntry">config map entry for the panel</param>
        /// <returns>true on success</returns>
        private bool initWidgetManager(PanelConfigMapEntry panelConfigMapEntry)
        {
            _widgetManager = new WidgetManager(ScannerForm);
            _widgetManager.Layout.SetColorScheme(ColorSchemes.ScannerSchemeName);
            _widgetManager.Layout.SetDisabledButtonColorScheme(ColorSchemes.DisabledScannerButtonSchemeName);

            bool retVal = _widgetManager.Initialize(panelConfigMapEntry.ConfigFileName);
            if (!retVal)
            {
                Log.Error("Unable to initialize widget manager");
            }
            else
            {
                _rootWidget = _widgetManager.RootWidget;
                if (String.IsNullOrEmpty(_rootWidget.SubClass))
                {
                    _rootWidget.SubClass = (ScannerForm is MenuPanelBase) ?
                                            PanelCategory.Menu.ToString() :
                                            PanelCategory.Scanner.ToString();
                }
            }

            return retVal;
        }

        /// <summary>
        /// The interpreter invoked a "close" command.  Close
        /// the scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Interpreter_EvtCloseNotify(object sender, InterpreterEventArgs e)
        {
            Windows.CloseAsync(ScannerForm);
        }

        /// <summary>
        /// A 'run' command was interpreted by the interpreter.  Perform
        /// the necessary action.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Interpreter_EvtRun(object sender, InterpreterRunEventArgs e)
        {
            if (PreviewMode)
            {
                return;
            }

            Log.Debug(e.Script);

            runCommand(e.Script);
        }

        /// <summary>
        /// Event handler to show a scanner as popup
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Interpreter_EvtShowPopup(object sender, InterpreterEventArgs e)
        {
            if (e.Args.Count == 0)
            {
                return;
            }

            String scannerName = e.Args[0];
            String title = (e.Args.Count > 1) ? e.Args[1] : string.Empty;

            (ScannerForm).Invoke(new MethodInvoker(delegate
            {
                IPanel panel = Context.AppPanelManager.CreatePanel(scannerName, title) as IPanel;
                if (panel != null)
                {
                    Context.AppPanelManager.ShowPopup(ScannerForm as IPanel, panel);
                }
            }));
        }

        /// <summary>
        /// The keystate of one of the modifier keys (shift, alt,
        /// ctrl or function) changed.  Update the scanner status bar
        /// </summary>
        private void KeyStateTracker_EvtKeyStateChanged()
        {
            updateStatusBar();
        }

        /// <summary>
        /// Raises an event to notify that the scanner was displayed
        /// </summary>
        private void notifyScannerShow()
        {
            if (Windows.GetVisible(ScannerForm) && EvtScannerShow != null)
            {
                var arg = new ScannerShowEventArg(_scannerPanel);
                EvtScannerShow(_scannerPanel, arg);
            }
        }

        /// <summary>
        /// Restores the aspect ratio of the form to the one at
        /// design time
        /// </summary>
        private void restoreAspectRatio()
        {
            float currentAspectRatio = (float)ScannerForm.ClientSize.Height / ScannerForm.ClientSize.Width;

            if (_designTimeAspectRatio != 0.0f && currentAspectRatio != _designTimeAspectRatio)
            {
                ScannerForm.ClientSize = new Size(ScannerForm.ClientSize.Width, (int)(_designTimeAspectRatio * ScannerForm.ClientSize.Width));
            }
        }

        /// <summary>
        /// Runs the specified command.  The command can be
        /// prefixed by 'agent.' in which case the application
        /// agent is invoked to run the command.  If the command
        /// is prefixed with 'scanner.' the scanner is invoked to run
        /// the command. If there is no prefix, then the scanner is
        /// invoked first, if it doesn't handle it, then the application
        /// agent is invoked.
        /// </summary>
        /// <param name="command"></param>
        private void runCommand(String command)
        {
            bool handled = false;

            if (command[0] == '@')
            {
                command = command.Substring(1);
            }

            Log.Debug("Calling scanner common runcomand with " + command);
            ScannerForm.Invoke(new MethodInvoker(delegate
            {
                String[] parts = command.Split('.');
                if (parts.Length > 1)
                {
                    if (String.Compare(parts[0], "agent", true) == 0)
                    {
                        runCommandAgent(parts[1], ref handled);
                    }
                    else if (String.Compare(parts[0], "scanner", true) == 0)
                    {
                        runCommandScanner(parts[1], ref handled);
                    }
                }
                else
                {
                    runCommandScanner(command, ref handled);
                    if (!handled)
                    {
                        runCommandAgent(command, ref handled);
                    }
                }
            }));
        }

        /// <summary>
        /// Invokes the active application agent to run the specified command
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <param name="handled">was it handled?</param>
        private void runCommandAgent(String command, ref bool handled)
        {
            Context.AppAgentMgr.RunCommand(command, null, ref handled);
        }

        /// <summary>
        /// Invokes the command dispatcher associated with this scanner
        /// to run the specified command
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="handled">was it handled?</param>
        private void runCommandScanner(String command, ref bool handled)
        {
            var dispatcher = _scannerPanel.CommandDispatcher;
            if (dispatcher != null)
            {
                dispatcher.Dispatch(command, ref handled);
            }
        }

        /// <summary>
        /// Scanner size changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ScannerForm_SizeChanged(object sender, EventArgs e)
        {
            if (_scannerPanel.PanelClass == "Alphabet")
            {
                Log.Debug("ScannerForm_SizeChanged " + ScannerForm.Width);
            }
            notifyScannerShow();
        }

        /// <summary>
        /// Handle visibility of scanner when it is paused
        /// </summary>
        /// <param name="mode">what to do?</param>
        private void setDisplayStateOnPause(PauseDisplayMode mode)
        {
            Log.Debug("CALIBTEST scannerocmmon2. setDisplayStateOnPause. mode: " + mode);
            if (mode == PauseDisplayMode.None)
            {
                Log.Debug("CALIBTEST scannerocmmon2. it is none. Returning");
                return;
            }

            switch (mode)
            {
                case PauseDisplayMode.HideScanner:
                    HideScanner();
                    break;

                case PauseDisplayMode.FadeScanner:
                    FadeScanner();
                    break;

                case PauseDisplayMode.Default:
                    if (Context.AppPanelManager.PreShowPanelDisplayMode == DisplayModeTypes.Popup)
                    {
                        FadeScanner();
                    }
                    else
                    {
                        HideScanner();
                    }
                    break;
            }
        }

        /// <summary>
        /// Get all the widgets whose "enabled" property is set to
        /// contextual in the config file.  Decide the "enabled" state
        /// of these widgets depending on the context
        /// </summary>
        /// <param name="monitorInfo">Context info about active window</param>
        private void setWidgetEnabledStates(WindowActivityMonitorInfo monitorInfo)
        {
            if (_syncLock.IsClosing())
            {
                Log.Debug("Form is closing " + ScannerForm.Name);
                WindowActivityMonitor.EvtWindowMonitorHeartbeat -= WindowActivityMonitor_EvtWindowMonitorHeartbeat;
                return;
            }

            if (_rootWidget != null && Context.AppAgentMgr != null && !_syncLock.IsClosing() && Windows.GetVisible(ScannerForm))
            {
                foreach (Widget widget in _rootWidget.WidgetLayout.ContextualWidgets)
                {
                    //Log.Debug("Widget: " + widget.Name + ", subclass: " + widget.SubClass);
                    if (widget.IsCommand)
                    {
                        var arg = new CommandEnabledArg(monitorInfo, widget.Command, widget);
                        if (!_syncLock.IsClosing())
                        {
                            _scannerPanel.CheckCommandEnabled(arg);

                            if (!arg.Handled)
                            {
                                Context.AppAgentMgr.CheckCommandEnabled(arg);
                            }

                            widget.Enabled = arg.Handled ? arg.Enabled : widget.DefaultEnabled;
                        }
                        else
                        {
                            break;
                        }

                        Log.Debug("widget.Enabled set to : " + widget.Enabled + " for feature " + widget.SubClass);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribes to events triggered when buttons are actuated -
        /// i.e., the button was triggered
        /// </summary>
        private void subscribeToButtonEvents()
        {
            var buttonList = new List<Widget>();
            _rootWidget.Finder.FindAllChildren(buttonList);

            foreach (var widget in buttonList)
            {
                if (widget is IButtonWidget)
                {
                    widget.EvtActuated += widget_EvtActuated;
                }
            }
        }

        /// <summary>
        /// Subscribe to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            KeyStateTracker.EvtKeyStateChanged += KeyStateTracker_EvtKeyStateChanged;

            Context.AppAgentMgr.EvtTextChanged += AppAgent_EvtTextChanged;

            AnimationManager.Interpreter.EvtCloseNotify += Interpreter_EvtCloseNotify;
            AnimationManager.Interpreter.EvtRun += Interpreter_EvtRun;
            AnimationManager.Interpreter.EvtShowPopup += Interpreter_EvtShowPopup;

            subscribeToButtonEvents();
        }

        /// <summary>
        /// Convers the indicated text to speech thorugh the
        /// active text-to-speech engine
        /// </summary>
        /// <param name="text">text to convert</param>
        private void textToSpeech(String text)
        {
            Log.Debug("Convert to speech. text=" + text);
            Context.AppTTSManager.ActiveEngine.Speak(text);
            AuditLog.Audit(new AuditEventTextToSpeech(Context.AppTTSManager.ActiveEngine.Descriptor.Name));
        }

        /// <summary>
        /// Unsubscribe from events previously subscribed to.
        /// </summary>
        private void unsubscribeEvents()
        {
            Context.AppAgentMgr.EvtTextChanged -= AppAgent_EvtTextChanged;

            Context.AppAgentMgr.EvtScannerHitTest -= AppAgent_EvtScannerHitTest;
            Windows.EvtWindowPositionChanged -= Windows_EvtWindowPositionChanged;
            ScannerForm.Shown -= form_Shown;
            ScannerForm.VisibleChanged -= form_VisibleChanged;
            ScannerForm.SizeChanged -= ScannerForm_SizeChanged;

            if (AnimationManager != null)
            {
                AnimationManager.Interpreter.EvtCloseNotify -= Interpreter_EvtCloseNotify;
                AnimationManager.Interpreter.EvtRun -= Interpreter_EvtRun;
            }

            KeyStateTracker.EvtKeyStateChanged -= KeyStateTracker_EvtKeyStateChanged;
        }

        /// <summary>
        /// Updates the status bar panels with the current state of the
        /// Shift, Ctrl and Alt keys
        /// </summary>
        private void updateStatusBar()
        {
            if (ScannerForm is ISupportsStatusBar && StatusBarController != null)
            {
                StatusBarController.UpdateStatusBar();
            }
        }

        /// <summary>
        /// The user actuated a widget.  perform the necessary
        /// action
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtActuated(object sender, WidgetActuatedEventArgs e)
        {
            if (PreviewMode || _isPaused)
            {
                return;
            }

            var widget = e.SourceWidget;

            bool handled = false;
            _scannerPanel.OnWidgetActuated(e, ref handled);

            if (!handled && widget is IButtonWidget)
            {
                var value = widget.Value;
                if (!String.IsNullOrEmpty(value))
                {
                    Log.Debug("**Actuate** " + widget.Name + " Value: " + value);

                    actuateButton(widget);
                }
            }
        }

        /// <summary>
        /// Heartbeat handler from the windowactivitymonitor.  Uses this
        /// monitor to set the 'enabled' state of the widgets in the scanner
        /// based on the context.
        /// </summary>
        /// <param name="monitorInfo">active window context info</param>
        private void WindowActivityMonitor_EvtWindowMonitorHeartbeat(WindowActivityMonitorInfo monitorInfo)
        {
            try
            {
                setWidgetEnabledStates(monitorInfo);
            }
            catch
            {
            }
        }

        /// <summary>
        /// The window position of the scanner changed.
        /// </summary>
        /// <param name="form">Scanner form</param>
        /// <param name="position">current position</param>
        private void Windows_EvtWindowPositionChanged(Form form, Windows.WindowPosition position)
        {
            if (form == ScannerForm)
            {
                if (PositionSizeController2.AutoPosition)
                {
                    Context.AppWindowPosition = position;
                }

                if (_scannerPanel.PanelClass == "Alphabet")
                {
                    Log.Debug("WindowPosChanged" + ScannerForm.Width);
                }
                notifyScannerShow();
            }
        }
    }
}