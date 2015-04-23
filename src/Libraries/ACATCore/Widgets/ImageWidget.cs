////////////////////////////////////////////////////////////////////////////
// <copyright file="ImageWidget.cs" company="Intel Corporation">
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
using System.Xml;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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
    /// A button that displays an image.  The image can
    /// be cropped to fit into the rectangle. All crop
    /// parameters are configurable through the xml file
    /// for the scanner
    /// </summary>
    public class ImageWidget : PictureBoxWidgetBase
    {
        /// <summary>
        /// Height of the cropped image
        /// </summary>
        private int _cropHeight;

        /// <summary>
        /// Width of the cropped image
        /// </summary>
        private int _cropWidth;

        /// <summary>
        /// X origin
        /// </summary>
        private int _cropX;

        /// <summary>
        /// Y origin
        /// </summary>
        private int _cropY;

        /// <summary>
        /// Has this object been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The image to display
        /// </summary>
        private Image _image;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="uiControl">the inner .NET Control for the widget</param>
        public ImageWidget(Control uiControl)
            : base(uiControl)
        {
        }

        /// <summary>
        /// Load settings we are interested in
        /// </summary>
        /// <param name="node">xml node</param>
        public override void Load(XmlNode node)
        {
            base.Load(node);
            _cropX = XmlUtils.GetXMLAttrInt(node, "cropX", 0);
            _cropY = XmlUtils.GetXMLAttrInt(node, "cropY", 0);
            _cropWidth = XmlUtils.GetXMLAttrInt(node, "cropWidth", 0);
            _cropHeight = XmlUtils.GetXMLAttrInt(node, "cropHeight", 0);
        }

        /// <summary>
        /// Sets the image.  The Label field in the attribute object
        /// is the name of the image file (not the full name, just the filename).
        /// The image is loaded from ACAT's Images dir
        /// </summary>
        /// <param name="attribute">WidgetAttribute object</param>
        public override void SetWidgetAttribute(WidgetAttribute attribute)
        {
            base.SetWidgetAttribute(attribute);

            // the label attribute points to the filename. Get the image,
            // resize it and prep it
            String imageFile = FileUtils.GetImagePath(attribute.Label);
            if (_cropX >= 0 && _cropY >= 0 && _cropWidth > 0 && _cropHeight > 0)
            {
                _image = ImageUtils.ImageCrop(imageFile, new Rectangle(_cropX, _cropY, _cropWidth, _cropHeight));
            }
            else
            {
                _image = Image.FromFile(imageFile);
            }

            _image = ImageUtils.ImageResize(_image, UIControl.Width, UIControl.Height);
            pictureBox.Image = _image;
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
        /// Render the image onto the screen
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">eventargs</param>
        protected override void UIControl_Paint(object sender, PaintEventArgs e)
        {
            if (isDisposing)
            {
                return;
            }

            try
            {
                SolidBrush backgroundBrush = createBackgroundBrush();
                e.Graphics.FillRectangle(backgroundBrush, UIControl.ClientRectangle);
                e.Graphics.DrawImage(_image, new Point(0, 0));
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        private void unInit()
        {
            if (_image != null)
            {
                _image.Dispose();
                _image = null;
            }
        }
    }
}