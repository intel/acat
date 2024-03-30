////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// SerialCom.cs
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGUtils
{
    internal class SerialComm
    {
        private const int DefaultBaudRate = 115200;
        private long _error = 0;
        private string _portName;
        private SerialPort _serialPort;

        public SerialComm(String portName)
        {
            _portName = portName;
        }

        public delegate void SerialPortDataReceivedDelegate(object sender, String data);

        public delegate void SerialPortDataReceiveErrorEventDelegate(object sender, string errmsg);

        public event SerialPortDataReceivedDelegate EvtSerialPortDataReceived;

        public event SerialPortDataReceiveErrorEventDelegate EvtSerialPortDataReceiveError;

        public String PortName
        {
            get
            {
                return _portName;
            }
            private set { }
        }

        private bool errorOccurred
        {
            get
            {
                return Interlocked.Read(ref _error) != 0;
            }
        }

        public static List<string> GetActivePortNames()
        {
            var portNames = SerialPort.GetPortNames();

            return portNames.ToList();
        }

        // https://www.codeproject.com/Tips/349002/Select-a-USB-Serial-Device-via-its-VID-PID
        public static List<string> GetComPortNames(String VID, String PID)
        {
            var pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);

            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comPortNames = new List<string>();

            RegistryKey regKey1 = Registry.LocalMachine;
            RegistryKey regKey2 = regKey1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            foreach (String subKeyName2 in regKey2.GetSubKeyNames())
            {
                RegistryKey regKey3 = regKey2.OpenSubKey(subKeyName2);
                foreach (String subKeyName3 in regKey3.GetSubKeyNames())
                {
                    if (regex.Match(subKeyName3).Success)
                    {
                        RegistryKey regKey4 = regKey3.OpenSubKey(subKeyName3);
                        foreach (String subKeyName4 in regKey4.GetSubKeyNames())
                        {
                            RegistryKey regKey5 = regKey4.OpenSubKey(subKeyName4);
                            RegistryKey regKey6 = regKey5.OpenSubKey("Device Parameters");
                            comPortNames.Add((string)regKey6.GetValue("PortName"));
                        }
                    }
                }
            }

            return comPortNames;
        }

        public void Close()
        {
            clearError();

            if (_serialPort == null || !_serialPort.IsOpen)
            {
                throw new Exception("Serial port is not open");
            }

            _serialPort.DataReceived -= serialCom_DataReceived;

            _serialPort.Close();

            _serialPort.Dispose();

            _serialPort = null;
        }

        public bool IsOpen()
        {
            return _serialPort != null && _serialPort.IsOpen && !errorOccurred;
        }

        public void Open(int baudRate = DefaultBaudRate)
        {
            if (errorOccurred)
            {
                clearError();
            }

            if (_serialPort != null)
            {
                if (IsOpen())
                {
                    throw new Exception("Serial port is already open");
                }

                Close();
            }

            var portNames = SerialPort.GetPortNames();

            _serialPort = new SerialPort(_portName)
            {
                BaudRate = baudRate
            };

            _serialPort.DataReceived += serialCom_DataReceived;

            _serialPort.Open();
        }

        public void SendCommand(String command)
        {
            if (_serialPort == null || !IsOpen())
            {
                throw new Exception("Serial port is not open");
            }

            _serialPort.WriteLine(command);
        }

        private void clearError()
        {
            Interlocked.Exchange(ref _error, 0);
        }

        private void DataReceivedCallback(IAsyncResult result)
        {
            try
            {
                int bytesRead = _serialPort.BaseStream.EndRead(result);
                byte[] buffer = (byte[])result.AsyncState;

                if (bytesRead > 0)
                {
                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    // Log.Debug("OPTSEN: Received data: " + receivedData);
                    EvtSerialPortDataReceived?.Invoke(this, receivedData);
                }

                if (!errorOccurred)
                {
                    _serialPort.BaseStream.BeginRead(buffer, 0, buffer.Length, DataReceivedCallback, buffer);
                }
            }
            catch (Exception ex)
            {
                if (!errorOccurred)
                {
                    Log.Debug("OPTSEN: Callback Error in data received callback: " + ex.Message);
                    setError();
                    notifySerialPortDataReceiveError(ex.Message);
                }
            }
        }

        private void notifySerialPortDataReceiveError(String errMsg)
        {
            EvtSerialPortDataReceiveError?.Invoke(this, errMsg);
        }

        private void serialCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            byte[] buffer = new byte[sp.BytesToRead];

            try
            {
                sp.BaseStream.BeginRead(buffer, 0, buffer.Length, DataReceivedCallback, buffer);
            }
            catch (Exception ex)
            {
                if (!errorOccurred)
                {
                    Log.Debug("OPTSEN: BeginRead Error in data received callback: " + ex.Message);
                    notifySerialPortDataReceiveError(ex.Message);
                    setError();
                }
            }
        }

        private void setError()
        {
            Interlocked.Increment(ref _error);
        }
    }
}