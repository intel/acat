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
using Microsoft.Win32.SafeHandles;
#endregion

namespace ACAT.Extensions.Default.Actuators.USBHid
{
    /// <summary>
    /// DLL imports from Windows kernel32, setupapi and hid DLL's
    /// </summary>
    public class DllImports
    {
        public const int DIGCF_PRESENT = 0x00000002;
        public const int DIGCF_DEVICEINTERFACE = 0x00000010;
        public const int DIGCF_INTERFACEDEVICE = 0x00000010;

        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;

        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;

        public const int OPEN_EXISTING = 3;
        
        public const int INVALID_HANDLE_VALUE = -1;
        public const int ERROR_INVALID_HANDLE = 6;

        public const int FILE_FLAG_OVERLAPED = 0x40000000;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;

        public const int DUPLICATE_SAME_ACCESS = 0x00000002;

        // GUID structure
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct GUID
        {
            public int Data1;
            public System.UInt16 Data2;
            public System.UInt16 Data3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] data4;
        }

        // Device interface data
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public GUID InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }

        // Device interface detail data
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct PSP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }

        // HIDP_CAPS
        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct HIDP_CAPS
        {
            public System.UInt16 Usage;
            public System.UInt16 UsagePage;
            public System.UInt16 InputReportByteLength;
            public System.UInt16 OutputReportByteLength;
            public System.UInt16 FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public System.UInt16[] Reserved;
            public System.UInt16 NumberLinkCollectionNodes;
            public System.UInt16 NumberInputButtonCaps;
            public System.UInt16 NumberInputValueCaps;
            public System.UInt16 NumberInputDataIndices;
            public System.UInt16 NumberOutputButtonCaps;
            public System.UInt16 NumberOutputValueCaps;
            public System.UInt16 NumberOutputDataIndices;
            public System.UInt16 NumberFeatureButtonCaps;
            public System.UInt16 NumberFeatureValueCaps;
            public System.UInt16 NumberFeatureDataIndices;
        }

        // Structures in the union belonging to HIDP_VALUE_CAPS (see below)

        [DllImport("hid.dll", SetLastError = true)]
        public static extern unsafe void HidD_GetHidGuid(
            ref GUID lpHidGuid);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern unsafe int SetupDiGetClassDevs(
            ref GUID lpHidGuid,
            int* Enumerator,
            int* hwndParent,
            int Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern unsafe int SetupDiEnumDeviceInterfaces(
            int DeviceInfoSet,
            int DeviceInfoData,
            ref  GUID lpHidGuid,
            int MemberIndex,
            ref  SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern unsafe int SetupDiGetDeviceInterfaceDetail(
            int DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData,
            int* aPtr,
            int detailSize,
            ref int requiredSize,
            int* bPtr);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern unsafe int SetupDiGetDeviceInterfaceDetail(
            int DeviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA lpDeviceInterfaceData,
            ref PSP_DEVICE_INTERFACE_DETAIL_DATA myPSP_DEVICE_INTERFACE_DETAIL_DATA,
            int detailSize,
            ref int requiredSize,
            int* bPtr);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int CreateFile(
            string lpFileName,							// file name
            uint dwDesiredAccess,						// access mode
            uint dwShareMode,							// share mode
            uint lpSecurityAttributes,					// security attirbutes
            uint dwCreationDisposition,					// how to create
            uint dwFlagsAndAttributes,					// file attributes
            uint hTemplateFile							// handle to template file
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
        public unsafe static extern int HidD_GetPreparsedData(
            int hObject,								// IN HANDLE  HidDeviceObject,
            ref int pPHIDP_PREPARSED_DATA);				// OUT PHIDP_PREPARSED_DATA  *PreparsedData

        [DllImport("hid.dll", SetLastError = true)]
        public unsafe static extern int HidP_GetCaps(
            int pPHIDP_PREPARSED_DATA,					// IN PHIDP_PREPARSED_DATA  PreparsedData,
            ref HIDP_CAPS myPHIDP_CAPS);				// OUT PHIDP_CAPS  Capabilities

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern bool ReadFile(
            int hFile,						// handle to file
            byte[] lpBuffer,				// data buffer
            int nNumberOfBytesToRead,		// number of bytes to read
            ref int lpNumberOfBytesRead,	// number of bytes read
            int* ptr
            );

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern unsafe int SetupDiDestroyDeviceInfoList(
            int DeviceInfoSet				// IN HDEVINFO  DeviceInfoSet
            );

        [DllImport("hid.dll", SetLastError = true)]
        public static extern unsafe int HidD_FreePreparsedData(
            int pPHIDP_PREPARSED_DATA			// IN PHIDP_PREPARSED_DATA  PreparsedData
            );

        [DllImport("kernel32.dll")]
        static public extern int CloseHandle(int hObject);

        [DllImport("kernel32.dll")]
        static public extern int WriteFile(int hFile, ref byte lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int lpOverlapped);
    }
}
