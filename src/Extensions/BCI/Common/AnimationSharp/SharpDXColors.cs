////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SharpDXColors.cs
//
/// Contains all the colors used by the UI of BCI
//
////////////////////////////////////////////////////////////////////////////

using SharpDX.Direct2D1;
using System.Drawing;

namespace ACAT.Extensions.BCI.Common.AnimationSharp
{
    public class SharpDXColors
    {
        /// <summary>
        /// Colors and objects used by the UI
        /// </summary>
        /// 
        public SharpDX.Mathematics.Interop.RawColor4 ColorActuateDecision = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 1.0f, 1.0f);

        public SharpDX.Mathematics.Interop.RawColor4 ColorBackground;

        public SharpDX.Mathematics.Interop.RawColor4 ColorCorrectDecision = new SharpDX.Mathematics.Interop.RawColor4(0.0f, 1.0f, 0.0f, 1.0f);

        public SharpDX.Mathematics.Interop.RawColor4 ColorDisabledButton = new SharpDX.Mathematics.Interop.RawColor4(0.35f, 0.35f, 0.35f, 1.0f);

        public SharpDX.Mathematics.Interop.RawColor4 ColorIncorrectDecision = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 0.0f, 0.0f, 1.0f);

        public SharpDX.Mathematics.Interop.RawColor4 ColorTarget = new SharpDX.Mathematics.Interop.RawColor4(0.0f, 1.0f, 0.0f, 1.0f);

        public SharpDX.Mathematics.Interop.RawColor4 ColorWhite = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 1.0f, 1.0f, 1.0f);

        public SolidColorBrush SolidColorBrushButtonTextOff;

        public SolidColorBrush SolidColorBrushButtonTextOn;

        public SolidColorBrush SolidColorBrushDecision;

        public SolidColorBrush SolidColorBrushDecisionActuate;

        public SolidColorBrush SolidColorBrushDecisionCorrect;

        public SolidColorBrush SolidColorBrushDecisionIncorrect;

        public SolidColorBrush SolidColorBrushDisabledButton;

        public SolidColorBrush SolidColorBrushExtraBoxOff;

        public SolidColorBrush SolidColorBrushExtraBoxOn;

        public SolidColorBrush SolidColorBrushOff;

        public SolidColorBrush SolidColorBrushOn;

        public SolidColorBrush SolidColorBrushProgressBars;

        public SolidColorBrush SolidColorBrushTarget;

        public SolidColorBrush SolidColorWhite;

        private RenderTarget SharpDX_d2dRenderTarget;
        public SharpDXColors(RenderTarget sharpDX_d2dRenderTarget) 
        {
            SharpDX_d2dRenderTarget = sharpDX_d2dRenderTarget;
            InitializeColors();
        }
        /// <summary>
        /// Gets the color codes from given Color
        /// </summary>
        /// <param name="colorCodeRegion">name of the color</param>
        /// <returns></returns>
        public SharpDX.Mathematics.Interop.RawColor4 GetColorScheme(Color colorCodeRegion)
        {
            return new SharpDX.Mathematics.Interop.RawColor4(
                ((float)colorCodeRegion.R / 255),
                ((float)colorCodeRegion.G / 255),
                ((float)colorCodeRegion.B / 255),
                1.0f
                );
        }

        /// <summary>
        /// Initialize the colors used in the UI of BCI
        /// </summary>
        private void InitializeColors()
        {
            var mainColorScheme = AnimationManagerUtils.GetMainColorScheme("BCIColorCodedRegionDefault");

            ColorBackground = GetColorScheme(mainColorScheme.Background);

            SolidColorBrushOn = new SolidColorBrush(SharpDX_d2dRenderTarget, GetColorScheme(mainColorScheme.Foreground));

            SolidColorBrushOff = new SolidColorBrush(SharpDX_d2dRenderTarget, GetColorScheme(mainColorScheme.Background));

            SolidColorBrushButtonTextOn = new SolidColorBrush(SharpDX_d2dRenderTarget, GetColorScheme(mainColorScheme.HighlightSelectedBackground));

            SolidColorBrushButtonTextOff = new SolidColorBrush(SharpDX_d2dRenderTarget, GetColorScheme(mainColorScheme.HighlightSelectedForeground));

            SolidColorBrushExtraBoxOn = new SolidColorBrush(SharpDX_d2dRenderTarget, GetColorScheme(mainColorScheme.HighlightForeground));

            SolidColorBrushExtraBoxOff = new SolidColorBrush(SharpDX_d2dRenderTarget, GetColorScheme(mainColorScheme.HighlightSelectedBackground));

            SolidColorBrushProgressBars = new SolidColorBrush(SharpDX_d2dRenderTarget, GetColorScheme(mainColorScheme.HighlightForeground));

            SolidColorBrushDecision = new SolidColorBrush(SharpDX_d2dRenderTarget, ColorCorrectDecision);

            SolidColorBrushDecisionCorrect = new SolidColorBrush(SharpDX_d2dRenderTarget, ColorCorrectDecision);

            SolidColorBrushDecisionIncorrect = new SolidColorBrush(SharpDX_d2dRenderTarget, ColorIncorrectDecision);

            SolidColorBrushTarget = new SolidColorBrush(SharpDX_d2dRenderTarget, ColorTarget);

            SolidColorBrushDecisionActuate = new SolidColorBrush(SharpDX_d2dRenderTarget, ColorActuateDecision);

            SolidColorBrushDisabledButton = new SolidColorBrush(SharpDX_d2dRenderTarget, ColorDisabledButton);

            SolidColorWhite = new SolidColorBrush(SharpDX_d2dRenderTarget, ColorWhite);
        }
    }
}
