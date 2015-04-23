////////////////////////////////////////////////////////////////////////////
// <copyright file="AuditEventAnimationEnd.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Interpreter
{
    /// <summary>
    /// Represents an intermediate 'interpreted' form of
    /// a script.  Contains a list of ActionVerbs that is
    /// a result if interpreting the input script
    /// For eg, if the input script is:
    ///    "highlightSelected(@SelectedWidget, false); select(@SelectedBox); transition(RowRotation)"
    /// This will result in three ActionVerb elements in the ActionVerbList array
    ///     "highlightSelected" with arguments "@SelectedWidget" and "false"
    ///     "select" with argument "@SelectedBox"
    ///     "transition" with argument "RowRotation"
    /// </summary>
    public class PCode
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PCode()
        {
            ActionVerbList = new List<ActionVerb>();
            Script = String.Empty;
        }

        /// <summary>
        /// Gets or sets an array of actionVerbs that's a result
        /// of interpreting the script
        /// </summary>
        public List<ActionVerb> ActionVerbList { get; set; }

        /// <summary>
        /// Gets or sets the script to be parsed
        /// </summary>
        public String Script { get; set; }

        /// <summary>
        /// Clears the list
        /// </summary>
        public void Clear()
        {
            Script = String.Empty;
            if (ActionVerbList != null)
            {
                ActionVerbList.Clear();
            }
        }

        /// <summary>
        /// Is there an interpretation of the script?
        /// </summary>
        /// <returns>true if so, false otherwise</returns>
        public bool HasCode()
        {
            return ActionVerbList.Count > 0;
        }
    }
}