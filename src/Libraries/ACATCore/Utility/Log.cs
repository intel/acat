////////////////////////////////////////////////////////////////////////////
// <copyright file="Log.cs" company="Intel Corporation">
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

#define DbgView

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Handles logging application messages of a variety of criticalities (Debug - Fatal)
    /// </summary>
    public class Log
    {
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
        private static Mutex mutex;

        /// <summary>
        /// Where log files are stored
        /// </summary>
        private static readonly string logFileFolder = FileUtils.GetLogsDir();

        /// <summary>
        /// Name of the log file
        /// </summary>
        private const string LogFileName = "ACATLog.txt";

        /// <summary>
        /// Full path to the log file
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

            logFileFullPath = Path.Combine(logFileFolder, LogFileName);

            mutex = new Mutex(false, MutexName);
            try
            {
                if (mutex.WaitOne())
                {
                    var fi = new FileInfo(logFileFullPath);
                    if (fi.Length > FileLenghThreshold)
                    {
                        string p = Path.ChangeExtension(logFileFullPath, null) + "_" + DateTime.Now.ToString("s").Replace(':', '_') + ".txt";
                        File.Move(logFileFullPath, p);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        } // end method

        /// <summary>
        /// Adds listeners to the logging function
        /// </summary>
        public static void SetupListeners()
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
#endif
                if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugLogMessagesToFile)
                {
                    var listener = new TextWriterTraceListener(logFileFullPath, "ACATDebugListener");
                    Trace.Listeners.Add(listener);
                }
#if !DEBUG
            }
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
#endif
                string output = formatClassNameAndMethod("DEBUG", new StackTrace().GetFrame(1));
                Trace.WriteLine(output);
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// Logs a debug message
        /// <param name="message">Message to log.</param>
        public static void Debug(string message)
        {
#if !DEBUG
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.DebugMessagesEnable)
            {
#endif
                string output = formatClassNameAndMethod("DEBUG", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
#if !DEBUG
            }
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
#endif
                string output = "DEBUG: " + message + " StackTrace:" + exc.StackTrace;
                Trace.WriteLine(output);
#if !DEBUG
            }
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
#endif
                string output = formatClassNameAndMethod("INFO", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
#if !DEBUG
            }
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
#endif
                string output = "INFO: " + message + " StackTrace:" + exc.StackTrace;
                Trace.WriteLine(output);
#if !DEBUG
            }
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
#endif
                string output = formatClassNameAndMethod("WARN", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
#if !DEBUG
            }
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
#endif
                string output = formatClassNameAndMethod("ERROR", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
                if (CoreGlobals.AppPreferences.DebugAssertOnError)
                {
                    Trace.Assert(AssertionMode);
                }
#if !DEBUG
            }
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
#endif
                string output = "ERROR: " + message + " StackTrace:" + exc.StackTrace;
                Trace.WriteLine(output);
                if (CoreGlobals.AppPreferences.DebugAssertOnError)
                {
                    Trace.Assert(AssertionMode);
                }
#if !DEBUG
            }
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
#endif
                string output = formatClassNameAndMethod("FATAL", new StackTrace().GetFrame(1)) + message;
                Trace.WriteLine(output);
                if (CoreGlobals.AppPreferences.DebugAssertOnError)
                {
                    Trace.Assert(AssertionMode);
                }
#if !DEBUG
            }
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
#endif
                string output = "FATAL: " + message + " StackTrace:" + exc.StackTrace;
                Trace.WriteLine(output);
                if (CoreGlobals.AppPreferences.DebugAssertOnError)
                {
                    Trace.Assert(AssertionMode);
                }
#if !DEBUG
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
        private static String formatClassNameAndMethod(String prefix, StackFrame stackFrame)
        {
            MethodBase methodBase = stackFrame.GetMethod();
            return prefix + ":[" + stackFrame.GetMethod().DeclaringType.Namespace + "][" +
                        stackFrame.GetMethod().DeclaringType.Name + "." + methodBase.Name + "] ";
        }
    }
}