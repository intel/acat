////////////////////////////////////////////////////////////////////////////
// <copyright file="PCode.cs" company="Intel Corporation">
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
using System.Collections.Generic;

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
        /// Returns true if there is an associated script
        /// </summary>
        /// <returns>true if so, false otherwise</returns>
        public bool HasCode()
        {
            return ActionVerbList.Count > 0;
        }
    }
}