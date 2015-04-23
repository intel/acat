////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowOverlapWatchdog.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    /// Keeps watch if the form is getting obscured by other windows and if so,
    /// brings it back on top. Based on a watchdog timer.  This is useful to
    /// make sure nothing else overlaps.  The TopMost attribute can be used
    /// to make a window the topmost window but if another window also has the
    /// TopMost attribute set, that window could overlap our window.  This
    /// class makes sure that doesn't happen
    /// </summary>
    public class WindowOverlapWatchdog : IDisposable
    {
        /// <summary>
        /// How often to check
        /// </summary>
        private const int Interval = 1500;

        /// <summary>
        /// Pause operations?
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Timer to check for overlap
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// The form
        /// </summary>
        private Form _window;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="window">The form that needs to stay on top</param>
        public WindowOverlapWatchdog(Form window)
        {
            _window = window;
            _timer = new Timer { Enabled = true, Interval = Interval };
            _timer.Tick += timer_Tick;
            window.VisibleChanged += window_VisibleChanged;
        }

        /// <summary>
        /// Stops the watchdog timer
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Tick -= timer_Tick;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            _window.VisibleChanged -= window_VisibleChanged;
            _window = null;
        }

        /// <summary>
        /// Pause the timer (don't check for now)
        /// </summary>
        public void Pause()
        {
            stopTimer();
            _isPaused = true;
        }

        /// <summary>
        /// Resume checking
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
            startTimer();
        }

        /// <summary>
        /// Checks if any part of the specified window is obscured by
        /// another window. Enumerates all the windows, checks the bounding
        /// rectangle of each and ensures that the rectangle doesn't intersect
        /// with our window
        /// </summary>
        /// <returns>true if it is</returns>
        private bool isObscuredWindow()
        {
            if (_window == null || _window.Handle == IntPtr.Zero || !Windows.GetVisible(_window))
            {
                return false;
            }

            IntPtr windowHandle = _window.Handle;

            // to keep track of whether we have already visited this window
            var cache = new HashSet<IntPtr> { windowHandle };

            User32Interop.RECT windowRect;
            User32Interop.GetWindowRect(windowHandle, out windowRect);

            // now, step through all active windows and check for overlap
            while ((windowHandle = User32Interop.GetWindow(windowHandle, User32Interop.GW_HWNDPREV)) != IntPtr.Zero &&
                   !cache.Contains(windowHandle))
            {
                User32Interop.RECT rect;
                User32Interop.RECT intersection;

                cache.Add(windowHandle);

                bool isScanner = false;
                if (User32Interop.IsWindowVisible(windowHandle))
                {
                    Control ctl = Form.FromHandle(windowHandle);
                    isScanner = (ctl is Form) && (ctl is IScannerPanel);
                }

                if (User32Interop.IsWindowVisible(windowHandle) &&
                    !Windows.IsMinimized(windowHandle) &&
                    !isScanner &&
                    User32Interop.GetWindowRect(windowHandle, out rect) &&
                    User32Interop.IntersectRect(out intersection, ref windowRect, ref rect))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        private void startTimer()
        {
            if (_timer != null)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        private void stopTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Checks if any part of the window is obscured and if so,
        /// brings it back on top.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isObscuredWindow())
                {
                    _window.Invoke(new MethodInvoker(delegate
                    {
                        _window.TopMost = false;
                        _window.TopMost = true;
                    }));
                }
            }
            catch (Exception ex)
            {
                Log.Debug("exception occured!  ex=" + ex);
            }
        }

        /// <summary>
        /// Event handler for when the form visibility changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void window_VisibleChanged(object sender, EventArgs e)
        {
            if (_window == null || !_window.IsHandleCreated)
            {
                return;
            }

            if (Windows.GetVisible(_window))
            {
                if (!_isPaused)
                {
                    startTimer();
                }
            }
            else
            {
                stopTimer();
            }
        }
    }
}