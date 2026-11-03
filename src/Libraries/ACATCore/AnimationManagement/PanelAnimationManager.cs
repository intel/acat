using ACAT.Core.ActuatorManagement;
using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.ActuatorManagement.Settings;
using ACAT.Core.AgentManagement;
using ACAT.Core.AnimationManagement.Interfaces;
using ACAT.Core.Audit;
using ACAT.Core.CommandManagement;
using ACAT.Core.Interpreter;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.PanelManagement.PanelConfig;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace ACAT.Core.AnimationManagement
{
    public class PanelAnimationManager : AnimationManager, IPanelAnimationManager
    {
        private readonly ILogger<PanelAnimationManager> _logger;

        private String _panelClass = String.Empty;

        private PanelConfigMapEntry _panelConfigMapEntry;

        public PanelAnimationManager(ILogger<PanelAnimationManager> logger) : base()
        {
            _logger = logger;
        }

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

            _logger.LogDebug("returning from Anim manager init()");

            return retVal;
        }

        public void Start(Widget panelWidget, String animationName = null)
        {
            _logger.LogDebug("Start animation for panel {PanelName}", panelWidget.Name);

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
            Animations animations = getAnimations(animationName);

            if (!CoreGlobals.AppPreferences.EnableAutoStartScan)
            {
                Transition();
            }
            else
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

                Transition(firstAnimation);
            }
        }

        /// <summary>
        /// Transition to the target animation named 'animationName'
        /// </summary>
        /// <param name="animationName">Name of the animation to transition to</param>
        public override void TransitionFromName(String animationName)
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
                _player.Transition(animation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExceptionMessage}", ex.Message);
            }
        }

        protected override void actuatorManager_EvtSwitchDown(object sender, ActuatorSwitchEventArgs e)
        {
            base.actuatorManager_EvtSwitchDown(sender, e);
        }


        protected override void actuatorManager_EvtSwitchActivated(object sender, ActuatorSwitchEventArgs e)
        {
            IActuatorSwitch switchObj = e.SwitchObj;
            try
            {
                if (_currentPanel == null)
                {
                    return;
                }

                _logger.LogDebug("switch: {SwitchName}", switchObj.Name);
                _logger.LogDebug("   Panel: {PanelName}", _currentPanel.Name);

                if (_currentPanel.UIControl is System.Windows.Forms.Form)
                {
                    bool visible = Windows.GetVisible(_currentPanel.UIControl);
                    _logger.LogDebug("Form: {FormName}, visible: {Visible}", _currentPanel.UIControl.Name, visible);
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

                if (_player == null)
                {
                    if (String.Compare(onTrigger, SwitchSetting.TriggerCommand, true) != 0)
                    {
                        runSwitchMappedCommand(switchObj);
                    }
                    return;
                }

                _logger.LogDebug("playerState: {PlayerState}", _player.State);

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

                if (_player.State == PlayerState.Timeout || _player.State == PlayerState.Interrupted)
                {
                    _logger.LogDebug("Calling player transition for firstanimation");
                    _player.Transition(_firstAnimation);
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

                    _logger.LogDebug("{PanelName}: Switch on {WidgetName} type: {WidgetType}",
                        _currentPanel.Name,
                        highlightedWidget.UIWidget.Name,
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
                    _logger.LogDebug("{PanelName}: No current animation or highlighed widget!!", _currentPanel.Name);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ExceptionMessage}", ex.Message);
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

                    var strTrigger = onTrigger;
                    if (strTrigger[0] == '@')
                    {
                        strTrigger = strTrigger.Substring(1);
                    }
                    CmdDescriptor cmdDescriptor = CommandManager.Instance.AppCommandTable.Get(strTrigger);
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
    }
}