////////////////////////////////////////////////////////////////////////////
// <copyright file="AnimationMouseClickEventArgs.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The currenly highlighted widget
        /// </summary>
        private AnimationWidget _highlightedWidget;

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
        /// Current state of the animation player
        /// </summary>
        private PlayerState _playerState;

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
                _syncObj = (rootWidget.UIControl as IPanel).SyncObj;
            }

            _transitionSync = _syncObj;

            _interpreter = interpreter;
            _timer = new System.Timers.Timer();
            _timer.Elapsed += timer_Elapsed;
            _highlightedWidget = null;
            _currentAnimation = null;
            _rootWidget = rootWidget;
            _playerState = PlayerState.Stopped;
            _variables = variables;
            _inTimer = false;
        }

        /// <summary>
        /// Delegate for event raised when the player state chagnes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public delegate void PlayerStateChanged(object sender, PlayerStateChangedEventArgs e);

        /// <summary>
        /// Event raised when the player state chagnes
        /// </summary>
        public event PlayerStateChanged EvtPlayerStateChanged;

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
        public AnimationWidget HighlightedWidget
        {
            get { return _highlightedWidget; }
        }

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
        /// Interrupt animation
        /// </summary>
        public void Interrupt()
        {
            Log.Debug("Interrupt(" + _rootWidget.Name + ")");
            if (_timer != null &&
                (_playerState == PlayerState.Running ||
                _playerState == PlayerState.Timeout))
            {
                Log.Debug("Interrupt timer for screen " + _rootWidget.Name);

                _timer.Stop();
                setPlayerState(PlayerState.Interrupted);
                _rootWidget.HighlightOff();
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
                Log.Debug("Stop timer for screen " + _rootWidget.Name);

                _timer.Stop();
                setPlayerState(PlayerState.Paused);
                _rootWidget.HighlightOff();
            }
        }

        /// <summary>
        /// Resume paused player and transitions to the
        /// animation
        /// </summary>
        /// <param name="animation">transition to this animation</param>
        public void Resume(Animation animation)
        {
            Log.Debug("Resume(" + _rootWidget.Name + ") +, state: " + _playerState);

            if ((_playerState == PlayerState.Paused ||
                    _playerState == PlayerState.Timeout ||
                    _playerState == PlayerState.Interrupted) &&
                    _timer != null)
            {
                setPlayerState(PlayerState.Running);
                Log.Debug("In ResumeCalling player transition");
                Transition(animation);
            }
        }

        /// <summary>
        /// Stop playing the animation sequence
        /// </summary>
        public void Stop()
        {
            if (_timer == null)
            {
                Log.Debug("_timer is null");
                return;
            }

            Log.Debug("Inside stopthread. before enter " + _rootWidget.UIControl.Name);

            while (!tryEnter(_transitionSync))
            {
                Log.Debug("CALLING DOEVENTS");
                if (Application.MessageLoop)
                {
                    Application.DoEvents();
                }
            }

            Log.Debug("Inside stopthread. after enter " + _rootWidget.UIControl.Name);
            _timer.Elapsed -= timer_Elapsed;
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
        /// Transition to the specified target animation. Stops the current
        /// animation and starts the new one.
        /// </summary>
        /// <param name="animation">Animation to transition to</param>
        public void Transition(Animation animation)
        {
            if (_syncObj == null)
            {
                Log.Debug("_syncObj is null. returning");
                return;
            }

            try
            {
                _lastIteration = false;

                _timer.Stop();

                Log.Debug("Transition to " + animation.Name);
                setPlayerState(PlayerState.Stopped);

                Log.Debug("Transition : Before Enter " + _rootWidget.UIControl.Name + ", threadid: " + GetCurrentThreadId());
                enter(_transitionSync);
                Log.Debug("Transition : After Enter " + _rootWidget.UIControl.Name + ", status:  " + _syncObj.Status);

                if (_syncObj.Status == SyncLock.StatusValues.Closing || _syncObj.Status == SyncLock.StatusValues.Closed)
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

                _currentAnimation = animation;
                _currentAnimation.ResolveUIWidgetsReferences(_rootWidget, _variables);
                _currentAnimation.OnEnterExecutionNotDone = true;

                _iterationCount = 0;
                _currentWidgetIndex = getFirstAnimatedWidget();
                _highlightedWidget = null;

                Log.Debug("Transition : Before Release" + _rootWidget.UIControl.Name);
                release(_transitionSync);
                Log.Debug("Transition : After Release" + _rootWidget.UIControl.Name);

                Log.Debug("Start new animation " + animation.Name);

                if (!animation.AutoStart && animation.OnStart)
                {
                    animation.OnStart = false;
                    setPlayerState(PlayerState.Timeout);
                }
                else if (_timer != null)
                {
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

        [DllImport("kernel32.dll")]
        static private extern uint GetCurrentThreadId();

        /// <summary>
        ///  Returns the count of number of animations.  Note that this is not
        ///  necessarily the total number of widget in the list as some can be disabled.
        /// </summary>
        /// <returns>count</returns>
        private int animatedWidgetCount()
        {
            try
            {
                //Log.Debug("animationName: " + _currentAnimation.Name + ",  widgetlistCount: " + _currentAnimation.AnimationWidgetList.Count);

                //Log.Debug("active widget count: " + count);
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

        /// <summary>
        /// Enters a critical section
        /// </summary>
        /// <param name="syncObj">Object used to synchronoze</param>
        private void enter(Object syncObj)
        {
            //Log.Debug("Before ENTER for " + _rootWidget.UIControl.Name);
            Monitor.Enter(syncObj);
            //Log.Debug("After ENTER for " + _rootWidget.UIControl.Name);
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
            if (_syncObj.Status == SyncLock.StatusValues.Closing ||
                _syncObj.Status == SyncLock.StatusValues.Closed)
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
                Log.Debug("Before tryEnter " + _rootWidget.UIControl.Name + ", threadid: " + GetCurrentThreadId());

                if (!tryEnter(_transitionSync))
                {
                    Log.Debug("_transition sync will block returning");
                    return;
                }

                Log.Debug("After tryEnter" + _rootWidget.UIControl.Name + ", status: " + _syncObj.Status);
                if (_syncObj.Status == SyncLock.StatusValues.Closing ||
                    _syncObj.Status == SyncLock.StatusValues.Closed)
                {
                    Log.Debug("Form is closing. Returning" + _rootWidget.UIControl.Name);
                    return;
                }

                check();

                Log.Debug("CurrentAnimation: " + _currentAnimation.Name +
                            ". Count: " + _currentAnimation.AnimationWidgetList.Count +
                            ". currentWidgetIndex: " + _currentWidgetIndex);

                if (animatedWidgetCount() == 0)
                {
                    Log.Debug("No widgets to animate.");
                    _interpreter.Execute(_currentAnimation.OnEnd);
                    return;
                }

                check();

                var animationWidget = _currentAnimation.AnimationWidgetList[_currentWidgetIndex];

                Log.Debug(_rootWidget.UIControl.Name + ", status: " + _syncObj.Status);

                // if any switch is currently engaged, keep the current widget
                // highlighted until the user releases the switch
                if (ActuatorManager.Instance.IsSwitchActive())
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

                // we have reached the end of the iteration. Turn off
                // the widget that was last highlighed and stop the
                // animation sequence
                if (_lastIteration)
                {
                    _lastIteration = false;

                    Widget selectedWidget = (_highlightedWidget != null &&
                                                _highlightedWidget.UIWidget != null) ? _highlightedWidget.UIWidget.Parent : null;

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

                if (_highlightedWidget != null && _highlightedWidget != animationWidget)
                {
                    Log.Debug(string.Format("Animation: {0}. Turning off . name = {1}. Count: {2}",
                                _currentAnimation.Name,
                                _highlightedWidget.UIWidget.Name,
                                _currentAnimation.AnimationWidgetList.Count));

                    check();

                    _highlightedWidget.UIWidget.HighlightOff();

                    check();

                    if (_highlightedWidget.OnHighlightOff.HasCode())
                    {
                        _interpreter.Execute(_highlightedWidget.OnHighlightOff);
                    }

                    _highlightedWidget = null;
                }

                check();

                // now turn the highlight on on the next widget in the  sequence
                animationWidget = _currentAnimation.AnimationWidgetList[_currentWidgetIndex];

                Log.Debug("Animation: " + _currentAnimation.Name +
                            ". Turning on " + _currentWidgetIndex +
                            ". name = " + animationWidget.UIWidget.Name);

                check();

                animationWidget.UIWidget.HighlightOn();

                _highlightedWidget = animationWidget;

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
    }
}