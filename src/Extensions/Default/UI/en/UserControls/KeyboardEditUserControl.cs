////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardEditUserControl.cs
//
// User control for edit functions such as character, word and sentence
// navigation, deleting characters, words, sentences etc.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Scanners.UserControls
{
    [DescriptorAttribute("82C88926-74B3-4FDA-B881-4ACF9998F4AF",
                    "KeyboardEditUserControl",
                    "User Control for Qwerty keyboard")]
    public partial class KeyboardEditUserControl
        : UserControl, IUserControl
    {
        private UserControlKeyboardCommon _keyboardCommon;

        public KeyboardEditUserControl()
        {
            InitializeComponent();
        }

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
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

            return retVal;
        }

        public void OnLoad()
        {
            _keyboardCommon.OnLoad();
            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public void OnPause()
        {
        }

        public void OnResume()
        {
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            handled = false;
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            if (EvtPlayerStateChanged != null)
            {
                EvtPlayerStateChanged(this, e);
            }
        }
    }
}