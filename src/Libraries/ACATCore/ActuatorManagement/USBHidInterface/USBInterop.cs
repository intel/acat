////////////////////////////////////////////////////////////////////////////
// <copyright file="USBInterop.cs" company="Intel Corporation">
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

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Interop methods into Windows HID and USB functions
    /// </summary>
    public class USBInterop
    {
        /// <summary>
        /// Path to the device
        /// </summary>
        public string DevicePathName = "";

        /// <summary>
        /// Handle to device info
        /// </summary>
        public int hDevInfo = -1;

        public DllImports.HIDP_CAPS HidCaps;

        /// <summary>
        /// Handle to the HID device
        /// </summary>
        public int HidHandle = -1;

        private DllImports.SP_DEVICE_INTERFACE_DATA _deviceInterfaceData;
        private DllImports.PSP_DEVICE_INTERFACE_DETAIL_DATA _deviceInterfaceDetailData;
        private DllImports.GUID _guid;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public USBInterop()
        {
            _guid = new DllImports.GUID();
        }

        /// <summary>
        /// Cancels synchronous io
        /// </summary>
        public void CancelSynchronousIo(IntPtr threadHandle)
        {
            DllImports.CancelSynchronousIo(threadHandle);
        }

        /// <summary>
        /// Closes device
        /// </summary>
        public int CloseFile()
        {
            if (HidHandle != -1)
            {
                DllImports.CloseHandle(HidHandle);
                HidHandle = -1;
            }
            return 1;
        }

        /// <summary>
        /// Opens device
        /// </summary>
        /// <param name="DeviceName">fullpath to the device</param>
        /// <returns>-1 on error, handle otherwise</returns>
        public int CreateFile(string DeviceName)
        {
            HidHandle = DllImports.CreateFile(DeviceName,
                                        DllImports.GENERIC_READ | DllImports.GENERIC_WRITE,
                                        DllImports.FILE_SHARE_READ | DllImports.FILE_SHARE_WRITE,
                                        0, DllImports.OPEN_EXISTING, 0, 0);
            return (HidHandle == -1) ? 0 : 1;
        }

        /// <summary>
        /// Creates a duplicate of the handle
        /// </summary>
        public IntPtr DuplicateHandle(IntPtr sourceHandle)
        {
            IntPtr newHandle;

            IntPtr currentProcess = DllImports.GetCurrentProcess();
            bool result = DllImports.DuplicateHandle(
                                    currentProcess,
                                    sourceHandle,
                                    currentProcess,
                                    out newHandle,
                                    0,
                                    false,
                                    DllImports.DUPLICATE_SAME_ACCESS);
            if (!result)
            {
                return IntPtr.Zero;
            }

            return newHandle;
        }

        /// <summary>
        /// Gets HID caps
        /// </summary>
        public int GetCaps(int preParsedData)
        {
            HidCaps = new DllImports.HIDP_CAPS();
            return DllImports.HidP_GetCaps(preParsedData, ref HidCaps);
        }

        /// <summary>
        /// Gets the current thread handle
        /// </summary>
        public IntPtr GetCurrentThreadHandle()
        {
            IntPtr handle = DllImports.GetCurrentThread();
            return DuplicateHandle(handle);
        }

        /// <summary>
        /// Gets raw data
        /// </summary>
        public int GetPreparsedData(int hObject, ref int preParsedData)
        {
            return DllImports.HidD_GetPreparsedData(hObject, ref preParsedData);
        }

        /// <summary>
        /// Frees parsed data
        /// </summary>
        public int InteropFreePreparsedData(int pPHIDP_PREPARSED_DATA)
        {
            return DllImports.SetupDiDestroyDeviceInfoList(pPHIDP_PREPARSED_DATA);
        }

        /// <summary>
        /// Reads data from the device
        /// </summary>
        public byte[] ReadFile(int inputReportByteLength)
        {
            int bytesRead = 0;
            byte[] inBuffer = new byte[inputReportByteLength];
            if (DllImports.ReadFile(HidHandle, inBuffer, inputReportByteLength, ref bytesRead, IntPtr.Zero))
            {
                byte[] outBuffer = new byte[bytesRead];
                Array.Copy(inBuffer, outBuffer, bytesRead);
                return outBuffer;
            }

            return null;
        }

        /// <summary>
        /// Destroys the device info list
        /// </summary>
        public int SetupDiDestroyDeviceInfoList()
        {
            return DllImports.SetupDiDestroyDeviceInfoList(hDevInfo);
        }

        /// <summary>
        /// Enumerates device interfaces
        /// </summary>
        public int SetupDiEnumDeviceInterfaces(int memberIndex)
        {
            _deviceInterfaceData = new DllImports.SP_DEVICE_INTERFACE_DATA();
            _deviceInterfaceData.cbSize = Marshal.SizeOf(_deviceInterfaceData);
            int retVal = DllImports.SetupDiEnumDeviceInterfaces(
                                hDevInfo,
                                0,
                                ref  _guid,
                                memberIndex,
                                ref _deviceInterfaceData);
            return retVal;
        }

        /// <summary>
        /// Gets class devs
        /// </summary>
        /// <returns>Handle to the device info object</returns>
        public int SetupDiGetClassDevs()
        {
            DllImports.HidD_GetHidGuid(ref _guid);
            hDevInfo = DllImports.SetupDiGetClassDevs(ref _guid,
                                                    IntPtr.Zero,
                                                    IntPtr.Zero,
                                                    DllImports.DIGCF_INTERFACEDEVICE | DllImports.DIGCF_PRESENT);
            return hDevInfo;
        }

        /// <summary>
        /// Gets device interface detail
        /// </summary>
        public int SetupDiGetDeviceInterfaceDetail(ref int requiredSize, int deviceInterfaceDetailDataSize)
        {
            int retVal = DllImports.SetupDiGetDeviceInterfaceDetail(
                                hDevInfo,
                                ref _deviceInterfaceData,
                                IntPtr.Zero,
                                deviceInterfaceDetailDataSize,
                                ref requiredSize,
                                IntPtr.Zero);
            return retVal;
        }

        /// <summary>
        /// Gets device interface detail
        /// </summary>
        public int SetupDiGetDeviceInterfaceDetailEx(ref int requiredSize, int deviceInterfaceDetailDataSize)
        {
            _deviceInterfaceDetailData = new DllImports.PSP_DEVICE_INTERFACE_DETAIL_DATA { cbSize = 5 };

            int retVal = DllImports.SetupDiGetDeviceInterfaceDetail(
                                hDevInfo,
                                ref _deviceInterfaceData,
                                ref _deviceInterfaceDetailData,
                                deviceInterfaceDetailDataSize,
                                ref requiredSize,
                                IntPtr.Zero);
            DevicePathName = _deviceInterfaceDetailData.DevicePath;
            return retVal;
        }
    }
}