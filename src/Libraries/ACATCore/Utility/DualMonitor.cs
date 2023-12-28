////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Manages dual monitor configurations
    /// </summary>
    public class DualMonitor
    {
        /// <summary>
        /// Returns true if multiple monitors were detected
        /// </summary>
        public static bool MultipleMonitors
        {
            get { return Screen.AllScreens.Length > 1; }
        }

        public static void CheckAndDisplayScaleFactorWarning()
        {
            if (!CoreGlobals.AppPreferences.ShowDisplayScaleMessageOnStartup)
            {
                return;
            }

            var tuple = GetDisplayWidthAndScaling();
            if (tuple.Item1 > 0 && tuple.Item1 <= 1920 && tuple.Item2 != 1.0 && tuple.Item2 != 1.25)
            {
                var prompt = String.Format("Display scaling is {0}%.  ACAT is optimized for 100% and 125% scaling. Some screens may not display properly.", tuple.Item2 * 100); ;
                var ret = ConfirmBoxSingleOption.ShowDialog(prompt, "OK", null, true);
                if (ret)
                {
                    CoreGlobals.AppPreferences.ShowDisplayScaleMessageOnStartup = false;
                    CoreGlobals.AppPreferences.Save();
                }
            }
        }

        public static Tuple<int, float> GetDisplayWidthAndScaling()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary)
                {
                    User32Interop.DEVMODE dm = new User32Interop.DEVMODE();
                    dm.dmSize = (short)Marshal.SizeOf(typeof(User32Interop.DEVMODE));
                    User32Interop.EnumDisplaySettings(screen.DeviceName, -1, ref dm);

                    return new Tuple<int, float>(dm.dmPelsWidth, (float)Math.Round(Decimal.Divide(dm.dmPelsWidth, screen.Bounds.Width), 2));
                }
            }

            return new Tuple<int, float>(0, 0.0f);
        }

        /// <summary>
        /// Returns the Screen object with contains the window represented
        /// by handle
        /// </summary>
        /// <param name="handle">Window handle</param>
        /// <returns>The screen which contains this window</returns>
        public static Screen GetMonitorForHandle(IntPtr handle)
        {
            return Screen.FromHandle(handle);
        }

        /// <summary>
        /// Returns the primary monitor Screen object
        /// </summary>
        /// <returns>Screen object for the primary monitor, null if not found</returns>
        public static Screen GetPrimaryMonitor()
        {
            foreach (var scr in Screen.AllScreens)
            {
                if (scr.Primary)
                {
                    return scr;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the Screen object for the second monitor
        /// </summary>
        /// <returns>The screen object, null if no second monitor</returns>
        public static Screen GetSecondaryMonitor()
        {
            if (!MultipleMonitors)
            {
                return null;
            }

            foreach (var scr in Screen.AllScreens)
            {
                if (!scr.Primary)
                {
                    return scr;
                }
            }

            return null;
        }

        /// <summary>
        /// Maximizes the specified window in the monitor other than the
        /// one it is currently in
        /// </summary>
        /// <param name="handle">Window handle</param>
        public static void MaximizeWindowInOtherMonitor(IntPtr handle)
        {
            if (handle == IntPtr.Zero || !MultipleMonitors || Windows.IsMinimized(handle))
            {
                return;
            }

            var scr = GetMonitorForHandle(handle);

            var other = scr.Primary ? GetSecondaryMonitor() : GetPrimaryMonitor();

            if (other == null)
            {
                return;
            }

            if (Windows.IsMaximized(handle))
            {
                Windows.RestoreWindow(handle);
            }

            User32Interop.RECT rect;
            User32Interop.GetWindowRect(handle, out rect);
            User32Interop.MoveWindow(handle, other.WorkingArea.Left, other.WorkingArea.Top, rect.right - rect.left, rect.bottom - rect.top, true);
            User32Interop.ShowWindow(handle.ToInt32(), Windows.ShowWindowFlags.SW_SHOWMAXIMIZED);
        }

        /// <summary>
        /// Moves the specified window to the monitor other than the one it is
        ///  currently in
        /// </summary>
        /// <param name="handle">Handle to the window</param>
        public static void MoveWindowToOtherMonitor(IntPtr handle)
        {
            if (handle == IntPtr.Zero || !MultipleMonitors || Windows.IsMinimized(handle))
            {
                return;
            }

            var scr = GetMonitorForHandle(handle);

            var other = scr.Primary ? GetSecondaryMonitor() : GetPrimaryMonitor();

            if (other == null)
            {
                return;
            }

            if (Windows.IsMaximized(handle))
            {
                Windows.RestoreWindow(handle);
            }

            User32Interop.RECT rect;
            User32Interop.GetWindowRect(handle, out rect);
            User32Interop.MoveWindow(handle, other.WorkingArea.Left, other.WorkingArea.Top, rect.right - rect.left, rect.bottom - rect.top, true);
        }
    }
}