////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// KeyboardNumberUserControl.cs
//
// User control for the numeric and punctuation keys. The numeric keys are
// displayed in the top row of the keyboard
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility;
using ACAT.UserControls;

namespace ACAT.Scanners.UserControls
{
    [Descriptor("F8E111D9-796A-4FE0-AC1E-7CD24839FD78",
                    "KeyboardNumberUserControl",
                    "User Control for number keyboard")]
    public partial class KeyboardNumberUserControl : KeyboardUserControl
    {
        public KeyboardNumberUserControl()
        {
            InitializeComponent();
        }
    }
}