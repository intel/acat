////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelManager.cs" company="Intel Corporation">
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
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.Audit;
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

#endregion

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Manages display of scanners.  On startup, walks the 
    /// extension directories and loads all the scanners, dialogs
    /// and contextual menus and maintains them in a cache. 
    /// When displayhing, scanners are stacked.  When the current 
    /// scanner is closed, the parent scanner is automatically displayed.
    /// </summary>
    public class PanelManager : IDisposable
    {
        /// <summary>
        /// The root directory under ACAT from where the scanners/dialog/menus
        /// are loaded
        /// </summary>
        public static String UiRootDir = "UI";

        /// <summary>
        /// Singleton instance of PanelManager
        /// </summary>
        private static PanelManager _instance;

        /// <summary>
        /// The currently visible scanner form
        /// </summary>
        private Form _currentForm;

        /// <summary>
        /// The current active panel. This is the the last scanner that
        /// shown not as a dialog.  Note that the _currentPanel scanner may
        /// not be visible as it may have created child scanners as dialogs
        /// </summary>
        private Form _currentPanel;
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes an instance of the PanelManager
        /// </summary>
        public PanelManager()
        {
            Context.AppAgentMgr.EvtPanelRequest += AppAgent_EvtPanelRequest;
            Context.AppAgentMgr.EvtFocusChanged += AppAgent_EvtFocusChanged;
            ScannerCommon.EvtScannerShow += ScannerCommon_EvtScannerShow;
        }

        /// <summary>
        /// Raised when a scanner is shown
        /// </summary>
        public event ScannerShow EvtScannerShow;

        /// <summary>
        /// Raised on startup when the PanelManager is enumerating
        /// forms that reside in the extension dirs.  The event subscriber
        /// should add scanner types that are not located in the extension
        /// directories.
        /// </summary>
        public event EventHandler EvtStartupAddForms;

        /// <summary>
        /// Returns the singleton instance of the PanelManager
        /// </summary>
        public static PanelManager Instance
        {
            get { return _instance ?? (_instance = new PanelManager()); }
        }

        /// <summary>
        /// Returns the currently visible Form object
        /// </summary>
        public Form CurrentForm
        {
            get { return _currentForm; }
        }

        /// <summary>
        /// Gets or sets the preferred names of groups of scanners to use.
        /// ACAT scanners can be grouped in any manner, each group has 
        /// a name.  This property contains an array of names of panel groups.
        /// </summary>
        public String [] PreferredPanelConfigNames
        {
            get { return PanelConfigMap.PreferredPanelConfigNames; }

            set { PanelConfigMap.PreferredPanelConfigNames = value; }
        }
        /// <summary>
        /// Add the form of the specified type to the form cache.
        /// </summary>
        /// <param name="type">the .NET type</param>
        public void AddFormToCache(Type type)
        {
            var guid = PanelConfigMap.GetFormId(type);
            PanelConfigMap.AddFormToCache(guid, type);
        }

        /// <summary>
        /// Closes the current form that is active
        /// </summary>
        public void CloseCurrentForm()
        {
            Windows.CloseForm(GetCurrentForm() as Form);
        }

        /// <summary>
        /// Closes the current panel.  Traces the 
        /// ancestors of the current panel until 
        /// there are no more parents.  Closes the
        /// topmost parent form
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
                    if (f != null && f == _currentForm)
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
            var startupArg = new StartupArg(panelClass)
            {
                ConfigFileName = PanelConfigMap.GetConfigFileForPanel(panelClass)
            };
            return CreatePanel(panelClass, String.Empty, startupArg);
        }

        /// <summary>
        /// Creates the panel with the specified panel class
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="title">title of the panel</param>
        /// <returns>the form for the panel</returns>
        public Form CreatePanel(String panelClass, String title)
        {
            StartupArg startupArg = new StartupArg(panelClass)
            {
                ConfigFileName = PanelConfigMap.GetConfigFileForPanel(panelClass)
            };
            return CreatePanel(panelClass, title, startupArg);
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
            if (form is IScannerPanel)
            {
                var scannerPanel = form as IScannerPanel;
                if (String.IsNullOrEmpty(startupArg.ConfigFileName))
                {
                    startupArg.ConfigFileName = PanelConfigMap.GetConfigFileForPanel(panelClass);
                }

                scannerPanel.Initialize(startupArg);
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
                Log.Debug("Could not find panel for " + panelClass + ". Using default " + panelConfigMapEntry.FormType.Name);
            }

            Log.Debug("panel: " + panelConfigMapEntry.FormType.Name);
            return createPanel(panelClass, panelTitle, panelConfigMapEntry.FormType, winHandle, focusedElement);
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
        /// Returns the currently visible panel Form
        /// </summary>
        /// <returns>form</returns>
        public IPanel GetCurrentForm()
        {
            return _currentForm as IPanel;
        }

        /// <summary>
        /// Returns the current panel.
        /// The current panel is the the last scanner that was
        /// not shown as a dialog.  Note that the Current Panel scanner may
        /// not be visible as it may have created child scanners as dialogs.
        /// </summary>
        /// <returns>The active panel</returns>
        public IPanel GetCurrentPanel()
        {
            return _currentPanel as IPanel;
        }

        /// <summary>
        /// Return the panel name of the currently
        /// active panel
        /// </summary>
        /// <returns>the name</returns>
        public String GetCurrentPanelName()
        {
            if (_currentForm is IScannerPanel)
            {
                IScannerPanel panel = (IScannerPanel)_currentForm;
                return panel.PanelClass;
            }

            return String.Empty;
        }

        /// <summary>
        /// Performs initialization. Walks the extension
        /// dirs and caches the Types of all the scanner/dialogs and menus.
        /// The Type will be used to create an instance using .NET
        /// relection.
        /// </summary>
        /// <param name="extensionDirs">extension dirs to walk</param>
        /// <returns>true on success</returns>
        public bool Init(IEnumerable<String> extensionDirs)
        {
            var retVal = PanelConfigMap.Load(extensionDirs);

            PanelConfigMap.Load(Preferences.ApplicationAssembly);

            if (EvtStartupAddForms != null)
            {
                EvtStartupAddForms(this, new EventArgs());
            }

            PanelConfigMap.CleanupOrphans();

            var configNames = CoreGlobals.AppPreferences.PreferredPanelConfigNames.Split(';');
            PreferredPanelConfigNames = configNames;

            return retVal;
        }
        /// <summary>
        /// Pause panel change requests.  This means any requests
        /// in the future to change panels from the 
        /// Agent Manager will not be honored. Call this to
        /// keep the current scanner locked.
        /// </summary>
        public void PausePanelChangeRequests()
        {
            Context.AppAgentMgr.PausePanelChangeRequests();
        }

        /// <summary>
        /// Resumes previously paused panel change requests. This means
        /// any requests in the future to change panels from the 
        /// Agent Manager will be honored.
        /// </summary>
        public void ResumePanelChangeRequests()
        {
            Context.AppAgentMgr.ResumePanelChangeRequests();
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
            return show(parent, panel, false);
        }

        /// <summary>
        /// Displays the panel
        /// </summary>
        /// <param name="form">panel to display</param>
        /// <returns></returns>
        public bool Show(IPanel form)
        {
            return show(null, form, false);
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

            var form = (Form) panel;

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
                    retVal = show((IPanel) _currentForm, panel, true);
                }
                else
                {
                    Log.Debug("Just showing " + form.GetType());
                    retVal = Show(panel);
                }
            }
            else
            {
                var parentForm = parent as Form;
                Log.Debug("showDialog parent: " + parentForm.Name + ", type: " + parentForm.GetType());
                retVal = show(parent, panel, true);
            }

            return retVal;
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
                }

                // Release unmanaged resources. 
            }

            _disposed = true;
        }

        /// <summary>
        /// Foreground window focus changed.  Let the active
        /// scanner know about this
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppAgent_EvtFocusChanged(object sender, FocusChangedEventArgs e)
        {
            if (_currentPanel is IScannerPanel)
            {
                ((IScannerPanel)_currentPanel).OnFocusChanged(e.WindowActivityInfo);
            }
        }

        /// <summary>
        /// Event handler for request to display a scanner. The 
        /// arg parameter contains information about which scanner
        /// to display
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        private void AppAgent_EvtPanelRequest(object sender, PanelRequestEventArgs arg)
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
                    Log.Debug("CurrentPanel is null. returning");
                    return;
                }

                Log.Debug("**** Not a new window.  CurrentPanel is " + currentScanner.PanelClass +
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
                }
            }
        }

        /// <summary>
        /// Creates an audit log entry for the scanner activity
        /// </summary>
        /// <param name="form">the panel form</param>
        /// <param name="operation">type of activity</param>
        private void auditLogScannerEvent(Form form, String operation)
        {
            var title = String.Empty;
            if (form is ContextualMenuBase)
            {
                var menu = (ContextualMenuBase)form;
                title = menu.Title;
            }

            String panelClass;

            if (form is ContextualMenuBase)
            {
                panelClass = PanelClasses.PanelCategory.ContextualMenu.ToString();
            }
            else if (form is IScannerPanel)
            {
                panelClass = PanelClasses.PanelCategory.Scanner.ToString();
            }
            else if (form is IDialogPanel)
            {
                panelClass = PanelClasses.PanelCategory.Dialog.ToString();
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
                ConfigFileName = PanelConfigMap.GetConfigFileForPanel(arg.PanelClass),
                Arg = arg.RequestArg
            };

            Log.Debug("panelClass:  " + arg.PanelClass + ", ConfigFIle: " + startupArg.ConfigFileName);
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

            form.FormClosed -= panel_FormClosed;

            Form parentForm = form.Owner;
            String panelClass = PanelClasses.None;
            Form[] array = form.OwnedForms;

            auditLogScannerEvent(form, "close");

            Log.Debug("number of owned forms: " + array.Length);

            // close all the forms this panel owns
            while (true)
            {
                Form[] a = form.OwnedForms;
                if (a.Length == 0)
                {
                    Log.Debug(form.Name + ": No more owned forms. Breaking");
                    break;
                }

                Log.Debug("Removing owned form from list. " + a[0].Name);
                form.RemoveOwnedForm(a[0]);
                Log.Debug("Calling close on " + a[0].Name);
                Windows.CloseForm(a[0]);
            }

            Log.Debug("form Name: " + form.Name + ", type: " + form.GetType());

            if (_currentPanel is IScannerPanel)
            {
                panelClass = ((IScannerPanel)_currentPanel).PanelClass;
            }

            // Exit the application if instructed to do so.
            if (Context.AppQuit)
            {
                Application.ExitThread();
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
                    Log.Debug("Calling OnResume on parentForm " + parentForm.Name);
                    parentPanel.OnResume();
                    Log.Debug("parentform is not null. Setting _currentPanel to " + parentForm.Name +
                                ", type: " + parentForm.GetType());

                    _currentPanel = parentForm;
                    _currentForm = parentForm;

                    auditLogScannerEvent(parentForm, "show");
                }
            }
            else
            {
                Log.Debug("parentform is null");
            }

            // Inform the AgentManager that a scanner just closed
            if (!PanelConfigMap.AreEqual(panelClass, PanelClasses.None))
            {
                Log.Debug("Calling OnPanelClosed for " + panelClass);
                Context.AppAgentMgr.OnPanelClosed(panelClass);
            }

            Log.Debug("Setting CLOSED for " + form.Name);

            (form as IPanel).SyncObj.Status = SyncLock.StatusValues.Closed;

            Log.Debug("Exit " + form.Name);
        }

        /// <summary>
        /// Handler for the event that is raised to indicate
        /// that a scanner was just shown. Notify subscribers
        /// about this
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void ScannerCommon_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (EvtScannerShow != null)
            {
                EvtScannerShow(sender, arg);
            }
        }

        /// <summary>
        /// Shows 'panel'.  'parent' is the form making the call to show.
        /// If 'showAsDialog' is true, shows panel as the Dialog with 'parent'
        /// as the parent form. Else, just shows 'panel'. Also Pauses the parent
        /// scanner.  It will be Resumed when the panel is closed.
        /// </summary>
        /// <param name="parent">parent panel</param>
        /// <param name="panel">panel to show</param>
        /// <param name="showAsDialog">true to show as dialog</param>
        /// <returns>true on success</returns>
        private bool show(IPanel parent, IPanel panel, bool showAsDialog)
        {
            Form panelForm = (Form)panel;
            Form parentForm = (Form)parent;

            Log.Debug("parentForm: " + ((parentForm != null) ? parentForm.Name : " null") + 
                        ".  panel: " + panelForm.Name);

            panelForm.FormClosed += panel_FormClosed;

            if (parent != null)
            {
                parent.OnPause();

                if (showAsDialog)
                {
                    Log.Debug("Showing Dialog" + panelForm.Name + " with parent " + parentForm.Name);
                    _currentForm = panelForm;
                    auditLogScannerEvent(panelForm, "show");
                    Windows.ShowDialog(parentForm, panelForm);
                }
                else
                {
                    Log.Debug("Showing " + panelForm.Name);
                    _currentForm = panelForm;
                    Log.Debug("parent is not null. Setting _currentPanel to " + panelForm.Name + 
                                ", type: " + panelForm.GetType());
                    _currentPanel = panelForm;
                    auditLogScannerEvent(panelForm, "show");
                    Windows.Show(parentForm, panelForm);
                }
            }
            else
            {
                Log.Debug("showing " + panelForm.Name + ", parent is null");
                Log.Debug("parent is null. Setting _currentPanel to " + panelForm.Name + 
                            ", type: " + panelForm.GetType());

                _currentPanel = panelForm;
                _currentForm = panelForm;
                auditLogScannerEvent(panelForm, "show");
                Windows.ShowForm(panelForm);
            }

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
                Log.Debug("FORM IS NULL!!");
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