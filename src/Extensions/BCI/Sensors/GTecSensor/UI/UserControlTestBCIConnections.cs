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

namespace ACAT.Extensions.BCI.Sensors.GTecSensor
{
    /// <summary>
    /// Displays "BCI Connecting..." gif while user waits for results from BCI device tests
    /// </summary>
    public partial class UserControlTestBCIConnections : UserControlBCIBase
    {

        public UserControlTestBCIConnections()
        {
            InitializeComponent();

        }
    }
}