using ACAT.Core.AgentManagement;
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.WordPredictorManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extension;
using ACAT.Extension.CommandHandlers;
using ACAT.Extension.UI;
using ACAT.Extension.UI.ScannerForms;
using ACATResources;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Scanners
{
    [ClassDescriptor("D9A5B53F-7119-445B-BDEA-F76EC53077F1",
                        "TalkApplicationScanner",
                        "Talk application main window")]
    /*
    public partial class TalkApplicationScanner : Form
    /*/
    public partial class TalkApplicationScanner : GenericScannerForm, ISupportsStatusBar
    //*/
    {
        private readonly ILogger<TalkApplicationScanner> _logger;
        private TalkWindowTextBoxPhraseModeUserControl _textBoxPhraseModeUserControl;
        private TextBox _textBoxTalkWindow;
        private TalkWindowTextBoxUserControl _textBoxUserControl;

        /// <summary>
        /// Optional callback invoked with the elapsed key-press handling time in milliseconds
        /// each time <see cref="TextBoxTalkWindowOnKeyPress"/> completes.
        /// Set from the application layer (e.g. Program.cs) to forward data to PerformanceMonitor.
        /// </summary>
        public static Action<double> OnUiKeyPressLatencyMs;
        public TalkApplicationScanner() : base()
        {
            _logger = LoggingConfiguration.CreateLogger<TalkApplicationScanner>();
            _dispatcher = new TalkAppDispatcher(this);
        }

        public override DefaultCommandDispatcher _dispatcher { get; }
        public override RunCommandDispatcher CommandDispatcher => _dispatcher;
        public ScannerStatusBar ScannerStatusBar
        {
            get { return ScannerCommon.StatusBar; }
        }

        public override ITextController TextController => ScannerCommon.TextController;

        public override bool CheckCommandEnabled(CommandEnabledArg arg)
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
                    WordPredictionModes mode = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode();
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

        public override bool HandleInitialize(StartupArg startupArg)
        {
            ScannerCommon.UserControlManager.GridScanIterations = Common.AppPreferences.GridScanIterations;
            ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelWordPrediction, "wordPrediction", "WordPredictionUserControl");
            ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelSentencePrediction, "sentencePrediction", "SentencePredictionUserControl");
            ScannerCommon.UserControlManager.AddUserControlByKeyOrName(panelKeyboard, "keyboard", "KeyboardQwertyUserControl");
            _textBoxUserControl = new TalkWindowTextBoxUserControl(this, panelTextBox);
            _textBoxPhraseModeUserControl = new TalkWindowTextBoxPhraseModeUserControl(this, panelTextBox);

            addTextBoxUserControl(_textBoxUserControl);

            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged += ActiveWordPredictor_EvtModeChanged;

            if (startupArg.PredictionMode != WordPredictionModes.None)
            {
                Context.AppWordPredictionManager.ActiveWordPredictor.SetMode(startupArg.PredictionMode);
            }

            return true;
        }

        protected override void ScannerFormClosing(Object sender, FormClosingEventArgs e)
        {
            ScannerCommon.OnFormClosing(e);
            if (e.Cancel)
            {
                return;
            }
            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged -= ActiveWordPredictor_EvtModeChanged;
            WordPredictionManager.Instance.ActiveWordPredictor.PredictionWordCount = 0;
        }

        protected override void HandlePause()
        {
            if (panelTextBox.Controls.Count > 0)
            {
                ITalkWindowTextBox tb = panelTextBox.Controls[0] as ITalkWindowTextBox;
                tb.OnPause();
            }
        }

        protected override void HandleResume()
        {
            if (panelTextBox.Controls.Count > 0)
            {
                ITalkWindowTextBox tb = panelTextBox.Controls[0] as ITalkWindowTextBox;
                tb.OnResume();
            }
        }

        protected override void ScannerFormLoaded(object sender, EventArgs e)
        {
            Icon icon = ImageUtils.GetEntryAssemblyIcon();
            if (icon != null)
            {
                Icon = icon;
            }

            _textBoxTalkWindow.Focus();

            WordPredictionManager.Instance.ActiveWordPredictor.PredictionWordCount = 10;

            ScannerCommon.OnLoad();

            SetColorScheme();

            ScannerCommon.ResizeToFitDesktop(this);

        }

        protected override void ScannerShown(object sender, EventArgs e)
        {
            setModeLabel(Context.AppWordPredictionManager.ActiveWordPredictor.GetMode());

        }

        protected override void SubscribeToEvents()
        {
            Load += ScannerFormLoaded;
            Shown += ScannerShown;
            FormClosing += ScannerFormClosing;
        }

        private void ActiveWordPredictor_EvtModeChanged(WordPredictionModes newMode)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => ActiveWordPredictor_EvtModeChanged(newMode)));
                return;
            }

            setModeLabel(newMode);

            if (newMode == WordPredictionModes.CannedPhrases)
            {
                addTextBoxUserControl(_textBoxPhraseModeUserControl);
            }
            else
            {
                addTextBoxUserControl(_textBoxUserControl);
            }

            if (Common.AppPreferences.ClearTalkWindowOnTypeModeChange)
            {
                Windows.SetText(_textBoxTalkWindow, String.Empty);
            }
        }

        private void addTextBoxUserControl(UserControl userControl)
        {
            if (panelTextBox.Controls.Count > 0 && panelTextBox.Controls[0] is ITalkWindowTextBox)
            {
                TextBox textBox = (panelTextBox.Controls[0] as ITalkWindowTextBox).TextBoxControl;
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
                    SetColorScheme();
                    _textBoxTalkWindow.TextAlign  = HorizontalAlignment.Center;
                    //_textBoxTalkWindow.TabIndex = 0;
                    _textBoxTalkWindow.KeyPress += TextBoxTalkWindowOnKeyPress;
                    //_textBoxTalkWindow.Focus();
                }
            }
        }

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
            using (AgentContext context = Context.AppAgentMgr.ActiveContext())
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
        private void setModeLabel(WordPredictionModes mode)
        {
            if (this.IsHandleCreated == false)
            {
                return;
            }

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

            String textToSpeak = getPreviousPara();
            ttsAndLearn(textToSpeak);
        }
        private void TextBoxTalkWindowOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
        {
            var swUi = Stopwatch.StartNew();
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

                    using (AgentContext context = Context.AppAgentMgr.ActiveContext())
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
                _logger.LogError(ex, "Error in speak");
            }
            finally
            {
                swUi.Stop();
                OnUiKeyPressLatencyMs?.Invoke(swUi.Elapsed.TotalMilliseconds);
            }
        }

        private void textToSpeech(String text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                _logger.LogDebug("*** TTS *** : {Text}", text);
                TTSManager.Instance.ActiveEngine.Speak(text);
                _logger.LogDebug("*** TTS *** : sent text!");

                AuditLog.Audit(new AuditEventTextToSpeech(TTSManager.Instance.ActiveEngine.Descriptor.Name));
            }
        }

        private void ttsAndLearn(String text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                textToSpeech(text);

                _logger.LogDebug("tts {Text}", text);

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
                    _logger.LogDebug("Learn {Text}", text);
                    WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnSentence);
                }
            }
        }

        [EnvironmentPermission(SecurityAction.LinkDemand, Unrestricted = true)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == User32Interop.Win32Constants.WM_NCLBUTTONDOWN) //cancels the drag this is IMP
            {
                if (m.WParam.ToInt32() == User32Interop.Win32Constants.HTCAPTION) return;
            }
            else if (m.Msg == User32Interop.Win32Constants.WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xfff0;
                if (command == User32Interop.Win32Constants.SC_MOVE)
                {
                    base.WndProc(ref m);
                    return;
                }
            }

            if (!ScannerCommon.HandleWndProc(m))
            {
                base.WndProc(ref m);
            }
        }

        private class TalkApplicationCommandHandler : RunCommandHandler
        {
            public TalkApplicationCommandHandler(String cmd) : base(cmd) { }

            public override bool Execute(ref bool handled)
            {
                var form = Dispatcher.Scanner.Form as TalkApplicationScanner;
                form._logger.LogInformation("Inside Talk Application Scanner TalkApplicationCommandHandler for command: {Command}", Command);

                handled = true;

                switch (Command)
                {
                    case "CmdAutocompleteWithFirstWord":
                        break;

                    case "CmdTalkWindowClear":

                        if (DialogUtils.ConfirmScanner(form, StringResources.ClearTalkWindow))
                        {
                            Windows.SetText(form._textBoxTalkWindow, String.Empty);
                        }
                        break;

                    case "CmdNumberScanner":
                        form.ScannerCommon.UserControlManager.PushUserControlByKeyOrName(form.panelKeyboard, "numeric", "KeyboardNumberUserControl");
                        form.ScannerCommon.UserControlManager.StartTopLevelAnimation();
                        break;

                    case "CmdEditScanner":
                        form.ScannerCommon.UserControlManager.PushUserControlByKeyOrName(form.panelKeyboard, "edit", "KeyboardEditUserControl");
                        form.ScannerCommon.UserControlManager.StartTopLevelAnimation();
                        break;

                    case "CmdPhraseSpeak":
                        //form._pauseWatchdog = true;
                        //form.handlePhraseSpeak();
                        break;

                    case "ExitApp":
                        if (DialogUtils.ConfirmScanner(StringResources.Close))
                        {
                            Windows.CloseForm(form);
                        }

                        break;

                    case "CmdMainKeyboard":
                        form.ScannerCommon.UserControlManager.AddUserControlByKeyOrName(form.panelKeyboard, "keyboard", "KeyboardQwertyUserControl");
                        form.ScannerCommon.UserControlManager.StartTopLevelAnimation();
                        break;

                    case "CmdMainMenu":
                        form._dispatcher.DispatchCommand(Command, ref handled);

                        break;

                    case "CmdSpeak":
                        form.speak();
                        break;

                    case "CmdEntryModeSelect":
                        {
                            Form panel = Context.AppPanelManager.CreatePanel("WordPredictionSetModeScanner", "ACAT");
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
                            Form panel = Context.AppPanelManager.CreatePanel("YesNoResponseScanner", "Select Response");
                            if (panel is IPanel)
                            {
                                Context.AppPanelManager.ShowDialog(form as IPanel, panel as IPanel);
                            }
                        }
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }

            public override bool Execute(ref bool handled, object source = null)
            {
                Console.WriteLine("Inside Talk Application Scanner TalkApplicationCommandHandler for command: " + Command);

                handled = true;

                var form = Dispatcher.Scanner.Form as TalkApplicationScanner;

                switch (Command)
                {
                    case "CmdGoBack":
                        if (source is UserControl)
                        {
                            var userControl = source as UserControl;
                            form.ScannerCommon.UserControlManager.PopUserControl(userControl.Parent);//form.panelKeyboard);
                            form.ScannerCommon.UserControlManager.StartTopLevelAnimation();
                        }
                        break;
                }

                return true;
            }
        }

        private class TalkAppDispatcher : DefaultCommandDispatcher
        {
            public TalkAppDispatcher(IScannerPanel panel) : base(panel)
            {
                Commands.Add(new TalkApplicationCommandHandler("CmdTalkWindowClear"));
                Commands.Add(new TalkApplicationCommandHandler("CmdNumberScanner"));
                Commands.Add(new TalkApplicationCommandHandler("CmdEditScanner"));
                Commands.Add(new TalkApplicationCommandHandler("ExitApp"));
                Commands.Add(new TalkApplicationCommandHandler("CmdPhraseSpeak"));
                Commands.Add(new TalkApplicationCommandHandler("CmdAutocompleteWithFirstWord"));
                Commands.Add(new TalkApplicationCommandHandler("CmdMainKeyboard"));
                Commands.Add(new TalkApplicationCommandHandler("CmdGoBack"));
                Commands.Add(new TalkApplicationCommandHandler("CmdMainMenu"));
                Commands.Add(new TalkApplicationCommandHandler("CmdSpeak"));
                Commands.Add(new TalkApplicationCommandHandler("CmdEntryModeSelect"));
                Commands.Add(new TalkApplicationCommandHandler("CmdSaveToCanned"));
                Commands.Add(new TalkApplicationCommandHandler("CmdYesNoResponse"));
            }
        }
    }
}