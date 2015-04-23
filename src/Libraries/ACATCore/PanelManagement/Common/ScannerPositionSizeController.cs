////////////////////////////////////////////////////////////////////////////
// <copyright file="ScannerPositionSizeController.cs" company="Intel Corporation">
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
using System.Windows.Forms;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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
    /// Reprents a controller that controls the position
    /// and size of the scanner. Scanner sizes can be scaled
    /// up/down depending on a scale-factor.  Also the scanner
    /// can be positioned in one of the four corners of the
    /// primary display.
    /// </summary>
    public class ScannerPositionSizeController
    {
        /// <summary>
        /// Multiplier to calculate the scanner size based
        /// on the scale factor
        /// </summary>
        public const int IntMultiplier = 10;

        /// <summary>
        /// How much to increment/decrement the scale factor by?
        /// </summary>
        public const float ScaleFactorAmount = 0.1f;

        /// <summary>
        /// Default scale factor
        /// </summary>
        public const float ScaleFactorInit = 10.0f;

        /// <summary>
        /// The maximum scale factor. Bounds the largest
        /// size of the scanner
        /// </summary>
        public const float ScaleFactorMaximum = 3.50f;

        /// <summary>
        /// The minimum scale factor. Bounds the smallest
        /// size of the scanner
        /// </summary>
        public const float ScaleFactorMinimum = 0.50f;

        /// <summary>
        /// The scanner form being resized
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// The ScannerCommon object associated with the form
        /// </summary>
        private readonly ScannerCommon _scannerCommon;

        /// <summary>
        /// Object the controls auto-positioning of the scanner
        /// </summary>
        private AutoPositionScanner _autoPositionScanner;

        /// <summary>
        /// Original size of the form.  The form will be scaled up
        /// or down depending on user setting
        /// </summary>
        private Size _originalSize;

        /// <summary>
        /// Previous value of autoposition scanner
        /// </summary>
        private bool _prevAutoPositionScannerValue;

        /// <summary>
        /// Root widget that represents the scanner form
        /// </summary>
        private Widget _rootWidget;

        private bool _savePositionOnAutoPositionStop;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="scannerCommon">ScannerForm object associated with the form</param>
        internal ScannerPositionSizeController(ScannerCommon scannerCommon)
        {
            AutoPosition = true;
            _prevAutoPositionScannerValue = AutoPosition;
            ScaleFactor = ScaleFactorInit;
            _scannerCommon = scannerCommon;
            _form = scannerCommon.ScannerForm;
            _form.LocationChanged += FormOnLocationChanged;
        }

        /// <summary>
        /// Event raised when reposition scanner stops repositioning
        /// </summary>
        public event EventHandler EvtAutoRepositionScannerStop;

        /// <summary>
        ///  Whether to position the screen app controlled or by previous
        ///  setting
        /// </summary>
        public bool AutoPosition { get; set; }

        /// <summary>
        /// Gets or sets the scaler factor
        /// </summary>
        public float ScaleFactor { get; set; }

        /// <summary>
        /// Starts timer-based auto-positioning of the scanner. On
        /// each tick, the scanner is positioned in the next corner
        /// </summary>
        public void AutoRepositionScannerStart(bool savePositionOnStop = true)
        {
            unsubscribeAndDisposeAutoPositionScanner();
            _savePositionOnAutoPositionStop = savePositionOnStop;
            _autoPositionScanner = new AutoPositionScanner(_form);
            _scannerCommon.StatusBarController.UpdateLockStatus(false);
            _autoPositionScanner.EvtAutoPostionScannerStopped += _autoPostionScanner_EvtAutoPostionScannerStopped;
            _prevAutoPositionScannerValue = AutoPosition;
            AutoPosition = false;
            _autoPositionScanner.Start();
        }

        /// <summary>
        /// Auto-positions the scanner to the default position stored
        /// in the application context
        /// </summary>
        public void AutoSetPosition()
        {
            if (AutoPosition)
            {
                Windows.SetWindowPosition(_form, Context.AppWindowPosition);
            }
        }

        /// <summary>
        /// Sets the position of the scanner and raises event to notify
        /// that the position has changed
        /// </summary>
        public void AutoSetPositionAndNotify()
        {
            if (AutoPosition)
            {
                Windows.SetWindowPositionAndNotify(_form, Context.AppWindowPosition);
            }
        }

        /// <summary>
        /// Initializes state variables
        /// </summary>
        public void Initialize()
        {
            _rootWidget = _scannerCommon.GetRootWidget();
        }

        /// <summary>
        /// Call this in the OnFormClosing override in the scanner
        /// </summary>
        public void OnClosing()
        {
            unsubscribeAndDisposeAutoPositionScanner();
        }

        /// <summary>
        /// Call this in the load event handler for the form
        /// </summary>
        public void OnLoad()
        {
            _originalSize = _form.Size;
        }

        /// <summary>
        /// Restore default size and position of the scanner.
        /// </summary>
        public void RestoreDefaults()
        {
            Log.Debug("Enter");
            ScaleFactor = 1.0f;
            ScaleForm(ScaleFactor);
            Windows.SetWindowPositionAndNotify(_form, Windows.WindowPosition.TopRight);
            Log.Debug("Exit");
        }

        /// <summary>
        /// Saves the current position and size to the preferences file
        /// </summary>
        public void SaveSettings()
        {
            Log.Debug("Enter");
            Log.Debug("saving scale factor. _scaleFactor=" + ScaleFactor);
            CoreGlobals.AppPreferences.ScannerScaleFactor = Convert.ToInt16(ScaleFactor * IntMultiplier);
            Log.Debug("Saving window position as " + Context.AppWindowPosition);
            CoreGlobals.AppPreferences.ScannerPosition = Context.AppWindowPosition;
            CoreGlobals.AppPreferences.Save();
            Log.Debug("scale factor saved is:" + CoreGlobals.AppPreferences.ScannerScaleFactor);
            Log.Debug("Exit");
        }

        /// <summary>
        /// Scales the size of the form down by ScaleFactorAmount
        /// </summary>
        public void ScaleDown()
        {
            Log.Debug("Enter");
            Log.Debug("scaling down. _scaleFactor=" + ScaleFactor + " SCALE_FACTOR_MINIMUM=" + ScaleFactorMinimum);
            if (ScaleFactor > ScaleFactorMinimum)
            {
                ScaleFactor = ScaleFactor - ScaleFactorAmount;
                ScaleForm(ScaleFactor);
                Windows.SetWindowPositionAndNotify(_form, Context.AppWindowPosition);
            }

            Log.Debug("Exit");
        }

        /// <summary>
        /// Sets the size of the form based on the scale factor
        /// </summary>
        public void ScaleForm()
        {
            ScaleFactor = CoreGlobals.AppPreferences.ScannerScaleFactor / (float)IntMultiplier;
            ScaleForm(ScaleFactor);
        }

        /// <summary>
        /// Scales the scanner to indicated scale factor
        /// </summary>
        /// <param name="scaleFactor">the scale factor</param>
        public void ScaleForm(float scaleFactor)
        {
            Log.Debug("Enter");
            _form.Invoke(new MethodInvoker(delegate
            {
                Log.Debug("scaleFactor: " + scaleFactor);

                _form.SuspendLayout();

                var newSize = new Size(Convert.ToInt32(_originalSize.Width * scaleFactor), Convert.ToInt32(_originalSize.Height * scaleFactor));

                Log.Debug("scalefactor: " + scaleFactor + " newFormWidth: " + newSize.Width);
                _rootWidget.Dump();
                _rootWidget.SetScaleFactor(scaleFactor);
                _form.Size = newSize;
                _form.Invalidate();
                _form.ResumeLayout();
            }));
            Log.Debug("Exit");
        }

        /// <summary>
        /// Scales the size of the form up by ScaleFactorAmount
        /// </summary>
        public void ScaleUp()
        {
            Log.Debug("Enter");
            Log.Debug("scaling up. _scaleFactor=" + ScaleFactor + " SCALE_FACTOR_MAXIMUM=" + ScaleFactorMaximum);
            if (ScaleFactor < ScaleFactorMaximum)
            {
                ScaleFactor = ScaleFactor + ScaleFactorAmount;
                ScaleForm(ScaleFactor);
                Windows.SetWindowPositionAndNotify(_form, Context.AppWindowPosition);
            }

            Log.Debug("Exit");
        }

        /// <summary>
        /// Event handler for the event raised when the autoposition
        /// timer stops
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _autoPostionScanner_EvtAutoPostionScannerStopped(object sender, EventArgs e)
        {
            AutoPosition = _prevAutoPositionScannerValue;
            if (_savePositionOnAutoPositionStop)
            {
                CoreGlobals.AppPreferences.ScannerPosition = Context.AppWindowPosition;
                AuditLog.Audit(new AuditEvent("ScannerPositionChange", Context.AppWindowPosition.ToString()));
                CoreGlobals.AppPreferences.Save();
            }

            _scannerCommon.StatusBarController.UpdateLockStatus(CoreGlobals.AppPreferences.AutoSaveScannerLastPosition);

            if (EvtAutoRepositionScannerStop != null)
            {
                EvtAutoRepositionScannerStop(this, new EventArgs());
            }
        }

        /// <summary>
        /// Location of scanner changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event argument</param>
        private void FormOnLocationChanged(object sender, EventArgs eventArgs)
        {
            if (AutoPosition)
            {
                AutoSetPosition();
            }
        }

        /// <summary>
        /// Disposes autoposition object
        /// </summary>
        private void unsubscribeAndDisposeAutoPositionScanner()
        {
            if (_autoPositionScanner != null)
            {
                _autoPositionScanner.EvtAutoPostionScannerStopped -= _autoPostionScanner_EvtAutoPostionScannerStopped;
                _autoPositionScanner.Dispose();
                _autoPositionScanner = null;
            }
        }
    }
}