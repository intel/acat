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

using ACAT.Core.Utility;
using ACAT.UserControls;

namespace ACAT.Scanners.UserControls
{
    [Descriptor("82C88926-74B3-4FDA-B881-4ACF9998F4AF",
                    "KeyboardEditUserControl",
                    "User Control for Qwerty keyboard")]
    public partial class KeyboardEditUserControl : KeyboardUserControl
    {
        public KeyboardEditUserControl()
        {
            InitializeComponent();
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }
    }
}