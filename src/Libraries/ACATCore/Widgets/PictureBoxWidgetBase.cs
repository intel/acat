////////////////////////////////////////////////////////////////////////////
// <copyright file="PictureBoxWidgetBase.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.Widgets
{
    /// <summary>
    /// Base class for all button widgets that use a PictureBox
    /// as the .NET control for displaying text/images.
    /// </summary>
    public class PictureBoxWidgetBase : ButtonWidgetBase
    {
        /// <summary>
        /// The Picturebox element for this widget
        /// </summary>
        protected PictureBox pictureBox;

        /// <summary>
        /// Has this been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="control">the inner .NET Control for the widget</param>
        public PictureBoxWidgetBase(Control control)
            : base(control)
        {
            IsHighlightOn = false;
            IsSelectedHighlightOn = false;

            if (control is PictureBox)
            {
                pictureBox = (PictureBox)control;
            }

            control.Invalidate();
        }

        /// <summary>
        /// Should this be added for animation?
        /// </summary>
        /// <returns>true if is should</returns>
        public override bool CanAddForAnimation()
        {
            return AddForAnimation;
        }

        /// <summary>
        /// Return the text for a buttonkey
        /// </summary>
        /// <returns>Text</returns>
        public override string GetText()
        {
            return (widgetAttribute != null) ? widgetAttribute.Value : String.Empty;
        }

        /// <summary>
        /// Set the text for this widget
        /// </summary>
        /// <param name="text">Text to set</param>
        public override void SetText(string text)
        {
            widgetAttribute.Label = text;
            widgetAttribute.Value = text;
        }

        /// <summary>
        /// Creates a background brush using the preferred ColorScheme depending
        /// on the highlight state of the widget
        /// </summary>
        /// <returns></returns>
        protected SolidBrush createBackgroundBrush()
        {
            SolidBrush backgroundBrush;

            if (IsSelectedHighlightOn)
            {
                backgroundBrush = new SolidBrush(WidgetLayout.Colors.HighlightSelectedBackground);
            }
            else if (IsHighlightOn)
            {
                backgroundBrush = new SolidBrush(WidgetLayout.Colors.HighlightBackground);
            }
            else
            {
                backgroundBrush = new SolidBrush(WidgetLayout.Colors.Background);
            }

            return backgroundBrush;
        }

        /// <summary>
        /// Creates a fg brush using the preferred ColorScheme depending on
        /// the highlight state of the widget
        /// </summary>
        /// <returns></returns>
        protected SolidBrush createForegroundBrush()
        {
            SolidBrush foregroundBrush;

            if (IsSelectedHighlightOn)
            {
                foregroundBrush = new SolidBrush(WidgetLayout.Colors.HighlightSelectedForeground);
            }
            else if (IsHighlightOn)
            {
                foregroundBrush = new SolidBrush(WidgetLayout.Colors.HighlightForeground);
            }
            else
            {
                foregroundBrush = new SolidBrush(WidgetLayout.Colors.Foreground);
            }

            return foregroundBrush;
        }

        /// <summary>
        /// Dispose resources
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
        /// Unhighlight this element using ColorScheme settings
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOff()
        {
            bool handled;
            triggerEvtHighlightOff(out handled);
            IsHighlightOn = false;
            if (!handled)
            {
                UIControl.Invalidate();
            }

            return true;
        }

        /// <summary>
        /// Highlight this element using Colorscheme settings
        /// </summary>
        /// <returns>true</returns>
        protected override bool highlightOn()
        {
            bool handled;

            triggerEvtHighlightOn(out handled);
            IsHighlightOn = true;
            if (!handled)
            {
                UIControl.Invalidate();
            }

            return true;
        }

        /// <summary>
        /// Turn selected hightlight off using ColorScheme settings
        /// </summary>
        /// <returns>true</returns>
        protected override bool selectedHighlightOff()
        {
            IsSelectedHighlightOn = false;
            UIControl.Invalidate();
            return true;
        }

        /// <summary>
        /// Turn selected hightlight on using ColorScheme settings
        /// </summary>
        /// <returns>true</returns>
        protected override bool selectedHighlightOn()
        {
            IsSelectedHighlightOn = true;
            UIControl.Invalidate();
            return true;
        }

        /// <summary>
        /// Release resources
        /// </summary>
        private void unInit()
        {
        }
    }
}