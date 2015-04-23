////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchWindowsScanner.cs" company="Intel Corporation">
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

namespace ACAT.Extensions.Hawking.FunctionalAgents.SwitchWindows
{
    /// <summary>
    /// Form that displays a list of active windows. User can sort
    /// the windows by name, select a window to activate.  User can
    /// also filter the list through a search filter.
    /// </summary>
    [DescriptorAttribute("FAE01845-95BD-4D52-9C57-3653888F1EFF", "SwitchWindowsScanner", "Switch Windows Scanner")]
    public partial class SwitchWindowsScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// Max length of the window title. If title exceeds this,
        /// ellipses are appended
        /// </summary>
        private const int MaxWindowTitleLength = 60;

        /// <summary>
        /// The command dispatcher object
        /// </summary>
        private readonly RunCommandDispatcher _dispatcher;

        /// <summary>
        /// Enables invoking methods and properties in this form
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The keyboard actuator object
        /// </summary>
        private readonly KeyboardActuator _keyboardActuator;

        /// <summary>
        /// List of all windows
        /// </summary>
        private List<EnumWindows.WindowInfo> _allWindowsList;

        /// <summary>
        /// Scanner to which this form is docked
        /// </summary>
        private Form _dockedWithForm;

        /// <summary>
        /// Entres per page
        /// </summary>
        private int _entriesPerPage;

        /// <summary>
        /// THe total number of pages
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
        /// Index into the list of the first entry in the current page
        /// </summary>
        private int _pageStartIndex;

        /// <summary>
        /// The scannercommon object
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// The current sort order
        /// </summary>
        private SortOrder _sortOrder = SortOrder.Ascending;

        /// <summary>
        /// Widget that displays the current sort order
        /// </summary>
        private Widget _sortOrderWidget;

        /// <summary>
        /// Number of tabstop widgets.  Each holds a info for
        /// one window
        /// </summary>
        private int _tabStopButtonCount;

        /// <summary>
        /// Ensures this form keeps focus
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// List of windows that match filter
        /// </summary>
        private List<EnumWindows.WindowInfo> _windowsList;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SwitchWindowsScanner()
        {
            InitializeComponent();

            PanelClass = "SwitchWindowsScanner";

            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;

            _invoker = new ExtensionInvoker(this);
            KeyPreview = true;

            FormClosing += SwitchWindowsScanner_FormClosing;
            KeyDown += SwitchWindowsScanner_KeyDown;
            LocationChanged += OnLocationChanged;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new RunCommandDispatcher(this);
        }

        /// <summary>
        /// For the event raised when the user selects a window
        /// to activate
        /// </summary>
        /// <param name="sender">this object</param>
        /// <param name="wInfo">info about the selected window</param>
        public delegate void ActivateWindowDelegate(object sender, EnumWindows.WindowInfo wInfo);

        /// <summary>
        /// For the event to indiate we are done
        /// </summary>
        public delegate void DoneEvent();

        /// <summary>
        /// Raised when the user selects a window to activate
        /// </summary>
        public event ActivateWindowDelegate EvtActivateWindow;

        /// <summary>
        /// Raised when we are done
        /// </summary>
        public event DoneEvent EvtDone;

        /// <summary>
        /// How to sort the windows in the list?
        /// </summary>
        private enum SortOrder
        {
            Ascending,
            Descending
        }

        /// <summary>
        /// Gets the command dispatcher object for RunCommand
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
        /// Gets or sets a filter to select only those windows
        /// that belong to the specfied process (eg all notepad windows)
        /// </summary>
        public String FilterByProcessName { get; set; }

        /// <summary>
        /// Gets this form
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class of this scanner
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Returns the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller object
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Sets the form style
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
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            return false;
        }

        /// <summary>
        /// Clears the search filter box
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
        /// Gets the extension invoker object
        /// </summary>
        /// <returns>the object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Initialize this class
        /// </summary>
        /// <param name="startupArg">startup argument</param>
        /// <returns></returns>
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
        /// Returns if the filter is empty
        /// </summary>
        /// <returns>true if it is</returns>
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
        /// <param name="eventArg"></param>
        /// <returns></returns>
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
        /// Invoked when there is a request to run a command. This
        /// could as a result of the user activating a button on the
        /// scanner and there is a command associated with the button
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="handled">was this handled?</param>
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
        /// Widget was actuated
        /// </summary>
        /// <param name="widget">widget that was actuated</param>
        /// <param name="handled">was it handled?</param>
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
        /// Release resources
        /// </summary>
        /// <param name="e">args</param>
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
        /// <param name="m">windows message</param>
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
            if (EvtDone != null)
            {
                int c = e.KeyChar;
                if (c == 27)
                {
                    EvtDone.BeginInvoke(null, null);
                }
            }
        }

        /// <summary>
        /// Find the widget that was actuated and act on it
        /// </summary>
        /// <param name="widgetName">name of the widget</param>
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
        /// Docks this form to the companian scanner
        /// </summary>
        /// <param name="scanner">the scanner form</param>
        private void dockToScanner(Form scanner)
        {
            if (scanner is IScannerPanel)
            {
                Windows.DockWithScanner(this, scanner, Context.AppWindowPosition);
            }
        }

        /// <summary>
        /// Enable watchdogs to keep this window in focus
        /// </summary>
        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }
        }

        /// <summary>
        /// Filters the list by the specified filter and returns only
        /// those entries that match
        /// </summary>
        /// <param name="windowList">input list</param>
        /// <param name="filter">filter</param>
        /// <returns>filtered list</returns>
        private List<EnumWindows.WindowInfo> filterWindows(IEnumerable<EnumWindows.WindowInfo> windowList, String filter)
        {
            var filteredList = new List<EnumWindows.WindowInfo>();
            foreach (var window in windowList)
            {
                if (window.Title.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase))
                {
                    filteredList.Add(window);
                }
            }

            return filteredList;
        }

        /// <summary>
        /// Gets a list of active windows sorted on the
        /// indcated sort order
        /// </summary>
        /// <param name="order">sort order</param>
        /// <returns>sorted list</returns>
        private List<EnumWindows.WindowInfo> getWindowList(SortOrder order)
        {
            var processName = FilterByProcessName.Trim();

            var windowList = EnumWindows.Enumerate();

            if (!String.IsNullOrEmpty(processName))
            {
                var filteredList = new List<EnumWindows.WindowInfo>();

                for (int ii = 0; ii < windowList.Count; ii++)
                {
                    var process = WindowActivityMonitor.GetProcessForWindow(windowList[ii].Handle);
                    if (String.Compare(process.ProcessName, FilterByProcessName, true) == 0)
                    {
                        filteredList.Add(windowList[ii]);
                    }
                }

                windowList = filteredList;
            }

            var sortedList = sortWindowList(windowList, order);
            if (String.IsNullOrEmpty(processName))
            {
                IntPtr desktopHWnd = User32Interop.GetDesktopWindow();
                if (desktopHWnd != null)
                {
                    sortedList.Insert(0, new EnumWindows.WindowInfo(desktopHWnd, "Show Desktop"));
                }
            }

            return sortedList;
        }

        /// <summary>
        /// Show the next pageful of entries in the
        /// window list
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _windowsList.Count())
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshWindowList();
                }
            }
        }

        /// <summary>
        /// Show the previous pageful of entries in the
        /// window list
        /// </summary>
        private void gotoPreviousPage()
        {
            if (_pageNumber < 1)
            {
                return;
            }

            int index = _pageStartIndex - _entriesPerPage;
            if (index < 0)
            {
                index = 0;
            }

            _pageStartIndex = index;
            _pageNumber--;
            refreshWindowList();
        }

        /// <summary>
        /// Highlight a widget. Cmd contains the
        /// index number of the widget in the list
        /// </summary>
        /// <param name="cmd">which one to highlight</param>
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
        /// User selected a window on the list.  Actuate
        /// the widget
        /// </summary>
        /// <param name="cmd">index of the widget actuated</param>
        private void handleSelect(String cmd)
        {
            int index = cmd.LastIndexOf('_');
            if (index >= 0 && index < cmd.Length - 1)
            {
                actuateWidget("Item" + cmd.Substring(index + 1));
            }
        }

        /// <summary>
        /// Perform the operation - page through the list,
        /// activate a window etc
        /// </summary>
        /// <param name="itemTag">Meta data about seleted item</param>
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

                case ItemTag.ItemType.Window:
                    if (itemTag.WInfo != null)
                    {
                        if (!User32Interop.IsWindow(itemTag.WInfo.Handle) || !User32Interop.IsWindowVisible(itemTag.WInfo.Handle))
                        {
                            DialogUtils.ShowTimedDialog(this, "Window does not exist");
                        }
                        else if (DialogUtils.ConfirmScanner("Switch to " + itemTag.WInfo.Title + "?"))
                        {
                            if (EvtActivateWindow != null)
                            {
                                EvtActivateWindow.BeginInvoke(this, itemTag.WInfo, null, null);
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Highlight the indicated widget
        /// </summary>
        /// <param name="widgetName">name of the widget to highlight</param>
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
        /// Turn highlight off for all widgets
        /// </summary>
        private void highlightOff()
        {
            _scannerCommon.GetRootWidget().HighlightOff();
        }

        /// <summary>
        /// Invoked when the companain scanner is shown
        /// Dock this form to the scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Instance_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (arg.Scanner != this)
            {
                _dockedWithForm = arg.Scanner.Form;
                dockToScanner(arg.Scanner.Form);
            }
        }

        /// <summary>
        /// Get a list of active windows and display it
        /// in the list
        /// </summary>
        private void loadWindowList()
        {
            _allWindowsList = getWindowList(_sortOrder);
            _windowsList = filterWindows(_allWindowsList, Windows.GetText(SearchFilter));

            if (_tabStopButtonCount >= 3)
            {
                _entriesPerPage = _tabStopButtonCount - 2;
                refreshWindowList();
            }
        }

        /// <summary>
        /// Don't let the window move. Re-dock it to
        /// the companian scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void OnLocationChanged(object sender, EventArgs eventArgs)
        {
            if (_dockedWithForm != null)
            {
                dockToScanner(_dockedWithForm);
            }
        }

        /// <summary>
        /// Refresh the list of windows in the UI
        ///
        /// </summary>
        private void refreshWindowList()
        {
            var list = new List<Widget>();
            _scannerCommon.GetRootWidget().Finder.FindChild(typeof(TabStopScannerButton), list);

            int count = list.Count();
            if (count < 3)
            {
                return;
            }

            foreach (var button in list)
            {
                button.UserData = null;
                button.SetText(String.Empty);
            }

            // calculate how many pages, number of entries per page
            _entriesPerPage = count - 2;
            _numPages = _windowsList.Count() / _entriesPerPage;

            if ((_windowsList.Count() % _entriesPerPage) != 0)
            {
                _numPages++;
            }

            updateStatusBar();

            if (!_windowsList.Any())
            {
                (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25 });
                list[0].SetText("------------- NO ACTIVE WINDOWS -------------");
                return;
            }

            int ii = 0;

            int displayIndex = (ii + 1) % 10;

            // Set the first entry in the list
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

            // fill remaining entries except the last one
            for (int jj = _pageStartIndex; jj < _windowsList.Count && ii < count - 1; ii++, jj++)
            {
                displayIndex = (ii + 1) % 10;
                (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25, 400 });
                list[ii].UserData = new ItemTag(_windowsList[jj]);

                var title = _windowsList[jj].Title;
                if (title.Length > MaxWindowTitleLength)
                {
                    title = title.Substring(0, MaxWindowTitleLength) + "...";
                }

                list[ii].SetText(displayIndex + ".\t" + title);
            }

            Log.Debug("_pageNumber: " + _pageNumber + ", _numPages: " + _numPages);

            // set last entry
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

        /// <summary>
        /// Remove all the watchdogs
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
        /// Search filter text changed. Reload the list and
        /// display only those windows that match
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SearchFilter_TextChanged(object sender, EventArgs e)
        {
            _pageNumber = 0;
            _pageStartIndex = 0;
            _windowsList = filterWindows(_allWindowsList, Windows.GetText(SearchFilter));
            refreshWindowList();
        }

        /// <summary>
        /// Set the sytle of this form
        /// </summary>
        /// <param name="createParams"></param>
        /// <returns></returns>
        private CreateParams setFormStyles(CreateParams createParams)
        {
            createParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_COMPOSITED;
            return createParams;
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
        /// Sort the list by the word order
        /// </summary>
        /// <param name="windowsList">input list</param>
        /// <param name="order">order to sort</param>
        /// <returns>sorted list</returns>
        private List<EnumWindows.WindowInfo> sortWindowList(List<EnumWindows.WindowInfo> windowsList, SortOrder order)
        {
            switch (order)
            {
                case SortOrder.Descending:
                    return windowsList.OrderByDescending(f => f.Title).ToList();

                case SortOrder.Ascending:
                    return windowsList.OrderBy(f => f.Title).ToList();

                default:
                    return windowsList;
            }
        }

        /// <summary>
        /// Switch the sort order and re-display the list
        /// of windows
        /// </summary>
        private void switchSortOrder()
        {
            _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

            _pageNumber = 0;
            _pageStartIndex = 0;
            loadWindowList();
            _windowsList = filterWindows(_allWindowsList, Windows.GetText(SearchFilter));
            refreshWindowList();
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void SwitchWindowsScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Key down event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SwitchWindowsScanner_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageUp)
            {
                gotoPreviousPage();
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                gotoNextPage();
            }
        }

        /// <summary>
        /// Initialze the form
        /// </summary>
        private void SwitchWindowsScanner_Load(object sender, EventArgs e)
        {
            enableWatchdogs();

            _scannerCommon.OnLoad();

            var list = new List<Widget>();
            _scannerCommon.GetRootWidget().Finder.FindChild(typeof(TabStopScannerButton), list);

            foreach (var widget in list)
            {
                widget.EvtMouseClicked += widget_EvtMouseClicked;
            }

            _tabStopButtonCount = list.Count;

            _sortOrderWidget = _scannerCommon.GetRootWidget().Finder.FindChild("SortOrderIcon");
            _pageNumberWidget = _scannerCommon.GetRootWidget().Finder.FindChild("PageNumber");

            this.SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;
            Shown += SwitchWindowsScanner_Shown;

            loadWindowList();

            _scannerCommon.GetRootWidget().HighlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }
        }

        /// <summary>
        /// Set focus to this form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SwitchWindowsScanner_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(this.Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Update status bar with page number info
        /// </summary>
        private void updateStatusBar()
        {
            if (_sortOrderWidget != null)
            {
                String text;
                if (!_windowsList.Any())
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
                var text = _windowsList.Any() ? "Page " + (_pageNumber + 1) + " of " + _numPages : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// User clicked on a widget. Act on it
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtMouseClicked(object sender, WidgetEventArgs e)
        {
            actuateWidget(e.SourceWidget.Name);
        }

        /// <summary>
        /// Contains meta data about each window in the  list
        /// </summary>
        private class ItemTag
        {
            public ItemTag(ItemType type)
            {
                DataType = type;
                WInfo = null;
            }

            public ItemTag(EnumWindows.WindowInfo info)
            {
                DataType = ItemType.Window;
                WInfo = info;
            }

            public enum ItemType
            {
                OrderBy,
                PreviousPage,
                NextPage,
                Window
            }

            public ItemType DataType { get; private set; }

            public EnumWindows.WindowInfo WInfo { get; private set; }
        }
    }
}