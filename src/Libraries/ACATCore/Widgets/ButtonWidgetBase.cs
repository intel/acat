////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// This is the base class for all the Button widgets in ACAT
    /// </summary>
    public class ButtonWidgetBase : Widget, IButtonWidget
    {
        /// <summary>
        /// The widgetAttribute object that encapsulates all
        /// the attributes for this widget
        /// </summary>
        protected WidgetAttribute widgetAttribute;

        /// <summary>
        /// Has this been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ButtonWidgetBase(Control uiControl)
            : base(uiControl)
        {
            widgetAttribute = null;
            Value = null;

            uiControl.Paint += UIControl_Paint;
        }

        /// <summary>
        /// Returns the widgetAttributes object for this widget
        /// </summary>
        /// <returns>the object</returns>
        public WidgetAttribute GetWidgetAttribute()
        {
            return widgetAttribute;
        }

        /// <summary>
        /// Sets the WidgetAttribute for the button. Override this
        /// to set properties specific to the individual widget
        /// types
        /// </summary>
        /// <param name="attr">the WidgetAttribute object</param>
        public virtual void SetWidgetAttribute(WidgetAttribute attr)
        {
            widgetAttribute = attr;
            Value = widgetAttribute.Value;
            if (!string.IsNullOrEmpty(Value) && Value[0] == '@' && Value.Length > 1 && !widgetAttribute.IsVirtualKey)
            {
                Command = Value.Substring(1);
                IsCommand = true;
            }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources

                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// Override this to render the widget
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void UIControl_Paint(object sender, PaintEventArgs e)
        {
        }

        /// <summary>
        /// Releases resources
        /// </summary>
        private void unInit()
        {
            if (UIControl != null)
            {
                UIControl.Paint -= UIControl_Paint;
            }
        }
    }
}