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
    /// Represents the log engry to audit events related
    /// to word auto-completion using word prediction.
    /// </summary>
    public class AuditEventAutoComplete : AuditEventBase
    {
        /// <summary>
        /// Name of the widget containing the selected
        /// word
        /// </summary>
        private readonly String _wordListItemName;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventAutoComplete()
            : base("AutoComplete")
        {
            _wordListItemName = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="wordListItemName">name of the widget</param>
        public AuditEventAutoComplete(String wordListItemName)
            : base("AutoComplete")
        {
            _wordListItemName = wordListItemName;
        }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(_wordListItemName);
        }
    }
}