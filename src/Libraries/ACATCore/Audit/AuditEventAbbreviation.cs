////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Represents audit log entry of an abbreviation expansion
    /// </summary>
    public class AuditEventAbbreviation : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventAbbreviation()
            : base("Abbreviation")
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="expansionMode">abbr expansion mode</param>
        public AuditEventAbbreviation(String expansionMode)
            : base("Abbreviation")
        {
            ExpansionMode = expansionMode;
        }

        /// <summary>
        /// Returns the expansion mode
        /// </summary>
        public String ExpansionMode { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(ExpansionMode);
        }
    }
}