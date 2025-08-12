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
using System.Runtime.CompilerServices;
//using System.Windows.Shapes;

namespace ACAT.Core.Utility
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
        /// Valid Trace Levels:
        ///    Off = 0,
        ///    Error = 1,
        ///    Warning = 2,
        ///    Info = 3,
        ///    Verbose = 4
        public static TraceSwitch TraceLevelSwitch = new("AppTraceSwitch", "Controls tracing level", "Warning");

        private static DateTime? prevMessageTimeStamp = null;

        /// <summary>
        // set  this to true if you don't want to trigger the assertions in this file
        /// </summary>
#if DEBUG
        private const bool AssertionMode = true;
        private const bool simpleLog = true;
#else
        private const bool AssertionMode = false;
        private const bool simpleLog = false;
#endif
        private const bool IncludeStackTrace = false;

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
        private static readonly string LogFileName = "ACATLog.txt";

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
        public static void Verbose(string msg = "",
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            WriteTrace($"{formatClassNameAndMethod("VERBOSE")} {msg}", file, line, TraceLevel.Verbose);
        }

        /// <summary>
        /// Logs a debug message
        /// <param name="message">Message to log.</param>
        [DebuggerStepThrough]
        public static void Debug(string message,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            WriteTrace($"{formatClassNameAndMethod("DEBUG")} {message}", file, line, TraceLevel.Info);
        }

        public static void Exception(String msg,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            WriteTrace($"{formatClassNameAndMethod("EXCEPTION")} {msg}",file, line, TraceLevel.Error, true);
        }
        public static void Exception(Exception exc,
            [CallerFilePath] string file = "",
            [CallerLineNumber] int line = 0)
        {
            var stackTrace = IncludeStackTrace ? $"\n\tStackTrace: {exc.StackTrace}" : string.Empty;
            WriteTrace($"{formatClassNameAndMethod("EXCEPTION")} {exc.Message}{stackTrace}", file, line, TraceLevel.Error, true);
        }

        /// <summary>
        /// Logs a Info message
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteTrace($"{formatClassNameAndMethod("INFO")} {message}", file, line, TraceLevel.Info);
        }

        /// <summary>
        /// Logs a Warning message
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Warn(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteTrace($"{formatClassNameAndMethod("WARN")} {message}", file, line, TraceLevel.Warning);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            WriteTrace($"{formatClassNameAndMethod("ERROR")} {message}", file, line, TraceLevel.Error, true);
        }

        /// <summary>
        /// Writes to Trace if Logging is enabled.
        /// </summary>
        /// <param name="message">Message to log.</param>
        public static void WriteTrace(string message, string filename, int linenumber, TraceLevel traceLevel, bool assert = false)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && !CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
                return;
            }
#else
            if (TraceLevelSwitch.Level >= traceLevel )
            {
                Trace.WriteLine($@"{filename}({linenumber},1): {message}");
            }
            if (assert)
            {
                Trace.Assert(AssertionMode);
            }
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
        private static String formatClassNameAndMethod(String prefix)
        {
            if (simpleLog)
            {
                return $"[{DateTime.Now:HH:mm:ss}] [{prefix}] - ";
            }
            else
            {
                // Get the stack frame of the caller of this method

#pragma warning disable CS0162 // Unreachable code detected
                StackFrame stackFrame = new StackTrace().GetFrame(2);  // Get the caller of the caller
#pragma warning restore CS0162 // Unreachable code detected
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

                var prefix2 = "[" + strNow + ", " + ((int)(elapsed.TotalMilliseconds / 1000)) + "." + (int)(elapsed.TotalMilliseconds % 1000) + ", " + elapsedSincePrev?.Seconds + "." + elapsedSincePrev?.Milliseconds + " " + prefix + "] ";

                prevMessageTimeStamp = nowUtc;

                return prefix2 + ":[" + stackFrame.GetMethod().DeclaringType.Namespace + "][" +
                            stackFrame.GetMethod().DeclaringType.Name + "." + methodBase.Name + "] ";
            }

        }
    }
}