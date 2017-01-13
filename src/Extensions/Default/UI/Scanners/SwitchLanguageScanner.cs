////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchLanguageScanner.cs" company="Intel Corporation">
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
using System.Globalization;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using Windows = ACAT.Lib.Core.Utility.Windows;

namespace ACAT.Extensions.Default.UI.Scanners
{
    /// <summary>
    /// Displays a list of languages (cultures) discovered. These
    /// are ACAT Language packs which contain translations of all
    /// ACAT strings into other languages, word prediction, and
    /// keyboard customized as well.
    /// User selects the preferred language to switch ACAT
    /// to that language.
    /// </summary>
    [DescriptorAttribute("FFC04277-CDAE-4A18-BB00-D941A18268E4",
                            "SwitchLanguageScanner",
                            "Enables switching ACAT between different languages")]
    public partial class SwitchLanguageScanner : Form, IScannerPanel, IExtension
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
        /// Scanner form with which this form is docked
        /// </summary>
        private Form _dockedWithForm;

        /// <summary>
        /// How may entries to display per page
        /// </summary>
        private int _entriesPerPage;

        /// <summary>
        /// List of languages
        /// </summary>
        private IEnumerable<CultureInfo> _languagesList;

        /// <summary>
        /// The total number of pages
        /// </summary>
        private int _numPages;

        /// <summary>
        /// The current page number
        /// </summary>
        private int _pageNumber;

        /// <summary>
        /// Starting index in the list of the current page of entries
        /// </summary>
        private int _pageStartIndex;

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
        public SwitchLanguageScanner()
        {
            _scannerCommon = new ScannerCommon(this);

            InitializeComponent();

            PanelClass = "SwitchLanguageScanner";

            _pageStartIndex = 0;
            _pageNumber = 0;
            _numPages = 0;

            _invoker = new ExtensionInvoker(this);
            SelectedLanguage = String.Empty;

            KeyPreview = true;

            subscribeToEvents();

            var actuator = ActuatorManager.Instance.GetActuator(typeof(KeyboardActuator));
            if (actuator is KeyboardActuator)
            {
                _keyboardActuator = actuator as KeyboardActuator;
                _keyboardActuator.EvtKeyPress += _keyboardActuator_EvtKeyPress;
            }

            _dispatcher = new RunCommandDispatcher(this);

            Text = R.GetString("SwitchLanguage");

            statusStrip1.SizingGrip = false;
        }

        /// <summary>
        /// For the event to indicate we are done
        /// </summary>
        /// <param name="flag">Confirm done?</param>
        public delegate void DoneEvent(bool flag);

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
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

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
        /// Gets the scannercommon object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the name of the language selected
        /// </summary>
        public String SelectedLanguage { get; private set; }

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

                default:
                    arg.Handled = false;
                    break;
            }

            return false;
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

            PanelManager.Instance.EvtScannerShow += Instance_EvtScannerShow;
            PanelManager.Instance.EvtScannerClosed += Instance_EvtScannerClosed;

            return true;
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
            PanelCommon.AnimationManager.Resume();
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
        /// Invoked when the user makes a selection
        /// </summary>
        /// <param name="widget">widget selected</param>
        /// <param name="handled">was it handled?</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            actuateWidget(widget, ref handled);
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

            removeWatchdogs();

            PanelManager.Instance.EvtScannerShow -= Instance_EvtScannerShow;
            PanelManager.Instance.EvtScannerClosed -= Instance_EvtScannerClosed;

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
            int c = e.KeyChar;
            if (c == 27)
            {
                Windows.CloseAsync(this);
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
        /// Docks this scanner to the companian scanner
        /// </summary>
        /// <param name="scanner">companian scanner</param>
        private void dockToScanner(Form scanner)
        {
            if (scanner is IScannerPanel)
            {
                if (((IPanel)scanner).PanelCommon.DisplayMode != DisplayModeTypes.Popup)
                {
                    Windows.DockWithScanner(this, scanner, Context.AppWindowPosition);
                }
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
        /// Enumerate installed languages and display the entries
        /// in the list
        /// </summary>
        private void enumerateLanguagesInstalled()
        {
            _languagesList = ResourceUtils.EnumerateInstalledLanguages(true);
            if (_tabStopButtonCount >= 3)
            {
                _entriesPerPage = _tabStopButtonCount - 2;
                refreshLanguagesList();
            }
        }

        /// <summary>
        /// Display the next pageful of entries
        /// </summary>
        private void gotoNextPage()
        {
            if (_pageNumber < _numPages - 1)
            {
                int index = _pageStartIndex + _entriesPerPage;
                if (index < _languagesList.Count())
                {
                    _pageStartIndex += _entriesPerPage;
                    _pageNumber++;
                    refreshLanguagesList();
                }
            }
        }

        /// <summary>
        /// Display the previous pageful of entries
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
                refreshLanguagesList();
            }
        }

        /// <summary>
        /// Handle actuation of a widget - navigate, select language
        /// etc depending on what the widget represents
        /// </summary>
        /// <param name="widget">widget to actuate</param>
        /// <param name="handled">true if handled</param>
        private void handleWidgetSelection(Widget widget, ref bool handled)
        {
            if (widget.UserData is CultureInfo)
            {
                onLanguageSelected((CultureInfo)widget.UserData);
                handled = true;
            }
            else
            {
                handled = true;
                switch (widget.Value)
                {
                    case "@Quit":
                        Windows.CloseAsync(this);
                        break;

                    case "@CmdNextPage":
                        gotoNextPage();
                        break;

                    case "@CmdPrevPage":
                        gotoPreviousPage();
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
        /// Event handler for when a scanner closes.  Reposition this scanner
        /// to its default position
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void Instance_EvtScannerClosed(object sender, ScannerCloseEventArg arg)
        {
            if (arg.Scanner != this)
            {
                if (_dockedWithForm == arg.Scanner)
                {
                    _dockedWithForm = null;
                }
                _scannerCommon.PositionSizeController.AutoSetPosition();
            }
        }

        /// <summary>
        /// Invoked when the companian scanner is shown. Dock
        /// this scanner with the companian.
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
        /// User selected a language from the list.  If reqd,
        /// ask the user to confirm the switch
        /// </summary>
        /// <param name="cultureInfo">Cultureinfo of the language selected</param>
        /// <returns>true on success</returns>
        private void onLanguageSelected(CultureInfo cultureInfo)
        {
            if (DialogUtils.ConfirmScanner(String.Format(R.GetString("ConfirmSwitchLanguage"), cultureInfo.DisplayName)))
            {
                Windows.SetVisible(this, false);

                var toastForm = new ToastForm(R.GetString("PleaseWait"), -1);
                Windows.SetWindowPosition(toastForm, Windows.WindowPosition.CenterScreen);
                toastForm.Show();

                Invoke(new MethodInvoker(delegate
                {
                    Context.ChangeCulture(cultureInfo);
                }));

                toastForm.Close();

                var prefs = ACATPreferences.Load();
                prefs.Language = cultureInfo.Name;
                prefs.Save();

                Windows.CloseAsync(this);
            }
        }

        /// <summary>
        /// Refreshes the list of entries in the UI
        /// </summary>
        private void refreshLanguagesList()
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
            _numPages = _languagesList.Count() / _entriesPerPage;

            if ((_languagesList.Count() % _entriesPerPage) != 0)
            {
                _numPages++;
            }

            updateStatusBar();

            if (!_languagesList.Any())
            {
                (list[0] as TabStopScannerButton).SetTabStops(0.0f, new float[] { 100 });
                list[0].SetText("\t" + R.GetString("LanguagesListIsEmpty"));
                return;
            }

            int ii = 0;
            var image = new Bitmap(1, 1);
            var graphics = Graphics.FromImage(image);
            int tabStop = 500;

            for (int jj = _pageStartIndex; jj < _languagesList.Count() && ii < count; ii++, jj++)
            {
                var tabStopScannerButton = list[ii] as TabStopScannerButton;
                tabStopScannerButton.SetTabStops(0.0f, new float[] { 25, tabStop });

                list[ii].UserData = _languagesList.ElementAt(jj);

                var name = _languagesList.ElementAt(jj).DisplayName;

                list[ii].SetText(name + " (" + _languagesList.ElementAt(jj).Name + ")");
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
        /// Displays a timed dialog with the title and message
        /// </summary>
        /// <param name="title">title of the dialog</param>
        /// <param name="message">message</param>
        private void showTimedDialog(String title, String message)
        {
            _windowActiveWatchdog.Pause();
            DialogUtils.ShowTimedDialog(PanelManager.Instance.GetCurrentPanel() as Form, title, message);
            _windowActiveWatchdog.Resume();
        }

        /// <summary>
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            FormClosing += SwitchLanguageScanner_FormClosing;
            Shown += SwitchLanguageScanner_Shown;
            KeyDown += SwitchLanguageScanner_KeyDown;
            LocationChanged += SwtichLanguageScanner_LocationChanged;
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void SwitchLanguageScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
        }

        /// <summary>
        /// Key down handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SwitchLanguageScanner_KeyDown(object sender, KeyEventArgs e)
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
        private void SwitchLanguageScanner_Load(object sender, EventArgs e)
        {
            enableWatchdogs();

            _scannerCommon.OnLoad();

            var list = new List<Widget>();
            PanelCommon.RootWidget.Finder.FindChild(typeof(TabStopScannerButton), list);

            _tabStopButtonCount = list.Count;

            enumerateLanguagesInstalled();

            if (!_languagesList.Any())
            {
                var prompt = R.GetString("NoOtherLanguagesAreInstalled");
                showTimedDialog(Common.AppPreferences.AppName, prompt);
                Close();
            }

            PanelCommon.RootWidget.HighlightOff();

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                dockToScanner(panel as Form);
            }

            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Set focus to this scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SwitchLanguageScanner_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// If there is a companion, keep it docked with this form
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void SwtichLanguageScanner_LocationChanged(object sender, EventArgs e)
        {
            if (_dockedWithForm != null)
            {
                dockToScanner(_dockedWithForm);
            }
        }

        /// <summary>
        /// Updates the status bar with page
        /// </summary>
        private void updateStatusBar()
        {
            var str = String.Format(R.GetString("PageNofM"), (_pageNumber + 1), _numPages);
            toolStripStatusLabel1.Text = (_languagesList.Any()) ? str : String.Empty;
        }
    }
}