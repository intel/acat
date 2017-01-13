////////////////////////////////////////////////////////////////////////////
// <copyright file="FileBrowserScanner.cs" company="Intel Corporation">
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
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using Font = System.Drawing.Font;
using Windows = ACAT.Lib.Core.Utility.Windows;

namespace ACAT.Extensions.Default.FunctionalAgents.FileBrowserAgent
{
    /// <summary>
    /// Presents a list of files as a list.  User can browse
    /// through the list, select a file and perform operations
    /// on it such as opening it, or deleting it.  The files
    /// are all resident from folders that can be specified.
    /// Wildcards can be specified to select files only of a
    /// specific type or exclude files of a specific type.  The
    /// list can be sorted either alphabetically or by date.
    /// </summary>
    [DescriptorAttribute("D5DABE09-4B8C-4D1C-A778-9E0A7F8B7D69",
                            "FileBrowserScanner",
                            "File Browser Scanner")]
    public partial class FileBrowserScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// Dispatches commands
        /// </summary>
        private readonly RunCommandDispatcher _dispatcher;

        /// <summary>
        /// Used to invoke methods and properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// The keyboard actuator
        /// </summary>
        private readonly KeyboardActuator _keyboardActuator;

        /// <summary>
        /// The scanner common object
        /// </summary>
        private readonly ScannerCommon _scannerCommon;

        /// <summary>
        /// List of all files
        /// </summary>
        private List<FileInfo> _allFilesList;

        /// <summary>
        /// How may files to display per page
        /// </summary>
        private int _entriesPerPage;

        /// <summary>
        /// List of file extensions to exclude from the file filter
        /// </summary>
        private String[] _excludeFileExtensions;

        /// <summary>
        /// List of files currently in display
        /// </summary>
        private List<FileInfo> _fileList;

        /// <summary>
        /// Has the window handle been created
        /// </summary>
        private bool _handleCreated;

        /// <summary>
        /// List of file extensions to include in the filter
        /// </summary>
        private String[] _includeFileExtensions;

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
        /// Starting index in the list of the current page of entries
        /// </summary>
        private int _pageStartIndex;

        /// <summary>
        /// Widget that the user clicks to resort
        /// </summary>
        private Widget _sortButton;

        /// <summary>
        /// The current sort order
        /// </summary>
        private SortOrder _sortOrder = SortOrder.DateDescending;

        /// <summary>
        /// Widget that displays the current sort order
        /// </summary>
        private Widget _sortOrderWidget;

        /// <summary>
        /// Number of tabstop button widgets
        /// </summary>
        private int _tabStopButtonCount;

        /// <summary>
        /// Ensures that the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FileBrowserScanner()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();
            ActionVerb = R.GetString("Open");
            PanelClass = "FileBrowserScanner";

            _allFilesList = new List<FileInfo>();
            _fileList = new List<FileInfo>();

            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;

            _invoker = new ExtensionInvoker(this);
            SelectedFile = String.Empty;
            Folders = new String[] { };

            _includeFileExtensions = new String[] { };
            _excludeFileExtensions = new String[] { };

            KeyPreview = true;

            subscribeToEvents();

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new Dispatcher(this);

            statusStrip.SizingGrip = false;
        }

        /// <summary>
        /// For the event to indicate we are done
        /// </summary>
        /// <param name="flag">Confirm done?</param>
        public delegate void DoneEvent(bool flag);

        /// <summary>
        /// Raised when the user wants to quit
        /// </summary>
        public event DoneEvent EvtDone;

        /// <summary>
        /// Raised when the user wants to open a file
        /// </summary>
        public event EventHandler EvtFileOpen;

        /// <summary>
        /// Event raised to display the alphabet scanner
        /// </summary>
        public event EventHandler EvtShowScanner;

        /// <summary>
        /// What kinda operation?
        /// </summary>
        internal enum FileOperation
        {
            None,
            Open,
            Delete,
            UserChoice
        }

        /// <summary>
        /// How should the list be sorted?
        /// </summary>
        private enum SortOrder
        {
            AtoZ,
            ZtoA,
            DateAscending,
            DateDescending
        }

        /// <summary>
        /// Gets the verb that will be used in the prompt
        /// when the user selects a file
        /// </summary>
        public String ActionVerb { get; set; }

        /// <summary>
        /// Gets the object used to dispatch commands
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// Gets/sets the date format for file display
        /// </summary>
        public String DateFormat { get; set; }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get
            {
                return DescriptorAttribute.GetDescriptor(GetType());
            }
        }

        /// <summary>
        /// Gets a list of file extensions that will
        /// be excluded from selection
        /// </summary>
        public String[] ExcludeFileExtensions
        {
            set
            {
                _excludeFileExtensions = value ?? new String[] { };
                sanitizeFileExtensions(_excludeFileExtensions);
            }
        }

        /// <summary>
        /// Gets or sets the folders from which files will be displayed
        /// </summary>
        public String[] Folders { get; set; }

        /// <summary>
        /// Gets the form
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a list of file extensions that will
        /// be used to select files
        /// </summary>
        public String[] IncludeFileExtensions
        {
            set
            {
                _includeFileExtensions = value ?? new String[] { };
                sanitizeFileExtensions(_includeFileExtensions);
            }
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
        /// Gets the scannercommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the name of the file selected
        /// </summary>
        public String SelectedFile { get; private set; }

        /// <summary>
        /// Gets the object used for synch
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _scannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the textcontroller object
        /// </summary>
        public ITextController TextController
        {
            get { return _scannerCommon.TextController; }
        }

        /// <summary>
        /// Gets or sets what kinda action on the file when the user selects it
        /// </summary>
        internal FileOperation Action { get; set; }

        /// <summary>
        /// Set form styles
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
                case "FileListClearFilter":
                    arg.Handled = true;
                    arg.Enabled = !IsFilterEmpty();
                    break;

                case "FileListSort":
                case "FileListSearch":
                    arg.Handled = true;
                    arg.Enabled = (_fileList != null && _fileList.Any());
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
                if (SearchFilter.Text.Length > 0 && DialogUtils.ConfirmScanner(R.GetString("ClearFilter")))
                {
                    SearchFilter.Text = String.Empty;
                }
            }));
        }

        /// <summary>
        /// Returns the extension invoker object
        /// </summary>
        /// <returns>The invoker object</returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="startupArg">arguments</param>
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
        /// Returns if there is a search filter
        /// </summary>
        /// <returns>true if there is</returns>
        public bool IsFilterEmpty()
        {
            bool retVal = true;

            if (_handleCreated)
            {
                Invoke(new MethodInvoker(delegate
                {
                    retVal = (SearchFilter.Text.Length == 0);
                }));
            }

            return retVal;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
        }

        /// <summary>
        /// Pauses scanning
        /// </summary>
        public void OnPause()
        {
            PanelCommon.AnimationManager.Pause();
        }

        /// <summary>
        /// not needed
        /// </summary>
        /// <param name="eventArg">arg</param>
        /// <returns>true always</returns>
        public bool OnQueryPanelChange(PanelRequestEventArgs eventArg)
        {
            return true;
        }

        /// <summary>
        /// Resumes scanning
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
        /// Invoked when the user makes a selection
        /// </summary>
        /// <param name="widget">widget selected</param>
        /// <param name="handled">was it handled?</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            actuateWidget(widget, ref handled);
        }

        /// <summary>
        /// Normalizes the file extension, check for period etc.
        /// </summary>
        /// <param name="extensions">list of extensions</param>
        public void sanitizeFileExtensions(String[] extensions)
        {
            if (extensions == null)
            {
                return;
            }

            for (int ii = 0; ii < extensions.Length; ii++)
            {
                String s = extensions[ii];
                s = s.ToLower().Trim();
                if (s.Length > 0)
                {
                    if (s.Length > 2 && s.StartsWith("*."))
                    {
                        s = s.Substring(2);
                    }
                    else if (s.Length > 1 && s[0] == '.')
                    {
                        s = s.Substring(1);
                    }

                    extensions[ii] = s;
                }
            }
        }

        /// <summary>
        /// Not needed
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
        /// Clean up
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);

            Windows.EvtWindowPositionChanged -= Windows_EvtWindowPositionChanged;
            SearchFilter.TextChanged -= SearchFilter_TextChanged;
            SortOrderIcon.Click -= SortOrderIcon_Click;

            removeWatchdogs();

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
        /// Key press handler.  Process the ESC key to quit
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
        /// Actuates a widget - performs associated action
        /// </summary>
        /// <param name="widget">widget to actuate</param>
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
        /// Docks this scanner to the companian scanner
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
        /// Make sure the scanner stays focused
        /// </summary>
        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void FileBrowserScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void FileBrowserScanner_KeyDown(object sender, KeyEventArgs e)
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
        /// The form has loaded.  Initialze it.
        /// </summary>
        private void FileBrowserScanner_Load(object sender, EventArgs e)
        {
            Text = R.GetString("FileBrowser");

            enableWatchdogs();

            _scannerCommon.OnLoad();

            var list = new List<Widget>();
            PanelCommon.RootWidget.Finder.FindChild(typeof(TabStopScannerButton), list);

            _tabStopButtonCount = list.Count;

            _sortOrderWidget = PanelCommon.RootWidget.Finder.FindChild("SortOrderIcon");
            _pageNumberWidget = PanelCommon.RootWidget.Finder.FindChild("PageNumber");
            _sortButton = PanelCommon.RootWidget.Finder.FindChild("ButtonSort");

            SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;

            loadFiles();

            PanelCommon.RootWidget.HighlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }

            _handleCreated = true;

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Set focus to this scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void FileBrowserScanner_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Filter files based on the extensions to include
        /// and exclude
        /// </summary>
        /// <param name="fileList">file list</param>
        /// <param name="includeExtension">what to include</param>
        /// <param name="excludeExtension">what to exclude</param>
        /// <param name="filter">names to match</param>
        /// <returns>matching files</returns>
        private List<FileInfo> filterFiles(List<FileInfo> fileList,
                                            String[] includeExtension,
                                            String[] excludeExtension,
                                            String filter)
        {
            var trimFilter = filter.Trim();
            var retVal = new List<FileInfo>();

            foreach (var fileInfo in fileList)
            {
                if (!fileInfo.Name.StartsWith("~$") && includeFile(fileInfo, includeExtension, excludeExtension, trimFilter))
                {
                    retVal.Add(fileInfo);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Get all the files from the specified folders. Sort them
        /// depending on the current sort order
        /// </summary>
        /// <param name="folders">which folders</param>
        /// <param name="order">sort order</param>
        /// <returns>list</returns>
        private List<FileInfo> getAllFiles(String[] folders, SortOrder order)
        {
            var retVal = new List<FileInfo>();

            foreach (var folder in folders)
            {
                var fileList = getAllFiles(folder, order);
                if (fileList.Any())
                {
                    retVal.AddRange(fileList);
                }
            }

            switch (order)
            {
                case SortOrder.DateDescending:
                    retVal = retVal.OrderByDescending(f => f.LastWriteTime).ToList();
                    break;

                case SortOrder.DateAscending:
                    retVal = retVal.OrderBy(f => f.LastWriteTime).ToList();
                    break;

                case SortOrder.AtoZ:
                    retVal = retVal.OrderBy(f => f.Name).ToList();
                    break;

                case SortOrder.ZtoA:
                    retVal = retVal.OrderByDescending(f => f.Name).ToList();
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Gets all files from the specified folder
        /// </summary>
        /// <param name="folder">which folder</param>
        /// <param name="order">sort order</param>
        /// <returns>list of all files</returns>
        private List<FileInfo> getAllFiles(String folder, SortOrder order)
        {
            var retVal = new List<FileInfo>();

            if (Directory.Exists(folder))
            {
                var fileInfo = new DirectoryInfo(folder).GetFiles();

                switch (order)
                {
                    case SortOrder.DateDescending:
                        retVal = fileInfo.OrderByDescending(f => f.LastWriteTime).ToList();
                        break;

                    case SortOrder.DateAscending:
                        retVal = fileInfo.OrderBy(f => f.LastWriteTime).ToList();
                        break;

                    case SortOrder.AtoZ:
                        retVal = fileInfo.OrderBy(f => f.Name).ToList();
                        break;

                    case SortOrder.ZtoA:
                        retVal = fileInfo.OrderByDescending(f => f.Name).ToList();
                        break;

                    default:
                        retVal = fileInfo.ToList();
                        break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Prompt the user to make a selection on what to do with the
        /// selected file.  Open it or delete it.
        /// </summary>
        /// <param name="fileInfo">file info</param>
        /// <returns>selected operation</returns>
        private FileOperation getFileOperationFromUser(FileInfo fileInfo)
        {
            var retVal = FileOperation.None;
            Form form = Context.AppPanelManager.CreatePanel("FileOperationConfirmScanner", fileInfo.Name);
            if (form is FileOperationConfirmScanner)
            {
                var fileOpScanner = form as FileOperationConfirmScanner;
                fileOpScanner.FInfo = fileInfo;
                Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentForm(), form as IPanel);

                if (fileOpScanner.OpenFile)
                {
                    retVal = FileOperation.Open;
                }
                else if (fileOpScanner.DeleteFile)
                {
                    retVal = FileOperation.Delete;
                }
            }

            return retVal;
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
        /// Display the next pageful of files
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _allFilesList.Count)
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshFileList();
                }
            }
        }

        /// <summary>
        /// Display the previous pageful of files
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
                refreshFileList();
            }
        }

        /// <summary>
        /// Delete the file
        /// </summary>
        /// <param name="fileInfo">info about the file</param>
        private void handleDeleteFile(FileInfo fileInfo)
        {
            if (DialogUtils.ConfirmScanner(String.Format(R.GetString("DeleteFileQ"), fileInfo.Name)))
            {
                Windows.SetText(SearchFilter, String.Empty);
                File.Delete(fileInfo.FullName);
                loadFiles();
            }
        }

        private void handleOpenFile(FileInfo fileInfo)
        {
            if (DialogUtils.ConfirmScanner(String.Format(R.GetString("OpenFileQ"), fileInfo.Name)))
            {
                SelectedFile = fileInfo.FullName;
                if (EvtFileOpen != null)
                {
                    EvtFileOpen.BeginInvoke(this, new EventArgs(), null, null);
                }
            }
        }

        /// Handle actuation of a widget - navigate, select file
        /// etc depending on what the widget represents
        /// </summary>
        /// <param name="widget">widget to actuate</param>
        /// <param name="handled">true if handled</param>
        private void handleWidgetSelection(Widget widget, ref bool handled)
        {
            if (widget.UserData is FileInfo)
            {
                onFileSelected((FileInfo)widget.UserData);
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

                    case "@FileListSort":
                        switchSortOrder();
                        break;

                    case "@CmdNextPage":
                        gotoNextPage();
                        break;

                    case "@CmdPrevPage":
                        gotoPreviousPage();
                        break;

                    case "@FileListClearFilter":
                        ClearFilter();
                        break;

                    case "@FileListSearch":
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
        /// Turn all highlighting off
        /// </summary>
        private void highlightOff()
        {
            PanelCommon.RootWidget.HighlightOff();
        }

        /// <summary>
        /// Checks whether the specifed file should be selected or not
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <param name="includeExtension">extensions to include</param>
        /// <param name="excludeExtension">extensions to exclude</param>
        /// <param name="filter">search filter</param>
        /// <returns>true if there is a match</returns>
        private bool includeFile(FileInfo fileInfo, String[] includeExtension, String[] excludeExtension, String filter)
        {
            var extension = fileInfo.Extension.ToLower();
            bool add = true;

            if (String.IsNullOrEmpty(extension))
            {
                extension = "*.";
            }
            else if (extension.Length > 0 && extension[0] == '.')
            {
                extension = extension.Substring(1).ToLower();
            }

            if (!String.IsNullOrEmpty(filter) &&
                !fileInfo.Name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase))
            {
                add = false;
            }

            if (add)
            {
                if ((excludeExtension.Length > 0 &&
                    excludeExtension.Contains(extension)) || (includeExtension.Length > 0 &&
                    !includeExtension.Contains(extension)))
                {
                    add = false;
                }
            }

            return add;
        }

        /// <summary>
        /// Look at filters, load files from the  specified folders
        /// </summary>
        private void loadFiles()
        {
            _allFilesList = getAllFiles(Folders, _sortOrder);
            _fileList = filterFiles(_allFilesList, _includeFileExtensions, _excludeFileExtensions, Windows.GetText(SearchFilter));

            if (_tabStopButtonCount >= 3)
            {
                _entriesPerPage = _tabStopButtonCount - 2;
                refreshFileList();
            }
        }

        /// <summary>
        /// User selected a file from the list.  If reqd,
        /// ask the user if she wants to open or delete the file
        /// </summary>
        /// <param name="fileInfo">FileInfo of the file selected</param>
        /// <returns>true on success</returns>
        private bool onFileSelected(FileInfo fileInfo)
        {
            bool doHighlightOff = true;

            if (fileInfo == null || !File.Exists(fileInfo.FullName))
            {
                return true;
            }

            var operation = (Action == FileOperation.UserChoice) ?
                                        getFileOperationFromUser(fileInfo) :
                                        Action;

            if (operation == FileOperation.Open)
            {
                handleOpenFile(fileInfo);
            }
            else if (operation == FileOperation.Delete)
            {
                handleDeleteFile(fileInfo);
            }

            if (operation == FileOperation.Open)
            {
                doHighlightOff = false;
            }
            else
            {
                SelectedFile = String.Empty;
            }

            return doHighlightOff;
        }

        /// <summary>
        /// Refreshes the list of files in the UI
        /// </summary>
        private void refreshFileList()
        {
            var list = new List<Widget>();
            PanelCommon.RootWidget.Finder.FindChild(typeof(TabStopScannerButton), list);

            int count = list.Count;
            if (count == 0)
            {
                return;
            }

            foreach (Widget button in list)
            {
                button.UserData = null;
                button.SetText(String.Empty);
            }

            _entriesPerPage = count;
            _numPages = _fileList.Count() / _entriesPerPage;

            if ((_fileList.Count() % _entriesPerPage) != 0)
            {
                _numPages++;
            }

            updateButtonBar();

            updateStatusBar();

            if (!_fileList.Any())
            {
                (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 100 });
                list[0].SetText("\t" + R.GetString("NoFilesFound"));
                return;
            }

            int ii = 0;
            var image = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(image);
            int tabStop = 500;

            for (int jj = _pageStartIndex; jj < _fileList.Count && ii < count; ii++, jj++)
            {
                var tabStopScannerButton = list[ii] as TabStopScannerButton;
                tabStopScannerButton.SetTabStops(0.0f, new float[] { 25, tabStop });
                list[ii].UserData = _fileList[jj];

                var name = _fileList[jj].Name;

                var str = getMeasuredString(graphics, tabStopScannerButton.UIControl.Font, tabStop, name);

                string shortDateFormatString = System.Globalization.CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat.ShortDatePattern;

                list[ii].SetText(str + "\t" + _fileList[jj].LastWriteTime.ToString(shortDateFormatString));
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
        /// The search filter changed.  Reload the file list
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SearchFilter_TextChanged(object sender, EventArgs e)
        {
            _pageNumber = 0;
            _pageStartIndex = 0;
            _fileList = filterFiles(_allFilesList, _includeFileExtensions, _excludeFileExtensions, Windows.GetText(SearchFilter));
            refreshFileList();
        }

        /// <summary>
        /// Set the style of the form
        /// </summary>
        /// <param name="createParams"></param>
        /// <returns>The form styles</returns>
        private CreateParams setFormStyles(CreateParams createParams)
        {
            createParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_COMPOSITED;
            return createParams;
        }

        /// <summary>
        /// User click on the button to resort
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
            FormClosing += FileBrowserScanner_FormClosing;
            Shown += FileBrowserScanner_Shown;
            KeyDown += FileBrowserScanner_KeyDown;
        }

        /// <summary>
        /// Resort the file list and refresh it in the display
        /// </summary>
        private void switchSortOrder()
        {
            switch (_sortOrder)
            {
                case SortOrder.DateDescending:
                    _sortOrder = SortOrder.DateAscending;
                    break;

                case SortOrder.DateAscending:
                    _sortOrder = SortOrder.AtoZ;
                    break;

                case SortOrder.AtoZ:
                    _sortOrder = SortOrder.ZtoA;
                    break;

                case SortOrder.ZtoA:
                    _sortOrder = SortOrder.DateDescending;
                    break;
            }

            _pageNumber = 0;
            _pageStartIndex = 0;
            _allFilesList = getAllFiles(Folders, _sortOrder);
            _fileList = filterFiles(_allFilesList, _includeFileExtensions, _excludeFileExtensions, Windows.GetText(SearchFilter));
            refreshFileList();
        }

        /// <summary>
        /// Updates the icons in the button bar depending on
        /// the context
        /// </summary>
        private void updateButtonBar()
        {
            String text;

            if (!_fileList.Any())
            {
                text = String.Empty;
            }
            else if (_sortOrder == SortOrder.DateAscending || _sortOrder == SortOrder.AtoZ)
            {
                text = "\u003A";
            }
            else
            {
                text = "\u003B";
            }

            if (_sortOrderWidget != null)
            {
                _sortOrderWidget.SetText(text);
            }

            if (_sortButton != null)
            {
                String buttonText;

                if (_sortOrder == SortOrder.DateAscending || _sortOrder == SortOrder.DateDescending)
                {
                    buttonText = "9";
                }
                else
                {
                    buttonText = "0";
                }

                _sortButton.SetText(buttonText);
            }

            if (_pageNumberWidget != null)
            {
                var str = String.Format(R.GetString("PageNofM"), (_pageNumber + 1), _numPages);
                text = (_fileList.Any()) ? str : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// Updates the status bar with the current sort order
        /// </summary>
        private void updateStatusBar()
        {
            var text = String.Empty;

            if (!_fileList.Any())
            {
                toolStripStatusLabel1.Text = String.Empty;
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

                case SortOrder.DateAscending:
                    text = R.GetString("SortOrderChronological");
                    break;

                case SortOrder.DateDescending:
                    text = R.GetString("SortOrderReverseChronological");
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

                var form = Dispatcher.Scanner.Form as FileBrowserScanner;

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