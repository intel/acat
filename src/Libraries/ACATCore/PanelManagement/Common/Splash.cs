////////////////////////////////////////////////////////////////////////////
// <copyright file="Splash.cs" company="Intel Corporation">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

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
    /// Displays a splash screen.  The splash screen has a
    /// bitmap and three lines of text, all of which can be
    /// set by the application. The SplashScreen class is used
    /// as the splash screen form
    /// </summary>
    public class Splash
    {
        /// <summary>
        /// Event that controls closing of the splash screen
        /// </summary>
        private readonly ManualResetEvent _closeEvent = new ManualResetEvent(false);

        /// <summary>
        /// The path to the image file
        /// </summary>
        private readonly String _image;

        /// <summary>
        /// Line 1 of the text to be displayed
        /// </summary>
        private readonly String _line1;

        /// <summary>
        /// Line 2 of the text to be displayed
        /// </summary>
        private readonly String _line2;

        /// <summary>
        /// Line 3 of the text to be displayed
        /// </summary>
        private readonly String _line3;

        /// <summary>
        /// Background worker object
        /// </summary>
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();

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
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="image">Path to the image</param>
        /// <param name="line1">First line of text</param>
        /// <param name="line2">Second line of text</param>
        /// <param name="line3">Third line of text</param>
        public Splash(String image, String line1 = null, String line2 = null, String line3 = null, int minUpTime = 0)
        {
            _line1 = line1;
            _line2 = line2;
            _line3 = line3;
            _image = image;
            _minUpTime = minUpTime;
            _stopWatch = new Stopwatch();
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

                if (_backgroundWorker != null)
                {
                    _backgroundWorker.CancelAsync();
                    _backgroundWorker.Dispose();
                    _backgroundWorker = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Call this to show the splash screen.  It is display asynchronously
        /// to enable the application to do its initializating tasks
        /// </summary>
        public void Show()
        {
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.WorkerReportsProgress = false;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _stopWatch.Start();
            _backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// The work handler function flr the background worker
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String appName = _line1 ?? String.Empty;
            String appVersion = _line2 ?? String.Empty;
            String appCopyright = _line3 ?? String.Empty;
            _closeEvent.Reset();
            _form = new SplashScreen(appName, appVersion, appCopyright, _image);
            _form.ShowDialog();
        }

        /// <summary>
        /// Background worker completed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

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