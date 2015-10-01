////////////////////////////////////////////////////////////////////////////
// <copyright file="AnimationManager.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Media;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Interpreter;
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
        /// Maps switches to actions.
        /// </summary>
        private SwitchConfig _switchConfig;

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

        /// <summary>
        /// Allcoate resources, parse the config file which contains all the
        /// animations and create a list of animation objects. Subscribe to
        /// events
        /// </summary>
        /// <param name="configPath">Name of the config file for the panel</param>
        /// <returns>true on success</returns>
        public bool Init(String configPath)
        {
            bool retVal = _animationsCollection.Load(configPath);
            if (retVal)
            {
                retVal = _interpreter.LoadScripts(configPath);
            }

            if (retVal)
            {
                _switchConfig = new SwitchConfig();
                _switchConfig.Load(configPath);
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
        /// Resumes animation
        /// </summary>
        public void Resume()
        {
            if (_player != null)
            {
                _firstAnimation.OnStart = true;
                _player.Resume(_firstAnimation);
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
        public void Transition(Animation animation)
        {
            try
            {
                Log.Debug("Transition( " + animation.Name + "). _currentPanel: " + _currentPanel.Name);
                _player.Transition(animation);
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

        private void actuatorManager_EvtSwitchAccepted(object sender, ActuatorSwitchEventArgs e)
        {
            setSwitchState(true);
        }

        /// <summary>
        /// A switch was activated. Figure out the context and execute the
        /// appropriate action. The input manager triggers this event.  Every
        /// switch has an action that is configured in the swtichconfigmap file.
        /// The action is executed depending on the state of the animation player.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void actuatorManager_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
        {
            IActuatorSwitch switchObj = e.SwitchObj;
            try
            {
                if (_player == null || _currentPanel == null)
                {
                    return;
                }

                Log.Debug("switch: " + switchObj.Name);
                Log.Debug("   Panel: " + _currentPanel.Name);

                if (_currentPanel.UIControl is System.Windows.Forms.Form)
                {
                    bool visible = Windows.GetVisible(_currentPanel.UIControl);
                    Log.Debug("Form: " + _currentPanel.UIControl.Name + ", playerState: " + _player.State + ", visible: " + visible);
                    if (!visible)
                    {
                        return;
                    }
                }

                // get the action associated with the switch
                PCode onTrigger = getOnTrigger(switchObj);
                if (onTrigger == null)
                {
                    Log.Debug("OnTrigger is null. returning");
                    return;
                }

                Log.Debug("onTrigger.HasCode: " + onTrigger.HasCode());

                // execute action if the player is in the right state.
                if (_player.State != PlayerState.Stopped &&
                    _player.State != PlayerState.Unknown &&
                    _player.State != PlayerState.Paused &&
                    onTrigger.HasCode())
                {
                    Log.Debug("Executing OnTrigger for panel..." + _currentPanel.Name);
                    _interpreter.Execute(onTrigger);
                    return;
                }

                if (_player.State == PlayerState.Timeout || _player.State == PlayerState.Interrupted)
                {
                    Log.Debug("Calling player transition for firstanimation");
                    _player.Transition(_firstAnimation);
                    return;
                }

                Log.Debug("PLayer state is " + _player.State);
                if (_player.State != PlayerState.Running)
                {
                    Log.Debug(_currentPanel.Name + ": Player is not Running. Returning");
                    return;
                }

                playBeep(switchObj);

                AnimationWidget highlightedWidget = _player.HighlightedWidget;
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
                    highlightedWidget = _player.HighlightedWidget;
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
                var widget = _player.HighlightedWidget;
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

            var animationWidget = _player.GetAnimationWidgetInCurrentAnimation(widget);
            PCode onMouseClick = null;

            SetSelectedWidget(widget);

            if (animationWidget != null && animationWidget.OnMouseClick != null)
            {
                onMouseClick = animationWidget.OnMouseClick;
            }
            else if (widget.OnMouseClick != null)
            {
                onMouseClick = widget.OnMouseClick;
            }

            if (onMouseClick != null && onMouseClick.HasCode())
            {
                _interpreter.Execute(onMouseClick);
            }
            else if (widget.IsMouseClickActuateOn)
            {
                widget.Actuate();
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
        /// Returns the action associated with the switch.  These actions
        /// are configured in the switch map config file. First checks the
        /// animation config file to see if there is a mapping there.  If not
        /// goes to the global mapping table
        /// </summary>
        /// <param name="switchObj">the switch object</param>
        /// <returns>associated pCode</returns>
        private PCode getOnTrigger(IActuatorSwitch switchObj)
        {
            const string defaultWidgetClass = "default";
            Log.Debug();

            if (_switchConfig == null)
            {
                return null;
            }

            // check local switchmap first
            PCode pCode = _switchConfig.GetOnTrigger(switchObj);
            if (pCode != null)
            {
                Log.Debug("Found local pcode for " + switchObj.Name);
                return pCode;
            }

            Log.Debug("Did not find local switchconfig.  Checking global one");

            // if this panel is a member of a 'class' of panels(configured thru
            // the subclass attribute), check if there is a switch map for the class.
            // otherwise, just use the default one.

            var widgetClass = (_currentPanel != null) ? _currentPanel.SubClass : String.Empty;

            if (String.IsNullOrEmpty(widgetClass))
            {
                widgetClass = defaultWidgetClass;
            }

            Log.Debug("widgetclass: " + widgetClass);

            SwitchConfig switchConfig = ActuatorManager.Instance.SwitchConfigMap;

            PCode retVal = switchConfig.GetOnTrigger(widgetClass, switchObj);

            if (retVal != null)
            {
                return retVal;
            }

            if (widgetClass != defaultWidgetClass)
            {
                Log.Debug("Could not find PCode for " + widgetClass + ", trying default");
                widgetClass = defaultWidgetClass;

                retVal = switchConfig.GetOnTrigger(widgetClass, switchObj);
            }

            Log.IsNull("retval ", retVal);

            return retVal;
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