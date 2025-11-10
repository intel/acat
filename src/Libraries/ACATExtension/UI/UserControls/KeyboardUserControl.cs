/// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0

using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement;
using System.ComponentModel;

namespace ACAT.Extension.UI.UserControls
{
    [DesignerCategory("Code")]
    public class KeyboardUserControl : GenericUserControl
    {
        public UserControlKeyboardCommon _keybordUserControlCommon => (UserControlKeyboardCommon)_userControlCommon;

        public override bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _userControlCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            bool retVal = _userControlCommon.Initialize();

            _userControlCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;
            retVal = HandleInitialize();
            
            return retVal;
        }

        protected override bool HandleInitialize()
        {
            return true;
        }
    }
}