////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchWindowsScanner.cs" company="Intel Corporation">
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

using ACAT.ACATResources;
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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.SwitchWindowsAgent
{
    /// <summary>
    /// Form that displays a list of active windows. User can sort
    /// the windows by name, select a window to activate.  User can
    /// also filter the list through a search filter.
    /// </summary>
    [DescriptorAttribute("52D33D6A-4254-4727-8291-DC9D26A51F4F",
                            "SwitchWindowsScanner",
                            "Switch Windows Scanner")]
    public partial class SwitchWindowsScanner : Form, IScannerPanel, IExtension
    {
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
        /// The scannercommon object
        /// </summary>
        private readonly ScannerCommon _scannerCommon;

        /// <summary>
        /// List of all windows
        /// </summary>
        private List<EnumWindows.WindowInfo> _allWindowsList;

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
        /// Widget that the user clicks to resort
        /// </summary>
        private Widget _sortButton;

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
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            PanelClass = "SwitchWindowsScanner";

            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;

            _invoker = new ExtensionInvoker(this);
            KeyPreview = true;

            FormClosing += SwitchWindowsScanner_FormClosing;
            KeyDown += SwitchWindowsScanner_KeyDown;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new RunCommandDispatcher(this);

            statusStrip1.SizingGrip = false;
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
        /// Event raised to display the alphabet scanner
        /// </summary>
        public event EventHandler EvtShowScanner;

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
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _scannerCommon; } }

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
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            arg.Handled = true;

            switch (arg.Command)
            {
                case "CmdPrevPage":
                    arg.Enabled = (_pageNumber != 0);
                    break;

                case "CmdNextPage":
                    arg.Enabled = (_numPages != 0 && (_pageNumber + 1) != _numPages);
                    break;

                case "Back":
                case "CmdDeletePrevWord":
                case "WindowListClearFilter":
                    arg.Handled = true;
                    arg.Enabled = !IsFilterEmpty();
                    break;

                case "WindowListSort":
                case "WindowListSearch":
                    arg.Handled = true;
                    arg.Enabled = (_windowsList != null && _windowsList.Any());
                    break;

                case "CmdPrevChar":
                case "CmdNextChar":
                    arg.Handled = true;
                    arg.Enabled = true;
                    break;

                default:
                    arg.Handled = false;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Clears the search filter box
        /// </summary>
        public void ClearFilter()
        {
            Invoke(new MethodInvoker(delegate
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
            _scannerCommon.PositionSizeController.AutoPosition = true;

            if (!_scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            Windows.EvtWindowPositionChanged += Windows_EvtWindowPositionChanged;

            return true;
        }

        /// <summary>
        /// Returns if the filter is empty
        /// </summary>
        /// <returns>true if it is</returns>
        public bool IsFilterEmpty()
        {
            bool retVal = true;
            Invoke(new MethodInvoker(delegate
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
            PanelCommon.AnimationManager.Pause();
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
            _scannerCommon.PositionSizeController.AutoSetPosition();

            PanelCommon.AnimationManager.Resume();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="handled">was this handled?</param>
        public void OnRunCommand(string command, ref bool handled)
        {
        }

        /// <summary>
        /// Widget was actuated
        /// </summary>
        /// <param name="widget">widget that was actuated</param>
        /// <param name="handled">was it handled?</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            actuateWidget(widget, ref handled);
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
        /// Size of the client changed
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            _scannerCommon.OnClientSizeChanged();
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="e">args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            removeWatchdogs();

            Windows.EvtWindowPositionChanged -= Windows_EvtWindowPositionChanged;

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
            if (_scannerCommon != null)
            {
                if (_scannerCommon.HandleWndProc(m))
                {
                    return;
                }
            }

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
        private void actuateWidget(Widget widget, ref bool handled)
        {
            handleWidgetSelection(widget, ref handled);
            highlightOff();
        }

        /// <summary>
        /// Docks this form to the companian scanner
        /// </summary>
        /// <param name="scanner">the scanner form</param>
        private void dockToScanner(Form scanner)
        {
            if (!Windows.GetVisible(this))
            {
                return;
            }

            if (scanner is IScannerPanel)
            {
                if (((IPanel)scanner).PanelCommon.DisplayMode != DisplayModeTypes.Popup)
                {
                    Windows.DockWithScanner(this, scanner, Context.AppWindowPosition);
                    Windows.SetTopMost(scanner);
                }
            }

            if (Left < 0)
            {
                Left = 0;
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
            return windowList.Where(window => window.Title.StartsWith(filter.Trim(),
                                    StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        /// <summary>
        /// Returns string that graphically fits into the specified width.  If it
        /// doesn't, curtails the string and adds ellipses
        /// </summary>
        /// <param name="graphics">Graphics object used to mesaure width of string</param>
        /// <param name="font">font to use</param>
        /// <param name="width">width to fit in</param>
        /// <param name="inputString">input string</param>
        /// <returns>output string that fits</returns>
        private String getMeasuredString(Graphics graphics, Font font, int width, String inputString)
        {
            int chop = 5;

            var str = inputString;

            try
            {
                while (true)
                {
                    SizeF sf = graphics.MeasureString(str, font);

                    if (sf.Width > width * ScannerCommon.PositionSizeController.ScaleFactor)
                    {
                        str = inputString.Substring(0, inputString.Length - chop) + "...";
                        chop += 5;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch
            {
                str = inputString;
            }

            return str;
        }

        /// <summary>
        /// Stores widget objects from the form
        /// </summary>
        private void getWidgets()
        {
            _sortOrderWidget = PanelCommon.RootWidget.Finder.FindChild("SortOrderIcon");
            _pageNumberWidget = PanelCommon.RootWidget.Finder.FindChild("PageNumber");
            _sortButton = PanelCommon.RootWidget.Finder.FindChild("ButtonSort");
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
                    sortedList.Insert(0, new EnumWindows.WindowInfo(desktopHWnd, R.GetString("ShowDesktop")));
                }
            }

            return sortedList;
        }

        /// <summary>
        /// Show the next pageful of entries in the window list
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _windowsList.Count)
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshWindowList();
                }
            }
        }

        /// <summary>
        /// Show the previous pageful of entries in the window list
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
        /// Perform the operation - page through the list,
        /// activate a window etc
        /// </summary>
        /// <param name="itemTag">Meta data about seleted item</param>
        private void handleWidgetSelection(Widget widget, ref bool handled)
        {
            if (widget.UserData is EnumWindows.WindowInfo)
            {
                handleWindowSelect((EnumWindows.WindowInfo)widget.UserData);
                handled = true;
            }
            else
            {
                switch (widget.Value)
                {
                    case "@Quit":
                        if (EvtDone != null)
                        {
                            EvtDone.BeginInvoke(null, null);
                        }
                        break;

                    case "@WindowListSort":
                        switchSortOrder();
                        break;

                    case "@CmdNextPage":
                        gotoNextPage();
                        break;

                    case "@CmdPrevPage":
                        gotoPreviousPage();
                        break;

                    case "@WindowListSearch":
                        if (EvtShowScanner != null)
                        {
                            EvtShowScanner.BeginInvoke(null, null, null, null);
                        }
                        break;

                    case "@WindowListClearFilter":
                        ClearFilter();
                        break;

                    default:
                        handled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// User selected a window to switch to.  Trigger an event
        /// to indicate this.
        /// </summary>
        /// <param name="wInfo">Window info of the window selected</param>
        private void handleWindowSelect(EnumWindows.WindowInfo wInfo)
        {
            if (!User32Interop.IsWindow(wInfo.Handle) || !User32Interop.IsWindowVisible(wInfo.Handle))
            {
                DialogUtils.ShowTimedDialog(this, R.GetString("WindowNotFound"));
            }
            else if (DialogUtils.ConfirmScanner(String.Format(R.GetString("ConfirmSwitchToWindow"), wInfo.Title)))
            {
                if (EvtActivateWindow != null)
                {
                    EvtActivateWindow.BeginInvoke(this, wInfo, null, null);
                }
            }
        }

        /// <summary>
        /// Turn highlight off for all widgets
        /// </summary>
        private void highlightOff()
        {
            PanelCommon.RootWidget.HighlightOff();
        }

        /// <summary>
        /// Get a list of active windows and display it in the list
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
        /// Refresh the list of windows in the UI
        ///
        /// </summary>
        private void refreshWindowList()
        {
            var list = new List<Widget>();
            PanelCommon.RootWidget.Finder.FindChild(typeof(TabStopScannerButton), list);

            int count = list.Count;
            if (count == 0)
            {
                return;
            }

            foreach (var button in list)
            {
                button.UserData = null;
                button.SetText(String.Empty);
            }

            // calculate how many pages, number of entries per page
            _entriesPerPage = count;
            _numPages = _windowsList.Count / _entriesPerPage;

            if ((_windowsList.Count() % _entriesPerPage) != 0)
            {
                _numPages++;
            }

            updateButtonBar();

            updateStatusBar();

            if (!_windowsList.Any())
            {
                (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 100 });
                list[0].SetText(R.GetString("NoActiveWindows"));
                return;
            }

            int ii = 0;
            var image = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(image);

            // fill titles of windows in the list
            for (int jj = _pageStartIndex; jj < _windowsList.Count && ii < count; ii++, jj++)
            {
                var tabStopScannerButton = list[ii] as TabStopScannerButton;

                tabStopScannerButton.SetTabStops(0.0f, new float[] { 0 });

                list[ii].UserData = _windowsList[jj];

                var str = getMeasuredString(graphics,
                                            tabStopScannerButton.UIControl.Font,
                                            tabStopScannerButton.Width,
                                            _windowsList[jj].Title);

                list[ii].SetText(str);
            }

            image.Dispose();
            graphics.Dispose();
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
        /// <param name="createParams">window create params</param>
        /// <returns>modified params</returns>
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
            PanelCommon.RootWidget.Finder.FindChild(typeof(TabStopScannerButton), list);

            _tabStopButtonCount = list.Count;

            getWidgets();

            SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;
            Shown += SwitchWindowsScanner_Shown;

            loadWindowList();

            PanelCommon.RootWidget.HighlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
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
        /// Update status bar with page number and sort order info
        /// </summary>
        private void updateButtonBar()
        {
            String text;
            var sortButtonText = "A-Z";
            if (!_windowsList.Any())
            {
                text = String.Empty;
            }
            else if (_sortOrder == SortOrder.Ascending)
            {
                text = "\u003A";
                sortButtonText = "A-Z";
            }
            else
            {
                text = "\u003B";
                sortButtonText = "Z-A";
            }

            if (_sortOrderWidget != null)
            {
                _sortOrderWidget.SetText(text);
            }

            if (_sortButton != null)
            {
                _sortButton.SetText(sortButtonText);
            }

            if (_pageNumberWidget != null)
            {
                var str = String.Format(R.GetString("PageNofM"), (_pageNumber + 1), _numPages);
                text = _windowsList.Any() ? str : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// Updates the status bar with sort order info
        /// </summary>
        private void updateStatusBar()
        {
            var text = String.Empty;

            if (!_windowsList.Any())
            {
                toolStripStatusLabel.Text = String.Empty;
                return;
            }

            switch (_sortOrder)
            {
                case SortOrder.Ascending:
                    text = R.GetString("SortOrderAlphabetical");
                    break;

                case SortOrder.Descending:
                    text = R.GetString("SortOrderReverseAlphabetical");
                    break;
            }

            toolStripStatusLabel.Text = text;
        }

        /// <summary>
        /// Position of the scanner changed.  If there is a companion
        /// scanner, dock to it
        /// </summary>
        /// <param name="form">the form</param>
        /// <param name="position">its position</param>
        private void Windows_EvtWindowPositionChanged(Form form, Windows.WindowPosition position)
        {
            if (form != this)
            {
                dockToScanner(form);
            }
        }
    }
}