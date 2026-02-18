using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.Audit;
using ACAT.Core.CommandManagement;
using ACAT.Core.Interpreter;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;
using ACAT.Core.UserControlManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using ACAT.Core.ActuatorManagement.Settings;
using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.AnimationManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System.Windows.Forms;

namespace ACAT.Core.AnimationManagement
{
    public class UserControlAnimationManager : AnimationManager, IUserControlAnimationManager
    {
        private readonly ILogger<UserControlAnimationManager> _logger;
        private UserControlConfigMapEntry mapEntry { get; set; } = null;
        private String name { get; set; } = null;

        public delegate void PlayerAnimationTransition(object sender, String animationName, bool isTopLevel);

        public event PlayerAnimationTransition EvtPlayerAnimationTransition;

        public UserControlAnimationManager() : base()
        {
            _logger = LoggingConfiguration.CreateLogger<UserControlAnimationManager>();
        }

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

            _logger.LogDebug("Returning from Anim manager init()");

            return retVal;
        }

        public bool IsPlayerRunning()
        {
            return (_player != null && _player.State == PlayerState.Running);
        }

        public void OnLoad(Widget panelWidget, String animationName = null)
        {
            _logger.LogDebug("Start animation for panel {PanelName}", panelWidget.Name);

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
            Animations animations = getAnimations(animationName);

            if (CoreGlobals.AppPreferences.EnableAutoStartScan)
            {
                if (animations == null)
                {
                    _logger.LogError("Could not find animations entry for panel {PanelName}", panelWidget.Name);
                    return;
                }

                // transition to the one that is marked as "first"
                Animation firstAnimation = animations.GetFirst();
                if (firstAnimation == null)
                {
                    return;
                }

                foreach (Animation animation in animations.Values)
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
                _logger.LogTrace("CALIBTEST: UserControlAnimationManager.Start.  Do AutoTransition");
                Transition();
            }
            else
            {
                _logger.LogTrace("CALIBTEST: UserControlAnimationManager.Start.");
                Transition(_firstAnimation);
            }
        }

        public override void TransitionFromName(string animationName)
        {
            try
            {
                _logger.LogTrace("TransitionFromName called");

                _logger.LogDebug("_currentPanel: {CurrentPanel}", _currentPanel);

                resetSwitchEventStates();

                if (_player == null)
                {
                    _logger.LogDebug("_player is null");
                    return;
                }

                if (_player.State != PlayerState.Running)
                {
                    return;
                }

                Animations animations = _animationsCollection["default"];
                Animation animation = animations[animationName];
                if (animation == null)
                {
                    _logger.LogDebug("Transition: animation is NULL!");
                    return;
                }

                _logger.LogDebug("Calling player transition");
                EvtPlayerAnimationTransition?.Invoke(this, animation.Name, animation.IsFirst);
                _player.Transition(animation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in TransitionFromName");
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

                _logger.LogDebug("switch: {SwitchName}", switchObj.Name);
                _logger.LogDebug("   Panel: {PanelName}", _currentPanel.Name);

                if (_currentPanel.UIControl is System.Windows.Forms.Form)
                {
                    bool visible = Windows.GetVisible(_currentPanel.UIControl);
                    _logger.LogDebug("Form: {FormName}, playerState: {PlayerState}, visible: {Visible}", _currentPanel.UIControl.Name, _player.State, visible);
                    if (!visible)
                    {
                        return;
                    }
                }

                // get the action associated with the switch
                String onTrigger = switchObj.Command;
                if (String.IsNullOrEmpty(onTrigger))
                {
                    _logger.LogDebug("OnTrigger is null. returning");
                    return;
                }

                ManualScanModes manualScanMode = (!CoreGlobals.AppPreferences.EnableManualScan)
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
                    _logger.LogDebug("HOOO form: {FormName} Player state: {PlayerState}", _currentPanel.UIControl.Name, _player.State);

                    if (_player.State == PlayerState.Paused)
                    {
                        _logger.LogDebug("{PanelName}: Player is paused. Returning", _currentPanel.Name);
                        return;
                    }

                    if (switchObj.IsSelectTriggerSwitch())
                    {
                        Widget widget = _player.HighlightedWidget;
                        if (widget != null)
                        {
                            _logger.LogDebug("Actuate. widgetname: {WidgetName} Text: {WidgetText}", widget.Name, widget.GetText());
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

                _logger.LogDebug("Player state is {PlayerState}", _player.State);
                if (_player.State != PlayerState.Running)
                {
                    _logger.LogDebug("{PanelName}: Player is not Running. Returning", _currentPanel.Name);
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

                    _logger.LogDebug("{PanelName}: Switch on {WidgetName} type: {WidgetType}", _currentPanel.Name, highlightedWidget.UIWidget.Name, highlightedWidget.UIWidget.GetType().Name);

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

                    //Log.Debug("AP1: _player.State is " + _player.State);
                }
                else
                {
                    _logger.LogDebug("{PanelName}: No current animation or highlighed widget!!", _currentPanel.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in actuatorManager_EvtSwitchActivated");
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

            Control form = _currentPanel.UIControl;
            if (form is IScannerPanel)
            {
                IPanelCommon panelCommon = (form as IScannerPanel).PanelCommon;
                var arg = new CommandEnabledArg(null, onTrigger);
                panelCommon.CheckCommandEnabled(new CommandEnabledArg(null, onTrigger));

                if (arg.Handled)
                {
                    if (!arg.Enabled)
                    {
                        _logger.LogDebug("Command {Command} is not currently enabled", onTrigger);
                        return;
                    }
                    else
                    {
                        _logger.LogDebug("Command {Command} IS ENABLED", onTrigger);
                    }
                }
                else
                {
                    _logger.LogDebug("arg.handled is false for {Command}", onTrigger);

                    CmdDescriptor cmdDescriptor = CommandManager.Instance.AppCommandTable.Get(onTrigger);
                    if (cmdDescriptor != null && !cmdDescriptor.EnableSwitchMap)
                    {
                        _logger.LogDebug("EnableswitchMap is not enabled for {Command}", onTrigger);
                        runCommand = false;
                    }
                }
            }
            else
            {
                _logger.LogDebug("Dialog is active. Will not handle");
                runCommand = false;
            }

            if (runCommand)
            {
                _logger.LogDebug("Executing OnTrigger command {Command} for panel...{PanelName}", onTrigger, _currentPanel.Name);
                PCode pcode = new() { Script = "run(" + onTrigger + ")" };
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