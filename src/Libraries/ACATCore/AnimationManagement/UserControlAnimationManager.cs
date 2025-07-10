using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.Audit;
using ACAT.Core.CommandManagement;
using ACAT.Core.Interpreter;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;
using ACAT.Core.UserControlManagement;

namespace ACAT.Core.AnimationManagement
{
    public class UserControlAnimationManager : AnimationManager, IUserControlAnimationManager
    {
        private UserControlConfigMapEntry mapEntry { get; set; } = null;
        private String name { get; set; } = null;

        public delegate void PlayerAnimationTransition(object sender, String animationName, bool isTopLevel);

        public event PlayerAnimationTransition EvtPlayerAnimationTransition;

        public UserControlAnimationManager() : base() { }

        public bool Init(UserControlConfigMapEntry mapentry)
        {
            mapEntry = mapentry;
            name = mapEntry.Name;

            bool retVal = _animationsCollection.Load(mapEntry.ConfigFileName);
            if (retVal)
            {
                retVal = _interpreter.LoadScripts(mapEntry.ConfigFileName);
            }

            if (retVal)
            {
                subscribeToInterpreterEvents();

                subscribeToActuatorEvents();
            }

            Log.Debug("returning from Anim manager init()");

            return retVal;
        }

        public bool IsPlayerRunning()
        {
            return (_player != null && _player.State == PlayerState.Running);
        }

        public void OnLoad(Widget panelWidget, String animationName = null)
        {
            Log.Debug("Start animation for panel " + panelWidget.Name);

            if (_player != null)
            {
                _player.EvtPlayerStateChanged -= _player_EvtPlayerStateChanged;
                _player.EvtPlayerAnimationTransition -= _player_EvtPlayerAnimationTransition;
                _player.Dispose();
            }

            resetSwitchEventStates();

            _currentPanel = panelWidget;

            subscribeToMouseClickEvents(panelWidget);

            _player = new AnimationPlayer(panelWidget, _interpreter, _variables);
            _player.EvtPlayerStateChanged += _player_EvtPlayerStateChanged;
            _player.EvtPlayerAnimationTransition += _player_EvtPlayerAnimationTransition;
            _variables.Set(Variables.SelectedWidget, panelWidget);
            _variables.Set(Variables.CurrentPanel, panelWidget);

            // get all the animations for the specified animation name.
            var animations = getAnimations(animationName);

            if (CoreGlobals.AppPreferences.EnableAutoStartScan)
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
            }
        }

        /// <summary>
        /// Starts the animation sequence for the specified panel. It starts
        /// with the animation that has the 'start' attribute set to true in
        /// the xml file
        /// </summary>
        /// <param name="panelWidget">Which panel to start the animations for?</param>
        /// <param name="animationName">Name of the animation sequence</param>
        public void Start(String animationName = null)
        {
            if (!CoreGlobals.AppPreferences.EnableAutoStartScan)
            {
                Log.Debug("CALIBTEST: UserControlAnimationManager.Start.  Do AutoTransition");
                Transition();
            }
            else
            {
                Log.Debug("CALIBTEST: UserControlAnimationManager.Start.");
                Transition(_firstAnimation);
            }
        }

        public override void TransitionFromName(string animationName)
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
                EvtPlayerAnimationTransition?.Invoke(this, animation.Name, animation.IsFirst);
                _player.Transition(animation);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        protected override void actuatorManager_EvtSwitchDown(object sender, ActuatorSwitchEventArgs e)
        {
            base.actuatorManager_EvtSwitchDown(sender, e);
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
        protected override void actuatorManager_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
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
                String onTrigger = switchObj.Command;
                if (String.IsNullOrEmpty(onTrigger))
                {
                    Log.Debug("OnTrigger is null. returning");
                    return;
                }

                var manualScanMode = (!CoreGlobals.AppPreferences.EnableManualScan)
                    ? ManualScanModes.None
                    : mapTriggerScanMode(switchObj.GetTriggerScanMode());

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

                    Log.Debug("AP1: _player.State is " + _player.State);
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

                    var cmdDescriptor = CommandManager.Instance.AppCommandTable.Get(onTrigger);
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

        private void _player_EvtPlayerAnimationTransition(object sender, string animationName, bool isTopLevel)
        {
            EvtPlayerAnimationTransition?.Invoke(sender, animationName, isTopLevel);
        }
    }
}