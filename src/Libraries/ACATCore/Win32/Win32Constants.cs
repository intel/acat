// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace ACAT.Win32
{
    /// <summary>
    /// Win32-related constants. Some of these definitions originated from https://pinvoke.net/
    /// </summary>
    public static class Win32Constants
    {
        //Declare the mouse hook constant.
        //For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
        public const int WM_LBUTTONUP = 0x0202;

        public const int TME_LEAVE = 0x00000002;

        public const int SWP_NOACTIVATE = 0x0010;

        public const int WM_DPICHANGED = 0x02E0;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_NCHITTEST = 0x0084;

        public const int HTTRANSPARENT = -1;
        public const int WM_ERASEBKGND = 0x0014;
        public const int HWND_TOPMOST = -1;

        public const int DT_LEFT = 0x00000000;
        public const int DT_SINGLELINE = 0x00000020;
        public const int DT_NOCLIP = 0x00000100;
        public const int DT_CALCRECT = 0x00000400;
        public const int DT_NOPREFIX = 0x00000800;
        public const int DT_END_ELLIPSIS = 0x00008000;

        public const uint TOKEN_QUERY = 0x0008;

        /// <summary>
        /// Constant identify if windows is minimized
        /// </summary>
        public const uint WS_MINIMIZE = 0x20000000;

        public const Int32 WM_NCLBUTTONDOWN = 0xA1;
        public const Int32 HTCAPTION = 0x2;

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;


    }
}
