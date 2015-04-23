////////////////////////////////////////////////////////////////////////////
// <copyright file="DLLImports.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
#endregion

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// pInvoke functions into Windows USB and HID functions
    /// </summary>
    public class DllImports
    {
        public const int DIGCF_INTERFACEDEVICE = 0x00000010;
        public const int DIGCF_PRESENT = 0x00000002;
        public const int DIGCF_DEVICEINTERFACE = 0x00000010;

        public const int OPEN_EXISTING = 3;

        public const int ERROR_INVALID_HANDLE = 6;
        public const int INVALID_HANDLE_VALUE = -1;

        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const int DUPLICATE_SAME_ACCESS = 0x00000002;

        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;

        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint FILE_SHARE_READ = 0x00000001;

        [StructLayout(LayoutKind.Sequential)]
        public struct GUID
        {
            public int Data1;
            public UInt16 Data2;
            public UInt16 Data3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] data4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_CAPS
        {
            public UInt16 Usage;
            public UInt16 UsagePage;
            public UInt16 InputReportByteLength;
            public UInt16 OutputReportByteLength;
            public UInt16 FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public UInt16[] Reserved;
            public UInt16 NumLinkCollectionNodes;
            public UInt16 NumInputButtonCaps;
            public UInt16 NumbInputValueCaps;
            public UInt16 NumInputDataIndices;
            public UInt16 NumOutputButtonCaps;
            public UInt16 NumOutputValueCaps;
            public UInt16 NumOutputDataIndices;
            public UInt16 NumFeatureButtonCaps;
            public UInt16 NumFeatureValueCaps;
            public UInt16 NumFeatureDataIndices;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PSP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public GUID InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }

        [DllImport("hid.dll", SetLastError = true)]
        public static extern void HidD_GetHidGuid(
            ref GUID lpHidGuid);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiGetClassDevs(
            ref GUID lpHidGuid,
            IntPtr Enumerator,
            IntPtr hwndParent,
            int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiEnumDeviceInterfaces(
            int DeviceInfoSet,
            int DeviceInfoData,
            ref  GUID lpHidGuid,
            int MemberIndex,
            ref  SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiGetDeviceInterfaceDetail(
            int DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData,
            IntPtr aPtr,
            int detailSize,
            ref int requiredSize,
            IntPtr bPtr);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern int SetupDiGetDeviceInterfaceDetail(
            int DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData,
            ref PSP_DEVICE_INTERFACE_DETAIL_DATA myPSP_DEVICE_INTERFACE_DETAIL_DATA,
            int detailSize,
            ref int requiredSize,
            IntPtr bPtr);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            uint lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            uint hTemplateFile
            );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CancelSynchronousIo(IntPtr threadHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DuplicateHandle(
            IntPtr hSourceProcessHandle,
            IntPtr hSourceHandle,
            IntPtr hTargetProcessHandle,
            out IntPtr lpTargetHandle,
            uint dwDesiredAccess,
            bool bInheritHandle,
            uint dwOptions);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidD_GetPreparsedData(
            int hObject,
            ref int pPHIDP_PREPARSED_DATA);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetCaps(
            int pPHIDP_PREPARSED_DATA,
            ref HIDP_CAPS myPHIDP_CAPS);

        [DllImport("kernel32.dll", SetLastError = true)]
        public  static extern bool ReadFile(
            int hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToRead,
            ref int lpNumberOfBytesRead,
            IntPtr ptr
            );

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern  int SetupDiDestroyDeviceInfoList(
            int DeviceInfoSet
            );

        [DllImport("hid.dll", SetLastError = true)]
        public static extern  int HidD_FreePreparsedData(
            int pPHIDP_PREPARSED_DATA
            );

        [DllImport("kernel32.dll")]
        static public extern int CloseHandle(int hObject);

        [DllImport("kernel32.dll")]
        static public extern int WriteFile(int hFile, ref byte lpBuffer, 
                        int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);
    }
}
