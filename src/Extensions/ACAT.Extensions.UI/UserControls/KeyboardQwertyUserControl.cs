////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardQwertyUserControl.cs
//
// User control for the QWERT layout of the alphabet keyboard
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.UserControls;

namespace ACAT.Scanners.UserControls
{
    [ClassDescriptor("C4668F6A-79D6-4D27-8C68-18172A49F333",
                    "KeyboardQwertyUserControl",
                    "User Control for Qwerty keyboard")]
    public partial class KeyboardQwertyUserControl : KeyboardUserControl
    {
        public KeyboardQwertyUserControl()
        {
            InitializeComponent();
        }
    }
}