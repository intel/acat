////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    ///  Has functions to track performance monitor stats such as 
    ///  free memory, handles, page file bytes, processor time etc
    /// </summary>
    public static class PerfMon
    {
        private const int cpuCounterIterations = 50;
        private const int MB = 1048576;
        private static PerformanceCounter committedMemCounter;
        private static PerformanceCounter freeMemCounter;
        private static PerformanceCounter handleCountCounter;
        private static volatile bool inTimer;
        private static PerformanceCounter pageFileBytesCounter;
        private static PerformanceCounter privateBytesCounter;
        private static PerformanceCounter processorTimeCounter;
        private static Timer timer;
        private static bool timerPaused;
        private static bool timerStarted;

        static PerfMon()
        {
            try
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

                if (!String.IsNullOrEmpty(CoreGlobals.AppId))
                {
                    PerfMonLogFileName = CoreGlobals.AppId + "_PerfMon" + ".csv";
                }
                else
                {
                    PerfMonLogFileName = "ACATPerfMon.csv";
                }

                PerfMonLogFileName = Path.Combine(logFileFolder, PerfMonLogFileName);
                PerfMonLogFileName = Path.Combine(logFileFolder, Path.GetFileNameWithoutExtension(PerfMonLogFileName) + CoreGlobals.LogFileSuffix + Path.GetExtension(PerfMonLogFileName));

                var category = PerformanceCounterCategory.GetCategories();
                var currentProcess = Process.GetCurrentProcess().ProcessName;

                freeMemCounter = new PerformanceCounter(categoryName: "Memory", counterName: "Available MBytes");
                committedMemCounter = new PerformanceCounter(categoryName: "Memory", counterName: "Committed Bytes");
                privateBytesCounter = new PerformanceCounter(categoryName: "Process", counterName: "Private Bytes", instanceName: currentProcess);
                pageFileBytesCounter = new PerformanceCounter(categoryName: "Process", counterName: "Page File Bytes", instanceName: currentProcess);
                handleCountCounter = new PerformanceCounter(categoryName: "Process", counterName: "Handle Count", instanceName: currentProcess);
                processorTimeCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            }
            catch
            {
            }
        }

        public static bool Enable { get; set; }

        public static bool EnablePerfMonCpu { get; set; }
        public static bool EnablePerfMonMemory { get; set; }
        public static String PerfMonLogFileName { get; private set; }

        public static PerfMonData GetPerfMonData()
        {
            var perfMonData = new PerfMonData();

            if (EnablePerfMonMemory)
            {
                perfMonData.FreeMemoryMB = freeMemCounter.NextValue();
                perfMonData.CommittedMemoryMB = committedMemCounter.NextValue() / MB;
                perfMonData.PrivateBytesMB = privateBytesCounter.NextValue() / MB;
                perfMonData.PageFileBytesMB = pageFileBytesCounter.NextValue() / MB;
                perfMonData.HandleCount = handleCountCounter.NextValue();
            }

            if (EnablePerfMonCpu)
            {
                float sum = 0;
                for (int ii = 0; ii < cpuCounterIterations; ii++)
                {
                    sum += processorTimeCounter.NextValue();
                }

                perfMonData.ProcessorUtilizationPercent = sum / cpuCounterIterations;
            }

            return perfMonData;
        }

        public static bool LogCounters(PerfMonData data = null)
        {
            PerfMonData perfMonData;

            if (data == null)
            {
                perfMonData = GetPerfMonData();
            }
            else
            {
                perfMonData = data;
            }

            StreamWriter file = null;

            try
            {
                bool writeHeader = !File.Exists(PerfMonLogFileName);

                file = new StreamWriter(PerfMonLogFileName, true);
                if (writeHeader)
                {
                    file.WriteLine("Timestamp, TimeSinceStart (msecs), Free Memory (MB), Committed Memory (MB), Private Bytes (MB), Page File Bytes (MB), Handle Count, CPU Utilization");
                }

                var sb = new StringBuilder();
                DateTime now = DateTime.UtcNow;

                var elapsed = now - Process.GetCurrentProcess().StartTime.ToUniversalTime();

                sb.Append(now.ToLocalTime() + ",");

                String elapsedTime = ((int)(elapsed.TotalMilliseconds / 1000)).ToString() + "." + (int)(elapsed.TotalMilliseconds % 1000);

                sb.Append(elapsedTime + ",");
                sb.Append(perfMonData.ToString());

                file.WriteLine(sb.ToString());
            }
            catch
            {
                return false;
            }
            finally
            {
                file?.Flush();
                file?.Close();
                file?.Dispose();
            }


            return true;
        }

        public static void PauseLogging()
        {
            if (timerStarted)
            {
                timerPaused = true;
            }
        }

        public static void ResumeLogging()
        {
            if (timerStarted && timerPaused)
            {
                timerPaused = false;
            }
        }

        public static bool StartLogging(int timerInterval)
        {
            if (timerInterval <= 0)
            {
                return false;
            }

            if (timerStarted)
            {
                return true;
            }

            timerPaused = false;

            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = timerInterval;
            timerStarted = true;
            timer.Start();

            return true;
        }

        public static void StopLogging()
        {
            if (timerStarted)
            {
                timer.Elapsed -= Timer_Elapsed;
                timer.Stop();
                timer.Dispose();
            }

            timerStarted = false;
            timerPaused = false;
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (inTimer || timerPaused)
            {
                return;
            }

            inTimer = true;

            LogCounters();

            inTimer = false;
        }
    }
}