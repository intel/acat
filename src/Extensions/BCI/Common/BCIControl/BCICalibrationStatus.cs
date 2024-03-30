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

    public class BCIClassifierInfo
    {
        public BCIScanSections ClassifierUsed;

        /// <summary>
        /// Status of the classifier (Ok, Expired, NotFound)
        /// </summary>
        public BCIClassifierStatus ClassifierStatus;

        /// <summary>
        /// AUC for the classifier
        /// </summary>
        public float Auc;

        /// <summary>
        /// Boolean, true if the classifier is required (in the mappings file)
        /// </summary>
        public bool IsRequired;

        public BCIClassifierInfo(bool isRequired, BCIScanSections classifierUsed, BCIClassifierStatus classifierStatus, float auc)
        {
            Auc = auc;
            ClassifierUsed = classifierUsed;
            ClassifierStatus = classifierStatus;
            IsRequired = isRequired;
        }
    }

    [Serializable]
    public class BCICalibrationStatus
    {
        /// <summary>
        /// General error (STATUS_OK if no error)
        /// </summary>
        public BCIError Error;

        /// <summary>
        /// Overall status for the classifiers (Ok, Expired, NotFound)
        /// </summary>
        public BCIClassifierStatus OverallStatus;

        /// <summary>
        /// Status for each particular classifier: (Ok/Expired/NotFound, Auc...)
        /// </summary>
        public Dictionary<BCIScanSections, BCIClassifierInfo> DictClassifierInfo;

        /// <summary>
        /// Boolean, true if ACAT should only display the default classifiers (Box, Sentences & KeyboardL)
        /// </summary>
        public bool ShowOnlyDefaults;

        /// <summary>
        /// Boolean, true if there are more classifiers than required in the mapping file
        /// </summary>
        public bool AreMoreClassifiersThanMapping;

        /// <summary>
        /// Boolean, true if all classifiers are loaded and ACAT can show "Start typing" message
        /// </summary>
        public bool OkToGoToTyping;

        public BCICalibrationStatus(bool showOnlyDefaults, bool areMoreClassifiersThanMapping, bool okToGoToTyping, BCIClassifierStatus overallStatus, Dictionary<BCIScanSections, BCIClassifierInfo> dictClassifierInfo, BCIError error)
        {
            Error = error;
            OverallStatus = overallStatus;
            DictClassifierInfo = dictClassifierInfo;
            AreMoreClassifiersThanMapping = areMoreClassifiersThanMapping;
            ShowOnlyDefaults = showOnlyDefaults;
            OkToGoToTyping = okToGoToTyping;
        }
    }
}