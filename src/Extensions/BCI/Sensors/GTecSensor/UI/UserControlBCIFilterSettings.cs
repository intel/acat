////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCIFilterSettings.cs
//
// User control which prompts the user for input to select the best filter setting
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Sensors.GTecSensor
{
    /// <summary>
    /// User control which prompts the user for input to select the best filter setting
    /// </summary>
    public partial class UserControlBCIFilterSettings : UserControlBCIBase
    {
        /// <summary>
        /// Unique ID for this step
        /// </summary>
        private readonly String _stepId;

        /// <summary>
        /// User control allowing selection of filter settings
        /// </summary>
        // public UserControlBCIFilterSettings(String stepId)
        public UserControlBCIFilterSettings()
        {
            InitializeComponent();

            //_stepId = stepId;

            int DAQ_NotchFilterIdx = _gTecBCI.BCIGtecActuatorSettings.DAQ_NotchFilterIdx;
            if (DAQ_NotchFilterIdx == 2)
            {
                // DAQ_NotchFilterIdx = 1; //50Hz
                // DAQ_NotchFilterIdx = 2; //60Hz

                checkBoxConfirm60HzCountry.Checked = true;
            }
        }
    }
}