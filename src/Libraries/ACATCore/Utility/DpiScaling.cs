////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// The original source for this class is from:
// https://stackoverflow.com/questions/32607468/get-scale-of-screen
// It is licensed by StackOverflow under the Creative Commons license
// CC BY-SA 4.0
// https://creativecommons.org/licenses/by-sa/4.0/
////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    public static class DpiScaling
    {
        /// <summary>
        /// Min OS version build that supports DPI per monitor
        /// </summary>
        private const int MinOSVersionBuild = 14393;

        /// <summary>
        /// Min OS version major build that support DPI per monitor
        /// </summary>
        private const int MinOSVersionMajor = 10;

        /// <summary>
        /// Flag, if OS version checked
        /// </summary>
        private static bool _isOSVersionChecked;

        /// <summary>
        /// Flag, if OS supports DPI per monitor
        /// </summary>
        private static bool _isSupportingDpiPerMonitor;
        /// <summary>
        /// DPI type
        /// </summary>
        private enum DpiType
        {
            /// <summary>
            /// The effective DPI. This value should be used when determining the correct scale factor for scaling UI elements.
            /// </summary>
            Effective = 0,

            /// <summary>
            /// The angular DPI. This DPI ensures rendering at a compliant angular resolution on the screen.
            /// </summary>
            Angular = 1,

            /// <summary>
            /// The raw DPI. This value is the linear DPI of the screen as measured on the screen itself. Use this value when you want to read the pixel density and not the recommended scaling setting.
            /// </summary>
            Raw = 2,
        }

        /// <summary>
        /// Flag, if OS supports DPI per monitor
        /// </summary>
        public static bool IsSupportingDpiPerMonitor
        {
            get
            {
                if (_isOSVersionChecked)
                {
                    return _isSupportingDpiPerMonitor;
                }

                _isOSVersionChecked = true;
                var osVersionInfo = new OSVERSIONINFOEXW
                {
                    dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEXW))
                };

                if (RtlGetVersion(ref osVersionInfo) != 0)
                {
                    _isSupportingDpiPerMonitor = Environment.OSVersion.Version.Major >= MinOSVersionMajor && Environment.OSVersion.Version.Build >= MinOSVersionBuild;

                    return _isSupportingDpiPerMonitor;
                }

                _isSupportingDpiPerMonitor = osVersionInfo.dwMajorVersion >= MinOSVersionMajor && osVersionInfo.dwBuildNumber >= MinOSVersionBuild;

                return _isSupportingDpiPerMonitor;
            }
        }

        /// <summary>
        /// Get DPI for a monitor
        /// </summary>
        /// <param name="control"> Any control for OS who doesn't support DPI per monitor </param>
        /// <param name="monitorPoint"> Monitor point (Screen.Bounds) </param>
        /// <returns> DPI </returns>
        public static uint GetDpi(Control control, Point monitorPoint)
        {
            uint dpiX;
            uint dpiY;

            if (IsSupportingDpiPerMonitor)
            {
                var monitorFromPoint = User32Interop.MonitorFromPoint(monitorPoint, 2);

                GetDpiForMonitor(monitorFromPoint, DpiType.Effective, out dpiX, out dpiY);
            }
            else
            {
                // If using with System.Windows.Forms - can be used Control.DeviceDpi
                dpiX = control == null ? 96 : (uint)control.DeviceDpi;
            }

            return dpiX;
        }

        /// <summary>
        /// Get scale factor for an each monitor
        /// </summary>
        /// <param name="control"> Any control for OS who doesn't support DPI per monitor </param>
        /// <param name="monitorPoint"> Monitor point (Screen.Bounds) </param>
        /// <returns> Scale factor </returns>
        public static double ScaleFactor(Control control, Point monitorPoint)
        {
            var dpi = GetDpi(control, monitorPoint);

            return dpi * 100 / 96.0;
        }

        /// <summary>
        /// Queries the dots per inch (dpi) of a display.
        /// </summary>
        /// <param name="hmonitor"> Handle of the monitor being queried. </param>
        /// <param name="dpiType"> The type of DPI being queried. </param>
        /// <param name="dpiX"> The value of the DPI along the X axis. </param>
        /// <param name="dpiY"> The value of the DPI along the Y axis. </param>
        /// <returns> Status success </returns>
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        /// <summary>
        /// The RtlGetVersion routine returns version information about the currently running operating system.
        /// </summary>
        /// <param name="versionInfo"> Operating system version information </param>
        /// <returns> Status success</returns>
        [SecurityCritical]
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlGetVersion(ref OSVERSIONINFOEXW versionInfo);

        /// <summary>
        /// Contains operating system version information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEXW
        {
            /// <summary>
            /// The size of this data structure, in bytes
            /// </summary>
            internal int dwOSVersionInfoSize;

            /// <summary>
            /// The major version number of the operating system.
            /// </summary>
            internal int dwMajorVersion;

            /// <summary>
            /// The minor version number of the operating system.
            /// </summary>
            internal int dwMinorVersion;

            /// <summary>
            /// The build number of the operating system.
            /// </summary>
            internal int dwBuildNumber;

            /// <summary>
            /// The operating system platform.
            /// </summary>
            internal int dwPlatformId;

            /// <summary>
            /// A null-terminated string, such as "Service Pack 3", that indicates the latest Service Pack installed on the system.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            internal string szCSDVersion;

            /// <summary>
            /// The major version number of the latest Service Pack installed on the system.
            /// </summary>
            internal ushort wServicePackMajor;

            /// <summary>
            /// The minor version number of the latest Service Pack installed on the system.
            /// </summary>
            internal ushort wServicePackMinor;

            /// <summary>
            /// A bit mask that identifies the product suites available on the system.
            /// </summary>
            internal short wSuiteMask;

            /// <summary>
            /// Any additional information about the system.
            /// </summary>
            internal byte wProductType;

            /// <summary>
            /// Reserved for future use.
            /// </summary>
            internal byte wReserved;
        }
    }
}