////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Diagnostics;
using System.Threading;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Displays a splash screen.  The splash screen has a
    /// bitmap and three lines of text, all of which can be
    /// set by the application. The SplashScreen class is used
    /// as the splash screen form
    /// </summary>
    public class Splash
    {
        /// <summary>
        /// The splash screen form
        /// </summary>
        private SplashScreen _form;

        /// <summary>
        /// Minimum time in ms the splash screen has to stay up
        /// </summary>
        private int _minUpTime;

        /// <summary>
        /// To track the time
        /// </summary>
        private Stopwatch _stopWatch;

        /// <summary>
        /// Thread to show the splash screen
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// Initializes a new instance of the class.  Fills in details
        /// from the assembly info (version, copyright etc)
        /// </summary>
        /// <param name="minUpTime">min time to stay on</param>
        public Splash(int minUpTime = 0)
        {
            initSplash(minUpTime);
        }

        /// <summary>
        /// Call this to dismiss the splash screen
        /// </summary>
        public void Close()
        {
            try
            {
                waitUntilUpTime();

                if (_form != null)
                {
                    _form.Dismiss();
                    _form = null;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Call this to show the splash screen.  It is display asynchronously
        /// to enable the application to do its initializating tasks
        /// </summary>
        public void Show()
        {
            _thread = new Thread(showSplash) { IsBackground = true };
            _thread.SetApartmentState(ApartmentState.STA);
            _stopWatch.Start();
            _thread.Start();
        }

        /// <summary>
        /// Initializes class
        /// </summary>
        /// <param name="minUpTime">Minimun length of time splash should be up</param>
        private void initSplash(int minUpTime = 0)
        {
            _minUpTime = minUpTime;
            _stopWatch = new Stopwatch();
        }

        /// <summary>
        /// Displays the splash screen
        /// </summary>
        private void showSplash()
        {
            _form = new SplashScreen();

            _form.ShowDialog();
        }

        /// <summary>
        /// Waits until the minimum uptime has elapsed
        /// </summary>
        private void waitUntilUpTime()
        {
            if (_stopWatch.IsRunning && _minUpTime > 0)
            {
                long elapsedTime = _stopWatch.ElapsedMilliseconds;
                if (elapsedTime < _minUpTime)
                {
                    long timeToSleep = _minUpTime - elapsedTime;

                    Thread.Sleep((int)timeToSleep);
                }
            }
        }
    }
}