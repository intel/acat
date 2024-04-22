﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace ACAT.Extensions.BCI.Common.AnimationSharp
{
    /// <summary>
    /// Caches auditlog entries in memory and dumps them to disk when
    /// the Save is called.
    /// </summary>
    public class CachedLogBCI
    {
        /// <summary>
        /// Full path to the log file
        /// </summary>
        private readonly String LogFileFullPath;

        /// <summary>
        /// Full path to the log file
        /// </summary>
        private readonly String LogFileFullPathEEG;

        private List<String> logEntries = new List<string>();

        /// <summary>
        /// Name of the audit log file
        /// </summary>
        private string LogFileName;

        public CachedLogBCI(string baseFileName, string baseDirPath = null)
        {
            string logFileFolder = string.Empty;
            if (!String.IsNullOrEmpty(baseFileName))
            {
                LogFileName = baseFileName + ".csv";
            }
            else
            {
                LogFileName = "CachedLog.csv";
            }

            logFileFolder = CreateDefaulLogPath(baseFileName);
            LogFileFullPath = Path.Combine(logFileFolder, Path.GetFileNameWithoutExtension(LogFileName) + CoreGlobals.LogFileSuffix + Path.GetExtension(LogFileName));
            if (!String.IsNullOrEmpty(baseDirPath) && Directory.Exists(baseDirPath))
            {
                LogFileFullPathEEG = Path.Combine(baseDirPath, Path.GetFileNameWithoutExtension(LogFileName) + CoreGlobals.LogFileSuffix + Path.GetExtension(LogFileName));  
            }
        }

        private string CreateDefaulLogPath(string baseFileName)
        {
            string logFileFolder = FileUtils.GetLogsDir();
            try
            {
                if (!Directory.Exists(logFileFolder))
                {
                    Directory.CreateDirectory(logFileFolder);
                }
            }
            catch
            {
                logFileFolder = SmartPath.ApplicationPath;
            }
            return logFileFolder;
        }

        public void LogEntry(String eventType, String logEntry)
        {
            var timeStamp = getTimeStamp();

            logEntries.Add(timeStamp + ", " + eventType + ", " + logEntry);
        }

        public bool Save()
        {
            StreamWriter streamWriter = null;
            Save(LogFileFullPath, streamWriter);
            Save(LogFileFullPathEEG, streamWriter);
            return true;
        }

        private void Save(string logFilePath, StreamWriter streamWriter)
        {
            try
            {
                streamWriter = new StreamWriter(logFilePath, true);
                foreach (var str in logEntries)
                {
                    streamWriter.WriteLine(str);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
            finally
            {
                streamWriter?.Flush();
                streamWriter?.Close();
                streamWriter?.Dispose();
            }
        }
        private String getTimeStamp()
        {
            DateTime now = DateTime.UtcNow;

            var elapsed = now - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();

            String elapsedTime = ((int)(elapsed.TotalMilliseconds / 1000)).ToString() + "." + (int)(elapsed.TotalMilliseconds % 1000);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpochUtc = (int)t.TotalSeconds;
            double milliSecondsSinceEpochUtc = t.TotalMilliseconds;

            t = DateTime.Now - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            double milliSecondsSinceEpoch = t.TotalMilliseconds;

            return now.ToLocalTime() + "," + elapsedTime + "," + secondsSinceEpochUtc + ", " + secondsSinceEpoch + ", "
                        + milliSecondsSinceEpochUtc + ", " + milliSecondsSinceEpoch;
        }
    }
}
