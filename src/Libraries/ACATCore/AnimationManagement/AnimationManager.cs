using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.Audit;
using ACAT.Core.CommandManagement;
using ACAT.Core.Interpreter;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Media;
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


namespace ACAT.Core.AnimationManagement
{
    public partial class AnimationManager : IAnimationManager, IDisposable
    {
        /// <summary>
        /// Collection of animations for this panel
        /// </summary>
        protected readonly AnimationsCollection _animationsCollection;

        /// <summary>
        /// Interpret script
        /// </summary>
        protected readonly Interpret _interpreter;

        /// <summary>
        /// Stores transient variables for animation
        /// </summary>
        internal readonly Variables _variables;

        /// <summary>
        /// The panel to which this Animation Manager belongs
        /// </summary>
        protected  Widget _currentPanel;

        /// <summary>
        /// has this object been disposed off yet?
        /// </summary>
        protected  bool _disposed;

        /// <summary>
        /// Points to the first animation in the sequence
        /// </summary>
        protected Animation _firstAnimation;


        /// <summary>
        /// The animation player that actually plays the animation
        /// </summary>
        internal AnimationPlayer _player;

        /// <summary>
        /// Plays beeps
        /// </summary>
        protected  SoundPlayer _soundPlayer;

        /// <summary>
        /// Animation that was in progress when a swtich-accept event was
        /// received
        /// </summary>
        protected  Animation _switchAcceptedAnimation;

        /// <summary>
        /// Highlighted widget when a switch accept event is recrived
        /// </summary>
        protected  AnimationWidget _switchAcceptedHighlightedWidget;

        /// <summary>
        /// Animation that was in progress when a switch-down was received
        /// </summary>
        protected  Animation _switchDownAnimation;

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
        /// Highlighted widget when a switchdown event is received
        /// </summary>
        protected AnimationWidget _switchDownHighlightedWidget;

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
        public  Interpret Interpreter
        {
            get { return _interpreter; }
        }

        /// <summary>
        /// Gets/sets whether an actuator switch is currently active
        /// </summary>
        public  bool IsSwitchActive { get; set; }


        /// <summary>
        /// Disposes resources
        /// </summary>
        public  void Dispose()
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
        public  PlayerState GetPlayerState()
        {
            return (_player != null) ? _player.State : PlayerState.Unknown;
        }

        public  void HighlightDefaultHome()
        {
            if (CoreGlobals.AppPreferences.EnableManualScan)
            {
                _player.HighlightDefaultHome();
            }
        }


        /// <summary>
        /// Interrupt the animation sequence
        /// </summary>
        public  void Interrupt()
        {
            _player?.Interrupt();
        }

        /// <summary>
        /// Pause animcation
        /// </summary>
        public  void Pause()
        {
            _player?.Pause();
        }

        /// <summary>
        /// Resolves variable references to their actual values.  Variables
        /// start with an @ symbol.
        /// </summary>
        /// <param name="args">args to resolve</param>
        /// <returns>The arglist with variables resolved</returns>
        public  List<String> ResolveArgs(List<String> args)
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
        public  bool ResolveBool(String arg)
        {
            return String.Compare(arg, "true", true) == 0;
        }

        /// <summary>
        /// Transitions to the starting sequence
        /// </summary>
        public  void Restart()
        {
            if (_firstAnimation != null)
            {
                Transition(_firstAnimation);
            }
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public  void Resume()
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
        public  void SetSelectedWidget(String widgetName)
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
        public  void SetSelectedWidget(Widget selectedWidget)
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
        /// Stop playing animations
        /// </summary>
        public  void Stop()
        {
            if (_player != null)
            {
                Log.Debug("Before animation player stop");
                try
                {
                    _player.Stop();
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                }
                Log.Debug("After animation player stop");
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
        protected  void Dispose(bool disposing)
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

                    _soundPlayer?.Dispose();

                    _animationsCollection?.Dispose();

                    unsubscribeFromActuatorEvents();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        protected virtual void actuatorManager_EvtSwitchAccepted(object sender, ActuatorSwitchEventArgs e)
        {
            setSwitchState(true);
        }


        /// <summary>
        /// Event handler for when an actuator switch is down
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        protected virtual void actuatorManager_EvtSwitchDown(object sender, ActuatorSwitchEventArgs e)
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
        protected  void actuatorManager_EvtSwitchRejected(object sender, ActuatorSwitchEventArgs e)
        {
            resetSwitchEventStates();

            setSwitchState(false);
        }

        /// <summary>
        /// Event handler for when an actuator switch is up
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        protected  void actuatorManager_EvtSwitchUp(object sender, ActuatorSwitchEventArgs e)
        {
        }

        /// <summary>
        /// Resolves a reference to a UI widget to its actual name and adds
        /// it to the 'args' list
        /// </summary>
        /// <param name="args">list of names to add to</param>
        /// <param name="variableName">variable to resolve</param>
        /// <param name="defaultValue">default value</param>
        protected  void addArg(List<String> args, String variableName, String defaultValue)
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
        protected  void animation_EvtResolveWidgetChildren(object sender, ResolveWidgetChildrenEventArgs e)
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
        protected  void AppInterpreter_EvtActuateNotify(object sender, InterpreterEventArgs e)
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
        protected  void AppInterpreter_EvtBeep(object sender, InterpreterEventArgs e)
        {
            playDefaultBeep();
        }

        /// <summary>
        /// Event handler to highlight/unhighlight the specified widget.
        /// This event is triggered by the Interpreter on the "highlight" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        protected virtual void AppInterpreter_EvtHighlightNotify(object sender, InterpreterEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler to highlight/unhighlight the specified widget.
        /// This event is triggered by the Interpreter on the "highlightSelected" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        protected void AppInterpreter_EvtHighlightSelectedNotify(object sender, InterpreterEventArgs e)
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
        protected  void AppInterpreter_EvtSelectNotify(object sender, InterpreterEventArgs e)
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
        protected  void AppInterpreter_EvtStop(object sender, InterpreterEventArgs e)
        {
            Interrupt();
        }

        /// <summary>
        /// Event handler to transition to the specified animation.
        /// The interpreter triggers this event on the "transition" verb
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">Argument list</param>
        protected void AppInterpreter_EvtTransitionNotify(object sender, InterpreterEventArgs e)
        {
            Log.Debug();

            List<String> resolvedArgs = ResolveArgs(e.Args);
            if (resolvedArgs.Count > 0)
            {
                String targetAnimation = resolvedArgs[0];
                Log.Debug(targetAnimation);
                //Transition(GetAnimation(targetAnimation));
                TransitionFromName(targetAnimation);
            }
        }

        public virtual void TransitionFromName(String targetAnimation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Button click event was detected.  Raises the mouse click event
        /// to notify the app
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        protected void button_EvtMouseClicked(object sender, WidgetEventArgs e)
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
        protected  Animations getAnimations(String name)
        {
            Animations animations = null;
            if (_animationsCollection.Count > 0)
            {
                animations = String.IsNullOrEmpty(name) ? _animationsCollection["default"] : _animationsCollection[name];
            }

            return animations;
        }

        /// <summary>
        /// Transition to the target animation named 'animation'
        /// </summary>
        /// <param name="animation">Name of the animation to transition to</param>
        protected Animation GetAnimation(String animationName = null)
        {
            return _animationsCollection["default"][animationName];
        }

        /// <summary>
        /// Maps the switch trigger scan mode to a scan mode
        /// </summary>
        /// <param name="triggerScanMode">The switch trigger scan mode</param>
        /// <returns>scan mode</returns>
        protected ManualScanModes mapTriggerScanMode(TriggerScanModes triggerScanMode)
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
        protected  void playBeep(IActuatorSwitch switchObj)
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
        protected  void playDefaultBeep()
        {
            try
            {
                _soundPlayer ??= new SoundPlayer(FileUtils.GetSoundPath("beep.wav"));

                _soundPlayer?.Play();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Sets all the variables related to switch events
        /// </summary>
        protected  void resetSwitchEventStates()
        {
            _switchDownHighlightedWidget = null;
            _switchAcceptedHighlightedWidget = null;
            _switchDownAnimation = null;
            _switchAcceptedAnimation = null;
        }


        protected  void setSwitchState(bool state)
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
        protected void subscribeToActuatorEvents()
        {
            ActuatorManager.Instance.EvtSwitchActivated += actuatorManager_EvtSwitchActivated;
            ActuatorManager.Instance.EvtSwitchDown += actuatorManager_EvtSwitchDown;
            ActuatorManager.Instance.EvtSwitchUp += actuatorManager_EvtSwitchUp;
            ActuatorManager.Instance.EvtSwitchAccepted += actuatorManager_EvtSwitchAccepted;
            ActuatorManager.Instance.EvtSwitchRejected += actuatorManager_EvtSwitchRejected;
        }

        protected virtual void actuatorManager_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Subscribes to the various events we are interested in from the interpreter.
        /// While the animation is executing, the interpreter interprets the code associated
        /// with the animation and raises events as and when the code needs to be acted on.
        /// </summary>
        protected  void subscribeToInterpreterEvents()
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
        protected  void subscribeToMouseClickEvents(Widget rootWidget)
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
        protected  void unsubscribeFromActuatorEvents()
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
        protected  void unsubscribeToMouseClickEvents(Widget rootWidget)
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

        /// <summary>
        /// Event triggered when the player state changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        protected void _player_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
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
    }
}