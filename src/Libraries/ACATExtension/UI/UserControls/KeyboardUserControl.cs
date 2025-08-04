/// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0

using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using System.Windows.Forms;

namespace ACAT.UserControls
{
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