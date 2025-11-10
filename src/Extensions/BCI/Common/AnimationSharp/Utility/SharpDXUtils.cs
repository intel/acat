////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////
using ACAT.Core.ThemeManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WidgetManagement.Interfaces;
using ACAT.Extensions.BCI.Common.BCIControl;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace ACAT.Extensions.BCI.Common.AnimationSharp.Utility
{
    public class SharpDXUtils
    {
        /// <summary>
        /// Get the border color from the Theme xml file
        /// </summary>
        /// <param name="colorCodeRegion">name of the color</param>
        /// <returns></returns>
        public static RawColor4 GetBorderColorScheme(string colorCodeRegion)
        {
            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(colorCodeRegion);
            return ConvertToRawColor4(colorScheme);
        }

        /// <summary>
        /// Gets a rectangle used as focal area for box calibration
        /// </summary>
        /// <param name="rawRoundRectangleList"></param>
        /// <param name="cornerRadius"></param>
        /// <returns></returns>
        public static RoundedRectangle GetFocalAreaForBox(List<RoundedRectangle> rawRoundRectangleList, float width, float height, float cornerRadius, float distanceFromEdge, BCIFocalAreaRegion bCIFocalAreaRegions)
        {
            try
            {
                RawRectangleF focalRect = GetFocalAreaFromRectangle(GetBoundingRectangle(rawRoundRectangleList), width, height, distanceFromEdge, bCIFocalAreaRegions);
                return GetRoundedRectangle(focalRect, cornerRadius);
            }
            catch (Exception)
            {
                return new RoundedRectangle();
            }
        }

        /// <summary>
        /// Gets the type of font style
        /// </summary>
        /// <param name="useItalic"></param>
        /// <returns></returns>
        public static SharpDX.DirectWrite.FontStyle GetFontStyle(bool useItalic)
        {
            return useItalic ? SharpDX.DirectWrite.FontStyle.Italic : SharpDX.DirectWrite.FontStyle.Normal;
        }

        /// <summary>
        /// Gets te Type of weight font
        /// </summary>
        /// <param name="useBold"></param>
        /// <returns></returns>
        public static FontWeight GetFontWeight(bool useBold)
        {
            return useBold ? FontWeight.Black : FontWeight.Normal;
        }

        /// <summary>
        /// Gets the list of all the type of box for each user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="widgetsData"></param>
        /// <param name="directWriteFactory"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <param name="typeOfBox"></param>
        /// <returns></returns>
        public static List<TextFormat>[] GetListButtonTextFormat(KeyValuePair<List<Control>, string> boxesData, List<Widget> widgetsData, SharpDX.DirectWrite.Factory directWriteFactory, int totalAmountOfBoxes, out string[] typeOfBox)
        {
            List<TextFormat>[] buttonTextFormatList = new List<TextFormat>[totalAmountOfBoxes];
            typeOfBox = new string[totalAmountOfBoxes];
            int indexBox = 0;
            try
            {
                var temptypeOfBox = AnimationManagerUtils.GetTypeOfBox(boxesData.Value, totalAmountOfBoxes);
                var tempbuttonTextFormat = GetTextFormat(widgetsData, boxesData.Value, totalAmountOfBoxes, directWriteFactory, temptypeOfBox);
                for (int boxIndex = 0; boxIndex < totalAmountOfBoxes; boxIndex++)
                {
                    typeOfBox[indexBox] = temptypeOfBox[boxIndex];
                    buttonTextFormatList[indexBox] = tempbuttonTextFormat[boxIndex];
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Error in GetListButtonTextFormat: " + ex.Message);
                return new List<TextFormat>[totalAmountOfBoxes];
            }
            return buttonTextFormatList;
        }

        /// <summary>
        /// Gets the location and shape for each control in the form, returns list with rectangles
        /// </summary>
        /// <param name="controls">Controls of the form</param>
        /// <param name="configFilePath">path for the config file</param>
        /// <param name="amountBoxes">amount of boxes in the layout</param>
        /// <returns></returns>
        public static List<RawRectangleF>[] GetRecButtons(List<Control> controls, string configFilePath, int amountBoxes)
        {
            var configNodes = GetNodesList(configFilePath, "/ACAT/Layout/Layouts");
            int index = 0;
            string xmlSection = "KeyboardBoxMapping".ToLower();
            List<RawRectangleF>[] rectBtns = Enumerable.Range(0, amountBoxes).Select(_ => new List<RawRectangleF>()).ToArray();
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    string name = XmlUtils.GetXMLAttrString(node, "name", null).ToLower();
                    if (name.Equals(xmlSection))
                    {
                        foreach (XmlNode subnode in node)
                        {
                            string name2 = XmlUtils.GetXMLAttrString(subnode, "name", null);
                            int tag = int.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var btn = AnimationManagerUtils.GetControl(controls, name2, tag, false);
                            if (btn != null)
                            {
                                btn.Tag = tag;
                                rectBtns[index].Add(GetRectangleFromLocationOnForm(btn));
                            }
                        }
                        index += 1;
                    }
                    name = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            configNodes = null;
            return rectBtns;
        }

        /// <summary>
        /// Gets the rectangles for the probability bars for the buttons
        /// </summary>
        /// <param name="PBcontrols">Controls of the probs bars</param>
        /// <param name="configFilePath">path for the config file</param>
        /// <param name="amountBoxes">amount of boxes in the layout</param>
        /// <returns></returns>
        public static Dictionary<int, RawRectangleF>[] GetRecProbabilityBars(List<Control> PBcontrols, string configFilePath, int amountBoxes)
        {
            var configNodes = GetNodesList(configFilePath, "/ACAT/Layout/Layouts");
            string xmlSection = "ProgressBarsMapping".ToLower();
            int index = 0;
            Dictionary<int, RawRectangleF>[] probBars = Enumerable.Range(0, amountBoxes).Select(_ => new Dictionary<int, RawRectangleF>()).ToArray();
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    string name = XmlUtils.GetXMLAttrString(node, "name", null).ToLower();
                    if (name.Equals(xmlSection))
                    {
                        foreach (XmlNode subnode in node)
                        {
                            string name2 = XmlUtils.GetXMLAttrString(subnode, "name", null);
                            int tag = int.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var btn = AnimationManagerUtils.GetControl(PBcontrols, name2, tag, false);
                            if (btn != null)
                            {
                                btn.Tag = tag;
                                probBars[index].Add(tag, GetRectangleFromLocationOnForm(btn));
                            }
                        }
                        index += 1;
                    }
                    name = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            configNodes = null;
            return probBars;
        }

        /// <summary>
        /// Gets the rectangles for the probability bars for the boxes
        /// </summary>
        /// <param name="PBcontrols">Controls of the probs bars</param>
        /// <param name="configFilePath">path for the config file</param>
        /// <returns></returns>
        public static Dictionary<int, RawRectangleF> GetRecProbabilityBarsBox(List<Control> PBcontrols, string configFilePath)
        {
            var configNodes = GetNodesList(configFilePath, "/ACAT/Layout/Layouts");
            string xmlSection = "ProgressBarsBoxMapping".ToLower();
            Dictionary<int, RawRectangleF> probBars = new();
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    string name = XmlUtils.GetXMLAttrString(node, "name", null).ToLower();
                    if (name.Equals(xmlSection))
                    {
                        foreach (XmlNode subnode in node)
                        {
                            string name2 = XmlUtils.GetXMLAttrString(subnode, "name", null);
                            int tag = int.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var btn = AnimationManagerUtils.GetControl(PBcontrols, name2, tag, false);
                            if (btn != null)
                            {
                                btn.Tag = tag;
                                probBars.Add(tag, GetRectangleFromLocationOnForm(btn));
                            }
                        }
                    }
                    name = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return probBars;
        }

        /// <summary>
        /// Gets the location and shape for each control in the form, returns list with rounded rectangles
        /// </summary>
        /// <param name="controls">Controls of the form</param>
        /// <param name="configFilePath">path for the config file</param>
        /// <param name="amountBoxes">amount of boxes in the layout</param>
        /// <param name="radius">Value for the UI radius of the buttons corners</param>
        /// <returns></returns>
        public static List<RoundedRectangle>[] GetRecRoundButtons(List<Control> controls, string configFilePath, int amountBoxes, float radius)
        {
            var configNodes = GetNodesList(configFilePath, "/ACAT/Layout/Layouts");
            int index = 0;
            string xmlSection = "KeyboardBoxMapping".ToLower();
            List<RoundedRectangle>[] rectBtns = Enumerable.Range(0, amountBoxes).Select(_ => new List<RoundedRectangle>()).ToArray();
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    string name = XmlUtils.GetXMLAttrString(node, "name", null).ToLower();
                    if (name.Equals(xmlSection))
                    {
                        foreach (XmlNode subnode in node)
                        {
                            string name2 = XmlUtils.GetXMLAttrString(subnode, "name", null);
                            int tag = int.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var btn = AnimationManagerUtils.GetControl(controls, name2, tag, false);
                            if (btn != null)
                            {
                                btn.Tag = tag;
                                var rectBtn = GetRectangleFromLocationOnForm(btn);
                                rectBtns[index].Add(GetRoundedRectangle(rectBtn, radius));
                            }
                        }
                        index += 1;
                    }
                    name = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            configNodes = null;
            return rectBtns;
        }

        /// <summary>
        /// Gets the location and shape for the control for the CRG text section
        /// </summary>
        /// <param name="controls">Controls of the form</param>
        /// <returns>Rectangle</returns>
        public static RawRectangleF GetRectangleCRG(List<Control> controls)
        {
            return GetRectangleFromName(controls, "BtnCRG");
        }

        /// <summary>
        /// Gets the list of all the rectangles of each button for each box within the user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <returns></returns>
        public static List<RawRectangleF>[] GetRectanglesButtonsList(KeyValuePair<List<Control>, string> boxesData, int totalAmountOfBoxes)
        {
            List<RawRectangleF>[] rectanglesButtonsList = new List<RawRectangleF>[totalAmountOfBoxes];
            int indexBox = 0;
            try
            {
                List<RawRectangleF>[] temprectBtnsAll = GetRecButtons(boxesData.Key, boxesData.Value, totalAmountOfBoxes);
                for (int boxIndex = 0; boxIndex < totalAmountOfBoxes; boxIndex++)
                {
                    rectanglesButtonsList[indexBox] = temprectBtnsAll[boxIndex];
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Error in GetRectanglesButtonsList: " + ex.Message);
                return new List<RawRectangleF>[totalAmountOfBoxes];
            }
            return rectanglesButtonsList;
        }

        /// <summary>
        /// Gets the list of all the rounded rectangles of each button for each box within the user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <param name="radiusCornersButtons"></param>
        /// <returns></returns>
        public static List<RoundedRectangle>[] GetRectanglesButtonsRoundList(KeyValuePair<List<Control>, string> boxesData, int totalAmountOfBoxes, float radiusCornersButtons)
        {
            List<RoundedRectangle>[] rectanglesButtonsRoundList = new List<RoundedRectangle>[totalAmountOfBoxes];
            int indexBox = 0;
            try
            {
                var temprectBtnsAllRound = GetRecRoundButtons(boxesData.Key, boxesData.Value, totalAmountOfBoxes, radiusCornersButtons);
                for (int boxIndex = 0; boxIndex < totalAmountOfBoxes; boxIndex++)
                {
                    rectanglesButtonsRoundList[indexBox] = temprectBtnsAllRound[boxIndex];
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Error in GetRectanglesButtonsRoundList: " + ex.Message);
                return new List<RoundedRectangle>[totalAmountOfBoxes];
            }
            return rectanglesButtonsRoundList;
        }

        /// <summary>
        /// Gets the trigger box rectangle
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="widgetsData"></param>
        /// <param name="directWriteFactory"></param>
        /// <param name="offsetCRG"></param>
        /// <param name="buttonTextFormatCRG"></param>
        /// <returns></returns>
        public static RawRectangleF GetRectanglesCRG(Dictionary<List<Control>, string> boxesData, List<Widget>[] widgetsData, SharpDX.DirectWrite.Factory directWriteFactory, out int offsetCRG, out TextFormat buttonTextFormatCRG)
        {
            RawRectangleF rectangleExtraButtonCRG = new();
            _ = GetDefaultButtonTextFormat(directWriteFactory, TextAlignment.Center);
            offsetCRG = 1;
            buttonTextFormatCRG = GetDefaultButtonTextFormat(directWriteFactory, TextAlignment.Center);
            int amountBoxesPerUserControl;
            int indexBox = 0;
            try
            {
                for (int indexBoxData = 0; indexBoxData < boxesData.Count; indexBoxData++)
                {
                    var boxData = boxesData.ElementAt(indexBoxData);
                    amountBoxesPerUserControl = AnimationManagerUtils.GetAmountBoxes(boxData.Value);
                    var temprectExtraButtonCRG = GetRectangleCRG(boxData.Key);
                    int tempoffsetCRG = AnimationManagerUtils.GetOffsetCRG(widgetsData[indexBoxData]);
                    TextFormat tempbuttonTextFormatCRG = GetTextFormatCRG(widgetsData[indexBoxData], directWriteFactory);
                    for (int boxIndex = 0; boxIndex < amountBoxesPerUserControl; boxIndex++)
                    {
                        if (temprectExtraButtonCRG.Bottom != 0 && temprectExtraButtonCRG.Top != 0 && temprectExtraButtonCRG.Left != 0 && temprectExtraButtonCRG.Right != 0)
                        {
                            rectangleExtraButtonCRG = temprectExtraButtonCRG;
                            offsetCRG = tempoffsetCRG;
                            buttonTextFormatCRG = tempbuttonTextFormatCRG;
                        }
                        indexBox += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Error in GetRectanglesCRG: " + ex.Message);
                return rectangleExtraButtonCRG;
            }
            return rectangleExtraButtonCRG;
        }

        /// <summary>
        /// Gets the trigger box rectangle
        /// </summary>
        /// <param name="boxesData"></param>
        /// <returns></returns>
        public static RawRectangleF GetRectanglesTriggerBox(Dictionary<List<Control>, string> boxesData)
        {
            RawRectangleF rectangleExtraButton = new();
            int amountBoxesPerUserControl;
            int indexBox = 0;
            try
            {
                foreach (var boxData in boxesData)
                {
                    amountBoxesPerUserControl = AnimationManagerUtils.GetAmountBoxes(boxData.Value);
                    var temprectExtraButton = GetRectangleTriggerBox(boxData.Key);
                    for (int boxIndex = 0; boxIndex < amountBoxesPerUserControl; boxIndex++)
                    {
                        if (temprectExtraButton.Bottom != 0 && temprectExtraButton.Top != 0 && temprectExtraButton.Left != 0 && temprectExtraButton.Right != 0)
                            rectangleExtraButton = temprectExtraButton;
                        indexBox += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Error in GetRectanglesTriggerBox: " + ex.Message);
                return rectangleExtraButton;
            }
            return rectangleExtraButton;
        }

        /// <summary>
        /// Gets the location and shape for the control known as trigger in the panel, returns a rectangle
        /// </summary>
        /// <param name="controls">Controls of the form</param>
        /// <returns>Rectangle</returns>
        public static RawRectangleF GetRectangleTriggerBox(List<Control> controls)
        {
            return GetRectangleFromName(controls, "BTrigger");
        }

        /// <summary>
        /// Get a rectangle with round edges
        /// </summary>
        /// <param name="rawRectangleF"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static RoundedRectangle GetRoundedRectangle(RawRectangleF rawRectangleF, float radius)
        {
            return new RoundedRectangle { Rect = rawRectangleF, RadiusX = radius, RadiusY = radius };
        }

        /// <summary>
        /// Gets the text format for each of the buttons in the User Control
        /// </summary>
        /// <param name="widgets">List of controls</param>
        /// <param name="configFilePath">Path to the config file</param>
        /// <param name="amountBoxes">Amount of Boxes in main UI</param>
        /// <param name="directWriteFactory">SharpDX object</param>
        /// <returns>List with the text format for each button</returns>
        public static List<TextFormat>[] GetTextFormat(List<Widget> widgets, string configFilePath, int amountBoxes, SharpDX.DirectWrite.Factory directWriteFactory, string[] typeBox)
        {
            var configNodes = GetNodesList(configFilePath, "/ACAT/Layout/Layouts");
            int index = 0;
            string xmlSection = "KeyboardBoxMapping".ToLower();
            List<TextFormat>[] textFormats = Enumerable.Range(0, amountBoxes).Select(_ => new List<TextFormat>()).ToArray();
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    string name = XmlUtils.GetXMLAttrString(node, "name", null).ToLower();
                    if (name.Equals(xmlSection))
                    {
                        foreach (XmlNode subnode in node)
                        {
                            string name2 = XmlUtils.GetXMLAttrString(subnode, "name", null);
                            int tag = int.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var btnWidget = widgets.SelectMany(w => w.Children).FirstOrDefault(w => w.Name.Equals(name2) && w.UIControl.Tag != null && int.Parse(w.UIControl.Tag.ToString()) == tag);
                            if (typeBox[index] != null)
                            {
                                if (typeBox[index].ToLower().Contains("keyboard".ToLower()) || typeBox[index].ToLower().Contains("Menus".ToLower()) || btnWidget.Name.Equals("PWLItem10") || btnWidget.Name.Equals("SPLItem5"))
                                    textFormats[index].Add(GetButtonTextFormat(btnWidget, directWriteFactory, TextAlignment.Center));
                                else
                                    textFormats[index].Add(GetButtonTextFormat(btnWidget, directWriteFactory, TextAlignment.Leading));
                            }
                            else
                                textFormats[index].Add(GetButtonTextFormat(btnWidget, directWriteFactory, TextAlignment.Center));
                        }
                        index += 1;
                    }
                    name = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            configNodes = null;
            return textFormats;
        }

        /// <summary>
        /// Gets the text format for the text in the CRG Section
        /// </summary>
        /// <param name="widgets">List of controls</param>
        /// <param name="directWriteFactory">SharpDX object</param>
        /// <returns>text Format</returns>
        public static TextFormat GetTextFormatCRG(List<Widget> widgets, SharpDX.DirectWrite.Factory directWriteFactory)
        {
            TextFormat textFormatCRG = null;
            try
            {
                var btnCRGWidget = widgets.SelectMany(w => w.Children).FirstOrDefault(w => w.Name.Equals("BtnCRG"));
                if (btnCRGWidget != null)
                    return GetButtonTextFormat(btnCRGWidget, directWriteFactory, TextAlignment.Center);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                textFormatCRG = GetDefaultButtonTextFormat(directWriteFactory, TextAlignment.Center);
            }
            return textFormatCRG;
        }

        private static void CalculateBottomLeftFocalPoint(RawRectangleF rawRectangleF, float height, float distance, out float xPoint, out float yPoint)
        {
            xPoint = rawRectangleF.Left + distance;
            yPoint = rawRectangleF.Bottom - distance - height;
        }

        private static void CalculateBottomRightFocalPoint(RawRectangleF rawRectangleF, float width, float height, float distance, out float xPoint, out float yPoint)
        {
            xPoint = rawRectangleF.Right - distance - width;
            yPoint = rawRectangleF.Bottom - distance - height;
        }

        private static void CalculateCenterFocalPoint(RawRectangleF rawRectangleF, float maxWidth, float width, float maxHeight, float height, float distance, out float xPoint, out float yPoint)
        {
            xPoint = (maxWidth - width) / 2 + rawRectangleF.Left;
            yPoint = (maxHeight - height) / 2 + rawRectangleF.Top;
        }

        private static void CalculateTopLeftFocalPoint(RawRectangleF rawRectangleF, float distance, out float xPoint, out float yPoint)
        {
            xPoint = rawRectangleF.Left + distance;
            yPoint = rawRectangleF.Top + distance;
        }

        private static void CalculateTopRightFocalPoint(RawRectangleF rawRectangleF, float width, float distance, out float xPoint, out float yPoint)
        {
            xPoint = rawRectangleF.Right - distance - width;
            yPoint = rawRectangleF.Top + distance;
        }

        /// <summary>
        /// Converts Color attibutes to RawColor4
        /// </summary>
        /// <param name="colorScheme"></param>
        /// <returns></returns>
        private static RawColor4 ConvertToRawColor4(ColorScheme colorScheme)
        {
            return new RawColor4(
                (float)colorScheme.HighlightSelectedForeground.R / 255,
                (float)colorScheme.HighlightSelectedForeground.G / 255,
                (float)colorScheme.HighlightSelectedForeground.B / 255,
                1.0f
                );
        }

        /// <summary>
        /// Convert round rectangles into sharp edges rectangles
        /// </summary>
        /// <param name="roundedRectangles"></param>
        /// <returns></returns>
        private static List<RawRectangleF> ConvertToRawRectangles(List<RoundedRectangle> roundedRectangles)
        {
            List<RawRectangleF> rawRectangles = new();
            foreach (RoundedRectangle roundedRect in roundedRectangles)
            {
                RawRectangleF rawRect = new(roundedRect.Rect.Left, roundedRect.Rect.Top, roundedRect.Rect.Right, roundedRect.Rect.Bottom);
                rawRectangles.Add(rawRect);
            }
            return rawRectangles;
        }

        /// <summary>
        /// Gets the rectangle that bounds all the controls
        /// </summary>
        /// <param name="rawRoundRectangleList"></param>
        /// <returns></returns>
        private static RawRectangleF GetBoundingRectangle(List<RoundedRectangle> rawRoundRectangleList)
        {
            List<RawRectangleF> rawRectangleList = ConvertToRawRectangles(rawRoundRectangleList);
            float minleft = rawRectangleList.Min(rect => rect.Left);
            float mintop = rawRectangleList.Min(rect => rect.Top);
            float maxright = rawRectangleList.Max(rect => rect.Right);
            float maxbottom = rawRectangleList.Max(rect => rect.Bottom);
            return new RawRectangleF(minleft, mintop, maxright, maxbottom);
        }

        /// <summary>
        /// Gets the Text Format in SharpDX format
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="directWriteFactory"></param>
        /// <param name="textAlignment"></param>
        /// <returns></returns>
        private static TextFormat GetButtonTextFormat(Widget widget, SharpDX.DirectWrite.Factory directWriteFactory, TextAlignment textAlignment)
        {
            IButtonWidget widbtn = widget as IButtonWidget;
            var fontData = widbtn.GetWidgetAttribute();
            var buttonTextFormat = new TextFormat(directWriteFactory,
                                            fontData.FontName,
                                            GetFontWeight(fontData.FontBold),
                                            GetFontStyle(fontData.FontItalic),
                                            (float)fontData.FontSize)
            {
                TextAlignment = textAlignment
            };
            return buttonTextFormat;
        }

        /// <summary>
        /// Gets a default TextFormat
        /// </summary>
        /// <param name="directWriteFactory"></param>
        /// <param name="textAlignment"></param>
        /// <returns></returns>
        private static TextFormat GetDefaultButtonTextFormat(SharpDX.DirectWrite.Factory directWriteFactory, TextAlignment textAlignment)
        {
            TextFormat buttonTextFormat = new(directWriteFactory,
                                            "montserrat",
                                            FontWeight.Normal,
                                            SharpDX.DirectWrite.FontStyle.Normal,
                                            28)
            {
                TextAlignment = textAlignment
            };
            return buttonTextFormat;
        }

        /// <summary>
        /// Calculates the focal area within a rectangle based on specified regions.
        /// </summary>
        /// <param name="rawRectangleF"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private static RawRectangleF GetFocalAreaFromRectangle(RawRectangleF rawRectangleF, float width, float height, float distanceFromEdge, BCIFocalAreaRegion bCIFocalAreaRegions)
        {
            float maxWidth = rawRectangleF.Right - rawRectangleF.Left;
            float maxHeight = rawRectangleF.Bottom - rawRectangleF.Top;
            float xPoint = 0;
            float yPoint = 0;
            switch (bCIFocalAreaRegions)
            {
                case BCIFocalAreaRegion.TopLeft:
                    CalculateTopLeftFocalPoint(rawRectangleF, distanceFromEdge, out xPoint, out yPoint);
                    break;

                case BCIFocalAreaRegion.TopRight:
                    CalculateTopRightFocalPoint(rawRectangleF, width, distanceFromEdge, out xPoint, out yPoint);
                    break;

                case BCIFocalAreaRegion.Center:
                    CalculateCenterFocalPoint(rawRectangleF, maxWidth, width, maxHeight, height, distanceFromEdge, out xPoint, out yPoint);
                    break;

                case BCIFocalAreaRegion.BottomLeft:
                    CalculateBottomLeftFocalPoint(rawRectangleF, height, distanceFromEdge, out xPoint, out yPoint);
                    break;

                case BCIFocalAreaRegion.BottomRight:
                    CalculateBottomRightFocalPoint(rawRectangleF, width, height, distanceFromEdge, out xPoint, out yPoint);
                    break;
            }
            return new RawRectangleF(xPoint, yPoint, xPoint + width, yPoint + height);
        }

        /// <summary>
        /// Gets the nodes from xml document
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <param name="nodeSection"></param>
        /// <returns></returns>
        private static XmlNodeList GetNodesList(string configFilePath, string nodeSection)
        {
            var doc = new XmlDocument();
            doc.Load(configFilePath);
            return doc.SelectNodes(nodeSection);
        }

        /// <summary>
        /// Get the rectangle based on the position of the button in the Form
        /// </summary>
        /// <param name="scannerButton"></param>
        /// <returns></returns>
        private static RawRectangleF GetRectangleFromLocationOnForm(ScannerButtonControl scannerButton)
        {
            Point locationOnForm = new();
            if (scannerButton.InvokeRequired)
                scannerButton.Invoke(new Action(() => { locationOnForm = scannerButton.FindForm().PointToClient(scannerButton.Parent.PointToScreen(scannerButton.Location)); }));
            else
                locationOnForm = scannerButton.FindForm().PointToClient(scannerButton.Parent.PointToScreen(scannerButton.Location));
            return new RawRectangleF(
                locationOnForm.X,
                locationOnForm.Y,
                locationOnForm.X + scannerButton.Width,
                locationOnForm.Y + scannerButton.Height);
        }

        /// <summary>
        /// Get the rectangle from the name of the control
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static RawRectangleF GetRectangleFromName(List<Control> controls, string name)
        {
            RawRectangleF rectExtraButton = new();
            var btn = AnimationManagerUtils.GetControl(controls, name, 1, false);
            if (btn != null)
                rectExtraButton = GetRectangleFromLocationOnForm(btn);
            return rectExtraButton;
        }
    }
}