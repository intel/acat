using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Docks a scanner to a specified parent window at a specified
    /// relative position.
    /// </summary>
    public class DockScanner : IDisposable
    {
        /// <summary>
        /// If docking to a window, relative position of the dock
        /// </summary>
        private readonly Windows.WindowPosition _dockPosition;

        /// <summary>
        /// The form that should be docked to the window
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// Handle of the window to dock to
        /// </summary>
        private readonly IntPtr _windowHandleDockTo;

        /// <summary>
        /// UI automation element represnting the window to dock to
        /// </summary>
        private AutomationElement _automationElementDockTo;

        /// <summary>
        /// Have we been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="windowHandleDockTo">handle of window to dock to</param>
        /// <param name="form">the dockee</param>
        /// <param name="dockPosition">Relative position of dock</param>
        public DockScanner(IntPtr windowHandleDockTo, Form form, Windows.WindowPosition dockPosition)
        {
            _windowHandleDockTo = windowHandleDockTo;
            _form = form;
            _automationElementDockTo = null;
            _dockPosition = dockPosition;
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
        /// Handles docking of the form to the center of the parent window
        /// </summary>
        private void handleDockCenter()
        {
            User32Interop.RECT windowRect;
            User32Interop.GetWindowRect(_windowHandleDockTo, out windowRect);

            switch (_dockPosition)
            {
                case Windows.WindowPosition.TopCenter:
                    _form.Top = windowRect.top;
                    break;

                case Windows.WindowPosition.BottomCenter:
                    _form.Top = windowRect.bottom - _form.Height;
                    break;
            }
        }

        /// <summary>
        /// Handles docking of the form to the left of the parent window
        /// </summary>
        private void handleDockLeft()
        {
            User32Interop.RECT windowRect;
            User32Interop.GetWindowRect(_windowHandleDockTo, out windowRect);

            int screenLeft = Screen.FromControl(_form).Bounds.Left;
            int parentWidth = windowRect.right - windowRect.left;
            int parentHeight = windowRect.bottom - windowRect.top;
            int spaceLeftHoriz = windowRect.left - screenLeft;

            var control = Control.FromHandle(_windowHandleDockTo);
            if (control == null || control is Form)
            {
                if (spaceLeftHoriz < _form.Width)
                {
                    var parentFormLeft = _form.Width;
                    User32Interop.MoveWindow(_windowHandleDockTo, parentFormLeft, windowRect.top, parentWidth, parentHeight, true);
                    User32Interop.GetWindowRect(_windowHandleDockTo, out windowRect);
                }
            }

            _form.Left = windowRect.left - _form.Width;

            switch (_dockPosition)
            {
                case Windows.WindowPosition.TopLeft:
                    _form.Top = windowRect.top;
                    break;

                case Windows.WindowPosition.MiddleLeft:
                    _form.Top = windowRect.top + (parentHeight - _form.Height) / 2;
                    break;

                case Windows.WindowPosition.BottomLeft:
                    _form.Top = windowRect.bottom - _form.Height;
                    break;
            }
        }

        /// <summary>
        /// Handles docking of the form to the right of the parent window
        /// </summary>
        private void handleDockRight()
        {
            User32Interop.RECT windowRect;
            User32Interop.GetWindowRect(_windowHandleDockTo, out windowRect);

            int screenWidth = Screen.FromControl(_form).Bounds.Width;
            int parentWidth = windowRect.right - windowRect.left;
            int parentHeight = windowRect.bottom - windowRect.top;
            int spaceLeftHoriz = screenWidth - windowRect.right;

            var control = Control.FromHandle(_windowHandleDockTo);
            if (control == null || control is Form)
            {
                if (spaceLeftHoriz < _form.Width)
                {
                    var parentFormLeft = (screenWidth - _form.Width - parentWidth);
                    User32Interop.MoveWindow(_windowHandleDockTo, parentFormLeft, windowRect.top, parentWidth, parentHeight, true);
                    User32Interop.GetWindowRect(_windowHandleDockTo, out windowRect);
                }
            }

            _form.Left = windowRect.right;

            switch (_dockPosition)
            {
                case Windows.WindowPosition.TopRight:
                    _form.Top = windowRect.top;
                    break;

                case Windows.WindowPosition.MiddleRight:
                    _form.Top = windowRect.top + (parentHeight - _form.Height) / 2;
                    break;

                case Windows.WindowPosition.BottomRight:
                    _form.Top = windowRect.bottom - _form.Height;
                    break;
            }
        }

        /// <summary>
        /// If the parent window moves, move the scanner as well so it
        /// always stays docked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void onWindowPositionChanged(object sender, AutomationPropertyChangedEventArgs e)
        {
            positionWindow();
        }

        /// <summary>
        /// Positions the dockee relative to the parent window at the specified
        /// dock position
        /// </summary>
        private void positionWindow()
        {
            switch (_dockPosition)
            {
                case Windows.WindowPosition.TopRight:
                case Windows.WindowPosition.MiddleRight:
                case Windows.WindowPosition.BottomRight:
                    handleDockRight();
                    break;

                case Windows.WindowPosition.TopLeft:
                case Windows.WindowPosition.MiddleLeft:
                case Windows.WindowPosition.BottomLeft:
                    handleDockLeft();
                    break;

                case Windows.WindowPosition.BottomCenter:
                case Windows.WindowPosition.TopCenter:
                    handleDockCenter();
                    break;
            }

            int screenHeight = Screen.FromControl(_form).Bounds.Height;
            if ((_form.Top + _form.Height) > screenHeight)
            {
                _form.Top = screenHeight - _form.Height;
            }

            if (_form.Top < 0)
            {
                _form.Top = 0;
            }

            if (_form.Left < 0)
            {
                _form.Left = 0;
            }
        }
    }
}