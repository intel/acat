////////////////////////////////////////////////////////////////////////////
// <copyright file="AnimationWidget.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Handles audit logging application events.  Events are
    /// logged to an audit log file. Filters can be enabled for
    /// the various event types to log only certain types of
    /// events.
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Audit log file extension
        /// </summary>
        private const string FileExtension = ".csv";

        /// <summary>
        /// Create a new file if the log file length exceeds
        /// this threshold
        /// </summary>
        private const int FileLengthThreshold = 2 * 1024 * 1024;

        /// <summary>
        /// Name of the audit log file
        /// </summary>
        private const string LogFileName = "ACATAuditLog.csv";

        /// <summary>
        /// Name of the mutex used for synchronization
        /// </summary>
        private const string MutexName = @"Global\ACAT_AuditLogging";

        /// <summary>
        /// Where to put the log file
        /// </summary>
        private static readonly string LogFileFolder = SmartPath.ApplicationPath + "\\AuditLogs";

        /// <summary>
        /// Full path to the log file
        /// </summary>
        private static readonly String LogFileFullPath;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private static readonly Object ObjSync = new Object();

        /// <summary>
        /// List of audit log events to ignore
        /// </summary>
        private static List<String> _disableFilter;

        /// <summary>
        /// Should all audit log events be enabled?
        /// </summary>
        private static bool _enableAll = true;

        /// <summary>
        /// List of audit log events to log
        /// </summary>
        private static List<String> _enableFilter;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private static Mutex objMutex;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        static AuditLog()
        {
            Log.Debug("Entering...LogFileFolder=" + LogFileFolder);

            loadFilters();

            try
            {
                if (!Directory.Exists(LogFileFolder))
                {
                    Log.Debug("Creating Audit directory.");
                    Directory.CreateDirectory(LogFileFolder);
                }
            }
            catch
            {
                LogFileFolder = SmartPath.ApplicationPath;
            }

            LogFileFullPath = Path.Combine(LogFileFolder, LogFileName);
            Log.Debug("LogFileFullPath=" + LogFileFullPath);

            objMutex = new Mutex(false, MutexName);

            try
            {
                if (objMutex.WaitOne())
                {
                    Log.Debug("obtained objMutex");
                    var fileInfo = new FileInfo(LogFileFullPath);

                    if (fileInfo.Length > FileLengthThreshold)
                    {
                        string p = Path.ChangeExtension(LogFileFullPath, null) + "_" + DateTime.Now.ToString("s").Replace(':', '_') + FileExtension;
                        Log.Debug("Moving audit file to " + p);
                        File.Move(LogFileFullPath, p);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            finally
            {
                Log.Debug("Releasing mutex...");
                objMutex.ReleaseMutex();
            }
        } // end method

        /// <summary>
        /// Logs an audit log entry. Checks if should be logged
        /// by looking up the filters.
        /// </summary>
        /// <param name="auditEvent">Event to log</param>
        public static void Audit(AuditEventBase auditEvent)
        {
            if (CoreGlobals.AppPreferences != null && CoreGlobals.AppPreferences.AuditLogEnable)
            {
                bool doAudit = false;

                String eventName = auditEvent.EventType.ToLower().Trim();

                if (_enableAll)
                {
                    if (!_disableFilter.Contains(eventName))
                    {
                        doAudit = true;
                    }
                }
                else
                {
                    if (_enableFilter.Contains(eventName))
                    {
                        doAudit = true;
                    }
                }

                if (doAudit)
                {
                    lock (ObjSync)
                    {
                        var sw = new StreamWriter(LogFileFullPath, true);
                        sw.WriteLine(auditEvent.ToString());
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Close the audit log
        /// </summary>
        public static void Close()
        {
        }

        /// <summary>
        /// Read the filters from the settings, parse them and store
        /// them in the enabled and disabled filters members
        /// </summary>
        private static void loadFilters()
        {
            _enableFilter = new List<string>();
            _disableFilter = new List<string>();
            _enableAll = false;
            var filter = CoreGlobals.AppPreferences.AuditLogFilter.Trim().ToLower();

            var array = filter.Split(',');
            foreach (String str in array)
            {
                var s = str.Trim();
                if (String.IsNullOrEmpty(s))
                {
                    continue;
                }

                if (s[0] == '~')
                {
                    if (s.Length > 1)
                    {
                        _disableFilter.Add(s.Substring(1));
                    }
                }
                else
                {
                    if (s == "*" || s == "all")
                    {
                        _enableAll = true;
                    }

                    _enableFilter.Add(s);
                }
            }
        }
    }
}