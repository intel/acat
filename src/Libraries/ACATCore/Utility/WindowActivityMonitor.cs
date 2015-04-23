////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowActivityMonitor.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.Audit;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Monitors the activity of the foreground window and notifies
    /// event subscribers if the focues of the active window changes,
    /// OR if the focus changes from one control to another inside
    /// the active window
    /// Also supports a heartbeat where subscribers can be periodically
    /// notified of the current focused window/ window element
    /// </summary>
    public class WindowActivityMonitor
    {
        /// <summary>
        /// Timer to track focus changes
        /// </summary>
        private static Timer _timer;

        /// <summary>
        /// Used to control whether the heartbeat will trigger
        /// an event or not
        /// </summary>
        private static bool _heartbeatToggle = true;

        /// <summary>
        /// How often to check for active window focus changes
        /// </summary>
        private const int Interval = 600;

        /// <summary>
        /// Currently active window
        /// </summary>
        private static IntPtr _currentHwnd = IntPtr.Zero;

        /// <summary>
        /// Automation element of the control that is currently in focus
        /// </summary>
        private static AutomationElement _currentFocusedElement;

        /// <summary>
        /// Normally events are raised only if something changes.  Set
        /// this to true to force raising an event even if nothing has changed
        /// </summary>
        private static volatile bool _forceGetActiveWindow;

        /// <summary>
        /// To prevent re-entrancy
        /// </summary>
        private static readonly object _timerSync = new object();

        /// <summary>
        /// For the event raised for activity monitoring
        /// </summary>
        /// <param name="monitorInfo">activity info</param>
        public delegate void ActivityMonitorDelegate(WindowActivityMonitorInfo monitorInfo);

        /// <summary>
        /// For the event raised when the focus changes
        /// </summary>
        /// <param name="element">currently focused element</param>
        public delegate void AutomationElementFocusChanged(AutomationElement element);

        /// <summary>
        /// Raised for heartbeat subscribers
        /// </summary>
        public static event ActivityMonitorDelegate EvtWindowMonitorHeartbeat;

        /// <summary>
        /// Raised when focus changes
        /// </summary>
        public static event ActivityMonitorDelegate EvtFocusChanged;

        /// <summary>
        /// Starts activity monitoring
        /// </summary>
        /// <returns>true</returns>
        public static bool Start()
        {
            if (_timer == null)
            {
                _timer = new Timer { Interval = Interval };
                _timer.Tick += _timer_Tick;
                _timer.Start();
            }

            return true;
        }

        /// <summary>
        /// Asyncrhonously forces an event to be raised
        /// regardless of whether focus changed or not
        /// </summary>
        public static void GetActiveWindowAsync()
        {
            _forceGetActiveWindow = true;
        }

        /// <summary>
        /// Synchronously raises an event to get info
        /// about the currently active window
        /// </summary>
        [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void GetActiveWindow()
        {
            getActiveWindow(true);
        }

        /// <summary>
        /// Pauses the activity monitoring.  No events
        /// will be raised when paused
        /// </summary>
        public static void Pause()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _currentHwnd = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Resumes window activity monitoring.
        /// </summary>
        public static void Resume()
        {
            try
            {
                if (_timer != null)
                {
                    _timer.Start();
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
        public static void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        /// <summary>
        /// Returns information of the currently focused window
        /// such as the window title, the element inside the window
        /// that is currently focused etc
        /// </summary>
        /// <returns>window monitor info</returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static WindowActivityMonitorInfo GetForegroundWindowInfo()
        {
            const int maxTries = 3;

            var monitorInfo = new WindowActivityMonitorInfo();

            // the reason we try a few times is because UI
            // automation sometimes throws an exception depending
            // the state of the focused window.
            for (int ii = 0; ii < maxTries; ii++)
            {
                try
                {
                    monitorInfo.FgHwnd = Windows.GetForegroundWindow();
                    monitorInfo.FocusedElement = AutomationElement.FocusedElement;
                    monitorInfo.Title = Windows.GetWindowTitle(monitorInfo.FgHwnd);
                    monitorInfo.FgProcess = GetProcessForWindow(monitorInfo.FgHwnd);
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(100);
                }
            }

            return monitorInfo;
        }

        /// <summary>
        /// Gets the parent process that owns the specified window handle
        /// </summary>
        /// <param name="hwnd">window handle</param>
        /// <returns>parent process</returns>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static Process GetProcessForWindow(IntPtr hwnd)
        {
            int pid;

            GetWindowThreadProcessId(hwnd, out pid);

            return Process.GetProcessById(pid);
        }

        /// <summary>
        /// Compares the two automation elements and returns if
        /// they are identical or not
        /// </summary>
        /// <param name="ele1">first element</param>
        /// <param name="ele2">second element</param>
        /// <returns></returns>
        public static bool IsDifferent(AutomationElement ele1, AutomationElement ele2)
        {
            bool retVal;
            if (ele1 == null || ele2 == null)
            {
                return true;
            }

            try
            {
                retVal = !Automation.Compare(ele1.GetRuntimeId(), ele2.GetRuntimeId());
            }
            catch
            {
                retVal = true;
            }

            Log.Debug(retVal ? "YES" : "NO");
            return retVal;
        }

        /// <summary>
        /// The timer function to check the currently focused window
        /// and to see if focus changed or not
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private static void _timer_Tick(object sender, EventArgs e)
        {
            // prevent re-entrancy
            if (!tryEnter(_timerSync))
            {
                return;
            }

            getActiveWindow();

            release(_timerSync);
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        private static void getActiveWindow(bool flag = false)
        {
            AutomationElement focusedElement = null;

            try
            {
                IntPtr foregroundWindow = Windows.GetForegroundWindow();
                var title = Windows.GetWindowTitle(foregroundWindow);

                Log.Debug("fgHwnd = " + ((foregroundWindow != IntPtr.Zero) ? foregroundWindow.ToString() : "null") + ", title: " + title);

                focusedElement = AutomationElement.FocusedElement;

                Log.Debug("focusedElement is " + ((focusedElement != null) ? "not null" : "null"));
                Log.Debug("_currentfocusedElement is " + ((_currentFocusedElement != null) ? "not null" : "null"));

                bool elementChanged = true;

                var process = GetProcessForWindow(foregroundWindow);

                // check if anything changed. did the window focus change?
                // did focus change within the window?
                if (focusedElement != null &&
                    (_forceGetActiveWindow || flag || foregroundWindow != _currentHwnd || _currentFocusedElement == null ||
                    (elementChanged = IsDifferent(focusedElement, _currentFocusedElement))))
                {
                    //Log.Debug("Reason: _forceGetActiveWindow: " + _forceGetActiveWindow);
                    //Log.Debug("Reason: flag: " + flag);
                    //Log.Debug("Reason: fgHwnd != _currentHwnd : " + (fgHwnd != _currentHwnd));
                    //Log.Debug("Reason: _currentFocusedElement == null : " + (_currentFocusedElement == null));
                    //Log.Debug("Reason: elementChanged : " + elementChanged);

                    _forceGetActiveWindow = false;

                    if (EvtFocusChanged != null)
                    {
                        var monitorInfo = new WindowActivityMonitorInfo
                        {
                            FgHwnd = foregroundWindow,
                            Title = title,
                            FgProcess = process,
                            FocusedElement = focusedElement,
                            IsNewWindow = _currentHwnd != foregroundWindow
                        };

                        if (flag)
                        {
                            monitorInfo.IsNewWindow = true;
                        }

                        if (monitorInfo.IsNewWindow || _currentFocusedElement == null || elementChanged)
                        {
                            monitorInfo.IsNewFocusedElement = true;
                        }
#if abc
                        Log.Debug("#$#>>>>>>>>>>>>>>>> Triggering FOCUS changed event");

                        Log.Debug("#$#    title: " + title);
                        Log.Debug("#$#    fgHwnd " + fgHwnd);
                        Log.Debug("#$#    nativewinhandle: " + focusedElement.Current.NativeWindowHandle);
                        Log.Debug("#$#    Process " + process.ProcessName);
                        Log.Debug("#$#    class: " + focusedElement.Current.ClassName);
                        Log.Debug("#$#    controltype:  " + focusedElement.Current.ControlType.ProgrammaticName);
                        Log.Debug("#$#    automationid: " + focusedElement.Current.AutomationId);
                        Log.Debug("#$#    newWindow: " + monitorInfo.IsNewWindow);
                        Log.Debug("#$#    newFocusElement: " + monitorInfo.IsNewFocusedElement);
                        Log.Debug("#$#    IsMinimized :  " + Windows.IsMinimized(monitorInfo.FgHwnd));
#endif
                        if (monitorInfo.IsNewWindow)
                        {
                            AuditLog.Audit(new AuditEventActiveWindowChange(process.ProcessName, title));
                        }

                        if (EvtFocusChanged != null)
                        {
                            EvtFocusChanged(monitorInfo);
                        }

                        _currentFocusedElement = focusedElement;
                    }
                    else
                    {
                        Log.Debug("EvtFocusChanged is null");
                    }
                }

                _currentHwnd = foregroundWindow;

                // raise the heartbeat event
                if (EvtWindowMonitorHeartbeat != null && focusedElement != null && _heartbeatToggle)
                {
                    var monitorInfo = new WindowActivityMonitorInfo
                    {
                        FgHwnd = foregroundWindow,
                        FocusedElement = focusedElement,
                        Title = title,
                        FgProcess = process
                    };
                    EvtWindowMonitorHeartbeat(monitorInfo);
                }

                _heartbeatToggle = !_heartbeatToggle;
            }
            catch (Exception e)
            {
                Log.Debug("exception: " + e.ToString());
                _currentFocusedElement = null;
            }

            //Log.Debug("*** EXIT ***");
        }

        /// <summary>
        /// Returns the parent process for the focused element
        /// </summary>
        /// <param name="focusedElement">element that has focus</param>
        /// <returns>the parent process</returns>
        private static Process getActiveProcess(AutomationElement focusedElement)
        {
            Process process = null;

            int pid = (int)focusedElement.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            if (pid != 0)
            {
                process = Process.GetProcessById(pid);
                Log.Debug("Active process is " + process.ProcessName + " MainWindowHandle: " + process.MainWindowHandle);
            }

            return process;
        }

        /// <summary>
        /// Uses Monitor to see if it can enter
        /// </summary>
        /// <param name="syncObj">synchronization object</param>
        /// <returns>true if it entered</returns>
        private static bool tryEnter(Object syncObj)
        {
            bool lockTaken = false;
            System.Threading.Monitor.TryEnter(syncObj, ref lockTaken);
            return lockTaken;
        }

        /// <summary>
        /// Releases the synch object
        /// </summary>
        /// <param name="syncObj">synch object</param>
        private static void release(Object syncObj)
        {
            try
            {
                System.Threading.Monitor.Exit(syncObj);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        [DllImportAttribute("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        private static extern int GetWindowThreadProcessId([InAttribute()] IntPtr handle, out int lpdwProcessId);
    }
}