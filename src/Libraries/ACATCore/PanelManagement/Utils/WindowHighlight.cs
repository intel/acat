using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Core.PanelManagement.Utils
{
    /// <summary>
    /// Highlights a window by drawing a border around it.  If the window
    /// is moved, tracks the window and redraws the rectangle
    /// </summary>
    public class WindowHighlight : IDisposable
    {
        private static readonly ILogger<WindowHighlight> _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<WindowHighlight>();

        /// <summary>
        /// Scanner form.
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly object _sync = new();

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
                _logger.LogError(ex, ex.Message);
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
                _logger.LogTrace("");

                if (disposing)
                {
                    // dispose all managed resources.
                    _logger.LogTrace("");

                    stopTimer();

                    if (_outlineWindow != null)
                    {
                        try
                        {
                            _logger.LogDebug("Disposing highlight overlay window");
                            _outlineWindow.Dispose();
                            _outlineWindow = null;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, ex.Message);
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

        // ...

        private void highlightWindow(AutomationElement focusedElement)
        {
            _logger.LogTrace("");
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
                                // Convert System.Windows.Rect to System.Drawing.Rectangle
                                var rect = focusedElement.Current.BoundingRectangle;
                                var drawingRect = new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
                                _outlineWindow.Draw(drawingRect, 6);
                            }
                        }
                        catch (Exception exp)
                        {
                            _logger.LogError(exp, exp.Message);
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
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
            _timer?.Stop();
        }
    }
}