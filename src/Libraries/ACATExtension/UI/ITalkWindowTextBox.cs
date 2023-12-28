////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Windows.Forms;

namespace ACAT.Extensions
{
    /// <summary>
    /// The interface for the textbox that is displayed in the talk application
    /// </summary>
    public interface ITalkWindowTextBox
    {
        TextBox TextBoxControl { get; }

        void OnPause();

        void OnResume();
    }
}