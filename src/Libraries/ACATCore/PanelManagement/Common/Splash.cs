/////////////////////////////////////////////////////////////////////////////
// <copyright file="Splash.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.Reflection;
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
        /// The path to the image file
        /// </summary>
        private String _image;

        /// <summary>
        /// Line 1 of the text to be displayed
        /// </summary>
        private String _line1;

        /// <summary>
        /// Line 2 of the text to be displayed
        /// </summary>
        private String _line2;

        /// <summary>
        /// Line 3 of the text to be displayed
        /// </summary>
        private String _line3;

        /// <summary>
        /// Line 4 of the text to be displayed
        /// </summary>
        private String _line4;

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
            initSplash(FileUtils.GetImagePath("SplashScreenImage.png"), null, null, null, null, minUpTime);
        }

        /// <summary>
        /// Initializes a new instance of the class.  Fills in details
        /// from the assembly info (version, copyright etc)
        /// </summary>
        /// <param name="image">Splash image</param>
        /// <param name="minUpTime">min time to stay on</param>
        public Splash(String image, int minUpTime = 0)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // get appname and copyright information
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var appName = (attributes.Length != 0)
                ? ((AssemblyTitleAttribute)attributes[0]).Title
                : String.Empty;

            var appVersion = "Version " + assembly.GetName().Version;
            attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            var appCopyright = (attributes.Length != 0)
                ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright
                : String.Empty;

            initSplash(image, appName, appVersion, String.Empty, appCopyright, minUpTime);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="line1">First line of text</param>
        /// <param name="line2">Second line of text</param>
        /// <param name="line3">Third line of text</param>
        public Splash(String line1 = null, String line2 = null, String line3 = null, String line4 = null, int minUpTime = 0)
        {
            initSplash(FileUtils.GetImagePath("SplashScreenImage.png"), line1, line2, line3, line4, minUpTime);
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="image">Path to the image</param>
        /// <param name="line1">First line of text</param>
        /// <param name="line2">Second line of text</param>
        /// <param name="line3">Third line of text</param>
        public Splash(String image, String line1 = null, String line2 = null, String line3 = null, String line4 = null, int minUpTime = 0)
        {
            initSplash(image, line1, line2, line3, line4, minUpTime);
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
        /// <param name="image">Path to the image</param>
        /// <param name="line1">First line of text</param>
        /// <param name="line2">Second line of text</param>
        /// <param name="line3">Third line of text</param>
        /// <param name="minUpTime">Minimun length of time splash should be up</param>
        private void initSplash(String image, String line1 = null, String line2 = null, String line3 = null, String line4 = null, int minUpTime = 0)
        {
            var assembly = Assembly.GetEntryAssembly();
            // get appname and copyright information
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

            var appName = (attributes.Length != 0) ?
                            ((AssemblyTitleAttribute)attributes[0]).Title :
                            String.Empty;

            var appVersion = "Version " + assembly.GetName().Version;

            attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            var companyName = (attributes.Length != 0) ?
                                ((AssemblyCompanyAttribute)attributes[0]).Company :
                                String.Empty;

            attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            var appCopyright = (attributes.Length != 0)
                ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright
                : String.Empty;

            _line1 = line1 ?? appName;
            _line2 = line2 ?? appVersion;
            _line3 = line3 ?? companyName;
            _line4 = line4 ?? appCopyright;
            _image = image;
            _minUpTime = minUpTime;

            _stopWatch = new Stopwatch();
        }

        /// <summary>
        /// Displays the splash screen
        /// </summary>
        private void showSplash()
        {
            var line1 = _line1 ?? String.Empty;
            var line2 = _line2 ?? String.Empty;
            var line3 = _line3 ?? String.Empty;
            var line4 = _line4 ?? String.Empty;

            _form = new SplashScreen(line1, line2, line3, line4, _image);

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