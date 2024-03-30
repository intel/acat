////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCIMapOptions
//
// Map Options
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCIMapOptions
    {
        /// <summary>
        /// General error (STATUS_OK if no error)
        /// </summary>
        public BCIError Error;

        /// <summary>
        /// Boolean, true if advance mode. False if restricted mode
        /// </summary>
        public bool IsAdvanced;

        /// <summary>
        /// Dictionary containing the info for each available classifier
        /// </summary>
        public Dictionary<BCIScanSections, List<BCIClassifierInfo>> AllowedMappingsDict;

        /// <summary>
        /// Dictionary containing the current mappings
        /// </summary>
        public Dictionary<BCIScanSections, BCIScanSections> CurrentMappingsDict;

        /// <summary>
        /// Boolean, true if all classifiers are loaded and ACAT can show "Start typing" message
        /// </summary>
        public bool OkToGoToTyping;

        public BCIMapOptions(bool isAdvanced, Dictionary<BCIScanSections, List<BCIClassifierInfo>> allowedMappingsDict, Dictionary<BCIScanSections, BCIScanSections> currentMappingsDict, BCIError error)
        {
            Error = error;
            IsAdvanced = isAdvanced;
            AllowedMappingsDict = allowedMappingsDict;
            CurrentMappingsDict = currentMappingsDict;
        }
    }
}