////////////////////////////////////////////////////////////////////////////
// <copyright file="LaunchAppScanner.cs" company="Intel Corporation">
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
using ACAT.Lib.Extension.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.LaunchAppAgent
{
    /// <summary>
    /// Displays a list of apps. The list of apps is read from an
    /// external XML file which includes the path to the executable,
    /// optional command line arguments and a friendly name.  The
    /// friendly name is displayed in the list.  User selets an app and the
    /// agent launches the app
    /// </summary>
    [DescriptorAttribute("D946E1D6-5D67-40E6-B27F-F8D062DE5450",
                        "LaunchAppScanner",
                        "Launch Applications Scanner")]
    public partial class LaunchAppScanner : Form, IScannerPanel, IExtension
    {
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
        /// Displays status
        /// </summary>
        //private readonly ToolStripStatusLabel _toolStripStatusLabel = new ToolStripStatusLabel();

        /// <summary>
        /// The scannercommon object
        /// </summary>
        private readonly ScannerCommon _scannerCommon;

        /// <summary>
        /// List of all apps
        /// </summary>
        private List<AppInfo> _allAppsList;

        /// <summary>
        /// List of apps that match the filter specified by the user
        /// </summary>
        private List<AppInfo> _appsList;

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
        /// Widget that the user clicks to resort
        /// </summary>
        private Widget _sortButton;

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
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            PanelClass = "LaunchAppScanner";
            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;
            _invoker = new ExtensionInvoker(this);
            KeyPreview = true;

            subscribeToEvents();

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new Dispatcher(this);

            createStatusBar();
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
        /// Event raised to display the alphabet scanner
        /// </summary>
        public event EventHandler EvtShowScanner;

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
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _scannerCommon; } }

        /// Gets scanner common object
        /// <summary>
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
        /// Sets the states of the buttons in the button bar
        /// and in the companion scanner (if any) depending
        /// on the current state.
        /// </summary>
        /// <param name="arg">widget information</param>
        /// <returns>true on success</returns>
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
                case "AppListClearFilter":
                    arg.Handled = true;
                    arg.Enabled = !IsFilterEmpty();
                    break;

                case "AppListSort":
                case "AppListSearch":
                    arg.Handled = true;
                    arg.Enabled = (_appsList != null && _appsList.Any());
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
        /// Clears the search filter
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
        /// Creates a status bar for the scanner
        /// </summary>
        public void createStatusBar()
        {
            statusStrip.SizingGrip = false;
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
        /// Returns if the filter string is empty
        /// </summary>
        /// <returns>true if so</returns>
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
            _scannerCommon.PositionSizeController.AutoSetPosition();

            PanelCommon.AnimationManager.Resume();
        }

        /// <summary>
        /// not used
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handled"></param>
        public void OnRunCommand(string command, ref bool handled)
        {
            switch (command)
            {
                case "CmdGoBack":
                    close();
                    break;
            }
        }

        /// <summary>
        /// Called when widget is actuated
        /// </summary>
        /// <param name="widget">which one</param>
        /// <param name="handled">are we handling it?</param>
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
        /// Invoked when the form is closing
        /// </summary>
        /// <param name="e">event args</param>
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
        /// <param name="m"></param>
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
        /// Key press event handler.  Process the ESC
        /// key and quit if it is pressed
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
        /// Find the widget that was actuated and act on it
        /// </summary>
        /// <param name="widgetName">name of the widget</param>
        private void actuateWidget(Widget widget, ref bool handled)
        {
            handleWidgetSelection(widget, ref handled);
            highlightOff();
        }

        /// <summary>
        /// Confirm and close the scanner
        /// </summary>
        private void close()
        {
            if (EvtQuit != null)
            {
                EvtQuit.BeginInvoke(this, null, null, null);
            }
            else
            {
                if (DialogUtils.ConfirmScanner(PanelManager.Instance.GetCurrentForm(), R.GetString("CloseQ")))
                {
                    Windows.CloseForm(this);
                }
            }
        }

        /// <summary>
        /// Docks to the companian scanner
        /// </summary>
        /// <param name="scanner">companian scanner</param>
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
            return list.Where(f => f.Name.StartsWith(filter.Trim(), StringComparison.InvariantCultureIgnoreCase)).ToList();
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
        /// Display next pageful of entries in the list
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _appsList.Count)
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshAppList();
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
                refreshAppList();
            }
        }

        /// <summary>
        /// Confirms if the user wants to launch the selected
        /// app. If so, triggers an event to indicate that the user
        /// wants to launch the app.
        /// </summary>
        /// <param name="appInfo"></param>
        private void handleAppSelect(AppInfo appInfo)
        {
            if (DialogUtils.ConfirmScanner(String.Format(R.GetString("LaunchAppQ"), appInfo.Name)))
            {
                if (EvtLaunchApp != null)
                {
                    EvtLaunchApp.BeginInvoke(this, appInfo, null, null);
                }
            }
        }

        /// <summary>
        /// Handle the selection - navigate, launch app etc
        /// </summary>
        /// <param name="widget">Widget selected</param>
        /// <param name="handled">was this handled</param>
        private void handleWidgetSelection(Widget widget, ref bool handled)
        {
            if (widget.UserData is AppInfo)
            {
                handleAppSelect((AppInfo)widget.UserData);
                handled = true;
            }
            else
            {
                handled = true;
                switch (widget.Value)
                {
                    case "@CmdGoBack":
                        close();
                        break;

                    case "@AppListSort":
                        switchSortOrder();
                        break;

                    case "@CmdNextPage":
                        gotoNextPage();
                        break;

                    case "@CmdPrevPage":
                        gotoPreviousPage();
                        break;

                    case "@AppListSearch":
                        if (EvtShowScanner != null)
                        {
                            EvtShowScanner.BeginInvoke(null, null, null, null);
                        }
                        break;

                    case "@AppListClearFilter":
                        ClearFilter();
                        break;

                    default:
                        handled = false;
                        break;
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
        /// The form has loaded.  Initialize
        /// </summary>
        private void LauncAppScanner_Load(object sender, EventArgs e)
        {
            enableWatchdogs();

            _scannerCommon.OnLoad();

            var list = new List<Widget>();
            PanelCommon.RootWidget.Finder.FindChild(typeof(TabStopScannerButton), list);

            _tabStopButtonCount = list.Count;

            getWidgets();

            SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;

            loadAppsList();

            PanelCommon.RootWidget.HighlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
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
        /// Set focus to this form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void LaunchAppScanner_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Loads a list of apps to launch and refreshes the UI
        /// </summary>
        private void loadAppsList()
        {
            _allAppsList = getAppList(_sortOrder);
            _appsList = filterApps(_allAppsList, Windows.GetText(SearchFilter));

            if (_tabStopButtonCount >= 3)
            {
                _entriesPerPage = _tabStopButtonCount - 2;
                refreshAppList();
            }
        }

        /// <summary>
        /// Refreshes the window list with a list of apps
        /// </summary>
        private void refreshAppList()
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

            _entriesPerPage = count;
            _numPages = _appsList.Count() / _entriesPerPage;

            if ((_appsList.Count() % _entriesPerPage) != 0)
            {
                _numPages++;
            }

            updateButtonBar();

            updateStatusBar();

            if (!_appsList.Any())
            {
                (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 100 });
                list[0].SetText("\t" + R.GetString("AppsListIsEmpty"));
                return;
            }

            int ii = 0;
            var image = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(image);

            for (int jj = _pageStartIndex; jj < _appsList.Count && ii < count; ii++, jj++)
            {
                var tabStopScannerButton = list[ii] as TabStopScannerButton;

                tabStopScannerButton.SetTabStops(0.0f, new float[] { 0 });

                list[ii].UserData = _appsList[jj];

                var title = _appsList[jj].Name;

                var str = getMeasuredString(graphics, tabStopScannerButton.UIControl.Font, tabStopScannerButton.Width, title);

                list[ii].SetText(str);
            }

            image.Dispose();
            graphics.Dispose();
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
            refreshAppList();
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
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            FormClosing += LaunchAppScanner_FormClosing;
            Shown += LaunchAppScanner_Shown;
            KeyDown += LaunchAppScanner_KeyDown;
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
            refreshAppList();
        }

        /// <summary>
        /// Updates the buttons on the button bar depending on the
        /// current state
        /// </summary>
        private void updateButtonBar()
        {
            String text;
            var sortButtonText = "A-Z";
            if (!_appsList.Any())
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
                text = _appsList.Any() ? str : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// Updates the status bar with sort order info
        /// </summary>
        private void updateStatusBar()
        {
            var text = String.Empty;

            if (!_appsList.Any())
            {
                toolStripStatusLabel1.Text = String.Empty;
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

            toolStripStatusLabel1.Text = text;
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

        /// <summary>
        /// Handler for dispatching commands
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">the command to execute</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">true if it was handled</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as LaunchAppScanner;

                switch (Command)
                {
                    case "CmdGoBack":
                        form.close();
                        break;
                }

                return true;
            }
        }

        /// <summary>
        /// Command dispatcher
        /// </summary>
        private class Dispatcher : DefaultCommandDispatcher
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="panel">the scanner object</param>
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
                Commands.Add(new CommandHandler("CmdGoBack"));
            }
        }
    }
}