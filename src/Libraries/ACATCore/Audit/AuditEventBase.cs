////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventBase.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Text;

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
            return TimeStamp.Ticks / TimeSpan.TicksPerMillisecond + "," +
                    TimeStamp.ToString("MM/dd/yy") + "," +
                    TimeStamp.ToString("HH:mm:ss.FFF") + "," +
                    EventType + (!String.IsNullOrEmpty(str) ? ("," + str) : String.Empty);
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