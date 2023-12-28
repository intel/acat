////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Utility;
using System;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// The panel stack reprsents a "stack" of panels (or
    /// scanners). This is similar to the stack model that Android
    /// uses to display activities.  When a scanner is displayed as the
    /// child of a parent scanner, the parent scanner is 'paused' (hidden).
    /// When the child closes, the parent scanner is 'resumed' (displayed)
    /// </summary>
    public class PanelStack
    {
        private static bool _appCloseNotifed;

        /// <summary>
        /// The currently active and visible scanner form.
        /// </summary>
        private Form _currentForm;

        /// <summary>
        /// The current active panel. This is the the last scanner that
        /// shown not as a dialog.  Note that the _currentPanel scanner may
        /// not be visible as it may have created child scanners as dialogs
        /// _currentPanel could be the same as _currentForm, or it is an
        /// ancestor of _currentForm.
        /// </summary>
        private Form _currentPanel;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public PanelStack()
        {
            PreShowPanel = null;
            PreShowPanelDisplayMode = DisplayModeTypes.None;
        }

        /// <summary>
        /// Raised when a scanner is closed
        /// </summary>
        public event ScannerClose EvtScannerClosed;

        /// <summary>
        /// Returns the currently visible Form object
        /// </summary>
        public Form CurrentForm
        {
            get { return _currentForm; }
        }

        public bool IsPaused { get; private set; }

        /// <summary>
        /// Gets the display mode of the panel that is currently displayed
        /// </summary>
        public DisplayModeTypes PanelDisplayMode
        {
            get
            {
                if (_currentForm == null || !(_currentForm is IPanel))
                {
                    return DisplayModeTypes.None;
                }

                IPanel panel = _currentForm as IPanel;

                return panel.PanelCommon.DisplayMode;
            }
        }

        /// <summary>
        /// Gets or sets the panel that is about to be shown.  Call
        /// this from the OnPause handler to see which panel is going
        /// to be displayed. Has non-null value JUST before the panel
        /// is shown, null all other times
        /// </summary>
        public IPanel PreShowPanel { get; private set; }

        /// <summary>
        /// Gets or sets the display mode of the panel that is about to
        /// be shown
        /// </summary>
        public DisplayModeTypes PreShowPanelDisplayMode { get; private set; }

        /// <summary>
        /// Event handler for request to display a scanner. The
        /// arg parameter contains information about which scanner
        /// to display
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        public void AppAgent_EvtPanelRequest(object sender, PanelRequestEventArgs arg)
        {
            Log.Debug("A request came in for panel " + arg.PanelClass + ".  NewWindow is " +
                      arg.MonitorInfo.IsNewWindow);

            if (_currentForm != null)
            {
                Log.Debug("_currentForm is " + _currentForm.Name + ", type: " + _currentForm.GetType() +
                            ", IsModal: " + _currentForm.Modal);

                Form owner = _currentForm.Owner;
                if (owner != null)
                {
                    Log.Debug("owner is : " + owner.Name + ", type: " + owner.GetType() +
                                ", is owner ipanel? " + (_currentForm.Owner is IPanel));
                }
                else
                {
                    Log.Debug("_currentForm.Owner is null");
                }

                // if a modal dialog is currently open, don't honor the request.
                // the modal dialog has to be closed first.
                if (arg.TargetPanel == null || arg.TargetPanel != _currentForm)
                {
                    if (_currentForm.Modal || (_currentForm.Owner != null && _currentForm.Owner.Modal))
                    {
                        Log.Debug("A modal dialog is open. Will not honor panel request");
                        return;
                    }
                }
            }

            // if no panel type, use the alphabet scanner as the
            String requestedPanelClass = arg.PanelClass;
            if (PanelConfigMap.AreEqual(arg.PanelClass, PanelClasses.None))
            {
                requestedPanelClass = PanelClasses.Alphabet;
            }

            IScannerPanel currentScanner = _currentPanel as IScannerPanel;

            if (arg.MonitorInfo.IsNewWindow)
            {
                Log.Debug("This is a new window. winHandle: " + arg.MonitorInfo.FgHwnd);

                if (currentScanner != null)
                {
                    Log.Debug("currentpanel: " + currentScanner.PanelClass + ", requested:  " + requestedPanelClass);
                }
                else
                {
                    Log.Debug("_currentPanel is null or not IScannerPanel. Activate alphabet scanner");
                    requestedPanelClass = PanelClasses.Alphabet;
                }

                // if the current scanner is the same as the requested one, just show it
                if (currentScanner != null && PanelConfigMap.AreEqual(currentScanner.PanelClass, requestedPanelClass) &&
                    _currentPanel.Owner == null)
                {
                    Log.Debug("Current panel is already " + requestedPanelClass + ", calling Show()");
                    _currentPanel.TopMost = true;

                    if (_currentPanel is MenuPanelBase)
                    {
                        (_currentPanel as MenuPanelBase).SetTitle(arg.Title);
                    }
                    Show(null, (IPanel)_currentPanel);
                }
                else
                {
                    // check with the agent if it is OK to switch panels.
                    if ((currentScanner == null) || currentScanner.OnQueryPanelChange(arg))
                    {
                        switchCurrentPanel(arg);
                    }
                }
            }
            else
            {
                if (currentScanner == null)
                {
                    Log.Debug("_currentPanel is null. returning");
                    return;
                }

                Log.Debug("Not a new window.  _currentPanel is " + currentScanner.PanelClass +
                                " requested panel is " + requestedPanelClass);

                // if the current panel is not the same as the requested one, query the
                // agent if it is OK to switch and then do the switch
                if (!PanelConfigMap.AreEqual(currentScanner.PanelClass, requestedPanelClass) &&
                                            currentScanner.OnQueryPanelChange(arg))
                {
                    switchCurrentPanel(arg);
                }
                else
                {
                    Log.Debug("Will not switch panels.  Current: " + currentScanner.PanelClass +
                                    ", requested: " + requestedPanelClass);

                    if (currentScanner is MenuPanelBase)
                    {
                        (currentScanner as MenuPanelBase).SetTitle(arg.Title);
                    }
                }
            }
        }

        public void CloseCurrentForm()
        {
            if (_currentForm != null)
            {
                Windows.CloseForm(_currentForm);

                if (_currentPanel == _currentForm)
                {
                    _currentPanel = null;
                }
                _currentForm = null;
            }
        }

        /// <summary>
        /// Closes the current panel.  Traces the
        /// ancestors of the current panel until
        /// there are no more parents.  Closes the
        /// topmost ancestor of the parent form
        /// </summary>
        public void CloseCurrentPanel()
        {
            Log.Debug();
            bool isCurrentForm = false;

            if (_currentPanel != null)
            {
                Log.Debug("Will close panel. _currentPanel.name is " + _currentPanel.Name);

                Control form = _currentPanel;
                Form f = _currentPanel;
                while (true)
                {
                    if (f == _currentForm)
                    {
                        isCurrentForm = true;
                    }

                    if (f is IScannerPanel)
                    {
                        Log.Debug("panelClass: " + ((IScannerPanel)f).PanelClass);
                    }

                    if (f.Owner != null)
                    {
                        Log.Debug("This one has a owner");
                        form = f.Owner;
                        f = (Form)form;
                    }
                    else
                    {
                        Log.Debug("Setting currentpanel to null. This one does not has a owner. Closing " +
                                    f.Name + ", type: " + f.GetType());

                        Windows.CloseForm(f);

                        _currentPanel = null;
                        break;
                    }
                }
            }
            else
            {
                Log.Debug("_currentPanel is null");
            }

            if (isCurrentForm)
            {
                _currentForm = null;
            }
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(String panelClass)
        {
            return CreatePanel(panelClass, String.Empty, new StartupArg(panelClass));
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="title">title of the panel</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(String panelClass, String title)
        {
            return CreatePanel(panelClass, title, new StartupArg(panelClass));
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="panelTitle">panel title</param>
        /// <param name="startupArg">statrtup arg for the panel</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(String panelClass, String panelTitle, StartupArg startupArg)
        {
            Log.Debug("panelClass: " + panelClass);

            Form form = CreatePanel(ref panelClass, panelTitle, IntPtr.Zero, null);

            Log.IsNull("Form for this panel ", form);

            if (form is IPanel)
            {
                (form as IPanel).Initialize(startupArg);
            }

            Log.Debug("Returning form from createPanel");

            return form;
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="panelTitle">panel title</param>
        /// <param name="winHandle">target window handle</param>
        /// <param name="focusedElement">currently focused element</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(
                        ref String panelClass,
                        String panelTitle,
                        IntPtr winHandle,
                        AutomationElement focusedElement)
        {
            Log.Debug("panelClass: " + panelClass);

            var panelConfigMapEntry = PanelConfigMap.GetPanelConfigMapEntry(panelClass);
            if (panelConfigMapEntry == null)
            {
                Log.Debug("Could not find panel for " + panelClass + ". Using default ");

                panelClass = PanelClasses.Alphabet;
                panelConfigMapEntry = PanelConfigMap.GetPanelConfigMapEntry(PanelClasses.Alphabet);

                if (panelConfigMapEntry == null)
                {
                    return null;
                }

                Log.Debug("Could not find panel for " + panelClass + ". Using default " + panelConfigMapEntry.FormType.Name);
            }

            Log.Debug("panel: " + panelConfigMapEntry.FormType.Name);

            return createPanel(panelClass, panelTitle, panelConfigMapEntry.FormType, winHandle, focusedElement);
        }

        /// <summary>
        /// Returns the currently visible panel Form
        /// </summary>
        /// <returns>form</returns>
        public IPanel GetCurrentForm()
        {
            return _currentForm as IPanel;
        }

        /// <summary>
        /// Returns the current panel. The current panel is the the
        /// last scanner that was not shown as a dialog.  Note that
        /// the Current Panel scanner may not be visible as it may
        /// have created child scanners as dialogs.
        /// </summary>
        /// <returns>The active panel</returns>
        public IPanel GetCurrentPanel()
        {
            return _currentPanel as IPanel;
        }

        /// <summary>
        /// Return the panel name of panel that is currently active
        /// and visible.
        /// </summary>
        /// <returns>the name</returns>
        public String GetCurrentPanelName()
        {
            if (_currentForm is IScannerPanel)
            {
                return ((IScannerPanel)_currentForm).PanelClass;
            }

            return String.Empty;
        }

        /// <summary>
        /// Pauses the current form
        /// </summary>
        public void Pause()
        {
            if (_currentForm is IPanel)
            {
                Log.Debug("Pausing _currentForm " + _currentForm.Name + ", IsModal: " + _currentForm.Modal);
                (_currentForm as IPanel).OnPause();
                IsPaused = true;
            }
        }

        /// <summary>
        /// Resumes the current form
        /// </summary>
        public void Resume()
        {
            if (_currentForm == null)
            {
                return;
            }

            Log.Debug("Calling OnResume on " + _currentForm.Name);

            (_currentForm as IPanel).OnResume();

            IsPaused = false;

            auditLogScannerEvent(_currentForm, "show");
        }

        /// <summary>
        /// Displays the panel. Parent is the panel
        /// making the call. Also Pauses the parent
        /// It will be Resumed when the 'panel' is closed.
        /// </summary>
        /// <param name="parent">The parent panel</param>
        /// <param name="panel">the panel to show</param>
        /// <returns>true on success</returns>
        public bool Show(IPanel parent, IPanel panel)
        {
            return show(parent, panel, DisplayModeTypes.Normal);
        }

        /// <summary>
        /// Displays the panel
        /// </summary>
        /// <param name="form">panel to display</param>
        /// <returns></returns>
        public bool Show(IPanel form)
        {
            return show(null, form, DisplayModeTypes.Normal);
        }

        /// <summary>
        /// Shows the specified panel as a dialog. If there
        /// is a scanner currently active, it uses that scanner
        /// as the parent of the dialog
        /// </summary>
        /// <param name="panel">panel to show</param>
        /// <returns>true on success</returns>
        public bool ShowDialog(IPanel panel)
        {
            return ShowDialog(null, panel);
        }

        /// <summary>
        /// Shows the specified panel as a dialog with
        /// the 'parent' panel as the parent of the dialog.
        /// If parent is null, if there is a panel currently
        /// active, it uses that as the parent
        /// </summary>
        /// <param name="parent">parent panel (can be null)</param>
        /// <param name="panel">panel to display</param>
        /// <returns>true on success</returns>
        public bool ShowDialog(IPanel parent, IPanel panel)
        {
            bool retVal = true;

            if (!(panel is Form))
            {
                return false;
            }

            var form = (Form)panel;

            Log.Debug("showDialog " + form.Name + ", type: " + form.GetType());

            // if parent has not been specified, used the current form
            // as the parent and Show as Dialog.  If there is no current form, just
            // show.
            if (parent == null)
            {
                Log.Debug("parent passed is null");
                Log.IsNull("_currentForm ", _currentForm);
                Log.IsNull("_currentPanel ", _currentPanel);

                if (_currentForm is IPanel)
                {
                    Log.Debug("Showing as dialog child: " + panel.GetType() + ", parent: " + _currentForm.GetType());
                    retVal = show((IPanel)_currentForm, panel, DisplayModeTypes.Dialog);
                }
                else
                {
                    Log.Debug("Just showing " + form.GetType());
                    //retVal = Show(panel);
                    retVal = show(null, panel, DisplayModeTypes.Dialog);
                }
            }
            else
            {
                var parentForm = parent as Form;
                Log.Debug("showDialog parent: " + parentForm.Name + ", type: " + parentForm.GetType());
                retVal = show(parent, panel, DisplayModeTypes.Dialog);
            }

            return retVal;
        }

        /// <summary>
        /// Displays the panel as a popup. Parent is the panel
        /// making the call. Also Pauses the parent
        /// It will be Resumed when the 'panel' is closed.
        /// </summary>
        /// <param name="parent">The parent panel</param>
        /// <param name="panel">the panel to show</param>
        /// <returns>true on success</returns>
        public bool ShowPopup(IPanel parent, IPanel panel)
        {
            return show(parent, panel, DisplayModeTypes.Popup);
        }

        /// <summary>
        /// Displays the panel as a popup
        /// </summary>
        /// <param name="form">panel to display</param>
        /// <returns>true on succes</returns>
        public bool ShowPopup(IPanel form)
        {
            return show(null, form, DisplayModeTypes.Popup);
        }

        /// <summary>
        /// Creates an audit log entry for the scanner activity
        /// </summary>
        /// <param name="form">the panel form</param>
        /// <param name="operation">type of activity</param>
        private void auditLogScannerEvent(Form form, String operation)
        {
            var title = String.Empty;
            if (form is MenuPanelBase)
            {
                var menu = (MenuPanelBase)form;
                title = menu.Title;
            }

            String panelClass;

            if (form is MenuPanelBase)
            {
                panelClass = PanelCategory.Menu.ToString();
            }
            else if (form is IScannerPanel)
            {
                panelClass = PanelCategory.Scanner.ToString();
            }
            else if (form is IDialogPanel)
            {
                panelClass = PanelCategory.Dialog.ToString();
            }
            else
            {
                panelClass = "Unknown";
            }

            AuditLog.Audit(new AuditEventScannerActivity(operation, panelClass, form.Name, title));
        }

        /// <summary>
        /// Creates the form requested in eventArg
        /// </summary>
        /// <param name="eventArg">info about panel to create</param>
        /// <returns>created form</returns>
        private Form createPanel(PanelRequestEventArgs eventArg)
        {
            String panelClass = eventArg.PanelClass;
            Form form = CreatePanel(
                            ref panelClass,
                            eventArg.Title,
                            eventArg.MonitorInfo.FgHwnd,
                            eventArg.MonitorInfo.FocusedElement);
            eventArg.PanelClass = panelClass;
            return form;
        }

        /// <summary>
        /// Creates the panel with the specified info.  If the
        /// panel Class has multiple constructors, find the
        /// one that works.  Uses reflection to create an instance
        /// of the panel Form.
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="panelTitle">title for the panel</param>
        /// <param name="type">the class Type of the panel</param>
        /// <param name="winHandle">target window handle</param>
        /// <param name="focusedElement">Target focused element</param>
        /// <returns></returns>
        private Form createPanel(
                        String panelClass,
                        String panelTitle,
                        Type type,
                        IntPtr winHandle,
                        AutomationElement focusedElement)
        {
            Form retVal = null;
            Log.Debug("***  panelClass: [" + panelClass + "], panel: [" + type.FullName + "]" +
                        " title: [" + (panelTitle ?? "null") + "]");
            try
            {
                Type[] types = { typeof(String) };
                ConstructorInfo info = type.GetConstructor(types);
                Object obj;

                if (info != null)
                {
                    obj = Activator.CreateInstance(type, panelClass);
                }
                else
                {
                    types = new[] { typeof(String), typeof(IntPtr) };
                    info = type.GetConstructor(types);

                    if (info != null)
                    {
                        obj = Activator.CreateInstance(type, panelClass, winHandle);
                    }
                    else
                    {
                        types = new[] { typeof(String), typeof(String) };
                        info = type.GetConstructor(types);
                        if (info != null)
                        {
                            obj = Activator.CreateInstance(type, panelClass, panelTitle);
                        }
                        else
                        {
                            types = new[] { typeof(String), typeof(IntPtr), typeof(AutomationElement) };
                            info = type.GetConstructor(types);
                            if (info != null)
                            {
                                obj = Activator.CreateInstance(type, panelClass, winHandle, focusedElement);
                            }
                            else
                            {
                                Log.Debug("Creating " + type + " with default constructor");
                                obj = Activator.CreateInstance(type);
                            }
                        }
                    }
                }

                if (obj is Form)
                {
                    retVal = obj as Form;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = null;
            }

            return retVal;
        }

        /// <summary>
        /// Initialzies the specified scanner panel
        /// </summary>
        /// <param name="scannerPanel">panel to initialize</param>
        /// <param name="arg">panel arguments</param>
        /// <returns>true on success</returns>
        private bool initializePanel(IScannerPanel scannerPanel, PanelRequestEventArgs arg)
        {
            var startupArg = new StartupArg
            {
                FocusedElement = arg.MonitorInfo.FocusedElement,
                PanelClass = arg.PanelClass,
                Arg = arg.RequestArg
            };

            var panelConfigMapEntry = PanelConfigMap.GetPanelConfigMapEntry(arg.PanelClass);

            Log.Debug("panelClass:  " + arg.PanelClass + ", ConfigFIle: " + ((panelConfigMapEntry != null) ? panelConfigMapEntry.ConfigFileName : String.Empty));
            return scannerPanel.Initialize(startupArg);
        }

        /// <summary>
        /// A scanner closed. If this panel has child windows (scanners)
        /// close them as well. If there is a parent, Resume it.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void panel_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = (Form)sender;

            Log.Debug("Enter (" + form.Name + ")");

            IPanel panel = form as IPanel;
            if (panel.SyncObj.Status == SyncLock.StatusValues.Closed)
            {
                Log.Debug("Form is already closed. Returning " + form.Name);
                return;
            }

            Log.Debug("Setting CLOSED for " + form.Name);
            (form as IPanel).SyncObj.Status = SyncLock.StatusValues.Closed;

            form.FormClosed -= panel_FormClosed;

            Form parentForm = form.Owner;

            Form[] array = form.OwnedForms;

            auditLogScannerEvent(form, "close");

            Log.Debug("number of owned forms: " + array.Length);

            // close all the forms this panel owns
            while (true)
            {
                Form[] ownedForms = form.OwnedForms;
                if (ownedForms.Length == 0)
                {
                    Log.Debug(form.Name + ": No more owned forms. Breaking");
                    break;
                }

                Log.Debug("Removing owned form from list. " + ownedForms[0].Name);
                form.RemoveOwnedForm(ownedForms[0]);
                Log.Debug("Calling close on " + ownedForms[0].Name);
                Windows.CloseForm(ownedForms[0]);
            }

            Log.Debug("form Name: " + form.Name + ", type: " + form.GetType());

            // Exit the application if instructed to do so.
            if (Context.AppQuit)
            {
                if (!_appCloseNotifed)
                {
                    _appCloseNotifed = true;

                    Context.AppPanelManager.NotifyQuitApplication();

                    Application.ExitThread();
                }
            }
            else if (parentForm != null)
            {
                // Resume the parent if it is prudent to do so.

                Log.Debug("parent Form is " + parentForm.Name);

                IPanel parentPanel = (IPanel)parentForm;

                if (parentPanel.SyncObj.IsClosing())
                {
                    Log.Debug("*** Parent is closing. Will not call OnResume");
                }
                else
                {
                    Log.Debug("parentform is not closing. Setting _currentPanel to " + parentForm.Name +
                                ", type: " + parentForm.GetType());

                    _currentPanel = parentForm;
                    _currentForm = parentForm;

                    Log.Debug("Calling OnResume on parentForm " + parentForm.Name);

                    parentPanel.OnResume();

                    //_currentPanel = parentForm;  // moved up
                    //_currentForm = parentForm;  // moved up

                    auditLogScannerEvent(parentForm, "show");
                }
            }
            else
            {
                Log.Debug("parentform is null");
                _currentPanel = null;
                _currentForm = null;
            }

            var panelClass = (form is IScannerPanel) ? ((IScannerPanel)form).PanelClass : PanelClasses.None;
            if (!PanelConfigMap.AreEqual(panelClass, PanelClasses.None))
            {
                Log.Debug("Calling AppAgentMgr.OnPanelClosed for " + panelClass);
                Context.AppAgentMgr.OnPanelClosed(panelClass);
            }

            if (EvtScannerClosed != null)
            {
                Log.Debug("Calling evetscannerclosed for " + form.Name);
                EvtScannerClosed(this, new ScannerCloseEventArg(form as IPanel));
            }
            else
            {
                Log.Debug("EvtScannerClosed is NULL");
            }

            // (form as IPanel).SyncObj.Status = SyncLock.StatusValues.Closed;  // moved this up

            Log.Debug("Exit " + form.Name);
        }

        /// <summary>
        /// Shows 'panel'.  'parent' is the form making the call to show.
        /// If 'showAsDialog' is true, shows panel as the Dialog with 'parent'
        /// as the parent form. Else, just shows 'panel'. Also Pauses the parent
        /// scanner.  It will be Resumed when the panel is closed.
        /// </summary>
        /// <param name="parent">parent panel</param>
        /// <param name="panel">panel to show</param>
        /// <param name="displayMode">how to display the panel</param>
        /// <returns>true on success</returns>
        private bool show(IPanel parent, IPanel panel, DisplayModeTypes displayMode)
        {
            Form panelForm = (Form)panel;
            Form parentForm = (Form)parent;

            Log.Debug("parentForm: " + ((parentForm != null) ? parentForm.Name : " null") +
                        ".  panel: " + panelForm.Name);

            panelForm.FormClosed += panel_FormClosed;

            _currentForm = panelForm;

            PreShowPanel = panel;
            PreShowPanelDisplayMode = displayMode;

            if (parent != null)
            {
                parent.OnPause();

                if (displayMode == DisplayModeTypes.Dialog)
                {
                    Log.Debug("Showing Dialog" + panelForm.Name + " with parent " + parentForm.Name);

                    auditLogScannerEvent(panelForm, "show");
                    Context.AppPanelManager.NotifyPanelPreShow(new PanelPreShowEventArg(panel, displayMode));
                    Windows.ShowDialog(parentForm, panelForm);
                }
                else
                {
                    Log.Debug("Showing " + panelForm.Name);

                    Log.Debug("parent is not null. Setting _currentPanel to " + panelForm.Name +
                                ", type: " + panelForm.GetType());
                    _currentPanel = panelForm;
                    auditLogScannerEvent(panelForm, "show");

                    Context.AppPanelManager.NotifyPanelPreShow(new PanelPreShowEventArg(panel, displayMode));

                    Windows.Show(parentForm, panelForm);
                }
            }
            else
            {
                Log.Debug("showing " + panelForm.Name + ", parent is null");
                Log.Debug("parent is null. Setting _currentPanel to " + panelForm.Name +
                            ", type: " + panelForm.GetType());

                _currentPanel = panelForm;

                auditLogScannerEvent(panelForm, "show");
                if (displayMode == DisplayModeTypes.Dialog)
                {
                    Context.AppPanelManager.NotifyPanelPreShow(new PanelPreShowEventArg(panel, displayMode));

                    panelForm.ShowDialog();
                }
                else
                {
                    Context.AppPanelManager.NotifyPanelPreShow(new PanelPreShowEventArg(panel, displayMode));

                    Windows.ShowForm(panelForm);
                }
            }

            PreShowPanel = null;
            PreShowPanelDisplayMode = DisplayModeTypes.None;

            return true;
        }

        /// <summary>
        /// Switches the currently active scanner to the one specified
        /// in eventArg.  Closes the current panel, creates nand shows
        /// the specified one.
        /// </summary>
        /// <param name="eventArg">Info about which scanner to display</param>
        private void switchCurrentPanel(PanelRequestEventArgs eventArg)
        {
            Log.Debug();

            Log.Debug(eventArg.ToString());

            if (!eventArg.UseCurrentScreenAsParent)
            {
                Log.Debug("UseCurrentScreenAsParent is false.  closing current panel " +
                          ((_currentPanel != null) ? _currentPanel.Name : "<null>"));

                CloseCurrentPanel();
            }

            Log.Debug("Creating panel ..." + eventArg.PanelClass);
            Form form = createPanel(eventArg);
            Log.Debug("Calling show for ..." + eventArg.PanelClass);
            if (form == null)
            {
                Log.Debug("createPanel returned null!!");
            }
            else
            {
                initializePanel(form as IScannerPanel, eventArg);
                Log.Debug("Calling show for ..." + eventArg.PanelClass);

                if (eventArg.UseCurrentScreenAsParent && _currentForm is IPanel)
                {
                    Log.Debug("Showing form " + form.Name + ", parent " + _currentForm.Name);
                    Show((IPanel)_currentForm, (IPanel)form);
                }
                else
                {
                    Log.Debug("Showing form " + form.Name + " without parent.");
                    Show(null, (IPanel)form);
                }
            }
        }
    }
}