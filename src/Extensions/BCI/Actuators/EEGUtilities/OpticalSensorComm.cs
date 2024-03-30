////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// OpticalSensorCom.cs
//
// Handles communication with the optical sensor
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGUtils
{
    public static class OpticalSensorComm
    {
        private static SerialComm _serialComm;

        private const String cmdEnableLight = "enable light 1";
        private const String cmdStartStreaming = "start";
        private const String cmdStopStreaming = "stop";
        private const String VID = "0483";
        private const String PID = "5740";
        private static StringBuilder sbResponse = new StringBuilder();

        public delegate void OpticalSensorDataReceiveError(String msg);

        public static event OpticalSensorDataReceiveError EvtOpticalSensorDataReceiveError;

        private static volatile bool errorShown = false;

        public static void Open()
        {
            openSerialPort();

            _serialComm.SendCommand(cmdEnableLight);
        }

        public static void StartStreaming()
        {
            _serialComm.SendCommand(cmdStartStreaming);
        }

        public static void StopStreaming()
        {
            openSerialPort();

            _serialComm.SendCommand(cmdStopStreaming);
        }

        public static void SetLuxThreshold(int threshold)
        {
            var cmd = "param light Lux " + threshold;

            openSerialPort();

            _serialComm.SendCommand(cmd);
        }

        public static bool IsConnected()
        {
            return _serialComm != null && _serialComm.IsOpen();
        }

        public static String PortName
        {
            get
            {
                return (_serialComm != null) ? _serialComm.PortName : String.Empty;
            }
            private set { }
        }

        public static void SendCommand(String command)
        {
            openSerialPort();

            _serialComm.SendCommand(command);
        }

        public static void Close()
        {
            if (_serialComm != null && _serialComm.IsOpen())
            {
                _serialComm.Close();
            }

            _serialComm = null;
        }

        private static void openSerialPort()
        {
            if (_serialComm != null && _serialComm.IsOpen())
            {
                return;
            }

            errorShown = false;

            var errorMsg = "Optical sensor not detected.";

            var comPorts = SerialComm.GetComPortNames(VID, PID);

            Log.Debug("OPTSEN: Number of COM ports: " + comPorts.Count);

            if (comPorts.Count == 0)
            {
                throw new Exception(errorMsg);
            }

            var activePorts = SerialComm.GetActivePortNames();

            Log.Debug("OPTSEN: Number of active ports: " + activePorts.Count);

            SerialComm serialComm = null;

            foreach (var port in comPorts)
            {
                if (activePorts.Contains(port))
                {
                    serialComm = tryOpen(port);

                    if (serialComm != null)
                    {
                        Log.Debug("OPTSEN: Successfully detected port " + port);
                        break;
                    }
                }
            }

            if (serialComm != null)
            {
                _serialComm = serialComm;
            }
            else
            {
                throw new Exception(errorMsg);
            }
        }

        private static SerialComm tryOpen(String port)
        {
            SerialComm serialComm = null;

            try
            {
                Log.Debug("OPTSEN: Trying port " + port);

                serialComm = new SerialComm(port);

                serialComm.EvtSerialPortDataReceiveError += _serialComm_EvtSerialPortDataReceiveError;
                serialComm.EvtSerialPortDataReceived += _serialComm_EvtSerialPortDataReceived;

                serialComm.Open();

                sbResponse.Clear();

                Log.Debug("OPTSEN: Sending config command to port  " + port);

                serialComm.SendCommand("config");

                Task.Delay(2000).Wait();

                //serialComm.EvtSerialPortDataReceived -= _serialComm_EvtSerialPortDataReceived;

                if (configResponseReceived())
                {
                    Log.Debug("OPTSEN: Received config command response from port " + port);
                    return serialComm;
                }
                else
                {
                    Log.Debug("OPTSEN: Did not receive config command response from port " + port);

                    serialComm.EvtSerialPortDataReceived -= _serialComm_EvtSerialPortDataReceived;
                    serialComm.EvtSerialPortDataReceiveError -= _serialComm_EvtSerialPortDataReceiveError;

                    serialComm.Close();
                }
            }
            catch
            {
            }

            return null;
        }

        private static bool configResponseReceived()
        {
            var str = sbResponse.ToString().ToLower();

            Log.Debug("OPTSEN: Config command response " + str);

            return (str.Contains("sample_latency") || str.Contains("current configuration") ||
                str.Contains("light:") || str.Contains("format = ") || str.Contains("destination"));
        }

        private static void _serialComm_EvtSerialPortDataReceiveError(object sender, string errmsg)
        {
            if (!errorShown)
            {
                errorShown = true;
                EvtOpticalSensorDataReceiveError?.Invoke(errmsg);
            }
        }

        private static void _serialComm_EvtSerialPortDataReceived(object sender, string data)
        {
            //Log.Debug(data.Length + ":[" + data + "]");
            sbResponse.Append(data);
        }
    }
}