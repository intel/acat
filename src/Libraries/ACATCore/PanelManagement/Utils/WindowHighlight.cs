using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Highlights a window by drawing a border around it.  If the window
    /// is moved, tracks the window and redraws the rectangle
    /// </summary>
    public class WindowHighlight : IDisposable
    {
        /// <summary>
        /// Scanner form.
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly object _sync = new object();

        /// <summary>
        /// Automation wrapper for the window
        /// </summary>
        private AutomationElement _automationElement;

        /// <summary>
        /// Has this been disposed yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Draws the outline
        /// </summary>
        private OutlineWindow _outlineWindow;

        /// <summary>
        /// Timer to track the window position
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="targetWindow">app window to highlight</param>
        /// <param name="form">Scanner form</param>
        public WindowHighlight(IntPtr targetWindow, Form form)
        {
            _form = form;
            _outlineWindow = new OutlineWindow(form);

            try
            {
                _automationElement = AutomationElement.FromHandle(targetWindow);
                startTimer();
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                _automationElement = null;
            }
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
                    Log.Debug();

                    stopTimer();

                    if (_outlineWindow != null)
                    {
                        try
                        {
                            Log.Debug("Disposing highlight overlay window");
                            _outlineWindow.Dispose();
                            _outlineWindow = null;
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(ex.ToString());
                        }
                    }

                    _automationElement = null;
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Timer tick that draws the rectangle
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            highlightWindow(_automationElement);
        }

        /// <summary>
        /// Highlights the window
        /// </summary>
        /// <param name="focusedElement">automation element of window</param>
        private void highlightWindow(AutomationElement focusedElement)
        {
            Log.Debug();
            try
            {
                lock (_sync)
                {
                    _form.Invoke(new MethodInvoker(delegate
                    {
                        try
                        {
                            if (focusedElement != null && _outlineWindow != null)
                            {
                                _outlineWindow.Draw(focusedElement.Current.BoundingRectangle, 6);
                            }
                        }
                        catch (Exception exp)
                        {
                            Log.Debug(exp.ToString());
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.ToString());
            }
        }

        /// <summary>
        /// Starts the timer to track the app window if it gets
        /// repositioned
        /// </summary>
        private void startTimer()
        {
            if (_timer == null)
            {
                _timer = new Timer { Interval = 100 };
                _timer.Tick += _timer_Tick;
            }

            _timer.Start();
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
    }
}