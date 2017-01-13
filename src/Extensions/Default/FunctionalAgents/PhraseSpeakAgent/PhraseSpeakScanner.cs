////////////////////////////////////////////////////////////////////////////
// <copyright file="PhraseSpeakScanner.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AbbreviationsManagement;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.InputActuators;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.TTSManagement;
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

namespace ACAT.Extensions.Default.FunctionalAgents.PhraseSpeakAgent
{
    /// <summary>
    /// This class represents the scanner that displays a list
    /// of Phrases from which the user can select a Phrase to
    /// convert to speech.  The list of phrases comes from
    /// the Phrases file (Phrases.xml).
    /// </summary>
    [DescriptorAttribute("D80D5448-13D7-472A-AF73-4C326F1CACAE",
                        "PhraseSpeakScanner",
                        "Phrase Speak Scanner")]
    public partial class PhraseSpeakScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// Run commands
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
        /// the scanner common object
        /// </summary>
        private readonly ScannerCommon _scannerCommon;

        /// <summary>
        /// list of all phrases as a IEnumerable
        /// </summary>
        private IEnumerable<Phrase> _allPhrasesList;

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
        /// List of phrases
        /// </summary>
        private List<Phrase> _phrasesList;

        /// <summary>
        /// Is the search button visible?
        /// </summary>
        private bool _searchButtonVisible;

        /// <summary>
        /// The widget representing the search button
        /// </summary>
        private Widget _searchButtonWidget;

        /// <summary>
        /// Widget that the user clicks to resort
        /// </summary>
        private Widget _sortButton;

        /// <summary>
        /// sort order of phrase display
        /// </summary>
        private SortOrder _sortOrder = SortOrder.None;

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
        public PhraseSpeakScanner()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            _searchButtonVisible = true;

            PanelClass = "PhraseSpeakScanner";
            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;
            _invoker = new ExtensionInvoker(this);

            subscribeToEvents();

            KeyPreview = true;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new Dispatcher(this);

            statusStrip1.SizingGrip = false;
        }

        /// <summary>
        /// For the event to indicate we are done
        /// </summary>
        /// <param name="flag">Confirm?</param>
        public delegate void DoneDelegate(bool flag);

        /// <summary>
        /// Raised to indicate the user wants to quit
        /// </summary>
        public event DoneDelegate EvtDone;

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
            /// Not sorted.  Natural order
            /// </summary>
            None,

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
        /// Gets the Panel Class of the phrases scanner
        /// </summary>
        public String PanelClass { get; private set; }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _scannerCommon; } }

        /// <summary>
        /// Gets the scannercommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets or sets whether the search button should be shown
        /// </summary>
        public bool ShowSearchButton
        {
            get
            {
                return _searchButtonVisible;
            }
            set
            {
                _searchButtonVisible = value;

                if (_searchButtonWidget != null)
                {
                    setSearchButtonVisibility(_searchButtonVisible);
                }
            }
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
        /// Set the form style of the phrase scanner
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
        /// Invoked to check if a widget on a scanner needs to
        /// be enabled or not.  This depends on the context.
        /// We are going to handle this in the EvtCommandEnabled handler
        /// </summary>
        /// <param name="arg">Contextual information</param>
        /// <returns>true on success</returns>
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            OnCommandEnabled(arg);
            return true;
        }

        /// <summary>
        /// Clears the search filter
        /// </summary>
        public void ClearFilter()
        {
            Invoke(new MethodInvoker(delegate
            {
                if (SearchFilter.Text.Length > 0 && PhraseSpeakAgent.Confirm(R.GetString("ClearFilter")))
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
        /// Initializes the form.  Sets the position, initializes
        /// the scannercommon object
        /// </summary>
        /// <param name="startupArg">args</param>
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

            _searchButtonWidget = PanelCommon.RootWidget.Finder.FindChild(SearchFilterIcon.Handle);

            return true;
        }

        /// <summary>
        /// Checks if the search filter is empty
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
        /// Get the list of and populates the list
        /// </summary>
        public void LoadPhrases()
        {
            Windows.SetText(SearchFilter, String.Empty);

            var phrases = Phrases.Load();

            _allPhrasesList = phrases.PhraseList;

            _phrasesList = _allPhrasesList.ToList();

            refreshPhrasesList();
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
            PanelCommon.AnimationManager.Pause();
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
            _scannerCommon.PositionSizeController.AutoSetPosition();

            PanelCommon.AnimationManager.Resume();
        }

        /// <summary>
        /// Not used
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
            PanelCommon.AnimationManager.Pause();
        }

        /// <summary>
        /// Resume key press handlers, resume animation
        /// </summary>
        public void Resume()
        {
            _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            PanelCommon.AnimationManager.Resume();
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
        /// Invoked when the form is closing. Release
        /// resources
        /// </summary>
        /// <param name="e">event arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            removeWatchdogs();

            PhraseSpeakAgent.Instance.EvtCommandEnabled -= OnCommandEnabled;
            Windows.EvtWindowPositionChanged -= Windows_EvtWindowPositionChanged;

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
                    close();
                }
            }
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
        /// Confirm and close the scanner
        /// </summary>
        private void close()
        {
            if (EvtDone != null)
            {
                EvtDone.BeginInvoke(false, null, null);
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
        /// Docks this form to the active scanner
        /// </summary>
        /// <param name="scanner"></param>
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
        /// If there is a search filter, show only phrases that match
        /// the filter
        /// </summary>
        /// <param name="phraseList">phrase list</param>
        /// <param name="filter">search filter</param>
        /// <returns>list with matching abbrs</returns>
        private List<Phrase> filterPhrases(IEnumerable<Phrase> phraseList, String filter)
        {
            var trimFilter = filter.Trim();

            return phraseList.Where(phrase => includePhrase(phrase, trimFilter)).ToList();
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
        /// Show the next pageful of entries
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _allPhrasesList.Count())
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshPhrasesList();
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

                refreshPhrasesList();
            }
        }

        /// <summary>
        /// Handles actuation of a widget
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">true if handled</param>
        private void handleWidgetSelection(Widget widget, ref bool handled)
        {
            // the user actuated an phrase in the list
            if (widget.UserData is Phrase)
            {
                Phrase phrase = (Phrase)widget.UserData;

                TTSManager.Instance.ActiveEngine.Stop();

                int bookmark;
                TTSManager.Instance.ActiveEngine.SpeakAsync(phrase.Text, out bookmark);
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

                    case "@PhraseListSort":
                        switchSortOrder();
                        break;

                    case "@CmdNextPage":
                        gotoNextPage();
                        break;

                    case "@CmdPrevPage":
                        gotoPreviousPage();
                        break;

                    case "@PhraseListClearFilter":
                        ClearFilter();
                        break;

                    case "@PhraseListSearch":
                        if (EvtShowScanner != null)
                        {
                            EvtShowScanner.BeginInvoke(null, null, null, null);
                        }
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
            PanelCommon.RootWidget.HighlightOff();
        }

        /// <summary>
        /// Matchs the phrase expansion with the filter and tells
        /// whether there is a match or not.
        /// </summary>
        /// <param name="phrase">phrase</param>
        /// <param name="filter">search filter</param>
        /// <returns>true on match</returns>
        private bool includePhrase(Phrase phrase, String filter)
        {
            bool add = true;

            if (!String.IsNullOrEmpty(filter) && !phrase.Text.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase))
            {
                add = false;
            }

            return add;
        }

        /// <summary>
        /// Event handler to check if widget is to be enabled
        /// </summary>
        /// <param name="arg">event args</param>
        private void OnCommandEnabled(CommandEnabledArg arg)
        {
            if (!Windows.GetVisible(this))
            {
                arg.Handled = false;
                return;
            }

            arg.Handled = true;

            switch (arg.Command)
            {
                case "CmdPunctuationScanner":
                case "CmdNumberScanner":
                case "CmdCursorScanner":
                    arg.Enabled = true;
                    break;

                case "CmdPrevPage":
                    arg.Enabled = (_pageNumber != 0);
                    break;

                case "CmdNextPage":
                    arg.Enabled = (_numPages != 0 && (_pageNumber + 1) != _numPages);
                    break;

                case "Back":
                case "CmdDeletePrevWord":
                case "CmdPrevChar":
                case "CmdNextChar":
                case "PhraseListClearFilter":
                    arg.Handled = true;
                    arg.Enabled = !IsFilterEmpty();
                    break;

                case "PhraseListSort":
                    arg.Handled = true;
                    arg.Enabled = (_phrasesList != null && _phrasesList.Any());
                    break;

                case "PhraseListSearch":
                    arg.Handled = true;
                    arg.Enabled = (_searchButtonVisible && _phrasesList != null && _phrasesList.Any());
                    break;

                default:
                    arg.Handled = false;
                    break;
            }
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void PhraseSpeakScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Key down event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void PhraseSpeakScanner_KeyDown(object sender, KeyEventArgs e)
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
        /// Initilaze. Load the phrases, populate list and display
        /// them.
        /// </summary>
        private void PhraseSpeakScanner_Load(object sender, EventArgs e)
        {
            enableWatchdogs();

            _scannerCommon.OnLoad();

            var list = new List<Widget>();
            PanelCommon.RootWidget.Finder.FindChild(typeof(TabStopScannerButton), list);

            _sortOrderWidget = PanelCommon.RootWidget.Finder.FindChild("SortOrderIcon");
            _pageNumberWidget = PanelCommon.RootWidget.Finder.FindChild("PageNumber");
            _sortButton = PanelCommon.RootWidget.Finder.FindChild("ButtonSort");

            SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;
            Shown += PhraseSpeakScanner_Shown;

            PhraseSpeakAgent.Instance.EvtCommandEnabled += OnCommandEnabled;

            LoadPhrases();

            updateButtonBar();

            updateStatusBar();

            highlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Invoked when the form is shown. Make it the
        /// active window with focus
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event arg</param>
        private void PhraseSpeakScanner_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Refreshes the list of phrases and displays the
        /// current pageful of phrases in the UI.
        /// </summary>
        private void refreshPhrasesList()
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
            _numPages = _phrasesList.Count / _entriesPerPage;

            if ((_phrasesList.Count() % _entriesPerPage) != 0)
            {
                _numPages++;
            }

            updateButtonBar();

            updateStatusBar();

            if (!_phrasesList.Any())
            {
                (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 100, 120 });
                list[0].SetText("\t" + R.GetString("PhrasesListEmpty"));
                return;
            }

            _phrasesList = sort(_phrasesList, _sortOrder);

            int ii = 0;

            var image = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(image);

            for (int jj = _pageStartIndex; jj < _phrasesList.Count && ii < count; ii++, jj++)
            {
                (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 200 });

                list[ii].UserData = _phrasesList[jj];

                var replaceWith = System.Text.RegularExpressions.Regex.Replace(_phrasesList[jj].Text, "\n", " ");

                var str = getMeasuredString(graphics, (list[ii] as TabStopScannerButton).UIControl.Font, (list[ii] as TabStopScannerButton).Width, replaceWith);

                list[ii].SetText(str);
            }

            image.Dispose();
            graphics.Dispose();
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
            _phrasesList = filterPhrases(_allPhrasesList, Windows.GetText(SearchFilter));
            refreshPhrasesList();
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
        /// Sets the visibility of the search button
        /// </summary>
        /// <param name="visible">true for visible</param>
        private void setSearchButtonVisibility(bool visible)
        {
            if (_searchButtonWidget != null)
            {
                if (visible)
                {
                    _searchButtonWidget.Show();
                }
                else
                {
                    _searchButtonWidget.Hide();
                }
            }
        }

        /// <summary>
        /// Sorts the input list based on the sort order
        /// </summary>
        /// <param name="list">input list</param>
        /// <param name="order">sort order</param>
        /// <returns>sorted list</returns>
        private List<Phrase> sort(List<Phrase> list, SortOrder order)
        {
            switch (order)
            {
                case SortOrder.None:
                    return list;

                case SortOrder.ZtoA:
                    return list.OrderByDescending(f => f.Text).ToList();

                case SortOrder.AtoZ:
                    return list.OrderBy(f => f.Text).ToList();

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
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            FormClosing += PhraseSpeakScanner_FormClosing;
            KeyDown += PhraseSpeakScanner_KeyDown;
        }

        /// <summary>
        /// Switch the sort order and refresh the phrases list
        /// </summary>
        private void switchSortOrder()
        {
            switch (_sortOrder)
            {
                case SortOrder.None:
                    _sortOrder = SortOrder.AtoZ;
                    break;

                case SortOrder.AtoZ:
                    _sortOrder = SortOrder.ZtoA;
                    break;

                case SortOrder.ZtoA:
                    _sortOrder = SortOrder.None;
                    break;
            }

            _pageNumber = 0;
            _pageStartIndex = 0;
            _phrasesList = filterPhrases(_allPhrasesList, Windows.GetText(SearchFilter));
            refreshPhrasesList();
        }

        /// <summary>
        /// Updates the button bar with page number info,
        /// sort order etc.
        /// </summary>
        private void updateButtonBar()
        {
            String text;
            String sortButtonText;

            if (!_phrasesList.Any())
            {
                text = String.Empty;
                sortButtonText = String.Empty;
            }
            else if (_sortOrder == SortOrder.None)
            {
                text = String.Empty;
                sortButtonText = R.GetString("Sort");
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
                var str = String.Format(R.GetString("PageNofM"), (_pageNumber + 1), _numPages);
                text = _phrasesList.Any() ? str : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// Updates the status bar
        /// </summary>
        private void updateStatusBar()
        {
            var text = String.Empty;

            if (!_phrasesList.Any())
            {
                toolStripStatusLabel.Text = String.Empty;
                return;
            }

            switch (_sortOrder)
            {
                case SortOrder.AtoZ:
                    text = R.GetString("SortOrderAlphabetical");
                    break;

                case SortOrder.ZtoA:
                    text = R.GetString("SortOrderReverseAlphabetical");
                    break;

                case SortOrder.None:
                    text = String.Empty;
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

                var form = Dispatcher.Scanner.Form as PhraseSpeakScanner;

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