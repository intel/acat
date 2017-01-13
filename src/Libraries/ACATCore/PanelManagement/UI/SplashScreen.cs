////////////////////////////////////////////////////////////////////////////
// <copyright file="SplashScreen.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Form for the splash screen.  Can be customized with
    /// three lines of text and a bitmap
    /// </summary>
    public partial class SplashScreen : Form
    {
        /// <summary>
        /// Make sure nothing overlaps this form
        /// </summary>
        private readonly WindowOverlapWatchdog _watchDog;

        /// <summary>
        /// Index of the image in the image list
        /// </summary>
        private int _imageIndex;

        /// <summary>
        /// Timer used to udpate info on the form
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the class..  Parameters
        /// can be used to cutomize the screen
        /// </summary>
        /// <param name="line1">First line</param>
        /// <param name="line2">Second line</param>
        /// <param name="line3">Third line</param>
        /// <param name="line4">Fourth line</param>
        /// <param name="image">Path to bitmap</param>
        public SplashScreen(String line1, String line2, String line3, String line4, String image)
        {
            InitializeComponent();

            this.labelLine1.Text = line1;
            this.labelLine2.Text = line2;
            this.labelLine3.Text = line3;
            this.labelLine4.Text = line4;

            Windows.SetWindowPosition(this, Windows.WindowPosition.BottomRight);

            try
            {
                var img = Image.FromFile(image);
                var bitmap = ImageUtils.ImageOpacity(img, 1.0F, new Rectangle(0, 0, img.Width, img.Height));
                splashPictureBox.BackgroundImage = bitmap;
            }
            catch
            {
            }

            FormClosing += Form1_FormClosing;

            ShowInTaskbar = false;

            TopMost = true;

            pictureBoxStatus.BackgroundImageLayout = ImageLayout.Stretch;

            _watchDog = new WindowOverlapWatchdog(this);

            Load += SplashScreen_Load;
            Shown += SplashScreen_Shown;
        }

        /// <summary>
        /// Call this to close the form
        /// </summary>
        public void Dismiss()
        {
            stopTimer();

            Windows.CloseForm(this);

            DialogResult = DialogResult.Yes;
        }

        /// <summary>
        /// Timer proc
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            pictureBoxStatus.BackgroundImage = imageList.Images[_imageIndex];
            _imageIndex = (_imageIndex + 1) % imageList.Images.Count;
        }

        /// <summary>
        /// Performs form close actions
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_watchDog != null)
            {
                _watchDog.Dispose();
            }

            _timer.Dispose();
        }

        /// <summary>
        /// Perfoms initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SplashScreen_Load(object sender, EventArgs e)
        {
            startTimer();
        }

        /// <summary>
        /// Event handler for when the form is showns
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            Windows.SetTopMost(this);
        }

        /// <summary>
        /// Starts the timer to animate the image list at the bottom of
        /// the splash screen to indicate 'activity'
        /// </summary>
        private void startTimer()
        {
            _timer = new Timer { Interval = 250 };
            _timer.Tick += _timer_Tick;
            _timer_Tick(null, null);
            Invoke(new MethodInvoker(() => _timer.Start()));
        }

        /// <summary>
        /// Stops the timer for animating the image list
        /// </summary>
        private void stopTimer()
        {
            try
            {
                _timer.Tick -= _timer_Tick;
                _timer.Stop();
            }
            catch
            {
            }
        }
    }
}