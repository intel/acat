////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowManager.cs" company="Intel Corporation">
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

//#define TALKWINDOW_DISPATCHER_THREAD

using ACAT.ACATResources;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ACAT.Lib.Core.TalkWindowManagement
{
    /// <summary>
    /// Manages the talk window that is used to converse.  The user types
    /// text into the talk window and then the text is converted to speech
    /// on user's request. This is a singleton class
    /// </summary>
    public class TalkWindowManager : IDisposable
    {
        /// <summary>
        /// On Win8, having the talk window docked exactly with the scanner
        /// causes problems of overlap where the scanner and talk window compete
        /// to stay on top causing flicker.  Let's leave a gap between the two
        /// </summary>
        private const int GapFromScanner = 15;

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static TalkWindowManager _instance;

        /// <summary>
        /// Design time height of the talk window form
        /// </summary>
        private int _designHeight;

        /// <summary>
        /// Design time width of the talk window form
        /// </summary>
        private int _designWidth;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Talk window font size
        /// </summary>
        private float _fontSize;

#if TALKWINDOW_DISPATCHER_THREAD
    /// <summary>
    /// Is inside execution of the creation thread
    /// </summary>
        private volatile bool _inTalkWindowCreationThread;
#endif

        /// <summary>
        /// Is execting the toggle talk window function
        /// </summary>
        private volatile bool _inToggleTalkWindow;

        /// <summary>
        /// The talk window object
        /// </summary>
        private ITalkWindow _talkWindow;

        /// <summary>
        /// The talk window form
        /// </summary>
        private Form _talkWindowForm;

        /// <summary>
        /// Text in the talk window.
        /// </summary>
        private String _talkWindowText = String.Empty;

        /// <summary>
        /// Was the talk window empty of text during zoom mode entry?
        /// </summary>
        private bool _zoomModeTalkWindowEmpty;

        /// <summary>
        /// In the process of creating/displaying the talk window
        /// </summary>
        private volatile bool _showingTalkWindow;

        /// <summary>
        /// In the process of closing the talk window
        /// </summary>
        private volatile bool _closingTalkWindow;

        /// <summary>
        /// Initializes singleton instance of the manager
        /// </summary>
        private TalkWindowManager()
        {
            _fontSize = CoreGlobals.AppPreferences.TalkWindowFontSize;
            PanelManager.Instance.EvtScannerShow += Instance_EvtScannerShow;
            AgentManagement.AgentManager.Instance.EvtPreActivateAgent += Instance_EvtPreActivateAgent;
        }

        /// <summary>
        /// Used to indicate that the talk window was created.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public delegate void TalkWindowCreated(object sender, TalkWindowCreatedEventArgs e);

        /// <summary>
        /// Used to indicate the change in the visibility state of the talk window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        public delegate void TalkWindowVisibilityChanged(object sender, TalkWindowVisibilityChangedEventArgs e);

        /// <summary>
        /// Event raised when the talk window is cleared
        /// </summary>
        public event EventHandler EvtTalkWindowCleared;

        /// <summary>
        /// Event raised when talk window is closed
        /// </summary>
        public event EventHandler EvtTalkWindowClosed;

        /// <summary>
        /// Event raised when the talk window is created.
        /// </summary>
        public event TalkWindowCreated EvtTalkWindowCreated;

        /// <summary>
        /// Event raised when the talk window visibility changes
        /// </summary>
        public event TalkWindowVisibilityChanged EvtTalkWindowVisibilityChanged;

        /// <summary>
        /// Gets the singleton instance of the manager
        /// </summary>
        public static TalkWindowManager Instance
        {
            get { return _instance ?? (_instance = new TalkWindowManager()); }
        }

        /// <summary>
        /// Gets or sets paused state of the talk window
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets the talk window state, whether it is currently active or not.
        /// This is different from visiblity.  The talk window may be active,
        /// just not visible
        /// </summary>
        public bool IsTalkWindowActive { get; private set; }

        /// <summary>
        /// Gets the visibility state of the talk window
        /// </summary>
        public bool IsTalkWindowVisible
        {
            get { return (IsTalkWindowActive && _talkWindowForm != null) && Windows.GetVisible(_talkWindowForm); }
        }

        /// <summary>
        /// Gets text of talk window
        /// </summary>
        public String TalkWindowText
        {
            get
            {
                var retVal = String.Empty;
                if (_talkWindow != null && IsTalkWindowVisible)
                {
                    retVal = _talkWindow.TalkWindowText;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Returns the text box used for communication
        /// </summary>
        public Control TalkWindowTextBox
        {
            get { return (_talkWindow != null) ? _talkWindow.TalkWindowTextBox : null; }
        }

        /// <summary>
        /// Clears the text in the talk window
        /// </summary>
        public void Clear()
        {
            if (_talkWindow != null && Windows.GetVisible(_talkWindowForm))
            {
                KeyStateTracker.ClearAll();
                _talkWindow.Clear();
                AuditLog.Audit(new AuditEventTalkWindow("clear"));
                if (EvtTalkWindowCleared != null)
                {
                    EvtTalkWindowCleared(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Closes the talk window.  Unsubscribe from events.  Set
        /// force to true to close the window even if it was not
        /// previously active
        /// </summary>
        /// <param name="force"></param>
        public void CloseTalkWindow(bool force = false)
        {
            if ((IsTalkWindowActive || force) && _talkWindow != null)
            {
                if (_showingTalkWindow)
                {
                    return;
                }

                _closingTalkWindow = true;

                IsPaused = false;

                _fontSize = _talkWindow.FontSize;

                Log.Debug("_fontsize: " + _fontSize);

                IsTalkWindowActive = false;

                _zoomModeTalkWindowEmpty = false;

                TTSManager.Instance.ActiveEngine.Stop();

                hideGlass();

                unsubscribeEvents();

                if (CoreGlobals.AppPreferences.RetainTalkWindowContentsOnHide)
                {
                    _talkWindowText = _talkWindow.TalkWindowText;
                }

                Log.Debug("Removing talkwindowagent");

                Context.AppAgentMgr.RemoveAgent(_talkWindowForm.Handle);

                Windows.CloseForm(_talkWindowForm);

                _talkWindowForm = null;
                _talkWindow = null;
                _closingTalkWindow = false;

                AuditLog.Audit(new AuditEventTalkWindow("close"));
            }
        }

        /// <summary>
        /// Copies talk window text to clipboard
        /// </summary>
        public void Copy()
        {
            if (_talkWindow != null && Windows.GetVisible(_talkWindowForm))
            {
                _talkWindow.Copy();
            }
        }

        /// <summary>
        /// Creates talk window.  Doesn't show the form though, just
        /// creates the object.
        /// </summary>
        /// <returns>true on success</returns>
        public bool CreateTalkWindow()
        {
            if (_talkWindowForm == null)
            {
                IsPaused = false;
                _talkWindowForm = Context.AppPanelManager.CreatePanel("TalkWindow");
                _talkWindow = _talkWindowForm as ITalkWindow;
                _talkWindowForm.FormClosed += _talkWindowForm_FormClosed;
                _talkWindow.FontSize = _fontSize;
                _fontSize = _talkWindow.FontSize;
                _talkWindowForm.TopMost = true;

                if (_designWidth == 0)
                {
                    _designWidth = _talkWindowForm.Width;
                }

                if (_designHeight == 0)
                {
                    _designHeight = _talkWindowForm.Height;
                }

                subscribeEvents();
            }
            else
            {
                Log.Debug("_TalkWindow is not null!!");
            }

            return true;
        }

        /// <summary>
        /// Gets the font size of the Talk window font
        /// </summary>
        public float FontSize
        {
            get { return _fontSize; }
        }

        /// <summary>
        /// Cuts text from the talk window into the clipboard
        /// </summary>
        public void Cut()
        {
            if (_talkWindow != null && Windows.GetVisible(_talkWindowForm))
            {
                _talkWindow.Cut();
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
        /// Returns the talk window form (can be null if not created)
        /// </summary>
        /// <returns>Talk window form</returns>
        public Form GetTalkWindow()
        {
            return _talkWindowForm;
        }

        /// <summary>
        /// Checks if the talk window has text or not
        /// </summary>
        /// <returns>true if empty</returns>
        public bool IsTalkWindowEmpty()
        {
            return TalkWindowText.Length == 0;
        }

        /// <summary>
        /// Checks if the talk window is the current foreground window
        /// </summary>
        /// <returns>true if so</returns>
        public bool IsTalkWindowForeground()
        {
            bool retVal;

            if (IsTalkWindowActive)
            {
                IntPtr handle = Windows.GetForegroundWindow();
                retVal = (handle == _talkWindowForm.Handle);
            }
            else
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Pastes clipboard into talk window
        /// </summary>
        public void Paste()
        {
            if (_talkWindow != null && _talkWindowForm.Visible)
            {
                _talkWindow.Paste();
            }
        }

        /// <summary>
        /// Pauses the talk window.  Makes the talk window invisible.
        /// Note that it is still active, only that the user cannot see it
        /// </summary>
        public void Pause()
        {
            if (_talkWindowForm != null)
            {
                IsPaused = true;
                hideGlass();
                Windows.SetVisible(_talkWindowForm, false);
            }
        }

        /// <summary>
        /// If a talk window was previously active, show it
        /// </summary>
        public void Resume()
        {
            if (IsTalkWindowActive && _talkWindowForm != null)
            {
                Windows.SetVisible(_talkWindowForm, true);
                showGlass();
                Windows.SetTopMost(_talkWindowForm);
            }

            IsPaused = false;
        }

        /// <summary>
        /// Selects all text in the talk window
        /// </summary>
        public void SelectAll()
        {
            if (_talkWindow != null && Windows.GetVisible(_talkWindowForm))
            {
                _talkWindow.SelectAll();
            }
        }

        /// <summary>
        /// Sets the talk window position relative to the currently actvie
        /// scanner
        /// </summary>
        /// <param name="scannerForm">the scanner</param>
        public void SetTalkWindowPosition(Form scannerForm)
        {
            Log.Debug("Entering...");

            if (_talkWindowForm == null || !_talkWindowForm.Visible)
            {
                return;
            }

            if (Context.AppPanelManager.PanelDisplayMode != DisplayModeTypes.Popup)
            {
                setTalkWindowPosition(scannerForm);
            }

            _talkWindowForm.Invoke(new MethodInvoker(() => _talkWindow.OnPositionChanged()));
        }

        /// <summary>
        /// Toggles the visibility talk window.
        /// </summary>
        public void ToggleTalkWindow()
        {
            Log.Debug("_inToggleTalkWindow: " + _inToggleTalkWindow);

            bool visibilityChanged = false;

            if (!_inToggleTalkWindow)
            {
                _inToggleTalkWindow = true;

                if (_talkWindowForm == null)
                {
                    Log.Debug("calling create and show");

                    if (!_closingTalkWindow)
                    {
                        _showingTalkWindow = true;
                        createAndShowTalkWindow();
                        _showingTalkWindow = false;

                        visibilityChanged = true;

                        Log.Debug("after create and show");
                    }
                }

                else if (Windows.GetVisible(_talkWindowForm))
                {
                    Log.Debug("closing talk window");
                    CloseTalkWindow();
                    visibilityChanged = true;
                }
                else
                {
                    if (!_closingTalkWindow)
                    {
                        _showingTalkWindow = true;

                        IsTalkWindowActive = true;

                        IsPaused = false;

                        showGlass();

                        Windows.SetTopMost(_talkWindowForm);

                        _talkWindowForm.Visible = true;

                        _showingTalkWindow = false;

                        visibilityChanged = true;

                    }
                }

                if (visibilityChanged)
                {
                    notifyTalkWindowVisibilityChanged();
                }

                _inToggleTalkWindow = false;
            }

            Log.Debug("returning");
        }

        /// <summary>
        /// Displays the talk window if it is not already active
        /// </summary>
        /// <returns>true on success</returns>
        public bool ShowTalkWindow()
        {
            if (_talkWindowForm == null || !Windows.GetVisible(_talkWindowForm))
            {
                ToggleTalkWindow();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Resets talk window text font size
        /// </summary>
        public void ZoomDefault()
        {
            if (_talkWindow != null && _talkWindowForm.Visible)
            {
                _talkWindow.ZoomDefault();
            }
        }

        /// <summary>
        /// Makes the talk window text larger
        /// </summary>
        public void ZoomIn()
        {
            if (_talkWindow != null && _talkWindowForm.Visible)
            {
                _talkWindow.ZoomIn();
            }
        }

        /// <summary>
        /// Enters zoom in/out mode. If the talk window is
        /// empty, uses a sample text so the user can check
        /// the font size
        /// </summary>
        public void ZoomModeEnter()
        {
            var text = _talkWindow.TalkWindowText;
            if (String.IsNullOrEmpty(text.Trim()))
            {
                _zoomModeTalkWindowEmpty = true;
                _talkWindow.TalkWindowText = R.GetString("TalkWindowTestString");
            }
        }

        /// <summary>
        /// Exits zoom in/out mode
        /// </summary>
        public void ZoomModeExit()
        {
            if (_zoomModeTalkWindowEmpty)
            {
                _talkWindow.Clear();
                _zoomModeTalkWindowEmpty = false;
            }
        }

        /// <summary>
        /// Makes the talk window text smaller
        /// </summary>
        public void ZoomOut()
        {
            if (_talkWindow != null && _talkWindowForm.Visible)
            {
                _talkWindow.ZoomOut();
            }
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
                    if (_talkWindowForm != null)
                    {
                        _talkWindowForm.Close();
                        _talkWindowForm = null;
                        _talkWindow = null;
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Talk window form closed. Notify subscribers and
        /// restore focus to the application window that at
        /// the top of the Z order
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _talkWindowForm_FormClosed(object sender, FormClosedEventArgs e)
        {
#if TALKWINDOW_DISPATCHER_THREAD

            Log.Debug("********* Calling EXiting all frames");
            System.Windows.Threading.Dispatcher.ExitAllFrames();
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Background);
#endif
            Log.Debug("appquit: " + Context.AppQuit);

            if (!Context.AppQuit)
            {
                if (EvtTalkWindowClosed != null)
                {
                    EvtTalkWindowClosed(this, new EventArgs());
                }

                //Context.AppPanelManager.CloseCurrentForm();
                EnumWindows.RestoreFocusToTopWindowOnDesktop();
                WindowActivityMonitor.GetActiveWindowAsync();
            }

            Log.Debug("Exiting");
        }

        /// <summary>
        /// Creates the a talk window form and show it. Restores talk window
        /// contents if configuredto do so.  Raises an event indicating the
        /// talk window was created.  Window creation is done in a separate
        /// thread with its own message loop
        /// </summary>
        private void createAndShowTalkWindow()
        {
#if TALKWINDOW_DISPATCHER_THREAD
            var viewerThread = new Thread(delegate()
            {
                if (!_inTalkWindowCreationThread)
                {
                    _inTalkWindowCreationThread = true;

                    // Create our context, and install it:
                    SynchronizationContext.SetSynchronizationContext(
                        new System.Windows.Threading.DispatcherSynchronizationContext(
                            System.Windows.Threading.Dispatcher.CurrentDispatcher));
#endif
            IsTalkWindowActive = true;

            CreateTalkWindow();

            showGlass();

            Windows.SetTopMost(_talkWindowForm);

            Form form = null;
            if (PanelManager.Instance.GetCurrentForm() != null)
            {
                form = PanelManager.Instance.GetCurrentForm() as Form;
            }

            if (form != null)
            {
                SetTalkWindowPosition(PanelManager.Instance.GetCurrentForm() as Form);
            }

            var talkWindowAgent = Context.AppAgentMgr.GetAgentByName("TalkWindow Agent");
            Log.IsNull("Talkwindowagent", talkWindowAgent);
            if (talkWindowAgent != null)
            {
                Context.AppAgentMgr.AddAgent(_talkWindowForm.Handle, talkWindowAgent);
                Log.Debug("Added talkwindowagent");
            }

            Windows.ShowForm(_talkWindowForm);

            Windows.ActivateForm(_talkWindowForm);

            AuditLog.Audit(new AuditEventTalkWindow("show"));

            if (CoreGlobals.AppPreferences.RetainTalkWindowContentsOnHide)
            {
                _talkWindow.TalkWindowText = _talkWindowText;
            }

            if (EvtTalkWindowCreated != null)
            {
                EvtTalkWindowCreated(this, new TalkWindowCreatedEventArgs(_talkWindowForm));
            }

#if TALKWINDOW_DISPATCHER_THREAD
                    System.Windows.Threading.Dispatcher.Run();
                    Log.Debug("Exited DISPATCHER.RUN");
                    _inTalkWindowCreationThread = false;
                }
            });

            viewerThread.SetApartmentState(ApartmentState.STA);
            Log.Debug("Starting thread, _inTalkWindowCreationThread is :  " + _inTalkWindowCreationThread);
            viewerThread.Start();
#endif
        }

        /// <summary>
        /// Handler for event raised when a scanner is displayed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Instance_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            SetTalkWindowPosition(arg.Scanner.Form);

            if (_talkWindowForm != null)
            {
                Windows.SetForegroundWindow(_talkWindowForm.Handle);
            }
        }

        /// <summary>
        /// Notifies change in visibility of talk window
        /// </summary>
        private void notifyTalkWindowVisibilityChanged()
        {
            if (EvtTalkWindowVisibilityChanged != null)
            {
                EvtTalkWindowVisibilityChanged(this, new TalkWindowVisibilityChangedEventArgs(IsTalkWindowActive));
            }
        }

        /// <summary>
        /// Sets the talk window position relative to the scanner.  Makes
        /// sure that the talk window is centered, and if the scanner is
        /// too big and the talk window cannot be centered, makes best effort
        /// to position it so there is no overlap with the scanner
        /// </summary>
        /// <param name="scannerForm"></param>
        private void setTalkWindowPosition(Form scannerForm)
        {
            Log.Debug("Entering...");

            if (_talkWindowForm == null)
            {
                return;
            }

            var scannerPosition = Windows.GetScannerPosition(scannerForm);

            int spaceLeft = Screen.PrimaryScreen.Bounds.Width - scannerForm.Width;

            var talkWindowRect = new Rectangle((Screen.PrimaryScreen.Bounds.Width - _designWidth) / 2,
                                                    (Screen.PrimaryScreen.Bounds.Height - _designHeight) / 2,
                                                    _designWidth,
                                                    _designHeight);

            var scannerRect = new Rectangle(scannerForm.Location.X,
                                            scannerForm.Location.Y,
                                            scannerForm.Size.Width,
                                            scannerForm.Size.Height);

            switch (scannerPosition)
            {
                case Windows.WindowPosition.BottomLeft:
                case Windows.WindowPosition.MiddleLeft:
                case Windows.WindowPosition.TopLeft:
                    int gap = 0;
                    if (talkWindowRect.IntersectsWith(scannerRect))
                    {
                        gap = GapFromScanner;
                        talkWindowRect.X = scannerRect.Right + GapFromScanner;
                    }

                    if (talkWindowRect.Right > Screen.PrimaryScreen.Bounds.Width)
                    {
                        talkWindowRect.Width = spaceLeft - gap;
                    }

                    break;

                case Windows.WindowPosition.TopRight:
                case Windows.WindowPosition.MiddleRight:
                case Windows.WindowPosition.BottomRight:

                    if (talkWindowRect.IntersectsWith(scannerRect))
                    {
                        talkWindowRect.X = scannerRect.X - talkWindowRect.Width - GapFromScanner;
                    }

                    if (talkWindowRect.X < 0)
                    {
                        talkWindowRect.X = 0;
                        talkWindowRect.Width = spaceLeft - GapFromScanner;
                    }

                    break;
            }

            _talkWindowForm.Location = new Point(talkWindowRect.X, talkWindowRect.Y);
            _talkWindowForm.Width = talkWindowRect.Width;

            if (CoreGlobals.AppPreferences.SnapTalkWindow)
            {
                _talkWindowForm.Height = Screen.PrimaryScreen.Bounds.Height;
                _talkWindowForm.Top = 0;
            }
        }

        /// <summary>
        /// Displays translucent glass behind the talk window
        /// </summary>
        private void showGlass()
        {
            //Glass.Enable = CoreGlobals.AppPreferences.EnableGlass;

            // permanently disable glass
            Glass.Enable = false;
            Glass.ShowGlass();
        }

        /// <summary>
        /// Hides the translucent glass
        /// </summary>
        private void hideGlass()
        {
            Glass.HideGlass();
        }

        /// <summary>
        /// Subscribes to talk window events
        /// </summary>
        private void subscribeEvents()
        {
            _talkWindowForm.VisibleChanged += talkWindowForm_VisibleChanged;
            _talkWindow.EvtRequestCloseTalkWindow += talkWindow_EvtRequestCloseTalkWindow;
            _talkWindow.EvtTalkWindowFontChanged += talkWindowForm_EvtFontChanged;
        }

        /// <summary>
        /// Event handler for talk window request to close.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void talkWindow_EvtRequestCloseTalkWindow(object sender, EventArgs e)
        {
            if (Windows.GetVisible(_talkWindowForm))
            {
                CloseTalkWindow();
            }
        }

        /// <summary>
        /// Handler for event when the talk window font changes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void talkWindowForm_EvtFontChanged(object sender, EventArgs e)
        {
            _fontSize = _talkWindow.FontSize;
        }

        /// <summary>
        /// Event handler for talk window visibility changed. If TTS is currently active,
        /// stop it.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void talkWindowForm_VisibleChanged(object sender, EventArgs e)
        {
            if (!_talkWindowForm.Visible)
            {
                TTSManager.Instance.ActiveEngine.Stop();
            }
        }

        /// <summary>
        /// Event handler for preactivation of a functional agent. Close
        /// the talk window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Instance_EvtPreActivateAgent(object sender, EventArgs e)
        {
            CloseTalkWindow();
        }

        /// <summary>
        /// Detaches from events previously subscribed to.
        /// </summary>
        private void unsubscribeEvents()
        {
            _talkWindow.EvtRequestCloseTalkWindow -= talkWindow_EvtRequestCloseTalkWindow;
        }
    }
}