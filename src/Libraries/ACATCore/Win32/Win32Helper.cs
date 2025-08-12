// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using ACAT.Core.Win32;
using Microsoft.Win32;
using System;
using System.Drawing;

namespace ACAT.Win32
{
    /// <summary>
    /// NativeMethods partial class to hold all Win32 related helper methods.
    /// </summary>
    public static partial class NativeMethods
    {
        /// <summary>
        /// Check whether App is running with the UIAccess privilege.
        /// </summary>
        /// <returns></returns>
        internal static bool IsRunningWithUIAccess()
        {
            if (NativeMethods.OpenProcessToken(System.Diagnostics.Process.GetCurrentProcess().Handle, Win32Constants.TOKEN_QUERY, out IntPtr hToken))
            {
                try
                {
                    if (NativeMethods.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenUIAccess, out uint uIAccess, sizeof(uint), out uint cbData))
                    {
                        if (uIAccess != 0)
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hToken);
                }
            }

            return false;
        }

        /// <summary>
        /// Set focus on the windows given the windows handle
        /// </summary>
        /// <param name="focusOnWindowHandle"></param>
        internal static void FocusWindow(IntPtr focusOnWindowHandle)
        {
            uint style = NativeMethods.GetWindowLong(focusOnWindowHandle, -16);

            // Minimize and restore to be able to make it active.
            if ((style & Win32Constants.WS_MINIMIZE) == Win32Constants.WS_MINIMIZE)
            {
                NativeMethods.ShowWindow(focusOnWindowHandle, ShowWindowCommands.Restore);
            }

            uint currentlyFocusedWindowProcessId = NativeMethods.GetWindowThreadProcessId(NativeMethods.GetForegroundWindow(), IntPtr.Zero);
            uint appThread = (uint)Environment.CurrentManagedThreadId;

            if (currentlyFocusedWindowProcessId != appThread)
            {
                NativeMethods.AttachThreadInput(currentlyFocusedWindowProcessId, appThread, true);
                NativeMethods.BringWindowToTop(focusOnWindowHandle);
                NativeMethods.ShowWindow(focusOnWindowHandle, ShowWindowCommands.Show);
                NativeMethods.AttachThreadInput(currentlyFocusedWindowProcessId, appThread, false);
            }

            else
            {
                NativeMethods.BringWindowToTop(focusOnWindowHandle);
                NativeMethods.ShowWindow(focusOnWindowHandle, ShowWindowCommands.Show);
            }
            NativeMethods.SetActiveWindow(focusOnWindowHandle);
        }

        /// <summary>
        /// Get RGB value
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        internal static int RGB(int r, int g, int b)
        {
            return r | g << 8 | b << 16;
        }

        /// <summary>
        /// Check if a 3rd party Screen Reader such as Jaws is running
        /// </summary>
        public static bool IsExternalScreenReaderActive()
        {
            const int SPI_GETSCREENREADER = 0x0046;  // Defined in winuser.h
            bool success = NativeMethods.SystemParametersInfo(SPI_GETSCREENREADER, 0, out bool active, 0);
            return success && active;
        }

        /// <summary>
        /// Extract the installed .NET Framework version from the registry. Based on information found at
        /// https://docs.microsoft.com/en-us/dotnet/framework/migration-guide/how-to-determine-which-versions-are-installed
        /// </summary>
        /// <returns>The integral version if it's available, null if not</returns>
        internal static int? GetInstalledDotNetFrameworkVersion()
        {
            const string keyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            const string valueName = "Release";

            return (int?)Registry.GetValue(keyName, valueName, null);
        }

        /// <summary>
        /// Check whether the current Windows is Windows 7 or not.
        /// </summary>
        /// <returns></returns>
        internal static bool IsWindows7()
        {
            return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
        }

        /// <summary>
        /// Check whether the user has selected dark mode. Reads the registry directly
        /// </summary>
        /// <returns>true if dark mode is enabled</returns>
        internal static bool IsDarkModeEnabled()
        {
            const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize\";
            const string valueName = "AppsUseLightTheme";

            int? value = (int?)Registry.GetValue(keyName, valueName, null);

            return value.HasValue && value.Value == 0;
        }

        /// <summary>
        /// Get DPI value from point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="dpiType"></param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        internal static void GetDpi(Point point, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            const uint defaultDpi = 96;
            const uint S_OK = 0;

            var mon = NativeMethods.MonitorFromPoint(point, 2/*MONITOR_DEFAULTTONEAREST*/);
            if (IsWindows7())
            {
                Graphics g = Graphics.FromHwnd(IntPtr.Zero);
                IntPtr desktop = g.GetHdc();

                dpiX = NativeMethods.GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSX);
                dpiY = NativeMethods.GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
            }
            else
            {
                if (NativeMethods.GetDpiForMonitor(mon, dpiType, out uint localDpiX, out uint localDpiY) == S_OK)
                {
                    dpiX = localDpiX;
                    dpiY = localDpiY;
                }
                else
                {
                    dpiX = defaultDpi;
                    dpiY = defaultDpi;
                }
            }
        }
    }
}
