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
        public BCIError Error { get; set; } = new BCIError
        {
            ErrorCode = BCIErrorCodes.Status_Ok,
            ErrorMessage = BCIMessages.Status_Ok
        };

        /// <summary>
        /// Boolean, true if advance mode. False if restricted mode
        /// </summary>
        public bool IsAdvanced { get; set; } = false;

        /// <summary>
        /// Dictionary containing the info for each available classifier
        /// </summary>
        public Dictionary<BCIScanSections, List<BCIClassifierInfo>> AllowedMappingsDict { get; set; } = new Dictionary<BCIScanSections, List<BCIClassifierInfo>>();

        /// <summary>
        /// Dictionary containing the current mappings
        /// </summary>
        public Dictionary<BCIScanSections, BCIScanSections> CurrentMappingsDict { get; set; } = new Dictionary<BCIScanSections, BCIScanSections>();

        /// <summary>
        /// Boolean, true if all classifiers are loaded and ACAT can show "Start typing" message
        /// </summary>
        public bool OkToGoToTyping { get; set; } = false;

        public BCIMapOptions()
        {
        }
    }
}