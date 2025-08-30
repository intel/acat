// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0

using ControlzEx.Standard;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;


namespace ACAT.Core.Utility
{
    public class WindowActivityMonitor
    {
        private static Form _form;
        private static volatile bool _isPaused = false;
        private static Timer _heartbeatTimer;
        private static bool _heartbeatToggle = true;
        private static int HeartbeatInterval = 5000; // Default heartbeat interval in milliseconds


        private static IntPtr _forgroundHook = IntPtr.Zero;
        private static IntPtr _createDestroyHook = IntPtr.Zero;
        private static WinEventDelegate _winEventDelegate;
        private static WinEventDelegate _createDestroyDelegate;

        private static Form _currentForm = null;
        private static AutomationElement _currentFocusedElement;

        public delegate void ActivityMonitorDelegate(WindowActivityMonitorInfo monitorInfo);
        public static event ActivityMonitorDelegate EvtFocusChanged;
        public static event ActivityMonitorDelegate EvtWindowMonitorHeartbeat;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        public event EventHandler<WindowActivityMonitorInfo> ForegroundChanged;

        private void RaiseForegroundChanged(WindowActivityMonitorInfo info)
        {
            var handler = ForegroundChanged;
            if (handler == null) return;

            // Find a form to marshal onto the UI thread
            if (Application.OpenForms.Count > 0)
            {
                var mainForm = Application.OpenForms[0];

                if (mainForm.IsHandleCreated)
                {
                    mainForm.BeginInvoke(new Action(() => handler(this, info)));
                }
                else
                {
                    // fallback: raise directly (rare, but safe)
                    handler(this, info);
                }
            }
            else
            {
                // no forms — just raise on the current thread
                handler(this, info);
            }
        }

        // example usage inside your polling loop or hook
        private void CheckForegroundWindow()
        {
            var info = GetForegroundWindowInfo();
            RaiseForegroundChanged(info);
        }

        public static bool Start()
        {
            if (_form == null)
            {
                _form = new Form { Visible = false };
                _form.Show();
                _form.Visible = false;
            }

            if (_forgroundHook == IntPtr.Zero)
            {
                _winEventDelegate = ForegroundEventCallback;
                _forgroundHook = NativeMethods.SetWinEventHook(
                    NativeMethods.EVENT_SYSTEM_FOREGROUND,
                    NativeMethods.EVENT_SYSTEM_FOREGROUND,
                    IntPtr.Zero,
                    _winEventDelegate,
                    0,
                    0,
                    NativeMethods.WINEVENT_OUTOFCONTEXT);
            }

            if (_createDestroyHook == IntPtr.Zero)
            {
                _createDestroyDelegate = CreateDestroyEventCallback;
                _createDestroyHook = NativeMethods.SetWinEventHook(
                    NativeMethods.EVENT_OBJECT_CREATE,
                    NativeMethods.EVENT_OBJECT_DESTROY,
                    IntPtr.Zero,
                    _createDestroyDelegate,
                    (uint)Process.GetCurrentProcess().Id,
                    0,
                    NativeMethods.WINEVENT_OUTOFCONTEXT);
            }

            if (_heartbeatTimer == null)
            {
                _form.Invoke(new MethodInvoker(() =>
                {
                    _heartbeatTimer = new Timer { Interval = HeartbeatInterval };
                    _heartbeatTimer.Tick += HeartbeatTick;
                    _heartbeatTimer.Start();
                }));
            }

            Automation.AddAutomationFocusChangedEventHandler(OnAutomationFocusChanged);
            return true;
        }

        public static void Refresh()
        {
            if (_isPaused) return;
            // Get the current foreground window and focused element
            var info = GetForegroundWindowInfo();
            if (info != null)
            {
                HandleFocusOrWindowChange(info);
            }
        }

        public static WindowActivityMonitorInfo CurrentWindowInfo()
        {
            return new WindowActivityMonitorInfo
            {
                FgForm = _currentForm,
                FocusedElement = _currentFocusedElement,
                FgProcess = _currentForm != null ? GetProcessForWindow(_currentForm.Handle) : null,
                Title = _currentForm != null ? NativeMethods.GetWindowTitle(_currentForm.Handle) : null
            };
        }

        public static void Pause()
        {
            _isPaused = true;
            _heartbeatTimer?.Stop();
        }

        public static void Resume()
        {
            _isPaused = false;
            _heartbeatTimer?.Start();
        }

        public static void Dispose()
        {
            if (_heartbeatTimer != null)
            {
                _heartbeatTimer.Stop();
                _heartbeatTimer.Dispose();
            }

            if (_forgroundHook != IntPtr.Zero)
            {
                NativeMethods.UnhookWinEvent(_forgroundHook);
            }

            Automation.RemoveAllEventHandlers();
        }

        private static void HeartbeatTick(object sender, EventArgs e)
        {
            if (_isPaused || _currentForm == null) return;

            if(_currentForm.Handle != IntPtr.Zero)
            {
                var info = GetForegroundWindowInfo();
                EvtWindowMonitorHeartbeat?.Invoke(info);
            }
        }

        private static void ForegroundEventCallback(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (_isPaused || hwnd == IntPtr.Zero) return;

            Form? fgForm = NativeMethods.GetForegroundWindow() == hwnd ? Control.FromHandle(hwnd) as Form : null;
            var info = GetForegroundWindowInfo(fgForm);
            HandleFocusOrWindowChange(info);
        }

        private static void CreateDestroyEventCallback(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (_isPaused || hwnd == IntPtr.Zero) return;

            const int OBJID_WINDOW = 0;
            if (idObject != OBJID_WINDOW || idChild != 0) return;

            var form = Form.FromChildHandle(hwnd);
            if (form == null) return;

            switch (eventType)
            {
                case NativeMethods.EVENT_OBJECT_CREATE:
                    TopMostManager.Register(form as Form);
                    break;
                case NativeMethods.EVENT_OBJECT_DESTROY:
                    TopMostManager.Unregister(form as Form);
                    break;
            }
        }

        private static void OnAutomationFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            if (_isPaused) return;

            var focusedElement = AutomationElement.FocusedElement;

            if (focusedElement == null) return;

            var info = GetForegroundWindowInfo();

            if (info == null)
                return;

            info.FocusedElement = focusedElement;

            if (_form.InvokeRequired)
            {
                _form.Invoke(new Action(() => HandleFocusOrWindowChange(info)));
                return;
            }
            else
            {
                HandleFocusOrWindowChange(info);
            }
        }

        private static void HandleFocusOrWindowChange(WindowActivityMonitorInfo info)
        {
            if (info == null || info.FgForm == null || _currentForm == null) return;

            bool isNewWindow = info.FgForm.Handle != _currentForm.Handle;
            bool isNewElement;

            if (info.FocusedElement == null && _currentFocusedElement == null)
            {
                // No focused element, nothing to compare
                isNewElement = false;
            }
            else if (info.FocusedElement == null || _currentFocusedElement == null)
            {
                // One of them is null, so they are different
                isNewElement = true;
            }
            else
            {
                isNewElement = !Automation.Compare(info.FocusedElement?.GetRuntimeId(),
                                                       _currentFocusedElement?.GetRuntimeId());
            }

            if (isNewWindow || isNewElement)
            {
                info.IsNewWindow = isNewWindow;
                info.IsNewFocusedElement = isNewElement;

                if (info.FgForm.InvokeRequired)
                {
                    info.FgForm.Invoke(new Action(() =>
                    {
                        EvtFocusChanged?.Invoke(info);
                    }));
                }

                else
                {
                    EvtFocusChanged?.Invoke(info);
                }

                _currentForm = info.FgForm;
                _currentFocusedElement = info.FocusedElement;
            }
        }

        private static WindowActivityMonitorInfo GetForegroundWindowInfo()
        {
            IntPtr hwnd = NativeMethods.GetForegroundWindow();

            Form? forgroundForm = Control.FromHandle(hwnd) as Form;

            return GetForegroundWindowInfo(forgroundForm);
        }

        private static WindowActivityMonitorInfo GetForegroundWindowInfo(Form form)
        {
            if (form == null) return null;

            if (form.InvokeRequired)
            {
                return (WindowActivityMonitorInfo)form.Invoke(new Func<WindowActivityMonitorInfo>(() =>
                {
                    return new WindowActivityMonitorInfo
                    {
                        FgForm = form,
                        Title = NativeMethods.GetWindowTitle(form.Handle),
                        FgProcess = GetProcessForWindow(form.Handle),
                        FocusedElement = Process.GetCurrentProcess().Id == NativeMethods.GetWindowProcessId(form.Handle) ? AutomationElement.FocusedElement : null
                    };
                }));
            }
            else
            {
                return new WindowActivityMonitorInfo
                {
                    FgForm = form,
                    Title = NativeMethods.GetWindowTitle(form.Handle),
                    FgProcess = GetProcessForWindow(form.Handle),
                    FocusedElement = Process.GetCurrentProcess().Id == NativeMethods.GetWindowProcessId(form.Handle) ? AutomationElement.FocusedElement : null
                };

            }

            //if (Process.GetCurrentProcess().Id == NativeMethods.GetWindowProcessId(form.Handle))
            //{
            //    info.FocusedElement = AutomationElement.FocusedElement;
            //}

            //return info;
        }

        public static Process GetProcessForWindow(IntPtr hwnd)
        {
            int pid = NativeMethods.GetWindowProcessId(hwnd);
            return Process.GetProcessById(pid);
        }

        private static class NativeMethods
        {
            public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
            public const uint EVENT_OBJECT_CREATE = 0x8000;
            public const uint EVENT_OBJECT_DESTROY = 0x8001;
            public const uint WINEVENT_OUTOFCONTEXT = 0;

            [DllImport("user32.dll")]
            public static extern IntPtr SetWinEventHook(
                uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
                WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

            [DllImport("user32.dll")]
            public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

            [DllImport("user32.dll")]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

            public static string GetWindowTitle(IntPtr hwnd)
            {
                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hwnd, sb, sb.Capacity);
                return sb.ToString();
            }

            public static int GetWindowProcessId(IntPtr hwnd)
            {
                GetWindowThreadProcessId(hwnd, out int pid);
                return pid;
            }
        }
    }
}