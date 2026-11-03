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

using ACAT.Core.Utility;
using ACAT.Extensions.BCI.Common.AnimationSharp.Utility;
using ACAT.Extensions.BCI.Common.BCIControl;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.UI.UserControls
{
    [ClassDescriptorAttribute("UC7A983F-7FDE-4811-AFED-8B8D18617E02",
                    "TalkApplicationScannerLayout",
                    "Talk application window with, added features")]
    public partial class LEDStatusUserControl : UserControl
    {
        public static bool getData = false;

        public LEDStatusUserControl()
        {
            InitializeComponent();
            getData = true;
            UpdateSensorStatus().Forget(ex => Trace.TraceError(ex.ToString()));
        }

        public static void OnFormClossing()
        {
            getData = false;
        }

        /// <summary>
        /// Task to update LED indicators of the Cap
        /// </summary>
        /// <returns></returns>
        public Task UpdateSensorStatus()
        {
            return UpdateSensorStatus(default);
        }

        public async Task UpdateSensorStatus(CancellationToken cancellationToken)
        {
            while (getData && !cancellationToken.IsCancellationRequested)
            {
                if (AnimationManagerUtils.StatusSignal == SignalStatus.SIGNAL_OK)
                    BT1.BackColor = Color.Green;
                else
                    BT1.BackColor = Color.Red;
                await Task.Delay(1000, cancellationToken);
            }
            await Task.Delay(50, cancellationToken);
        }
    }
}