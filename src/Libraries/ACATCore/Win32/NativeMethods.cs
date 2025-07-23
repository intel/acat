// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ACAT.Win32
{
    /// <summary>
    /// Class NativeMethods for Win32 Apis pInvoke and Win32 specific code.
    /// Some of these definitions originated from https://pinvoke.net/
    /// </summary>
    public static partial class NativeMethods
    {
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern uint GetDeviceCaps(IntPtr hDC, int nIndex);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        internal static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        internal static extern uint GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        [DllImport("Gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll")]
        internal static extern bool SystemParametersInfo(int iAction, int iParam, out bool bActive, int iUpdate);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        /// <summary>
        /// Close handle
        /// </summary>
        /// <param name="hHandle"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hHandle);

        /// <summary>
        /// Open process Token
        /// </summary>
        /// <param name="ProcessHandle"></param>
        /// <param name="DesiredAccess"></param>
        /// <param name="TokenHandle"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, out uint TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        internal static extern bool DeleteDC([In] IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        internal static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateFont(int nHeight, int nWidth, int nEscapement,
         int nOrientation, FontWeight fnWeight, bool fdwItalic, bool fdwUnderline,
         bool fdwStrikeOut, FontCharSet fdwCharSet, FontPrecision fdwOutputPrecision,
         FontClipPrecision fdwClipPrecision, FontQuality fdwQuality, FontPitchAndFamily fdwPitchAndFamily, string lpszFace);

        [DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        internal static extern uint SetBkColor(IntPtr hdc, int crColor);

        [DllImport("gdi32.dll")]
        internal static extern uint SetTextColor(IntPtr hdc, int crColor);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int DrawText(IntPtr hDC, string lpString, int nCount, ref RECT lpRect, uint uFormat);

        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr GetModuleHandle(string strModuleName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.U2)]
        internal static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateWindowEx(
           WindowStylesEx dwExStyle,
           string lpClassName,
           string lpWindowName,
           WindowStyles dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll")]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern System.UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr GetStockObject(StockObjects fnObject);

        [DllImport("user32.dll")]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("user32.dll")]
        internal static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

        [DllImport("gdi32.dll")]
        internal static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);

        [DllImport("gdi32.dll")]
        internal static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref RECT rect, [MarshalAs(UnmanagedType.U4)] int cPoints);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        internal static extern int CombineRgn(IntPtr hrgnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, int fnCombineMode);

        [DllImport("user32.dll")]
        internal static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("User32.dll")]
        internal static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        //[DllImport("wintrust.dll", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Unicode)]
        //public static extern WinVerifyTrustResult WinVerifyTrust(
        //[In] IntPtr hwnd,
        //[In][MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID,
        //[In] WinTrustData pWVTData);

        //public static readonly Guid WINTRUST_ACTION_GENERIC_VERIFY_V2 = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");

    }
}
