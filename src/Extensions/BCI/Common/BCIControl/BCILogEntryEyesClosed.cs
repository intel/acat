////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// BCILogEntryEyesClosed.cs
//
// Auditlog entry for eyes open/closed detection
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Extensions.BCI.Common.BCIControl
{
    [Serializable]
    public class BCILogEntryEyesClosed
    {
        public BCILogEntryEyesClosed()
        {
            EyesClosedDetected = false;
            EyesClosedEnabled = false;
            AlphaValues = null;
            BetaValues = null;
            AvgAlphaValue = 0;
            AvgBetaValue = 0;
            GroundTruth = "";
        }

        public BCILogEntryEyesClosed(bool eyesClosedEnable, double eyesClosedThreshold, bool eyesClosedDetected, double[] alphaValues, double[] betaValues, double avgAlpha, double avgBeta, string groundTruth = "")
        {
            GroundTruth = groundTruth;
            EyesClosedEnabled = eyesClosedEnable;
            EyesClosedDetected = eyesClosedDetected;
            EyesClosedThreshold = eyesClosedThreshold;
            AlphaValues = alphaValues;
            BetaValues = betaValues;
            AvgAlphaValue = avgAlpha;
            AvgBetaValue = avgBeta;
        }

        // In calibration, ground truth if eyes open/closed
        public String GroundTruth { get; set; }

        /// <summary>
        /// Boolean, true if eyes closed detection is enabled (controlled from settings)
        /// </summary>
        public bool EyesClosedEnabled { get; set; }

        /// <summary>
        /// Threshold (on avg of alpha value) for eyes closing detection
        /// </summary>
        public double EyesClosedThreshold { get; set; }

        /// <summary>
        /// Boolean, true if eyes closed have been detected
        /// </summary>
        public bool EyesClosedDetected { get; set; }

        /// <summary>
        /// Avereged alpha values from all channels
        /// </summary>
        public double AvgAlphaValue { get; set; }

        /// <summary>
        /// Array containing the alpha values for each channel
        /// </summary>
        public double[] AlphaValues { get; set; }

        /// <summary>
        /// Averaged beta values from all channels
        /// </summary>
        public double AvgBetaValue { get; set; }

        /// <summary>
        /// Array containing the beta values for each channel
        /// </summary>
        public double[] BetaValues { get; set; }
    }
}