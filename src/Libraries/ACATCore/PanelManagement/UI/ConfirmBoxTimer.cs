////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// A message box with a countdown timer
    /// </summary>
    public partial class ConfirmBoxTimer : Form
    {
        public bool Result;

        public ConfirmBoxTimer()
        {
            InitializeComponent();
            Load += ConfirmBox_Load;
        }

        /// <summary>
        /// Custom Message Box showing a countdown
        /// </summary>
        public String Prompt { get; set; }

        public int SecondsCounter { get; set; }

        public static bool ShowDialog(String prompt, int secondsCounter, Form parent = null)
        {
            var confirmBoxTimer = new ConfirmBoxTimer();
            confirmBoxTimer.Prompt = prompt;
            confirmBoxTimer.SecondsCounter = secondsCounter;
            confirmBoxTimer.ShowDialog(parent);

            var result = confirmBoxTimer.Result;

            confirmBoxTimer.Dispose();

            return result;
        }

        public async Task startCountdown()
        {
            String promptText;

            await Task.Delay(25);
            while (SecondsCounter > 0)
            {
                promptText = Prompt + SecondsCounter + (SecondsCounter > 1 ? " seconds" : " second");

                if (labelPrompt.InvokeRequired)
                {
                    labelPrompt.Invoke(new Action(() =>
                    {
                        labelPrompt.Text = promptText;
                    }));
                }
                else
                {
                    labelPrompt.Text = promptText;
                }
                await Task.Delay(1000);
                SecondsCounter -= 1;
            }
            Result = true;
            Close();
        }

        private void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            _ = startCountdown().ConfigureAwait(false);
        }
    }
}