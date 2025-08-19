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
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ACAT.Extension.UI.UserControls
{
    [DesignerCategory("Code")]
    public abstract class GenericUserControl : UserControl, IUserControl
    {
        public event AnimationPlayerStateChanged EvtPlayerStateChanged;

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public ClassDescriptorAttribute Descriptor => ClassDescriptorAttribute.GetDescriptor(GetType());

        public Guid Id => Descriptor.Id;

        /// <summary>
        /// Gets the snchronization object
        /// </summary>
        public SyncLock SyncObj => _userControlCommon.SyncObj;

        public IUserControlCommon UserControlCommon => _userControlCommon;
        protected UserControlCommon _userControlCommon { get; set; }

        public virtual bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _userControlCommon = new UserControlCommon(this, mapEntry, scanner);

            bool retVal = _userControlCommon.Initialize();

            _userControlCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;
            retVal = HandleInitialize();

            return retVal;
        }

        public virtual void OnLoad()
        {
            _userControlCommon.OnLoad();
            _userControlCommon.AnimationManager.OnLoad(_userControlCommon.RootWidget);
        }
        
        public virtual void OnPause()
        {
            _userControlCommon.OnPause();
        }

        public virtual void OnResume()
        {
            _userControlCommon.OnResume();
        }

        public virtual void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            handled = false;
        }

        protected virtual bool HandleInitialize()
        {
            //Override this method in derived classes to perform additional initialization
            return true;
        }

        protected void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
        }
    }
}