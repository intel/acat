////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.Extension.UI.UserControls;
using System;

namespace ACAT.Extension
{
    /// <summary>
    /// The ACAT talk interface description of the main keyboard layout
    /// </summary>
    [ClassDescriptor("30D1EF21-E8F5-4E78-8D98-C8E93B992A81",
                        "UserControlDefaultTalkInterface",
                    "User Control show description of keboard")]
    public partial class UserControlLayoutInterface : KeyboardUserControl
    {
        public UserControlLayoutInterface()
        {
            InitializeComponent();
        }

        protected override bool HandleInitialize()
        {
            checkBoxDontShowThisOnStartup.Checked = false;
            return true;
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (checkBoxDontShowThisOnStartup.Checked)
            {
                Common.AppPreferences.ShowTalkInterfaceDescOnStartup = false;
                Common.AppPreferences.Save();
            }
            _userControlCommon.ScannerForm.Close();
        }
    }
}