////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FileNames.cs
//
// Names for BCI files containing data and markers
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGDataAcquisition
{
    /// <summary>
    /// Names used to store BCI data
    /// </summary>
    internal class FileNames
    {
        public static readonly string suffix_eegRaw = "_eegDataRaw.txt";
        public static readonly string suffix_eegFiltered = "_eegDataFiltered.txt";
        public static readonly string suffix_markerValues = "_markerValues.txt";
    }
}