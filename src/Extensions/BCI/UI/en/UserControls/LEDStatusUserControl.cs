////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// LEDStatusUserControl.cs
//
// User control for status of the sensor
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Extensions.BCI.Common.AnimationSharp;
using ACAT.Extensions.BCI.Common.BCIControl;
using ACAT.Lib.Core.Utility;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.UI.UserControls
{

    [DescriptorAttribute("UC7A983F-7FDE-4811-AFED-8B8D18617E02",
                    "TalkApplicationScannerLayout",
                    "Talk application window with, added features")]

    public partial class LEDStatusUserControl : UserControl
    {
        public static bool getData = false;
        public LEDStatusUserControl()
        {
            InitializeComponent();
            getData = true;
            _ = UpdateSensorStatus().ConfigureAwait(false);
        }
        public static void OnFormClossing()
        {
            getData = false;
        }

        /// <summary>
        /// Task to update LED indicators of the Cap
        /// </summary>
        /// <returns></returns>
        public async Task UpdateSensorStatus()
        {
            while (getData)
            {
                if (AnimationManagerUtils.StatusSignal == SignalStatus.SIGNAL_OK)
                    BT1.BackColor = Color.Green;
                else
                    BT1.BackColor = Color.Red;
                await Task.Delay(1000);
            }
            await Task.Delay(50);
        }
    }
}
