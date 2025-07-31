////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.UserControls;
using System;
using System.Windows.Forms;

namespace ACAT.Extension
{
    /// <summary>
    /// The ACAT talk interface description of the main keyboard layout
    /// </summary>
    [ClassDescriptor("30D1EF21-E8F5-4E78-8D98-C8E93B992A81",
                        "UserControlDefaultTalkInterface",
                    "User Control show description of keboard")]
    public partial class UserControlLayoutInterface : GenericUserControl, IUserControl
    {
        private UserControlKeyboardCommon _keyboardCommon;

        public UserControlLayoutInterface()
        {
            InitializeComponent();
        }

        public override bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            bool retVal = _keyboardCommon.Initialize();

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            checkBoxDontShowThisOnStartup.Checked = false;

            return retVal;
        }


        private void buttonDone_Click(object sender, EventArgs e)
        {
            if (checkBoxDontShowThisOnStartup.Checked)
            {
                Common.AppPreferences.ShowTalkInterfaceDescOnStartup = false;
                Common.AppPreferences.Save();
            }
            _keyboardCommon.ScannerForm.Close();
        }
    }
}