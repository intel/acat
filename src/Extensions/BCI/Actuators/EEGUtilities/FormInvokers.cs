////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// FormInvokers.cs
//
// Handles from invokers
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.Actuators.EEG.EEGUtils
{
    public class FormInvokers
    {
        /// <summary>
        /// Class used to debug from ACAT using Visual Studio. All UI elements need to be implemented like that (option 2)
        /// ///

        private delegate void appendTextInRichTextBoxDelegate(RichTextBox control, string text);

        private delegate void setButtonTextDelegate(Button control, String text);

        private delegate void SetCheckDelegate(CheckBox control, bool check);

        private delegate void setPictureBoxVisibleDelegate(PictureBox control, bool isVisible);

        private delegate void setTrackBackValueDelegate(TrackBar control, int value);

        public static void appendTextInRichTextBox(RichTextBox control, string text)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new appendTextInRichTextBoxDelegate(appendTextInRichTextBox), control, text);
            }
            else
            {
                control.AppendText(text);
                control.ScrollToCaret();
            }
        }

        public static void setButtonText(Button control, String text)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new setButtonTextDelegate(setButtonText), control, text);
            }
            else
            {
                control.Text = text;
            }
        }

        public static void setCheck(CheckBox control, bool check)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetCheckDelegate(setCheck), control, check);
            }
            else
            {
                control.Checked = check;
            }
        }

        public static void setPictureBoxVisible(PictureBox control, bool isVisible)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new setPictureBoxVisibleDelegate(setPictureBoxVisible), control, isVisible);
            }
            else
            {
                control.Visible = isVisible;
            }
        }

        public static void setTrackBackValue(TrackBar control, int value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new setTrackBackValueDelegate(setTrackBackValue), control, value);
            }
            else
            {
                control.Value = value;
            }
        }
    }
}