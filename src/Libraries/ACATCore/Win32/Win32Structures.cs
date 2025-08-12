// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Runtime.InteropServices;

namespace ACAT.Core.Win32
{
    // Some of these definitions originated from https://pinvoke.net/

    //Declare the wrapper managed POINT class.
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WNDCLASSEX
    {
        public uint cbSize;
        public uint style;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WndProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        public string lpszMenuName;
        public string lpszClassName;
        public IntPtr hIconSm;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TRACKMOUSEEVENT
    {
        public int cbSize;    // using Int32 instead of UInt32 is safe here, and this avoids casting the result  of Marshal.SizeOf()
        [MarshalAs(UnmanagedType.U4)]
        public int dwFlags;
        public IntPtr hWnd;
        public uint dwHoverTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class WinTrustFileInfo : IDisposable
    {
        readonly uint StructSize = (uint)Marshal.SizeOf(typeof(WinTrustFileInfo));
        IntPtr pszFilePath;                     // required, file name to be verified
        readonly IntPtr hFile = IntPtr.Zero;             // optional, open handle to FilePath
        readonly IntPtr pgKnownSubject = IntPtr.Zero;    // optional, subject type if it is known

        public WinTrustFileInfo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            pszFilePath = Marshal.StringToCoTaskMemAuto(filePath);
        }

        ~WinTrustFileInfo()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (pszFilePath != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pszFilePath);
                pszFilePath = IntPtr.Zero;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal class WinTrustData : IDisposable
    {
        readonly uint StructSize = (uint)Marshal.SizeOf(typeof(WinTrustData));
        readonly IntPtr PolicyCallbackData = IntPtr.Zero;
        readonly IntPtr SIPClientData = IntPtr.Zero;

        // required: UI choice
        readonly WinTrustDataUIChoice UIChoice = WinTrustDataUIChoice.None;

        // required: certificate revocation check options
        readonly WinTrustDataRevocationChecks RevocationChecks = WinTrustDataRevocationChecks.None;

        // required: which structure is being passed in?
        readonly WinTrustDataChoice UnionChoice = WinTrustDataChoice.File;
        // individual file
        IntPtr FileInfoPtr = IntPtr.Zero;
        readonly WinTrustDataStateAction StateAction = WinTrustDataStateAction.Verify;
        readonly IntPtr StateData = IntPtr.Zero;
        readonly string URLReference;
        readonly WinTrustDataProvFlags ProvFlags = WinTrustDataProvFlags.RevocationCheckChainExcludeRoot;
        readonly WinTrustDataUIContext UIContext = WinTrustDataUIContext.Execute;

        // constructor for silent WinTrustDataChoice.File check
        public WinTrustData(WinTrustFileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            // On Win7SP1+, don't allow MD2 or MD4 signatures
            if (Environment.OSVersion.Version.Major > 6 ||
                Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 1 ||
                Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1 && !string.IsNullOrEmpty(Environment.OSVersion.ServicePack))
            {
                ProvFlags |= WinTrustDataProvFlags.DisableMD2andMD4;
            }

            FileInfoPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(fileInfo.GetType()));
            Marshal.StructureToPtr(fileInfo, FileInfoPtr, false);
        }

        ~WinTrustData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (FileInfoPtr != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(FileInfoPtr);
                FileInfoPtr = IntPtr.Zero;
            }
        }
    }
}
