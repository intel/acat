////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System.Text.Json;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using ACAT.ACATResources;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// Form that handles different calibraitons options to configure and initialize calibration sesion
    /// </summary>

    [DescriptorAttribute("5A13AD81-2943-4A11-885F-37D4C2F19918",
        "RemapCalibrationForm",
        "Application window used to display the remap of calibrations")]
    public partial class RemapCalibrationForm : Form
    {
        /// <summary>
        /// Main object of the actuator
        /// </summary>
        private IActuator _bciActuator = null;

        /// <summary>
        /// Object holding all the parameters of calibrations
        /// </summary>
        private BCIMapOptions _bCIMapOptions;

        /// <summary>
        /// Return value when the Form is closed
        /// </summary>
        private bool OptionResult;

        private Screen primaryScreen = Screen.PrimaryScreen;

        // TODO - Localize Me
        private String _htmlText = "<!DOCTYPE html>\r\n<html>\r\n  <head>\r\n  <style>\r\n    a:link{color: rgb(255, 170, 0);}\r\n  " +
                            "</style>\r\n  </head>\r\n  <body style=\"background-color:#232433;\">\r\n    " +
                            "<p style=\"font-family:'Montserrat Medium'; font-size:18px; color:white; text-align: center;\">\r\n" +
                            "Each calibration that you have completed is assigned to the specific section for which you have trained. " +
                            "Sometimes using the calibration from one section and applying it to a different section can help with accuracy. " +
                            "To change where a calibration is  applied. Use the interface below.<br/>" +
                            "For more information watch this <a href=\"$ASSETS_VIDEOS_DIR#ACATOverviewBCI.mp4\">video</a> or review the <a href=$ACAT_USER_GUIDE#BCICalibrationRemap>set up guide</a>" +
                            "</p>\r\n</body>\r\n</html>";


        /// <summary>
        /// Confirm Box with multiple results
        /// </summary>
        public RemapCalibrationForm()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;

            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            var html = _htmlText.Replace(CoreGlobals.MacroACATUserGuide, HtmlUtils.EncodeString(CoreGlobals.ACATUserGuideFileName));
            webBrowser.DocumentText = html;
        }

        public static bool ShowFormDialog(Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new RemapCalibrationForm();
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            confirmBox.TopMost = true;
            //To always display the form in the main screen
            confirmBox.StartPosition = FormStartPosition.Manual;
            confirmBox.Location = confirmBox.primaryScreen.WorkingArea.Location;
            confirmBox.ShowDialog(parent);
            bool retVal = confirmBox.OptionResult;
            if (parent != null && setTopMost)
            {
                parent.TopMost = true;
                confirmBox.TopMost = false;
            }
            confirmBox.TopMost = false;
            confirmBox.Dispose();
            return retVal;
        }

        /// <summary>
        /// Sets the items for each of the combo box
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="bCIClassifierInfos"></param>
        private void AddItemsToComboBox(ComboBox comboBox, List<BCIClassifierInfo> bCIClassifierInfos)
        {
            try
            {
                foreach (var classifier in bCIClassifierInfos)
                {
                    string auc = " - ";
                    if (classifier.Auc > 0)
                        auc = (classifier.Auc * 100).ToString();
                    comboBox.Items.Add(classifier.ClassifierUsed.ToString() + " (" + auc + ")");
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Error AddItemsToComboBox: " + ex.Message);
            }
        }

        /// <summary>
        /// Actuator event handler
        /// </summary>
        /// <param name="opcode"></param>
        /// <param name="response"></param>
        private void BciActuator_EvtIoctlResponse(int opcode, string response)
        {
            switch (opcode)
            {
                case (int)OpCodes.SendMapOptions:
                    _bCIMapOptions = JsonSerializer.Deserialize<BCIMapOptions>(response);
                    ProcessMapOptionsAnswer();
                    break;
            }
        }

        private void ButtonCancel_Click_1(object sender, EventArgs e)
        {
            OptionResult = false;
            Close();
        }

        private void ButtonDone_Click(object sender, EventArgs e)
        {
            var strBCICalibrationUpdatedMappings = JsonSerializer.Serialize(GetMappingsValues());
            _bciActuator?.IoctlRequest((int)OpCodes.SendUpdatedMappings, strBCICalibrationUpdatedMappings);
            OptionResult = CheckIfComboBoxValuesChanged();
            Log.Debug("BCI LOG | Mappings change: " + OptionResult);
            Close();
        }

        private void ButtonRestoreDefaults_Click(object sender, EventArgs e)
        {
            SetComboBoxDefaultItem(comboBoxBox);
            SetComboBoxDefaultItem(comboBoxSentence);
            SetComboBoxDefaultItem(comboBoxKeyboardL);
            SetComboBoxDefaultItem(comboBoxWord);
            SetComboBoxDefaultItem(comboBoxKeyboardR);
        }

        /// <summary>
        /// Checks if a value from the combo box changed
        /// </summary>
        /// <returns></returns>
        private bool CheckIfComboBoxValuesChanged()
        {
            if (comboBoxBox.Items.Count > 0 && comboBoxBox.SelectedIndex > 0)
                return true;
            if (comboBoxSentence.Items.Count > 0 && comboBoxSentence.SelectedIndex > 0)
                return true;
            if (comboBoxKeyboardL.Items.Count > 0 && comboBoxKeyboardL.SelectedIndex > 0)
                return true;
            if (comboBoxWord.Items.Count > 0 && comboBoxWord.SelectedIndex > 0)
                return true;
            if (comboBoxKeyboardR.Items.Count > 0 && comboBoxKeyboardR.SelectedIndex > 0)
                return true;
            return false;
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            _bciActuator = Context.AppActuatorManager.GetActuator(new Guid("77809D19-F450-4D36-A633-D818400B3D9A"));
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse += BciActuator_EvtIoctlResponse;
            }
            var strBciModeParams = JsonSerializer.Serialize(new BCIUserInputParameters());
            _bciActuator?.IoctlRequest((int)OpCodes.RequestMapOptions, strBciModeParams);
        }

        private void ConfirmBoxCalibrationModes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_bciActuator != null)
            {
                _bciActuator.EvtIoctlResponse -= BciActuator_EvtIoctlResponse;
            }
        }

        /// <summary>
        /// Save the values selected from the combo boxes
        /// </summary>
        private BCICalibrationUpdatedMappings GetMappingsValues()
        {
            BCICalibrationUpdatedMappings bCICalibrationUpdatedMappings = new BCICalibrationUpdatedMappings();
            try
            {
                bCICalibrationUpdatedMappings.DictUpdatedMappings.Add(BCIScanSections.Box, GetScanningSection(comboBoxBox.SelectedItem.ToString()));
                bCICalibrationUpdatedMappings.DictUpdatedMappings.Add(BCIScanSections.Sentence, GetScanningSection(comboBoxSentence.SelectedItem.ToString()));
                bCICalibrationUpdatedMappings.DictUpdatedMappings.Add(BCIScanSections.KeyboardL, GetScanningSection(comboBoxKeyboardL.SelectedItem.ToString()));
                bCICalibrationUpdatedMappings.DictUpdatedMappings.Add(BCIScanSections.Word, GetScanningSection(comboBoxWord.SelectedItem.ToString()));
                bCICalibrationUpdatedMappings.DictUpdatedMappings.Add(BCIScanSections.KeyboardR, GetScanningSection(comboBoxKeyboardR.SelectedItem.ToString()));
            }
            catch (Exception ex)
            {
                Log.Debug("Error SaveMappingsValues: " + ex.Message);
            }
            return bCICalibrationUpdatedMappings;
        }

        /// <summary>
        /// Gets the Section based on the value selected in the combo box
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private BCIScanSections GetScanningSection(string value)
        {
            BCIScanSections bCIScanSections = BCIScanSections.None;
            switch (value)
            {
                case var _ when value.Contains("Box"):
                    bCIScanSections = BCIScanSections.Box;
                    break;

                case var _ when value.Contains("Sentence"):
                    bCIScanSections = BCIScanSections.Sentence;
                    break;

                case var _ when value.Contains("KeyboardL"):
                    bCIScanSections = BCIScanSections.KeyboardL;
                    break;

                case var _ when value.Contains("Word"):
                    bCIScanSections = BCIScanSections.Word;
                    break;

                case var _ when value.Contains("KeyboardR"):
                    bCIScanSections = BCIScanSections.KeyboardR;
                    break;
            }
            return bCIScanSections;
        }

        /// <summary>
        /// Proccess the actuator result
        /// </summary>
        private void ProcessMapOptionsAnswer()
        {
            try
            {
                foreach (var mapping in _bCIMapOptions.AllowedMappingsDict)
                {
                    switch (mapping.Key)
                    {
                        case BCIScanSections.Box:
                            AddItemsToComboBox(comboBoxBox, mapping.Value);
                            SetDefaultItemInComboBox(comboBoxBox, BCIScanSections.Box);
                            break;

                        case BCIScanSections.Sentence:
                            AddItemsToComboBox(comboBoxSentence, mapping.Value);
                            SetDefaultItemInComboBox(comboBoxSentence, BCIScanSections.Sentence);
                            break;

                        case BCIScanSections.KeyboardL:
                            AddItemsToComboBox(comboBoxKeyboardL, mapping.Value);
                            SetDefaultItemInComboBox(comboBoxKeyboardL, BCIScanSections.KeyboardL);
                            break;

                        case BCIScanSections.Word:
                            AddItemsToComboBox(comboBoxWord, mapping.Value);
                            SetDefaultItemInComboBox(comboBoxWord, BCIScanSections.Word);
                            break;

                        case BCIScanSections.KeyboardR:
                            AddItemsToComboBox(comboBoxKeyboardR, mapping.Value);
                            SetDefaultItemInComboBox(comboBoxKeyboardR, BCIScanSections.KeyboardR);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error ProcessMapOptionsAnswer: " + ex.Message);
            }
        }

        /// <summary>
        /// Sets the default value for the combo box
        /// </summary>
        /// <param name="comboBox"></param>
        private void SetComboBoxDefaultItem(ComboBox comboBox)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Set the default value assigned by the actuator
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="bCIScanSection"></param>
        private void SetDefaultItemInComboBox(ComboBox comboBox, BCIScanSections bCIScanSection)
        {
            try
            {
                if(comboBox.Items.Count > 0)
                {
                    BCIScanSections localbCIScanSection = BCIScanSections.None;
                    if (_bCIMapOptions.CurrentMappingsDict.ContainsKey(bCIScanSection))
                        localbCIScanSection = _bCIMapOptions.CurrentMappingsDict[bCIScanSection];
                    if (localbCIScanSection != BCIScanSections.None)
                    {
                        int matchIndex = -1;
                        for (int index = 0; index < comboBox.Items.Count; index++)
                        {
                            string itemString = comboBox.Items[index].ToString();
                            if (itemString.Contains(localbCIScanSection.ToString()))
                            {
                                matchIndex = index;
                                break;
                            }
                        }
                        if (matchIndex > -1)
                        {
                            object itemTomove = comboBox.Items[matchIndex];
                            comboBox.Items.RemoveAt(matchIndex);
                            comboBox.Items.Insert(0, itemTomove);
                        }
                    }
                    if (comboBox.Items.Count > 0)
                        comboBox.SelectedIndex = 0;
                }
                
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error SetDefaultItemInComboBox: " + ex.Message);
            }
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser.Navigating -= WebBrowserDesc_Navigating;
            webBrowser.Navigating += WebBrowserDesc_Navigating;
        }

        private void WebBrowserDesc_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            var str = e.Url.ToString();

            Log.Debug("Url is [" + str + "]");

            if (str.ToLower().Contains("blank"))
            {
                return;
            }

            e.Cancel = true;

            String param1 = String.Empty;
            String param2 = String.Empty;

            if (str.Contains("about:"))
            {
                var index = str.IndexOf(':');

                str = str.Substring(index + 1);

                index = str.IndexOf('#');

                if (index > 0)
                {
                    param1 = str.Substring(0, index);
                    param2 = str.Substring(index + 1, str.Length - index - 1);
                }
                else
                {
                    param1 = str;
                }
            }

            List<String> list = new List<String>();

            if (param2.ToLower().EndsWith(".mp4"))
            {
                list.Add("Video");
                list.Add(String.Empty);
                list.Add(String.Empty);
                list.Add((param2));
                list.Add(String.Empty);
            }
            else if (param1.ToLower().EndsWith(".pdf"))
            {
                list.Add("PDF");
                list.Add("true");
                list.Add(R.GetString("PDFLoaderHtml"));
                list.Add(param1);
                list.Add(param2);
            }

            try
            {
                this.TopMost = false;
                HtmlUtils.LoadHtml(SmartPath.ApplicationPath, list.ToArray());
            }
            catch
            {

            }
            finally
            {

            }
        }
    }
}