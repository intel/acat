////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Plots.cs
//
// Handles plotting of data in graphs
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using Accord.Math;
using Accord.Statistics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGProcessing
{
    internal static class Plots
    {
        /// <summary>
        /// List of colors to be used when plotting
        /// </summary>
        private static readonly string[] ColorValues = new string[] { "#000000",  "#00FFFF", "#808080",
             "#FF00FF", "#800000", "#FF0000", "#808000", "#808080", "#FFFF00", "#008000",
             "#008080", "#000080", "#0000FF"," #800080", "#00FF00",  "#606000", "#000000"};

    }
}