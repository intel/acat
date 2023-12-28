////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;

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
        /// Full path to the log file
        /// </summary>
        private static readonly String LogFileFullPath;

        /// <summary>
        /// Where to put the log file
        /// </summary>
        //private static readonly string LogFileFolder = SmartPath.ApplicationPath + "\\AuditLogs";
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
        /// Name of the audit log file
        /// </summary>
        private static readonly string LogFileName;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        static AuditLog()
        {
            string logFileFolder = FileUtils.GetLogsDir();

            loadFilters();

            try
            {
                if (!Directory.Exists(logFileFolder))
                {
                    Log.Debug("Creating log directory.");
                    Directory.CreateDirectory(logFileFolder);
                }
            }
            catch
            {
                logFileFolder = SmartPath.ApplicationPath;
            }

            if (!String.IsNullOrEmpty(CoreGlobals.AppId))
            {
                LogFileName = CoreGlobals.AppId + "_Audit" + ".csv";
            }
            else
            {
                LogFileName = "ACATAuditLog.csv";
            }

            LogFileFullPath = Path.Combine(logFileFolder, LogFileName);

            LogFileFullPath = Path.Combine(logFileFolder, Path.GetFileNameWithoutExtension(LogFileName) + CoreGlobals.LogFileSuffix + Path.GetExtension(LogFileName));

            Log.Debug("LogFileFullPath=" + LogFileFullPath);

            /*
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
            */
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
                    StreamWriter sw = null;

                    lock (ObjSync)
                    {
                        try
                        {
                            sw = new StreamWriter(LogFileFullPath, true);
                            sw.WriteLine(auditEvent.ToString());
                        }
                        catch
                        {

                        }
                        finally
                        {
                            sw?.Flush();
                            sw?.Close();
                            sw?.Dispose();
                        }
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