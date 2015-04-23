#region Author Information
// Author: Sai Prasad
// Group : XTL, IXR
#endregion

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;
#endregion

namespace ACAT.Extensions.Default.Actuators.USBHid
{
    /// <summary>
    /// Interop class into the various Windows
    /// HID and USB functions
    /// </summary>
    public class USBInterop
    {
        public int HidHandle = -1;
        public int hDevInfo = -1;
        public string DevicePathName = "";
        
        private DllImports.GUID _guid;
        private DllImports.SP_DEVICE_INTERFACE_DATA _deviceInterfaceData;
        private DllImports.PSP_DEVICE_INTERFACE_DETAIL_DATA _deviceInterfaceDetailData;
        public DllImports.HIDP_CAPS HidCaps;

        public USBInterop()
        {
            _guid = new DllImports.GUID();
        }

        public  int SetupDiGetClassDevs()
        {
            DllImports.HidD_GetHidGuid(ref _guid);
            hDevInfo = DllImports.SetupDiGetClassDevs(ref _guid, null, null, DllImports.DIGCF_INTERFACEDEVICE | DllImports.DIGCF_PRESENT);
            return hDevInfo;
        }

        public  int SetupDiEnumDeviceInterfaces(int memberIndex)
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

        public  int SetupDiGetDeviceInterfaceDetail(ref int RequiredSize, int DeviceInterfaceDetailDataSize)
        {
            int retVal = DllImports.SetupDiGetDeviceInterfaceDetail(
                                hDevInfo,
                                ref _deviceInterfaceData,
                                null,
                                DeviceInterfaceDetailDataSize,
                                ref RequiredSize,
                                null);
            return retVal;
        }

        public  int SetupDiGetDeviceInterfaceDetailx(ref int RequiredSize, int DeviceInterfaceDetailDataSize)
        {
            _deviceInterfaceDetailData = new DllImports.PSP_DEVICE_INTERFACE_DETAIL_DATA();
            _deviceInterfaceDetailData.cbSize = 5;

            int retVal = DllImports.SetupDiGetDeviceInterfaceDetail(
                                hDevInfo,
                                ref _deviceInterfaceData,
                                ref _deviceInterfaceDetailData,
                                DeviceInterfaceDetailDataSize,
                                ref RequiredSize,
                                null);
            DevicePathName = _deviceInterfaceDetailData.DevicePath;
            return retVal;
        }

        public  int CreateFile(string DeviceName)
        {

            HidHandle = DllImports.CreateFile(DeviceName, DllImports.GENERIC_READ | DllImports.GENERIC_WRITE,
                                DllImports.FILE_SHARE_READ | DllImports.FILE_SHARE_WRITE, 0, DllImports.OPEN_EXISTING, 0, 0);
            return (HidHandle == -1) ? 0 : 1;
        }

        public  int CloseFile()
        {
            if (HidHandle != -1)
            {
                DllImports.CloseHandle(HidHandle);
                HidHandle = -1;
            }
            return 1;
        }

        public  int GetPreparsedData(int hObject, ref int preParsedData)
        {
            return DllImports.HidD_GetPreparsedData(hObject, ref preParsedData);
        }

        public  int GetCaps(int preParsedData)
        {
            HidCaps = new DllImports.HIDP_CAPS();
            return DllImports.HidP_GetCaps(preParsedData, ref HidCaps);
        }

        public  byte[] ReadFile(int inputReportByteLength)
        {
            int bytesRead = 0;
            byte[] inBuffer = new byte[inputReportByteLength];
            if (DllImports.ReadFile(HidHandle, inBuffer, inputReportByteLength, ref bytesRead, null))
            {
                byte[] outBuffer = new byte[bytesRead];
                Array.Copy(inBuffer, outBuffer, bytesRead);
                return outBuffer;
            }
            else
            {
                return null;
            }
        }

        public int SetupDiDestroyDeviceInfoList()
        {
            return DllImports.SetupDiDestroyDeviceInfoList(hDevInfo);

        }

        public int InteropFreePreparsedData(int pPHIDP_PREPARSED_DATA)
        {
            return DllImports.SetupDiDestroyDeviceInfoList(pPHIDP_PREPARSED_DATA);
        }

        public void CancelSynchronousIo(IntPtr threadHandle)
        {
            DllImports.CancelSynchronousIo(threadHandle);
        }

        public IntPtr GetCurrentThreadHandle()
        {
            IntPtr handle = DllImports.GetCurrentThread();
            return DuplicateHandle(handle);
        }

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
    }
}
