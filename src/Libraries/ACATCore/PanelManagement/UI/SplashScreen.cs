////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Form for the splash screen.  Can be customized with
    /// three lines of text and a bitmap
    /// </summary>
    public partial class SplashScreen : Form
    {
        private const int maxPeriods = 6;

        /// <summary>
        /// Make sure nothing overlaps this form
        /// </summary>
        private readonly WindowOverlapWatchdog _watchDog;

        private int _count = 0;

        /// <summary>
        /// Timer used to udpate info on the form
        /// </summary>
        private Timer _timer;

        //String starting = "Starting.";
        private readonly String starting = ".";

        /// <summary>
        /// Initializes a new instance of the class..  Parameters
        /// can be used to cutomize the screen
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();

            Windows.SetWindowPosition(this, Windows.WindowPosition.CenterScreen);

            FormClosing += Form1_FormClosing;

            ShowInTaskbar = false;

            TopMost = true;

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
            String str;
            _count = (++_count % maxPeriods);
            if (_count == 0)
            {
                str = starting;
            }
            else
            {
                str = labelStarting.Text.Trim();
                str += " .";
            }

            labelStarting.Text = str;
        }

        /// <summary>
        /// Performs form close actions
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _watchDog?.Dispose();

            _timer.Dispose();
        }

        /// <summary>
        /// Perfoms initialization
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SplashScreen_Load(object sender, EventArgs e)
        {
            var assembly = Assembly.GetEntryAssembly();
            // get appname and copyright information
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

            var appName = (attributes.Length != 0) ?
                            ((AssemblyTitleAttribute)attributes[0]).Title :
                            String.Empty;

            var appVersion = "Version " + assembly.GetName().Version.Major + "." + assembly.GetName().Version.Minor;

            labelAppNameAndVersion.Text = appName + "\r\n" + appVersion;

            attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            var appCopyright = (attributes.Length != 0)
                ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright
                : String.Empty;

            labelLicense.Text = appCopyright;

            labelStarting.Text = starting;

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