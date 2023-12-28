////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Lib.Core.UserControlManagement
{
    /// <summary>
    /// This is a helper class for all usercontrols.  It contains functions
    /// that are common across all usercontrol and using this avoids duplication
    /// on code, consistency in how events are handled and makes it easier
    /// for the developer to add new scanners to ACAT
    /// The usercontrol should contain a field that returns a UserControlKeyboardCommon
    /// object and call functions in this class at the appropriate points in the
    /// usercontrol.  The documentation for this class contains info on where each
    /// of the functions need to be invoked.
    /// </summary>
    public class UserControlKeyboardCommon : IDisposable, IUserControlCommon
    {
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
        private AnimationManager2 _animationManager;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Is the scanner paused?
        /// </summary>
        private bool _isPaused;

        private readonly UserControlConfigMapEntry _mapEntry;

        /// <summary>
        /// is scanner in preview mode?
        /// </summary>
        private bool _previewMode;

        /// <summary>
        /// The widget represnting the window
        /// </summary>
        private Widget _rootWidget;

        private readonly IUserControl _userControl;

        /// <summary>
        /// The widget manager object.  Maintains a list of all
        /// widgets in this window
        /// </summary>
        private WidgetManager _widgetManager;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="iScannerPanel">The scanner Form object</param>
        public UserControlKeyboardCommon(IUserControl userControl, UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel iScannerPanel)
        {
            ScannerForm = iScannerPanel.Form;
            _mapEntry = mapEntry;
            _scannerPanel = iScannerPanel;
            TextController = textController;
            ConfigId = mapEntry.ConfigId;
            _syncLock = new SyncLock();
            _userControl = userControl;
        }

        /// <summary>
        /// Gets the widget that was actuated as a result of one
        /// of the actuator switches trigerring
        /// </summary>
        public Widget ActuatedWidget { get; private set; }

        /// <summary>
        /// Gets the Animation Manager object
        /// </summary>
        public AnimationManager2 AnimationManager
        { get { return _animationManager; } }

        /// <summary>
        /// Gets the panel config id for this scanner
        /// </summary>
        public Guid ConfigId { get; private set; }

        public int GridScanIterations { get; set; }

        /// <summary>
        /// Gets the paused state of the form
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
        }

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
            }
        }

        /// <summary>
        /// Gets the widget that reprensents the form
        /// </summary>
        public Widget RootWidget
        { get { return _rootWidget; } }

        /// <summary>
        /// Gets the parent scanner form
        /// </summary>
        public Form ScannerForm { get; private set; }

        /// <summary>
        /// Gets the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncLock; }
        }

        /// <summary>
        /// Helps with auto completing words, handling abbreviations,
        /// spell check etc.
        /// </summary>
        public TextController TextController { get; private set; }

        /// <summary>
        /// Gets the WidgetManager objet
        /// </summary>
        public WidgetManager WidgetManager
        { get { return _widgetManager; } }

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
        /// Check to see if a command should be enabled or not.
        /// This depends on the context.   The arg parameter
        /// contains the widget/command object in question.
        /// </summary>
        /// <param name="arg">Argument</param>
        public void CheckCommandEnabled(CommandEnabledArg arg)
        {
            if (_syncLock.IsClosing())
            {
                return;
            }

            _scannerPanel.CheckCommandEnabled(arg);

            if (!arg.Handled)
            {
                Context.AppAgentMgr.CheckCommandEnabled(arg);
            }
        }

        public void Close()
        {
            OnFormClosing(null);

            OnClosing();
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
        /// Performs initialization.  Reads the config file for the form, creates
        /// the animation manager, widget manager, loads in  all the widgets,
        /// subscribes to events
        /// Call this function in the Initialize() function in the scanner.
        /// </summary>
        /// <param name="startupArg"></param>
        /// <returns></returns>
        public bool Initialize()
        {
            Log.Debug("Entered from Initialize");

            bool retVal = initWidgetManager(_mapEntry);

            if (retVal)
            {
                retVal = initAnimationManager(_mapEntry);
            }

            Log.Debug("Returning from Initialize " + retVal);

            WindowActivityMonitor.EvtWindowMonitorHeartbeat += WindowActivityMonitor_EvtWindowMonitorHeartbeat;

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

                _animationManager = null;

                _rootWidget.Dispose();

                _rootWidget = null;


                _widgetManager.Dispose();

                _widgetManager = null;
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
        /// <param name="monitorInfo">info about the active application</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            setWidgetEnabledStates(monitorInfo);
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

            //TextController.OnClosing();

            unsubscribeEvents();

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
            //TextController.OnLoad();

            subscribeToEvents();

            setWidgetEnabledStates(WindowActivityMonitor.GetForegroundWindowInfo());
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
                AnimationManager.Pause();
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
                //AnimationManager.Resume();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
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
                    // dispose all managed resources.
                    _widgetManager?.Dispose();

                    _animationManager?.Dispose();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        /// <summary>
        /// User actuated a button on the form.  Handles the event.
        /// </summary>
        /// <param name="widget">Widget to acutate</param>
        private void actuateButton(Widget widget)
        {
            ActuatedWidget = null;

            if (_isPaused ||
                widget is WordListItemWidget ||
                String.IsNullOrEmpty(widget.Value) ||
                !(widget is IButtonWidget))
            {
                return;
            }

            var button = (IButtonWidget)widget;

            ActuatedWidget = widget;

            if (widget.IsCommand)
            {
                runCommand(widget.Command);
            }
            else if (button.GetWidgetAttribute().IsVirtualKey)
            {
                actuateVirtualKey(button.GetWidgetAttribute(), widget.Value);
            }
            else if (!Context.AppAgentMgr.TextChangedNotifications.OnHold())
            {
                if (widget.Value.Length > 1)
                {
                    CoreGlobals.Stopwatch1.Reset();
                    CoreGlobals.Stopwatch1.Start();
                    Context.AppAgentMgr.TextChangedNotifications.Hold();
                    SendKeys.SendWait(widget.Value + " ");
                    Context.AppAgentMgr.TextChangedNotifications.Release();
                    CoreGlobals.Stopwatch1.Stop();
                    Log.Debug("TimeElapsed 1: " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);
                }
                else
                {
                    CoreGlobals.Stopwatch1.Reset();
                    CoreGlobals.Stopwatch1.Start();

                    actuateKey(button.GetWidgetAttribute(), widget.Value[0]);

                    CoreGlobals.Stopwatch1.Stop();
                    Log.Debug("TimeElapsed 2 : " + CoreGlobals.Stopwatch1.ElapsedMilliseconds);
                }
            }

            ActuatedWidget = null;
        }

        /// <summary>
        /// Handles actuation of a keyboard key (eg a-z, punctuations etc). Modifiers
        /// are optionally shift, ctrl, alt etc
        /// </summary>
        /// <param name="modifiers">modifier keys</param>
        /// <param name="value">String to send</param>
        private void actuateKey(WidgetAttribute widgetAttribute, char value)
        {
            Log.Debug(value.ToString());
            if (!TextController.HandlePunctuation(widgetAttribute.Modifiers, value))
            {
                if ((KeyStateTracker.IsShiftOn() || KeyStateTracker.IsCapsLockOn()) &&
                    !String.IsNullOrEmpty(widgetAttribute.ShiftValue))
                {
                    TextController.HandleAlphaNumericChar(widgetAttribute.ShiftValue[0]);
                }
                else
                {
                    TextController.HandleAlphaNumericChar(widgetAttribute.Modifiers, value);
                }
            }
        }

        /// <summary>
        /// Handles actuation of a button whose "virtualkey" attribute is set to
        /// true in the WidgetAtribute for the button in the panel config file.
        /// This means the key is a string representation of the Keys enum.
        /// </summary>
        /// <param name="widgetAttribute">widgetattribute of the button</param>
        /// <param name="value">value of the key</param>
        private void actuateVirtualKey(WidgetAttribute widgetAttribute, String value)
        {
            Log.Debug("VirtualKey: " + value);

            TextController.HandleVirtualKey(widgetAttribute.Modifiers, value);
        }

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
        /// Creates the animation manager and initialize it  Load
        /// all the animations from the config file.
        /// </summary>
        /// <param name="configmapEntrFileName">name of the animation file</param>
        /// <returns></returns>
        private bool initAnimationManager(UserControlConfigMapEntry panelConfigMapEntry)
        {
            bool retVal;

            _animationManager = new AnimationManager2();
            retVal = _animationManager.Init(panelConfigMapEntry);

            if (!retVal)
            {
                Log.Error("Error initializing animation manager");
                _animationManager = null;
            }

            return retVal;
        }

        /// <summary>
        /// Create the widget manager. Load all the widgets
        /// from the config file
        /// </summary>
        /// <param name="panelConfigMapEntry">config map entry for the panel</param>
        /// <returns>true on success</returns>
        private bool initWidgetManager(UserControlConfigMapEntry mapEntry)
        {
            _widgetManager = new WidgetManager(_userControl as Control);
            _widgetManager.Layout.SetColorScheme(ColorSchemes.ScannerSchemeName);
            _widgetManager.Layout.SetDisabledButtonColorScheme(ColorSchemes.DisabledScannerButtonSchemeName);

            bool retVal = _widgetManager.Initialize(mapEntry.ConfigFileName);
            if (!retVal)
            {
                Log.Error("Unable to initialize widget manager");
            }
            else
            {
                _rootWidget = _widgetManager.RootWidget;
                if (String.IsNullOrEmpty(_rootWidget.SubClass))
                {
                    _rootWidget.SubClass = (ScannerForm is MenuPanelBase) ?
                                            PanelCategory.Menu.ToString() :
                                            PanelCategory.Scanner.ToString();
                }
            }

            return retVal;
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
                if (!handled)
                {
                    dispatcher.Dispatch2(_userControl, command, ref handled);
                }
            }
        }

        /// <summary>
        /// Get all the widgets whose "enabled" property is set to
        /// contextual in the config file.  Decide the "enabled" state
        /// of these widgets depending on the context
        /// </summary>
        /// <param name="monitorInfo">Context info about active window</param>
        private void setWidgetEnabledStates(WindowActivityMonitorInfo monitorInfo)
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
                    //Log.Debug("Widget: " + widget.Name + ", subclass: " + widget.SubClass);
                    if (widget.IsCommand)
                    {
                        var arg = new CommandEnabledArg(monitorInfo, widget.Command, widget);
                        if (!_syncLock.IsClosing())
                        {
                            _scannerPanel.CheckCommandEnabled(arg);

                            if (!arg.Handled)
                            {
                                Context.AppAgentMgr.CheckCommandEnabled(arg);
                            }

                            widget.Enabled = arg.Handled ? arg.Enabled : widget.DefaultEnabled;
                        }
                        else
                        {
                            break;
                        }

                        Log.Debug("widget.Enabled set to : " + widget.Enabled + " for feature " + widget.SubClass);
                    }
                }
            }
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
            subscribeToButtonEvents();
        }

        /// <summary>
        /// Convers the indicated text to speech thorugh the
        /// active text-to-speech engine
        /// </summary>
        /// <param name="text">text to convert</param>
        private void textToSpeech(String text)
        {
            Log.Debug("Convert to speech. text=" + text);
            Context.AppTTSManager.ActiveEngine.Speak(text);
        }

        /// <summary>
        /// Unsubscribe from events previously subscribed to.
        /// </summary>
        private void unsubscribeEvents()
        {
        }

        /// <summary>
        /// The user actuated a widget.  perform the necessary
        /// action
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtActuated(object sender, WidgetActuatedEventArgs e)
        {
            if (PreviewMode || _isPaused)
            {
                return;
            }

            var widget = e.SourceWidget;

            bool handled = false;

            _userControl.OnWidgetActuated(e, ref handled);

            if (!handled)
            {
                _scannerPanel.OnWidgetActuated(e, ref handled);
            }

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
        /// Heartbeat handler from the windowactivitymonitor.  Uses this
        /// monitor to set the 'enabled' state of the widgets in the scanner
        /// based on the context.
        /// </summary>
        /// <param name="monitorInfo">active window context info</param>
        private void WindowActivityMonitor_EvtWindowMonitorHeartbeat(WindowActivityMonitorInfo monitorInfo)
        {
            try
            {
                setWidgetEnabledStates(monitorInfo);
            }
            catch
            {
            }
        }
    }
}