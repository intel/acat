////////////////////////////////////////////////////////////////////////////
// <copyright file="DualMonitor.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2019 Intel Corporation 
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
