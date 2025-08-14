////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Core.WidgetManagement.Interfaces
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

        string ToggleGroup { get; set; }
        bool ToggleState { get; set; }
    }
}