////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Keeps watch if the form is getting obscured by other
    /// windows and if so, brings it back on top. Based on a
    /// watchdog timer.  This is useful to make sure nothing else
    /// overlaps.  The TopMost attribute can be used to make
    /// a window the topmost window but if another window
    /// also has the TopMost attribute set, that window could
    /// overlap our window.  This class makes sure that
    /// doesn't happen
    /// </summary>
    public class WindowOverlapWatchdog : IDisposable
    {
        /// <summary>
        /// How often to check
        /// </summary>
        private const int Interval = 1500;

        /// <summary>
        /// Set to true to Force to stay on top.
        /// </summary>
        private readonly bool _force;

        /// <summary>
        /// Pause operations?
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Timer to check for overlap
        /// </summary>
        private System.Timers.Timer _timer;

        /// <summary>
        /// The form
        /// </summary>
        private Form _window;

        private Int32 _windowHandle;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="window">The form that needs to stay on top</param>
        /// <param name="force">set to true to force to stay on top</param>
        public WindowOverlapWatchdog(Form window, bool force = false)
        {
            _window = window;
            _force = force;
            window.VisibleChanged += window_VisibleChanged;
            init();
        }

        public WindowOverlapWatchdog(Int32 windowHandle, bool force = false)
        {
            _windowHandle = windowHandle;
            _force = force;
            init();
        }

        /// <summary>
        /// Stops the watchdog timer
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            if (_window != null)
            {
                Log.Debug("DISPOSE!!  for " + _window.Name);
                _window.VisibleChanged -= window_VisibleChanged;
            }

            if (_timer != null)
            {
                _timer.Elapsed -= timer_Elapsed;
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }

            if (_window != null)
            {
                _window.VisibleChanged -= window_VisibleChanged;
                _window = null;
            }
        }

        /// <summary>
        /// Pause the timer (don't check for now)
        /// </summary>
        public void Pause()
        {
            if (_window != null)
            {
                Log.Debug("PAUSE!!  for " + _window.Name);
            }

            stopTimer();
            _isPaused = true;
        }

        /// <summary>
        /// Resume checking
        /// </summary>
        public void Resume()
        {
            if (_window != null)
            {
                Log.Debug("RESUME!!  for " + _window.Name);
            }

            _isPaused = false;
            startTimer();
        }

        private void init()
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += timer_Elapsed;
            _timer.Interval = Interval;
            _timer.Start();
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
            if (_window == null && _windowHandle == 0)
            {
                return false;
            }

            if (_window != null && (_window.Handle == IntPtr.Zero || !Windows.GetVisible(_window)))
            {
                return false;
            }

            IntPtr windowHandle = (_window != null) ? _window.Handle : new IntPtr(_windowHandle);

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
                if (!_force && User32Interop.IsWindowVisible(windowHandle))
                {
                    try
                    {
                        Control ctl = null;
                        if (_window != null)
                        {
                            ctl = Form.FromHandle(windowHandle);
                        }

                        isScanner = (ctl is Form) && (ctl is IScannerPanel);
                        if (isScanner)
                        {
                            double opacity = Windows.GetOpacity(ctl as Form);
                            isScanner = (opacity == 1.0f);
                        }
                    }
                    catch (Exception ex)
                    {
                        isScanner = false;
                        Log.Debug("isScanner logic exception " + ex);
                    }
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

            cache.Clear();

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
        private void timer_Elapsed(object sender, EventArgs e)
        {
            if (_window == null && _windowHandle == 0)
            {
                return;
            }

            try
            {
                if (_window != null && _window.IsDisposed)
                {
                    return;
                }

                if (_window != null)
                {
                    //Log.Debug("Setting topmost for " + _window.Name);
                    Windows.SetTopMost(_window, false);
                    Windows.SetTopMost(_window, true);
                }
                else
                {
                    //Log.Debug("Setting topmost for non Form" + _windowHandle);
                    uint flags = (uint)User32Interop.SetWindowPosFlags.SWP_NOMOVE | (uint)User32Interop.SetWindowPosFlags.SWP_NOSIZE;
                    User32Interop.SetWindowPos(_windowHandle, (int)User32Interop.HWND_TOPMOST, 0, 0, 0, 0, flags);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("exception occured!  ex=" + ex + "for " + (_window != null ? _window.Name : "null"));
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

            Log.Debug("Visibility changed for " + _window.Name + " To " + Windows.GetVisible(_window));
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