////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCILogEntryClassifierLoaded.cs
//
// Auditlog entry when a classifier is loaded
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Audit;
using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCILogEntryClassifierLoaded : AuditEventBase
    {
        public BCILogEntryClassifierLoaded() : base("BCIClassifiersLoaded")
        {
            TypingSection = BCIScanSections.None.ToString();
            Classifier = BCIScanSections.None.ToString();
            ClassifierID = "";
            ClassifierAUC = 0;
            ScanTime = 0;
            NumberOfTargets = 0;
            IterationsPerTarget = 0;
            MinimumScoreRequired = 0;
            IsClassifierLoaded = false;
        }

        public BCILogEntryClassifierLoaded(String typingSection, String classifier, String classifierID, float classifierAUC, int scanTime, int numberOfTargets, int iterationsPerTarget, int minimumScoreRerquired, bool isClassifierLoaded)
        : base("BCIClassifiersLoaded")
        {
            TypingSection = typingSection;
            Classifier = classifier;
            IsClassifierLoaded = isClassifierLoaded;
            ClassifierID = classifierID;
            ClassifierAUC = classifierAUC;
            ScanTime = scanTime;
            NumberOfTargets = numberOfTargets;
            IterationsPerTarget = iterationsPerTarget;
            MinimumScoreRequired = minimumScoreRerquired;
        }

        /// <summary>
        /// Calibration Mode
        /// Options: "Box" "Words" "Sentences" "KeyboardR" "KeyboardL"
        /// </summary>
        public String TypingSection { get; set; }

        /// <summary>
        /// Calibration Mode
        /// Options: "Box" "Words" "Sentences" "KeyboardR" "KeyboardL"
        /// </summary>
        public String Classifier { get; set; }

        /// <summary>
        /// True if classifier load correctly
        /// </summary>
        public bool IsClassifierLoaded { get; set; }

        /// <summary>
        /// Classifier ID
        /// </summary>
        public String ClassifierID { get; set; }

        /// <summary>
        /// AUC of the classifier
        /// </summary>
        public float ClassifierAUC { get; set; }

        /// <summary>
        /// Scan time (in ms)
        /// </summary>
        public int ScanTime { get; set; }

        /// <summary>
        /// Number of targets
        /// </summary>
        public int NumberOfTargets { get; set; }

        /// <summary>
        /// Number of iterations per target
        /// </summary>
        public int IterationsPerTarget { get; set; }

        /// <summary>
        /// Minimum score required
        /// </summary>
        public int MinimumScoreRequired { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(TypingSection, Classifier, IsClassifierLoaded, ClassifierID, ClassifierAUC, ScanTime, NumberOfTargets, IterationsPerTarget, MinimumScoreRequired);
        }
    }
}