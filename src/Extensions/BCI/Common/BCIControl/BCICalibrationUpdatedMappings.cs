////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCiCalibrationUpdatedMappings.xml
//
// Send updated typing section - calibration classifier mappings
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCICalibrationUpdatedMappings
    {
        /// <summary>
        /// Error
        /// </summary>
        public BCIError Error { get; set; }

        /// <summary>
        /// Dictionary of mappings typing section - classifier calibrated
        /// </summary>
        public Dictionary<BCIScanSections, BCIScanSections> DictUpdatedMappings { get; set; }

        public BCICalibrationUpdatedMappings()
        {
            Error = new BCIError(BCIErrorCodes.Status_Ok, BCIMessages.Status_Ok);
            DictUpdatedMappings = new Dictionary<BCIScanSections, BCIScanSections>();
        }

        public BCICalibrationUpdatedMappings(Dictionary<BCIScanSections, BCIScanSections> dictUpdatedMappings, BCIError error)
        {
            dictUpdatedMappings = DictUpdatedMappings;
            Error = error;
        }
    }
}