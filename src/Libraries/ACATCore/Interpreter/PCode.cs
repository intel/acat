////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
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