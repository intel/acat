////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCICalibrationStatus.cs
//
// Status of the calibration
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    public enum BCIClassifierStatus
    {
        Ok,
        Expired,
        NotFound,
    }

    [Serializable]
    public class BCIClassifierInfo
    {
        public BCIScanSections ClassifierUsed { get; set; } = BCIScanSections.None;

        /// <summary>
        /// Status of the classifier (Ok, Expired, NotFound)
        /// </summary>
        public BCIClassifierStatus ClassifierStatus { get; set; } = BCIClassifierStatus.Ok;

        /// <summary>
        /// AUC for the classifier
        /// </summary>
        public float Auc { get; set; } = 0.0f;

        /// <summary>
        /// Boolean, true if the classifier is required (in the mappings file)
        /// </summary>
        public bool IsRequired { get; set; } = false;
    }

    [Serializable]
    public class BCICalibrationStatus
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
        /// Overall status for the classifiers (Ok, Expired, NotFound)
        /// </summary>
        public BCIClassifierStatus OverallStatus { get; set; } = BCIClassifierStatus.Ok;

        /// <summary>
        /// Status for each particular classifier: (Ok/Expired/NotFound, Auc...)
        /// </summary>
        public Dictionary<BCIScanSections, BCIClassifierInfo> DictClassifierInfo { get; set; } = new Dictionary<BCIScanSections, BCIClassifierInfo>();

        /// <summary>
        /// Boolean, true if ACAT should only display the default classifiers (Box, Sentences & KeyboardL)
        /// </summary>
        public bool ShowOnlyDefaults { get; set; } = false;

        /// <summary>
        /// Boolean, true if there are more classifiers than required in the mapping file
        /// </summary>
        public bool AreMoreClassifiersThanMapping { get; set; } = false;

        /// <summary>
        /// Boolean, true if all classifiers are loaded and ACAT can show "Start typing" message
        /// </summary>
        public bool OkToGoToTyping { get; set; } = false;

    }
}