////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowActiveWatchdog.cs" company="Intel Corporation">
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

using System;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Checks if the specified form has lost focus, and if so,
    /// immediately restores the focus back.
    /// </summary>
    public class WindowActiveWatchdog : IDisposable
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Which form to watch
        /// </summary>
        private Form _form;

        /// <summary>
        /// Pause the watchdog?
        /// </summary>
        private bool _paused;

        /// <summary>
        /// Constructor.  Allocates resources, event handlers
        /// </summary>
        /// <param name="form">The form to watch</param>
        public WindowActiveWatchdog(Form form)
        {
            _form = form;
            _form.TopMost = false;
            _form.TopMost = true;

            _form.Deactivate += _form_Deactivate;
            _form.VisibleChanged += _form_VisibleChanged;
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Pause the watchdog
        /// </summary>
        public void Pause()
        {
            _paused = true;
        }

        /// <summary>
        /// Resume the watchdog
        /// </summary>
        public void Resume()
        {
            _paused = false;
            reactivateForm();
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                if (disposing)
                {
                    Log.Debug();

                    try
                    {
                        _form.Deactivate -= _form_Deactivate;
                        _form.VisibleChanged -= _form_VisibleChanged;
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.ToString());
                    }

                    _form = null;
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Some other window just got focus.  Restores focus
        /// back to the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _form_Deactivate(object sender, EventArgs e)
        {
            if (!_paused)
            {
                Log.Debug("DEACTVATED!! Re-activating " + getFormName());

                reactivateForm();
            }
        }

        /// <summary>
        /// Triggered when the visibility of the form changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _form_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if (_form != null && _form.Visible)
                {
                    reactivateForm();
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }

        /// <summary>
        /// Sets focus to the form
        /// </summary>
        private void focusThisForm()
        {
            Log.Debug();

            try
            {
                if (_form != null && _form.Visible && _form.WindowState != FormWindowState.Minimized)
                {
                    Log.Debug("Activating form " + getFormName());
                    try
                    {
                        _form.Invoke(new MethodInvoker(delegate
                        {
                            // this is a windows defect.  If topmost
                            // is already true, it has not effect.
                            // so we set it to false and then to true
                            _form.TopMost = false;
                            _form.TopMost = true;

                            Windows.SetForegroundWindow(_form.Handle);

                            _form.Select();
                            _form.Activate();
                            _form.Focus();
                        }));
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }

            Log.Debug("Returning");
        }

        /// <summary>
        /// Returns the name of the form
        /// </summary>
        /// <returns>form name</returns>
        private String getFormName()
        {
            return (_form != null) ? _form.Name : "null";
        }

        /// <summary>
        /// Asychronously sets focus back to the form
        /// </summary>
        private void reactivateForm()
        {
            System.Threading.Tasks.Task t = System.Threading.Tasks.Task.Factory.StartNew(focusThisForm);
        }
    }
}