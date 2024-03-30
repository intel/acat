////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using SharpDX.Direct2D1;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System;
using static ACAT.Extensions.BCI.Common.AnimationSharp.AnimationSharpManagerV2;

namespace ACAT.Extensions.BCI.Common.AnimationSharp
{
    /// <summary>
    /// Helper functions to get objects and variables used by BCI 
    /// </summary>
    public class AnimationManagerUtils
    {
        /// <summary>
        /// String messages for BCI
        /// </summary>
        public const string MessageRecalibration = " Calibration request, wait while the process starts";

        public const string StatusMessageAnalyzingCalibrationData = "     Analyzing calibration data. Please wait ";
        /// <summary>
        /// List of the current letter probabilities 
        /// </summary>
        private static List<KeyValuePair<string, double>> _lettersProbs = new List<KeyValuePair<string, double>>();

        /// <summary>
        /// List of the previous letter probabilities 
        /// </summary>
        private static List<KeyValuePair<string, double>> _prevLettersProbs = new List<KeyValuePair<string, double>>();

        /// <summary>
        /// List of the previous letter probabilities 
        /// </summary>
        private static List<KeyValuePair<string, double>> _prevWordsProbs = new List<KeyValuePair<string, double>>();

        /// <summary>
        /// List of the current letter probabilities 
        /// </summary>
        private static Dictionary<string, double> _sentenceProbs = new Dictionary<string, double>();

        /// <summary>
        /// Parameters used by BCI
        /// </summary>
        private static Dictionary<string, int> _UIBCIparameters = new Dictionary<string, int>();
        /// <summary>
        /// List of the current letter probabilities 
        /// </summary>
        private static List<KeyValuePair<string, double>> _wordsProbs = new List<KeyValuePair<string, double>>();
        /// <summary>
        /// Signal status
        /// </summary>
        public static SignalStatus StatusSignal { get; set; }

        /// <summary>
        /// Gets the text from all the controls in the form
        /// </summary>
        /// <param name="controls">List of controls</param>
        /// <param name="configFilePath">Path to the config file</param>
        /// <param name="amountBoxes">Amount of Boxes in main UI</param>
        /// <returns>List with the text from each button</returns>
        public static List<string>[] ExtractButtonText(List<Control> controls, string configFilePath, int amountBoxes)
        {
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATLayoutLayouts);
            int index = 0;
            string xmlSection = XmlSectionName.KeyboardBoxMapping.ToLower();
            List<string>[] btnStringsAll = Enumerable.Range(0, amountBoxes).Select(_ => new List<string>()).ToArray();
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    string sectionName = XmlUtils.GetXMLAttrString(node, "name", null).ToLower();
                    if (sectionName.Equals(xmlSection))
                    {
                        foreach (XmlNode subnode in node)
                        {
                            string buttonName = XmlUtils.GetXMLAttrString(subnode, "name", null);
                            int tag = Int32.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            ScannerButtonControl btn = GetControl(controls, buttonName, tag);
                            if (btn != null)
                                btnStringsAll[index].Add(btn.Text);
                        }
                        index += 1;
                    }
                    sectionName = null;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            configNodes = null;
            return btnStringsAll;
        }

        /// <summary>
        /// Gets the amount of possible boxes inside a user control
        /// </summary>
        /// <param name="configFilePath">Path to the config file</param>
        /// <returns></returns>
        public static int GetAmountBoxes(string configFilePath)
        {
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATLayoutLayouts);
            string xmlSection = XmlSectionName.KeyboardBoxMapping.ToLower();
            int amountBoxes = configNodes.Cast<XmlNode>().Count(node => XmlUtils.GetXMLAttrString(node, "name", null)?.ToLower() == xmlSection.ToLower());
            configNodes = null;
            return amountBoxes;
        }
        /// <summary>
        /// Gets the widgets for each box
        /// </summary>
        /// <param name="widgets">List of controls</param>
        /// <param name="configFilePath">Path to the config file</param>
        /// <param name="amountBoxes">Amount of Boxes in main UI</param>
        /// <returns>List with the text from each button</returns>
        public static List<Widget>[] GetBoxWidgets(List<Widget> widgets, string configFilePath, int amountBoxes)
        {
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATLayoutLayouts);
            int index = 0;
            string xmlSection = XmlSectionName.KeyboardBoxMapping.ToLower();
            List<Widget>[] widgetsBox = Enumerable.Range(0, amountBoxes).Select(_ => new List<Widget>()).ToArray();
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
                            int tag = Int32.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var widgetButton = widgets.SelectMany(w => w.Children).FirstOrDefault(w => w.Name.Equals(name2) && w.UIControl.Tag != null && Int32.Parse(w.UIControl.Tag.ToString()) == tag);
                            widgetsBox[index].Add(widgetButton);
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
            return widgetsBox;
        }
        /// <summary>
        /// Gets the list of all the widgets of each box for each user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="widgetsData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <returns></returns>
        public static List<Widget>[] GetBoxWidgetsList(KeyValuePair<List<Control>, string> boxesData, List<Widget> widgetsData, int totalAmountOfBoxes)
        {
            List<Widget>[] widgets = new List<Widget>[totalAmountOfBoxes];
            int indexBox = 0;
            try
            {
                var tempwidgets = AnimationManagerUtils.GetBoxWidgets(widgetsData, boxesData.Value, totalAmountOfBoxes);
                for (int boxIndex = 0; boxIndex < totalAmountOfBoxes; boxIndex++)
                {
                    widgets[indexBox] = tempwidgets[boxIndex];
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error in GetBoxWidgetsList: " + ex.Message);
                return new List<Widget>[totalAmountOfBoxes];
            }
            return widgets;
        }
        /// <summary>
        /// Gets the list of all the data of each button for each box within the user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <param name="renderTarget"></param>
        /// <returns></returns>
        public static List<ButtonsData>[] GetButtonDataList(KeyValuePair<List<Control>, string> boxesData, int totalAmountOfBoxes, RenderTarget renderTarget)
        {
            List<ButtonsData>[] buttonDataList = new List<ButtonsData>[totalAmountOfBoxes];
            int indexBox = 0;
            try
            {
                List<AnimationSharpManagerV2.ButtonsData>[] tempmatrixButtonListAll = AnimationManagerUtils.GetMatrixButtons(boxesData.Key, renderTarget, boxesData.Value, totalAmountOfBoxes);
                for (int boxIndex = 0; boxIndex < totalAmountOfBoxes; boxIndex++)
                {
                    buttonDataList[indexBox] = tempmatrixButtonListAll[boxIndex];
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error in GetButtonDataList: " + ex.Message);
                return new List<ButtonsData>[totalAmountOfBoxes];
            }
            return buttonDataList;
        }

        /// <summary>
        /// Gets the index from the sequences based on the tag ID
        /// </summary>
        /// <param name="controls">Controls of the probs bars</param>
        /// <param name="amountBoxes">amount of boxes in the layout</param>
        /// <param name="sequences">flashing seuqnces containing the tagID</param>
        /// <returns>List with index of controls referecnce</returns>
        public static List<int[]>[] GetButtonsIndex(List<ScannerButtonControl>[] controls, List<int[]>[] sequences, int amountBoxes)
        {
            int indexkeyboard = 0;
            int indexarray = 0;
            int tempindex = 0;
            List<int> seq = new List<int>();
            List<int[]>[] flashingSeqAll = Enumerable.Range(0, amountBoxes).Select(_ => new List<int[]>()).ToArray();
            if (indexkeyboard < amountBoxes)
            {
                for (int i = 0; i < amountBoxes; i++)
                {
                    foreach (int[] array in sequences[i])
                    {
                        foreach (int tagID in sequences[i][indexarray])
                        {
                            foreach (ScannerButtonControl btn in controls[i])
                            {
                                if (tagID == Int32.Parse(btn.Tag.ToString()))
                                {
                                    seq.Add(tempindex);
                                    break;
                                }
                                else
                                {
                                    tempindex += 1;
                                }
                            }
                            tempindex = 0;
                        }
                        indexarray += 1;
                        flashingSeqAll[i].Add(seq.ToArray());
                        seq.Clear();
                    }
                    indexarray = 0;
                }
            }
            return flashingSeqAll;
        }
        /// <summary>
        /// Gets the list of all the offsets of each button for each user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="widgetsData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <returns></returns>
        public static List<int>[] GetButtonsOffsetList(KeyValuePair<List<Control>, string> boxesData, List<Widget> widgetsData, int totalAmountOfBoxes)
        {
            List<int>[] offsetStrings = new List<int>[totalAmountOfBoxes];
            int indexBox = 0;
            try
            {
                var tempoffsetStrings = AnimationManagerUtils.GetOffset(widgetsData, boxesData.Value, totalAmountOfBoxes, boxesData.Key);
                for (int boxIndex = 0; boxIndex < totalAmountOfBoxes; boxIndex++)
                {
                    offsetStrings[indexBox] = tempoffsetStrings[boxIndex];
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error in GetButtonsOffsetList: " + ex.Message);
                return new List<int>[totalAmountOfBoxes];
            }
            return offsetStrings;
        }

        /// <summary>
        /// Gets the list of all the string of each button for each box within the user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <returns></returns>
        public static List<string>[] GetButtonsStringsList(Dictionary<List<Control>, string> boxesData, int totalAmountOfBoxes)
        {
            List<string>[] buttonsStringsList = new List<string>[totalAmountOfBoxes];
            int amountBoxesPerUserControl;
            int indexBox = 0;
            try
            {
                foreach (var boxData in boxesData)
                {
                    amountBoxesPerUserControl = AnimationManagerUtils.GetAmountBoxes(boxData.Value);
                    List<string>[] tempbtnsStringsAll = AnimationManagerUtils.ExtractButtonText(boxData.Key, boxData.Value, amountBoxesPerUserControl);
                    for (int boxIndex = 0; boxIndex < amountBoxesPerUserControl; boxIndex++)
                    {
                        buttonsStringsList[indexBox] = tempbtnsStringsAll[boxIndex];
                        indexBox += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error in GetButtonsStringsList: " + ex.Message);
                return new List<string>[totalAmountOfBoxes];
            }
            return buttonsStringsList;
        }

        /// <summary>
        /// Get the desired button control 
        /// </summary>
        /// <param name="controls">List with all the controls</param>
        /// <param name="name">Name of the button to look for</param>
        /// <param name="tag">ID of the button</param>
        /// <param name="tagValidation">Use the TagID as validation</param>
        /// <returns>ScannerButtonControl</returns>
        public static ScannerButtonControl GetControl(List<Control> controls, string name, int tag, bool tagValidation = true)
        {
            if (tagValidation)
                return (ScannerButtonControl)controls.FirstOrDefault(control => control.Name.Equals(name) && control.Tag != null && Int32.TryParse(control.Tag.ToString(), out int controlTag) && controlTag == tag);
            else
                return (ScannerButtonControl)controls.FirstOrDefault(control => control.Name.Equals(name));
        }

        /// <summary>
        /// Get the desired button control 
        /// </summary>
        /// <param name="controls">List with all the controls</param>
        /// <param name="name">Name of the button to look for</param>
        /// <param name="tag">ID of the button</param>
        /// <param name="tagValidation">Use the TagID as validation</param>
        /// <returns>ScannerButtonControl</returns>
        public static ScannerButtonControl GetControl2(List<Widget> controls, string name, int tag, bool tagValidation = true)
        {
            if(tagValidation)
                return (ScannerButtonControl)controls.SelectMany(w => w.Children).FirstOrDefault(w => w.Name.Equals(name) && w.UIControl.Tag != null && Int32.Parse(w.UIControl.Tag.ToString()) == tag).UIControl;
            else
                return (ScannerButtonControl)controls.SelectMany(w => w.Children).FirstOrDefault(w => w.Name.Equals(name)).UIControl;
        }

        /// <summary>
        /// Get the control based on the ID assigned to the button
        /// </summary>
        /// <param name="tag">ID</param>
        /// <param name="controlsBtns">List of buttons</param>
        /// <param name="activeKeyboard">Active keyboard layout</param>
        /// <returns></returns>
        public static ScannerButtonControl GetControlFromID(int tag, List<ScannerButtonControl>[] controlsBtns, int activeKeyboard)
        {
            ScannerButtonControl tempButton = null;
            foreach (var button in controlsBtns[activeKeyboard])
            {
                if (Int32.Parse(button.Tag.ToString()) == tag)
                {
                    tempButton = button;
                }
            }
            return tempButton;
        }
        /// <summary>
        /// Gets the list of all the data of each button for each box within the user control
        /// </summary>
        /// <param name="boxesData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <param name="flashingSequenceIDList"></param>
        /// <param name="flashingSequenceList"></param>
        /// <returns></returns>
        public static List<ScannerButtonControl>[] GetControlsBtns(KeyValuePair<List<Control>, string> boxesData, int totalAmountOfBoxes, out List<int[]>[] flashingSequenceIDList, out List<int[]>[] flashingSequenceList)
        {
            List<ScannerButtonControl>[] buttonDataList = new List<ScannerButtonControl>[totalAmountOfBoxes];
            flashingSequenceIDList = new List<int[]>[totalAmountOfBoxes];
            flashingSequenceList = new List<int[]>[totalAmountOfBoxes];
            int indexBox = 0;
            try
            {
                List<ScannerButtonControl>[] tempcontrolsBtns = AnimationManagerUtils.GetControlsButtons(boxesData.Key, boxesData.Value, totalAmountOfBoxes);
                List<int[]>[] tempflashingSeqAllID = AnimationManagerUtils.GetSequences(boxesData.Key, boxesData.Value, totalAmountOfBoxes);
                List<int[]>[] tempflashingSeqAll = AnimationManagerUtils.GetButtonsIndex(tempcontrolsBtns, tempflashingSeqAllID, totalAmountOfBoxes);//
                for (int boxIndex = 0; boxIndex < totalAmountOfBoxes; boxIndex++)
                {
                    buttonDataList[indexBox] = tempcontrolsBtns[boxIndex];
                    flashingSequenceIDList[indexBox] = tempflashingSeqAllID[boxIndex];
                    flashingSequenceList[indexBox] = tempflashingSeqAll[boxIndex];
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error in GetControlsBtns: " + ex.Message);
                flashingSequenceIDList = new List<int[]>[totalAmountOfBoxes];
                flashingSequenceList = new List<int[]>[totalAmountOfBoxes];
                return new List<ScannerButtonControl>[totalAmountOfBoxes];
            }
            return buttonDataList;
        }

        /// <summary>
        /// Gets the controls for a specific box layout
        /// </summary>
        /// <param name="controls">Controls of the form</param>
        /// <param name="amountBoxes">amount of boxes in the layout</param>
        /// <param name="configFilePath">path for the config file</param>
        /// <returns></returns>
        public static List<ScannerButtonControl>[] GetControlsButtons(List<Control> controls, string configFilePath, int amountBoxes)
        {
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATLayoutLayouts);
            int index = 0;
            string xmlSection = XmlSectionName.KeyboardBoxMapping.ToLower();
            List<ScannerButtonControl>[] ctrlBtnsAll = Enumerable.Range(0, amountBoxes).Select(_ => new List<ScannerButtonControl>()).ToArray();
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
                            int tag = Int32.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var btn = GetControl(controls, name2, tag);
                            if (btn != null)
                                ctrlBtnsAll[index].Add(btn);
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
            return ctrlBtnsAll;
        }
        /// <summary>
        /// Gets the list of all the sequences of each button
        /// </summary>
        /// <param name="widgetsData"></param>
        /// <param name="totalAmountOfBoxes"></param>
        /// <param name="flashingSequenceBoxList"></param>
        /// <returns></returns>
        public static List<int>[] GetFlashingSequenceIDBoxList(List<Widget>[] widgetsData, int totalAmountOfBoxes, out List<int>[] flashingSequenceBoxList)
        {
            List<int>[] flashingSequenceIDBoxList = new List<int>[totalAmountOfBoxes];
            flashingSequenceIDBoxList = Enumerable.Range(0, totalAmountOfBoxes).Select(_ => new List<int>()).ToArray();
            flashingSequenceBoxList = Enumerable.Range(0, totalAmountOfBoxes).Select(_ => new List<int>()).ToArray();
            int indexBox = 0;
            try
            {
                foreach (var widgetArray in widgetsData)
                {
                    foreach (var widget in widgetArray)
                    {
                        if (widget.UIControl.Tag != null)
                        {
                            var tagId = Int32.Parse(widget.UIControl.Tag.ToString());
                            flashingSequenceIDBoxList[indexBox].Add(tagId);
                            flashingSequenceBoxList[indexBox].Add(tagId - 1);
                        }
                    }
                    indexBox += 1;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error in GetFlashingSequenceIDBoxList: " + ex.Message);
                return new List<int>[totalAmountOfBoxes];
            }
            return flashingSequenceIDBoxList;
        }

        /// <summary>
        /// Obtain the next letter probabilities given the controls (Buttons)
        /// </summary>
        /// <param name="controls">Controls from the form</param>
        /// <param name="get">Obtain whether if the same probabilities or not</param>
        /// <returns></returns>
        public static Dictionary<int, double> GetLettersProbs(List<ScannerButtonControl> controls, bool get = false)
        {
            Dictionary<int, double> nextProbs = new Dictionary<int, double>();
            try
            {
                if (!_prevLettersProbs.Equals(_lettersProbs) || get == true)
                {
                    _prevLettersProbs = _lettersProbs;
                    foreach (ScannerButtonControl control in controls)
                    {
                        if (!control.Name.Contains("PWLItem") && control.Text.Length == 1 && ((control.Text[0] >= 'a' && control.Text[0] <= 'z') || control.Text[0] == ' ') || ((control.Text[0] >= 'A' && control.Text[0] <= 'Z') || control.Text[0] == ' '))
                        {
                            foreach (var probs in _lettersProbs)
                            {
                                string s = probs.Key.Trim('\'');
                                if (control.Text.ToLower().Equals(s))
                                {
                                    nextProbs.Add((int)control.Tag, probs.Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception es)
            {
                Log.Debug("Exception geting values probs " + es);
            }
            return nextProbs;
        }

        /// <summary>
        /// Obtain the next letter probabilities given the widgets
        /// </summary>
        /// <param name="controls">Controls from the form</param>
        /// <param name="get">Obtain whether if the same probabilities or not</param>
        /// <returns></returns>
        public static Dictionary<int, double> GetLettersProbs(List<Widget> controls, bool get = false)
        {
            Dictionary<int, double> nextProbs = new Dictionary<int, double>();
            try
            {
                if (!_prevLettersProbs.Equals(_lettersProbs) || get == true)
                {
                    _prevLettersProbs = _lettersProbs;
                    foreach (Widget widget in controls)
                    {
                        if(widget.GetText().Length == 1)
                        {
                            if (!widget.Name.Contains("PWLItem") && ((widget.GetText()[0] >= 'a' && widget.GetText()[0] <= 'z') || widget.GetText()[0] == ' ') || ((widget.GetText()[0] >= 'A' && widget.GetText()[0] <= 'Z') || widget.GetText()[0] == ' '))
                            {
                                IButtonWidget widbtn = widget as IButtonWidget;
                                var fontData = widbtn.GetWidgetAttribute();
                                if (fontData.FontName.Equals("acat font 1"))
                                {
                                    foreach (var probs in _lettersProbs)
                                    {
                                        string s = probs.Key.Trim('\'');
                                        // Validation for the "space" charcater for "acat font 1" -> m = space
                                        if (s.Contains(" ") && widget.GetText().ToLower().Equals("m"))
                                            nextProbs.Add(Int32.Parse(widget.UIControl.Tag.ToString()), probs.Value);
                                    }
                                }
                                else
                                {
                                    foreach (var probs in _lettersProbs)
                                    {
                                        string s = probs.Key.Trim('\'');
                                        if (widget.GetText().ToLower().Equals(s))
                                        {
                                            nextProbs.Add(Int32.Parse(widget.UIControl.Tag.ToString()), probs.Value);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Exception in getLettersProbs: " + ex.Message.ToString());
                return nextProbs = new Dictionary<int, double>();
            }
            return nextProbs;
        }

        /// <summary>
        /// Gets the main color from the Theme xml file
        /// </summary>
        /// <param name="colorCodeRegion">name of the color</param>
        /// <returns></returns>
        public static ColorScheme GetMainColorScheme(string colorCodeRegion)
        {
            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(colorCodeRegion);
            return colorScheme;
        }

        /// <summary>
        /// Gets the data from each control into a matrix, returns list with controls info
        /// </summary>
        /// <param name="controls">Controls of the form</param>
        /// <param name="sharpDX_d2dRenderTarget"></param>
        /// <param name="configFilePath">path for the config file</param>
        /// <param name="amountBoxes">amount of boxes in the layout</param>
        /// <param name="margin"></param>
        /// <returns></returns>
        public static List<AnimationSharpManagerV2.ButtonsData>[] GetMatrixButtons(List<Control> controls, RenderTarget sharpDX_d2dRenderTarget, string configFilePath, int amountBoxes, int margin = 0)
        {
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATLayoutLayouts);
            string xmlSection = XmlSectionName.KeyboardBoxMapping.ToLower();
            int index = 0;
            List<AnimationSharpManagerV2.ButtonsData>[] matrixButtonList = Enumerable.Range(0, amountBoxes).Select(_ => new List<AnimationSharpManagerV2.ButtonsData>()).ToArray();
            AnimationSharpManagerV2.ButtonsData currMatrixButton = new AnimationSharpManagerV2.ButtonsData();
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
                            int tag = Int32.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            string borderColor = XmlUtils.GetXMLAttrString(subnode, "borderColor", "BCIColorCodedRegionDefault");
                            var btn = GetControl(controls, name2, tag);
                            if (btn != null)
                            {
                                currMatrixButton.id = Int32.Parse(btn.Tag.ToString());
                                currMatrixButton.text = btn.Text;
                                currMatrixButton.action = currMatrixButton.text;
                                currMatrixButton.name = btn.Name;
                                currMatrixButton.borderColor = new SolidColorBrush(sharpDX_d2dRenderTarget, SharpDXUtils.GetBorderColorScheme(borderColor));
                                matrixButtonList[index].Add(currMatrixButton);
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
            return matrixButtonList;
        }

        /// <summary>
        /// Gets the list offset for the text from all the controls in the form
        /// </summary>
        /// <param name="controls">List of controls</param>
        /// <param name="configFilePath">Path to the config file</param>
        /// <param name="amountBoxes">Amount of Boxes in main UI</param>
        /// <returns>List with the text from each button</returns>
        public static List<int>[] GetOffset(List<Widget> widgets, string configFilePath, int amountBoxes, List<Control> controls)
        {
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATLayoutLayouts);
            int index = 0;
            string xmlSection = XmlSectionName.KeyboardBoxMapping.ToLower();
            List<int>[] offsets = Enumerable.Range(0, amountBoxes).Select(_ => new List<int>()).ToArray();
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
                            int tag = Int32.Parse(XmlUtils.GetXMLAttrString(subnode, "tagID", null));
                            var btnWidget = widgets.SelectMany(w => w.Children).FirstOrDefault(w => w.Name.Equals(name2) && w.UIControl.Tag != null && Int32.Parse(w.UIControl.Tag.ToString()) == tag);
                            if (btnWidget != null)
                                offsets[index].Add(GetOffset(btnWidget));
                            else
                                offsets[index].Add(5);
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
            return offsets;
        }

        /// <summary>
        /// Gets the offset for the text used in the CRG Section
        /// </summary>
        /// <param name="widgets">List of controls</param>
        /// <returns>Offset for the CRG buttonn</returns>
        public static int GetOffsetCRG(List<Widget> widgets)
        {
            int offset = 0;
                var btnWidget = widgets.SelectMany(w => w.Children).FirstOrDefault(w => w.Name.Equals("BtnCRG"));
                if(btnWidget != null)
                    offset = GetOffset(btnWidget);
            return offset;
        }
        /// <summary>
        /// Gets a specific parameter
        /// </summary>
        /// <param name="parameter">Name of the parameter</param>
        /// <returns>Value of the requested parameter</returns>
        public static int GetParameter(string parameter)
        {
            int value = 0;
            try
            {
                if (_UIBCIparameters.TryGetValue(parameter, out value))
                    return value;
            }
            catch (Exception)
            {
                value = 0;
            }
            return value;
        }

        /// <summary>
        /// Defines the flashing sequences based on a xml file retunrns array of sequences
        /// </summary>
        /// <param name="controls">Controls of the form</param>
        /// <param name="configFilePath">path for the config file</param>
        /// <param name="amountBoxes">amount of boxes in the layout</param>
        /// <returns>Array of List of arrays</returns>
        public static List<int[]>[] GetSequences(List<Control> controls, string configFilePath, int amountBoxes)
        {
            int index = 0;
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATAnimationsAnimation);
            string xmlSection = XmlSectionName.KeyboardSequences.ToLower();
            List<int> seq = new List<int>();
            List<int[]>[] flashingSeqAll = Enumerable.Range(0, amountBoxes).Select(_ => new List<int[]>()).ToArray();
            try
            {
                foreach (XmlNode node in configNodes)
                {
                    string name = XmlUtils.GetXMLAttrString(node, "name", null).ToLower();
                    if (name.Equals(xmlSection))
                    {
                        foreach (XmlNode subnode in node)
                        {
                            string nameRowCol = XmlUtils.GetXMLAttrString(subnode, "name", null);
                            if (nameRowCol.ToLower().Equals("row") || nameRowCol.ToLower().Equals("column"))
                            {
                                foreach (XmlNode secNode in subnode)
                                {
                                    string name2 = XmlUtils.GetXMLAttrString(secNode, "name", null);
                                    int tag = Int32.Parse(XmlUtils.GetXMLAttrString(secNode, "tagID", null));
                                    var btn = GetControl(controls, name2, tag);
                                    if (btn != null && Int32.Parse(btn.Tag.ToString()) == tag && btn.Name.Equals(name2))
                                        seq.Add(tag);
                                }
                                flashingSeqAll[index].Add(seq.ToArray());
                                seq.Clear();
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
            return flashingSeqAll;
        }
        /// <summary>
        /// Gets type of keyboard from a especific box
        /// </summary>
        /// <param name="configFilePath">Path to the config file</param>
        /// <param name="amountBoxes">Amount of Boxes in main UI</param>
        /// <returns>Type of box for each layout</returns>
        public static string[] GetTypeOfBox(string configFilePath, int amountBoxes)
        {
            var configNodes = GetNodesList(configFilePath, XmlSectionName.ACATLayoutLayouts);
            string xmlSection = XmlSectionName.KeyboardType.ToLower();
            string[] type = new string[amountBoxes];
            int index = 0;
            foreach (XmlNode node in configNodes)
            {
                string name = XmlUtils.GetXMLAttrString(node, "name", "NA").ToLower();
                if (name.Equals(xmlSection))
                {
                    foreach (XmlNode subnode in node)
                    {
                        type[index] = XmlUtils.GetXMLAttrString(subnode, "name", "NA").ToLower();
                    }
                    index += 1;
                }
                name = null;
            }
            configNodes = null;
            return type;
        }

        /// <summary>
        /// Obtain the next words probabilities 
        /// </summary>
        /// <param name="controls">Controls from the form</param>
        /// <param name="get">Obtain whether if the same probabilities or not</param>
        /// <returns></returns>
        public static Dictionary<int, double> GetWordsProbs(List<ScannerButtonControl> controls, bool get = false)
        {
            Dictionary<int, double> nextProbs = new Dictionary<int, double>();
            try
            {
                if (!_prevWordsProbs.Equals(_wordsProbs) || get == true)
                {
                    _prevWordsProbs = _wordsProbs;
                    foreach (ScannerButtonControl control in controls)
                    {
                        if (control.Name.Contains("PWLItem") && control.Text.Length >= 1)
                        {
                            foreach (var probs in _wordsProbs)
                            {
                                string s = probs.Key.Trim('\'');
                                if (control.Text.ToLower().Equals(s))
                                {
                                    nextProbs.Add((int)control.Tag, probs.Value);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception es)
            {
                Log.Debug("Exception geting values probs " + es);
            }
            return nextProbs;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Init()
        {
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtNotifyNextLetterProbabilities += ActiveWordPredictor_EvtNotifyNextLetterProbabilities;
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtNotifyNextWordProbabilities += ActiveWordPredictor_EvtNotifyNextWordProbabilities;
        }

        /// <summary>
        /// Sets the default parameters
        /// </summary>
        public static void SetDefaultParameters()
        {
            _UIBCIparameters.Add("TypingTargetCount", 0);
            _UIBCIparameters.Add("TypingIterationsPerTarget", 1);
            _UIBCIparameters.Add("MinimumProgressBarsValue", 10);
            _UIBCIparameters.Add("cornerRadius", 6);
            _UIBCIparameters.Add("borderWidth", 1);
        }

        /// <summary>
        /// Refresh letters probabilities object
        /// </summary>
        /// <param name="letterList"></param>
        /// <param name="lettersChanged"></param>
        private static void ActiveWordPredictor_EvtNotifyNextLetterProbabilities(List<KeyValuePair<string, double>> letterList, bool lettersChanged)
        {
            _lettersProbs = letterList;
        }

        /// <summary>
        /// Refresh words probabilities object
        /// </summary>
        /// <param name="wordsProbs"></param>
        /// <param name="wordsChanged"></param>
        private static void ActiveWordPredictor_EvtNotifyNextWordProbabilities(List<KeyValuePair<string, double>> wordList, bool wordsChanged)
        {
            List<KeyValuePair<string, double>> tempwordsProbs = new List<KeyValuePair<string, double>>();
            IDictionary<string, Double> words = new Dictionary<string, Double>();
            try
            {
                foreach (var element in wordList)
                {
                    string newValue = element.Key.Trim(new char[] { (char)39 });
                    newValue = newValue.Replace("\"", "");
                    words.Add(newValue, element.Value);
                }
                tempwordsProbs = words.ToList();
                _wordsProbs = tempwordsProbs;
            }
            catch (Exception)
            {
                _wordsProbs = wordList;
            }
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
        /// Get the offset value from the button
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        private static int GetOffset(Widget widget)
        {
            IButtonWidget widbtn = widget as IButtonWidget;
            var fontData = widbtn.GetWidgetAttribute();
            // Set a dynamic offset of the letters if the buttons change size due to the resolution of the screen The value "1.333" is an aproximation of the value from Font units to Pixels units (6pt aprox 8px)
            return (((widget.Height) - ((int)(fontData.FontSize * 1.333))) / 2);
        }
        private static class XmlSectionName
        {
            public const string ACATAnimationsAnimation = "/ACAT/Animations/Animation";
            public const string ACATLayoutLayouts = "/ACAT/Layout/Layouts";
            public const string CalibrationMapping = "CalibrationMapping";
            public const string KeyboardBoxMapping = "KeyboardBoxMapping";
            public const string KeyboardSequences = "KeyboardSequences";
            public const string KeyboardType = "KeyboardType";
        }
    }
}
