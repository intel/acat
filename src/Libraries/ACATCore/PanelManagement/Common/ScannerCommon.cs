////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerCommon.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.TalkWindowManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

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
    public class ScannerCommon : IDisposable
    {
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
        /// Has the scanner been shown as a modal dialog?
        /// </summary>
        private bool _dialogMode;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

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
        /// Did we pause the talk window?
        /// </summary>
        private bool _talkWindowPaused;

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
        /// Instantiates a new instance of the class
        /// </summary>
        /// <param name="iScannerPanel">The scanner Form object</param>
        public ScannerCommon(IScannerPanel iScannerPanel)
        {
            ScannerForm = iScannerPanel.Form;
            StartupArg = null;
            _scannerPanel = iScannerPanel;
            ScannerForm.ShowInTaskbar = false;
            PositionSizeController = new ScannerPositionSizeController(this);
            TextController = new TextController();
            KeepTalkWindowActive = false;
            _syncLock = new SyncLock();

            var scannerStatusBar = (ScannerForm is ISupportsStatusBar) ?
                                                ((ISupportsStatusBar)ScannerForm).ScannerStatusBar :
                                                null;
            StatusBarController = new StatusBarController(scannerStatusBar);
        }

        /// <summary>
        /// Raised when the scanner is shown
        /// </summary>
        public static event ScannerShow EvtScannerShow;

        /// <summary>
        /// Gets the widget that was actuated as a result of one
        /// of the actuator switches trigerring
        /// </summary>
        public Widget ActuatedWidget { get; private set; }

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
        /// Gets the paused state of the form
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
        }

        /// <summary>
        /// Gets or sets the keep talk window property
        /// When this scanner goes inactive and a new one replaces it, whether
        /// to hide the talk window or keep it in its current state.
        /// </summary>
        public bool KeepTalkWindowActive { get; set; }

        /// <summary>
        /// Helps with managing the position and size of the scanner panel
        /// </summary>
        public ScannerPositionSizeController PositionSizeController { get; private set; }

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
        /// The parent scanner form
        /// </summary>
        public Form ScannerForm { get; private set; }

        /// <summary>
        /// Arguments that were sent to the form during
        /// initialization
        /// </summary>
        public StartupArg StartupArg { get; private set; }

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
        /// Helps with auto completing words, handling abbreviations, spell check
        /// etc.
        /// </summary>
        public TextController TextController { get; private set; }

        /// <summary>
        /// Activates the button that was triggered by
        /// a switch actuator. 'command' is the character associated
        /// with the button. For eg, if 'command' is the letter 'a',
        /// the letter a will be sent to the application window
        /// </summary>
        /// <param name="widget">widget to activate</param>
        /// <param name="value">char associated with the button</param>
        public void ActuateButton(Widget widget, char value)
        {
            if (widget is IButtonWidget)
            {
                var button = (IButtonWidget)widget;
                actuateNormalKey(button.GetWidgetAttribute().Modifiers, value);
            }
        }

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
        /// Returns the animation manager object
        /// </summary>
        /// <returns></returns>
        public AnimationManager GetAnimationManager()
        {
            return _animationManager;
        }

        /// <summary>
        /// Returns the root widget for the form (this is the
        /// widget wrapper for the form itself)
        /// </summary>
        /// <returns></returns>
        public Widget GetRootWidget()
        {
            return _rootWidget;
        }

        /// <summary>
        /// Returns the widgetmanager object
        /// </summary>
        /// <returns></returns>
        public WidgetManager GetWidgetManager()
        {
            return _widgetManager;
        }

        /// <summary>
        /// If the user clicked anywhere on the form, pause the animation.
        /// Call this function in the WndProc override in the form
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public bool HandleWndProc(Message m)
        {
            bool retVal = false;
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                Control control = Control.FromHandle(m.HWnd);
                if (control != null && (control == ScannerForm || ScannerForm.Contains(control)))
                {
                    retVal = true;
                    _animationManager.Interrupt();
                }
            }

            return retVal;
        }

        /// <summary>
        /// Hides the glass window
        /// </summary>
        public void HideGlass()
        {
            Glass.HideGlass();
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
        /// Hides the talk window
        /// </summary>
        public void HideTalkWindow()
        {
            if (Context.AppTalkWindowManager.IsTalkWindowActive)
            {
                ScannerForm.Invoke(new MethodInvoker(() => Context.AppTalkWindowManager.ToggleTalkWindow()));
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

            Glass.Enable = CoreGlobals.AppPreferences.EnableGlass;

            Glass.EvtShowGlass += Glass_EvtShowGlass;

            StartupArg = startupArg;

            ScannerForm.AutoScaleMode = AutoScaleMode.None;

            ScannerForm.TopMost = true;

            Windows.ShowWindowBorder(ScannerForm,
                                    CoreGlobals.AppPreferences.ScannerShowBorder,
                                    CoreGlobals.AppPreferences.ScannerShowTitleBar ? ScannerForm.Text : String.Empty);

            Windows.EvtWindowPositionChanged += Windows_EvtWindowPositionChanged;

            ScannerForm.SizeChanged += ScannerForm_SizeChanged;

            subscribeTalkWindowManager();

            ScannerForm.Shown += form_Shown;
            ScannerForm.VisibleChanged += form_VisibleChanged;

            _dialogMode = startupArg.DialogMode;

            var configFile = startupArg.ConfigFileName;
            if (String.IsNullOrEmpty(configFile))
            {
                configFile = PanelConfigMap.GetConfigFileForScreen(ScannerForm.GetType());
            }

            bool retVal = initWidgetManager(configFile);

            if (retVal)
            {
                retVal = initAnimationManager(configFile);
            }

            if (retVal)
            {
                createIdleTimer();
            }

            PositionSizeController.Initialize();

            PositionSizeController.AutoSetPosition();

            _windowOverlapWatchdog = new WindowOverlapWatchdog(ScannerForm);

            WindowActivityMonitor.EvtWindowMonitorHeartbeat += WindowActivityMonitor_EvtWindowMonitorHeartbeat;

            Log.Debug("Returning from Initialize");
            return retVal;
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
                    Context.AppTalkWindowManager.Dispose();
                    Glass.HideGlass();
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
        /// <param name="monitorInfo"></param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            SetWidgetEnabledStates(monitorInfo);
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

            Glass.EvtShowGlass -= Glass_EvtShowGlass;

            Log.Debug(ScannerForm.Name + ", _syncObj.Status: " + _syncLock.Status + ", hashcode: " + _syncLock.GetHashCode());

            if (_syncLock.Status != SyncLock.StatusValues.None)
            {
                Log.Debug(ScannerForm.Name + ", _syncObj.Status: " + _syncLock.Status + ", form already closed.  returning");
                return;
            }

            Log.Debug(ScannerForm.Name + ", _syncObj.Status: " + _syncLock.Status + ", Will continue closing");

            Log.Debug("Setting _syncLock.Status to CLOSING " + ScannerForm.Name);
            _syncLock.Status = SyncLock.StatusValues.Closing;

            Log.Debug("Before animationmangoer.stop");
            _animationManager.Stop();
            Log.Debug("After animationmangoer.stop");

            Log.Debug("Unsubscribe to EvtHeartbeat for " + ScannerForm.Name);
            WindowActivityMonitor.EvtWindowMonitorHeartbeat -= WindowActivityMonitor_EvtWindowMonitorHeartbeat;
            Log.Debug("Unsubscribe to EvtHeartbeat DONE for " + ScannerForm.Name);

            PositionSizeController.OnClosing();

            TextController.OnClosing();

            unsubscribeEvents();

            unsubscribeTalkWindowManager();

            Log.Debug("Exiting FormClosing for " + ScannerForm.Name);
        }

        /// <summary>
        /// Call this function in the OnLoad event handler for the form.
        /// Initializates the controllers, sets scanner position
        /// </summary>
        /// <param name="resetTalkWindowPosition"></param>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnLoad(bool resetTalkWindowPosition = true)
        {
            PositionSizeController.OnLoad();

            TextController.OnLoad();

            PositionSizeController.ScaleForm();
            Windows.SetWindowPosition(ScannerForm, Context.AppWindowPosition);

            if (Context.ShowTalkWindowOnStartup)
            {
                Context.AppTalkWindowManager.ToggleTalkWindow();
                Context.ShowTalkWindowOnStartup = false;
            }

            if (resetTalkWindowPosition)
            {
                Context.AppTalkWindowManager.SetTalkWindowPosition(ScannerForm);
            }

            StatusBarController.UpdateStatusBar();

            subscribeToEvents();

            SetWidgetEnabledStates(WindowActivityMonitor.GetForegroundWindowInfo());
        }

        /// <summary>
        /// Call this in the OnPause hander in the form.
        /// </summary>
        public void OnPause()
        {
            if (_isPaused)
            {
                return;
            }

            _isPaused = true;

            try
            {
                if (!DialogMode)
                {
                    if (!KeepTalkWindowActive)
                    {
                        if (!Context.AppTalkWindowManager.IsPaused)
                        {
                            ScannerForm.Invoke(new MethodInvoker(delegate
                            {
                                Context.AppTalkWindowManager.Pause();

                                _talkWindowPaused = true;
                            }));
                        }
                        else
                        {
                            _talkWindowPaused = false;
                        }
                    }
                }
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
            if (!_isPaused)
            {
                return;
            }

            _isPaused = false;

            try
            {
                StatusBarController.UpdateStatusBar();

                PositionSizeController.ScaleForm();

                if (!DialogMode)
                {
                    if (!KeepTalkWindowActive)
                    {
                        Log.Debug(ScannerForm.Name + ", _talkWindowPaused : " + _talkWindowPaused);
                        if (_talkWindowPaused)
                        {
                            Log.Debug(ScannerForm.Name + ", Resuming talk window: " + _talkWindowPaused);

                            Context.AppTalkWindowManager.Resume();

                            _talkWindowPaused = false;
                        }
                    }

                    PositionSizeController.AutoSetPositionAndNotify();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Displays the transluscent glass that overlays
        /// the screen behind the scanner
        /// </summary>
        public void ShowGlass()
        {
            Glass.ShowGlass();
            if (Glass.Enable)
            {
                Windows.SetTopMost(ScannerForm);
            }
        }

        /// <summary>
        /// Displays the scanner. Makes it the topmost window
        /// </summary>
        public void ShowScanner()
        {
            if (ScannerForm != null)
            {
                Windows.SetTopMost(ScannerForm);
                Windows.SetVisible(ScannerForm, true);
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
                    if (_windowOverlapWatchdog != null)
                    {
                        _windowOverlapWatchdog.Dispose();
                    }

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
        /// User actuated a button on the form.  Handle the event.
        /// </summary>
        /// <param name="widget">Widget to acutate</param>
        private void actuateButton(Widget widget)
        {
            ActuatedWidget = null;

            if (widget is WordListItemWidget)
            {
                return;
            }

            if (String.IsNullOrEmpty(widget.Value) || !(widget is IButtonWidget))
            {
                return;
            }

            var button = (IButtonWidget)widget;

            ActuatedWidget = widget;

            // If the first letter of text is '@', this is a special key.
            if (widget.Value[0] != '@')
            {
                actuateNormalKey(button.GetWidgetAttribute().Modifiers, widget.Value[0]);
            }
            else
            {
                actuateSpecialKey(button.GetWidgetAttribute().Modifiers, widget.Value, button.GetWidgetAttribute().IsVirtualKey);
            }

            ActuatedWidget = null;
        }

        /// <summary>
        /// Handle actuation of a non-special key (eg a-z, etc). Modifiers
        /// are optionally shift, ctrl, alt etc
        /// </summary>
        /// <param name="modifiers">modifier keys</param>
        /// <param name="value">String to send</param>
        private void actuateNormalKey(ArrayList modifiers, char value)
        {
            Log.Debug(value.ToString());
            if (!TextController.HandlePunctuation(modifiers, value))
            {
                TextController.HandleAlphaNumericChar(modifiers, value);
            }
        }

        /// <summary>
        /// If the button command begins with an @, it could either
        /// be a virtual key defined in the .NET class Keys, or
        /// it could be a command
        /// If is a command, the form is notified and it's handled there
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="value"></param>
        /// <param name="isVirtualKey"></param>
        private void actuateSpecialKey(ArrayList modifiers, String value, bool isVirtualKey)
        {
            Log.Debug("command=" + value + " isVirtualKey=" + isVirtualKey.ToString());

            if (isVirtualKey)
            {
                Keys key = TextController.MapVirtualKey(value);
                if (key == Keys.Escape && Context.AppTalkWindowManager.IsTalkWindowVisible)
                {
                    Context.AppTalkWindowManager.CloseTalkWindow();
                }
                else if (key == Keys.Escape && _dialogMode)
                {
                    ScannerForm.Close();
                }
                else
                {
                    TextController.HandleVirtualKey(modifiers, value);
                }
            }
            else
            {
                // this is a command.
                runCommand(value);
            }
        }

        /// <summary>
        /// The play state of the animation manager changed.  If the
        /// scanner is not running, start the idle timer if configured)
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void animationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            if (!CoreGlobals.AppPreferences.HideScannerOnIdle || _isPaused || PreviewMode)
            {
                return;
            }

            try
            {
                var newState = _animationManager.GetPlayerState();
                Log.Debug(ScannerForm.Name + ": PlayerState changed from " + e.OldState.ToString() + " to " + newState.ToString());
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
        /// Tests whether the point x,y lies within
        /// the bounds of the scanner
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns></returns>
        private bool AppAgent_EvtScannerHitTest(int x, int y)
        {
            bool retVal = false;
            if (ScannerForm != null)
            {
                retVal = new Rectangle(ScannerForm.Location.X, ScannerForm.Location.Y, ScannerForm.Width, ScannerForm.Height).Contains(x, y);
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
            Log.Debug("Enter " + GetCurrentThreadId());
            try
            {
                if (Windows.GetVisible(ScannerForm) && Context.AppAgentMgr.CurrentEditingMode == EditingMode.TextEntry)
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
                            Log.Debug("Calling spellccheck " + GetCurrentThreadId());
                            TextController.SpellCheck();
                            Log.Debug("Returned from spellccheck " + GetCurrentThreadId());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            Log.Debug("Leave " + GetCurrentThreadId());
        }

        /// <summary>
        /// Event handler called when the talk window is cleared
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppTalkWindowManager_EvtTalkWindowCleared(object sender, EventArgs e)
        {
            TextController.OnTextCleared();
            KeyStateTracker.ClearAll();
        }

        /// <summary>
        /// Event handler for the event raised when the talk window is closed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppTalkWindowManager_EvtTalkWindowClosed(object sender, EventArgs e)
        {
            _talkWindowPaused = false;
        }

        /// <summary>
        /// Handler for the event raised when the talk window is created
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AppTalkWindowManager_EvtTalkWindowCreated(object sender, TalkWindowCreatedEventArgs e)
        {
            Context.AppTalkWindowManager.SetTalkWindowPosition(ScannerForm);
        }

        /// <summary>
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
        /// Create the timer to track scanner idle time
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
            PositionSizeController.AutoSetPositionAndNotify();
        }

        /// <summary>
        /// Form visibility changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void form_VisibleChanged(object sender, EventArgs e)
        {
            if (ScannerForm.Visible)
            {
                Log.Debug("FormVisible " + ScannerForm.Name);
                Log.Debug("Form: " + ScannerForm.Name + ", SUBSCRIBE EVTSCANNERHITTEST");
                Context.AppAgentMgr.EvtScannerHitTest += AppAgent_EvtScannerHitTest;
                notifyScannerShow();
            }
            else
            {
                Log.Debug("Form: " + ScannerForm.Name + ", UNSUBSCRIBE EVTSCANNERHITTEST");
                Context.AppAgentMgr.EvtScannerHitTest -= AppAgent_EvtScannerHitTest;
            }
        }

        /// <summary>
        /// Handler for the event raised when the translucent
        /// glass is displayed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Glass_EvtShowGlass(object sender, EventArgs e)
        {
            if (!_syncLock.IsClosing() && Windows.GetVisible(ScannerForm))
            {
                Windows.SetTopMost(ScannerForm);
            }
        }

        /// <summary>
        /// User did not interact with the scanner for 'idletime'.
        /// Make it fade away.
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
        /// Create the animation manager and initialize it  Load
        /// all the animations from the config file.
        /// </summary>
        /// <param name="configFileName">name of the animation file</param>
        /// <returns></returns>
        private bool initAnimationManager(String configFileName)
        {
            bool retVal = true;

            _animationManager = new AnimationManager();
            if (_animationManager.Init(configFileName))
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
        /// <param name="configFileName">name of the animation file</param>
        /// <returns></returns>
        private bool initWidgetManager(String configFileName)
        {
            _widgetManager = new WidgetManager(ScannerForm);
            _widgetManager.Layout.SetColorScheme(ColorSchemes.ScannerSchemeName);
            _widgetManager.Layout.SetDisabledButtonColorScheme(ColorSchemes.DisabledScannerButtonSchemeName);
            bool retVal = _widgetManager.Initialize(configFileName);
            if (!retVal)
            {
                Log.Error("Unable to initialize widget manager");
            }
            else
            {
                _rootWidget = _widgetManager.RootWidget;
                if (String.IsNullOrEmpty(_rootWidget.SubClass))
                {
                    _rootWidget.SubClass = (ScannerForm is ContextualMenuBase) ?
                                            PanelClasses.PanelCategory.ContextualMenu.ToString() :
                                            PanelClasses.PanelCategory.Scanner.ToString();
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
            ScannerForm.Invoke(new MethodInvoker(delegate
                {
                    Log.Debug("Calling close for " + ScannerForm.Name);
                    ScannerForm.Close();
                }));
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
        /// The keystate of one of the modifier keys (shift, alt,
        /// ctrl or function) changed.  Update the scanner status bar
        /// </summary>
        private void KeyStateTracker_EvtKeyStateChanged()
        {
            StatusBarController.UpdateStatusBar();
        }

        /// <summary>
        /// Raises an event to notify that the scanner was displayed
        /// </summary>
        private void notifyScannerShow()
        {
            if (ScannerForm.Visible && EvtScannerShow != null)
            {
                var arg = new ScannerShowEventArg(_scannerPanel);
                EvtScannerShow(_scannerPanel, arg);
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
            notifyScannerShow();
        }

        /// <summary>
        /// Get all the widgets whose "enabled" property is set to
        /// contextual in the config file.  Decide the "enabled" state
        /// of these widgets depending on the context
        /// </summary>
        /// <param name="monitorInfo">Context info about active window</param>
        private void SetWidgetEnabledStates(WindowActivityMonitorInfo monitorInfo)
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
                    ////Log.Debug("Widget: " + widget.Name + ", subclass: " + widget.SubClass);

                    if (!String.IsNullOrEmpty(widget.SubClass))
                    {
                        var arg = new CheckEnabledArgs(monitorInfo, widget);
                        if (!_syncLock.IsClosing())
                        {
                            _scannerPanel.CheckWidgetEnabled(arg);

                            if (!arg.Handled)
                            {
                                Context.AppAgentMgr.CheckWidgetEnabled(arg);
                            }

                            widget.Enabled = arg.Handled ? arg.Enabled : widget.DefaultEnabled;
                        }
                        else
                        {
                            break;
                        }

                        ////Log.Debug("widget.Enabled set to : " + widget.Enabled + " for feature " + widget.SubClass);
                    }
                }
            }
        }

        /// <summary>
        /// Subscribe to events from the talk window manager
        /// </summary>
        private void subscribeTalkWindowManager()
        {
            Context.AppTalkWindowManager.EvtTalkWindowCreated += AppTalkWindowManager_EvtTalkWindowCreated;
            Context.AppTalkWindowManager.EvtTalkWindowCleared += AppTalkWindowManager_EvtTalkWindowCleared;
            Context.AppTalkWindowManager.EvtTalkWindowClosed += AppTalkWindowManager_EvtTalkWindowClosed;
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

            GetAnimationManager().Interpreter.EvtCloseNotify += Interpreter_EvtCloseNotify;
            GetAnimationManager().Interpreter.EvtRun += Interpreter_EvtRun;

            subscribeToButtonEvents();
        }

        /// <summary>
        /// Convers the indicated text to speech thorugh the
        /// active text-to-speech engine
        /// </summary>
        /// <param name="text">text to convert</param>
        private void textToSpeech(String text)
        {
            Log.Debug("about to speak text=" + text);
            Context.AppTTSManager.ActiveEngine.Speak(text);
            AuditLog.Audit(new AuditEventTextToSpeech(Context.AppTTSManager.ActiveEngine.Descriptor.Name));
        }

        /// <summary>
        /// Unsubscribe from events previously subscribed to.
        /// </summary>
        private void unsubscribeEvents()
        {
            Context.AppAgentMgr.EvtTextChanged -= AppAgent_EvtTextChanged;

            Log.Debug("Form: " + ScannerForm.Name + ", UNSUBSCRIBE EVTSCANNERHITTEST");

            Context.AppAgentMgr.EvtScannerHitTest -= AppAgent_EvtScannerHitTest;
            Windows.EvtWindowPositionChanged -= Windows_EvtWindowPositionChanged;
            ScannerForm.Shown -= form_Shown;
            ScannerForm.VisibleChanged -= form_VisibleChanged;
            ScannerForm.SizeChanged -= ScannerForm_SizeChanged;

            Context.AppTalkWindowManager.EvtTalkWindowCreated -= AppTalkWindowManager_EvtTalkWindowCreated;

            GetAnimationManager().Interpreter.EvtCloseNotify -= Interpreter_EvtCloseNotify;
            GetAnimationManager().Interpreter.EvtRun -= Interpreter_EvtRun;

            KeyStateTracker.EvtKeyStateChanged -= KeyStateTracker_EvtKeyStateChanged;
        }

        /// <summary>
        /// Unsubscribes fom events
        /// </summary>
        private void unsubscribeTalkWindowManager()
        {
            Context.AppTalkWindowManager.EvtTalkWindowCreated -= AppTalkWindowManager_EvtTalkWindowCreated;
            Context.AppTalkWindowManager.EvtTalkWindowCleared -= AppTalkWindowManager_EvtTalkWindowCleared;
            Context.AppTalkWindowManager.EvtTalkWindowClosed -= AppTalkWindowManager_EvtTalkWindowClosed;
        }

        /// <summary>
        /// The user actuated a widget.  perform the necessary
        /// action
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtActuated(object sender, WidgetEventArgs e)
        {
            if (PreviewMode)
            {
                return;
            }

            var widget = e.SourceWidget;

            bool handled = false;
            _scannerPanel.OnWidgetActuated(e.SourceWidget, ref handled);

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
        /// Heartbeat handler from the windowactivitymonitor.  Use this
        /// monitor to set the 'enabled' state of the widgets in the scanner
        /// based on the context.
        /// </summary>
        /// <param name="monitorInfo">active window context info</param>
        private void WindowActivityMonitor_EvtWindowMonitorHeartbeat(WindowActivityMonitorInfo monitorInfo)
        {
            try
            {
                SetWidgetEnabledStates(monitorInfo);
            }
            catch
            {
            }
        }

        /// <summary>
        /// The window position of the scanner changed.  Set
        /// the talk window position relative to the scanner.
        /// </summary>
        /// <param name="form">Scanner form</param>
        /// <param name="position">current position</param>
        private void Windows_EvtWindowPositionChanged(Form form, Windows.WindowPosition position)
        {
            if (form == ScannerForm)
            {
                Context.AppWindowPosition = position;
                notifyScannerShow();
            }
        }
    }
}