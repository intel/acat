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

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public ClassDescriptorAttribute Descriptor
        {
            get { return ClassDescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the snchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _keyboardCommon.SyncObj; }
        }

        public IUserControlCommon UserControlCommon
        {
            get
            {
                return _keyboardCommon;
            }
        }

        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            bool retVal = _keyboardCommon.Initialize();

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            checkBoxDontShowThisOnStartup.Checked = false;

            return retVal;
        }

        public void OnLoad()
        {
            _keyboardCommon.OnLoad();
            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public void OnPause()
        {
            _keyboardCommon.OnPause();
        }

        public void OnResume()
        {
            _keyboardCommon.OnResume();
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            throw new NotImplementedException();
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
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