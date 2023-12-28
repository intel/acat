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
    /// An action verb consists of a function name and arguments.
    /// This class encapsulates the attributes of an action verb.
    /// E.g. if the action verb is "transition(TopLevelRotation)",
    /// action name would be "transition" and the argument would be
    /// "TopLevelRotation"

    /// </summary>
    public class ActionVerb
    {
        /// <summary>
        /// Initiates the Action verb class
        /// </summary>
        public ActionVerb()
        {
            ArgList = new List<string>();
        }

        /// <summary>
        /// The name of the function
        ///  eg if the action verb is "transition(TopLevelRotation)",
        ///  action name would be "transition"
        /// </summary>
        public String Action { get; set; }

        /// <summary>
        /// List of arguments for the function
        /// eg if the action verb is "transition(TopLevelRotation)",
        /// arglist would contain the string "TopLevelRotation"
        /// </summary>
        public List<String> ArgList { get; set; }
    }
}