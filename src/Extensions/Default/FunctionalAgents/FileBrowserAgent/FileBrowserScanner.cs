////////////////////////////////////////////////////////////////////////////
// <copyright file="FileBrowserScanner.cs" company="Intel Corporation">
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
using System.IO;
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

namespace ACAT.Extensions.Hawking.FunctionalAgents.FileBrowser
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
    [DescriptorAttribute("D5DABE09-4B8C-4D1C-A778-9E0A7F8B7D69", "FileBrowserScanner", "File Browser Scanner")]
    public partial class FileBrowserScanner : Form, IScannerPanel, IExtension
    {
        /// <summary>
        /// Max chars of file name.  if length exceeds, ellipses
        /// are displayed
        /// </summary>
        private const int MaxFileNameChars = 30;

        /// <summary>
        /// Dispatches commands
        /// </summary>
        private readonly RunCommandDispatcher _dispatcher;

        /// <summary>
        /// Used to invoke methods and properties in this class
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        private readonly KeyboardActuator _keyboardActuator;

        /// <summary>
        /// List of all files
        /// </summary>
        private List<FileInfo> _allFilesList;

        /// <summary>
        /// Scanner form with which this form is docked
        /// </summary>
        private Form _dockedWithForm;

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
        /// The scanner common object
        /// </summary>
        private ScannerCommon _scannerCommon;

        /// <summary>
        /// The current sort order
        /// </summary>
        private SortOrder _sortOrder = SortOrder.Date;

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
            InitializeComponent();
            ActionVerb = "Open";
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

            FormClosing += FileBrowserScanner_FormClosing;
            Shown += FileBrowserScanner_Shown;
            KeyDown += FileBrowserScanner_KeyDown;
            LocationChanged += FileBrowserScanner_LocationChanged;

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new RunCommandDispatcher(this);
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
        /// What kinda operation?
        /// </summary>
        private enum FileOperation
        {
            None,
            Open,
            Delete
        }

        /// <summary>
        /// How should the list be sorted?
        /// </summary>
        private enum SortOrder
        {
            Name,
            Date
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
        /// Gets the scannercommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets or sets whether the file should be opened when
        /// the user selects it.  If false, presents options
        /// </summary>
        public bool SelectActionOpen { get; set; }

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
        public bool CheckWidgetEnabled(CheckEnabledArgs arg)
        {
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
        /// Returns if there is a search filter
        /// </summary>
        /// <returns>true if there is</returns>
        public bool IsFilterEmpty()
        {
            bool retVal = true;
            if (_handleCreated)
            {
                Invoke(new MethodInvoker(delegate()
                {
                    retVal = (SearchFilter.Text.Length == 0);
                }));
            }

            return retVal;
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
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
        /// not needed
        /// </summary>
        /// <param name="eventArg">arg</param>
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
                Log.Debug("Unlandled command " + command);
                handled = false;
            }
        }

        /// <summary>
        /// Invoked when the user makes a selection
        /// </summary>
        /// <param name="widget">widget selected</param>
        /// <param name="handled">was it handled?</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            if (widget is TabStopScannerButton)
            {
                handled = true;
            }
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
        /// Clean up
        /// </summary>
        /// <param name="e">event args</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            SearchFilter.TextChanged -= SearchFilter_TextChanged;
            SortOrderIcon.Click -= SortOrderIcon_Click;

            removeWatchdogs();

            PanelManager.Instance.EvtScannerShow -= Instance_EvtScannerShow;

            _keyboardActuator.EvtKeyPress -= _keyboardActuator_EvtKeyPress;
            base.OnFormClosing(e);
        }

        /// <summary>
        /// window proc
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            _scannerCommon.HandleWndProc(m);
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
        /// Actuate a widget
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
                    handleSelect(obj as ItemTag);
                }

                highlightOff();
            }
        }

        /// <summary>
        /// Docks this scanner to the companian scanner
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
        /// The form has loaded.  Initialze
        /// </summary>
        private void FileBrowserScanner_Load(object sender, EventArgs e)
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

            SearchFilter.TextChanged += SearchFilter_TextChanged;
            SortOrderIcon.Click += SortOrderIcon_Click;

            loadFiles();

            _scannerCommon.GetRootWidget().HighlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }

            _handleCreated = true;
        }

        /// <summary>
        /// Keep the scanner docked
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void FileBrowserScanner_LocationChanged(object sender, EventArgs e)
        {
            if (_dockedWithForm != null)
            {
                dockToScanner(_dockedWithForm);
            }
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
                case SortOrder.Date:
                    retVal = retVal.OrderByDescending(f => f.LastWriteTime).ToList();
                    break;

                case SortOrder.Name:
                    retVal = retVal.OrderBy(f => f.Name).ToList();
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
                    case SortOrder.Date:
                        retVal = fileInfo.OrderByDescending(f => f.LastWriteTime).ToList();
                        break;

                    case SortOrder.Name:
                        retVal = fileInfo.OrderBy(f => f.Name).ToList();
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
        /// <param name="itemTag">file info</param>
        /// <returns>selected operation</returns>
        private FileOperation getFileOperationFromUser(ItemTag itemTag)
        {
            var retVal = FileOperation.None;
            Form form = Context.AppPanelManager.CreatePanel("FileOperationConfirmScanner", itemTag.FInfo.Name);
            if (form is FileOperationConfirmScanner)
            {
                var fileOpScanner = form as FileOperationConfirmScanner;
                fileOpScanner.FInfo = itemTag.FInfo;
                Context.AppPanelManager.ShowDialog(Context.AppPanelManager.GetCurrentPanel(), form as IPanel);

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
        /// Display the next pageful of files
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _allFilesList.Count())
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
        /// <param name="itemTag">info about the file</param>
        private void handleDeleteFile(ItemTag itemTag)
        {
            Windows.SetText(SearchFilter, String.Empty);
            File.Delete(itemTag.FInfo.FullName);
            loadFiles();
        }

        /// <summary>
        /// Perform action on the file
        /// </summary>
        /// <param name="operation">what to do?</param>
        /// <param name="itemTag">File info</param>
        private void handleFileOperation(FileOperation operation, ItemTag itemTag)
        {
            switch (operation)
            {
                case FileOperation.Open:
                    SelectedFile = itemTag.FInfo.FullName;
                    if (EvtFileOpen != null)
                    {
                        EvtFileOpen.BeginInvoke(this, new EventArgs(), null, null);
                    }

                    break;

                case FileOperation.Delete:
                    handleDeleteFile(itemTag);
                    break;
            }
        }

        /// <summary>
        /// Highlight the specifed widget. cmd suffix indicates
        ///  the index number of the widget
        /// </summary>
        /// <param name="cmd">highlight command</param>
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
                    String widgetName = "Item" + cmd.Substring(index + 1);
                    highlight(widgetName);
                }
            }
        }

        /// <summary>
        /// Handle selecion of a file
        /// </summary>
        /// <param name="cmd">which one to select?</param>
        private void handleSelect(String cmd)
        {
            int index = cmd.LastIndexOf('_');
            if (index >= 0 && index < cmd.Length - 1)
            {
                actuateWidget("Item" + cmd.Substring(index + 1));
            }
        }

        /// <summary>
        /// Handle actuation of a widget - navigate, select file
        /// etc depending on what the widget represents
        /// </summary>
        /// <param name="itemTag"></param>
        private void handleSelect(ItemTag itemTag)
        {
            bool doHighlightOff = true;

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

                case ItemTag.ItemType.File:
                    doHighlightOff = onFileSelected(itemTag);
                    break;
            }

            if (doHighlightOff)
            {
                highlightOff();
            }
        }

        /// <summary>
        /// Highlight the specified widget
        /// </summary>
        /// <param name="widgetName">which one?</param>
        private void highlight(String widgetName)
        {
            _scannerCommon.GetRootWidget().HighlightOff();
            Widget widget = _scannerCommon.GetRootWidget().Finder.FindChild(widgetName);
            if (widget != null)
            {
                widget.HighlightOn();
            }
        }

        /// <summary>
        /// Turn all highlighting off
        /// </summary>
        private void highlightOff()
        {
            _scannerCommon.GetRootWidget().HighlightOff();
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

            if (!String.IsNullOrEmpty(filter) && !fileInfo.Name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase))
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
        /// Invoked when the companian scanner is shown. Dock
        /// this scanner with the companian.
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
        /// <param name="itemTag">Tag of the file item selected</param>
        /// <returns>true on success</returns>
        private bool onFileSelected(ItemTag itemTag)
        {
            bool doHighlightOff = true;

            if (itemTag != null && itemTag.FInfo != null && File.Exists(itemTag.FInfo.FullName))
            {
                FileOperation operation = FileOperation.None;
                if (SelectActionOpen)
                {
                    if (DialogUtils.ConfirmScanner(ActionVerb + " " + itemTag.FInfo.Name + "?"))
                    {
                        operation = FileOperation.Open;
                    }
                }
                else
                {
                    operation = getFileOperationFromUser(itemTag);
                }

                if (operation != FileOperation.None)
                {
                    handleFileOperation(operation, itemTag);
                    if (operation == FileOperation.Open)
                    {
                        doHighlightOff = false;
                    }
                }
                else
                {
                    SelectedFile = String.Empty;
                }
            }

            return doHighlightOff;
        }

        /// <summary>
        /// Refreshes the list of files in the UI
        /// </summary>
        private void refreshFileList()
        {
            var list = new List<Widget>();
            _scannerCommon.GetRootWidget().Finder.FindChild(typeof(TabStopScannerButton), list);

            int count = list.Count();
            if (count >= 3)
            {
                foreach (Widget button in list)
                {
                    button.UserData = null;
                    button.SetText(String.Empty);
                }

                _entriesPerPage = count - 2;
                _numPages = _fileList.Count() / _entriesPerPage;

                if ((_fileList.Count() % _entriesPerPage) != 0)
                {
                    _numPages++;
                }

                updateStatusBar();

                if (!_fileList.Any())
                {
                    (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25, 400 });
                    list[0].SetText("------------- NO FILES FOUND -------------");
                    return;
                }

                int ii = 0;

                int displayIndex = (ii + 1) % 10;

                (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25, 400 });
                if (_pageNumber == 0)
                {
                    list[ii].UserData = new ItemTag(ItemTag.ItemType.OrderBy);
                    if (_sortOrder == SortOrder.Date)
                    {
                        list[ii].SetText(displayIndex + ".\t------------- SORT BY NAME -------------");
                    }
                    else
                    {
                        list[ii].SetText(displayIndex + ".\t------------- SORT BY DATE -------------");
                    }
                }
                else
                {
                    list[ii].UserData = new ItemTag(ItemTag.ItemType.PreviousPage);
                    list[ii].SetText(displayIndex + ".\t------------- PREVIOUS PAGE  -------------");
                }

                ii++;

                for (int jj = _pageStartIndex; jj < _fileList.Count && ii < count - 1; ii++, jj++)
                {
                    displayIndex = (ii + 1) % 10;
                    (list[ii] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 25, 400 });
                    list[ii].UserData = new ItemTag(_fileList[jj]);
                    String name = _fileList[jj].Name;
                    if (name.Length > MaxFileNameChars)
                    {
                        name = name.Substring(0, MaxFileNameChars) + "...";
                    }

                    list[ii].SetText(displayIndex + ".\t" + name + "\t" + _fileList[jj].LastWriteTime.ToString(Common.AppPreferences.FileBrowserDateFormat));
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
        /// Resort the file list and refresh it in the display
        /// </summary>
        private void switchSortOrder()
        {
            _sortOrder = _sortOrder == SortOrder.Date ? SortOrder.Name : SortOrder.Date;

            _pageNumber = 0;
            _pageStartIndex = 0;
            _allFilesList = getAllFiles(Folders, _sortOrder);
            _fileList = filterFiles(_allFilesList, _includeFileExtensions, _excludeFileExtensions, Windows.GetText(SearchFilter));
            refreshFileList();
        }

        /// <summary>
        /// Updates status bar with page number
        /// </summary>
        private void updateStatusBar()
        {
            if (_sortOrderWidget != null)
            {
                String text;
                if (_fileList.Any() && _sortOrder == SortOrder.Date)
                {
                    text = "9";
                }
                else
                {
                    text = "0";
                }

                _sortOrderWidget.SetText(text);
            }

            if (_pageNumberWidget != null)
            {
                var text = (_fileList.Any()) ? "Page " + (_pageNumber + 1) + " of " + _numPages : String.Empty;
                _pageNumberWidget.SetText(text);
            }
        }

        /// <summary>
        /// User clicked on a file
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void widget_EvtMouseClicked(object sender, WidgetEventArgs e)
        {
            actuateWidget(e.SourceWidget.Name);
        }

        /// <summary>
        /// Tag to keep track of info of a widget
        /// in the file list.  Holds file information
        /// </summary>
        private class ItemTag
        {
            public ItemTag(ItemType type)
            {
                DataType = type;
                FInfo = null;
            }

            public ItemTag(FileInfo info)
            {
                DataType = ItemType.File;
                FInfo = info;
            }

            public enum ItemType
            {
                OrderBy,
                PreviousPage,
                NextPage,
                File
            }

            public ItemType DataType { get; private set; }

            public FileInfo FInfo { get; private set; }
        }
    }
}