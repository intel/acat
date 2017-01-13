////////////////////////////////////////////////////////////////////////////
// <copyright file="DialogCommon.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Helper class that contains helper function for all the dialog
    /// windows.  It contains functions that are common across all dialogs
    /// and using this avoids duplication of code, consistency in how
    /// events are handled and makes it easier for the developer to add
    /// new dialogs to ACAT.
    /// A dialog form will contain a DialogCommon field and
    /// call the methods in this class whereever they are needed. Refer
    /// to the documentation for the methods in this class for info on when these
    /// methods need to be invoked.
    /// This class creates the WidgetManager and AnimationManager objects
    /// required by the form and has getters for the various fields.
    /// </summary>
    public class DialogCommon : IDisposable, IPanelCommon
    {
        /// <summary>
        /// All dialog forms should derive from IDialogPanel
        /// </summary>
        private readonly IDialogPanel _dialogPanel;

        /// <summary>
        /// The dialog form
        /// </summary>
        private readonly Form _form;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly SyncLock _syncLock;

        /// <summary>
        /// The animation manager for this form
        /// </summary>
        private AnimationManager _animationManager;

        /// <summary>
        /// Has this object been disposed off?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Name of the form in the Panel config map
        /// </summary>
        private String _panelName;

        /// <summary>
        /// The root widget for this form.  This is the window itself
        /// </summary>
        private Widget _rootWidget;

        /// <summary>
        /// The widget manager for this form
        /// </summary>
        private WidgetManager _widgetManager;

        /// <summary>
        /// Used to give the form rounded corners
        /// </summary>
        //private GraphicsPath _formGraphicsPath;

        /// <summary>
        /// Make sure nothing overlaps the form
        /// </summary>
        private WindowOverlapWatchdog _windowOverlapWatchdog;

        /// <summary>
        /// Initializes an new instance of the DialogCommon class.  Create this
        /// class in the constructor of the form.
        /// </summary>
        /// <param name="form">The dialog form </param>
        public DialogCommon(Form form)
        {
            _form = form;
            _form.ShowInTaskbar = false;
            _panelName = String.Empty;
            _dialogPanel = (IDialogPanel)_form;
            _syncLock = new SyncLock();
            AutoDockScanner = true;
        }

        /// <summary>
        /// Delegate to stop animation
        /// </summary>
        private delegate void StopDelegate();

        /// <summary>
        /// Gets the Animation Manager object
        /// </summary>
        public AnimationManager AnimationManager { get { return _animationManager; } }

        /// <summary>
        /// Get/sets whether a scanner will be docked to the dialog
        /// or not.  The scanner is typically used to enter data
        /// into the form.  If set to false, the scanner will be displayed
        /// in its default position.
        /// </summary>
        public bool AutoDockScanner { get; set; }

        /// <summary>
        /// Gets the panel config id for this panel
        /// </summary>
        public Guid ConfigId { get; private set; }

        /// <summary>
        /// Gets the display mode of the panel
        /// </summary>
        public DisplayModeTypes DisplayMode { get; private set; }

        /// <summary>
        /// Gets the object that manages the size and position
        /// of the panel
        /// </summary>
        public ScannerPositionSizeController PositionSizeController { get { return null; } }

        /// <summary>
        /// Gets the widget that reprensents the form
        /// </summary>
        public Widget RootWidget { get { return _rootWidget; } }

        /// <summary>
        /// Returns the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncLock; }
        }

        /// <summary>
        /// Gets the WidgetManager object
        /// </summary>
        public WidgetManager WidgetManager { get { return _widgetManager; } }

        /// <summary>
        /// Sets the style of the form.  No sys menu
        /// </summary>
        /// <param name="createParams">Windows createparams</param>
        /// <returns>new create params</returns>
        public static CreateParams SetFormStyles(CreateParams createParams)
        {
            const int WS_SYSMENU = 0x80000;
            createParams.Style &= ~WS_SYSMENU;
            return createParams;
        }

        /// <summary>
        /// Check to see if a command should be enabled or not.
        /// This depends on the context.   The arg parameter
        /// contains the widget/command object in question.
        /// </summary>
        /// <param name="arg">Argument</param>
        public void CheckCommandEnabled(CommandEnabledArg arg)
        {
            if (_syncLock.IsClosing())
            {
                return;
            }

            arg.Handled = true;
            arg.Enabled = false;
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Winproc handler.  If the user clicks anywhere on the
        /// form, pause animation
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        public bool HandleWndProc(Message m)
        {
            bool retVal = false;
            const int WM_MOUSEACTIVATE = 0x21;
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                Control control = Control.FromHandle(m.HWnd);
                if (control != null && (control == _form || _form.Contains(control)))
                {
                    retVal = true;
                    _animationManager.Interrupt();
                }
            }

            return retVal;
        }

        /// <summary>
        /// If the form doesn't have a panel name, call this in the
        /// constructor of the form
        /// </summary>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _panelName = startupArg.PanelClass;

            var panelConfigMapEntry = PanelConfigMap.GetPanelConfigMapEntry(startupArg.PanelClass);
            if (panelConfigMapEntry == null) // did not find the panel
            {
                return false;
            }

            bool retVal = initWidgetManager(panelConfigMapEntry);
            if (retVal)
            {
                retVal = initAnimationManager(panelConfigMapEntry);
            }

            Windows.SetTopMost(_form);

            PanelManager.Instance.EvtScannerShow += Instance_EvtScannerShow;
            Windows.EvtWindowPositionChanged += Windows_EvtWindowPositionChanged;

            Windows.SetWindowPositionAndNotify(_form, Windows.WindowPosition.CenterScreen);

            _windowOverlapWatchdog = new WindowOverlapWatchdog(_form);

            return retVal;
        }

        /// <summary>
        /// Call this in the OnClose event handler in the form
        /// </summary>
        public void OnClosing()
        {
            Dispose();
        }

        /// <summary>
        /// Call this function from the FormClosing override in Form
        /// </summary>
        /// <param name="e">event args</param>
        public void OnFormClosing(FormClosingEventArgs e)
        {
            if (_syncLock.Status != SyncLock.StatusValues.None)
            {
                Log.Debug(_form.Name + ", _syncObj.Status: " + _syncLock.Status + ", form already closed.  returning");
                return;
            }

            _syncLock.Status = SyncLock.StatusValues.Closing;

            Log.Debug("Before animationmangoer.stop");
            _form.Invoke(new StopDelegate(_animationManager.Stop));
            Log.Debug("After animationmangoer.stop");

            unsubscribeFromEvents();
        }

        /// <summary>
        /// Call this in the OnLoad event handler in the form.
        /// </summary>
        public void OnLoad()
        {
            Resize(_form.Size);
            Windows.ActivateForm(_form);
            subscribeToEvents();
        }

        /// <summary>
        /// Pause handler.  Pauses the animation manager and any
        /// watchdogs that are active
        /// </summary>
        public void OnPause()
        {
            if (_windowOverlapWatchdog != null)
            {
                _windowOverlapWatchdog.Pause();
            }

            _animationManager.Pause();
        }

        /// <summary>
        /// Resume handler.  Resumes the animation manager and
        /// any watchdogs that are active
        /// </summary>
        public void OnResume()
        {
            Windows.SetWindowPositionAndNotify(_form, Windows.WindowPosition.CenterScreen);

            if (_windowOverlapWatchdog != null)
            {
                _windowOverlapWatchdog.Resume();
            }

            _animationManager.Resume();
        }

        /// <summary>
        /// Call this in the Resize event handler in the form
        /// </summary>
        /// <param name="newSize">the new size to use</param>
        public void Resize(Size newSize)
        {
            _form.Invoke(new MethodInvoker(delegate
            {
                if (newSize.Width > 0 && newSize.Height > 0)
                {
                    _form.Size = newSize;
                    Windows.SetWindowPositionAndNotify(_form, Windows.WindowPosition.CenterScreen);
                }
            }));
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
                    PanelManager.Instance.EvtScannerShow -= Instance_EvtScannerShow;

                    // dispose all managed resources.
                    Log.Debug();

                    if (_animationManager != null)
                    {
                        _animationManager.Dispose();
                    }

                    if (_windowOverlapWatchdog != null)
                    {
                        _windowOverlapWatchdog.Dispose();
                    }

                    if (_rootWidget != null)
                    {
                        _rootWidget.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// If the user selected a text box, set focus to it.
        /// </summary>
        /// <param name="widget"></param>
        private void actuateTextBox(Widget widget)
        {
            if (widget.UIControl is TextBoxBase)
            {
                var tb = (TextBoxBase)widget.UIControl;

                tb.SelectionStart = tb.Text.Length;
                tb.ScrollToCaret();
            }
        }

        /// <summary>
        /// If the widget requires a scanner to interact, create the scanner. For
        /// eg if the user want to enter text into a TextBox in the dialog.
        /// </summary>
        /// <param name="widget"></param>
        private void createAndShowScannerForWidget(Widget widget)
        {
            if (!(_form is IPanel))
            {
                return;
            }

            var startupArg = createStartupArgForScanner(widget);

            Log.Debug("Creating Panel " + widget.Panel);
            Form panel = Context.AppPanelManager.CreatePanel(widget.Panel, String.Empty, startupArg);
            var child = panel as IScannerPanel;
            if (child != null)
            {
                var scanner = child;
                scanner.SetTargetControl(_form, widget);
                Context.AppPanelManager.Show((IPanel)_form, child);
            }
        }

        /// <summary>
        /// Creates and initializes the startup arg required to start the
        /// scanner for the widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        private StartupArg createStartupArgForScanner(Widget widget)
        {
            var startupArg = new StartupArg
            {
                DialogMode = true,
                PanelClass = widget.Panel,
                FocusedElement = AutomationElement.FromHandle(widget.UIControl.Handle),
            };

            return startupArg;
        }

        /// <summary>
        /// Loads all animations from the configfile for the form
        /// </summary>
        private bool initAnimationManager(PanelConfigMapEntry panelConfigMapEntry)
        {
            _animationManager = new AnimationManager();

            bool retVal = _animationManager.Init(panelConfigMapEntry);
            if (!retVal)
            {
                Log.Error("Error initializing animation manager");
            }

            return retVal;
        }

        /// <summary>
        /// Initializes the widget manager.  Load the widget layout,
        /// set the color scheme and get the root widget object
        /// </summary>
        private bool initWidgetManager(PanelConfigMapEntry panelConfigMapEntry)
        {
            _widgetManager = new WidgetManager(_form);

            _widgetManager.Layout.SetColorScheme(ColorSchemes.DialogSchemeName);

            bool retVal = _widgetManager.Initialize(panelConfigMapEntry.ConfigFileName);

            if (!retVal)
            {
                Log.Error("Unable to initialize widget manager");
            }
            else
            {
                _rootWidget = _widgetManager.RootWidget;
                if (String.IsNullOrEmpty(_rootWidget.SubClass))
                {
                    _rootWidget.SubClass = PanelCategory.Dialog.ToString();
                }
            }

            return retVal;
        }

        /// <summary>
        /// Event handler for when a scanner is shown.  Dock it to the dialog form.
        /// The scanner is a companion to the dialog
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void Instance_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (AutoDockScanner &&
                Windows.GetVisible(_form) &&
                arg.Scanner != _form &&
                Windows.GetOpacity(_form) != 0.0f)
            {
                Windows.WindowPosition position = Context.AppWindowPosition;

                if (position == Windows.WindowPosition.CenterScreen)
                {
                    position = CoreGlobals.AppPreferences.ScannerPosition;
                }

                if (position == Windows.WindowPosition.CenterScreen)
                {
                    position = Windows.WindowPosition.MiddleRight;
                }

                if (((IPanel) arg.Scanner).PanelCommon.DisplayMode != DisplayModeTypes.Popup)
                {
                    Windows.DockWithScanner(_form, arg.Scanner as Form, position, false);
                }

                if (_form.Left < 0)
                {
                    _form.Left = 0;
                }

                Windows.SetTopMost(arg.Scanner as Form);
            }
        }

        /// <summary>
        /// Event handler to run a command. The interpreter raises the event when
        /// it encounters a command in the animation config file
        ///  command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Interpreter_EvtRun(object sender, InterpreterRunEventArgs e)
        {
            bool handled = false;

            _dialogPanel.OnRunCommand(e.Script, ref handled);
        }

        /// <summary>
        /// Subscribes to all the events triggered by the
        /// widgets and the interpreter
        /// </summary>
        private void subscribeToEvents()
        {
            AnimationManager.Interpreter.EvtRun += Interpreter_EvtRun;

            var widgetList = new List<Widget>();

            _rootWidget.Finder.FindAllChildren(widgetList);

            foreach (var widget in widgetList)
            {
                if (widget is IButtonWidget || !String.IsNullOrEmpty(widget.Panel))
                {
                    widget.EvtActuated += widget_EvtActuated;
                }
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        private void unsubscribeFromEvents()
        {
            AnimationManager.Interpreter.EvtRun -= Interpreter_EvtRun;
        }

        /// <summary>
        /// The user actuated a widget. Performs the necessary action.  If the
        /// widget requires a scanner to interact (text boxes may require the
        /// alphabet scanner for eg) activates the scanner
        /// </summary>
        private void widget_EvtActuated(object sender, WidgetEventArgs e)
        {
            Widget widget = e.SourceWidget;

            if (widget is IButtonWidget)
            {
                String value = widget.Value;
                if (!String.IsNullOrEmpty(value))
                {
                    Log.Debug("**Actuate** " + widget.Name + " Value: " + value);

                    _dialogPanel.OnButtonActuated(widget);
                }
            }
            else if (!String.IsNullOrEmpty(widget.Panel))
            {
                _form.Invoke(new MethodInvoker(delegate
                {
                    Windows.SetFocus(widget.UIControl);

                    if (widget.UIControl is TextBoxBase)
                    {
                        actuateTextBox(widget);
                    }

                    createAndShowScannerForWidget(widget);
                }));
            }
        }

        /// <summary>
        /// Event handler for window position changed.  IF there is a companion
        /// scanner, dock with it.
        /// </summary>
        /// <param name="form">form whose position changed</param>
        /// <param name="position">the position</param>
        private void Windows_EvtWindowPositionChanged(Form form, Windows.WindowPosition position)
        {
            if (AutoDockScanner &&
                (form is IScannerPanel) &&
                Windows.GetVisible(_form) &&
                form != _form &&
                Windows.GetOpacity(_form) != 0.0f)
            {
                if (((IPanel) form).PanelCommon.DisplayMode != DisplayModeTypes.Popup)
                {
                    Windows.DockWithScanner(_form, form, Context.AppWindowPosition, false);
                }

                Log.Debug("Left: " + _form.Left);

                if (_form.Left < 0)
                {
                    _form.Left = 0;
                }

                Windows.SetTopMost(form);
            }
        }
    }
}