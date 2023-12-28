////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Represents a button control on a form.
    /// </summary>
    public interface IToggleButtonWidget : IButtonWidget
    {
        /// <summary>
        /// Event raised which this widget is actuated
        /// </summary>
        event EventHandler EvtToggleStateChanged;

        String ToggleGroup { get; set; }
        bool ToggleState { get; set; }
    }
}