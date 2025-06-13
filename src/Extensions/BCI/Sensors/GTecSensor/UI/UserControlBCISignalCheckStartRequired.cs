////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCISignalCheckStartRequired.cs
//
// User control which is displayed when the user is required to do a signal quality check
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Sensors.GTecSensor
{
    /// <summary>
    /// User control which is displayed when the user is required to do a signal quality check
    /// </summary>
    public partial class UserControlBCISignalCheckStartRequired : UserControlBCIBase
    {
        /// <summary>
        /// User control which is displayed when the user is required to do a signal quality check
        /// </summary>
        /// <param name="stepId"></param>
        //public UserControlBCISignalCheckStartRequired(String stepId)
        public UserControlBCISignalCheckStartRequired()
        {
            InitializeComponent();
        }
    }
}