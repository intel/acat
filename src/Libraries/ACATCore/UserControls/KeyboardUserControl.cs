////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardAbcUserControl.cs
//
// User control for the keyboard that is alphabetically arranged.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System.Windows.Forms;

namespace ACAT.UserControls
{
    public partial class KeyboardUserControl : UserControl, IUserControl
    {
        protected UserControlKeyboardCommon _keyboardCommon { get; set; }

        public KeyboardUserControl()
        {
        }

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public ClassDescriptorAttribute Descriptor => ClassDescriptorAttribute.GetDescriptor(GetType());

        /// <summary>
        /// Gets the snchronization object
        /// </summary>
        public SyncLock SyncObj => _keyboardCommon.SyncObj;

        public IUserControlCommon UserControlCommon => _keyboardCommon;

        public virtual bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            bool retVal = _keyboardCommon.Initialize();

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            return retVal;
        }

        public virtual void OnLoad()
        {
            _keyboardCommon.OnLoad();
            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public virtual void OnPause()
        {
            _keyboardCommon.OnPause();
        }

        public virtual void OnResume()
        {
            _keyboardCommon.OnResume();
        }

        public virtual void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            handled = false;
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
        }
    }
}