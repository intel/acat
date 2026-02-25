//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//

using ACAT.Core.AgentManagement;
using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.ThemeManagement;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using ACAT.Core.Widgets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Core.UserControlManagement
{
    public class UserControlCommon : IUserControlCommon, IDisposable
    {
        private readonly ILogger<UserControlCommon> _logger;

        public UserControlCommon(IUserControl userControl, UserControlConfigMapEntry mapEntry, IScannerPanel iScannerPanel, ILogger<UserControlCommon> logger = null)
        {
            _logger = logger ?? LogManager.GetLogger<UserControlCommon>();
            ScannerForm = iScannerPanel.Form;
            this.mapEntry = mapEntry;
            ScannerPanel = iScannerPanel;
            configId = mapEntry.ConfigId;
            SyncLock = new SyncLock();
            UserControl = userControl;
        }

        public Widget ActuatedWidget { get; set; }
        public UserControlAnimationManager AnimationManager { get; private set; }
        public Guid configId { get; private set; }
        public int gridScanIterations { get; set; }
        public bool isPaused { get; private set; }
        public bool previewMode { get; set; }
        public Widget RootWidget { get; private set; }
        public Form ScannerForm { get; private set; }
        public SyncLock SyncObj
        {
            get { 
                return this.SyncLock; 
            } 
        }
        public WidgetManager WidgetManager { get; private set; }
        private bool disposed { get; set; }
        private UserControlConfigMapEntry mapEntry { get; }
        public IScannerPanel ScannerPanel { get; }

        public AgentManager AppAgentMgr => Context.AppAgentMgr;

        private SyncLock SyncLock { get; }

        private IUserControl UserControl { get; }

        public void CheckCommandEnabled(CommandEnabledArg arg)
        {
            if (SyncLock.IsClosing())
            {
                return;
            }

            ScannerPanel.CheckCommandEnabled(arg);

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

        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        public bool Initialize()
        {
            _logger.LogDebug("Entered from Initialize");

            bool retVal = initWidgetManager(mapEntry);

            if (retVal)
            {
                retVal = initAnimationManager(mapEntry);
            }

            _logger.LogDebug("Returning from Initialize {RetVal}", retVal);

            WindowActivityMonitor.EvtWindowMonitorHeartbeat += WindowActivityMonitorEvtWindowMonitorHeartbeat;

            return retVal;
        }

        public void OnClosing()
        {
            try
            {
                AnimationManager.Dispose();
                AnimationManager = null;

                RootWidget.Dispose();
                RootWidget = null;

                WidgetManager.Dispose();
                WidgetManager = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in OnClosing");
            }
        }

        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            setWidgetEnabledStates(monitorInfo);
        }

        public void OnFormClosing(FormClosingEventArgs e)
        {
            var buttonList = new List<Widget>();
            RootWidget.Finder.FindAllChildren(buttonList);

            foreach (Widget widget in buttonList)
            {
                if (widget is IButtonWidget)
                {
                    widget.EvtActuated -= widgetEvtActuated;
                }
            }

            _logger.LogDebug("{ScannerFormName}, SyncObj.Status: {Status}, hashcode: {HashCode}", ScannerForm.Name, SyncLock.Status, SyncLock.GetHashCode());

            if (SyncLock.Status != SyncLock.StatusValues.None)
            {
                _logger.LogDebug("{ScannerFormName}, SyncObj.Status: {Status}, form already closed.  returning", ScannerForm.Name, SyncLock.Status);
                return;
            }

            _logger.LogDebug("{ScannerFormName}, SyncObj.Status: {Status}, Will continue closing", ScannerForm.Name, SyncLock.Status);

            _logger.LogDebug("Setting SyncLock.Status to CLOSING {ScannerFormName}", ScannerForm.Name);
            SyncLock.Status = SyncLock.StatusValues.Closing;

            _logger.LogDebug("Before animationmangoer.stop");
            AnimationManager.Stop();

            _logger.LogDebug("After animationmangoer.stop");

            _logger.LogDebug("Unsubscribe to EvtHeartbeat for {ScannerFormName}", ScannerForm.Name);
            WindowActivityMonitor.EvtWindowMonitorHeartbeat -= WindowActivityMonitorEvtWindowMonitorHeartbeat;
            _logger.LogDebug("Unsubscribe to EvtHeartbeat DONE for {ScannerFormName}", ScannerForm.Name);

            unsubscribeEvents();

            _logger.LogDebug("Exiting FormClosing for {ScannerFormName}", ScannerForm.Name);
        }

        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnLoad(bool resetTalkWindowPosition = true)
        {
            subscribeToEvents();

            setWidgetEnabledStates(WindowActivityMonitor.CurrentWindowInfo());
        }

        public void OnPause()
        {
            if (isPaused)
            {
                return;
            }

            isPaused = true;

            try
            {
                AnimationManager.Pause();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in OnPause");
            }
        }

        public void OnResume()
        {
            if (!isPaused)
            {
                return;
            }

            isPaused = false;

            try
            {
                //PanelAnimationManager.Resume();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in OnResume");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                _logger.LogTrace("Disposing UserControlCommon");

                if (disposing)
                {
                    // dispose all managed resources.
                    WidgetManager?.Dispose();

                    AnimationManager?.Dispose();
                }

                // Release unmanaged resources.
            }

            disposed = true;
        }

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        protected virtual void actuateButton(Widget widget)
        {
            ActuatedWidget = null;

            if (isPaused ||
                widget is WordListItemWidget ||
                String.IsNullOrEmpty(widget.Value) ||
                widget is not IButtonWidget)
            {
                return;
            }

            ActuatedWidget = widget;

            if (widget.IsCommand)
            {
                runCommand(widget.Command);
            }

            ActuatedWidget = null;
        }

        private bool initAnimationManager(UserControlConfigMapEntry panelConfigMapEntry)
        {
            bool retVal;

            AnimationManager = new UserControlAnimationManager();

            // Wire up the new animation engine when available via DI.
            // AnimationService is null when DI is not configured; the manager
            // falls back to the legacy AnimationPlayer transparently.
            AnimationManager.AnimationService = Context.AppAnimationService;

            retVal = AnimationManager.Init(panelConfigMapEntry);

            if (!retVal)
            {
                _logger.LogError("Error initializing animation manager");
                AnimationManager = null;
            }

            return retVal;
        }

        private bool initWidgetManager(UserControlConfigMapEntry mapEntry)
        {
            WidgetManager = new WidgetManager(UserControl as Control, LogManager.GetLogger<WidgetManager>());
            WidgetManager.Layout.SetColorScheme(ColorSchemes.ScannerSchemeName);
            WidgetManager.Layout.SetDisabledButtonColorScheme(ColorSchemes.DisabledScannerButtonSchemeName);

            bool retVal = WidgetManager.Initialize(mapEntry.ConfigFileName);
            if (!retVal)
            {
                _logger.LogError("Unable to initialize widget manager");
            }
            else
            {
                RootWidget = WidgetManager.RootWidget;
                if (String.IsNullOrEmpty(RootWidget.SubClass))
                {
                    RootWidget.SubClass = (ScannerForm is MenuPanelBase) ?
                                            PanelCategory.Menu.ToString() :
                                            PanelCategory.Scanner.ToString();
                }
            }

            return retVal;
        }

        public void runCommand(String command)
        {
            bool handled = false;

            if (command[0] == '@')
            {
                command = command.Substring(1);
            }

            _logger.LogDebug("Calling scanner common runcomand with {Command}", command);
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

        private void runCommandAgent(String command, ref bool handled)
        {
            Context.AppAgentMgr.RunCommand(command, null, ref handled);
        }

        private void runCommandScanner(String command, ref bool handled)
        {
            RunCommandDispatcher dispatcher = ScannerPanel.CommandDispatcher;
            if (dispatcher != null)
            {
                dispatcher.Dispatch(command, ref handled);
                if (!handled)
                {
                    dispatcher.Dispatch(UserControl, command, ref handled);
                }
            }
        }

        private void setWidgetEnabledStates(WindowActivityMonitorInfo monitorInfo)
        {
            if (SyncLock.IsClosing())
            {
                _logger.LogDebug("Form is closing {ScannerFormName}", ScannerForm.Name);
                WindowActivityMonitor.EvtWindowMonitorHeartbeat -= WindowActivityMonitorEvtWindowMonitorHeartbeat;
                return;
            }

            if (RootWidget != null && Context.AppAgentMgr != null && !SyncLock.IsClosing() && Windows.GetVisible(ScannerForm))
            {
                foreach (Widget widget in RootWidget.WidgetLayout.ContextualWidgets)
                {
                    //Log.Debug("Widget: " + widget.Name + ", subclass: " + widget.SubClass);
                    if (widget.IsCommand)
                    {
                        var arg = new CommandEnabledArg(monitorInfo, widget.Command, widget);
                        if (!SyncLock.IsClosing())
                        {
                            ScannerPanel.CheckCommandEnabled(arg);

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

                        _logger.LogTrace("widget.Enabled set to: {Enabled} for feature {WidgetName}", widget.Enabled, widget.Name);
                    }
                }
            }
        }

        private void subscribeToButtonEvents()
        {
            var buttonList = new List<Widget>();
            RootWidget.Finder.FindAllChildren(buttonList);

            foreach (Widget widget in buttonList)
            {
                if (widget is IButtonWidget)
                {
                    widget.EvtActuated += widgetEvtActuated;
                }
            }
        }

        private void subscribeToEvents()
        {
            subscribeToButtonEvents();
        }

        private void textToSpeech(String text)
        {
            _logger.LogDebug("Convert to speech. text={Text}", text);
            Context.AppTTSManager.ActiveEngine.Speak(text);
        }

        private void unsubscribeEvents()
        {
        }

        private void widgetEvtActuated(object sender, WidgetActuatedEventArgs e)
        {
            if (previewMode || isPaused)
            {
                return;
            }

            Widget widget = e.SourceWidget;

            bool handled = false;

            UserControl.OnWidgetActuated(e, ref handled);

            if (!handled)
            {
                ScannerPanel.OnWidgetActuated(e, ref handled);
            }

            if (!handled && widget is IButtonWidget)
            {
                var value = widget.Value;
                if (!String.IsNullOrEmpty(value))
                {
                    _logger.LogDebug("**Actuate** {WidgetName} Value: {Value}", widget.Name, value);

                    actuateButton(widget);
                }
            }
        }

        private void WindowActivityMonitorEvtWindowMonitorHeartbeat(WindowActivityMonitorInfo monitorInfo)
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