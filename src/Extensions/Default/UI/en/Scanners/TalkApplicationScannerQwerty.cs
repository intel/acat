////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkApplicationScannerQwerty.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using ACAT.Lib.Extension.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Scanners
{
    /// <summary>
    /// Scanner form for a Talk-only interface.  Displays a text box with a
    /// reduced alphabet scanner below it enabling the user to type text (with
    /// word prediction) and have the text converted to speech.  The keyboard
    /// layout is QWERTY.
    /// </summary>
    [DescriptorAttribute("D9A5B53F-7119-445B-BDEA-F76EC53077F1",
                        "TalkApplicationScannerQwerty",
                        "Talk application window with Qwerty layout")]
    public partial class TalkApplicationScannerQwerty : Form, IScannerPanel, ISupportsStatusBar, IScannerPreview
    {
        /// <summary>
        /// The command dispatcher object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// The AlphabetScannerCommon object. Has a number of
        /// helper functions
        /// </summary>
        private readonly AlphabetScannerCommon _alphabetScannerCommon;

        /// <summary>
        /// Should the scanner be dimmed
        /// </summary>
        private bool _dimScanner;

        /// <summary>
        /// Should all buttons be disabled?
        /// </summary>
        private bool _disableAllButtons;

        /// <summary>
        /// Should the ActiveWindow watchdog be paused?
        /// </summary>
        private bool _pauseWatchdog;

        /// <summary>
        /// Ensures that the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TalkApplicationScannerQwerty()
        {
            _alphabetScannerCommon = new AlphabetScannerCommon(this);

            InitializeComponent();

            subscribeToEvents();

            _dimScanner = true;

            _dispatcher = new Dispatcher(this);
        }

        /// <summary>
        /// Gets the command dispatcher object
        /// </summary>
        public RunCommandDispatcher CommandDispatcher
        {
            get { return _dispatcher; } //_alphabetScannerCommon.Dispatcher; }
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets this form object
        /// </summary>
        public Form Form
        {
            get { return this; }
        }

        /// <summary>
        /// Gets the panel class for the scanner
        /// </summary>
        public String PanelClass
        {
            get { return _alphabetScannerCommon.PanelClass; }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _alphabetScannerCommon.PanelCommon; } }

        /// <summary>
        /// Gets or sets the preview mode. This is used in
        /// the design mode where the user can zoom in/out
        /// the scanner
        /// </summary>
        public bool PreviewMode
        {
            get
            {
                return _alphabetScannerCommon.PreviewMode;
            }

            set
            {
                _alphabetScannerCommon.PreviewMode = value;
            }
        }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon ScannerCommon
        {
            get { return _alphabetScannerCommon.ScannerCommon; }
        }

        /// <summary>
        /// Gets the status bar control for this scanner
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon.StatusBar; }
        }

        /// <summary>
        /// Gets the synch object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _alphabetScannerCommon.SyncObj; }
        }

        /// <summary>
        /// Gets the text controller object for this scanner
        /// </summary>
        public ITextController TextController
        {
            get { return _alphabetScannerCommon.TextController; }
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            switch (arg.Command)
            {
                case "CmdTalkWindowClear":
                    arg.Handled = true;
                    arg.Enabled = TextBoxTalkWindow.Text.Length != 0;
                    break;

                default:
                    _alphabetScannerCommon.CheckCommandEnabled(arg);
                    break;
            }

            return true;
        }

        /// <summary>
        /// Returns all the controls in the form (recusrively finds them)
        /// </summary>
        /// <param name="control">parent control</param>
        /// <param name="type">type of control to look for</param>
        /// <returns>list of controls</returns>
        public IEnumerable<Control> GetAll(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, type))
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        /// <summary>
        /// Intitialize the class
        /// </summary>
        /// <param name="startupArg">startup params</param>
        /// <returns>true on cussess</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _alphabetScannerCommon.EvtFormatPreditionWord += _homeScreenCommon_EvtFormatPreditionWord;

            bool retVal = _alphabetScannerCommon.Initialize(startupArg);
            if (retVal)
            {
                ScannerCommon.SetStatusBar(statusStrip);
            }

            //FormBorderStyle = FormBorderStyle.Sizable;
            ControlBox = true;
            return retVal;
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _alphabetScannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animations
        /// </summary>
        public void OnPause()
        {
            if (_pauseWatchdog)
            {
                removeWatchdogs();
            }

            if (_disableAllButtons)
            {
                disableAllButtons();
            }

            _alphabetScannerCommon.PanelCommon.AnimationManager.Pause();

            _alphabetScannerCommon.ScannerCommon.OnPause((_dimScanner ?
                                ScannerCommon.PauseDisplayMode.FadeScanner :
                                ScannerCommon.PauseDisplayMode.None));
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
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            enableWatchdogs();

            enableAllButtons();

            _dimScanner = true;

            _alphabetScannerCommon.PanelCommon.AnimationManager.Resume();

            _alphabetScannerCommon.ScannerCommon.OnResume();

            _alphabetScannerCommon.ResizeToFitDesktop(this);
        }

        /// <summary>
        /// Triggered when the user actuates a widget
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled?</param>
        public void OnWidgetActuated(Widget widget, ref bool handled)
        {
            _alphabetScannerCommon.OnWidgetActuated(widget, ref handled);
        }

        /// <summary>
        /// Restores default zoom and position settings
        /// </summary>
        public void RestoreDefaults()
        {
            _alphabetScannerCommon.RestoreDefaults();
        }

        /// <summary>
        /// Save the current scale setting
        /// </summary>
        public void SaveScaleSetting()
        {
            _alphabetScannerCommon.SaveScaleSetting();
        }

        /// <summary>
        /// Saves the current zoom settings
        /// </summary>
        public void SaveSettings()
        {
            _alphabetScannerCommon.SaveSettings();
        }

        /// <summary>
        /// Sets the default scale factor
        /// </summary>
        public void ScaleDefault()
        {
            _alphabetScannerCommon.ScaleDefault();
        }

        /// <summary>
        /// Zooms out the scanner
        /// </summary>
        public void ScaleDown()
        {
            _alphabetScannerCommon.ScaleDown();
        }

        /// <summary>
        /// Zooms in the scanner
        /// </summary>
        public void ScaleUp()
        {
            _alphabetScannerCommon.ScaleUp();
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
            _alphabetScannerCommon.OnClientSizeChanged();
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">closing param</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _alphabetScannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m">windows message</param>
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xfff0;
                if (command == SC_MOVE)
                {
                    base.WndProc(ref m);
                    return;
                }
            }

            if (!_alphabetScannerCommon.WndProc(ref m))
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// Handler to format the word prediction words.  Add
        /// an index number prefix.
        /// </summary>
        /// <param name="index">index of the word</param>
        /// <param name="word">word</param>
        /// <returns>formatted string</returns>
        private string _homeScreenCommon_EvtFormatPreditionWord(int index, string word)
        {
            return Common.AppPreferences.PrefixNumbersInWordPredictionList ?
                        (((index == 10) ? 0 : index) + ": " + word) :
                        word;
        }

        /// <summary>
        /// Event handler for when a button gets focus. Set focus back to the
        /// text box
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void button_GotFocus(object sender, EventArgs e)
        {
            TextBoxTalkWindow.Focus();
        }

        /// <summary>
        /// Creates and shows the specified scanner
        /// </summary>
        /// <param name="panelClass">type of the scanner</param>
        /// <param name="form">scanner form</param>
        /// <returns>true on success</returns>
        private bool createAndShowScanner(String panelClass, Form form)
        {
            bool retVal = true;

            form.Invoke(new MethodInvoker(delegate
            {
                IPanel panel = Context.AppPanelManager.CreatePanel(panelClass) as IPanel;

                if (panel != null)
                {
                    _disableAllButtons = true;
                    _dimScanner = false;
                    IScannerPanel scannerPanel = panel as IScannerPanel;
                    scannerPanel.ScannerCommon.PositionSizeController.DockScanner(form.Handle, Windows.WindowPosition.TopRight);
                    Context.AppPanelManager.Show(form as IPanel, panel);
                }
                else
                {
                    retVal = false;
                }
            }));

            return retVal;
        }

        /// <summary>
        /// Disables all buttons on the scanner
        /// </summary>
        private void disableAllButtons()
        {
            var list = new List<Widget>();
            _alphabetScannerCommon.PanelCommon.RootWidget.Finder.FindAllButtons(list);
            foreach (var widget in list.Where(widget => !(widget is WordListItemWidget)))
            {
                widget.Enabled = false;
            }
        }

        /// <summary>
        /// Enables all buttons on scanner
        /// </summary>
        private void enableAllButtons()
        {
            _disableAllButtons = false;

            var list = new List<Widget>();
            _alphabetScannerCommon.PanelCommon.RootWidget.Finder.FindAllButtons(list);
            foreach (var widget in list.Where(widget => !(widget is WordListItemWidget)))
            {
                widget.Enabled = true;
            }
        }

        /// <summary>
        /// Makes sure the scanner stays focused
        /// </summary>
        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }

            _pauseWatchdog = false;
        }

        /// <summary>
        /// Returns the previous para of text from where the cursor is
        /// </summary>
        /// <returns>text of previous para</returns>
        private String getPreviousPara()
        {
            int index = TextBoxTalkWindow.SelectionStart;
            var text = TextBoxTalkWindow.Text;

            if (text.Length == 0)
            {
                return String.Empty;
            }

            if (index >= text.Length)
            {
                index = text.Length - 1;
            }

            while (index > 0 && (text[index] == '\r' || text[index] == '\n'))
            {
                index--;
            }

            int endPos = index;

            while (index > 0 && text[index] != '\r' && text[index] != '\n')
            {
                index--;
            }

            if (index > 0 && (text[index] == '\r' || text[index] == '\n'))
            {
                index++;
            }

            int startPos = index;

            return text.Substring(startPos, endPos - startPos + 1);
        }

        /// <summary>
        /// Launches the agent to speak phrases
        /// </summary>
        private async void handlePhraseSpeak()
        {
            var agent = Context.AppAgentMgr.GetAgentByCategory("PhraseSpeakAgent");

            if (agent != null)
            {
                var invoker = agent.GetInvoker();
                invoker.SetValue("EnableSearch", false);
                await Context.AppAgentMgr.ActivateAgent(agent as IFunctionalAgent);
            }
        }

        /// <summary>
        /// Removes all the watchdogs
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
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            var ctrls = GetAll(this, typeof(Button));
            foreach (var button in ctrls.Select(control => control as Button))
            {
                button.GotFocus += button_GotFocus;
            }

            Load += TalkApplicationScanner_Load;
            Shown += TalkApplicationScannerQwerty_Shown;
            FormClosing += TalkApplicationScanner_FormClosing;
            TextBoxTalkWindow.KeyPress += TextBoxTalkWindowOnKeyPress;
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void TalkApplicationScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            removeWatchdogs();

            _alphabetScannerCommon.OnClosing(sender, e);
        }

        /// <summary>
        /// The form has loaded.  Perform initialization.
        /// </summary>
        private void TalkApplicationScanner_Load(object sender, EventArgs e)
        {
            var icon = ImageUtils.GetEntryAssemblyIcon();
            if (icon != null)
            {
                Icon = icon;
            }

            TextBoxTalkWindow.Focus();

            WordPredictionManager.Instance.ActiveWordPredictor.PredictionWordCount = 9;

            _alphabetScannerCommon.OnLoad(sender, e);

            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.TalkWindowSchemeName);
            TextBoxTalkWindow.BackColor = colorScheme.Background;
            TextBoxTalkWindow.ForeColor = colorScheme.Foreground;

            _alphabetScannerCommon.ResizeToFitDesktop(this);

            enableWatchdogs();
        }

        /// <summary>
        /// Event handler for when form is shown
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TalkApplicationScannerQwerty_Shown(object sender, EventArgs e)
        {
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Key press event for the text box.  If user hit enter,
        /// convert text to speech
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="keyPressEventArgs">event args</param>
        private void TextBoxTalkWindowOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            try
            {
                if (keyPressEventArgs.KeyChar == '\r')
                {
                    var para = getPreviousPara();

                    if (String.IsNullOrEmpty(TextBoxTalkWindow.Text.Trim()))
                    {
                        return;
                    }

                    String textToSpeak;

                    using (var context = Context.AppAgentMgr.ActiveContext())
                    {
                        context.TextAgent().GetParagraphAtCaret(out textToSpeak);
                    }

                    if (String.IsNullOrEmpty(textToSpeak) && !String.IsNullOrEmpty(para))
                    {
                        keyPressEventArgs.Handled = true;
                        textToSpeech(para);
                    }
                    else
                    {
                        ttsAndLearn(textToSpeak);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Converts the specified text to speech
        /// </summary>
        /// <param name="text">text to convert</param>
        private void textToSpeech(String text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                Log.Debug("*** TTS *** : " + text);
                TTSManager.Instance.ActiveEngine.Speak(text);
                Log.Debug("*** TTS *** : sent text!");

                AuditLog.Audit(new AuditEventTextToSpeech(TTSManager.Instance.ActiveEngine.Descriptor.Name));
            }
        }

        /// <summary>
        /// Converts the current para to speech and notify app about this
        /// </summary>
        /// <param name="text">text to convert</param>
        private void ttsAndLearn(String text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                textToSpeech(text);

                if (WordPredictionManager.Instance.ActiveWordPredictor.SupportsLearning)
                {
                    WordPredictionManager.Instance.ActiveWordPredictor.Learn(text);
                }
            }
        }

        /// <summary>
        /// Handles yes/no command, sets the choice and then
        /// closes the scanner
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

                var form = Dispatcher.Scanner.Form as TalkApplicationScannerQwerty;

                switch (Command)
                {
                    case "CmdAutocompleteWithFirstWord":
                        form.ScannerCommon.AnimationManager.Interrupt();
                        form._alphabetScannerCommon.AutocompleteWithFirstWord();
                        break;

                    case "CmdTalkWindowClear":
                        if (DialogUtils.ConfirmScanner(R.GetString("ClearTalkWindow")))
                        {
                            Windows.SetText(form.TextBoxTalkWindow, String.Empty);
                        }
                        break;

                    case "CmdNumberScanner":
                        form.createAndShowScanner(PanelClasses.Number, form);
                        break;

                    case "CmdPhraseSpeak":
                        form._pauseWatchdog = true;
                        form.handlePhraseSpeak();
                        break;

                    case "ExitApp":
                        if (DialogUtils.ConfirmScanner(R.GetString("CloseQ")))
                        {
                            Windows.CloseForm(form);
                        }

                        break;

                    default:
                        form._alphabetScannerCommon.Dispatcher.Dispatch(Command, ref handled);
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
                Commands.Add(new CommandHandler("CmdTalkWindowClear"));
                Commands.Add(new CommandHandler("CmdNumberScanner"));
                Commands.Add(new CommandHandler("ExitApp"));
                Commands.Add(new CommandHandler("CmdPhraseSpeak"));
                Commands.Add(new CommandHandler("CmdAutocompleteWithFirstWord"));
            }
        }
    }
}