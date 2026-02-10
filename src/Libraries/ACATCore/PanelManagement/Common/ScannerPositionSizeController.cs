////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement.Utils;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Core.PanelManagement.Common
{
    /// <summary>
    /// Represents a controller that controls the position
    /// and size of the scanner. Scanner sizes can be scaled
    /// up/down depending on a scale-factor.  Also the scanner
    /// can be positioned in one of the pre-defined spots on
    /// the display.
    /// </summary>
    public class ScannerPositionSizeController
    {
        private readonly ILogger<ScannerPositionSizeController> _logger;

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
        /// Should the scanner be docked to another window?
        /// </summary>
        private bool _dock;

        /// <summary>
        /// The DockScanner object that handles docking of the
        /// scanner to another window
        /// </summary>
        private DockScanner _dockScanner;

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
        /// Size of the scanner before resizing commenses
        /// </summary>
        private Size _resizeBeginSize;

        /// <summary>
        /// Root widget that represents the scanner form
        /// </summary>
        private Widget _rootWidget;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="scannerCommon">ScannerForm object associated with the form</param>
        internal ScannerPositionSizeController(ScannerCommon scannerCommon, ILogger<ScannerPositionSizeController> logger = null)
        {
            _logger = logger;
            AutoPosition = true;
            ManualPosition = Context.AppWindowPosition;
            _prevAutoPositionScannerValue = AutoPosition;
            ScaleFactor = ScaleFactorInit;
            _scannerCommon = scannerCommon;
            _form = scannerCommon.ScannerForm;
            _dock = false;
            _form.LocationChanged += FormOnLocationChanged;
            _form.ResizeEnd += _form_ResizeEnd;
            _form.ResizeBegin += FormOnResizeBegin;
            _form.Shown += _form_Shown;
        }

        /// <summary>
        /// Event raised when reposition scanner stops repositioning
        /// </summary>
        public event EventHandler EvtAutoRepositionScannerStop;

        public enum ResizeType
        {
            None,
            Vertical,
            Horizontal,
            Both
        }

        /// <summary>
        /// Gets or sets whether the position of the scanner is in
        /// the default position or whether the caller is going to
        /// manually position the scanner.
        /// </summary>
        public bool AutoPosition { get; set; }

        /// <summary>
        /// Gets or sets the manual position of the scanner.  If AutoPosition
        /// is Off then, the scanner is positioned in the ManualPosition spot
        /// on the display.
        /// </summary>
        public Windows.WindowPosition ManualPosition { get; set; }

        public ResizeType ResizeToFitDesktop { get; set; }

        /// <summary>
        /// Gets or sets the scaler factor
        /// </summary>
        public float ScaleFactor { get; set; }

        /// <summary>
        /// Starts timer-based auto-positioning of the scanner. On
        /// each tick, the scanner is positioned in the next corner
        /// </summary>
        public void AutoRepositionScannerStart()
        {
            unsubscribeAndDisposeAutoPositionScanner();
            _autoPositionScanner = new AutoPositionScanner(_form);
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
            Windows.SetWindowPosition(_form, AutoPosition ? Context.AppWindowPosition : ManualPosition);
        }

        /// <summary>
        /// Docks the scanner to another window (the parent parameter)
        /// </summary>
        /// <param name="parent">window to dock to</param>
        /// <param name="position">relative position of docking</param>
        public void DockScanner(IntPtr parent, Windows.WindowPosition position)
        {
            _dock = true;
            AutoPosition = false;

            _dockScanner = new DockScanner(parent, _form, position);
            if (Windows.GetVisible(_form))
            {
                _dockScanner.Dock();
            }
        }

        /// <summary>
        /// Initializes state variables
        /// </summary>
        public void Initialize()
        {
            _rootWidget = _scannerCommon.RootWidget;
        }

        /// <summary>
        /// Call this in the OnFormClosing override in the scanner
        /// </summary>
        public void OnClosing()
        {
            unsubscribeAndDisposeAutoPositionScanner();

            if (_dock && _dockScanner != null)
            {
                _dockScanner.Dispose();
                _dock = false;
            }
        }

        /// <summary>
        /// Call this in the load event handler for the form
        /// </summary>
        public void OnLoad()
        {
            _originalSize = _form.Size;
        }

        /// <summary>
        /// Restores default size and position of the scanner.
        /// </summary>
        public void RestoreDefaults()
        {
            ScaleFactor = 1.0f;
            ScaleForm(ScaleFactor);
            AutoPosition = true;
            Context.AppWindowPosition = Windows.WindowPosition.CenterScreen;
            Windows.SetWindowPositionAndNotify(_form, Context.AppWindowPosition);
        }

        /// <summary>
        /// Saves the current scale factor setting
        /// </summary>
        public void SaveScaleSetting(SystemPreferences prefs)
        {
            _logger?.LogDebug("Saving scale factor: {ScaleFactor}", ScaleFactor);
            prefs.ScannerScaleFactor = CoreGlobals.AppPreferences.ScannerScaleFactor = Convert.ToInt16(ScaleFactor * IntMultiplier);
            prefs.Save();
            CoreGlobals.AppPreferences.NotifyPreferencesChanged();
            _logger?.LogDebug("Scale factor saved: {SavedScaleFactor}", prefs.ScannerScaleFactor);
        }

        /// <summary>
        /// Saves the current position and size to the preferences file
        /// </summary>
        public void SaveSettings(SystemPreferences prefs)
        {
            _logger?.LogDebug("Saving scale factor: {ScaleFactor}", ScaleFactor);
            prefs.ScannerScaleFactor = CoreGlobals.AppPreferences.ScannerScaleFactor = Convert.ToInt16(ScaleFactor * IntMultiplier);
            _logger?.LogDebug("Saving window position: {WindowPosition}", Context.AppWindowPosition);
            prefs.ScannerPosition = CoreGlobals.AppPreferences.ScannerPosition = Context.AppWindowPosition;
            prefs.Save();

            AutoPosition = true;

            CoreGlobals.AppPreferences.NotifyPreferencesChanged();
            _logger?.LogDebug("Scale factor saved: {SavedScaleFactor}", prefs.ScannerScaleFactor);
        }

        /// <summary>
        /// Sets the default scale factor
        /// </summary>
        public void ScaleDefault()
        {
            ScaleFactor = 1.0f;
            ScaleForm(ScaleFactor);
            SetPositionAndNotify();
        }

        /// <summary>
        /// Scales the size of the form down by ScaleFactorAmount
        /// </summary>
        public void ScaleDown()
        {
            _logger?.LogDebug("Scaling down - current scale factor: {ScaleFactor}, minimum: {Minimum}", ScaleFactor, ScaleFactorMinimum);
            if (ScaleFactor > ScaleFactorMinimum)
            {
                ScaleFactor -= ScaleFactorAmount;
                ScaleForm(ScaleFactor);
                SetPositionAndNotify();
            }
        }

        /// <summary>
        /// Sets the size of the form based on the scale factor
        /// </summary>
        public void ScaleForm()
        {
            if (ResizeToFitDesktop != ResizeType.None)
            {
                resizeScannerToFitDesktop(CoreGlobals.AppPreferences.ScannerScaleFactor / IntMultiplier);
            }
            else
            {
                ScaleFactor = CoreGlobals.AppPreferences.ScannerScaleFactor / IntMultiplier;
                ScaleForm(ScaleFactor);
            }
        }

        /// <summary>
        /// Scales the scanner to indicated scale factor
        /// </summary>
        /// <param name="scaleFactor">the scale factor</param>
        public void ScaleForm(float scaleFactor)
        {
            _logger?.LogDebug("Scaling form - scale factor: {ScaleFactor}", scaleFactor);

            var newSize = new Size(Convert.ToInt32(_originalSize.Width * scaleFactor), Convert.ToInt32(_originalSize.Height * scaleFactor));

            _logger?.LogDebug("Form: {FormName}, scale factor: {ScaleFactor}, original width: {OrigWidth}, new width: {NewWidth}, original height: {OrigHeight}, new height: {NewHeight}",
                _form.Name, scaleFactor, _originalSize.Width, newSize.Width, _originalSize.Height, newSize.Height);

            //_rootWidget.Dump();

            _rootWidget.SetScaleFactor(scaleFactor);
            _ = Screen.PrimaryScreen.WorkingArea.Height;

            _form.Size = newSize;

            _logger?.LogDebug("Form scaling complete");
        }

        /// <summary>
        /// Scales the size of the form up by ScaleFactorAmount
        /// </summary>
        public void ScaleUp()
        {
            _logger?.LogDebug("Scaling up - current scale factor: {ScaleFactor}, maximum: {Maximum}", ScaleFactor, ScaleFactorMaximum);
            if (ScaleFactor < ScaleFactorMaximum)
            {
                ScaleFactor += ScaleFactorAmount;
                ScaleForm(ScaleFactor);
                SetPositionAndNotify();
            }

            _logger?.LogDebug("Scale up complete");
        }

        /// <summary>
        /// Sets the position of the scanner and raises event to notify
        /// that the position has changed
        /// </summary>
        public void SetPositionAndNotify()
        {
            Windows.SetWindowPositionAndNotify(_form, AutoPosition ? Context.AppWindowPosition : ManualPosition);
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

            EvtAutoRepositionScannerStop?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Form resizing ends. Adjust the height/width
        /// to maintain the aspect ratio to the one set at
        /// design time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _form_ResizeEnd(object sender, EventArgs e)
        {
            _logger?.LogDebug("Form resize end - form width: {Width}, original width: {OriginalWidth}", _form.Width, _originalSize.Width);

            float aspectRatio = (float)_originalSize.Width / _originalSize.Height;
            float scaleFactor = 0.0f;

            if (_form.Width != _resizeBeginSize.Width)
            {
                _form.Height = (int)(_form.Width / aspectRatio);

                scaleFactor = (float)_form.Width / _originalSize.Width;
            }
            else if (_form.Height != _resizeBeginSize.Height)
            {
                _form.Width = (int)(_form.Height * aspectRatio);

                scaleFactor = (float)_form.Height / _originalSize.Height;
            }

            if (scaleFactor != 0.0f)
            {
                ScaleForm(scaleFactor);
            }
        }

        /// <summary>
        /// Event handler for when the scanner is shown. Dock it if
        /// necesary otherwise just position it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _form_Shown(object sender, EventArgs e)
        {
            if (_dock && _dockScanner != null)
            {
                _dockScanner.Dock();
            }
            else
            {
                SetPositionAndNotify();
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
        /// User started to resize the form. Stores the
        /// initial size of the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void FormOnResizeBegin(object sender, EventArgs eventArgs)
        {
            _resizeBeginSize = _form.Size;
        }

        private void resizeScannerToFitDesktop(float scaleFactor)
        {
            _form.Invoke(new MethodInvoker(delegate
            {
                _logger?.LogDebug("Resizing scanner to fit desktop - scale factor: {ScaleFactor}", scaleFactor);

                var newSize = new Size(Convert.ToInt32(_originalSize.Width * scaleFactor), Convert.ToInt32(_originalSize.Height * scaleFactor));

                _rootWidget.SetScaleFactor(scaleFactor);

                switch (ResizeToFitDesktop)
                {
                    case ResizeType.Vertical:

                        int desktopHeight = Screen.PrimaryScreen.WorkingArea.Height;
                        if (newSize.Height > desktopHeight)
                            newSize.Height = desktopHeight;

                        float ratio = (float)newSize.Width / newSize.Height;

                        newSize = new Size((int)(desktopHeight * ratio), desktopHeight);
                        _form.Size = newSize;

                        _form.Top = 0;

                        //Windows.WidestScannerWidth = Width;
                        switch (Context.AppWindowPosition)
                        {
                            case Windows.WindowPosition.TopRight:
                            case Windows.WindowPosition.MiddleRight:
                            case Windows.WindowPosition.BottomRight:

                                _form.Left = Screen.PrimaryScreen.WorkingArea.Width - _form.Width;
                                break;

                            case Windows.WindowPosition.MiddleLeft:
                            case Windows.WindowPosition.TopLeft:
                            case Windows.WindowPosition.BottomLeft:
                                _form.Left = 0;
                                break;

                            case Windows.WindowPosition.TopCenter:
                            case Windows.WindowPosition.BottomCenter:
                                _form.Left = (Screen.PrimaryScreen.WorkingArea.Width - _form.Width) / 2;
                                break;
                        }
                        break;
                }
            }));
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