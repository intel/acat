//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//

using ACAT.Core.AgentManagement;
using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.ThemeManagement;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using ACAT.Core.Widgets;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Core.UserControlManagement
{
    public class UserControlCommon : IUserControlCommon, IDisposable
    {
        public UserControlCommon(IUserControl userControl, UserControlConfigMapEntry mapEntry, IScannerPanel iScannerPanel)
        {
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
            Log.Debug("Entered from Initialize");

            bool retVal = initWidgetManager(mapEntry);

            if (retVal)
            {
                retVal = initAnimationManager(mapEntry);
            }

            Log.Debug("Returning from Initialize " + retVal);

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
                Log.Exception(ex);
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

            foreach (var widget in buttonList)
            {
                if (widget is IButtonWidget)
                {
                    widget.EvtActuated -= widgetEvtActuated;
                }
            }

            Log.Debug(ScannerForm.Name + ", SyncObj.Status: " + SyncLock.Status + ", hashcode: " + SyncLock.GetHashCode());

            if (SyncLock.Status != SyncLock.StatusValues.None)
            {
                Log.Debug(ScannerForm.Name + ", SyncObj.Status: " + SyncLock.Status + ", form already closed.  returning");
                return;
            }

            Log.Debug(ScannerForm.Name + ", SyncObj.Status: " + SyncLock.Status + ", Will continue closing");

            Log.Debug("Setting SyncLock.Status to CLOSING " + ScannerForm.Name);
            SyncLock.Status = SyncLock.StatusValues.Closing;

            Log.Debug("Before animationmangoer.stop");
            AnimationManager.Stop();

            Log.Debug("After animationmangoer.stop");

            Log.Debug("Unsubscribe to EvtHeartbeat for " + ScannerForm.Name);
            WindowActivityMonitor.EvtWindowMonitorHeartbeat -= WindowActivityMonitorEvtWindowMonitorHeartbeat;
            Log.Debug("Unsubscribe to EvtHeartbeat DONE for " + ScannerForm.Name);

            //TextController.OnClosing();

            unsubscribeEvents();

            Log.Debug("Exiting FormClosing for " + ScannerForm.Name);
        }

        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public void OnLoad(bool resetTalkWindowPosition = true)
        {
            //TextController.OnLoad();

            subscribeToEvents();

            setWidgetEnabledStates(WindowActivityMonitor.GetForegroundWindowInfo());
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
                Log.Exception(ex.ToString());
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
                Log.Exception(ex.ToString());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                Log.Verbose();

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
            retVal = AnimationManager.Init(panelConfigMapEntry);

            if (!retVal)
            {
                Log.Error("Error initializing animation manager");
                AnimationManager = null;
            }

            return retVal;
        }

        private bool initWidgetManager(UserControlConfigMapEntry mapEntry)
        {
            WidgetManager = new WidgetManager(UserControl as Control);
            WidgetManager.Layout.SetColorScheme(ColorSchemes.ScannerSchemeName);
            WidgetManager.Layout.SetDisabledButtonColorScheme(ColorSchemes.DisabledScannerButtonSchemeName);

            bool retVal = WidgetManager.Initialize(mapEntry.ConfigFileName);
            if (!retVal)
            {
                Log.Error("Unable to initialize widget manager");
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

        private void runCommandAgent(String command, ref bool handled)
        {
            Context.AppAgentMgr.RunCommand(command, null, ref handled);
        }

        private void runCommandScanner(String command, ref bool handled)
        {
            var dispatcher = ScannerPanel.CommandDispatcher;
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
                Log.Debug("Form is closing " + ScannerForm.Name);
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

                        Log.Verbose($"widget.Enabled set to: {widget.Enabled} for feature {widget.Name}");
                    }
                }
            }
        }

        private void subscribeToButtonEvents()
        {
            var buttonList = new List<Widget>();
            RootWidget.Finder.FindAllChildren(buttonList);

            foreach (var widget in buttonList)
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
            Log.Debug("Convert to speech. text=" + text);
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

            var widget = e.SourceWidget;

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
                    Log.Debug("**Actuate** " + widget.Name + " Value: " + value);

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