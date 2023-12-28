////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Base class for all audit logs events
    /// </summary>
    public class AuditEventBase
    {
        /// <summary>
        /// this is used if the value is not known
        /// </summary>
        public const String UnknownValue = "unknown";

        /// <summary>
        /// this is used if the value is null
        /// </summary>
        private const String NullValue = "<null>";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="eventType">type of the audig log event</param>
        public AuditEventBase(String eventType)
        {
            EventType = eventType;
            TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Gets the type of the audit event
        /// </summary>
        public String EventType { get; internal set; }

        /// <summary>
        /// Gets the timestamp of the audit event
        /// </summary>
        public DateTime TimeStamp { get; internal set; }

        /// <summary>
        /// Converts log entry into a string that can be
        /// written to the audit log file
        /// </summary>
        /// <returns>String representation</returns>
        public override String ToString()
        {
            var str = toString();

            DateTime now = DateTime.UtcNow;

            var elapsed = now - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();

            String elapsedTime = ((int)(elapsed.TotalMilliseconds / 1000)).ToString() + "." + (int)(elapsed.TotalMilliseconds % 1000);

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpochUtc = (int)t.TotalSeconds;
            double milliSecondsSinceEpochUtc = t.TotalMilliseconds;

            t = DateTime.Now - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            double milliSecondsSinceEpoch = t.TotalMilliseconds;

            //return now.ToLocalTime() + "," + elapsedTime + "," + EventType + (!String.IsNullOrEmpty(str) ? ("," + str) : String.Empty);
            return now.ToLocalTime() + "," + elapsedTime + "," + secondsSinceEpochUtc + ", " + secondsSinceEpoch + ", "
                        + milliSecondsSinceEpochUtc + ", " + milliSecondsSinceEpoch + ", "
                        + EventType + (!String.IsNullOrEmpty(str) ? ("," + str) : String.Empty);
        }

        /// <summary>
        /// Formats the audit log entry string
        /// </summary>
        /// <param name="args">log entry arguments</param>
        /// <returns>string representation</returns>
        protected virtual String formatEventString(params object[] args)
        {
            var sb = new StringBuilder();
            for (int ii = 0; ii < args.Length - 1; ii++)
            {
                sb.Append(formatArg(args[ii]) + "|");
            }

            sb.Append(formatArg(args[args.Length - 1]));
            return sb.ToString();
        }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected virtual String toString()
        {
            return string.Empty;
        }

        /// <summary>
        /// Formats the argument as a string, checks for null
        /// value
        /// </summary>
        /// <param name="arg">the argument</param>
        /// <returns>string representation</returns>
        private String formatArg(object arg)
        {
            String retVal = NullValue;
            if (arg != null)
            {
                retVal = arg.ToString();
                if (String.IsNullOrEmpty(retVal))
                {
                    retVal = NullValue;
                }
            }

            return retVal;
        }
    }
}