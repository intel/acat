////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////


#define DbgView

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Handles logging application messages of a variety of criticalities (Debug - Fatal)
    /// Debug log messages are sent to the debug console (can be viewed using the DebugView
    /// utility from SysInternals) and they are also logged to a file.
    /// Note:  Debug logging should be turned on only for troubleshooting.  It can
    /// slow down the app. Also, the debug file sizes can get pretty big and ACAT
    /// doesn't automatically cleanup. The user should delete these files manually.
    /// </summary>
    public class Log
    {
        private static DateTime? prevMessageTimeStamp = null;

        /// <summary>
        // set  this to true if you don't want to trigger the assertions in this file
        /// </summary>
        private const bool AssertionMode = false;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private const string MutexName = @"Global\ACAT_Logging";

        /// <summary>
        /// Used for synchronization
        /// </summary>
        //private static Mutex mutex;

        /// <summary>
        /// Where log files are stored
        /// </summary>
        private static readonly string logFileFolder = FileUtils.GetLogsDir();

        /// <summary>
        /// Name of the log file
        /// </summary>
        private static string LogFileName = "ACATLog.txt";

        /// <summary>
        /// Full path to the log file in which the debug messages are stored
        /// </summary>
        private static readonly String logFileFullPath;

        /// <summary>
        ///  IF log file exceeds this limit, it is renamed and
        /// a new log file is created
        /// </summary>
        private const int FileLenghThreshold = 2 * 1024 * 1024;

        /// <summary>
        /// Initialzies an instance of the class
        /// If the log file exceeds the threshold, renames it
        /// </summary>
        static Log()
        {
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

            if (!String.IsNullOrEmpty(GlobalPreferences.LogFileName))
            {
                LogFileName = GlobalPreferences.LogFileName;
            }
            else if (!String.IsNullOrEmpty(CoreGlobals.AppId))
            {
                LogFileName = CoreGlobals.AppId + "_Log.txt";
            }
            else
            {
                LogFileName = "ACATLog.txt";
            }


            logFileFullPath = Path.Combine(logFileFolder, LogFileName);

            logFileFullPath = Path.ChangeExtension(logFileFullPath, null) + CoreGlobals.LogFileSuffix + ".txt";

        } // end method

        /// <summary>
        /// Adds listeners to the logging function
        /// </summary>
        public static void SetupListeners()
        {
            if (CoreGlobals.AppPreferences.EnableLogs)
            {
                CoreGlobals.AppPreferences.DebugLogMessagesToFile = true;
                CoreGlobals.AppPreferences.DebugMessagesEnable = true;
                CoreGlobals.AppPreferences.AuditLogEnable = true;
            }
            else
            {
                CoreGlobals.AppPreferences.DebugLogMessagesToFile = false;
                CoreGlobals.AppPreferences.DebugMessagesEnable = false;
                CoreGlobals.AppPreferences.AuditLogEnable = false;

                //TODO:
                // cleanup logs folder if the flag is turned off
            }

#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugLogMessagesToFile)
                {
                    var listener = new TextWriterTraceListener(logFileFullPath, "ACATDebugListener");
                    Trace.Listeners.Add(listener);
                }
            }
#else
            var listener = new TextWriterTraceListener(logFileFullPath, "ACATDebugListener");
            Trace.Listeners.Add(listener);
#endif
        }

        /// <summary>
        /// Flushes the trace log
        /// </summary>
        public static void Close()
        {
            Trace.Flush();
        }

        /// <summary>
        /// Logs the name of the function that called this function.
        /// </summary>
        public static void Debug()
        {

#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = formatClassNameAndMethod("DEBUG", new StackTrace().GetFrame(1));
                Trace.WriteLine(output);
            }
#else
        string output = formatClassNameAndMethod("DEBUG", new StackTrace().GetFrame(1));
        Trace.WriteLine(output);
#endif
        }

        /// <summary>
        /// Logs a debug message
        /// <param name="message">Message to log.</param>
        [DebuggerStepThrough]
        public static void Debug(string message)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = formatClassNameAndMethod("DEBUG", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
            }
#else
        string output = formatClassNameAndMethod("DEBUG", new StackTrace().GetFrame(1)) + message;
        Trace.WriteLine(output);
#endif
        }

        public static void Exception(Exception exc)
        {
            string output = "*** EXCEPTION: ****" + exc.ToString() + ". StackTrace:" + exc.StackTrace;
            System.Diagnostics.Trace.WriteLine(output);
        }

        /// <summary>
        /// Logs a debug message
        /// <param name="message">Message to log.</param>
        /// <param name="exc">Exception to log information of.</param>
        public static void Debug(string message, Exception exc)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
            string output = "DEBUG: " + message + " StackTrace:" + exc.StackTrace;
            Trace.WriteLine(output);
            }
#else
            string output = "DEBUG: " + message + " StackTrace:" + exc.StackTrace;
            Trace.WriteLine(output);
#endif
        }

        /// <summary>
        /// Logs a Info message
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Info(string message)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = formatClassNameAndMethod("INFO", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
            }
#else
            string output = formatClassNameAndMethod("INFO", new StackTrace().GetFrame(1)) + message;
            Trace.WriteLine(output);
#endif
        }

        /// <summary>
        /// Logs a Info message
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exc">Exception to log information of.</param>
        public static void Info(string message, Exception exc)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = "INFO: " + message + " StackTrace:" + exc.StackTrace;
                Trace.WriteLine(output);
            }
#else
            string output = "INFO: " + message + " StackTrace:" + exc.StackTrace;
            Trace.WriteLine(output);
#endif
        }

        /// <summary>
        /// Logs a Warning message
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Warn(string message)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = formatClassNameAndMethod("WARN", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
            }
#else
            string output = formatClassNameAndMethod("WARN", new StackTrace().GetFrame(1)) + message;
            Trace.WriteLine(output);
#endif
        }

        /// <summary>
        /// Logs a Warning message
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exc">Exception to log information of.</param>
        public static void Warn(string message, Exception exc)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
#endif
            string output = "WARN: " + message + " StackTrace:" + exc.StackTrace;
            System.Diagnostics.Trace.WriteLine(output);
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Error(string message)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = formatClassNameAndMethod("ERROR", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
            }
#else
            string output = formatClassNameAndMethod("ERROR", new StackTrace().GetFrame(1)) + message;
            Trace.WriteLine(output);
            Trace.Assert(AssertionMode);
#endif
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exc">Exception to log information of.</param>
        public static void Error(string message, Exception exc)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = "ERROR: " + message + " StackTrace:" + exc.StackTrace;
                Trace.WriteLine(output);
            }
#else
            string output = "ERROR: " + message + " StackTrace:" + exc.StackTrace;
            Trace.WriteLine(output);
            Trace.Assert(AssertionMode);
#endif
        }

        /// <summary>
        /// Logs a Fatal error message
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Fatal(string message)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = formatClassNameAndMethod("FATAL", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
            }
#else
            string output = formatClassNameAndMethod("FATAL", new StackTrace().GetFrame(1)) + message;
            Trace.WriteLine(output);
            Trace.Assert(AssertionMode);
#endif
        }

        /// <summary>
        /// Logs a Fatal error message
        /// </summary>
        /// <param name="message">Message to log.</param>
        /// <param name="exc">Exception to log information of.</param>
        public static void Fatal(string message, Exception exc)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                string output = "FATAL: " + message + " StackTrace:" + exc.StackTrace;
                Trace.WriteLine(output);
            }
#else
            string output = "FATAL: " + message + " StackTrace:" + exc.StackTrace;
            Trace.WriteLine(output);
            Trace.Assert(AssertionMode);
#endif
        }

        public static void IsNull(String message, object obj)
        {
            Debug(message + ". " + ((obj != null) ? " is not null " : " is null "));
        }

        /// <summary>
        /// Format the prefix to a message using the class name and the method
        /// name that triggered the message
        /// </summary>
        /// <param name="prefix">DEBUG, INFO, etc</param>
        /// <param name="stackFrame">Stackframe of the calling method</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        private static String formatClassNameAndMethod(String prefix, StackFrame stackFrame)
        {
            DateTime nowUtc = DateTime.UtcNow;
            DateTime now = DateTime.Now;

            string strNow = now.ToString("h:mm:ss tt");

            if (prevMessageTimeStamp == null)
            {
                prevMessageTimeStamp = nowUtc;
            }

            var elapsed = nowUtc - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            
            MethodBase methodBase = stackFrame.GetMethod();

            var elapsedSincePrev = nowUtc - prevMessageTimeStamp;


            var prefix2 = "[" + strNow + ", " + ((int) (elapsed.TotalMilliseconds / 1000)) + "." + (int)(elapsed.TotalMilliseconds % 1000) + ", " + elapsedSincePrev?.Seconds + "." + elapsedSincePrev?.Milliseconds + " " + prefix + "] ";

            prevMessageTimeStamp = nowUtc;

            return prefix2 + ":[" + stackFrame.GetMethod().DeclaringType.Namespace + "][" +
                        stackFrame.GetMethod().DeclaringType.Name + "." + methodBase.Name + "] ";
        }
    }
}