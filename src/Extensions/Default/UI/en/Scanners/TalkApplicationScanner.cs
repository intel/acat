////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// TalkApplicationScanner.cs
//
// The main form for the ACAT Talk application. This is a container for
// user controls for word prediction, sentence prediction, talk text box
// and the keyboard.
// It also handles commands associated with keys such as Undo, Backspace,
// text navigation etc.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
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
    [DescriptorAttribute("D9A5B53F-7119-445B-BDEA-F76EC53077F1",
                        "TalkApplicationScanner",
                        "Talk application main window")]
    public partial class TalkApplicationScanner : Form, IScannerPanel, ISupportsStatusBar
    {
        private const Int32 HTCAPTION = 0x00000002;

        private const Int32 WM_NCLBUTTONDOWN = 0xA1;

        /// <summary>
        /// The command dispatcher object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// The AlphabetScannerCommon object. Has a number of
        /// helper functions
        /// </summary>
        private readonly ScannerCommon2 _scannerCommon;

        /// <summary>
        /// Should the scanner be dimmed
        /// </summary>
        private bool _dimScanner;

        private String _panelClass;

        //private UserControl _prevUserControl = null;

        /// <summary>
        /// The ScannerHelper object
        /// </summary>
        private ScannerHelper _scannerHelper;

        private TalkWindowTextBoxPhraseModeUserControl _textBoxPhraseModeUserControl;

        private TextBox _textBoxTalkWindow;

        private TalkWindowTextBoxUserControl _textBoxUserControl;

        /// <summary>
        /// Ensures that the window stays focused
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TalkApplicationScanner()
        {
            _scannerCommon = new ScannerCommon2(this);

            InitializeComponent();

            subscribeToEvents();

            _dimScanner = true;

            CoreGlobals.LnRMode = false;
            CoreGlobals.SwitchLnR = false;

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
            get { return _panelClass; }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon
        { get { return _scannerCommon; } }

        public ScannerCommon ScannerCommon
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the scanner common object
        /// </summary>
        public ScannerCommon2 ScannerCommon2
        {
            get { return _scannerCommon; }
        }

        /// <summary>
        /// Gets the status bar control for this scanner
        /// </summary>
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon2.StatusBar; }
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
        /// Set the form style
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                base.CreateParams.ExStyle |= Windows.WindowStyleFlags.WS_EX_COMPOSITED;
                return base.CreateParams;
            }
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
                case "CmdEditScanner":
                case "CmdTalkWindowClear":
                case "CmdBackSpace":
                case "CmdDelPrevWord":
                case "CmdSmartDeletePrevWord":
                    arg.Handled = true;
                    arg.Enabled = _textBoxTalkWindow != null && _textBoxTalkWindow.Text.Length != 0;
                    break;

                case "CmdSpeak":
                    arg.Handled = true;
                    arg.Enabled = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases &&
                                    _textBoxTalkWindow != null && _textBoxTalkWindow.Text.Length != 0;
                    break;

                case "CmdSaveToCanned":
                    arg.Handled = true;
                    var mode = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode();
                    arg.Enabled = mode != WordPredictionModes.None && mode != WordPredictionModes.CannedPhrases &&
                        _textBoxTalkWindow != null && _textBoxTalkWindow.Text.Length != 0;
                    break;

                case "CmdEnterKey":
                    arg.Handled = true;
                    arg.Enabled = _textBoxTalkWindow != null && _textBoxTalkWindow.Multiline;
                    break;

                default:
                    _scannerHelper.CheckCommandEnabled(arg);
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
            _panelClass = startupArg.PanelClass;

            _scannerHelper = new ScannerHelper(this, startupArg);

            bool retVal = _scannerCommon.Initialize(startupArg);

            if (retVal)
            {
                _scannerCommon.SetStatusBar(statusStrip);
            }

            ControlBox = true;

            _scannerCommon.UserControlManager.GridScanIterations = Common.AppPreferences.GridScanIterations;

            _scannerCommon.UserControlManager.AddUserControlByKeyOrName(panelWordPrediction, "wordPrediction", "WordPredictionUserControl");

            _scannerCommon.UserControlManager.AddUserControlByKeyOrName(panelSentencePrediction, "sentencePrediction", "SentencePredictionUserControl");

            _scannerCommon.UserControlManager.AddUserControlByKeyOrName(panelKeyboard, "keyboard", "KeyboardQwertyUserControl");

            _textBoxUserControl = new TalkWindowTextBoxUserControl(this, panelTextBox);
            _textBoxPhraseModeUserControl = new TalkWindowTextBoxPhraseModeUserControl(this, panelTextBox);

            addTextBoxUserControl(_textBoxUserControl);

            List<IUserControl> list = new List<IUserControl>();

            UserControlManager.FindAllUserControls(this, list);

            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged += ActiveWordPredictor_EvtModeChanged;

            return retVal;
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        public void OnFocusChanged(WindowActivityMonitorInfo monitorInfo)
        {
            _scannerCommon.OnFocusChanged(monitorInfo);
        }

        /// <summary>
        /// Pauses animations
        /// </summary>
        public void OnPause()
        {
            Log.Debug("CALIBTEST TalkScanner OnPause. Pausing watchdog");
            _windowActiveWatchdog?.Pause();

            Log.Debug("CALIBTEST calling usercontrolmanager.pause");
            _scannerCommon.UserControlManager.OnPause();

            Log.Debug("CALIBTEST calling scannercommon2.pause");
            _scannerCommon.OnPause(_dimScanner ?
                                ScannerCommon2.PauseDisplayMode.FadeScanner :
                                ScannerCommon2.PauseDisplayMode.None);

            if (panelTextBox.Controls.Count > 0)
            {
                ITalkWindowTextBox tb = panelTextBox.Controls[0] as ITalkWindowTextBox;
                tb.OnPause();
            }
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
            if (panelTextBox.Controls.Count > 0)
            {
                ITalkWindowTextBox tb = panelTextBox.Controls[0] as ITalkWindowTextBox;
                tb.OnResume();
            }

            Log.Debug("CALIBTEST TalkScanner OnResume. Resuming watchdog");
            _windowActiveWatchdog?.Resume();

            _dimScanner = true;

            Log.Debug("CALIBTEST TalkScanner OnResume. calling user control manager.OnREsume");
            _scannerCommon.UserControlManager.OnResume();

            Log.Debug("CALIBTEST TalkScanner OnResume. calling scannercommon2 resume");
            _scannerCommon.OnResume();

            _scannerCommon.ResizeToFitDesktop(this);
        }

        /// <summary>
        /// Triggered when the user actuates a widget
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled?</param>
        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            //_alphabetScannerCommon.OnWidgetActuated(e, ref handled);
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
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="e">closing param</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _scannerCommon.OnFormClosing(e);
            base.OnFormClosing(e);
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            if (m.Msg == WM_NCLBUTTONDOWN) //cancels the drag this is IMP
            {
                if (m.WParam.ToInt32() == HTCAPTION) return;
            }
            else if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xfff0;
                if (command == SC_MOVE)
                {
                    base.WndProc(ref m);
                    return;
                }
            }

            if (!_scannerCommon.HandleWndProc(m))
            {
                base.WndProc(ref m);
            }
        }

        private void ActiveWordPredictor_EvtModeChanged(WordPredictionModes newMode)
        {
            setModeLabel(newMode);
            Invoke(new MethodInvoker(delegate
            {
                if (newMode == WordPredictionModes.CannedPhrases)
                {
                    addTextBoxUserControl(_textBoxPhraseModeUserControl);
                }
                else
                {
                    addTextBoxUserControl(_textBoxUserControl);
                }
            }));
            
            if (Common.AppPreferences.ClearTalkWindowOnTypeModeChange)
            {
                Windows.SetText(_textBoxTalkWindow, String.Empty);
            }
        }

        private void addTextBoxUserControl(UserControl userControl)
        {
            if (panelTextBox.Controls.Count > 0 && panelTextBox.Controls[0] is ITalkWindowTextBox)
            {
                var textBox = (panelTextBox.Controls[0] as ITalkWindowTextBox).TextBoxControl;
                if (textBox != null)
                {
                    textBox.KeyPress -= TextBoxTalkWindowOnKeyPress;
                }

                panelTextBox.Controls.Clear();
            }

            panelTextBox.Controls.Add(userControl);
            userControl.Dock = DockStyle.Fill;
            userControl.TabStop = true;
            userControl.TabIndex = 0;
            if (userControl is ITalkWindowTextBox)
            {
                _textBoxTalkWindow = (userControl as ITalkWindowTextBox).TextBoxControl;
                if (_textBoxTalkWindow != null)
                {
                    setColorScheme();

                    //_textBoxTalkWindow.TabIndex = 0;
                    _textBoxTalkWindow.KeyPress += TextBoxTalkWindowOnKeyPress;
                    //_textBoxTalkWindow.Focus();
                }
            }
        }

        /// <summary>
        /// Window procedure
        /// </summary>
        /// <param name="m">windows message</param>
        /// <summary>
        /// Returns the previous para of text from where the cursor is
        /// </summary>
        /// <returns>text of previous para</returns>
        private String getPreviousPara()
        {
            int index = _textBoxTalkWindow.SelectionStart;
            var text = _textBoxTalkWindow.Text;

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

        private void LearnCannedPhrases()
        {
            var text = _textBoxTalkWindow.Text;
            if (String.IsNullOrEmpty(_textBoxTalkWindow.Text.Trim()))
            {
                return;
            }
            String textToLearn = String.Empty;
            using (var context = Context.AppAgentMgr.ActiveContext())
            {
                int caretPos = context.TextAgent().GetCaretPos();
                var start = TextUtils.GetStartIndexCurrOrPrevSentence(text, caretPos);
                int end = -1;
                if (start >= 0)
                {
                    end = TextUtils.GetIndexNextSentence(text, start);
                }

                if (start >= 0 && end >= 0 && (end - start) > 0)
                {
                    textToLearn = text.Substring(start, end - start);
                }
            }
            if (!String.IsNullOrEmpty(textToLearn))
            {
                var shortenedText = TextUtils.EmbedEllipses(textToLearn, 60);
                var prompt = "Save this text to Canned Phrases?\r\n\r\n" + shortenedText;

                if (DialogUtils.ConfirmScanner(this, prompt))
                {
                    WordPredictionManager.Instance.ActiveWordPredictor.Learn(textToLearn, WordPredictorMessageTypes.LearnCanned);
                }
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

        private void setColorScheme()
        {
            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.TalkWindowSchemeName);
            _textBoxTalkWindow.BackColor = colorScheme.Background;
            _textBoxTalkWindow.ForeColor = colorScheme.Foreground;
            panelTextBox.BackColor = colorScheme.Background;
        }

        private void setModeLabel(WordPredictionModes mode)
        {
            Invoke(new MethodInvoker(delegate
            {
                String modeStr = String.Empty; ;
                switch (mode)
                {
                    case WordPredictionModes.Sentence:
                        modeStr = "Sentence";
                        break;

                    case WordPredictionModes.CannedPhrases:
                        modeStr = "Canned Phrase";
                        break;

                    case WordPredictionModes.Shorthand:
                        modeStr = "Shorthand";
                        break;
                }

                labelCurrentTypingMode.Text = "Mode: " + modeStr;
            }));
        }

        private void speak()
        {
            if (String.IsNullOrEmpty(_textBoxTalkWindow.Text.Trim()))
            {
                return;
            }

            /*
            using (var context = Context.AppAgentMgr.ActiveContext())
            {
                context.TextAgent().GetParagraphAtCaret(out textToSpeak);
            }
            */

            String textToSpeak = getPreviousPara();
            ttsAndLearn(textToSpeak);
        }

        /// <summary>
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            Load += TalkApplicationScanner_Load;
            Shown += TalkApplicationScanner_Shown;
            FormClosing += TalkApplicationScanner_FormClosing;
        }

        /// <summary>
        /// Release resources and stop threads/timers
        /// </summary>
        private void TalkApplicationScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            removeWatchdogs();

            _scannerCommon.OnClosing();
            _scannerCommon.Dispose();
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

            _textBoxTalkWindow.Focus();

            WordPredictionManager.Instance.ActiveWordPredictor.PredictionWordCount = 10;

            _scannerCommon.OnLoad();

            setColorScheme();

            _scannerCommon.ResizeToFitDesktop(this);

            _windowActiveWatchdog = new WindowActiveWatchdog(this);
        }

        /// <summary>
        /// Event handler for when form is shown
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TalkApplicationScanner_Shown(object sender, EventArgs e)
        {
            setModeLabel(Context.AppWordPredictionManager.ActiveWordPredictor.GetMode());

            ScannerFocus.SetFocus(this);
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
                if (Common.AppPreferences.SpeakOnEnterKey && keyPressEventArgs.KeyChar == '\r')
                {
                    if (String.IsNullOrEmpty(_textBoxTalkWindow.Text.Trim()))
                    {
                        return;
                    }

                    var para = getPreviousPara();

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

                Log.Debug("tts " + text);

                if (WordPredictionManager.Instance.ActiveWordPredictor.SupportsLearning)
                {
                    switch (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode())
                    {
                        case WordPredictionModes.Sentence:
                            WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnWords);
                            break;

                        case WordPredictionModes.Shorthand:
                            WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnShorthand);
                            break;

                        case WordPredictionModes.CannedPhrases:
                            WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnCanned);
                            break;
                    }
                    Log.Debug("Learn " + text);
                    WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnSentence);
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

            public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as TalkApplicationScanner;

                switch (Command)
                {
                    case "CmdAutocompleteWithFirstWord":
                        //form.ScannerCommon.AnimationManager.Interrupt();
                        //form._alphabetScannerCommon.AutocompleteWithFirstWord();
                        break;

                    case "CmdTalkWindowClear":

                        if (DialogUtils.ConfirmScanner(form, R.GetString("ClearTalkWindow")))
                        {
                            Windows.SetText(form._textBoxTalkWindow, String.Empty);
                        }
                        /*
                        if (form.panelTextBox.Controls.Count > 0)
                        {
                            form._prevUserControl = form.panelTextBox.Controls[0] as UserControl;
                        }

                        form.addTextBoxUserControl(form._screenLockTextBoxUserControl);
                        */
                        break;

                    case "CmdNumberScanner":
                        //form._scannerCommon.UserControlManager.AddUserControlByKeyOrName(form.panelKeyboard, "numeric", "KeyboardNumberUserControl");
                        form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.panelKeyboard, "numeric", "KeyboardNumberUserControl");
                        form._scannerCommon.UserControlManager.StartTopLevelAnimation();
                        break;

                    case "CmdEditScanner":
                        form._scannerCommon.UserControlManager.PushUserControlByKeyOrName(form.panelKeyboard, "edit", "KeyboardEditUserControl");
                        form._scannerCommon.UserControlManager.StartTopLevelAnimation();
                        break;

                    case "CmdPhraseSpeak":
                        //form._pauseWatchdog = true;
                        //form.handlePhraseSpeak();
                        break;

                    case "ExitApp":
                        if (DialogUtils.ConfirmScanner(R.GetString("CloseQ")))
                        {
                            Windows.CloseForm(form);
                        }

                        break;

                    case "CmdMainKeyboard":
                        form._scannerCommon.UserControlManager.AddUserControlByKeyOrName(form.panelKeyboard, "keyboard", "KeyboardQwertyUserControl");
                        form._scannerCommon.UserControlManager.StartTopLevelAnimation();
                        break;

                    case "CmdMainMenu":
                        form._dispatcher.DefaultDispatch(Command, ref handled);

                        break;

                    case "CmdSpeak":
                        form.speak();
                        break;

                    case "CmdEntryModeSelect":
                        {
                            var panel = Context.AppPanelManager.CreatePanel("WordPredictionSetModeScanner", "ACAT");
                            if (panel is IPanel)
                            {
                                Context.AppPanelManager.ShowDialog(form as IPanel, panel as IPanel);
                            }
                        }
                        break;

                    case "CmdSaveToCanned":
                        if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)
                        {
                            form.LearnCannedPhrases();
                        }
                        break;

                    case "CmdYesNoResponse":
                        {
                            var panel = Context.AppPanelManager.CreatePanel("YesNoResponseScanner", "Select Response");
                            if (panel is IPanel)
                            {
                                Context.AppPanelManager.ShowDialog(form as IPanel, panel as IPanel);
                            }
                        }
                        break;

                    case "CmdToggleLnR":
                        if (DialogUtils.ConfirmScanner(form, "Turn L&&R mode on?"))
                        {
                            CoreGlobals.SwitchLnR = true;
                            Windows.CloseForm(form);
                        }
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">true if it was handled</param>
            /// <returns>true on success</returns>
            public override bool Execute2(object source, ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as TalkApplicationScanner;

                switch (Command)
                {
                    case "CmdGoBack":
                        if (source is UserControl)
                        {
                            var userControl = source as UserControl;
                            form._scannerCommon.UserControlManager.PopUserControl(userControl.Parent);//form.panelKeyboard);
                            form._scannerCommon.UserControlManager.StartTopLevelAnimation();
                        }
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
                Commands.Add(new CommandHandler("CmdEditScanner"));
                Commands.Add(new CommandHandler("ExitApp"));
                Commands.Add(new CommandHandler("CmdPhraseSpeak"));
                Commands.Add(new CommandHandler("CmdAutocompleteWithFirstWord"));
                Commands.Add(new CommandHandler("CmdMainKeyboard"));
                Commands.Add(new CommandHandler("CmdGoBack"));
                Commands.Add(new CommandHandler("CmdMainMenu"));
                Commands.Add(new CommandHandler("CmdSpeak"));
                Commands.Add(new CommandHandler("CmdEntryModeSelect"));
                Commands.Add(new CommandHandler("CmdSaveToCanned"));
                Commands.Add(new CommandHandler("CmdYesNoResponse"));
                Commands.Add(new CommandHandler("CmdToggleLnR"));
            }
        }
    }
}