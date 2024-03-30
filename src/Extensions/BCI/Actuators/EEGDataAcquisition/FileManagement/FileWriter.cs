////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FileWriter.cs
//
// Handles file writting for BCI data
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition
{
    public class FileWriter
    {
        /// <summary>
        /// Directory for session
        /// </summary>
        public String sessionDirectory = "";

        private StreamWriter sw_dataRaw;
        private StreamWriter sw_dataFiltered;
        private StreamWriter sw_markerValues;

        public bool isFileOpened;

        /// <summary>
        /// Constructor: generate files for the session
        /// </summary>
        public FileWriter()
        {
            // Create filename and directory
            string sessionID = "EEG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string dataDir = Path.Combine(UserManager.CurrentUserDir, "Actuators\\BCI\\" + BCIActuatorSettings.Settings.DAQ_OutputDirectory);

            // Create files
            CreateFiles(dataDir, sessionID);
            isFileOpened = true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sessionID"></param>
        public FileWriter(String sessionID)
        {
            //Create directory
            string dataDir = Path.Combine(UserManager.CurrentUserDir, "Actuators\\BCI\\" + BCIActuatorSettings.Settings.DAQ_OutputDirectory);

            // Create files
            CreateFiles(dataDir, sessionID);
            isFileOpened = true;
        }

        /// <summary>
        /// Create files
        /// </summary>
        /// <param name="dataDir"> directory where files will be created </param>
        /// <param name="sessionID"> ID of the session</param>
        /// <returns></returns>
        public bool CreateFiles(String dataDir, string sessionID)
        {
            if (!Directory.Exists(dataDir))
                Directory.CreateDirectory(dataDir);

            // filenames
            sessionDirectory = Path.Combine(dataDir, sessionID);
            string fileName_eegRaw = sessionID + FileNames.suffix_eegRaw;
            string fileName_eegFiltered = sessionID + FileNames.suffix_eegFiltered;
            string fileName_markerValues = sessionID + FileNames.suffix_markerValues;

            // Create directory
            Directory.CreateDirectory(sessionDirectory);

            // Create streamwritter for each file
            sw_dataRaw = new StreamWriter(Path.Combine(sessionDirectory, fileName_eegRaw));
            sw_dataFiltered = new StreamWriter(Path.Combine(sessionDirectory, fileName_eegFiltered));
            sw_markerValues = new StreamWriter(Path.Combine(sessionDirectory, fileName_markerValues));

            return true;
        }

        /// <summary>
        /// Write headers in file
        /// </summary>
        /// <param name="sampleRate"></param>
        /// <param name="indEEGChannels"></param>
        /// <param name="indOpticalSensor"></param>
        public void WriteHeaders(int sampleRate, int[] indEEGChannels, int indOpticalSensor)
        {
            WriteHeaderToFile(sw_dataRaw, sampleRate, indEEGChannels, indOpticalSensor, true);
            WriteHeaderToFile(sw_dataFiltered, sampleRate, indEEGChannels, indOpticalSensor, false);
        }

        /// <summary>
        /// Write Header to file
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="sampleRate"></param>
        /// <param name="indEEGChannels"></param>
        /// <param name="indOpticalSensor"></param>
        /// <param name="isRawData"></param>
        private void WriteHeaderToFile(StreamWriter sw, int sampleRate, int[] indEEGChannels, int indOpticalSensor, bool isRawData)
        {
            if (isFileOpened)
            {
                sw.WriteLine("%");
                sw.WriteLine("%  OpenBCI-Brainflow EEG Data");
                sw.WriteLine("%  Raw data?  " + isRawData);
                sw.WriteLine("%  Sample rate: " + sampleRate);
                sw.WriteLine("%  Indices EEG channels: [" + String.Join(",", indEEGChannels) + "]");
                sw.WriteLine("%  Index Optical sensor channel: " + indOpticalSensor);
                sw.WriteLine("%");
            }
        }

        /// <summary>
        /// Write raw data to file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isRawData"></param>
        public void WriteRawDataToFile(double[,] data)
        {
            WriteDataToFile(sw_dataRaw, data);
        }

        /// <summary>
        /// Write raw data to file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isRawData"></param>
        public void WriteFilteredDataToFile(double[,] data)
        {
            WriteDataToFile(sw_dataFiltered, data);
        }

        /// <summary>
        /// Write data to file
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="data"></param>
        private void WriteDataToFile(StreamWriter sw, double[,] data)
        {
           
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                EnterCriticalSection(_syncObj);
                if (isFileOpened && data != null && data.Length > 0)
                {
                    int numChannels = data.GetLength(0);
                    int numSamples = data.GetLength(1);

                    for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                    {
                        for (int channelIdx = 0; channelIdx < numChannels; channelIdx++)
                        {
                            try
                            {
                                //sw.Write(data[channelIdx, sampleIdx]);
                                stringBuilder.Append(data[channelIdx, sampleIdx]);
                            }
                            catch (Exception ex)
                            {
                                Log.Debug(data[channelIdx, sampleIdx].ToString());
                                Log.Debug(ex.ToString());
                            }
                            if (channelIdx < numChannels - 1)
                                stringBuilder.Append(", ");
                            //sw.Write(", ");
                        }
                        //sw.Write("\n");
                        stringBuilder.Append("\n");
                    }
                    sw.Write(stringBuilder.ToString());
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
            finally
            {
                ExitCriticalSection(_syncObj);
            }
        }

        /// <summary>
        /// Used the synchronization for multiple calls
        /// </summary>
        private readonly object _syncObj = new object();

        private void EnterCriticalSection(object syncObj)
        {
            while (!TryEnter(_syncObj))
            {
                if (Application.MessageLoop)
                {
                    Application.DoEvents();
                }
            }
        }

        /// <summary>
        /// Tries to enter a critical section
        /// </summary>
        /// <param name="syncObj">Object used to synchronoze</param>
        /// <returns>true if entered successfully</returns>
        private bool TryEnter(Object syncObj)
        {
            bool lockTaken = false;
            Monitor.TryEnter(syncObj, ref lockTaken);
            return lockTaken;
        }

        private void ExitCriticalSection(object syncObj)
        {
            Monitor.Exit(syncObj);
        }

        /// <summary>
        /// Write single marker to markers file
        /// </summary>
        /// <param name="marker"></param>
        public void WriteMarkerValueToFile(int marker)
        {
            if (isFileOpened)
                sw_markerValues.WriteLine("{0}", marker);
        }

        /// <summary>
        /// Write array of markers to markers file
        /// </summary>
        /// <param name="markers"></param>
        public void WriteMarkerValueToFile(List<int> markers)
        {
            if (isFileOpened)
            {
                foreach (double markerValue in markers)
                {
                    sw_markerValues.WriteLine("{0}", markerValue);
                }
                sw_markerValues.Flush();
            }
        }

        /// <summary>
        /// Write Set of markers to markers file
        /// </summary>
        /// <param name="markers"></param>
        public void WriteMarkerValuesToFile(List<int[]> markers)
        {
            if (isFileOpened)
            {
                foreach (int[] markerValues in markers)
                {
                    foreach (Object v in markerValues)
                    {
                        sw_markerValues.Write(v.ToString() + " ");
                    }
                    sw_markerValues.Write("\n");
                }
                sw_markerValues.Flush();
            }
        }

        /// <summary>
        /// Close all files
        /// </summary>
        public void CloseFiles()
        {
            if (isFileOpened)
            {
                sw_dataRaw.Close();
                sw_dataFiltered.Close();
                sw_markerValues.Close();
                isFileOpened = false;
            }
        }
    }
}