////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowActiveWatchdog.cs" company="Intel Corporation">
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
using System.Windows.Forms;

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

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Checks if the specified form has lost focus, and if so,
    /// immediately restores the focus back.
    /// </summary>
    public class WindowActiveWatchdog : IDisposable
    {
        /// <summary>
        /// How often to check?
        /// </summary>
        private const int Interval = 10;

        /// <summary>
        /// Timer to track the form focus
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// Which form to watch
        /// </summary>
        private Form _form;

        /// <summary>
        /// Constructor.  Allocates resources, event handlers
        /// </summary>
        /// <param name="form">The form to watch</param>
        public WindowActiveWatchdog(Form form)
        {
            _form = form;
            _form.TopMost = true;
            _timer = new Timer { Interval = Interval };
            _form.Activated += _form_Activated;
            _form.Deactivate += _form_Deactivate;
            _timer.Tick += timer_Tick;
            _form.VisibleChanged += _form_VisibleChanged;
        }

        /// <summary>
        /// Call this to unallocate resources.
        /// </summary>
        public void Dispose()
        {
            Log.Debug();

            try
            {
                _timer.Dispose();
                Log.Debug("_timer is disposed");
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }

            _form = null;
        }

        /// <summary>
        /// Our form is active.  Stop timer
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _form_Activated(object sender, EventArgs e)
        {
            Log.Debug("Stopping timer");
            _timer.Stop();
        }

        /// <summary>
        /// Some other window just got focus.  Start timer and
        /// restore focus
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _form_Deactivate(object sender, EventArgs e)
        {
            Log.Debug("Starting timer");
            _timer.Start();
        }

        /// <summary>
        /// Triggered when the visibility of the form changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void _form_VisibleChanged(object sender, EventArgs e)
        {
            if (_form != null)
            {
                if (_form.Visible)
                {
                    Log.Debug("Starting timer");
                    _timer.Start();
                }
                else
                {
                    Log.Debug("Stopping timer");
                    _timer.Stop();
                }
            }
        }

        /// <summary>
        /// Timer tick.  Activates window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            Log.Debug();
            if (_form == null)
            {
                _timer.Stop();
                return;
            }

            if (_form.Visible && _form.WindowState != FormWindowState.Minimized)
            {
                Log.Debug("Activating form");
                try
                {
                    _form.Invoke(new MethodInvoker(delegate()
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
                catch
                {
                }
            }
            else
            {
                Log.Debug("Form is not visible");
            }

            _timer.Stop();
        }
    }
}