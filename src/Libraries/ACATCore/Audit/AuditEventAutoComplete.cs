////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventAutoComplete.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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