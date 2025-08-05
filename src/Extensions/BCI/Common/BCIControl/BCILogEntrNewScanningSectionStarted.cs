////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCILogEntrNewScanningSectionStarted.cs
//
// Auditlog entry when a new scanning section
// ("Boxes", "Words" "Sentences" "keyboard" "Menus) started
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCILogEntrNewScanningSectionStarted
    {
        public BCILogEntrNewScanningSectionStarted()
        {
        }

        /// <summary>
        /// Corresponding scanning section
        /// Options: "Boxes" "Words" "Sentences" "Keyboard" "Menus"
        /// </summary>
        public String ScanningSection { get; set; } = "";

        /// <summary>
        /// Boolean, true if LM probabilities are used for this scanning section
        /// </summary>
        public bool UseLanguageModelProbabilities { get; set; } = false;

        /// <summary>
        /// Lookup table with ID and value.
        /// Eg: [1, 'Box1'] [2, "Box 2'] ...
        /// Eg: [1, 'a'] [2, 'b'], [3, 'c']...
        /// </summary>
        public Dictionary<int, string> ButtonIDValuesLookupTable { get; set; } = new Dictionary<int, string>();
    }
}