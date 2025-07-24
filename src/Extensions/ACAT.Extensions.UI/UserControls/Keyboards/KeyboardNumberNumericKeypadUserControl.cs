////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardNumberNumericKeypadUserControl.cs
//
// User control for the numeric and punctuation keys. The numeric keys are
// displayed as a keypard as on real keyboard.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.UserControls;

namespace ACAT.Scanners.UserControls
{
    [ClassDescriptor("3E99C700-C3C4-4D98-9D3C-CA17FF811E25",
                    "KeyboardNumberUserControlNumericKeypad",
                    "User Control for Numerc keyboard with numeric keypad on the left")]
    public partial class KeyboardNumberNumericKeypadUserControl : KeyboardUserControl
    {
        public KeyboardNumberNumericKeypadUserControl()
        {
            InitializeComponent();
        }
    }
}