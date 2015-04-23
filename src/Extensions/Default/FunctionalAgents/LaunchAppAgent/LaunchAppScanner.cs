////////////////////////////////////////////////////////////////////////////
// <copyright file="LaunchAppScanner.cs" company="Intel Corporation">
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
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Extension;

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

namespace ACAT.Extensions.Hawking.FunctionalAgents.LaunchApp
{
    /// <summary>
    /// Displays a list of apps. User selets an app and the
    /// agent launches the app
    /// </summary>
    [DescriptorAttribute("D946E1D6-5D67-40E6-B27F-F8D062DE5450", "LaunchAppScanner", "Launch Applications Scanner")]
    public partial class LaunchAppScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// Max length of the title. If the len is greater than
        /// this number, ellipses are appended.
        /// </summary>
        private const int MaxWindowTitleLength = 60;

        /// <summary>
        /// Command dispatcher
        /// </summary>
        private readonly RunCommandDispatcher _dispatcher;

        /// <summary>
        /// Used to invoke methods/properties of this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The keyboard actuator object
        /// </summary>
        private readonly KeyboardActuator _keyboardActuator;

        /// <summary>
        /// List of all apps
        /// </summary>
        private List<AppInfo> _allAppsList;

        /// <summary>
        /// List of apps that match the filter specified by the user
        /// </summary>
        private List<AppInfo> _appsList;

        /// <summary>
        /// Scanner form with which this form is docked
        /// </summary>
        private Form _dockedWithForm;

        /// <summary>
        /// The number of entries per page
        /// </summary>
        private int _entriesPerPage;

        /// <summary>
        /// The total number of pages
        /// </summary>
        private int _numPages;

        /// <summary>
        /// The current page number
        /// </summary>
        private int _pageNumber;

        /// <summary>
        /// Widget that displays the current page number
        /// </summary>
        private Widget _pageNumberWidget;

        /// <summary>
        /// Index in the list of the first entry in the current
        /// pageful of entries
        /// </summary>
        private int _pageStartIndex;

        /// <summary>
        /// The scannercommon object
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// Holds the current sort order
        /// </summary>
        private SortOrder _sortOrder = SortOrder.Ascending;

        /// <summary>
        /// Widget that displays the sort order
        /// </summary>
        private Widget _sortOrderWidget;

        /// <summary>
        /// Number of widgets in the list that will hold the
        /// info about the apps
        /// </summary>
        private int _tabStopButtonCount;

        /// <summary>
        /// ensures this form retains focus
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public LaunchAppScanner()
        {
            InitializeComponent();
            PanelClass = "LaunchAppScanner";
            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;
            _invoker = new ExtensionInvoker(this);
            KeyPreview = true;

            FormClosing += LaunchAppScanner_FormClosing;
            Shown += LaunchAppScanner_Shown;
            KeyDown += LaunchAppScanner_KeyDown;
            LocationChanged += LaunchAppScanner_LocationChanged;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new RunCommandDispatcher(this);
        }

        /// <summary>
        /// For the event raised to quit
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="args">event args</param>
        public delegate void QuitEventDelegate(object sender, EventArgs args);

        /// <summary>
        /// For the event raised to launch an app
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="appInfo">info about the app</param>
        internal delegate void LaunchAppDelegate(object sender, AppInfo appInfo);

        /// <summary>
        /// For the event to quit
        /// </summary>
        public event QuitEventDelegate EvtQuit;

        /// <summary>
        /// Raised when the user wants to launch the app
        /// </summary>
        internal event LaunchAppDelegate EvtLaunchApp;

        /// <summary>
        /// How to sort the list of apps
        /// </summary>
        private enum SortOrder
        {
            Ascending,
            Descending
        }

        /// <summary>
        /// Gets dispatcher to run commands
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets filter used to match process names
        /// </summary>
        public String FilterByProcessName { get; set; }

        /// <summary>
        /// Gets the form
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class of the scanner
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets scanner common object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller object for this scanner
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Sets form styles
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
                return setFormStyles(base.CreateParams);
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="arg">not used</param>
        /// <returns>not used</returns>
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            return false;
        }

        /// <summary>
        /// Clears the search filter
        /// </summary>
        public void ClearFilter()
        {
            Invoke(new MethodInvoker(delegate()
            {
                if (SearchFilter.Text.Length > 0 && DialogUtils.ConfirmScanner("Clear filter?"))
                {
                    SearchFilter.Text = String.Empty;
                }
            }));
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns>invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Initialzes this class
        /// </summary>
        /// <param name="startupArg">startup args</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _scannerCommon = new ScannerCommon(this) { PositionSizeController = { AutoPosition = false } };

            if (!_scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            PanelManager.Instance.EvtScannerShow += Instance_EvtScannerShow;
            return true;
        }

        /// <summary>
        /// Returns if the filter string is empty
        /// </summary>
        /// <returns>true if so</returns>
        public bool IsFilterEmpty()
        {
            bool retVal = true;
            Invoke(new MethodInvoker(delegate()
            {
                retVal = (SearchFilter.Text.Length == 0);
            }));
            return retVal;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="monitorInfo"></param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="eventArg">event arguments</param>
        /// <returns>true always</returns>
        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnResume()
        {
        }

        /// <summary>
        /// Runs the specified command
        /// </summary>
        /// <param name="command">the command to execute</param>
        /// <param name="handled">true if it was handled</param>
        public void OnRunCommand(string command, ref bool handled)
        {
            if (command.StartsWith("highlight", StringComparison.InvariantCultureIgnoreCase))
            {
                handleHighlight(command);
            }

            if (command.StartsWith("select", StringComparison.InvariantCultureIgnoreCase))
            {
                handleSelect(command);
            }
            else
            {
                Log.Debug("unlandled command " + command);
                handled = false;
            }
        }

        /// <summary>
        /// Called when widget is actuated
        /// </summary>
        /// <param name="widget">which one</param>
        /// <param name="handled">are we handling it?</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            if (widget is TabStopScannerButton)
            {
                handled = true;
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="widget"></param>
        public void SetTargetControl(Form parent, Widget widget)
        {
        }

        /// <summary>
        /// Invoked when there is a request to run a command. This
        /// could as a result of the user activating a button on the
        /// scanner and there is a command associated with the button
        /// in the animation file
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="handled">was this handled?</param>
        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            removeWatchdogs();

            PanelManager.Instance.EvtScannerShow -= Instance_EvtScannerShow;

            _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window proc
        /// </summary>
        /// <param name="m"></param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _scannerCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Key press event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _keyboardActuator_EvtKeyPress(object sender, KeyPressEventArgs e)
        {
            if (EvtQuit != null)
            {
                int c = (int)e.KeyChar;
                if (c == 27)
                {
                    EvtQuit.BeginInvoke(this, null, null, null);
                }
            }
        }

        /// <summary>
        /// Actuate the widget
        /// </summary>
        /// <param name="widgetName">which one?</param>
        private void actuateWidget(String widgetName)
        {
            var widget = _scannerCommon.GetRootWidget().Finder.FindChild(widgetName);
            if (widget != null)
            {
                object obj = widget.UserData;
                if (obj is ItemTag)
                {
                    handleSelect((ItemTag)obj);
                }

                highlightOff();
            }
        }

        /// <summary>
        /// Docks to the companian scanner
        /// </summary>
        /// <param name="scanner">companian scanner</param>
        private void dockToScanner(Form scanner)
        {
            if (scanner is IScannerPanel)
            {
                Windows.DockWithScanner(this, scanner, Context.AppWindowPosition);
            }
        }

        /// <summary>
        /// Enables watchdogs
        /// </summary>
        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }
        }

        /// <summary>
        /// Selects apps whose names match with the specified
        /// filter
        /// </summary>
        /// <param name="list">list of apps</param>
        /// <param name="filter">search filter</param>
        /// <returns>matching list</returns>
        private List<AppInfo> filterApps(List<AppInfo> list, String filter)
        {
            String trimFilter = filter.Trim();

            return list.Where(f => f.Name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Returns a sorted list based on the sort order
        /// </summary>
        /// <param name="order">sort order</param>
        /// <returns>sorted list</returns>
        private List<AppInfo> getAppList(SortOrder order)
        {
            return sortFiles(LaunchAppAgent.Settings.Applications.ToList(), order);
        }

        /// <summary>
        /// Display next pageful of entries in the list
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _appsList.Count())
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshWindowList();
                }
            }
        }

        /// <summary>
        /// Display previous pageful of entries in the list
        /// </summary>
        private void gotoPreviousPage()
        {
            if (_pageNumber >= 1)
            {
                int index = _pageStartIndex - _entriesPerPage;
                if (index < 0)
                {
                    index = 0;
                }

                _pageStartIndex = index;
                _pageNumber--;
                refreshWindowList();
            }
        }

        /// <summary>
        /// Highlight command to highlight a list item
        /// </summary>
        /// <param name="cmd">which one to highlight?</param>
        private void handleHighlight(String cmd)
        {
            if (cmd.Equals("highlight_off", StringComparison.InvariantCultureIgnoreCase))
            {
                highlightOff();
            }
            else
            {
                int index = cmd.LastIndexOf('_');
                if (index >= 0 && index < cmd.Length - 1)
                {
                    var widgetName = "Item" + cmd.Substring(index + 1);
                    highlight(widgetName);
                }
            }
        }

        /// <summary>
        /// User made a selection. Handle it
        /// </summary>
        /// <param name="cmd">index of the widget selected</param>
        private void handleSelect(String cmd)
        {
            int index = cmd.LastIndexOf('_');
            if (index >= 0 && index < cmd.Length - 1)
            {
                actuateWidget("Item" + cmd.Substring(index + 1));
            }
        }

        /// <summary>
        /// Handle the selection - navigate, launch app etc
        /// </summary>
        /// <param name="itemTag">item tag of selected item</param>
        private void handleSelect(ItemTag itemTag)
        {
            switch (itemTag.DataType)
            {
                case ItemTag.ItemType.NextPage:
                    gotoNextPage();
                    break;

                case ItemTag.ItemType.PreviousPage:
                    gotoPreviousPage();
                    break;

                case ItemTag.ItemType.OrderBy:
                    switchSortOrder();
                    break;

                case ItemTag.ItemType.App:
                    if (itemTag.ApplicationInfo != null)
                    {
                        if (DialogUtils.ConfirmScanner("Launch " + itemTag.ApplicationInfo.Name + "?"))
                        {
                            if (EvtLaunchApp != null)
                            {
                                EvtLaunchApp.BeginInvoke(this, itemTag.ApplicationInfo, null, null);
                                return;
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Highlight the specified widget
        /// </summary>
        /// <param name="widgetName">name of the widget</param>
        private void highlight(String widgetName)
        {
            _scannerCommon.GetRootWidget().HighlightOff();
            var widget = _scannerCommon.GetRootWidget().Finder.FindChild(widgetName);
            if (widget != null)
            {
                widget.HighlightOn();
            }
        }

        /// <summary>
        /// Turn hilight off for all widgets
        /// </summary>
        private void highlightOff()
        {
            _scannerCommon.GetRootWidget().HighlightOff();
        }

        /// <summary>
        /// Invoked when companian scanner is displayed. Dock to it
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void Instance_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (arg.Scanner != this)
            {
                _dockedWithForm = arg.Scanner.Form;
                dockToScanner(arg.Scanner.Form);
            }
        }

        /// <summary>
        /// The form has loaded.  Initialize
        /// </summary>
        private void LauncAppScanner_Load(object sender, EventArgs e)
        {
            enableWatchdogs();

            _scannerCommon.OnLoad();

            var list = new List<Widget>();
            _scannerCommon.GetRootWidget().Finder.FindChild(typeof(TabStopScannerButton), list);

            foreach (var widget in list)
            {
                widget.EvtMouseClicked += new WidgetEventDelegate(widget_EvtMouseClicked);
            }

            _tabStopButtonCount = list.Count;

            _sortOrderWidget = _scannerCommon.GetRootWidget().Finder.FindChild("SortOrderIcon");
            _pageNumberWidget = _scannerCommon.GetRootWidget().Finder.FindChild("PageNumber");

            SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;

            loadAppsList();

            _scannerCommon.GetRootWidget().HighlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void LaunchAppScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void LaunchAppScanner_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
            {
                gotoNextPage();
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                gotoPreviousPage();
            }
        }

        /// <summary>
        /// Don't allow window to be moved. Re-dock
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void LaunchAppScanner_LocationChanged(object sender, EventArgs e)
        {
            if (_dockedWithForm != null)
            {
                dockToScanner(_dockedWithForm);
            }
        }

        /// <summary>
        /// Set focus to this form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void LaunchAppScanner_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(this.Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Loads a list of apps to launch and refreshes
        /// the UI
        /// </summary>
        private void loadAppsList()
        {
            _allAppsList = getAppList(_sortOrder);
            _appsList = filterApps(_allAppsList, Windows.GetText(SearchFilter));

            if (_tabStopButtonCount >= 3)
            {
                _entriesPerPage = _tabStopButtonCount - 2;
                refreshWindowList();
            }
        }

        /// <summary>
        /// Refreshes the window list with a list of apps
        /// </summary>
        private void refreshWindowList()
        {
            var list = new List<Widget>();
            _scannerCommon.GetRootWidget().Finder.FindChild(typeof(TabStopScannerButton), list);

            int count = list.Count();
            if (count >= 3)
            {
                foreach (var button in list)
                {
                    button.UserData = null;
                    button.SetText(String.Empty);
                }

                _entriesPerPage = count - 2;
                _numPages = _appsList.Count() / _entriesPerPage;

                if ((_appsList.Count() % _entriesPerPage) != 0)
                {
                    _numPages++;
                }

                updateStatusBar();

                if (!_appsList.Any())
                {
                    (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25 });
                    list[0].SetText("------------- APPS LIST IS EMPTY -------------");
                    return;
                }

                int ii = 0;

                int displayIndex = (ii + 1) % 10;

                (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25 });
                if (_pageNumber == 0)
                {
                    list[ii].UserData = new ItemTag(ItemTag.ItemType.OrderBy);
                    if (_sortOrder == SortOrder.Ascending)
                    {
                        list[ii].SetText(displayIndex + ".\t------------- SORT Z-A -------------");
                    }
                    else
                    {
                        list[ii].SetText(displayIndex + ".\t------------- SORT A-Z -------------");
                    }
                }
                else
                {
                    list[ii].UserData = new ItemTag(ItemTag.ItemType.PreviousPage);
                    list[ii].SetText(displayIndex + ".\t------------- PREVIOUS PAGE  -------------");
                }

                ii++;

                for (int jj = _pageStartIndex; jj < _appsList.Count && ii < count - 1; ii++, jj++)
                {
                    displayIndex = (ii + 1) % 10;
                    (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25, 400 });
                    list[ii].UserData = new ItemTag(_appsList[jj]);
                    String title = _appsList[jj].Name;
                    if (title.Length > MaxWindowTitleLength)
                    {
                        title = title.Substring(0, MaxWindowTitleLength) + "...";
                    }

                    list[ii].SetText(displayIndex + ".\t" + title);
                }

                Log.Debug("_pageNumber: " + _pageNumber + ", _numPages: " + _numPages);

                if (_pageNumber < _numPages - 1)
                {
                    displayIndex = (ii + 1) % 10;
                    (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25, 400 });
                    list[ii].UserData = new ItemTag(ItemTag.ItemType.NextPage);
                    list[ii].SetText(displayIndex + ".\t------------- NEXT PAGE  -------------");
                    ii++;
                }

                for (; ii < count; ii++)
                {
                    list[ii].SetText(String.Empty);
                    list[ii].UserData = null;
                }
            }
        }

        /// <summary>
        /// Disables watchdogs
        /// </summary>
        private void removeWatchdogs()
        {
            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
                _windowActiveWatchdog = null;
            }
        }

        /// <summary>
        /// Search filter text changed
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SearchFilter_TextChanged(object sender, EventArgs e)
        {
            _pageNumber = 0;
            _pageStartIndex = 0;
            _appsList = filterApps(_allAppsList, Windows.GetText(SearchFilter));
            refreshWindowList();
        }

        /// <summary>
        /// Sets the style of this form
        /// </summary>
        /// <param name="createParams">parms</param>
        /// <returns>The form style</returns>
        private CreateParams setFormStyles(CreateParams createParams)
        {
            createParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_COMPOSITED;
            return createParams;
        }

        /// <summary>
        /// Sorts the list by the sort order
        /// </summary>
        /// <param name="list">input list</param>
        /// <param name="order">order</param>
        /// <returns>sorted list</returns>
        private List<AppInfo> sortFiles(List<AppInfo> list, SortOrder order)
        {
            switch (order)
            {
                case SortOrder.Descending:
                    return list.OrderByDescending(f => f.Name).ToList();

                case SortOrder.Ascending:
                    return list.OrderBy(f => f.Name).ToList();

                default:
                    return list;
            }
        }

        /// <summary>
        /// Change the sort order
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SortOrderIcon_Click(object sender, EventArgs e)
        {
            switchSortOrder();
        }

        /// <summary>
        /// Switch the sort order and reload the list
        /// </summary>
        private void switchSortOrder()
        {
            _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

            _pageNumber = 0;
            _pageStartIndex = 0;
            loadAppsList();
            _appsList = filterApps(_allAppsList, Windows.GetText(SearchFilter));
            refreshWindowList();
        }

        /// <summary>
        /// Updates the status bar with the page number info
        /// </summary>
        private void updateStatusBar()
        {
            if (_sortOrderWidget != null)
            {
                var text = String.Empty;
                if (!_appsList.Any())
                {
                    text = String.Empty;
                }
                else if (_sortOrder == SortOrder.Ascending)
                {
                    text = "\u003A";
                }
                else
                {
                    text = "\u003B";
                }

                _sortOrderWidget.SetText(text);
            }

            if (_pageNumberWidget != null)
            {
                String text = _appsList.Any() ? "Page " + (_pageNumber + 1) + " of " + _numPages : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// User clicked on a widget with the mouse
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtMouseClicked(object sender, WidgetEventArgs e)
        {
            actuateWidget(e.SourceWidget.Name);
        }

        /// <summary>
        /// Keeps track of meta data associated with
        /// each list item.
        /// </summary>
        private class ItemTag
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="type">Type of the data</param>
            public ItemTag(ItemType type)
            {
                DataType = type;
                ApplicationInfo = null;
            }

            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="info">application info</param>
            public ItemTag(AppInfo info)
            {
                DataType = ItemType.App;
                ApplicationInfo = info;
            }

            /// <summary>
            /// which type of item does this represent?
            /// </summary>
            public enum ItemType
            {
                OrderBy,
                PreviousPage,
                NextPage,
                App
            }

            /// <summary>
            /// Gets the app info associated with this item
            /// </summary>
            public AppInfo ApplicationInfo { get; private set; }

            /// <summary>
            /// Gets the type of item
            /// </summary>
            public ItemType DataType { get; private set; }
        }
    }
}