////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace ACAT.Extensions.BCI.Common.BCIInterfaceUtilities
{
    /// <summary>
    /// Form that handles different calibraitons options to configure and initialize calibration sesion
    /// </summary>
    ///

    [DescriptorAttribute("AEC69561-1525-4C71-8388-651277AFABC3",
        "OtherTestForm",
        "Application window used to display other test for BCI")]
    public partial class OtherTestForm : Form
    {
        /// <summary>
        /// Custom Tooltip object
        /// </summary>
        private CustomToolTip customToolTip = new CustomToolTip();

        private BCIMenuOptions.Options Options;

        private Screen primaryScreen = Screen.PrimaryScreen;

        /// <summary>
        /// Confirm Box with multiple results
        /// </summary>
        public OtherTestForm()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }

        public static BCIMenuOptions.Options ShowFormDialog(Form parent = null, bool setTopMost = false)
        {
            var confirmBox = new OtherTestForm();
            if (parent != null && setTopMost)
            {
                parent.TopMost = false;
                confirmBox.TopMost = true;
            }
            //To always display the form in the main screen
            confirmBox.StartPosition = FormStartPosition.Manual;
            confirmBox.Location = confirmBox.primaryScreen.WorkingArea.Location;
            confirmBox.ShowDialog(parent);
            BCIMenuOptions.Options retVal = confirmBox.Options;
            if (parent != null && setTopMost)
            {
                parent.TopMost = true;
                confirmBox.TopMost = false;
            }
            confirmBox.Dispose();
            return retVal;
        }

        private void ButtonCancel_Click_1(object sender, EventArgs e)
        {
            Options = BCIMenuOptions.Options.None;
            Close();
        }

        private void ButtonInfoModes_MouseEnter(object sender, EventArgs e)
        {
            ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
            switch (scannerRoundedButtonControl.Name)
            {
                case var _ when scannerRoundedButtonControl.Name.Contains("ReMapCalibrations"):
                    customToolTip.ShowToolTip(BCIR.GetString("HintReMapCalibrations"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("TriggerCheck"):
                    customToolTip.ShowToolTip(BCIR.GetString("HintTriggerCheck"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("SignalCheck"):
                    customToolTip.ShowToolTip(BCIR.GetString("HintSignalCheck"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;

                case var _ when scannerRoundedButtonControl.Name.Contains("EyesOpenClose"):
                    customToolTip.ShowToolTip(BCIR.GetString("HintEyesOpenCloseCalibration"), (ScannerRoundedButtonControl)sender, 5, 5);
                    break;
            }
        }

        private void ButtonInfoModes_MouseLeave(object sender, EventArgs e)
        {
            customToolTip.HideToolTip();
        }

        private void ButtonOpc_Click(object sender, EventArgs e)
        {
            try
            {
                ScannerRoundedButtonControl scannerRoundedButtonControl = (ScannerRoundedButtonControl)sender;
                switch (scannerRoundedButtonControl.Name)
                {
                    case var _ when scannerRoundedButtonControl.Name.Contains("ReMapCalibrations"):
                        Log.Debug("BCI LOG | AdvancedOptions | ButtonClick: ReMapCalibrations");
                        Options = BCIMenuOptions.Options.RemapCalibrations;
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("TriggerCheck"):
                        Log.Debug("BCI LOG | AdvancedOptions | ButtonClick: TriggerCheck");
                        Options = BCIMenuOptions.Options.TriggerTest;
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("SignalCheck"):
                        Log.Debug("BCI LOG | AdvancedOptions | ButtonClick: SignalCheck");
                        Options = BCIMenuOptions.Options.SignalCheck;
                        break;

                    case var _ when scannerRoundedButtonControl.Name.Contains("EyesOpenClose"):
                        Log.Debug("BCI LOG | AdvancedOptions | ButtonClick: EyesOpenClose");
                        Options = BCIMenuOptions.Options.EyesCalibration;
                        break;
                }

                Close();
            }
            catch (Exception ex)
            {
                Log.Debug("BCI LOG | Error ButtonCalibrate_Click: " + ex.Message);
                Close();
            }
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            HighlightButton(ButtonOpcReMapCalibrations);
        }

        private void ConfirmBoxCalibrationModes_FormClosing(object sender, FormClosingEventArgs e)
        {
            customToolTip.CloseToolTip();
            customToolTip.Dispose();
            customToolTip = null;
        }

        /// <summary>
        /// Highlights a button to display it as selected
        /// </summary>
        /// <param name="scannerRoundedButtonControl"></param>
        private void HighlightButton(ScannerRoundedButtonControl scannerRoundedButtonControl)
        {
            scannerRoundedButtonControl.BackColor = Color.FromArgb(255, 170, 0);
            scannerRoundedButtonControl.ForeColor = Color.Black;
        }
    }
}