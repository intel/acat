////////////////////////////////////////////////////////////////////////////
// <copyright file="DockScanner.cs" company="Intel Corporation">
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
using System.Windows.Automation;
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
    /// Docks a scanner to a specified window.  the scanner is always
    /// docked to the left of the parent window
    /// </summary>
    public class DockScanner : IDisposable
    {
        /// <summary>
        /// UI automation element represnting the window to dock to
        /// </summary>
        private AutomationElement _automationElementDockTo;

        /// <summary>
        /// Have we been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The form that should be docked to the window
        /// </summary>
        private Form _form;

        /// <summary>
        /// Handle of the window to dock to
        /// </summary>
        private IntPtr _windowHandleDockTo;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="windowHandleDockTo">handle of window to dock to</param>
        /// <param name="form">the dockee</param>
        public DockScanner(IntPtr windowHandleDockTo, Form form)
        {
            _windowHandleDockTo = windowHandleDockTo;
            _form = form;
            _automationElementDockTo = null;
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
        /// Docks the form to the window.  If the parent
        /// window moves, the docked window moves with it
        /// </summary>
        public void Dock()
        {
            if (_windowHandleDockTo == IntPtr.Zero)
            {
                return;
            }

            if (_automationElementDockTo == null)
            {
                try
                {
                    _automationElementDockTo = AutomationElement.FromHandle(_windowHandleDockTo);
                    AutomationEventManager.AddAutomationPropertyChangedEventHandler(_windowHandleDockTo,
                                                                            AutomationElement.BoundingRectangleProperty,
                                                                            _automationElementDockTo,
                                                                            onWindowPositionChanged);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex.ToString());
                    _automationElementDockTo = null;
                }
            }

            positionWindow();
        }

        /// <summary>
        /// Undocks the dockee
        /// </summary>
        public void UnDock()
        {
            if (_windowHandleDockTo != IntPtr.Zero && _automationElementDockTo != null)
            {
                try
                {
                    AutomationEventManager.RemoveAutomationPropertyChangedEventHandler(_windowHandleDockTo,
                                                                                        AutomationElement.BoundingRectangleProperty,
                                                                                        _automationElementDockTo,
                                                                                        onWindowPositionChanged);
                }
                catch
                {
                }
            }

            _automationElementDockTo = null;
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
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                    UnDock();
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// If the parent window moves, move us as well
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void onWindowPositionChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            positionWindow();
        }

        /// <summary>
        /// Positions the dockee to the left of the parent window
        /// </summary>
        private void positionWindow()
        {
            User32Interop.RECT windowRect;
            User32Interop.GetWindowRect(_windowHandleDockTo, out windowRect);

            var rect = new Rectangle(0, 0, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top);
            var point = new Point(windowRect.left, windowRect.top);

            Log.Debug("parent location: " + point);
            Log.Debug("parent rectangle: " + rect);

            int screenWidth = Screen.FromControl(_form).Bounds.Width;

            int left = Math.Max(windowRect.left, 0);
            if (left + _form.Width > screenWidth)
            {
                left = screenWidth - _form.Width;
            }

            _form.Left = left;
            int top = Math.Max(windowRect.top - _form.Height, 0);
            _form.Top = top;
        }
    }
}