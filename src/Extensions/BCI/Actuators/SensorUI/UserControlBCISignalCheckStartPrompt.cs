////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// UserControlBCISignalCheckStartPrompt.cs
//
// User control which prompts the user for input to determine whether signal
// quality check should be executed (first step in the signal check process)
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.SensorUI
{
    /// <summary>
    /// User control which prompts the user for input to determine whether signal quality check 
    /// should be executed (first step in the signal check process)
    /// </summary>
    public partial class UserControlBCISignalCheckStartPrompt : UserControl
    {
        private bool userRequestedSignalQualityRecheck = false;

        /// <summary>
        /// User control that determines whether or not user has to complete signal quality checking procedure
        /// </summary>
        /// <param name="stepId"></param>
        public UserControlBCISignalCheckStartPrompt(String stepId)
        {
            InitializeComponent();
            resetCheckbox();
        }

        /// <summary>
        /// Getter / setter for variable set to true when user requested signal quality recheck
        /// </summary>
        public bool UserRequestedRecheck
        {
            get
            {
                return userRequestedSignalQualityRecheck;
            }
            set
            {
                userRequestedSignalQualityRecheck = value;
            }
        }

        /// <summary>
        /// Set the checkbox and corresponding variable to unchecked 
        /// </summary>
        public void resetCheckbox()
        {
            UserRequestedRecheck = false;
            btnUserRequestSignalQualityRecheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
            btnUserRequestSignalQualityRecheck.ForeColor = Color.White;
        }

        // User selected the button indicating they've recently done some action that
        // warrants a signal quality recheck
        private void btnUserRequestSignalQualityRecheck_Click(object sender, EventArgs e)
        {
            // Button was not selected previously
            if (!UserRequestedRecheck)
            {
                UserRequestedRecheck = true;
                btnUserRequestSignalQualityRecheck.BackColor = Color.LimeGreen;
                btnUserRequestSignalQualityRecheck.ForeColor = Color.Black;
            }

            // Button already selected previously - unselect it
            else if (UserRequestedRecheck)
            {
                UserRequestedRecheck = false;
                btnUserRequestSignalQualityRecheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(36)))), ((int)(((byte)(51)))));
                btnUserRequestSignalQualityRecheck.ForeColor = Color.White;
            }
        }
    }
}