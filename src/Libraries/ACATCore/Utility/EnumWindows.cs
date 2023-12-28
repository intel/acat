////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Enumerates active windows on the desktop.
    /// </summary>
    public static class EnumWindows
    {
        /// <summary>
        /// Ignore windows with these class names during enumeration
        /// </summary>
        private static readonly string[] _ignoreClassNames =
        {
            "MsgrIMEWindowClass",
            "SysShadow",
            "Button",
            "Shell_TrayWnd",
            "DV2ControlHost"
        };

        /// <summary>
        /// This process
        /// </summary>
        private static Process _currentProcess;

        /// <summary>
        /// Whether to exclude windows from this process
        /// </summary>
        private static bool _excludeThisProcess;

        /// <summary>
        /// Handle to windows shell
        /// </summary>
        private static IntPtr _shellWindowHandle;

        /// <summary>
        /// Holds the list of active windows
        /// </summary>
        private static List<WindowInfo> windowList;

        /// <summary>
        /// Synchronously enumerates windows and returns a window list
        /// </summary>
        /// <returns>list of windows</returns>
        public static List<WindowInfo> Enumerate(bool excludeThisProcess = true)
        {
            _currentProcess = Process.GetCurrentProcess();
            _excludeThisProcess = excludeThisProcess;
            _shellWindowHandle = User32Interop.GetShellWindow();

            windowList = new List<WindowInfo>();
            User32Interop.EnumDesktopWindows(IntPtr.Zero, enumWindowsCallback, IntPtr.Zero);

            return windowList;
        }

        /// <summary>
        /// Sets focus to the top window in the Z Order (including
        /// window in this application)
        /// </summary>
        /// <param name="ignoreHandle">ignore this window</param>
        public static void RestoreFocusToTopWindow(int ignoreHandle = 0)
        {
            var winList = Enumerate(false);
            Log.Debug("winList Count: " + winList.Count);
            bool found = false;
            var handle = IntPtr.Zero;
            IntPtr ignoreWindowHandle = new IntPtr(ignoreHandle);

            foreach (var windowInfo in winList)
            {
                //Log.Debug("Title: " + w.Title);
                handle = windowInfo.Handle;

                if (ignoreHandle != 0 && handle == ignoreWindowHandle)
                {
                    continue;
                }

                var control = Form.FromHandle(handle);
                if (control is IDialogPanel)
                {
                    Log.Debug("Setting focus to ACAT dialog." + windowInfo.Title);
                    found = true;
                    break;
                }

                // we don't want to set focus to the DebugView window
                // as it causes ACAT to freeze.  DebugVIew is a SysInternals
                // utility used to display debug messages from ACAT
                if (!windowInfo.Title.Contains("DebugView") &&
                    !Windows.IsMinimized(handle) &&
                    !(control is MenuPanelBase) &&
                    !(control is IScannerPanel))
                {
                    Log.Debug("Found top window " + windowInfo.Title);
                    found = true;
                    break;
                }
            }

            //Log.Debug("found: " + found);
            if (!found || handle == IntPtr.Zero)
            {
                Windows.SetFocusToDesktop();
            }
            else
            {
                Windows.SetForegroundWindow(handle);
            }
        }

        /// <summary>
        /// Restores focus to the top window on the desktop, excluding
        /// windows in this application
        /// </summary>
        public static void RestoreFocusToTopWindowOnDesktop()
        {
            var winList = Enumerate();
            Log.Debug("winList Count: " + winList.Count);
            bool found = false;
            var handle = IntPtr.Zero;

            foreach (var windowInfo in winList)
            {
                //Log.Debug("Title: " + w.Title);
                handle = windowInfo.Handle;

                // we don't want to set focus to the DebugView window
                // as it causes ACAT to freeze.  DebugVIew is a SysInternals
                // utility used to display debug messages from ACAT
                if (!windowInfo.Title.Contains("DebugView") &&
                    !Windows.IsMinimized(handle))
                {
                    Log.Debug("Found top window " + windowInfo.Title);
                    found = true;
                    break;
                }
            }

            //Log.Debug("found: " + found);
            if (!found || handle == IntPtr.Zero)
            {
                Windows.SetFocusToDesktop();
            }
            else
            {
                Windows.SetForegroundWindow(handle);
            }
        }

        /// <summary>
        /// Verifies whether the window can be activated
        /// </summary>
        /// <param name="hWnd">WIndow handle</param>
        /// <returns>true if it can</returns>
        private static bool canBeActivated(IntPtr hWnd)
        {
            if (isShellWindow(hWnd))
            {
                return false;
            }

            var root = User32Interop.GetAncestor(hWnd, User32Interop.GetAncestorFlags.GetRootOwner);

            if (getWindowPopup(root) != hWnd)
            {
                return false;
            }

            var className = Windows.GetWindowClassName(hWnd);
            if (String.IsNullOrEmpty(className))
            {
                return false;
            }

            if (Array.IndexOf(_ignoreClassNames, className) > -1)
            {
                return false;
            }

            if (className.StartsWith("WMP9MediaBarFlyout"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Callback function for the enumwindows function
        /// </summary>
        /// <param name="winHandle">enumerated window handle</param>
        /// <param name="lParam">nod used </param>
        /// <returns></returns>
        private static bool enumWindowsCallback(IntPtr winHandle, int lParam)
        {
            if (!User32Interop.IsWindowVisible(winHandle))
            {
                return true;
            }

            if (_excludeThisProcess)
            {
                uint pid = 0;
                User32Interop.GetWindowThreadProcessId(winHandle, out pid);

                if (pid == 0 || pid == _currentProcess.Id)
                {
                    return true;
                }
            }

            if (!Windows.IsApplicationWindow(winHandle))
            {
                return true;
            }

            if (!canBeActivated(winHandle))
            {
                return true;
            }

            if (Windows.IsCloakedWindow(winHandle))
            {
                return true;
            }

            var buffer = new StringBuilder(1024);
            User32Interop.GetWindowText(winHandle, buffer, buffer.Capacity);
            var windowTitle = buffer.ToString();

            // save the window only if it is visible and contains a title
            // and has an app icon (this criterium is needed to not display "Start" and "Program Manager")
            // this can be altered in the future to add other filtering criteria

            if (!string.IsNullOrEmpty(windowTitle))
            {
                Log.Debug("hWnd=" + winHandle + "  windowTitle=" + windowTitle);

                var info = new WindowInfo(winHandle, windowTitle);
                windowList.Add(info);
            }

            // return true to continue enumerating windows.
            return true;
        }

        /// <summary>
        /// Gets the pop window (if any) of the specified window
        /// </summary>
        /// <param name="window">Window handle</param>
        /// <returns>handle if found, IntPtr.Zero else</returns>
        private static IntPtr getWindowPopup(IntPtr window)
        {
            var windowLevel = 32;
            var targetWindow = window;

            while (windowLevel-- > 0)
            {
                IntPtr popupWindow = User32Interop.GetLastActivePopup(targetWindow);

                if (User32Interop.IsWindowVisible(popupWindow))
                {
                    return popupWindow;
                }

                if (popupWindow == targetWindow)
                {
                    return IntPtr.Zero;
                }

                targetWindow = popupWindow;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Checks if the specified handle is the shell window handle
        /// </summary>
        /// <param name="hWnd">handle to check</param>
        /// <returns>true if it is</returns>
        private static bool isShellWindow(IntPtr hWnd)
        {
            return (hWnd == _shellWindowHandle);
        }

        /// <summary>
        /// Encapsulates info about enumerated window
        /// </summary>
        public class WindowInfo
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="handle">window handle</param>
            /// <param name="title">window title</param>
            public WindowInfo(IntPtr handle, String title)
            {
                Handle = handle;
                Title = title;
            }

            /// <summary>
            /// Gets  window handle
            /// </summary>
            public IntPtr Handle { get; private set; }

            /// <summary>
            /// Gets title
            /// </summary>
            public String Title { get; private set; }
        }
    }
}