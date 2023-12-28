////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Current state of the animation player
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// Not set
        /// </summary>
        Unknown,

        /// <summary>
        /// It's stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// Currently paused
        /// </summary>
        Paused,

        /// <summary>
        /// Currently running
        /// </summary>
        Running,

        /// <summary>
        /// Player timed out
        /// </summary>
        Timeout,

        /// <summary>
        /// User interrupted animation by clicking
        /// </summary>
        Interrupted
    }

    /// <summary>
    /// This is the heartbeat (literally) of ACAT.
    /// Handles UI animations. Plays out the animation sequence as laid out
    /// in the xml file.  Handles transitions between animations.  A timer
    /// is used to move the highlight from one widget to the other.  The
    /// calling app can start/stop/pause/resume the animation.
    /// </summary>
    internal class AnimationPlayer : IDisposable
    {
        /// <summary>
        /// Interpreter to interpret animation code
        /// </summary>
        private readonly Interpret _interpreter;

        /// <summary>
        /// For manual scanning, the path taken for scanning. This
        /// is a list of widgets that are highlighed in the current
        /// scan direction
        /// </summary>
        private readonly List<Widget> _manualScanPath = new List<Widget>();

        /// <summary>
        /// root widget of the scanner
        /// </summary>
        private readonly Widget _rootWidget;

        /// <summary>
        /// Used for synchronization through critical sections
        /// </summary>
        private readonly SyncLock _syncObj;

        /// <summary>
        /// Used the synchronization for animation transition
        /// </summary>
        private readonly object _transitionSync;

        /// <summary>
        /// List of variables and their values
        /// </summary>
        private readonly Variables _variables;

        /// <summary>
        /// The animation in the sequence that is currently active
        /// </summary>
        private Animation _currentAnimation;

        /// <summary>
        /// index of the currently highlighted widget
        /// </summary>
        private int _currentWidgetIndex;

        private bool _delayedSelect = false;

        private bool _delayedSelect2 = false;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The currenly highlighted widget
        /// </summary>
        private AnimationWidget _highlightedAnimationWidget;

        /// <summary>
        /// The currently highlighted widget
        /// </summary>
        private Widget _highlightedWidget;

        /// <summary>
        /// are we already in the timer function?
        /// </summary>
        private volatile bool _inTimer;

        /// <summary>
        /// number if animation iterations
        /// </summary>
        private int _iterationCount = 1;

        /// <summary>
        /// is this the last iteration in the animation sequence
        /// </summary>
        private bool _lastIteration;

        /// <summary>
        /// The scan mode (for manual scanning)
        /// </summary>
        private ManualScanModes _manualScanMode = ManualScanModes.None;

        /// <summary>
        /// Current state of the animation player
        /// </summary>
        private PlayerState _playerState;

        private ManualScanModes _prevManualScanMode = ManualScanModes.None;

        private System.Diagnostics.Stopwatch _stopwatch;

        /// <summary>
        /// Timer used for animation
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// Initializes the player
        /// </summary>
        /// <param name="rootWidget">root widget for the scanner</param>
        /// <param name="interpreter">the interpreter object</param>
        /// <param name="variables">variables and their values</param>
        public AnimationPlayer(Widget rootWidget, Interpret interpreter, Variables variables)
        {
            Log.Debug("CTOR(" + rootWidget.Name + ")");
            if (rootWidget.UIControl is IPanel)
            {
                _syncObj = ((IPanel)rootWidget.UIControl).SyncObj;
            }
            else if (rootWidget.UIControl is IUserControl)
            {
                _syncObj = ((IUserControl)rootWidget.UIControl).SyncObj;
            }

            _transitionSync = _syncObj;

            _interpreter = interpreter;
            _timer = new System.Timers.Timer();
            _timer.Elapsed += timer_Elapsed;
            _highlightedAnimationWidget = null;
            _currentAnimation = null;
            _rootWidget = rootWidget;
            _playerState = PlayerState.Stopped;
            _variables = variables;
            _inTimer = false;
            IsSwitchActive = false;

            _stopwatch = new System.Diagnostics.Stopwatch();
        }

        public delegate void PlayerAnimationTransition(object sender, String animationName, bool isTopLevel);

        /// <summary>
        /// Delegate for event raised when the player state chagnes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public delegate void PlayerStateChanged(object sender, PlayerStateChangedEventArgs e);

        public event PlayerAnimationTransition EvtPlayerAnimationTransition;

        /// <summary>
        /// Event raised when the player state chagnes
        /// </summary>
        public event PlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Direction of scanning for manual scan
        /// </summary>
        private enum ScanDirection
        {
            None,
            Horizontal,
            Vertical
        }

        /// <summary>
        /// Gets the current animation sequence object
        /// </summary>
        public Animation CurrentAnimation
        {
            get { return _currentAnimation; }
        }

        /// <summary>
        /// Gets the currently highlighted widget in the animation sequence
        /// </summary>
        public AnimationWidget HighlightedAnimationWidget
        {
            get { return _highlightedAnimationWidget; }
        }

        /// <summary>
        /// Returns the currently highlighted widget
        /// </summary>
        public Widget HighlightedWidget
        {
            get
            {
                if (!CoreGlobals.AppPreferences.EnableManualScan)
                {
                    return _highlightedAnimationWidget?.UIWidget;
                }

                return _highlightedWidget;
            }
        }

        public bool IsSwitchActive { get; set; }

        /// <summary>
        /// Gets the player state
        /// </summary>
        public PlayerState State
        {
            get { return _playerState; }
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
        /// Returns the animation widget that is the wrapper for the widget specified.
        /// </summary>
        /// <param name="widget">source widget</param>
        /// <returns>corresponding animation widget</returns>
        public AnimationWidget GetAnimationWidgetInCurrentAnimation(Widget widget)
        {
            AnimationWidget retVal = null;
            if (_currentAnimation != null)
            {
                retVal = _currentAnimation.GetAnimationWidget(widget);
            }

            return retVal;
        }

        /// <summary>
        /// For manual scanning, highlight the default home widget
        /// </summary>
        public void HighlightDefaultHome()
        {
            _rootWidget.HighlightOff();

            var defaultHome = _rootWidget.Finder.FindDefaultHome();

            if (defaultHome == null)
            {
                var buttonList = new List<Widget>();
                _rootWidget.Finder.FindAllButtons(buttonList);

                if (buttonList.Any())
                {
                    defaultHome = buttonList[0];
                }
            }

            if (defaultHome != null)
            {
                defaultHome.HighlightOn();
            }

            if (!_manualScanPath.Contains(defaultHome))
            {
                _manualScanPath.Add(defaultHome);
            }

            _highlightedWidget = defaultHome;
            _prevManualScanMode = ManualScanModes.None;
        }

        /// <summary>
        /// Interrupt animation
        /// </summary>
        public void Interrupt()
        {
            Log.Debug("Interrupt(" + _rootWidget.Name + ")");
            if (_timer != null &&
                (_playerState == PlayerState.Running ||
                _playerState == PlayerState.Timeout))
            {
                Log.Debug("Interrupt timer for panel " + _rootWidget.Name);

                _timer.Stop();
                setPlayerState(PlayerState.Interrupted);
                _rootWidget.HighlightOff();
            }
        }

        /// <summary>
        /// For manual scan, actuates the widget and either
        /// keeps it highlighted or goes back to default home
        /// </summary>
        /// <param name="widget">Widget to actuate</param>
        public void ManualScanActuateWidget(Widget widget)
        {
            if (widget != null)
            {
                widget.Actuate();

                AuditLog.Audit(new AuditEventManualScanExperiments(_prevManualScanMode.ToString(), _highlightedWidget.Panel.ToString(),
                   _highlightedWidget.Name, _highlightedWidget.Value, _highlightedWidget.Command, _stopwatch.ElapsedMilliseconds));

                AuditLog.Audit(new AuditEventManualScanExperiments("Actuate", widget.Panel.ToString(),
                      widget.Name, widget.Value, widget.Command, _stopwatch.ElapsedMilliseconds));

                _stopwatch.Stop();
            }

            if (CoreGlobals.AppPreferences.ManualScanHighlightDefaultHomePostActuate)
            {
                HighlightDefaultHome();
            }
            else if (widget != null)
            {
                widget.HighlightOn();
            }
        }

        /// <summary>
        /// Pause animation
        /// </summary>
        public void Pause()
        {
            Log.Debug("Pause(" + _rootWidget.Name + ")");

            if (_timer != null &&
                (_playerState == PlayerState.Running ||
                _playerState == PlayerState.Timeout ||
                _playerState == PlayerState.Interrupted))
            {
                Log.Debug("AP1: Stop timer for panel " + _rootWidget.Name);

                _timer.Stop();

                setPlayerState(PlayerState.Paused);
                _rootWidget.HighlightOff();
            }
        }

        /// <summary>
        /// Resumes paused player and transitions to the
        /// animation
        /// </summary>
        /// <param name="animation">transition to this animation</param>
        public void Resume(Animation animation = null)
        {
            Log.Debug("Resume(" + _rootWidget.Name + ") +, state: " + _playerState);

            if ((_playerState == PlayerState.Paused ||
                    _playerState == PlayerState.Timeout ||
                    _playerState == PlayerState.Interrupted) &&
                    _timer != null)
            {
                Log.Debug("Resume(" + _rootWidget.Name + ") Setting player state to running");
                setPlayerState(PlayerState.Running);

                if (!CoreGlobals.AppPreferences.EnableManualScan)
                {
                    if (animation != null)
                    {
                        Log.Debug("In Resume Calling Transition");
                        Transition(animation);
                    }
                }
                else
                {
                    TransitionManualScan();
                }
            }
        }

        /// <summary>
        /// Stops playing the animation sequence
        /// </summary>
        public void Stop()
        {
            if (_timer == null)
            {
                Log.Debug("_timer is null");
                return;
            }

            _timer.Elapsed -= timer_Elapsed;

            Log.Debug("Inside stopthread. before enter " + _rootWidget.UIControl.Name);

            tryEnterUntilSuccess(_transitionSync);

            Log.Debug("Inside stopthread. after enter " + _rootWidget.UIControl.Name);

            _timer.Stop();

            Log.Debug("Inside stopthread. before exit" + _rootWidget.UIControl.Name);
            Monitor.Exit(_transitionSync);
            Log.Debug("Inside stopthread. after exit" + _rootWidget.UIControl.Name);

            Log.Debug("Stop" + _rootWidget.UIControl.Name + ", syncobj.status " + _syncObj.Status + ", intimer: " + _inTimer);
            setPlayerState(PlayerState.Stopped);

            _timer.Dispose();
            _timer = null;
        }

        /// <summary>
        /// Transitions to the specified target animation. Stops the current
        /// animation and starts the new one.
        /// </summary>
        /// <param name="animation">Animation to transition to</param>
        public void Transition(Animation animation = null)
        {
            if (_syncObj == null)
            {
                Log.Debug("_syncObj is null. returning");
                return;
            }

            try
            {
                _manualScanMode = ManualScanModes.None;

                _lastIteration = false;

                if (_timer != null)
                {
                    _timer.Stop();
                }

                if (animation != null)
                {
                    Log.Debug("Transition to " + animation.Name);
                }

                setPlayerState(PlayerState.Stopped);

                Log.Debug("Transition : Before Enter " + _rootWidget.UIControl.Name + ", threadid: " + Kernel32Interop.GetCurrentThreadId());
                tryEnterUntilSuccess(_transitionSync);
                Log.Debug("Transition : After Enter " + _rootWidget.UIControl.Name + ", status:  " + _syncObj.Status);

                if (_syncObj.IsClosing())
                {
                    Log.Debug("FORM IS CLOSING. releasing _transitionSync and returning" + _rootWidget.UIControl.Name);
                    release(_transitionSync);
                    return;
                }

                _rootWidget.HighlightOff();

                if (_currentAnimation != null)
                {
                    _currentAnimation.Stop();
                }

                if (animation == null)
                {
                    _currentAnimation = null;
                    release(_transitionSync);
                    setPlayerState(PlayerState.Running);
                    return;
                }

                _currentAnimation = animation;
                _currentAnimation.ResolveUIWidgetsReferences(_rootWidget, _variables);
                _currentAnimation.OnEnterExecutionNotDone = true;

                _iterationCount = 0;
                _currentWidgetIndex = getFirstAnimatedWidget();
                _highlightedAnimationWidget = null;

                Log.Debug("Transition : Before Release " + _rootWidget.UIControl.Name);
                release(_transitionSync);
                Log.Debug("Transition : After Release " + _rootWidget.UIControl.Name);

                Log.Debug("Start new animation " + animation.Name);

                if (!animation.AutoStart && animation.OnStart)
                {
                    animation.OnStart = false;
                    setPlayerState(PlayerState.Timeout);
                }
                else if (!CoreGlobals.AppPreferences.TopLevelScanIncludeEmptyGrids && animatedWidgetCount() == 0)
                {
                    setPlayerState(PlayerState.Timeout);
                }
                else if (_timer != null)
                {/*
                    if (EvtPlayerAnimationTransition != null)
                    {
                        EvtPlayerAnimationTransition(this, (animation != null) ? animation.Name : String.Empty, (animation != null) ? animation.IsFirst : false);
                    }
                    */
                    _timer.Interval = _currentAnimation.SteppingTime;
                    Log.Debug(_rootWidget.UIControl.Name + ", syncobj.status " + _syncObj.Status);

                    if (_syncObj.Status == SyncLock.StatusValues.None)
                    {
                        timer_Elapsed(null, null);
                        Log.Debug("Starting timer " + _rootWidget.UIControl.Name);
                        _timer.Start();
                    }
                    else
                    {
                        Log.Debug("******** WILL NOT START TIMER!!!" +
                                    _rootWidget.UIControl.Name +
                                    ", syncobj.status " + _syncObj.Status);
                    }

                    setPlayerState(PlayerState.Running);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            Log.Debug("Returning");
        }

        /// <summary>
        /// For manual scanning, begins scanning in the specified mode
        /// </summary>
        /// <param name="manualScanMode">Scan mode</param>
        public void TransitionManualScan(ManualScanModes manualScanMode = ManualScanModes.None)
        {
            if (_syncObj == null)
            {
                Log.Debug("_syncObj is null. returning");
                return;
            }

            try
            {
                if (manualScanMode != ManualScanModes.None && _highlightedWidget != null)
                {
                    if (_prevManualScanMode == ManualScanModes.None)
                    {
                        // Save past iteration
                        AuditLog.Audit(new AuditEventManualScanExperiments("Init", _highlightedWidget.Panel.ToString(),
                        _highlightedWidget.Name, _highlightedWidget.Value, _highlightedWidget.Command, 0));

                        // Start stopwatch (will stop when actuates)
                        _stopwatch.Reset();
                        _stopwatch.Start();
                    }
                    else
                    {
                        // Save past iteration
                        AuditLog.Audit(new AuditEventManualScanExperiments(_prevManualScanMode.ToString(), _highlightedWidget.Panel.ToString(),
                            _highlightedWidget.Name, _highlightedWidget.Value, _highlightedWidget.Command, _stopwatch.ElapsedMilliseconds));
                    }
                }

                _prevManualScanMode = manualScanMode;

                _delayedSelect = false;
                _delayedSelect2 = false;

                Log.Debug("manualScanMode is " + manualScanMode);

                Log.Debug(_playerState + ", " + _manualScanMode);
                if (_playerState == PlayerState.Running)
                {
                    if (checkStopManualAutoScan(manualScanMode))
                    {
                        _timer.Stop();
                        setPlayerState(PlayerState.Stopped);
                        if (_highlightedWidget != null)
                        {
                            _delayedSelect = true;
                            _timer.Interval = CoreGlobals.AppPreferences.ManualScanPreActuatePauseTime;
                            _timer.Start();
                        }
                        return;
                    }
                }

                if (_timer != null)
                {
                    _timer.Stop();
                }

                setPlayerState(PlayerState.Stopped);

                Log.Debug("Transition : Before Enter " + _rootWidget.UIControl.Name + ", threadid: " + Kernel32Interop.GetCurrentThreadId());
                tryEnterUntilSuccess(_transitionSync);
                Log.Debug("Transition : After Enter " + _rootWidget.UIControl.Name + ", status:  " + _syncObj.Status);

                if (_syncObj.IsClosing())
                {
                    Log.Debug("FORM IS CLOSING. releasing _transitionSync and returning" + _rootWidget.UIControl.Name);
                    release(_transitionSync);
                    return;
                }

                if (_highlightedWidget == null)
                {
                    Log.Debug("_highlightedWidget is null");

                    HighlightDefaultHome();
                }
                else
                {
                    _highlightedWidget.HighlightOn();
                }

                if (manualScanMode == ManualScanModes.None)
                {
                    setPlayerState(PlayerState.Timeout);
                    release(_transitionSync);
                    return;
                }

                var currentScanDirection = getScanDirection(_manualScanMode);
                var scanDirection = getScanDirection(manualScanMode);

                if ((currentScanDirection == ScanDirection.Horizontal && scanDirection == ScanDirection.Vertical) ||
                    currentScanDirection == ScanDirection.Vertical && scanDirection == ScanDirection.Horizontal)
                {
                    _manualScanPath.Clear();
                    if (_highlightedWidget != null)
                    {
                        _manualScanPath.Add(_highlightedWidget);
                    }
                }

                _manualScanMode = manualScanMode;

                bool handled = handleSingleMove();

                Log.Debug("Transition : Before Release " + _rootWidget.UIControl.Name);

                release(_transitionSync);
                Log.Debug("Transition : After Release " + _rootWidget.UIControl.Name);

                if (handled)
                {
                    setPlayerState(PlayerState.Running);
                    return;
                }

                if (_timer != null)
                {
                    _timer.Interval = CoreGlobals.AppPreferences.ScanTime;
                    Log.Debug(_rootWidget.UIControl.Name + ", syncobj.status " + _syncObj.Status);

                    if (_syncObj.Status == SyncLock.StatusValues.None)
                    {
                        timer_Elapsed(null, null);
                        Log.Debug("Starting timer " + _rootWidget.UIControl.Name);
                        _timer.Start();
                    }
                    else
                    {
                        Log.Debug("******** WILL NOT START TIMER!!!" + _rootWidget.UIControl.Name + ", syncobj.status " + _syncObj.Status);
                    }

                    setPlayerState(PlayerState.Running);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            Log.Debug("Returning");
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
                    // dispose all managed resources.
                    //Stop();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        ///  Returns the count of number of animations.  Note that this is not
        ///  necessarily the total number of widget in the list as some can be disabled.
        /// </summary>
        /// <returns>count</returns>
        private int animatedWidgetCount()
        {
            try
            {
                return _currentAnimation.AnimationWidgetList.Count(t => t.UIWidget.CanAddForAnimation());
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                return 0;
            }
        }

        /// <summary>
        /// Checks if we need to exit because the scanner is closing
        /// and throws an exception
        /// </summary>
        private void check()
        {
            if (_syncObj != null && _syncObj.IsClosing())
            {
                Log.Debug("Scanner closed" + _rootWidget.UIControl.Name);
                throw new Exception();
            }
        }

        private bool checkStopManualAutoScan(ManualScanModes scanMode)
        {
            if (_manualScanMode == ManualScanModes.ScanLeft &&
                (scanMode == ManualScanModes.MoveRight || scanMode == ManualScanModes.ScanRight))
            {
                return true;
            }

            if (_manualScanMode == ManualScanModes.ScanRight &&
                (scanMode == ManualScanModes.MoveLeft || scanMode == ManualScanModes.ScanLeft))
            {
                return true;
            }

            if (_manualScanMode == ManualScanModes.ScanUp &&
                (scanMode == ManualScanModes.MoveDown || scanMode == ManualScanModes.ScanDown))
            {
                return true;
            }

            if (_manualScanMode == ManualScanModes.ScanDown &&
                (scanMode == ManualScanModes.MoveUp || scanMode == ManualScanModes.ScanUp))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Displays the names of the widgets in the manual scan path
        /// </summary>
        /// <param name="tag">Tag</param>
        private void dumpManualPath(String tag)
        {
            Log.Debug(tag + " ---------------------");
            if (_manualScanPath != null)
            {
                if (_manualScanPath.Count == 0)
                {
                    Log.Debug("None found");
                }
                else
                {
                    foreach (var h in _manualScanPath)
                    {
                        Log.Debug(h.Name);
                    }
                }
            }
            Log.Debug("---------------------");
        }

        /// <summary>
        /// Returns the first animation widget in the animation sequence.
        /// Note that this is not necessaritly the one with index=0 as the previous
        /// ones in the widget list could be disabled for animation.
        /// </summary>
        /// <returns>index of the first animation widget</returns>
        private int getFirstAnimatedWidget()
        {
            Log.Debug();
            for (int ii = 0; ii < _currentAnimation.AnimationWidgetList.Count; ii++)
            {
                if (_currentAnimation.AnimationWidgetList[ii].UIWidget.CanAddForAnimation())
                {
                    return ii;
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns the next animation widget in the sequence after index.
        /// </summary>
        /// <param name="index">where to start from</param>
        /// <returns>index of the next animation widget</returns>
        private int getNextAnimatedWidget(int index)
        {
            try
            {
                index++;

                while (true)
                {
                    if (index >= _currentAnimation.AnimationWidgetList.Count)
                    {
                        index = -1;
                        break;
                    }
                    if (_currentAnimation.AnimationWidgetList[index].UIWidget.CanAddForAnimation())
                    {
                        break;
                    }
                    index++;
                }

                return index;
            }
            catch (Exception ex)
            {
                Log.Debug("ex=" + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Returns the scan direction corresponding to the specified
        /// manual scan mode
        /// </summary>
        /// <param name="manualScanMode">the scan mode</param>
        /// <returns>scan direction</returns>
        private ScanDirection getScanDirection(ManualScanModes manualScanMode)
        {
            switch (manualScanMode)
            {
                case ManualScanModes.MoveDown:
                case ManualScanModes.ScanDown:
                case ManualScanModes.MoveUp:
                case ManualScanModes.ScanUp:
                    return ScanDirection.Vertical;

                case ManualScanModes.MoveLeft:
                case ManualScanModes.MoveRight:
                case ManualScanModes.ScanLeft:
                case ManualScanModes.ScanRight:
                    return ScanDirection.Horizontal;
            }

            return ScanDirection.None;
        }

        /// <summary>
        /// For manual scanning, gets the bottom-most widget along the
        /// manual scanning path.
        /// </summary>
        private Widget getWraparoundWidgetBottom()
        {
            Widget widget;
            if (_manualScanPath == null || _manualScanPath.Count == 0)
            {
                widget = _highlightedWidget;
            }
            else
            {
                widget = _manualScanPath[0];
            }

            var start = widget;
            while (widget != null)
            {
                if (widget.Below.Count == 0)
                {
                    return widget;
                }

                if (_manualScanPath != null)
                {
                    foreach (var w in widget.Below)
                    {
                        if (_manualScanPath.Contains(w))
                        {
                            if (w.Below.Count == 0)
                            {
                                return w;
                            }
                        }
                    }
                }

                widget = widget.Below[0];
                if (widget == start)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// For manual scanning, gets the left most widget in the
        /// manual scanning path.
        /// </summary>
        private Widget getWraparoundWidgetLeft()
        {
            Widget widget;
            if (_manualScanPath == null || _manualScanPath.Count == 0)
            {
                widget = _highlightedWidget;
            }
            else
            {
                widget = _manualScanPath[0];
            }

            //dumpManualPath("getWraparoundLeft");

            var start = widget;
            while (widget != null)
            {
                if (widget.Left.Count == 0)
                {
                    return widget;
                }

                if (_manualScanPath != null)
                {
                    foreach (var w in widget.Left)
                    {
                        if (_manualScanPath.Contains(w))
                        {
                            if (w.Left.Count == 0)
                            {
                                return w;
                            }
                        }
                    }
                }

                widget = widget.Left[0];
                if (widget == start)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// For manual scanning, gets the right most widget in the
        /// manual scanning path.
        /// </summary>
        private Widget getWraparoundWidgetRight()
        {
            Widget widget;
            if (_manualScanPath == null || _manualScanPath.Count == 0)
            {
                widget = _highlightedWidget;
            }
            else
            {
                widget = _manualScanPath[0];
            }

            var start = widget;
            while (widget != null)
            {
                if (widget.Right.Count == 0)
                {
                    return widget;
                }

                if (_manualScanPath != null)
                {
                    foreach (var w in widget.Right)
                    {
                        if (_manualScanPath.Contains(w))
                        {
                            if (w.Right.Count == 0)
                            {
                                return w;
                            }
                        }
                    }
                }

                widget = widget.Right[0];
                if (widget == start)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// For manual scanning, gets the top most widget in the
        /// manual scanning path.
        /// </summary>
        private Widget getWraparoundWidgetTop()
        {
            Widget widget;
            if (_manualScanPath == null || _manualScanPath.Count == 0)
            {
                widget = _highlightedWidget;
            }
            else
            {
                widget = _manualScanPath[0];
            }

            var start = widget;
            while (widget != null)
            {
                if (widget.Above.Count == 0)
                {
                    return widget;
                }

                if (_manualScanPath != null)
                {
                    foreach (var w in widget.Above)
                    {
                        if (_manualScanPath.Contains(w))
                        {
                            if (w.Above.Count == 0)
                            {
                                return w;
                            }
                        }
                    }
                }

                widget = widget.Above[0];
                if (widget == start)
                {
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Moves the highlight one step to the neighbor
        /// of the currently highlighted widget
        /// </summary>
        /// <returns>true if it did</returns>
        private bool handleSingleMove()
        {
            bool retVal = true;
            switch (_manualScanMode)
            {
                case ManualScanModes.MoveLeft:
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }

                    highlightNeighborLeft();

                    break;

                case ManualScanModes.MoveRight:
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }

                    highlightNeighborRight();
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }
                    break;

                case ManualScanModes.MoveUp:
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }

                    highlightNeighborAbove();
                    break;

                case ManualScanModes.MoveDown:
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }

                    highlightNeighborBelow();
                    break;

                default:
                    retVal = false;
                    break;
            }

            if (retVal)
            {
                if (_timer != null)
                {
                    _delayedSelect = true;
                    _timer.Interval = CoreGlobals.AppPreferences.ManualScanPreActuatePauseTime;
                    _timer.Start();
                }
            }

            return retVal;
        }

        /// <summary>
        /// For manual scanning, highlights the neighbor above currently highlighted widget
        /// </summary>
        private void highlightNeighborAbove()
        {
            Log.Debug();

            if (_highlightedWidget == null)
            {
                Log.Debug("_widgethighlighted is null");
                return;
            }

            _highlightedWidget.HighlightOff();

            Widget above = null;
            if (_highlightedWidget.Above.Count > 0)
            {
                foreach (var widget in _highlightedWidget.Above)
                {
                    if (_manualScanPath.Contains(widget))
                    {
                        above = widget;
                        break;
                    }
                }

                if (above == null)
                {
                    above = _highlightedWidget.Above[0];
                }

                Log.Debug("above: " + above.Name);
                above.HighlightOn();
                if (!_manualScanPath.Contains(above))
                {
                    _manualScanPath.Add(above);
                }

                _highlightedWidget = above;
            }
            else
            {
                Log.Debug("above is null. Will get wraparound");
                var bottomMost = getWraparoundWidgetBottom();
                if (bottomMost != null)
                {
                    Log.Debug("bottomMost is " + bottomMost.Name);

                    bottomMost.HighlightOn();
                    if (!_manualScanPath.Contains(bottomMost))
                    {
                        _manualScanPath.Add(bottomMost);
                    }

                    _highlightedWidget = bottomMost;
                }
            }
        }

        /// <summary>
        /// For manual scanning, highlights the neighbor below currently highlighted widget
        /// </summary>
        private void highlightNeighborBelow()
        {
            Log.Debug();

            if (_highlightedWidget == null)
            {
                Log.Debug("_widgethighlighted is null");
                return;
            }

            _highlightedWidget.HighlightOff();

            Widget below = null;
            if (_highlightedWidget.Below.Count > 0)
            {
                foreach (var widget in _highlightedWidget.Below)
                {
                    if (_manualScanPath.Contains(widget))
                    {
                        below = widget;
                        break;
                    }
                }

                if (below == null)
                {
                    below = _highlightedWidget.Below[0];
                }

                Log.Debug("below: " + below.Name);
                below.HighlightOn();
                if (!_manualScanPath.Contains(below))
                {
                    _manualScanPath.Add(below);
                }
                _highlightedWidget = below;
            }
            else
            {
                Log.Debug("Below is null. Will get wraparound");
                var topMost = getWraparoundWidgetTop();
                if (topMost != null)
                {
                    Log.Debug("topMost is " + topMost.Name);

                    topMost.HighlightOn();
                    if (!_manualScanPath.Contains(topMost))
                    {
                        _manualScanPath.Add(topMost);
                    }

                    _highlightedWidget = topMost;
                }
            }
        }

        /// <summary>
        /// For manual scanning, highlights the neighbor to the left
        /// of the currently highlighted widget
        /// </summary>
        private void highlightNeighborLeft()
        {
            Log.Debug();

            if (_highlightedWidget == null)
            {
                return;
            }

            _highlightedWidget.HighlightOff();

            Widget left = null;
            if (_highlightedWidget.Left.Count > 0)
            {
                foreach (var widget in _highlightedWidget.Left)
                {
                    if (_manualScanPath.Contains(widget))
                    {
                        left = widget;
                        break;
                    }
                }

                if (left == null)
                {
                    left = _highlightedWidget.Left[0];
                }

                Log.Debug("Left: " + left.Name);
                left.HighlightOn();
                if (!_manualScanPath.Contains(left))
                {
                    _manualScanPath.Add(left);
                }
                _highlightedWidget = left;
            }
            else
            {
                // reached the left edge of the form. Wrap around
                // to start scanning at the right edge
                Log.Debug("Left is null. Will get wraparound");
                var rightmost = getWraparoundWidgetRight();
                if (rightmost != null)
                {
                    Log.Debug("Leftmost is " + rightmost.Name);

                    rightmost.HighlightOn();
                    if (!_manualScanPath.Contains(rightmost))
                    {
                        _manualScanPath.Add(rightmost);
                    }

                    _highlightedWidget = rightmost;
                }
            }
        }

        /// <summary>
        /// For manual scanning, highlights the neighbor to the right
        /// of the currently highlighted widget
        /// </summary>
        private void highlightNeighborRight()
        {
            if (_highlightedWidget == null)
            {
                return;
            }

            _highlightedWidget.HighlightOff();

            Widget right = null;
            if (_highlightedWidget.Right.Count > 0)
            {
                foreach (var widget in _highlightedWidget.Right)
                {
                    if (_manualScanPath.Contains(widget))
                    {
                        right = widget;
                        break;
                    }
                }

                if (right == null)
                {
                    right = _highlightedWidget.Right[0];
                }

                Log.Debug("Right: " + right.Name);
                right.HighlightOn();
                if (!_manualScanPath.Contains(right))
                {
                    _manualScanPath.Add(right);
                }
                _highlightedWidget = right;
            }
            else
            {
                // reached the right edge of the form. Wrap around
                // to start scanning at the left edge
                var leftMost = getWraparoundWidgetLeft();
                if (leftMost != null)
                {
                    leftMost.HighlightOn();
                    if (!_manualScanPath.Contains(leftMost))
                    {
                        _manualScanPath.Add(leftMost);
                    }

                    _highlightedWidget = leftMost;
                }
            }
        }

        /// <summary>
        /// Checks if the animation widget at the specified widget
        /// is the first one in the sequence.  Note that this is
        /// not necessaritly the one with index=0 as the previous
        /// ones in the widget list could be disabled for animation.
        /// </summary>
        /// <param name="index">Last index in the array</param>
        /// <returns></returns>
        private bool isFirstAnimatedWidget(int index)
        {
            for (int ii = 0; ii < index; ii++)
            {
                if (_currentAnimation.AnimationWidgetList[index].UIWidget.CanAddForAnimation())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Exits the critical section
        /// </summary>
        /// <param name="syncObj">the synchronization object</param>
        private void release(Object syncObj)
        {
            try
            {
                //Log.Debug("Before EXIT for " + _rootWidget.UIControl.Name);
                Monitor.Exit(syncObj);
                //Log.Debug("After EXIT for " + _rootWidget.UIControl.Name);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Sets the state of the player and triggers event
        /// that the state changed
        /// </summary>
        /// <param name="playerState">new state</param>
        private void setPlayerState(PlayerState playerState)
        {
            Log.Debug();

            PlayerState oldState = _playerState;
            if (oldState != playerState)
            {
                Log.Debug(_rootWidget.Name + ":Set player state to " + playerState);
                _playerState = playerState;
                if (EvtPlayerStateChanged != null)
                {
                    Log.Debug("Calling evtPlayerStateChanged");
                    EvtPlayerStateChanged.BeginInvoke(this, new PlayerStateChangedEventArgs(oldState, _playerState), null, null);
                }
            }
        }

        /// <summary>
        /// This function is the heart beat of ACAT.  All animations are handled in this
        /// event.  It unhighlights the currenlty highlighted animation widget and
        /// highlights the next one in the sequence.  If it has reached the end of the
        /// sequence, it wraps around and repeats the animation until the number of
        /// iterations has reached.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!CoreGlobals.AppPreferences.EnableManualScan)
            {
                timerElapsedAutoScan(sender, e);
            }
            else
            {
                timerElapsedManual(sender, e);
            }
        }

        /// <summary>
        /// This function is the heart beat of ACAT.  All animations are handled in this
        /// event.  It unhighlights the currenlty highlighted animation widget and
        /// highlights the next one in the sequence.  If it has reached the end of the
        /// sequence, it wraps around and repeats the animation until the number of
        /// iterations has reached.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void timerElapsedAutoScan(object sender, ElapsedEventArgs e)
        {
            if (_syncObj.IsClosing())
            {
                Log.Debug("Form is closing. Returning" + _rootWidget.UIControl.Name);
                return;
            }

            if (_inTimer)
            {
                Log.Debug("Timer is busy. returning");
                return;
            }

            _inTimer = true;

            try
            {
                Log.Debug("Before tryEnter " + _rootWidget.UIControl.Name + ", threadid: " + Kernel32Interop.GetCurrentThreadId());

                if (!tryEnter(_transitionSync))
                {
                    Log.Debug("_transition sync will block returning");
                    return;
                }

                Log.Debug("After tryEnter" + _rootWidget.UIControl.Name + ", status: " + _syncObj.Status);
                if (_syncObj.IsClosing())
                {
                    Log.Debug("Form is closing. Returning" + _rootWidget.UIControl.Name);
                    return;
                }

                check();

                Log.Debug("CurrentAnimation: " + _currentAnimation.Name +
                            ". Count: " + _currentAnimation.AnimationWidgetList.Count +
                            ". currentWidgetIndex: " + _currentWidgetIndex);

                check();

                var animationWidget = _currentAnimation.AnimationWidgetList[_currentWidgetIndex];

                Log.Debug(_rootWidget.UIControl.Name + ", status: " + _syncObj.Status);

                // if any switch is currently engaged, keep the current widget
                // highlighted until the user releases the switch
                //if (ActuatorManager.Instance.IsSwitchActive())
                if (IsSwitchActive)
                {
                    Log.Debug("Some switch is active. Will try again");
                    return;
                }

                // if there is code associated with an onEnter, execute that
                if (_currentAnimation.OnEnterExecutionNotDone && _currentAnimation.OnEnter != null)
                {
                    _interpreter.Execute(_currentAnimation.OnEnter);
                    _currentAnimation.OnEnterExecutionNotDone = false;
                }

                check();

                Log.Debug(_rootWidget.UIControl.Name + ", status: " + _syncObj.Status);

                if (!_currentAnimation.IsFirst && animatedWidgetCount() == 0)
                {
                    _lastIteration = true;
                }

                // we have reached the end of the iteration. Turn off
                // the widget that was last highlighed and stop the
                // animation sequence
                if (_lastIteration)
                {
                    _lastIteration = false;

                    Widget selectedWidget = (_highlightedAnimationWidget != null &&
                                                _highlightedAnimationWidget.UIWidget != null) ? _highlightedAnimationWidget.UIWidget.Parent : null;

                    _rootWidget.HighlightOff();
                    if (animationWidget.OnHighlightOff.HasCode())
                    {
                        _interpreter.Execute(animationWidget.OnHighlightOff);
                    }

                    if (_currentAnimation.IsFirst)
                    {
                        setPlayerState(PlayerState.Timeout);
                    }

                    if (_timer != null)
                    {
                        _timer.Stop();
                    }
                    else
                    {
                        Log.Debug("Timer is null. returning");
                        return;
                    }

                    check();

                    AuditLog.Audit(new AuditEventAnimationEnd(
                                        _rootWidget.Name,
                                        (selectedWidget != null) ? selectedWidget.Name : string.Empty,
                                        (selectedWidget != null) ? selectedWidget.GetType().Name : string.Empty,
                                        _currentAnimation.Name));

                    // is there an onEnd code that we have to execute
                    _interpreter.Execute(_currentAnimation.OnEnd);

                    return;
                }

                check();

                if (_highlightedAnimationWidget != null && _highlightedAnimationWidget != animationWidget)
                {
                    Log.Debug(string.Format("Animation: {0}. Turning off . name = {1}. Count: {2}",
                                _currentAnimation.Name,
                                _highlightedAnimationWidget.UIWidget.Name,
                                _currentAnimation.AnimationWidgetList.Count));

                    check();

                    _highlightedAnimationWidget.UIWidget.HighlightOff();

                    check();

                    if (_highlightedAnimationWidget.OnHighlightOff.HasCode())
                    {
                        _interpreter.Execute(_highlightedAnimationWidget.OnHighlightOff);
                    }
                }

                check();

                // now turn the highlight on on the next widget in the  sequence
                animationWidget = _currentAnimation.AnimationWidgetList[_currentWidgetIndex];

                Log.Debug("Animation: " + _currentAnimation.Name +
                            ". Turning on " + _currentWidgetIndex +
                            ". name = " + animationWidget.UIWidget.Name);

                check();

                animationWidget.UIWidget.HighlightOn();

                _highlightedAnimationWidget = animationWidget;

                // calculate how long to wait before triggering the
                // next timer event.  (this is how long the highlighted
                // widget will stay highlighted)

                int hesitateTime = animationWidget.HesitateTime;

                check();

                if (hesitateTime == 0 && isFirstAnimatedWidget(_currentWidgetIndex))
                {
                    hesitateTime = _currentAnimation.HesitateTime;
                }

                if (_timer != null)
                {
                    _timer.Interval = _currentAnimation.SteppingTime + hesitateTime;
                }
                else
                {
                    Log.Debug("timer is null. returning");
                    return;
                }

                check();

                if (animationWidget.OnHighlightOn.HasCode())
                {
                    _interpreter.Execute(animationWidget.OnHighlightOn);
                }

                check();

                int nextIndex = getNextAnimatedWidget(_currentWidgetIndex);

                // if we have reached the end of the animation sequence, wrap around
                if (nextIndex < 0)
                {
                    _iterationCount++;
                    int iterations = CoreGlobals.AppPreferences.ResolveVariableInt(_currentAnimation.Iterations, 1, 1);
                    if (iterations >= 0 && _iterationCount >= iterations)
                    {
                        _lastIteration = true;
                        return;
                    }

                    _currentWidgetIndex = getFirstAnimatedWidget();
                }
                else
                {
                    _currentWidgetIndex = nextIndex;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("AnimationPlayerexception " + ex);
            }
            finally
            {
                Log.Debug("Before release " + _rootWidget.UIControl.Name);
                release(_transitionSync);
                Log.Debug("After release " + _rootWidget.UIControl.Name);

                Log.Debug("Setting intimer to false " + _rootWidget.UIControl.Name);
                _inTimer = false;

                Log.Debug("Exiting timer " + _rootWidget.UIControl.Name);
            }
        }

        /// <summary>
        /// For manual scanning, depending on the scan mode, highlights the next
        /// widget in the sequence (vertical or horizontal)
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void timerElapsedManual(object sender, ElapsedEventArgs e)
        {
            bool actuate = false;

            Log.Debug("------------->>> ENTER ");

            if (_syncObj.IsClosing())
            {
                Log.Debug("Form is closing. Returning" + _rootWidget.UIControl.Name);
                return;
            }

            if (_inTimer)
            {
                Log.Debug("Timer is busy. returning");
                return;
            }

            if (_manualScanMode == ManualScanModes.None)
            {
                return;
            }

            _inTimer = true;

            try
            {
                Log.Debug("Before tryEnter " + _rootWidget.UIControl.Name + ", threadid: " + Kernel32Interop.GetCurrentThreadId());

                if (!tryEnter(_transitionSync))
                {
                    Log.Debug("_transition sync will block returning");
                    return;
                }

                Log.Debug("After tryEnter" + _rootWidget.UIControl.Name + ", status: " + _syncObj.Status);
                if (_syncObj.IsClosing())
                {
                    Log.Debug("Form is closing. Returning" + _rootWidget.UIControl.Name);
                    return;
                }

                check();

                Log.Debug(_rootWidget.UIControl.Name + ", status: " + _syncObj.Status);

                // if any switch is currently engaged, keep the current widget
                // highlighted until the user releases the switch
                //if (ActuatorManager.Instance.IsSwitchActive())
                if (IsSwitchActive)
                {
                    Log.Debug("Some switch is active. Will try again");
                    return;
                }

                if (_highlightedWidget == null)
                {
                    return;
                }

                if (CoreGlobals.AppPreferences.ManualScanDelayedActuateEnable)
                {
                    if (_delayedSelect)
                    {
                        _delayedSelect = false;
                        _delayedSelect2 = true;
                        _timer.Interval = _timer.Interval = CoreGlobals.AppPreferences.ManualScanActuatePauseTime;
                        _highlightedWidget.SelectedHighlightOn2();
                        return;
                    }

                    if (_delayedSelect2)
                    {
                        _delayedSelect = false;
                        _delayedSelect2 = false;
                        _timer.Stop();
                        actuate = true;
                        return;
                    }
                }

                Log.Debug("_manualScanMode is " + _manualScanMode);

                var prevWidgetHighlighted = _highlightedWidget;

                switch (_manualScanMode)
                {
                    case ManualScanModes.ScanLeft:
                        highlightNeighborLeft();
                        break;

                    case ManualScanModes.ScanRight:
                        highlightNeighborRight();
                        break;

                    case ManualScanModes.ScanUp:
                        highlightNeighborAbove();
                        break;

                    case ManualScanModes.ScanDown:
                        highlightNeighborBelow();
                        break;
                }

                if (prevWidgetHighlighted == _highlightedWidget)
                {
                    Log.Debug("Same widget. Stopping timer");
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }
                }
                check();
            }
            catch (Exception ex)
            {
                Log.Debug("AnimationPlayerexception " + ex);
            }
            finally
            {
                Log.Debug("Before release " + _rootWidget.UIControl.Name);
                release(_transitionSync);
                Log.Debug("After release " + _rootWidget.UIControl.Name);

                Log.Debug("Setting intimer to false " + _rootWidget.UIControl.Name);
                _inTimer = false;

                if (actuate)
                {
                    ManualScanActuateWidget(_highlightedWidget);
                }
                Log.Debug("-----------  <<<< Exiting timer " + _rootWidget.UIControl.Name);
            }
        }

        /// <summary>
        /// Tries to enter a critical section
        /// </summary>
        /// <param name="syncObj">Object used to synchronoze</param>
        /// <returns>true if entered successfully</returns>
        private bool tryEnter(Object syncObj)
        {
            bool lockTaken = false;
            Monitor.TryEnter(syncObj, ref lockTaken);
            return lockTaken;
        }

        private void tryEnterUntilSuccess(object syncObj)
        {
            while (!tryEnter(syncObj))
            {
                Log.Debug("CALLING DOEVENTS");
                if (Application.MessageLoop)
                {
                    Application.DoEvents();
                }
            }
        }
    }
}