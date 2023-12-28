////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Represents a button control on a form.
    /// </summary>
    public interface IButtonWidget
    {
        /// <summary>
        /// Returns attributes of the button
        /// </summary>
        /// <returns>The attributes object</returns>
        WidgetAttribute GetWidgetAttribute();

        /// <summary>
        /// Sets the WidgetAttribute for the button. Override this
        /// to set properties specific to the individual button
        /// types
        /// </summary>
        /// <param name="attr">the attribute object</param>
        void SetWidgetAttribute(WidgetAttribute attr);
    }
}