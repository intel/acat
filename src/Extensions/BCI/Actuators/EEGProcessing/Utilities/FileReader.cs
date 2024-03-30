////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FileReader.cs
//
// Reads data from file
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Actuators.EEG.EEGSettings;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    internal class FileReader
    {
        /// <summary>
        /// Constructor: read settings
        /// </summary>
        public FileReader()
        {
        }

        /// <summary>
        /// Read  data and markers from file stored in Actuators\\BCI\\EEGData
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="rawData"></param>
        /// <param name="triggerSignal"></param>
        /// <param name="markerValues"></param>
        /// <param name="sessionDirectory"></param>
        public void ReadDataAndMarkersFromFiles(String sessionID, out double[,] rawData, out int[] triggerSignal, out List<int> markerValues, out string sessionDirectory)
        {
            //if (_settings.Calibration_ForceRecalibrateFromFile && _settings.Calibration_CalibrationFileId != null && _settings.Calibration_CalibrationFileId != "")
            //{
            //    // Use calibration file for training classifiers
            //    sessionID = _settings.Calibration_CalibrationFileId;
            //    sessionDirectory = Path.Combine(UserManager.CurrentUserDir, _settings.Calibration_DirectoryCalibrationFiles, sessionID);
            //}
            //else
            //{
            sessionDirectory = Path.Combine(UserManager.CurrentUserDir, "Actuators\\BCI\\EEGData", sessionID);
            //}

            string fileNameMarkers = sessionID + "_markerValues.txt";
            string filePathMarkers = Path.Combine(sessionDirectory, fileNameMarkers);

            string fileNameEEG = sessionID + "_eegDataFiltered.txt";
            string filePathEEG = Path.Combine(sessionDirectory, fileNameEEG);

            // 1--------- Get Markers
            markerValues = ReadMarkersFromFile(filePathMarkers, sessionDirectory);

            // 2--------- Get EEG data
            ReadEEGDataFromFile(out rawData, out triggerSignal, filePathEEG, sessionDirectory);
        }

        /// <summary>
        ///  Reads markers from file
        /// </summary>
        /// <param name="filePathMarkers"></param>
        /// <param name="initDirectory"></param>
        /// <returns></returns>
        public List<int> ReadMarkersFromFile(String filePathMarkers = "", String initDirectory = "")
        {
            List<int> markerValues = new List<int>();

            if (filePathMarkers == "") // If not path provided, show popup window to select it
            {
                // Popup window to select file

                // Displays an OpenFileDialog so the user can select a Cursor.
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                if (initDirectory != "")
                    openFileDialog1.InitialDirectory = initDirectory;
                openFileDialog1.Filter = "markers file|*.txt";
                openFileDialog1.Title = "Select Markers File";

                // Show the Dialog.
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    filePathMarkers = openFileDialog1.FileName;
                    //initDirectory = Path.GetDirectoryName(filePathMarkers);
                }

                openFileDialog1.Dispose();
            }

            // Read marker values from file and place them in a list
            if (filePathMarkers != "" && File.Exists(filePathMarkers))
            {
                using (StreamReader sr = new StreamReader(filePathMarkers))
                {
                    while (sr.Peek() >= 0)
                    {
                        int markerValue = int.Parse(sr.ReadLine());
                        markerValues.Add(markerValue);
                    }
                }
            }
            else
            {
                Log.Debug("Markers file doesn't exist");
            }
            return markerValues;
        }

        /// <summary>
        /// Read EEG Data from file (shows popup to select file)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="initDirectory"></param>
        /// <returns></returns>
        public void ReadEEGDataFromFile(out double[,] rawData, out int[] triggerSignal, String filePath = "", String initDirectory = "")
        {
            rawData = null;
            triggerSignal = null;

            if (filePath == "")
            {
                // Popup window
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                if (initDirectory == "")
                    openFileDialog1.InitialDirectory = UserManager.CurrentUserDir;
                else
                    openFileDialog1.InitialDirectory = initDirectory;

                openFileDialog1.Filter = "eeg file|*.txt";
                openFileDialog1.Title = "Select EEG File";

                // Show the Dialog.
                // If the user clicked OK in the dialog and
                // a .CUR file was selected, open it.
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    filePath = openFileDialog1.FileName;
                }

                openFileDialog1.Dispose();
            }

            try
            {
                if (filePath != "" && File.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        int lineIdx = 0;
                        List<int> triggerSignalList = new List<int>();
                        List<double[]> rawDataList = new List<double[]>();
                        int triggervalueNextSample = 0;

                        while (sr.Peek() >= 0)
                        {
                            string line = sr.ReadLine();
                            lineIdx++;
                            if (lineIdx > BCISettingsFixed.DataParser_HeaderLinesToSkip)
                            {
                                String[] values = line.Split(',');

                                // triggerValues are flipped (with the new sensor) 1=0, 0=1
                                if (BCIActuatorSettings.Settings.DataParser_UseSoftwareTrigers)
                                {
                                    int tmpTriggerSample = Convert.ToInt32(values[BCISettingsFixed.DataParser_IdxTriggerSignal_Sw - 1]); // sw trigger is stored in last channel
                                                                                                                                         //Generate signal of 0 and 1 pulses (sw trigger is 1=on, 2=off, 0=no marker sent)
                                    switch (tmpTriggerSample)
                                    {
                                        case 1: triggervalueNextSample = 0; break;
                                        case 2: triggervalueNextSample = 1; break;
                                    }
                                    triggerSignalList.Add(triggervalueNextSample);
                                }
                                else
                                {
                                    double t = Convert.ToDouble(values[BCISettingsFixed.DataParser_IdxTriggerSignal_Hw - 1]);//Indexed from 0, value given from 1
                                    if (t == 0)
                                        triggerSignalList.Add(1);
                                    else
                                        triggerSignalList.Add(0);
                                }

                                double[] chData = new double[BCIActuatorSettings.Settings.DAQ_NumEEGChannels];
                                for (int ch = 0; ch < BCIActuatorSettings.Settings.DAQ_NumEEGChannels; ch++)
                                    chData[ch] = Convert.ToDouble(values[BCISettingsFixed.DataParser_IdxStartEEGData - 1 + ch]); //Indexed from 0, value given from 1
                                rawDataList.Add(chData);
                            }
                        }
                        int numSamples = rawDataList.Count;
                        int numChannels = BCIActuatorSettings.Settings.DAQ_NumEEGChannels;

                        rawData = new double[numChannels, numSamples];
                        for (int chIdx = 0; chIdx < numChannels; chIdx++)
                            for (int sampleIdx = 0; sampleIdx < numSamples; sampleIdx++)
                                rawData[chIdx, sampleIdx] = rawDataList[sampleIdx][chIdx];

                        triggerSignal = triggerSignalList.ToArray();
                    }
                }
                else
                {
                    Log.Debug("EEG file doesn't exist");
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }
        }
    }
}