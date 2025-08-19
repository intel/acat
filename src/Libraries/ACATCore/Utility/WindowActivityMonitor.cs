// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Core.Utility
{
    public class WindowActivityMonitor
    {
        private const int Interval = 600;
        private static readonly object _timerSync = new();
        private static AutomationElement _currentFocusedElement;
        private static IntPtr _currentHwnd = IntPtr.Zero;
        private static volatile bool _forceGetActiveWindow;
        private static bool _heartbeatToggle = true;
        private static Timer _timer;
        private static Form _form;
        private static volatile bool _isPaused = false;

        public delegate void ActivityMonitorDelegate(WindowActivityMonitorInfo monitorInfo);
        public static event ActivityMonitorDelegate EvtFocusChanged;
        public static event ActivityMonitorDelegate EvtWindowMonitorHeartbeat;

        public static void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }

        public static bool Start()
        {
            if (_form == null)
            {
                _form = new Form { Visible = false };
                _form.Show();
                _form.Visible = false;
            }

            if (_timer == null)
            {
                _form.Invoke(new MethodInvoker(() =>
                {
                    _timer = new Timer { Interval = Interval };
                    _timer.Tick += Timer_Tick;
                    _timer.Start();
                }));
            }

            return true;
        }

        public static void Pause()
        {
            _isPaused = true;
            _timer?.Stop();
            _currentHwnd = IntPtr.Zero;
        }

        public static void Resume()
        {
            _isPaused = false;
            _timer?.Start();
        }

        private static void Timer_Tick(object sender, EventArgs e)
        {
            if (_isPaused) return;
            if (!TryEnter(_timerSync)) return;

            try
            {
                GetActiveWindowInternal();
            }
            finally
            {
                Release(_timerSync);
            }
        }

        private static void GetActiveWindowInternal()
        {
            var windowInfo = GetForegroundWindowInfo();
            if (windowInfo == null) return;

            bool elementChanged = IsDifferent(windowInfo.FocusedElement, _currentFocusedElement);

            // Raise focus changed event only if focus actually changed
            if (_forceGetActiveWindow || elementChanged || windowInfo.FgHwnd != _currentHwnd)
            {
                _forceGetActiveWindow = false;
                windowInfo.IsNewWindow = windowInfo.FgHwnd != _currentHwnd;
                windowInfo.IsNewFocusedElement = elementChanged;
                EvtFocusChanged?.Invoke(windowInfo);
                _currentFocusedElement = windowInfo.FocusedElement;
            }

            _currentHwnd = windowInfo.FgHwnd;

            // Heartbeat
            if (_heartbeatToggle)
            {
                EvtWindowMonitorHeartbeat?.Invoke(windowInfo);
            }
            _heartbeatToggle = !_heartbeatToggle;
        }

        public static WindowActivityMonitorInfo GetForegroundWindowInfo()
        {
            IntPtr hwnd = Windows.GetForegroundWindow();
            if (hwnd == IntPtr.Zero) return null;

            var info = new WindowActivityMonitorInfo
            {
                FgHwnd = hwnd,
                Title = Windows.GetWindowTitle(hwnd),
                FgProcess = GetProcessForWindow(hwnd)
            };

            try
            {
                uint processId = (uint)Windows.GetWindowThreadProcessId(hwnd);
                uint currentProcessId = (uint)Process.GetCurrentProcess().Id;

                if (currentProcessId == processId)
                {
                    info.FocusedElement = AutomationElement.FocusedElement;
                }
            }
            catch
            {
                info.FocusedElement = null;
            }

            return info;
        }

        public static Process GetProcessForWindow(IntPtr hwnd)
        {
            User32Interop.GetWindowThreadProcessId(hwnd, out int pid);
            return Process.GetProcessById(pid);
        }

        public static bool IsDifferent(AutomationElement ele1, AutomationElement ele2)
        {
            if (ele1 == null || ele2 == null) return true;

            try
            {
                return !Automation.Compare(ele1.GetRuntimeId(), ele2.GetRuntimeId());
            }
            catch
            {
                return true;
            }
        }

        private static bool TryEnter(object syncObj)
        {
            bool lockTaken = false;
            System.Threading.Monitor.TryEnter(syncObj, ref lockTaken);
            return lockTaken;
        }

        private static void Release(object syncObj)
        {
            try { System.Threading.Monitor.Exit(syncObj); }
            catch { }
        }
    }
}