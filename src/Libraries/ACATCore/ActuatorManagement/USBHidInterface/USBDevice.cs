////////////////////////////////////////////////////////////////////////////
// <copyright file="USBDevice.cs" company="Intel Corporation">
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
using System.Threading;

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
    /// Opens a HID device and reads data from it
    /// </summary>
    public class USBDevice : IDisposable
    {
        /// <summary>
        /// The USB PID
        /// </summary>
        private readonly String _productId;

        /// <summary>
        /// USB Interop object to invoke usb functions
        /// </summary>
        private readonly USBInterop _usbInterop = new USBInterop();

        /// <summary>
        /// The USB VID
        /// </summary>
        private readonly String _vendorId;

        /// <summary>
        /// Has this object been disposed off yet?
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Inform the read thread to quit
        /// </summary>
        private volatile bool _quitReadThread;

        /// <summary>
        /// Data reader thread
        /// </summary>
        private Thread _readThread;

        /// <summary>
        /// The thread handle for data reader
        /// </summary>
        private IntPtr _readThreadHandle;

        /// <summary>
        /// Initializes a new instance of the class..
        /// The vendor ID has to be specified. Product ID may be ""
        /// </summary>
        /// <param name="vendorId">Vendor ID of the Hid device manufacturer</param>
        /// <param name="productId">Vendor ID of the Hid device manufacturer</param>
        public USBDevice(String vendorId, String productId)
        {
            _vendorId = vendorId;
            _productId = productId;
            _readThread = null;
        }

        /// <summary>
        /// For the event raised when the USB device is disconnected
        /// </summary>
        public delegate void DeviceDisconnected();

        /// <summary>
        /// For the event raised to notify incoming data
        /// </summary>
        /// <param name="data">the data read</param>
        public delegate void ReadDataNotify(byte[] data);

        /// <summary>
        /// Event raised when the device is disconnected
        /// </summary>
        public event DeviceDisconnected EvtDeviceDisconnected;

        /// <summary>
        /// Event raised to notify incoming data
        /// </summary>
        public event ReadDataNotify EvtReadDataNotify;

        /// <summary>
        /// Starts an async read operation.  Results are notified
        /// through events
        /// </summary>
        public void BeginReadAsync()
        {
            if (_readThread == null)
            {
                _readThread = new Thread(readThreadProc);
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
        /// Close the hid device.  This has to be called
        /// for all the threads to be closed properly
        /// </summary>
        public void Close()
        {
            EndRead();
            _usbInterop.CloseFile();
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
        /// Opens the Hid device
        /// </summary>
        /// <returns>true on success</returns>
        public bool Open()
        {
            bool deviceFound = false;
            String devicePath = string.Empty;
            int deviceHandle = -1;
            int index = 0;
            int size = 0;
            int sizeNeeded = 0;

            _usbInterop.SetupDiGetClassDevs();

            // enumarate all the hid devices till we find
            // the one we want (matching vid and pid)
            while (deviceHandle != 0)
            {
                deviceHandle = _usbInterop.SetupDiEnumDeviceInterfaces(index);
                _usbInterop.SetupDiGetDeviceInterfaceDetail(ref sizeNeeded, 0);
                size = sizeNeeded;
                _usbInterop.SetupDiGetDeviceInterfaceDetailEx(ref sizeNeeded, size);

                string deviceID = _vendorId + "&" + _productId;

                if (_usbInterop.DevicePathName.IndexOf(deviceID) > 0) // device found!
                {
                    deviceFound = true;

                    devicePath = _usbInterop.DevicePathName;

                    _usbInterop.SetupDiEnumDeviceInterfaces(index);

                    size = 0;
                    sizeNeeded = 0;

                    _usbInterop.SetupDiGetDeviceInterfaceDetail(ref sizeNeeded, size);
                    _usbInterop.SetupDiGetDeviceInterfaceDetailEx(ref sizeNeeded, size);

                    _usbInterop.CreateFile(devicePath);

                    break;
                }
                index++;
            }
            return deviceFound;
        }

        /// <summary>
        /// Disposer
        /// </summary>
        /// <param name="disposeManagedResources">dispose managed resources</param>
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

        /// <summary>
        /// Notifies subscribers that the device disconnected
        /// </summary>
        private void notifyDeviceDisconnected()
        {
            if (EvtDeviceDisconnected != null)
            {
                EvtDeviceDisconnected.BeginInvoke(null, null);
            }
        }

        /// <summary>
        /// Notifies subscribers of data read
        /// </summary>
        /// <param name="data">data</param>
        private void notifyReadData(byte[] data)
        {
            if (EvtReadDataNotify != null)
            {
                EvtReadDataNotify(data);
            }
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
                        notifyReadData(data);
                    }
                    else
                    {
                        noDataStat++;
                        if (noDataStat > 10)
                        {
                            notifyDeviceDisconnected();
                            break;
                        }
                    }
                }
            }
        }
    }
}