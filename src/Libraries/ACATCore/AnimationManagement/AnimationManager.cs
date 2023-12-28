////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.CommandManagement;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Media;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Enmeration of the different modes for manual scanning
    /// </summary>
    public enum ManualScanModes
    {
        /// <summary>
        /// Undefined
        /// </summary>
        None,

        /// <summary>
        /// Scan horizontal in the left direction
        /// </summary>
        ScanLeft,

        /// <summary>
        /// Scan horizontal in the right direction
        /// </summary>
        ScanRight,

        /// <summary>
        /// Scan vertical in the upward direction
        /// </summary>
        ScanUp,

        /// <summary>
        /// Scan vertical in the downward direction
        /// </summary>
        ScanDown,

        /// <summary>
        /// Move scan one widget to the left
        /// </summary>
        MoveLeft,

        /// <summary>
        /// Move scan one widget to the right
        /// </summary>
        MoveRight,

        /// <summary>
        /// Move scan one widget above
        /// </summary>
        MoveUp,

        /// <summary>
        /// Move scan one widget down
        /// </summary>
        MoveDown,

        /// <summary>
        /// Stop scanning
        /// </summary>
        Stop,

        /// <summary>
        /// Pause scanning
        /// </summary>
        Pause,

        /// <summary>
        /// Resume scanning
        /// </summary>
        Resume,

        /// <summary>
        /// Toggle between Pause and Resume
        /// </summary>
        TogglePause
    }

    /// <summary>
    /// Manages the display states of the various widgets, starts and stops
    /// animations and and also handles transitions between animations.
    /// </summary>
    public class AnimationManager : IDisposable
    {
        /// <summary>
        /// Collection of animations for this panel
        /// </summary>
        private readonly AnimationsCollection _animationsCollection;

        /// <summary>
        /// Interpret script
        /// </summary>
        private readonly Interpret _interpreter;

        /// <summary>
        /// Stores transient variables for animation
        /// </summary>
        private readonly Variables _variables;

        /// <summary>
        /// The panel to which this Animation Manager belongs
        /// </summary>
        private Widget _currentPanel;

        /// <summary>
        /// has this object been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Points to the first animation in the sequence
        /// </summary>
        private Animation _firstAnimation;

        /// <summary>
        /// Panel class this scanner represents
        /// </summary>
        private String _panelClass = String.Empty;

        private PanelConfigMapEntry _panelConfigMapEntry;

        /// <summary>
        /// The animation player that actually plays the animation
        /// </summary>
        private AnimationPlayer _player;

        /// <summary>
        /// Plays beeps
        /// </summary>
        private SoundPlayer _soundPlayer;

        /// <summary>
        /// Animation that was in progress when a swtich-accept event was
        /// received
        /// </summary>
        private Animation _switchAcceptedAnimation;

        /// <summary>
        /// Highlighted widget when a switch accept event is recrived
        /// </summary>
        private AnimationWidget _switchAcceptedHighlightedWidget;

        /// <summary>
        /// Animation that was in progress when a switch-down was received
        /// </summary>
        private Animation _switchDownAnimation;

        /// <summary>
        /// Highlighted widget when a switchdown event is received
        /// </summary>
        private AnimationWidget _switchDownHighlightedWidget;

        /// <summary>
        /// Initializes the AnimationManager class
        /// </summary>
        public AnimationManager()
        {
            _interpreter = new Interpret();
            _animationsCollection = new AnimationsCollection();
            _soundPlayer = null;
            _currentPanel = null;
            _player = null;
            IsSwitchActive = false;
            _variables = new Variables();
            resetSwitchEventStates();
        }

        /// <summary>
        /// Delegate for the event raised when the player state changes
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event args</param>
        public delegate void PlayerStateChanged(object sender, PlayerStateChangedEventArgs e);

        /// <summary>
        /// Delegate for the event raised to resolve widget references
        /// in the animation sequence
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">arg</param>
        public delegate void ResolveWidgetChildren(object sender, ResolveWidgetChildrenEventArgs e);

        /// <summary>
        /// Raised when the animation player state changes
        /// </summary>
        public event PlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Raised to resolve widget references
        /// in the animation sequence
        /// </summary>
        public event ResolveWidgetChildren EvtResolveWidgetChildren;

        /// <summary>
        /// Get interpreter object used by the animation manager
        /// </summary>
        public Interpret Interpreter
        {
            get { return _interpreter; }
        }

        /// <summary>
        /// Gets/sets whether an actuator switch is currently active
        /// </summary>
        public bool IsSwitchActive { get; set; }

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
        /// Returns player state
        /// </summary>
        /// <returns></returns>
        public PlayerState GetPlayerState()
        {
            return (_player != null) ? _player.State : PlayerState.Unknown;
        }

        public void HighlightDefaultHome()
        {
            if (CoreGlobals.AppPreferences.EnableManualScan)
            {
                _player.HighlightDefaultHome();
            }
        }

        /// <summary>
        /// Allcoate resources, parse the config file which contains all the
        /// animations and create a list of animation objects. Subscribe to
        /// events. The parameter panelConfigMapEntry contains all the info about
        /// the current scanner
        /// </summary>
        /// <param name="panelConfigMapEntry">Config object for the panel</param>
        /// <returns>true on success</returns>
        public bool Init(PanelConfigMapEntry panelConfigMapEntry, Widget panelWidget = null)
        {
            _panelConfigMapEntry = panelConfigMapEntry;

            _currentPanel = panelWidget;

            _panelClass = panelConfigMapEntry.PanelClass;

            bool retVal = _animationsCollection.Load(panelConfigMapEntry.ConfigFileName);
            if (retVal)
            {
                retVal = _interpreter.LoadScripts(panelConfigMapEntry.ConfigFileName);
            }

            if (retVal)
            {
                subscribeToInterpreterEvents();

                subscribeToActuatorEvents();
            }

            Log.Debug("returning from Anim manager init()");

            return retVal;
        }

        /// <summary>
        /// Interrupt the animation sequence
        /// </summary>
        public void Interrupt()
        {
            if (_player != null)
            {
                _player.Interrupt();
            }
        }

        /// <summary>
        /// Pause animcation
        /// </summary>
        public void Pause()
        {
            if (_player != null)
            {
                _player.Pause();
            }
        }

        /// <summary>
        /// Resolves variable references to their actual values.  Variables
        /// start with an @ symbol.
        /// </summary>
        /// <param name="args">args to resolve</param>
        /// <returns>The arglist with variables resolved</returns>
        public List<String> ResolveArgs(List<String> args)
        {
            var argList = new List<String>();

            foreach (String arg in args)
            {
                switch (arg)
                {
                    case "@SelectedWidget":
                        addArg(argList, Variables.SelectedWidget, arg);
                        break;

                    case "@SelectedBox":
                        addArg(argList, Variables.SelectedBox, arg);
                        break;

                    case "@SelectedRow":
                        addArg(argList, Variables.SelectedRow, arg);
                        break;

                    case "@SelectedButton":
                        addArg(argList, Variables.SelectedButton, arg);
                        break;

                    default:
                        argList.Add(arg);
                        break;
                }
            }

            return argList;
        }

        /// <summary>
        /// Converts "true" "false" to a bool
        /// </summary>
        /// <param name="arg">"true" or "false"</param>
        /// <returns>translated value</returns>
        public bool ResolveBool(String arg)
        {
            return String.Compare(arg, "true", true) == 0;
        }

        /// <summary>
        /// Transitions to the starting sequence
        /// </summary>
        public void Restart()
        {
            if (_firstAnimation != null)
            {
                Transition(_firstAnimation);
            }
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void Resume()
        {
            if (_player != null)
            {
                if (CoreGlobals.AppPreferences.EnableAutoStartScan)
                {
                    _firstAnimation.OnStart = true;
                    _player.Resume(_firstAnimation);
                }
                else
                {
                    _player.Resume();
                }
            }
        }

        /// <summary>
        /// Marks the specified widget as the currently selected one
        /// by setting the @SelectedWidget variable
        /// </summary>
        /// <param name="widgetName">Name of the widget</param>
        public void SetSelectedWidget(String widgetName)
        {
            Widget selectedWidget = _currentPanel.Finder.FindChild(widgetName);
            if (selectedWidget != null)
            {
                SetSelectedWidget(selectedWidget);
            }
        }

        /// <summary>
        /// Marks the specified widget as the currently selected one
        /// by setting the @SelectedWidget variable
        /// </summary>
        /// <param name="selectedWidget">the widget object</param>
        public void SetSelectedWidget(Widget selectedWidget)
        {
            _variables.Set(Variables.SelectedWidget, selectedWidget);
            Widget widget = selectedWidget;

            _variables.Clear(Variables.SelectedBox);
            _variables.Clear(Variables.SelectedRow);
            _variables.Clear(Variables.SelectedButton);

            while (widget != null)
            {
                if (widget is IBoxWidget)
                {
                    _variables.Set(Variables.SelectedBox, widget);
                }
                else if (widget is IRowWidget)
                {
                    _variables.Set(Variables.SelectedRow, widget);
                }
                else if (widget is IButtonWidget)
                {
                    _variables.Set(Variables.SelectedButton, widget);
                }

                widget = widget.Parent;
            }
        }

        /// <summary>
        /// Starts the animation sequence for the specified panel. It starts
        /// with the animation that has the 'start' attribute set to true in
        /// the xml file
        /// </summary>
        /// <param name="panelWidget">Which panel to start the animations for?</param>
        /// <param name="animationName">Name of the animation sequence</param>
        public void Start(Widget panelWidget, String animationName = null)
        {
            Log.Debug("Start animation for panel " + panelWidget.Name);

            if (_player != null)
            {
                _player.EvtPlayerStateChanged -= _player_EvtPlayerStateChanged;
                _player.Dispose();
            }

            resetSwitchEventStates();

            _currentPanel = panelWidget;

            subscribeToMouseClickEvents(panelWidget);

            _player = new AnimationPlayer(panelWidget, _interpreter, _variables);
            _player.EvtPlayerStateChanged += _player_EvtPlayerStateChanged;
            _variables.Set(Variables.SelectedWidget, panelWidget);
            _variables.Set(Variables.CurrentPanel, panelWidget);

            // get all the animations for the specified animation name.
            var animations = getAnimations(animationName);

            if (!CoreGlobals.AppPreferences.EnableAutoStartScan)
            {
                Transition();
            }
            else
            {
                if (animations == null)
                {
                    Log.Error("Could not find animations entry for panel " + panelWidget.Name);
                    return;
                }

                // transition to the one that is marked as "first"
                var firstAnimation = animations.GetFirst();
                if (firstAnimation == null)
                {
                    return;
                }

                foreach (var animation in animations.Values)
                {
                    animation.EvtResolveWidgetChildren += animation_EvtResolveWidgetChildren;
                }

                _firstAnimation = firstAnimation;

                Transition(firstAnimation);
            }
        }

        /// <summary>
        /// Stop playing animations
        /// </summary>
        public void Stop()
        {
            if (_player != null)
            {
                Log.Debug("Before animation player stop");
                _player.Stop();
                Log.Debug("After animation player stop");
            }
        }

        /// <summary>
        /// Transition to the target animation named 'animationName'
        /// </summary>
        /// <param name="animationName">Name of the animation to transition to</param>
        public void Transition(String animationName)
        {
            try
            {
                Log.Debug();

                Log.Debug("_currentPanel: " + _currentPanel);

                resetSwitchEventStates();

                if (_player == null)
                {
                    Log.Debug("_player is null");
                    return;
                }

                if (_player.State != PlayerState.Running)
                {
                    return;
                }

                var animations = _animationsCollection["default"];
                var animation = animations[animationName];
                if (animation == null)
                {
                    Log.Debug("Transition: animation is NULL!");
                    return;
                }

                Log.Debug("Calling player transition");
                _player.Transition(animation);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Transition to the animation referred to by the
        /// animation object
        /// </summary>
        /// <param name="animation">target animation object</param>
        public void Transition(Animation animation = null)
        {
            try
            {
                if (!CoreGlobals.AppPreferences.EnableManualScan)
                {
                    if (animation != null)
                    {
                        Log.Debug("Transition( " + animation.Name + "). _currentPanel: " + _currentPanel.Name);
                        _player.Transition(animation);
                    }
                    else
                    {
                        _player.Transition(null);
                    }
                }
                else
                {
                    _player.TransitionManualScan();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">disposed yet?</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    unsubscribeToMouseClickEvents(_currentPanel);

                    // dispose all managed resources.
                    if (_player != null)
                    {
                        _player.Dispose();
                        _player = null;
                    }

                    if (_soundPlayer != null)
                    {
                        _soundPlayer.Dispose();
                    }

                    if (_animationsCollection != null)
                    {
                        _animationsCollection.Dispose();
                    }

                    unsubscribeFromActuatorEvents();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Event triggered when the player state changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _player_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            if (EvtPlayerStateChanged != null)
            {
                var delegates = EvtPlayerStateChanged.GetInvocationList();
                foreach (var del in delegates)
                {
                    var playerstateChanged = (PlayerStateChanged)del;
                    playerstateChanged.BeginInvoke(sender, e, null, null);
                }
            }
        }

        /// <summary>
        /// Event handler for event raised when the actuator manager has
        /// decided that the switch has been engaged long enough to be
        /// treated as a valid trigger
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void actuatorManager_EvtSwitchAccepted(object sender, ActuatorSwitchEventArgs e)
        {
            setSwitchState(true);
        }

        /// <summary>
        /// A switch was activated. Figure out the context and execute the
        /// appropriate action. The input manager triggers this event.  Every
        /// switch has an associated action.  It could be a command or the switch
        /// can be used to select highlighted item on a trigger.
        /// The action is executed depending on the state of the animation player.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void actuatorManager_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
        {
            IActuatorSwitch switchObj = e.SwitchObj;
            try
            {
                if (_currentPanel == null)
                {
                    return;
                }

                Log.Debug("switch: " + switchObj.Name);
                Log.Debug("   Panel: " + _currentPanel.Name);

                if (_currentPanel.UIControl is System.Windows.Forms.Form)
                {
                    bool visible = Windows.GetVisible(_currentPanel.UIControl);
                    Log.Debug("Form: " + _currentPanel.UIControl.Name + ", visible: " + visible);
                    if (!visible)
                    {
                        return;
                    }
                }

                // get the action associated with the switch
                String onTrigger = switchObj.Command;
                if (String.IsNullOrEmpty(onTrigger))
                {
                    Log.Debug("OnTrigger is null. returning");
                    return;
                }

                var manualScanMode = (!CoreGlobals.AppPreferences.EnableManualScan)
                    ? ManualScanModes.None
                    : mapTriggerScanMode(switchObj.GetTriggerScanMode());

                if (_player == null)
                {
                    if (String.Compare(onTrigger, SwitchSetting.TriggerCommand, true) != 0)
                    {
                        runSwitchMappedCommand(switchObj);
                    }
                    return;
                }

                Log.Debug("playerState: " + _player.State);

                // execute action if the player is in the right state.
                if (_player.State != PlayerState.Stopped &&
                    _player.State != PlayerState.Unknown &&
                    _player.State != PlayerState.Paused &&
                    manualScanMode == ManualScanModes.None &&
                    String.Compare(onTrigger, SwitchSetting.TriggerCommand, true) != 0)
                {
                    runSwitchMappedCommand(switchObj);
                    return;
                }

                if (CoreGlobals.AppPreferences.EnableManualScan)
                {
                    Log.Debug("HOOO form: " + _currentPanel.UIControl.Name + " Player state: " + _player.State);

                    if (_player.State == PlayerState.Paused)
                    {
                        Log.Debug(_currentPanel.Name + ": Player is paused. Returning");
                        return;
                    }

                    if (switchObj.IsSelectTriggerSwitch())
                    {
                        var widget = _player.HighlightedWidget;
                        if (widget != null)
                        {
                            Log.Debug("Actuate. widgetname: " + widget.Name + " Text: " + widget.GetText());
                            _player.Interrupt();
                            _player.ManualScanActuateWidget(widget);
                        }
                    }
                    else
                    {
                        _player.TransitionManualScan(manualScanMode);
                    }

                    return;
                }

                if (_player.State == PlayerState.Timeout || _player.State == PlayerState.Interrupted)
                {
                    Log.Debug("Calling player transition for firstanimation");
                    _player.Transition(_firstAnimation);
                    return;
                }

                Log.Debug("Player state is " + _player.State);
                if (_player.State != PlayerState.Running)
                {
                    Log.Debug(_currentPanel.Name + ": Player is not Running. Returning");
                    return;
                }

                playBeep(switchObj);

                AnimationWidget highlightedWidget = _player.HighlightedAnimationWidget;
                Animation currentAnimation = _player.CurrentAnimation;

                highlightedWidget = _switchDownHighlightedWidget;
                currentAnimation = _switchDownAnimation;

                if (highlightedWidget == null)
                {
                    highlightedWidget = _switchAcceptedHighlightedWidget;
                    currentAnimation = _switchAcceptedAnimation;
                }

                if (highlightedWidget == null)
                {
                    highlightedWidget = _player.HighlightedAnimationWidget;
                    currentAnimation = _player.CurrentAnimation;
                }

                resetSwitchEventStates();

                if (currentAnimation != null && highlightedWidget != null)
                {
                    setSwitchState(false);

                    var widgetName = (highlightedWidget.UIWidget is IButtonWidget) ?
                                                        "Button" :
                                                        highlightedWidget.UIWidget.Name;

                    AuditLog.Audit(new AuditEventUISwitchDetect(switchObj.Name,
                                                            _currentPanel.Name,
                                                            highlightedWidget.UIWidget.GetType().Name,
                                                            widgetName));

                    Log.Debug(_currentPanel.Name + ": Switch on " +
                                highlightedWidget.UIWidget.Name + " type: " +
                                highlightedWidget.UIWidget.GetType().Name);

                    // check if the widget has a onSelect code fragment. If so execute it.  Otherwise
                    // then check if the animation seq that this widget is a part of, has a onSelect.
                    // If it does, execute that.

                    PCode code;
                    SetSelectedWidget(highlightedWidget.UIWidget);
                    if (highlightedWidget.OnSelect.HasCode())
                    {
                        code = highlightedWidget.OnSelect;
                        _interpreter.Execute(code);
                    }
                    else if (currentAnimation.OnSelect.HasCode())
                    {
                        code = currentAnimation.OnSelect;
                        _interpreter.Execute(code);
                    }
                }
                else
                {
                    Log.Debug(_currentPanel.Name + ": No current animation or highlighed widget!!");
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
            finally
            {
                setSwitchState(false);
            }
        }

        /// <summary>
        /// Event handler for when an actuator switch is down
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void actuatorManager_EvtSwitchDown(object sender, ActuatorSwitchEventArgs e)
        {
            setSwitchState(true);

            if (_player != null)
            {
                _switchDownAnimation = _player.CurrentAnimation;
                var widget = _player.HighlightedAnimationWidget;
                if (widget != null)
                {
                    Log.Debug("Highlighted widget: " + widget.UIWidget.Name);
                    _switchDownHighlightedWidget = widget;
                }
                else
                {
                    _switchDownHighlightedWidget = null;
                }
            }
        }

        /// <summary>
        /// Event handler for when an actuator switch is rejected
        /// This is when a switch is held down for < acceptTime.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event argument</param>
        private void actuatorManager_EvtSwitchRejected(object sender, ActuatorSwitchEventArgs e)
        {
            resetSwitchEventStates();

            setSwitchState(false);
        }

        /// <summary>
        /// Event handler for when an actuator switch is up
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void actuatorManager_EvtSwitchUp(object sender, ActuatorSwitchEventArgs e)
        {
        }

        /// <summary>
        /// Resolves a reference to a UI widget to its actual name and adds
        /// it to the 'args' list
        /// </summary>
        /// <param name="args">list of names to add to</param>
        /// <param name="variableName">variable to resolve</param>
        /// <param name="defaultValue">default value</param>
        private void addArg(List<String> args, String variableName, String defaultValue)
        {
            var widget = (Widget)_variables.Get(variableName);
            args.Add(widget != null ? widget.Name : defaultValue);
        }

        /// <summary>
        /// Event triggered to resovle any wild cards in the animation sequence.  The
        /// wildcards are expanded into actual animation objects
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void animation_EvtResolveWidgetChildren(object sender, ResolveWidgetChildrenEventArgs e)
        {
            if (EvtResolveWidgetChildren != null)
            {
                EvtResolveWidgetChildren(sender, e);
                var children = new List<Widget>();
                e.ContainerWidget.Finder.FindAllButtons(children);

                foreach (var widget in children)
                {
                    widget.EvtMouseClicked -= button_EvtMouseClicked;
                    widget.EvtMouseClicked += button_EvtMouseClicked;
                }
            }
        }

        /// <summary>
        /// Event handler to actuate this widget.
        /// The interpreter triggers this event on the "actuate" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        private void AppInterpreter_EvtActuateNotify(object sender, InterpreterEventArgs e)
        {
            List<String> resolvedArgs = ResolveArgs(e.Args);
            if (resolvedArgs.Count > 0)
            {
                String widgetName = resolvedArgs[0];

                // get the widget object
                var widget = _currentPanel.Finder.FindChild(widgetName);
                if (widget != null)
                {
                    Log.Debug("Actuate. widgetname: " + widget.Name + " Text: " + widget.GetText());

                    widget.Actuate();
                }
                else
                {
                    Log.Debug("Did not actuate.  Could not find widget  " + widgetName);
                }
            }
        }

        /// <summary>
        /// Event triggered to play a beep sound
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        private void AppInterpreter_EvtBeep(object sender, InterpreterEventArgs e)
        {
            playDefaultBeep();
        }

        /// <summary>
        /// Event handler to highlight/unhighlight the specified widget.
        /// This event is triggered by the Interpreter on the "highlight" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        private void AppInterpreter_EvtHighlightNotify(object sender, InterpreterEventArgs e)
        {
            if (e.Args.Count == 0)
            {
                return;
            }

            bool onOff = false;
            List<String> resolvedArgs = ResolveArgs(e.Args);

            if (e.Args.Count > 1)
            {
                // translate argument from "true" "false" to a boolean
                onOff = ResolveBool(e.Args[1]);
            }

            var widgetName = resolvedArgs[0];
            var widget = _currentPanel.Finder.FindChild(widgetName);
            if (widget != null)
            {
                // turn off everything except the one we want
                Widget parent = widget.Parent;
                foreach (Widget child in parent.Children)
                {
                    child.HighlightOff();
                }

                if (onOff)
                {
                    widget.HighlightOn();
                }
            }
        }

        /// <summary>
        /// Event handler to highlight/unhighlight the specified widget.
        /// This event is triggered by the Interpreter on the "highlightSelected" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        private void AppInterpreter_EvtHighlightSelectedNotify(object sender, InterpreterEventArgs e)
        {
            if (e.Args.Count == 0)
            {
                return;
            }

            bool onOff = false;
            List<String> resolvedArgs = ResolveArgs(e.Args);

            if (e.Args.Count > 1)
            {
                // translate argument from "true" "false" to a boolean
                onOff = ResolveBool(e.Args[1]);
            }

            String widgetName = resolvedArgs[0];

            Log.Debug("_currentPanel " + _currentPanel.Name + " widgetname: " + widgetName);
            var widget = _currentPanel.Finder.FindChild(widgetName);
            if (widget != null)
            {
                if (onOff)
                {
                    widget.SelectedHighlightOn();
                }
                else
                {
                    widget.SelectedHighlightOff();
                }
            }
        }

        /// <summary>
        /// Event handler to select the specified widget.
        /// The interpreter triggers this event on the "select" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        private void AppInterpreter_EvtSelectNotify(object sender, InterpreterEventArgs e)
        {
            List<String> resolvedArgs = ResolveArgs(e.Args);
            if (resolvedArgs.Count > 0)
            {
                var widgetName = e.Args[0];
                SetSelectedWidget(widgetName);
            }
        }

        /// <summary>
        /// The stop command was interpreted.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void AppInterpreter_EvtStop(object sender, InterpreterEventArgs e)
        {
            Interrupt();
        }

        /// <summary>
        /// Event handler to transition to the specified animation.
        /// The interpreter triggers this event on the "transition" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        private void AppInterpreter_EvtTransitionNotify(object sender, InterpreterEventArgs e)
        {
            Log.Debug();

            List<String> resolvedArgs = ResolveArgs(e.Args);
            if (resolvedArgs.Count > 0)
            {
                String targetAnimation = resolvedArgs[0];
                Log.Debug(targetAnimation);
                Transition(targetAnimation);
            }
        }

        /// <summary>
        /// Button click event was detected.  Raises the mouse click event
        /// to notify the app
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void button_EvtMouseClicked(object sender, WidgetEventArgs e)
        {
            if (_player == null)
            {
                return;
            }

            var widget = e.SourceWidget;

            SetSelectedWidget(widget);

            if (widget.Enabled)
            {
                if (widget.OnMouseClick != null && widget.OnMouseClick.HasCode())
                {
                    if (_player.State != PlayerState.Paused)
                    {
                        _interpreter.Execute(widget.OnMouseClick);
                    }
                }
                else if (widget.IsMouseClickActuateOn)
                {
                    widget.Actuate();
                }
            }
        }

        /// <summary>
        /// Returns animations for the specified animation name
        /// If name is null or empty, uses "default"
        /// </summary>
        /// <param name="name">Name of animation</param>
        /// <returns>Animation collection</returns>
        private Animations getAnimations(String name)
        {
            Animations animations = null;
            if (_animationsCollection.Count > 0)
            {
                animations = String.IsNullOrEmpty(name) ? _animationsCollection["default"] : _animationsCollection[name];
            }

            return animations;
        }

        /// <summary>
        /// Maps the switch trigger scan mode to a scan mode
        /// </summary>
        /// <param name="triggerScanMode">The switch trigger scan mode</param>
        /// <returns>scan mode</returns>
        private ManualScanModes mapTriggerScanMode(TriggerScanModes triggerScanMode)
        {
            var scanMode = ManualScanModes.None;

            switch (triggerScanMode)
            {
                case TriggerScanModes.TriggerScanLeft:
                    scanMode = ManualScanModes.ScanLeft;
                    break;

                case TriggerScanModes.TriggerScanRight:
                    scanMode = ManualScanModes.ScanRight;
                    break;

                case TriggerScanModes.TriggerScanUp:
                    scanMode = ManualScanModes.ScanUp;
                    break;

                case TriggerScanModes.TriggerScanDown:
                    scanMode = ManualScanModes.ScanDown;
                    break;

                case TriggerScanModes.TriggerMoveLeft:
                    scanMode = ManualScanModes.MoveLeft;
                    break;

                case TriggerScanModes.TriggerMoveRight:
                    scanMode = ManualScanModes.MoveRight;
                    break;

                case TriggerScanModes.TriggerMoveUp:
                    scanMode = ManualScanModes.MoveUp;
                    break;

                case TriggerScanModes.TriggerMoveDown:
                    scanMode = ManualScanModes.MoveDown;
                    break;

                case TriggerScanModes.TriggerStop:
                    scanMode = ManualScanModes.Stop;
                    break;

                case TriggerScanModes.TriggerPause:
                    scanMode = ManualScanModes.Pause;
                    break;

                case TriggerScanModes.TriggerResume:
                    scanMode = ManualScanModes.Resume;
                    break;

                case TriggerScanModes.TriggerPauseToggle:
                    scanMode = ManualScanModes.TogglePause;
                    break;
            }

            return scanMode;
        }

        /// <summary>
        /// Plays a beep associated with the switch.  If none, plays
        /// the default beep
        /// </summary>
        /// <param name="switchObj">the source siwtch</param>
        private void playBeep(IActuatorSwitch switchObj)
        {
            try
            {
                if (CoreGlobals.AppPreferences.SelectClick)
                {
                    if (switchObj.Audio != null)
                    {
                        switchObj.Audio.Play();
                    }
                    else
                    {
                        playDefaultBeep();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Plays the default beep sound.
        /// </summary>
        private void playDefaultBeep()
        {
            try
            {
                if (_soundPlayer == null)
                {
                    _soundPlayer = new SoundPlayer(FileUtils.GetSoundPath("beep.wav"));
                }

                if (_soundPlayer != null)
                {
                    _soundPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Sets all the variables related to switch events
        /// </summary>
        private void resetSwitchEventStates()
        {
            _switchDownHighlightedWidget = null;
            _switchAcceptedHighlightedWidget = null;
            _switchDownAnimation = null;
            _switchAcceptedAnimation = null;
        }

        /// <summary>
        /// Runds the command mapped to the specified switch. Checks
        /// the command permissions if it CAN be executed.
        /// </summary>
        /// <param name="switchObj">The switch object</param>
        private void runSwitchMappedCommand(IActuatorSwitch switchObj)
        {
            bool runCommand = true;
            String onTrigger = switchObj.Command;

            var form = _currentPanel.UIControl;
            if (form is IScannerPanel)
            {
                var panelCommon = (form as IScannerPanel).PanelCommon;
                var arg = new CommandEnabledArg(null, onTrigger);
                panelCommon.CheckCommandEnabled(new CommandEnabledArg(null, onTrigger));

                if (arg.Handled)
                {
                    if (!arg.Enabled)
                    {
                        Log.Debug("Command " + onTrigger + " is not currently enabled");
                        return;
                    }
                    else
                    {
                        Log.Debug("Command " + onTrigger + " IS ENABLED");
                    }
                }
                else
                {
                    Log.Debug("arg.handled is false for " + onTrigger);

                    var strTrigger = onTrigger;
                    if (strTrigger[0] == '@')
                    {
                        strTrigger = strTrigger.Substring(1);
                    }
                    var cmdDescriptor = CommandManager.Instance.AppCommandTable.Get(strTrigger);
                    if (cmdDescriptor != null && !cmdDescriptor.EnableSwitchMap)
                    {
                        Log.Debug("EnableswitchMap is not enabled for " + onTrigger);
                        runCommand = false;
                    }
                }
            }
            else
            {
                Log.Debug("Dialog is active. Will not handle");
                runCommand = false;
            }

            if (runCommand)
            {
                Log.Debug("Executing OnTrigger command " + onTrigger + " for panel..." + _currentPanel.Name);
                PCode pcode = new PCode { Script = "run(" + onTrigger + ")" };
                var parser = new Parser();
                if (parser.Parse(pcode.Script, ref pcode))
                {
                    _interpreter.Execute(pcode);
                }
            }
        }

        private void setSwitchState(bool state)
        {
            IsSwitchActive = state;
            if (_player != null)
            {
                _player.IsSwitchActive = state;
            }
        }

        /// <summary>
        /// Subscribes to events from the actuator manager
        /// </summary>
        private void subscribeToActuatorEvents()
        {
            ActuatorManager.Instance.EvtSwitchActivated += actuatorManager_EvtSwitchActivated;
            ActuatorManager.Instance.EvtSwitchDown += actuatorManager_EvtSwitchDown;
            ActuatorManager.Instance.EvtSwitchUp += actuatorManager_EvtSwitchUp;
            ActuatorManager.Instance.EvtSwitchAccepted += actuatorManager_EvtSwitchAccepted;
            ActuatorManager.Instance.EvtSwitchRejected += actuatorManager_EvtSwitchRejected;
        }

        /// <summary>
        /// Subscribes to the various events we are interested in from the interpreter.
        /// While the animation is executing, the interpreter interprets the code associated
        /// with the animation and raises events as and when the code needs to be acted on.
        /// </summary>
        private void subscribeToInterpreterEvents()
        {
            _interpreter.EvtTransitionNotify += AppInterpreter_EvtTransitionNotify;
            _interpreter.EvtActuateNotify += AppInterpreter_EvtActuateNotify;
            _interpreter.EvtSelectNotify += AppInterpreter_EvtSelectNotify;
            _interpreter.EvtHighlightNotify += AppInterpreter_EvtHighlightNotify;
            _interpreter.EvtHighlightSelectedNotify += AppInterpreter_EvtHighlightSelectedNotify;
            _interpreter.EvtBeep += AppInterpreter_EvtBeep;
            _interpreter.EvtStopNotify += AppInterpreter_EvtStop;
        }

        /// <summary>
        /// Subscribes to mouse click events for all the buttons in the
        /// layout
        /// </summary>
        /// <param name="rootWidget">Root widget for the scanner</param>
        private void subscribeToMouseClickEvents(Widget rootWidget)
        {
            var list = new List<Widget>();
            rootWidget.Finder.FindAllButtons(list);
            foreach (var button in list)
            {
                button.EvtMouseClicked += button_EvtMouseClicked;
            }
        }

        /// <summary>
        /// Unsubscribes from actuator events
        /// </summary>
        private void unsubscribeFromActuatorEvents()
        {
            ActuatorManager.Instance.EvtSwitchActivated -= actuatorManager_EvtSwitchActivated;
            ActuatorManager.Instance.EvtSwitchDown -= actuatorManager_EvtSwitchDown;
            ActuatorManager.Instance.EvtSwitchUp -= actuatorManager_EvtSwitchUp;
            ActuatorManager.Instance.EvtSwitchAccepted -= actuatorManager_EvtSwitchAccepted;
            ActuatorManager.Instance.EvtSwitchRejected -= actuatorManager_EvtSwitchRejected;
        }

        /// <summary>
        /// Unsubscribe button events
        /// </summary>
        /// <param name="rootWidget">root widget for the scanner</param>
        private void unsubscribeToMouseClickEvents(Widget rootWidget)
        {
            if (rootWidget != null)
            {
                var list = new List<Widget>();
                rootWidget.Finder.FindAllButtons(list);
                foreach (var button in list)
                {
                    button.EvtMouseClicked -= button_EvtMouseClicked;
                }
            }
        }
    }
}