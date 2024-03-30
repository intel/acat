////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlTestBCIConnections.cs
//
// Displays "BCI Connecting..." gif while user waits for results from BCI device tests
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// Displays "BCI Connecting..." gif while user waits for results from BCI device tests
    /// </summary>
    public partial class UserControlTestBCIConnections : UserControl
    {
        /// <summary>
        /// Unique ID for this step
        /// </summary>
        private String _stepId;

        public UserControlTestBCIConnections(String stepId)
        {
            InitializeComponent();

            _stepId = stepId;
        }
    }
}