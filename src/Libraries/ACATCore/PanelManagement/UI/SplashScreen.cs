////////////////////////////////////////////////////////////////////////////
// <copyright file="SplashScreen.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Form for the splash screen.  Can be customized with
    /// three lines of text and a bitmap
    /// </summary>
    public partial class SplashScreen : Form
    {
        /// <summary>
        /// Says 'Loading..."
        /// </summary>
        private readonly String _loadingLabel;

        /// <summary>
        /// Make sure nothing overlaps this form
        /// </summary>
        private readonly WindowOverlapWatchdog _watchDog;

        /// <summary>
        /// A counter
        /// </summary>
        private int _periodCounter;

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
        /// <param name="image">Path to bitmap</param>
        public SplashScreen(String line1, String line2, String line3, String image)
        {
            InitializeComponent();

            this.line1.Text = line1;
            this.line2.Text = line2;
            this.line3.Text = line3;

            Windows.SetWindowPosition(this, Windows.WindowPosition.CenterScreen);

            try
            {
                var img = Image.FromFile(image);
                var bitmap = ImageUtils.ImageOpacity(img, 1.0F, new Rectangle(0, 0, img.Width, img.Height));
                splashPictureBox.Image = bitmap;
            }
            catch
            {
            }

            FormClosing += Form1_FormClosing;

            ShowInTaskbar = false;

            TopMost = true;

            _loadingLabel = lblLoading.Text;

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
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int leftRect, // x-coordinate of upper-left corner
            int topRect, // y-coordinate of upper-left corner
            int rightRect, // x-coordinate of lower-right corner
            int bottomRect, // y-coordinate of lower-right corner
            int widthEllipse, // height of ellipse
            int heightEllipse);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject([In] IntPtr handle);

        /// <summary>
        /// Timer proc
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            _periodCounter = (_periodCounter + 1) % 5;
            var loading = _loadingLabel;
            for (int ii = 0; ii < _periodCounter; ii++)
            {
                loading = loading + ". ";
            }

            Windows.SetText(lblLoading, loading);
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
            IntPtr handle = CreateRoundRectRgn(0, 0, Width, Height, 15, 15);
            if (handle != IntPtr.Zero)
            {
                Region = Region.FromHrgn(handle);
                DeleteObject(handle);
            }

            startTimer();
        }

        /// <summary>
        /// Called when the form is shown
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SplashScreen_Shown(object sender, EventArgs e)
        {
            Windows.SetTopMost(this);
        }

        /// <summary>
        /// Start the timer to display the animated "Loading..." text
        /// </summary>
        private void startTimer()
        {
            _timer = new Timer { Interval = 250 };
            _timer.Tick += _timer_Tick;
            Invoke(new MethodInvoker(() => _timer.Start()));
        }

        private void stopTimer()
        {
            _timer.Tick -= _timer_Tick;
            _timer.Stop();
        }

        // width of ellipse
    }
}