using ACAT.Core.ActuatorManagement;
using ACAT.Core.AgentManagement;
using ACAT.Core.Audit;
using ACAT.Core.CommandManagement;
using ACAT.Core.Interpreter;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;

namespace ACAT.Core.AnimationManagement
{
    public class PanelAnimationManager : AnimationManager, IPanelAnimationManager
    {
        private String _panelClass = String.Empty;

        private PanelConfigMapEntry _panelConfigMapEntry;

        public PanelAnimationManager() : base() { }

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
        /// Transition to the target animation named 'animationName'
        /// </summary>
        /// <param name="animationName">Name of the animation to transition to</param>
        public override void TransitionFromName(String animationName)
        {
            try
            {
                Log.Verbose();

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
                Log.Exception(ex.ToString());
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
                Log.Exception(ex.ToString());
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
    }
}