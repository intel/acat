////////////////////////////////////////////////////////////////////////////
// <copyright file="DialogCommon.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.ThemeManagement;
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
    /// Helper class that contains helper function for all the dialog
    /// windows.  It contains functions that are common across all dialogs
    /// and using this avoids duplication of code, consistency in how
    /// events are handled and makes it easier for the developer to add
    /// new dialogs to ACAT.
    /// A dialog form will contain a DialogCommon field and
    /// call the methods in this class whereever they are needed. The
    /// documentation for the methods have information on when these
    /// methods need to be invoked.
    /// This class creates the WidgetManager and AnimationManager objects
    /// required by the form and has getters for the various fields.
    /// </summary>
    public class DialogCommon : IDisposable
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
        /// Name of the form in the screen config map
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
        }

        /// <summary>
        /// Delegate to stop animation
        /// </summary>
        private delegate void StopDelegate();

        /// <summary>
        /// Returns the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncLock; }
        }

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
        /// Returns the animation manager object
        /// </summary>
        /// <returns>animation manager object</returns>
        public AnimationManager GetAnimationManager()
        {
            return _animationManager;
        }

        /// <summary>
        /// Returns the root widget manager
        /// </summary>
        /// <returns></returns>
        public Widget GetRootWidget()
        {
            return _rootWidget;
        }

        /// <summary>
        /// Returns the widget manager object
        /// </summary>
        /// <returns>Widget manager object</returns>
        public WidgetManager GetWidgetManager()
        {
            return _widgetManager;
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
        /// Initializes the class using the panel name.  Call this in
        /// the constructor of the form.
        /// </summary>
        /// <param name="panelClass">name/class of the panel</param>
        /// <returns>true on success</returns>
        public bool Initialize(String panelClass)
        {
            _panelName = panelClass;
            return Initialize();
        }

        /// <summary>
        /// If the form doesn't have a panel name, call this in the
        /// constructor of the form
        /// </summary>
        /// <returns>true on success</returns>
        public bool Initialize()
        {
            bool retVal = initWidgetManager();
            if (retVal)
            {
                retVal = initAnimationManager();
            }

            _form.TopMost = true;
            _form.Paint += Form_Paint;

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
        /// Call this function from the FormClosing override in
        /// Form
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
            subscribeToEvents();
        }

        /// <summary>
        /// Pause handler.  Pauses the animation manager
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
        /// Resume handler.  Resumes the animation manager
        /// </summary>
        public void OnResume()
        {
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
        /// If the widget requires a scanner to interact, create the scanner
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
                FocusedElement = AutomationElement.FromHandle(widget.UIControl.Handle),
            };

            return startupArg;
        }

        /// <summary>
        /// Draw a border around the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Paint(object sender, PaintEventArgs e)
        {
            //RoundedCornerControl.DrawBorder(_formGraphicsPath, e, Color.Black);
        }

        /// <summary>
        /// Returns the config file for this form.
        /// </summary>
        /// <returns></returns>
        private String getConfigFile()
        {
            return !String.IsNullOrEmpty(_panelName) ?
                            PanelConfigMap.GetConfigFileForPanel(_panelName) :
                            PanelConfigMap.GetConfigFileForScreen(_form.GetType());
        }

        /// <summary>
        /// Loads all animations from the configfile for the form
        /// </summary>
        private bool initAnimationManager()
        {
            _animationManager = new AnimationManager();
            bool retVal = _animationManager.Init(getConfigFile());
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
        private bool initWidgetManager()
        {
            _widgetManager = new WidgetManager(_form);

            _widgetManager.Layout.SetColorScheme(ColorSchemes.DialogSchemeName);

            bool retVal = _widgetManager.Initialize(getConfigFile());

            if (!retVal)
            {
                Log.Error("Unable to initialize widget manager");
            }
            else
            {
                _rootWidget = _widgetManager.RootWidget;
                if (String.IsNullOrEmpty(_rootWidget.SubClass))
                {
                    _rootWidget.SubClass = PanelClasses.PanelCategory.Dialog.ToString();
                }
            }

            return retVal;
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
            GetAnimationManager().Interpreter.EvtRun += Interpreter_EvtRun;

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
            GetAnimationManager().Interpreter.EvtRun -= Interpreter_EvtRun;
        }

        /// <summary>
        /// The user actuated a widget.  perform the necessary action.  If the
        ///  widget requires a scanner to interact (text boxes may require the
        ///  alphabet scanner for eg) active the scanner
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
    }
}