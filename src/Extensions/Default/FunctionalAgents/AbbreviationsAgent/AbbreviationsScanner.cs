////////////////////////////////////////////////////////////////////////////
// <copyright file="AbbreviationsScanner.cs" company="Intel Corporation">
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
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using ACAT.Lib.Core.AbbreviationsManagement;
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

namespace ACAT.Extensions.Default.FunctionalAgents.AbbreviationsAgent
{
    /// <summary>
    /// This class represents the scanner that displays a list
    /// of abbreviations that the user can edit, add or delete.
    /// User can sort the abbreviations, page up and page down
    /// and browse all the abbreviations, filter based on a
    /// search string.
    /// </summary>
    [DescriptorAttribute("D174DAD7-A9FD-49CB-9DE1-7EB856109A79",
                            "AbbreviationsScanner",
                            "Abbreviations Scanner")]
    public partial class AbbreviationsScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// How many chars in the abbr to display. ellipses
        /// are displayed if longer
        /// </summary>
        private const int MaxAbbrCharacters = 5;

        /// <summary>
        /// Run command dispatcher object
        /// </summary>
        private readonly RunCommandDispatcher _dispatcher;

        /// <summary>
        /// Used to invoke methods and properties in the class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The keyboard actuator object
        /// </summary>
        private readonly KeyboardActuator _keyboardActuator;

        /// <summary>
        /// Status bar for the scanner form
        /// </summary>
        private readonly StatusBar _statusBar = new StatusBar();

        /// <summary>
        /// Displays the state of the Alt key
        /// </summary>
        private readonly StatusBarPanel _statusBarPanelSort = new StatusBarPanel();

        /// <summary>
        /// List of abbreviations
        /// </summary>
        private List<Abbreviation> _abbreviationsList;

        /// <summary>
        /// list of all abbreviations as a IEnumerable
        /// </summary>
        private IEnumerable<Abbreviation> _allAbbreviationsList;

        /// <summary>
        /// Scanner Form to which this form is docked
        /// </summary>
        private Form _dockedWithForm;

        /// <summary>
        /// How many entries can be displayed at a time
        /// </summary>
        private int _entriesPerPage;

        /// <summary>
        /// Total number of pages
        /// </summary>
        private int _numPages;

        /// <summary>
        /// Current page number
        /// </summary>
        private int _pageNumber;

        /// <summary>
        /// Widget that represents the current page number
        /// </summary>
        private Widget _pageNumberWidget;

        /// <summary>
        /// Where in the list does the currently displayed entries start
        /// </summary>
        private int _pageStartIndex;

        /// <summary>
        /// the scanner common object
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// Widget that the user clicks to resort
        /// </summary>
        private Widget _sortButton;

        /// <summary>
        /// sort order of abbr display
        /// </summary>
        private SortOrder _sortOrder = SortOrder.AtoZ;

        /// <summary>
        /// Widget that represents the sort order
        /// </summary>
        private Widget _sortOrderWidget;

        /// <summary>
        /// ensures that the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AbbreviationsScanner()
        {
            InitializeComponent();

            PanelClass = "AbbreviationsScanner";
            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;
            _invoker = new ExtensionInvoker(this);

            FormClosing += AbbreviationsScanner_FormClosing;
            KeyDown += AbbreviationsScanner_KeyDown;
            LocationChanged += AbbreviationsScanner_LocationChanged;

            KeyPreview = true;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new RunCommandDispatcher(this);

            createStatusBar();
        }

        /// <summary>
        /// For the event to indicate the user wants to
        /// add an abbr
        /// </summary>
        /// <param name="abbr">abbr to add</param>
        public delegate void AddAbbreviationDelegate(String abbr);

        /// <summary>
        /// For the event to indicate we are done
        /// </summary>
        /// <param name="flag">Confirm?</param>
        public delegate void DoneDelegate(bool flag);

        /// <summary>
        /// For the event to indicate the user wants to
        /// edit an abbr
        /// </summary>
        /// <param name="abbr">abbr to add</param>
        public delegate void EditAbbreviationDelegate(Abbreviation abbr);

        /// <summary>
        /// Raised when the user wants to add an abbr
        /// </summary>
        public event AddAbbreviationDelegate EvtAddAbbreviation;

        /// <summary>
        /// Raised to indicate the user wants to quit
        /// </summary>
        public event DoneDelegate EvtDone;

        /// <summary>
        /// Raised if the user wants to edit an abbr
        /// </summary>
        public event EditAbbreviationDelegate EvtEditAbbreviation;

        /// <summary>
        /// Event raised to display the alphabet scanner
        /// </summary>
        public event EventHandler EvtShowScanner;

        /// <summary>
        /// How to sort the list?
        /// </summary>
        private enum SortOrder
        {
            /// <summary>
            /// Sort alphabetically
            /// </summary>
            AtoZ,

            /// <summary>
            /// Sort reverse alphabetically
            /// </summary>
            ZtoA
        }

        /// <summary>
        /// Gets the dispatcher that handles commands
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
        /// Gets the form
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the Panel Class of the abbreviations scanner
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets the scannercommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller inferface
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Set the form style of the abbr scanner
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
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            arg.Handled = true;

            switch (arg.Widget.SubClass)
            {
                case "PreviousPage":
                    arg.Enabled = (_pageNumber != 0);
                    break;

                case "NextPage":
                    arg.Enabled = (_numPages != 0 && (_pageNumber + 1) != _numPages);
                    break;

                case "Back":
                case "DeletePreviousWord":
                case "ClearFilter":
                    arg.Handled = true;
                    arg.Enabled = !IsFilterEmpty();
                    break;

                case "Sort":
                case "Search":
                    arg.Handled = true;
                    arg.Enabled = (_abbreviationsList != null && _abbreviationsList.Any());
                    break;

                case "PrevChar":
                case "NextChar":
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
            Invoke(new MethodInvoker(delegate()
            {
                if (SearchFilter.Text.Length > 0 && AbbreviationsAgent.Confirm("Clear filter?"))
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
            _statusBarPanelSort.BorderStyle = StatusBarPanelBorderStyle.None;
            _statusBarPanelSort.AutoSize = StatusBarPanelAutoSize.Contents;
            _statusBar.Panels.Add(_statusBarPanelSort);

            _statusBar.SizingGrip = false;
            _statusBar.ShowPanels = true;
            _statusBar.Height = 30;
            _statusBar.Margin = new Padding(4, 4, 4, 4);
            _statusBar.Font = new Font("Arial", 16.0f);
            Controls.Add(_statusBar);
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
        /// Initializes the form.  Sets the position, initializes
        /// the scannercommon object
        /// </summary>
        /// <param name="startupArg">args</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _scannerCommon = new ScannerCommon(this) { PositionSizeController = { AutoPosition = true } };

            if (!_scannerCommon.Initialize(startupArg))
            {
                Log.Debug("Could not initialize form " + Name);
                return false;
            }

            PanelManager.Instance.EvtScannerShow += Instance_EvtScannerShow;
            PanelManager.Instance.EvtScannerClosed += Instance_EvtScannerClosed;

            return true;
        }

        /// <summary>
        /// Checks if the search filter is empty
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
        /// Get the list of abbreviations and populates
        /// the list
        /// </summary>
        public void LoadAbbreviations()
        {
            Windows.SetText(SearchFilter, String.Empty);

            _allAbbreviationsList = Context.AppAbbreviations.AbbrevationList;
            _abbreviationsList = _allAbbreviationsList.ToList();

            refreshAbbreviationsList();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="monitorInfo"></param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
        }

        /// <summary>
        /// Pauses animation
        /// </summary>
        public void OnPause()
        {
            _scannerCommon.GetAnimationManager().Pause();
        }

        /// <summary>
        /// Show we allow deactivation?
        /// </summary>
        /// <param name="eventArg">event arg</param>
        /// <returns>true if so</returns>
        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            _scannerCommon.GetAnimationManager().Resume();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="command"></param>
        /// <param name="handled"></param>
        public void OnRunCommand(string command, ref bool handled)
        {
        }

        /// <summary>
        /// Called when a widget on the scanner is activated
        /// </summary>
        /// <param name="widget">widget activated</param>
        /// <param name="handled">true if handled</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            actuateWidget(widget, ref handled);
        }

        /// <summary>
        /// Temporary disable keypress, pause animation
        /// </summary>
        public void Pause()
        {
            _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
            _scannerCommon.GetAnimationManager().Pause();
        }

        /// <summary>
        /// Resume key press handlers, resume animation
        /// </summary>
        public void Resume()
        {
            _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            _scannerCommon.GetAnimationManager().Resume();
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
        /// Invoked when the form is closing. Release
        /// resources
        /// </summary>
        /// <param name="e">event arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            removeWatchdogs();

            PanelManager.Instance.EvtScannerShow -= Instance_EvtScannerShow;
            PanelManager.Instance.EvtScannerClosed -= Instance_EvtScannerClosed;

            _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Windows procedure
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
        /// Key press handler
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
                    EvtDone.BeginInvoke(false, null, null);
                }
            }
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void AbbreviationsScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Key down event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void AbbreviationsScanner_KeyDown(object sender, KeyEventArgs e)
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
        /// Initilaze. Load the abbreviations, populate list and display
        /// the abbreviations.
        /// </summary>
        private void AbbreviationsScanner_Load(object sender, EventArgs e)
        {
            enableWatchdogs();

            _scannerCommon.OnLoad();

            var list = new List<Widget>();
            _scannerCommon.GetRootWidget().Finder.FindChild(typeof(TabStopScannerButton), list);

            _sortOrderWidget = _scannerCommon.GetRootWidget().Finder.FindChild("SortOrderIcon");
            _pageNumberWidget = _scannerCommon.GetRootWidget().Finder.FindChild("PageNumber");
            _sortButton = _scannerCommon.GetRootWidget().Finder.FindChild("ButtonSort");

            SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;
            Shown += AbbreviationsScanner_Shown;

            LoadAbbreviations();

            highlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }

            _scannerCommon.GetAnimationManager().Start(_scannerCommon.GetRootWidget());
        }

        /// <summary>
        /// Event handler for when location of the form changes.
        /// Disallow this and redock the form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void AbbreviationsScanner_LocationChanged(object sender, EventArgs e)
        {
            if (_dockedWithForm != null)
            {
                dockToScanner(_dockedWithForm);
            }
        }

        /// <summary>
        /// Invoked when the form is shown. Make it the
        /// active window with focus
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void AbbreviationsScanner_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(this.Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Handles actuation of a widget
        /// </summary>
        /// <param name="widget">widget actauted</param>
        /// <param name="handled">true if handled</param>
        private void actuateWidget(Widget widget, ref bool handled)
        {
            handleWidgetSelection(widget, ref handled);
            highlightOff();
        }

        /// <summary>
        /// Docks this form to the active scanner
        /// </summary>
        /// <param name="scanner"></param>
        private void dockToScanner(Form scanner)
        {
            if (scanner is IScannerPanel)
            {
                Windows.DockWithScanner(this, scanner, Context.AppWindowPosition);
            }
        }

        /// <summary>
        /// Enable watchdogs to see tha the form stays active
        /// </summary>
        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }
        }

        /// <summary>
        /// If there is a search filter, show only abbrs that match
        ///  the filter
        /// </summary>
        /// <param name="abbrList">abbr list</param>
        /// <param name="filter">search filter</param>
        /// <returns>list with matching abbrs</returns>
        private List<Abbreviation> filterAbbreviations(IEnumerable<Abbreviation> abbrList, String filter)
        {
            var trimFilter = filter.Trim();

            return abbrList.Where(abbr => includeAbbr(abbr, trimFilter)).ToList();
        }

        /// <summary>
        /// Show the next pageful of entries
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _allAbbreviationsList.Count())
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshAbbreviationsList();
                }
            }
        }

        /// <summary>
        /// Show the previous pageful of entries
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
                refreshAbbreviationsList();
            }
        }

        /// <summary>
        /// User wants to add a new abbreviation
        /// </summary>
        private void handleAddNewAbbreviation()
        {
            if (EvtAddAbbreviation != null)
            {
                if (DialogUtils.ConfirmScanner("Add new abbreviation?"))
                {
                    Invoke(new MethodInvoker(() => EvtAddAbbreviation(Windows.GetText(SearchFilter))));
                }
            }
        }

        /// <summary>
        /// User wants to edit or delete an abbr
        /// </summary>
        /// <param name="itemTag">Tag associated with the list item selected</param>
        private void handleEditAbbreviation(Abbreviation abbr)
        {
            if (EvtEditAbbreviation != null)
            {
                Invoke(new MethodInvoker(() => EvtEditAbbreviation(abbr)));
            }
        }

        /// <summary>
        /// Handles actuation of a widget
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">true if handled</param>
        private void handleWidgetSelection(Widget widget, ref bool handled)
        {
            // the user actuated an abbreviation in the list
            if (widget.UserData is Abbreviation)
            {
                handleEditAbbreviation((Abbreviation)widget.UserData);
                handled = true;
            }
            else
            {
                handled = true;
                switch (widget.Value)
                {
                    case "@Quit":
                        if (EvtDone != null)
                        {
                            EvtDone.BeginInvoke(false, null, null);
                        }
                        break;

                    case "@AbbrListSort":
                        switchSortOrder();
                        break;

                    case "@AbbrListNextPage":
                        gotoNextPage();
                        break;

                    case "@AbbrListPrevPage":
                        gotoPreviousPage();
                        break;

                    case "@AbbrListClearFilter":
                        ClearFilter();
                        break;

                    case "@AbbrListSearch":
                        if (EvtShowScanner != null)
                        {
                            EvtShowScanner.BeginInvoke(null, null, null, null);
                        }
                        break;

                    case "@AbbrListAdd":
                        handleAddNewAbbreviation();
                        break;

                    default:
                        handled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Turn highlight off on everything
        /// </summary>
        private void highlightOff()
        {
            _scannerCommon.GetRootWidget().HighlightOff();
        }

        /// <summary>
        /// Matchs the abbr with the filter and tells
        /// whether there is a match or not. Checks only the
        /// mnemonic
        /// </summary>
        /// <param name="abbr">abbr</param>
        /// <param name="filter">search filter</param>
        /// <returns>true on match</returns>
        private bool includeAbbr(Abbreviation abbr, String filter)
        {
            bool add = true;

            if (!String.IsNullOrEmpty(filter) &&
                !abbr.Mnemonic.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase))
            {
                add = false;
            }

            return add;
        }

        /// <summary>
        /// Event handler for when a scanner closes.  Reposition this scanner
        /// to its default position
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void Instance_EvtScannerClosed(object sender, ScannerCloseEventArg arg)
        {
            if (arg.Scanner != this && Windows.GetVisible(this))
            {
                if (_dockedWithForm == arg.Scanner)
                {
                    _dockedWithForm = null;
                }
                _scannerCommon.PositionSizeController.AutoSetPosition();
            }
        }

        /// <summary>
        /// Displayed when the alphaet scanner is displayed. Dock
        /// this form with the currently active scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event arg</param>
        private void Instance_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (arg.Scanner != this && Windows.GetVisible(this))
            {
                _dockedWithForm = arg.Scanner.Form;
                dockToScanner(arg.Scanner.Form);
            }
        }

        /// <summary>
        /// Refreshes the list of abbreviations and displays the
        /// current pageful of abbreviations in the UI.
        /// </summary>
        private void refreshAbbreviationsList()
        {
            var list = new List<Widget>();
            _scannerCommon.GetRootWidget().Finder.FindChild(typeof(TabStopScannerButton), list);

            int count = list.Count();
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
            _numPages = _abbreviationsList.Count() / _entriesPerPage;

            if ((_abbreviationsList.Count() % _entriesPerPage) != 0)
            {
                _numPages++;
            }

            updateButtonBar();

            updateStatusBar();

            if (!_abbreviationsList.Any())
            {
                (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 100, 120 });
                list[0].SetText("\t-------- ABBREVIATIONS LIST EMPTY --------");
                return;
            }

            _abbreviationsList = sort(_abbreviationsList, _sortOrder);

            int ii = 0;

            for (int jj = _pageStartIndex; jj < _abbreviationsList.Count && ii < count; ii++, jj++)
            {
                (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 100, 120 });

                list[ii].UserData = _abbreviationsList[jj];

                var word = _abbreviationsList[jj].Mnemonic;
                if (word.Length > MaxAbbrCharacters)
                {
                    word = word.Substring(0, MaxAbbrCharacters) + "...";
                }

                var replaceWith = System.Text.RegularExpressions.Regex.Replace(_abbreviationsList[jj].Expansion, "\n", " ");
                if (replaceWith.Length > 40)
                {
                    replaceWith = replaceWith.Substring(0, 40) + "...";
                }

                list[ii].SetText(word + "\t" + _abbreviationsList[jj].Mode + "\t" + replaceWith);
            }
        }

        /// <summary>
        /// Disable watchdogs
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
        /// User typed something in the search filter.  Do a
        /// search to find matches in the list and display only those
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void SearchFilter_TextChanged(object sender, EventArgs e)
        {
            _pageNumber = 0;
            _pageStartIndex = 0;
            _abbreviationsList = filterAbbreviations(_allAbbreviationsList, Windows.GetText(SearchFilter));
            refreshAbbreviationsList();
        }

        /// <summary>
        /// Set the style of the form
        /// </summary>
        /// <param name="createParams"></param>
        /// <returns></returns>
        private CreateParams setFormStyles(CreateParams createParams)
        {
            createParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_NOACTIVATE;
            return createParams;
        }

        /// <summary>
        /// Sorts the input list based on the sort order
        /// </summary>
        /// <param name="list">input list</param>
        /// <param name="order">sort order</param>
        /// <returns>sorted list</returns>
        private List<Abbreviation> sort(List<Abbreviation> list, SortOrder order)
        {
            switch (order)
            {
                case SortOrder.ZtoA:
                    return list.OrderByDescending(f => f.Mnemonic).ToList();

                case SortOrder.AtoZ:
                    return list.OrderBy(f => f.Mnemonic).ToList();

                default:
                    return list;
            }
        }

        /// <summary>
        /// User clicked on the sort order button. Switch the
        /// sort order
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void SortOrderIcon_Click(object sender, EventArgs e)
        {
            switchSortOrder();
        }

        /// <summary>
        /// Switch the sort order and refresh the abbreviations list
        /// </summary>
        private void switchSortOrder()
        {
            _sortOrder = _sortOrder == SortOrder.AtoZ ? SortOrder.ZtoA : SortOrder.AtoZ;

            _pageNumber = 0;
            _pageStartIndex = 0;
            _abbreviationsList = filterAbbreviations(_allAbbreviationsList, Windows.GetText(SearchFilter));
            refreshAbbreviationsList();
        }

        /// <summary>
        /// Updates the status bar with page number info,
        /// sort order etc.
        /// </summary>
        private void updateButtonBar()
        {
            String text;
            var sortButtonText = "A-Z";
            if (!_abbreviationsList.Any())
            {
                text = String.Empty;
            }
            else if (_sortOrder == SortOrder.AtoZ)
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
                text = _abbreviationsList.Any() ? "Page " + (_pageNumber + 1) + " of " + _numPages : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// Updates the status bar
        /// </summary>
        private void updateStatusBar()
        {
            var text = String.Empty;

            if (!_abbreviationsList.Any())
            {
                _statusBarPanelSort.Text = String.Empty;
                return;
            }

            switch (_sortOrder)
            {
                case SortOrder.AtoZ:
                    text = "Sort Order:  ALPHABETICAL";
                    break;

                case SortOrder.ZtoA:
                    text = "Sort Order:  REVERSE ALPHABETICAL";
                    break;
            }

            _statusBarPanelSort.Text = text;
        }
    }
}