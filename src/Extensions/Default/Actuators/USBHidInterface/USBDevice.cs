#region Author Information
// Author: Sai Prasad
// Group : XTL, IXR
#endregion

#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
#endregion

namespace ACAT.Extensions.Default.Actuators.USBHid
{
    /// <summary>
    /// Opens and reads from a USB Hid device
    /// </summary>
    public class USBDevice : IDisposable
    {
        public delegate void ReadDataNotify(byte [] data);
        public event ReadDataNotify EvtReadDataNotify;

        public delegate void DeviceDisconnected();
        public event DeviceDisconnected EvtDeviceDisconnected;

        private IntPtr _readThreadHandle;
        private String _productID;
        private String _vendorID;
        private USBInterop _usbInterop = new USBInterop();
        private Thread _readThread;
        private bool _isDisposed = false;
        private volatile bool _quitReadThread = false;

        /// <summary>
        /// Constructor. The vendor ID has to be specified. Product
        /// ID may be ""
        /// </summary>
        /// <param name="vendorID">Vendor ID of the Hid device manufacturer</param>
        /// <param name="productID">Vendor ID of the Hid device manufacturer</param>
        public USBDevice(String vendorID, String productID)
        {
            _vendorID = vendorID;
            _productID = productID;
            _readThread = null;
        }

        /// <summary>
        /// Open the Hid device
        /// </summary>
        /// <returns>true on success</returns>
        public bool Open()
        {
            bool deviceFound = false;
            String devicePath = string.Empty;
            int deviceHandle = -1;
            int retVal = -1;
            int index = 0;
            int size = 0;
            int sizeNeeded = 0;

            _usbInterop.SetupDiGetClassDevs();

            // enumarate all the hid devices till we find
            // the one we want (matching vid and pid)
            while (deviceHandle != 0)
            {
                deviceHandle = _usbInterop.SetupDiEnumDeviceInterfaces(index);
                retVal = _usbInterop.SetupDiGetDeviceInterfaceDetail(ref sizeNeeded, 0);
                size = sizeNeeded;
                retVal = _usbInterop.SetupDiGetDeviceInterfaceDetailx(ref sizeNeeded, size);

                string deviceID = _vendorID + "&" + _productID;
                if (_usbInterop.DevicePathName.IndexOf(deviceID) > 0)
                {
                    // yes we found it!

                    deviceFound = true;

                    devicePath = _usbInterop.DevicePathName;

                    _usbInterop.SetupDiEnumDeviceInterfaces(index);

                    size = 0;
                    sizeNeeded = 0;

                    _usbInterop.SetupDiGetDeviceInterfaceDetail(ref sizeNeeded, size);
                    _usbInterop.SetupDiGetDeviceInterfaceDetailx(ref sizeNeeded, size);

                    _usbInterop.CreateFile(devicePath);

                    break;
                }
                index++;
            }
            return deviceFound;
        }

        /// <summary>
        /// Starts an async read operation.  Results are notified
        /// through events
        /// </summary>
        public void BeginReadAsync()
        {
            if (_readThread == null)
            {
                _readThread = new Thread(new ThreadStart(readThreadProc));
            }
            else if (_readThread.ThreadState != ThreadState.Unstarted)
            {
                return;
            }

            _quitReadThread = false;
            _readThread.Start();
            Thread.Sleep(0);
        }

        /// <summary>
        /// Stop reading
        /// </summary>
        public void EndRead()
        {
            _quitReadThread = true;
            if (_readThreadHandle != IntPtr.Zero)
            {
                _usbInterop.CancelSynchronousIo(_readThreadHandle);
            }
            if (_readThread != null && _readThread.ThreadState == ThreadState.Running)
            {
                _readThread.Join(1000);
                _readThread.Abort();
            }
            _readThread = null;
        }

        /// <summary>
        /// Reads data from the USB device and invokes the event handlers
        /// </summary>
        private void readThreadProc()
        {
            int noDataStat = 0;
            int preParsedData = -1;

            _readThreadHandle = _usbInterop.GetCurrentThreadHandle();

            if (_usbInterop.GetPreparsedData(_usbInterop.HidHandle, ref preParsedData) != 0)
            {
                int code = _usbInterop.GetCaps(preParsedData);
                int reportLength = _usbInterop.HidCaps.InputReportByteLength;

                while (!_quitReadThread)
                {
                    byte[] data = _usbInterop.ReadFile(_usbInterop.HidCaps.InputReportByteLength);
                    if (data != null)
                    {
                        noDataStat = 0;
                        EvtReadDataNotify(data);
                    }
                    else
                    {
                        noDataStat++;
                        if (noDataStat > 10)
                        {
                            EvtDeviceDisconnected.BeginInvoke(null, null);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Close the hid device.  This has to be called
        /// for all the threads to be closed properly
        /// </summary>
        public void Close()
        {
            EndRead();
            _usbInterop.CloseFile();
        }

        /// <summary>
        /// Disposer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposer
        /// </summary>
        /// <param name="disposeManagedResources"></param>
        protected void Dispose(bool disposeManagedResources)
        {
            if (!_isDisposed)
            {
                if (disposeManagedResources)
                {
                    //dispose managed data
                }

                // dispose unmanaged data
                _usbInterop.CloseFile();

                if (_usbInterop.hDevInfo != -1)
                {
                    _usbInterop.SetupDiDestroyDeviceInfoList();
                }
                _isDisposed = true;
            }
        }
    }
}
