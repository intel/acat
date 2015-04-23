////////////////////////////////////////////////////////////////////////////
// <copyright file="ImageUtils.cs" company="Intel Corporation">
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
using System.Drawing.Imaging;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Utility functions to manipulate images
    /// </summary>
    public class ImageUtils
    {
        /// <summary>
        /// Converts the specified icon to a bitmap
        /// </summary>
        /// <param name="icon">icon to convert</param>
        /// <param name="size">size to convert to</param>
        /// <returns>bitmap version of the icon</returns>
        public static Bitmap IconToBitmap(Icon icon, Size size)
        {
            var bitmap = new Bitmap(size.Width, size.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.DrawIcon(icon, new Rectangle(0, 0, size.Width, size.Height));
            return bitmap;
        }

        /// <summary>
        /// Crops an image to the specified rectangle
        /// </summary>
        /// <param name="imageFile">ful path to the image file</param>
        /// <param name="cropRectangle">rect to crop to</param>
        /// <returns>cropped image</returns>
        public static Bitmap ImageCrop(String imageFile, Rectangle cropRectangle)
        {
            Bitmap retVal = null;
            try
            {
                var image = Image.FromFile(imageFile) as Bitmap;
                retVal = ImageCrop(image, cropRectangle);
            }
            catch (Exception e)
            {
                Log.Error("ImageCrop:  Could not crop image " + imageFile + ". Exception: " + e.ToString());
            }

            return retVal;
        }

        /// <summary>
        /// Crops an image to the specified crop rectangle
        /// </summary>
        /// <param name="image">the image to crop</param>
        /// <param name="cropRectangle">rectangle to crop to</param>
        /// <returns>cropped image</returns>
        public static Bitmap ImageCrop(Image image, Rectangle cropRectangle)
        {
            var bitmap = new Bitmap(cropRectangle.Width, cropRectangle.Height);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), cropRectangle, GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        /// <summary>
        /// Sets the transparency of an image and renders it into the destination rectangle
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="opacity">Opacity 0.0 to 1.0</param>
        /// <param name="destRectangle">Destination rect</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ImageOpacity(Image img, float opacity, Rectangle destRectangle)
        {
            var bitmap = new Bitmap(destRectangle.Width, destRectangle.Height);
            var graphic = Graphics.FromImage(bitmap);

            var colormatrix = new ColorMatrix { Matrix33 = opacity };

            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            graphic.DrawImage(img, new Rectangle(destRectangle.X, destRectangle.Y,
                                                bitmap.Width, bitmap.Height),
                                                0, 0, img.Width, img.Height,
                                                GraphicsUnit.Pixel, imageAttributes);

            return bitmap;
        }

        /// <summary>
        /// Resize an image to the specified width/height
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="width">Resize to this width.</param>
        /// <param name="height">Resize to this height.</param>
        /// <returns>The resultant resized image.</returns>
        public static Bitmap ImageResize(Image image, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
            }
            return bitmap;
        }
    }
}