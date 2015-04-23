////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowsAccess.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;

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
    /// Enumerates active windows on the desktop.
    /// </summary>
    public static class EnumWindows
    {
        private const int GCL_HICON = -14;
        private const int GCL_HICONSM = -34;
        private const int ICON_BIG = 1;
        private const int ICON_SMALL = 0;
        private const int ICON_SMALL2 = 2;
        private const int WM_GETICON = 0x7F;

        /// <summary>
        /// Holds the list of active windows
        /// </summary>
        private static List<WindowInfo> windowList;

        /// <summary>
        /// Synchronously enumerates windows and returns a window list
        /// </summary>
        /// <returns>list of windows</returns>
        public static List<WindowInfo> Enumerate()
        {
            windowList = new List<WindowInfo>();
            User32Interop.EnumDesktopWindows(IntPtr.Zero, enumWindowsCallback, IntPtr.Zero);
            return windowList;
        }

        /// <summary>
        /// Gets the icon associated with the specified window
        /// </summary>
        /// <param name="winHandle">window handle</param>
        /// <returns>the icon, null if it can't find one</returns>
        public static Icon GetAppIcon(IntPtr winHandle)
        {
            Log.Debug("hWnd=" + winHandle);

            IntPtr hIcon = User32Interop.SendMessage(winHandle, WM_GETICON, ICON_BIG, 0);

            if (hIcon == IntPtr.Zero)
            {
                hIcon = User32Interop.SendMessage(winHandle, WM_GETICON, ICON_SMALL, 0);
                if (hIcon == IntPtr.Zero)
                {
                    hIcon = User32Interop.SendMessage(winHandle, WM_GETICON, ICON_SMALL2, 0);
                    if (hIcon == IntPtr.Zero)
                    {
                        hIcon = getClassLongPtr(winHandle, GCL_HICON);
                        if (hIcon == IntPtr.Zero)
                        {
                            hIcon = getClassLongPtr(winHandle, GCL_HICONSM);
                            if (hIcon == IntPtr.Zero)
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            Icon icon = null;
            if (hIcon != IntPtr.Zero)
            {
                icon = Icon.FromHandle(hIcon);
            }

            return icon;
        }

        static public void RestoreFocusToTopWindow()
        {
            var winList = Enumerate();
            Log.Debug("winList Count: " + winList.Count);
            bool found = false;
            var handle = IntPtr.Zero;
            foreach (var windowInfo in winList)
            {
                //Log.Debug("Title: " + w.Title);
                handle = windowInfo.Handle;
                var control = Form.FromHandle(handle);
                if (control is IDialogPanel)
                {
                    Log.Debug("Setting focus to ACAT dialog." + windowInfo.Title);
                    found = true;
                    break;
                }

                if (!windowInfo.Title.Contains("DebugView") &&
                    !Windows.IsMinimized(handle) &&
                    !(control is ContextualMenuBase) &&
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
        /// Callback function for the enumwindows function
        /// </summary>
        /// <param name="winHandle">enumerated window handle</param>
        /// <param name="lParam">nod used </param>
        /// <returns></returns>
        private static bool enumWindowsCallback(IntPtr winHandle, int lParam)
        {
            var buffer = new StringBuilder(1024);
            User32Interop.GetWindowText(winHandle, buffer, buffer.Capacity);
            var windowTitle = buffer.ToString();

            // save the window only if it is visible and contains a title
            // and has an app icon (this criterium is needed to not display "Start" and "Program Manager")
            // this can be altered in the future to add other filtering criteria

            if (User32Interop.IsWindowVisible(winHandle) &&
                (!string.IsNullOrEmpty(windowTitle)) && (GetAppIcon(winHandle) != null))
            {
                Log.Debug("hWnd=" + winHandle + "  windowTitle=" + windowTitle);

                var info = new WindowInfo(winHandle, windowTitle);
                windowList.Add(info);
            }

            // return true to continue enumerating windows.
            return true;
        }

        /// <summary>
        /// Gets window class long pointer for hWnd
        /// </summary>
        /// <param name="hWnd">window handle</param>
        /// <param name="nIndex">index number</param>
        /// <returns>window class long</returns>
        private static IntPtr getClassLongPtr(IntPtr hWnd, int nIndex)
        {
            try
            {
                if (IntPtr.Size > 4)
                {
                    return User32Interop.GetClassLongPtr64(hWnd, nIndex);
                }

                uint ret = User32Interop.GetClassLongPtr32(hWnd, nIndex);
                return new IntPtr((int)ret);  // without the cast, it may result in an overflow
            }
            catch
            {
                return IntPtr.Zero;
            }
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