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

using ACAT.Core.Utility;
using ACAT.UserControls;

namespace ACAT.Scanners.UserControls
{
    [Descriptor("17E10490-5322-4C8A-801A-656A79BBA4EF",
                    "KeyboardAbcUserControl",
                    "User Control for Abc keyboard")]
    public partial class KeyboardAbcUserControl : KeyboardUserControl
    {
        public KeyboardAbcUserControl()
        {
            InitializeComponent();
        }
    }
}