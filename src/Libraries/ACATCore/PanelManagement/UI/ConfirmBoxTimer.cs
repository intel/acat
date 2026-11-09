////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Core.PanelManagement
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
            var confirmBoxTimer = new ConfirmBoxTimer
            {
                Prompt = prompt,
                SecondsCounter = secondsCounter
            };
            confirmBoxTimer.ShowDialog(parent);

            var result = confirmBoxTimer.Result;

            confirmBoxTimer.Dispose();

            return result;
        }

        public Task startCountdown()
        {
            return startCountdown(default);
        }

        public async Task startCountdown(CancellationToken cancellationToken)
        {
            String promptText;

            await Task.Delay(25, cancellationToken);
            while (SecondsCounter > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
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
                await Task.Delay(1000, cancellationToken);
                SecondsCounter -= 1;
            }
            Result = true;
            Close();
        }

        private async void ConfirmBox_Load(object sender, EventArgs e)
        {
            CenterToScreen();
            labelPrompt.Text = Prompt;
            await startCountdown();
        }
    }
}