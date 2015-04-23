////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowManager.cs" company="Intel Corporation">
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
using System.Threading;
using System.Windows.Forms;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.TTSManagement;
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
        /// Singleton instance
        /// </summary>
        private static TalkWindowManager _instance;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Talk window font size
        /// </summary>
        private float _fontSize;

        /// <summary>
        /// Is inside execution of the creation thread
        /// </summary>
        private volatile bool _inTalkWindowCreationThread;

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
        /// Initializes singleton instance of the manager
        /// </summary>
        private TalkWindowManager()
        {
            _fontSize = CoreGlobals.AppPreferences.TalkWindowFontSize;
            PanelManagement.PanelManager.Instance.EvtScannerShow += Instance_EvtScannerShow;
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
            get
            {
                return (IsTalkWindowActive && _talkWindowForm != null) && Windows.GetVisible(_talkWindowForm);
            }
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
            get
            {
                return (_talkWindow != null) ? _talkWindow.TalkWindowTextBox : null;
            }
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
                IsPaused = false;

                _fontSize = _talkWindow.FontSize;

                Log.Debug("_fontsize: " + _fontSize);

                IsTalkWindowActive = false;

                _zoomModeTalkWindowEmpty = false;

                TTSManager.Instance.ActiveEngine.Stop();

                PanelManagement.Glass.HideGlass();

                unsubscribeEvents();

                if (CoreGlobals.AppPreferences.RetainTalkWindowContentsOnHide)
                {
                    _talkWindowText = _talkWindow.TalkWindowText;
                }

                Windows.CloseForm(_talkWindowForm);

                _talkWindowForm = null;
                _talkWindow = null;

                AuditLog.Audit(new AuditEventTalkWindow("close"));
            }
        }

        /// <summary>
        /// Copy talk window text to clipboard
        /// </summary>
        public void Copy()
        {
            if (_talkWindow != null && Windows.GetVisible(_talkWindowForm))
            {
                _talkWindow.Copy();
            }
        }

        /// <summary>
        /// Create talk window.  Doesn't show the form though, just
        /// creates the object.
        /// </summary>
        /// <returns>true on success</returns>
        public bool CreateTalkWindow()
        {
            if (_talkWindowForm == null)
            {
                IsPaused = false;
                _talkWindowForm = PanelManagement.Context.AppPanelManager.CreatePanel("TalkWindow");
                _talkWindow = _talkWindowForm as ITalkWindow;
                _talkWindowForm.FormClosed += _talkWindowForm_FormClosed;
                _talkWindow.FontSize = _fontSize;
                _fontSize = _talkWindow.FontSize;
                _talkWindowForm.TopMost = true;

                subscribeEvents();
            }
            else
            {
                Log.Debug("ACHTUNG! _TalkWindow is not null!!");
            }

            return true;
        }

        /// <summary>
        /// Cut text from the talk window
        /// </summary>
        public void Cut()
        {
            if (_talkWindow != null && Windows.GetVisible(_talkWindowForm))
            {
                _talkWindow.Cut();
            }
        }

        /// <summary>
        /// Dispose resources
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
        /// Paste clipboard into talk window
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
        /// Saves talk window settings
        /// </summary>
        public void SaveFont()
        {
            CoreGlobals.AppPreferences.TalkWindowFontSize = _fontSize;
            CoreGlobals.AppPreferences.Save();
        }

        /// <summary>
        /// Select all text in the talk window
        /// </summary>
        public void SelectAll()
        {
            if (_talkWindow != null && Windows.GetVisible(_talkWindowForm))
            {
                _talkWindow.SelectAll();
            }
        }

        /// <summary>
        /// Sets the talk window positin relative to the scanner
        /// </summary>
        /// <param name="scannerForm">the scanner</param>
        public void SetTalkWindowPosition(Form scannerForm)
        {
            Log.Debug("Entering...");

            if (_talkWindowForm == null || !Windows.GetVisible(_talkWindowForm))
            {
                return;
            }

            setTalkWindowPosition(scannerForm);

            _talkWindowForm.Invoke(new MethodInvoker(() => _talkWindow.OnPositionChanged()));
        }

        /// <summary>
        /// Toggles the visibility talk window.
        /// </summary>
        public void ToggleTalkWindow()
        {
            Log.Debug("1 " + _inToggleTalkWindow);
            if (!_inToggleTalkWindow)
            {
                Log.Debug("2");
                _inToggleTalkWindow = true;
                if (_talkWindowForm == null)
                {
                    Log.Debug("calling create and show");
                    createAndShowTalkWindow();
                    Log.Debug("after create and show");
                }
                else if (Windows.GetVisible(_talkWindowForm))
                {
                    Log.Debug("clsoing talk window");
                    CloseTalkWindow();
                }
                else
                {
                    IsTalkWindowActive = true;

                    showGlass();

                    Windows.SetTopMost(_talkWindowForm);

                    //notifyTalkWindowPreVisibilityChanged(_talkWindowActive);
                    _talkWindowForm.Visible = true;
                }

                Log.Debug("3");
                notifyTalkWindowVisibilityChanged();
                Log.Debug("4");
                _inToggleTalkWindow = false;
            }

            Log.Debug("returning");
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
                _talkWindow.TalkWindowText = "This is a sample";
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
            Log.Debug("********* Calling EXiting all frames");
            System.Windows.Threading.Dispatcher.ExitAllFrames();
            System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Background);

            Log.Debug("appquit: " + PanelManagement.Context.AppQuit);

            if (!PanelManagement.Context.AppQuit)
            {
                if (EvtTalkWindowClosed != null)
                {
                    EvtTalkWindowClosed(this, new EventArgs());
                }

                EnumWindows.RestoreFocusToTopWindow();
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
            var viewerThread = new Thread(delegate()
            {
                if (!_inTalkWindowCreationThread)
                {
                    _inTalkWindowCreationThread = true;

                    IsTalkWindowActive = true;

                    // Create our context, and install it:
                    SynchronizationContext.SetSynchronizationContext(
                        new System.Windows.Threading.DispatcherSynchronizationContext(
                            System.Windows.Threading.Dispatcher.CurrentDispatcher));
                    
                    CreateTalkWindow();
                    
                    showGlass();
                    

                    //notifyTalkWindowPreVisibilityChanged(true);

                    Windows.SetTopMost(_talkWindowForm);
                    
                    Form form = null;
                    if (PanelManagement.PanelManager.Instance.GetCurrentForm() != null)
                    {
                        form = PanelManagement.PanelManager.Instance.GetCurrentForm() as Form;
                    }

                    if (form != null)
                    {
                        SetTalkWindowPosition(PanelManagement.PanelManager.Instance.GetCurrentForm() as Form);
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

                    System.Windows.Threading.Dispatcher.Run();
                    Log.Debug("Exited DISPATCHER.RUN");
                    _inTalkWindowCreationThread = false;
                }
            });

            viewerThread.SetApartmentState(ApartmentState.STA);
            Log.Debug("Starting thread, _inTalkWindowCreationThread is :  " + _inTalkWindowCreationThread);
            viewerThread.Start();
        }

        /// <summary>
        /// Handler for event raised when a scanner is displayed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Instance_EvtScannerShow(object sender, PanelManagement.ScannerShowEventArg arg)
        {
            SetTalkWindowPosition(arg.Scanner.Form);
        }

        /// <summary>
        /// Checks if the rectangle is off the bounds of the primary
        /// display
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <returns>true if it is</returns>
        private Boolean isOffScreen(Rectangle rect)
        {
            var isContained = Screen.PrimaryScreen.WorkingArea.Contains(rect);

            Log.Debug("mainScreen bounds: " + Screen.PrimaryScreen.WorkingArea +
                        ", talk window bounds: " + rect +
                        ". isContained is " + isContained);

            return !isContained;
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

            switch (scannerPosition)
            {
                case Windows.WindowPosition.TopLeft:
                    if (_talkWindowForm.Location.Y < scannerForm.Location.Y + scannerForm.Size.Height)
                    {
                        Windows.SetWindowPositionRelative(_talkWindowForm, scannerForm, Windows.WindowRelative.Below);
                    }

                    if (isOffScreen(_talkWindowForm.Bounds))
                    {
                        if (spaceLeft < _talkWindowForm.Width)
                        {
                            _talkWindowForm.Width = spaceLeft;
                        }

                        Windows.SetWindowPositionRelative(_talkWindowForm, scannerForm, Windows.WindowRelative.Right);
                    }

                    break;

                case Windows.WindowPosition.TopRight:
                    if (_talkWindowForm.Location.Y < (scannerForm.Location.Y + scannerForm.Size.Height))
                    {
                        Windows.SetWindowPositionRelative(_talkWindowForm, scannerForm, Windows.WindowRelative.Below);
                    }

                    if (isOffScreen(_talkWindowForm.Bounds))
                    {
                        if (spaceLeft < _talkWindowForm.Width)
                        {
                            _talkWindowForm.Width = spaceLeft;
                        }

                        Windows.SetWindowPositionRelative(_talkWindowForm, scannerForm, Windows.WindowRelative.Left);
                    }

                    break;

                case Windows.WindowPosition.BottomLeft:
                    if ((_talkWindowForm.Location.Y + _talkWindowForm.Height) > scannerForm.Location.Y)
                    {
                        Windows.SetWindowPositionRelative(GetTalkWindow(), scannerForm, Windows.WindowRelative.Above);
                    }

                    if (isOffScreen(_talkWindowForm.Bounds))
                    {
                        if (spaceLeft < _talkWindowForm.Width)
                        {
                            _talkWindowForm.Width = spaceLeft;
                        }

                        Windows.SetWindowPositionRelative(GetTalkWindow(), scannerForm, Windows.WindowRelative.Right);
                    }

                    break;

                case Windows.WindowPosition.BottomRight:

                    if ((_talkWindowForm.Location.Y + _talkWindowForm.Height) > scannerForm.Location.Y)
                    {
                        Windows.SetWindowPositionRelative(GetTalkWindow(), scannerForm, Windows.WindowRelative.Above);
                    }

                    if (isOffScreen(_talkWindowForm.Bounds))
                    {
                        if (spaceLeft < _talkWindowForm.Width)
                        {
                            _talkWindowForm.Width = spaceLeft;
                        }

                        Windows.SetWindowPositionRelative(GetTalkWindow(), scannerForm, Windows.WindowRelative.Left);
                    }

                    break;

                default:
                    _talkWindow.Center();
                    break;
            }
        }

        /// <summary>
        /// Displays translucent glass behind the talk window
        /// </summary>
        private void showGlass()
        {
            PanelManagement.Glass.Enable = CoreGlobals.AppPreferences.EnableGlass;
            PanelManagement.Glass.ShowGlass();
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
        /// Detaches from events previously subscribed to.
        /// </summary>
        private void unsubscribeEvents()
        {
            _talkWindow.EvtRequestCloseTalkWindow -= talkWindow_EvtRequestCloseTalkWindow;
        }
    }
}