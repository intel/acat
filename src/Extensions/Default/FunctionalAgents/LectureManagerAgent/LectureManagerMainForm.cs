////////////////////////////////////////////////////////////////////////////
// <copyright file="LectureManagerMainForm.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Task = System.Threading.Tasks.Task;
using Windows = ACAT.Lib.Core.Utility.Windows;

namespace ACAT.Extensions.Default.FunctionalAgents.LectureManager
{
    /// <summary>
    /// Main form for the lecture manager. Displays the text
    /// of the lecture.  User can navigate back and forth either
    /// by sentence or by para.  User can also speak the text,
    /// again by sentence, para or the whole text in one shot.
    /// User can pause the lecture at any time.  Text for the
    /// lecture can either be specified as a property or it
    /// can be specified through a text or a word document.
    /// Also allows the user to pace the lecture, pause and
    /// resume etc.
    /// The form itself resizes itself to fit the display and
    /// automatically adjusts all the controls (buttons). This
    /// is to provide the maximum real estate to the user so
    /// she can view the text.
    /// </summary>
    [Descriptor("1E19F5C8-548B-4B9F-AF5F-984DA001C3C3",
                "LectureManagerMainForm",
                "Lecture Manager Main Form")]
    public partial class LectureManagerMainForm : IExtension, IPanel
    {
        /// <summary>
        /// Pause time between paras in the speak all mode
        /// </summary>
        public const int LectureManagerSpeakAllParagraphPause = 4000;

        /// <summary>
        /// Windows constant
        /// </summary>
        private const int EM_LINESCROLL = 0x00B6;

        /// <summary>
        /// How many lines to scroll by at a time
        /// </summary>
        private const int ScrollNumberOfLines = 4;

        /// <summary>
        /// If the number of lines from the bottom of the window is less than
        /// this constant, scroll the window by ScrollNumberOfLines lines
        /// </summary>
        private const int ScrollThresholdLines = 6;

        /// <summary>
        /// Windows constant
        /// </summary>
        private const int WS_SYSMENU = 0x80000;

        /// <summary>
        /// The extension invoker object
        /// </summary>
        private readonly ExtensionInvoker _invoker;

        /// <summary>
        /// Length of time to wait between paras when
        /// speaking the text in its entirety
        /// </summary>
        private readonly Timer _speakAllParagraphTimer;

        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly SyncLock _syncObj;

        /// <summary>
        /// Reads text from a text/word file
        /// </summary>
        private readonly TextDocumentReader _textDocumentReader;

        /// <summary>
        /// List of abbreviations to look for.  If a period follows any
        /// of these words, don't treat it as a sentence terminator.
        /// </summary>
        private readonly string[] abbreviations = { "dr", "mr", "prof", "mrs", "st", "etc" };

        /// <summary>
        /// Has the loading completed?
        /// </summary>
        private bool _formLoadComplete;

        /// <summary>
        /// The last sentence that was converted to speech
        /// </summary>
        private Sentence _lastSentenceSpoken;

        /// <summary>
        /// Mode of speech. Speak by Para, Sentence or All
        /// </summary>
        private SpeechMode _mode = SpeechMode.Paragraph;

        /// <summary>
        /// List of paragraphs in the parsed text
        /// </summary>
        private List<Paragraph> _paragraphs;

        /// <summary>
        /// Are we paused now?
        /// </summary>
        private bool _paused;

        /// <summary>
        /// In case of error, retry the sentence
        /// </summary>
        private bool _retrySentence = false;

        /// <summary>
        /// List onf sentences in the parsed text
        /// </summary>
        private List<Sentence> _sentences;

        /// <summary>
        /// Current state of lecture manager
        /// </summary>
        private SpeakState _speakState;

        /// <summary>
        /// Height of the text font
        /// </summary>
        private int _textHeight;

        /// <summary>
        /// Makes sure the window stays active
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public LectureManagerMainForm()
        {
            InitializeComponent();

            _textDocumentReader = new TextDocumentReader();

            _syncObj = new SyncLock();
            Mode = SpeechMode.Paragraph;
            FileLoaded = false;
            ShowInTaskbar = false;
            _paragraphs = new List<Paragraph>();
            _sentences = new List<Sentence>();
            _invoker = new ExtensionInvoker(this);
            LectureText = String.Empty;

            Top = 0;
            Left = 0;

            _speakAllParagraphTimer = new Timer();
            _speakAllParagraphTimer.Tick += _speakAllParagraphTimer_Tick;
            LocationChanged += LectureManagerMainForm_LocationChanged;
            Windows.EvtWindowPositionChanged += Windows_EvtWindowPositionChanged;
        }

        /// <summary>
        /// The unit of navigation and speech delivery
        /// </summary>
        public enum SpeechMode
        {
            Sentence,
            Paragraph,
            All
        }

        /// <summary>
        /// Which way is the form being resized so the
        /// controls can be repositioned
        /// </summary>
        private enum MoveDirection
        {
            Vertical,
            Horizontal,
            Both
        }

        /// <summary>
        /// Possible states of the lecture manager
        /// </summary>
        private enum SpeakState
        {
            None,
            Speaking,
            Error
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets whether the file been loaded
        /// </summary>
        public bool FileLoaded { get; set; }

        /// <summary>
        /// Gets whether the form has been loaded and
        /// is now active
        /// </summary>
        public bool FormLoaded { get; private set; }

        /// <summary>
        /// Gets or sets name of the file to load the lecture from
        /// </summary>
        public String LectureFile { get; set; }

        /// <summary>
        /// Gets or sets the text of the lecture
        /// </summary>
        public String LectureText { get; set; }

        /// <summary>
        /// Gets or sets whether to load from file
        /// </summary>
        public bool LoadFromFile { get; set; }

        /// <summary>
        /// Gets or sets the lecture mode (sentence, para, all)
        /// </summary>
        public SpeechMode Mode
        {
            get
            {
                return _mode;
            }

            set
            {
                _mode = value;
                if (_formLoadComplete)
                {
                    updateModeField();
                }
            }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return null; } }

        /// <summary>
        /// Gets or sets whether speaking is
        /// in progress
        /// </summary>
        public bool Speaking
        {
            get
            {
                return _speakState == SpeakState.Speaking;
            }

            set
            {
                _speakState = (value) ? SpeakState.Speaking : SpeakState.None;

                if (_formLoadComplete)
                {
                    updateStatusField();
                }
            }
        }

        /// <summary>
        /// Gets the synchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _syncObj; }
        }

        /// <summary>
        /// Sets form styles
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.Style &= ~WS_SYSMENU;
                return myCp;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// Gets the extension invoker object
        /// </summary>
        /// <returns></returns>
        public ExtensionInvoker GetInvoker()
        {
            return _invoker;
        }

        /// <summary>
        /// Goes to the top of file
        /// </summary>
        public void GoToTop()
        {
            if (!Speaking)
            {
                setCaretPosAndSelect(0);
                _lastSentenceSpoken = null;
            }
        }

        public bool Initialize(StartupArg startupArg)
        {
            return true;
        }

        /// <summary>
        /// Goes backward by the navigational unit
        /// </summary>
        public void NavigateBackward()
        {
            if (!Speaking)
            {
                _lastSentenceSpoken = null;
                switch (Mode)
                {
                    case SpeechMode.All:
                    case SpeechMode.Paragraph:
                        gotoPrevPara();
                        break;

                    case SpeechMode.Sentence:
                        gotoPrevSentence();
                        break;
                }
            }
        }

        /// <summary>
        /// Goes forward by the navigational unit
        /// </summary>
        public void NavigateForward()
        {
            if (!Speaking)
            {
                _lastSentenceSpoken = null;
                switch (Mode)
                {
                    case SpeechMode.All:
                    case SpeechMode.Paragraph:
                        gotoNextPara();
                        break;

                    case SpeechMode.Sentence:
                        gotoNextSentence();
                        break;
                }
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// Not used
        /// </summary>
        public void OnResume()
        {
        }

        /// <summary>
        /// Pauses the lecture
        /// </summary>
        public void PauseSpeaking()
        {
            StopIfSpeaking();
            _paused = true;
            updateStatusField();
        }

        /// <summary>
        /// Read the entire text to speech
        /// </summary>
        public void ProcessReadAllSpeech()
        {
            speakNextSentence();
        }

        /// <summary>
        /// Speak the next unit of text
        /// </summary>
        public void ProcessSpeakNext()
        {
            switch (Mode)
            {
                case SpeechMode.Paragraph:
                    ProcessSpeakParagraph();
                    break;

                case SpeechMode.Sentence:
                    ProcessSpeakSentence();
                    break;

                default:
                    ProcessSpeakParagraph();
                    break;
            }
        }

        /// <summary>
        /// Stops speaking and update ui
        /// </summary>
        /// <returns>true on success</returns>
        public bool StopIfSpeaking()
        {
            StopSpeaking();
            Speaking = false;
            _paused = false;
            updateStatusField();
            return true;
        }

        /// <summary>
        /// Stop speaking
        /// </summary>
        public void StopSpeaking()
        {
            speechTimer.Stop();
            _speakAllParagraphTimer.Stop();

            if (Speaking)
            {
                Speaking = false;
                TTSManager.Instance.ActiveEngine.Stop();
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            removeWatchdogs();

            _syncObj.Status = SyncLock.StatusValues.Closing;

            TTSManager.Instance.ActiveEngine.EvtBookmarkReached -= ActiveEngine_EvtBookmarkReached;
            PanelManager.Instance.EvtScannerShow -= Instance_EvtScannerShow;
            Windows.EvtWindowPositionChanged -= Windows_EvtWindowPositionChanged;

            base.OnFormClosing(e);
        }

        /// <summary>
        /// TImer for speaking the entire para
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _speakAllParagraphTimer_Tick(object sender, EventArgs e)
        {
            _speakAllParagraphTimer.Stop();
            if (Speaking)
            {
                ProcessNextSpeakAllSentence();
            }
        }

        /// <summary>
        /// The text to speech engine informs us when it has exhausted
        /// its buffer so we can load the next unit
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ActiveEngine_EvtBookmarkReached(object sender, TTSBookmarkReachedEventArgs e)
        {
            if (Common.AppPreferences.TTSUseBookmarks)
            {
                Log.Debug("BookmarkReached " + e.Bookmark);

                speechTimer.Stop();
                if (Speaking)
                {
                    // not really using an index now.  Just need to
                    // get the event
                    OnIndexReached(0);
                }
            }
        }

        /// <summary>
        /// Since we are resizing the form, the controls on the form
        /// are all anchored. We have to make sure they are all
        /// repositioned correctly.  Use the center of gravity method
        /// to move all the controls relative to each other.  The CG
        /// can move vertically, horizontally or in both directions
        /// </summary>
        /// <param name="controls">list of controls in the form</param>
        /// <param name="direction">which direction to move the CG</param>
        private void centerControls(List<Control> controls, MoveDirection direction)
        {
            if (controls.Count > 0)
            {
                int sumX = 0;
                int sumY = 0;
                int offsetX = 0;
                int offsetY = 0;

                Point center;

                foreach (Control control in controls)
                {
                    center = new Point(control.Location.X + control.Width / 2, control.Location.Y + control.Height / 2);
                    sumX = sumX + center.X;
                    sumY = sumY + center.Y;
                }

                Point mean = new Point(sumX / controls.Count, sumY / controls.Count);

                center = new Point(Width / 2, Height / 2);
                offsetX = center.X - mean.X;
                offsetY = center.Y - mean.Y;

                foreach (Control control in controls)
                {
                    switch (direction)
                    {
                        case MoveDirection.Vertical:
                            control.Location = new Point(control.Location.X + offsetX, control.Location.Y);
                            break;

                        case MoveDirection.Horizontal:
                            control.Location = new Point(control.Location.X, control.Location.Y + offsetY);
                            break;

                        case MoveDirection.Both:
                            control.Location = new Point(control.Location.X + offsetX, control.Location.Y + offsetY);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Restore focus to this window by simulating a mouse click on it.  This
        /// seems to be the only fool-proof reliable method to make sure that the
        /// window has focus.
        /// </summary>
        private void clickOnThisWindow()
        {
            if (Visible)
            {
                int xpos = Left + 5;
                int ypos = Top + 5;

                Point oldPos = Cursor.Position;
                MouseUtils.ClickLeftMouseButton(xpos, ypos);
                Cursor.Position = oldPos;
            }
        }

        /// <summary>
        /// Dumps para to the debug output
        /// </summary>
        private void dumpParagraphs()
        {
            foreach (Paragraph para in _paragraphs)
            {
                Log.Debug("** NEXT PARA **");
                dumpSentences(para);
            }
        }

        /// <summary>
        /// Dumps sentences in the para to the debug output
        /// </summary>
        /// <param name="para">paragraph</param>
        private void dumpSentences(Paragraph para)
        {
            foreach (Sentence sentence in para.Sentences)
            {
                Log.Debug("** NEXT SENTENCE **");
                String text = textBox1.Text.Substring(sentence.Start, sentence.Length);
                Log.Debug(text);
            }
        }

        /// <summary>
        /// enable watchdogs to keep the window active
        /// </summary>
        private void enableWatchdogs()
        {
            if (_windowActiveWatchdog == null)
            {
                _windowActiveWatchdog = new WindowActiveWatchdog(this);
            }
        }

        /// <summary>
        /// If the sentence is longer than a specified length, show it
        /// in a different color. SOme TTS engines may have a buffer limit
        /// and this flags sentences that will not fit in the buffer
        /// </summary>
        private void flagLongSentences()
        {
            int bufferSize = getSpeechBufferSize();
            if (bufferSize <= 0)
            {
                return;
            }

            foreach (Sentence sentence in _sentences)
            {
                if (sentence.Length > bufferSize)
                {
                    textBox1.SelectionStart = sentence.Start;
                    textBox1.SelectionLength = sentence.Length;
                    textBox1.SelectionColor = Color.Red;
                }
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _speakAllParagraphTimer.Dispose();
            speechTimer.Dispose();
        }

        /// <summary>
        /// Form load. Initialize
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            Text = R.GetString("LectureManager");

            _speakState = SpeakState.None;

            var list = new List<Control> { tableLayoutPanel1, panel1 };
            centerControls(list, MoveDirection.Vertical);

            float fontSize = textBox1.Font.Size;
            float scaleFactor = Common.AppPreferences.ScannerScaleFactor / 10;
            if (scaleFactor > 0.0f)
            {
                Font font = new Font(textBox1.Font.FontFamily.Name, fontSize * scaleFactor, textBox1.Font.Style);
                textBox1.Font = font;

                font = new Font(txtStatusBox.Font.FontFamily.Name, fontSize * scaleFactor, txtStatusBox.Font.Style);
                txtStatusBox.Font = font;
            }

            Size s = TextRenderer.MeasureText("Hello", textBox1.Font);
            _textHeight = s.Height;

            MaximizeBox = false;

            var panel = PanelManager.Instance.GetCurrentPanel();
            if (panel != null)
            {
                positionForm(panel as Form);
            }

            enableWatchdogs();

            PanelManager.Instance.EvtScannerShow += Instance_EvtScannerShow;

#pragma warning disable 4014
            loadFile();
#pragma warning restore 4014

            TTSManager.Instance.ActiveEngine.EvtBookmarkReached += ActiveEngine_EvtBookmarkReached;
            FormLoaded = true;
        }

        /// <summary>
        /// Gets the last word in the sentence
        /// </summary>
        /// <param name="sentence">input sentence</param>
        /// <returns>last word, empty if empty</returns>
        private String getLastWord(String sentence)
        {
            String retVal = String.Empty;
            var words = getWords(sentence);
            if (words.Any())
            {
                retVal = words[words.Count - 1];
            }

            return retVal;
        }

        /// <summary>
        /// Gets the paragraph object of the next paragraph
        /// </summary>
        /// <returns>para object, null if last para</returns>
        private Paragraph getNextPara()
        {
            if (textBox1.SelectionLength == 0)
            {
                foreach (var para in _paragraphs)
                {
                    if (textBox1.SelectionStart == para.Start && para.Sentences.Any())
                    {
                        return para;
                    }
                }
            }

            return _paragraphs.FirstOrDefault(para => para.Start > textBox1.SelectionStart);
        }

        /// <summary>
        /// Gets the next sentence object
        /// </summary>
        /// <returns>sentence object, null if last</returns>
        private Sentence getNextSentence()
        {
            if (textBox1.SelectionLength == 0)
            {
                foreach (var sentence in _sentences)
                {
                    if (textBox1.SelectionStart == sentence.Start)
                    {
                        return sentence;
                    }
                }
            }

            return _sentences.FirstOrDefault(sentence => sentence.Start > textBox1.SelectionStart);
        }

        /// <summary>
        /// Gets the sentence object of the next sentence that
        /// has to be spoken
        /// </summary>
        /// <returns>sentence object</returns>
        private Sentence getNextSentenceToSpeak()
        {
            int selectionStart = textBox1.SelectionStart;
            if (_lastSentenceSpoken == null || textBox1.SelectionLength == 0)
            {
                foreach (Paragraph para in _paragraphs)
                {
                    if (selectionStart == para.Start && para.Sentences.Any())
                    {
                        return para.Sentences[0];
                    }
                }
            }

            if (textBox1.SelectionLength == 0)
            {
                foreach (Sentence sentence in _sentences)
                {
                    if (textBox1.SelectionStart == sentence.Start)
                    {
                        return sentence;
                    }
                }
            }
            else if (_lastSentenceSpoken == null)
            {
                foreach (Sentence sentence in _sentences)
                {
                    if (textBox1.SelectionStart >= sentence.Start && textBox1.SelectionStart < sentence.End)
                    {
                        return sentence;
                    }
                }
            }
            else
            {
                selectionStart = _lastSentenceSpoken.Start + _lastSentenceSpoken.Length;
            }

            return _sentences.FirstOrDefault(sentence => sentence.Start >= selectionStart);
        }

        /// <summary>
        /// Gets the prevous para object
        /// </summary>
        /// <returns>para object, null if already at the top</returns>
        private Paragraph getPreviousPara()
        {
            for (int ii = _paragraphs.Count - 1; ii >= 0; ii--)
            {
                if (_paragraphs[ii].Start < textBox1.SelectionStart)
                {
                    return _paragraphs[ii];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets previous sentence object
        /// </summary>
        /// <returns>sentence object</returns>
        private Sentence getPreviousSentence()
        {
            for (int ii = _sentences.Count - 1; ii >= 0; ii--)
            {
                if (_sentences[ii].Start < textBox1.SelectionStart)
                {
                    return _sentences[ii];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the buffer size of the TTS engine. Some hardware
        /// engines have limited buffers before an overflow will occur
        /// </summary>
        /// <returns>buffer size</returns>
        private int getSpeechBufferSize()
        {
            int retVal = -1;
            ITTSEngine speechEngine = Context.AppTTSManager.ActiveEngine;
            if (speechEngine != null)
            {
                ExtensionInvoker invoker = speechEngine.GetInvoker();
                if (invoker != null)
                {
                    retVal = invoker.GetIntValue("SpeechBufferSize", -1);
                }
            }

            return retVal;
        }

        /// <summary>
        /// If not using bookmarks, use an algorithm to
        /// determine how long to wait for TTS to complete,
        /// depending on the lenght of the text
        /// </summary>
        /// <param name="speechText">speech text</param>
        /// <returns>interval in ms</returns>
        private int getSpeechTimerInterval(string speechText)
        {
            int speechInterval;

            if (speechText.Length > 160)
            {
                speechInterval = 18000;
            }
            else if (speechText.Length > 100)
            {
                speechInterval = 15000;
            }
            else if (speechText.Length > 40)
            {
                speechInterval = 10000;
            }
            else
            {
                speechInterval = 7000;
            }

            Log.Debug("speechInterval=" + speechInterval.ToString());
            return speechInterval;
        }

        /// <summary>
        /// Splits the specified string into words
        /// </summary>
        /// <param name="s">input string</param>
        /// <returns>list of words</returns>
        private List<String> getWords(String s)
        {
            String[] words = Regex.Split(s, @"\W");

            var wordlist = new List<string>();
            foreach (var word in words)
            {
                if (!String.IsNullOrEmpty(word))
                {
                    wordlist.Add(word);
                }
            }

            return wordlist;
        }

        /// <summary>
        /// Navigates to the next paragraph
        /// </summary>
        private void gotoNextPara()
        {
            Paragraph para = getNextPara();
            if (para != null)
            {
                setCaretPosAndSelect(para.Start, para.Length);
            }
            else if (textBox1.Text.Length > 0)
            {
                setCaretPosAndSelect(textBox1.Text.Length);
            }
        }

        /// <summary>
        /// Starting from the specified index offset
        /// in the text, goes to the beginning of the
        /// next para
        /// </summary>
        /// <param name="text">input text</param>
        /// <param name="index">where to start from</param>
        /// <returns>starting index of next para</returns>
        private int gotoNextPara(String text, int index)
        {
            while (index < text.Length && (isWhiteSpace(text[index])
                                           || isSentenceTerminator(text[index]) ||
                                           isParagraphTerminator(text[index])))
            {
                index++;
            }

            return index;
        }

        /// <summary>
        /// Goes to the next sentence
        /// </summary>
        private void gotoNextSentence()
        {
            var sentence = getNextSentence();
            if (sentence != null)
            {
                setCaretPosAndSelect(sentence.Start, sentence.Length);
            }
            else if (textBox1.Text.Length > 0)
            {
                setCaretPosAndSelect(textBox1.Text.Length);
            }
        }

        /// <summary>
        /// Navigates to the previous paragraph
        /// </summary>
        private void gotoPrevPara()
        {
            Paragraph para = getPreviousPara();
            if (para != null)
            {
                setCaretPosAndSelect(para.Start, para.Length);
            }
        }

        /// <summary>
        /// Navigate to the previous sentence
        /// </summary>
        private void gotoPrevSentence()
        {
            var sentence = getPreviousSentence();
            if (sentence != null)
            {
                setCaretPosAndSelect(sentence.Start, sentence.Length);
            }
        }

        /// <summary>
        /// Inovked when the companian scanner is displayed.
        /// Dock this form to the scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="arg">event args</param>
        private void Instance_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (arg.Scanner != this)
            {
                positionForm(arg.Scanner.Form);
            }
        }

        /// <summary>
        /// Have we reched the end of the text stream?
        /// </summary>
        /// <param name="text">input text</param>
        /// <param name="index">starting index</param>
        /// <returns>true if it is</returns>
        private bool isEnd(String text, int index)
        {
            return index >= text.Length;
        }

        /// <summary>
        /// Is this the last sentence in the paragraph
        /// </summary>
        /// <param name="sentence">input sentence</param>
        /// <returns>true if it is</returns>
        private bool isLastSentence(Sentence sentence)
        {
            if (sentence == null)
            {
                return false;
            }

            return _paragraphs.Any(para => para.isLastSentence(sentence));
        }

        /// <summary>
        /// Checks if the char is a para terminator
        /// </summary>
        /// <param name="ch">input char</param>
        /// <returns>true if it is<returns>
        private bool isParagraphTerminator(char ch)
        {
            return ch == 0x0D || ch == 0x0A;
        }

        /// <summary>
        /// Checks if the specified char is a sentence
        /// terminator
        /// </summary>
        /// <param name="ch">input char</param>
        /// <returns>true if it is</returns>
        private bool isSentenceTerminator(char ch)
        {
            return ch == '.' || ch == '?' || ch == '!';
        }

        /// <summary>
        /// Checks if the char is a space or tab
        /// </summary>
        /// <param name="ch">char</param>
        /// <returns>true if it is </returns>
        private bool isWhiteSpace(char ch)
        {
            return ch == ' ' || ch == '\t';
        }

        /// <summary>
        /// Make this window active
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void LectureManagerMainForm_LocationChanged(object sender, EventArgs e)
        {
            clickOnThisWindow();
        }

        /// <summary>
        /// Load the file asynchronously so as to not tie
        /// up the ui
        /// </summary>
        /// <returns></returns>
        private async Task loadFile()
        {
            Task t = Task.Factory.StartNew(() =>
            {
                if (LoadFromFile)
                {
                    if (!String.IsNullOrEmpty(LectureFile))
                    {
                        LectureText = _textDocumentReader.GetText(LectureFile);
                        FileLoaded = true;
                    }
                }
                else
                {
                    FileLoaded = true;
                }

                updateFileName(LectureFile);

                parseAndDisplayText();

                _formLoadComplete = true;
            });
            await t;
        }

        /// <summary>
        /// Callback invoked when the TTS engine has reached
        /// the specified book mark in the stream
        /// </summary>
        /// <param name="index">bookmark index</param>
        private void OnIndexReached(int index)
        {
            Log.Debug("OnIndexReached() - index=" + index);

            switch (Mode)
            {
                case SpeechMode.All:
                    if (isLastSentence(_lastSentenceSpoken) &&
                            LectureManagerSpeakAllParagraphPause > 0)
                    {
                        Invoke(new MethodInvoker(delegate
                            {
                                _speakAllParagraphTimer.Interval = LectureManagerSpeakAllParagraphPause;
                                _speakAllParagraphTimer.Start();
                            }));
                    }
                    else
                    {
                        ProcessNextSpeakAllSentence();
                    }

                    break;

                case SpeechMode.Paragraph:
                    Log.Debug("OnIndexReached() - currentState=" + Mode.ToString());
                    Log.Debug(" now continuing with next sentence...");
                    ProcessNextSpeakParagraphSentence();
                    break;

                case SpeechMode.Sentence:
                    Speaking = false;
                    break;
            }
        }

        /// <summary>
        /// Parses the text, breaks it up into paras and
        /// sentences and stores the entire text as a list
        /// of paras and sentences
        /// </summary>
        private void parse()
        {
            _paragraphs = new List<Paragraph>();
            _sentences = new List<Sentence>();

            if (String.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                return;
            }

            var text = textBox1.Text;

            for (int ii = 0; ii < text.Length;)
            {
                ii = gotoNextPara(text, ii);

                if (isEnd(text, ii))
                {
                    break;
                }

                var para = new Paragraph { Start = ii };
                ii = parseSentences(para, text, ii);
                if (para.Sentences.Any())
                {
                    var lastSentence = para.Sentences[para.Sentences.Count - 1];
                    int lastSentenceEndOffset = lastSentence.Start + lastSentence.Length;
                    para.Length = lastSentenceEndOffset - para.Start;

                    _paragraphs.Add(para);
                }
            }

            _sentences = new List<Sentence>();
            foreach (Paragraph para in _paragraphs)
            {
                foreach (Sentence sentence in para.Sentences)
                {
                    _sentences.Add(sentence);
                }
            }
        }

        /// <summary>
        /// Parses the text into para and sentences and displays
        /// the text in the form
        /// </summary>
        private void parseAndDisplayText()
        {
            Invoke(new MethodInvoker(delegate
            {
                textBox1.Text = LectureText;
                textBox1.Select(0, 0);
                parse();
                flagLongSentences();
                textBox1.Select(0, 0);
                textBox1.ScrollToCaret();
                textBox1.Focus();
            }));
        }

        /// <summary>
        /// Parses the specified paragraph into sentences, creates
        /// a list of sentence object and adds the list to the paragraph
        /// object
        /// </summary>
        /// <param name="para">paragraph object</param>
        /// <param name="text">para text</param>
        /// <param name="index">index in the text to start from</param>
        /// <returns>index in the text where parsing stopped</returns>
        private int parseSentences(Paragraph para, String text, int index)
        {
            int ii;
            for (ii = index; ii < text.Length;)
            {
                if (isParagraphTerminator(text[ii]))
                {
                    break;
                }

                ii = skipWhiteSpaces(text, ii);
                int start = ii;
                ii = skipSentenceAndParaTerminators(text, start);
                if (ii > start)
                {
                    var sentence = new Sentence(para) { Start = start, Length = ii - start };
                    if (sentence.Length > 0 &&
                        sentence.IsValid(textBox1.Text.Substring(sentence.Start, sentence.Length)))
                    {
                        para.Sentences.Add(sentence);
                    }
                }

                ii = skipSentenceTerminators(text, ii);
            }

            while (true)
            {
                int sentenceNum = resolveAbbreviations(para, text);
                if (sentenceNum < 0)
                {
                    break;
                }

                Sentence s = para.Sentences[sentenceNum + 1];
                para.Sentences[sentenceNum].Length += s.Length + (s.Start - para.Sentences[sentenceNum].End);
                para.Sentences.Remove(s);
            }

            return ii;
        }

        /// <summary>
        /// Dock the form next to the scanner
        /// </summary>
        /// <param name="form">scanner form</param>
        private void positionForm(Form form)
        {
            Left = 0;
            Top = 0;
            Width = Screen.PrimaryScreen.WorkingArea.Width - form.Width;
            Height = Screen.PrimaryScreen.WorkingArea.Height;
            textBox1.Focus();
        }

        /// <summary>
        /// Speak all the next sentences
        /// </summary>
        private void ProcessNextSpeakAllSentence()
        {
            speakNextSentence();
        }

        /// <summary>
        /// Speak the next sentence in the paragraph
        /// </summary>
        private void ProcessNextSpeakParagraphSentence()
        {
            if (isLastSentence(_lastSentenceSpoken))
            {
                Log.Debug("REACHED END OF PARAGRAPH!");
                Speaking = false;
                textBox1.Focus();
            }
            else
            {
                speakNextSentence();
            }
        }

        /// <summary>
        /// Speak the next paragraph sentence by sentence
        /// </summary>
        private void ProcessSpeakParagraph()
        {
            Mode = SpeechMode.Paragraph;
            speakNextSentence();
        }

        /// <summary>
        /// Speak the next sentence
        /// </summary>
        private void ProcessSpeakSentence()
        {
            Mode = SpeechMode.Sentence;
            speakNextSentence();
        }

        /// <summary>
        /// Remove watchdogs
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
        /// Resolves any abbreviations in the text. For example,
        /// Dr. should not be interpreted as the end of the sentence, but
        /// merely as a part of the sentence.
        /// </summary>
        /// <param name="para">paragraph object</param>
        /// <param name="text">paragraph text</param>
        /// <returns>index offset where parsing stopped</returns>
        private int resolveAbbreviations(Paragraph para, String text)
        {
            var removeList = new List<Sentence>();
            int retVal = -1;
            for (int ii = 0; ii < para.Sentences.Count() - 1; ii++)
            {
                Sentence sentence = para.Sentences[ii];
                if (sentence.Length > 0)
                {
                    String sentenceString = text.Substring(sentence.Start, sentence.Length);
                    if (sentenceString.Length > 0)
                    {
                        if (sentenceString[sentenceString.Length - 1] == '.')
                        {
                            String word = getLastWord(sentenceString);
                            if (word.Length == 1 || abbreviations.Contains(word.ToLower()))
                            {
                                retVal = ii;
                                break;
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Sends the specified text to the TTS engine
        /// </summary>
        /// <param name="speechText">text to send</param>
        private bool SendTextImmediate(string speechText)
        {
            Log.Debug("Entering...speechText>>>" + speechText + "<<<");

            bool retVal = true;

            if (!string.IsNullOrEmpty(speechText) && !speechText.Equals("\r\n\r\n"))
            {
                speechTimer.Interval = getSpeechTimerInterval(speechText);
                Log.Debug("SendTextImmediate() - Text to speak=" + speechText);
                Log.Debug("speechTimer.Interval=" + speechTimer.Interval);

                if (Common.AppPreferences.TTSUseBookmarks)
                {
                    int bookmark;
                    retVal = TTSManager.Instance.ActiveEngine.SpeakAsync(speechText, out bookmark);
                }
                else
                {
                    retVal = TTSManager.Instance.ActiveEngine.Speak(speechText);
                    if (retVal)
                    {
                        AuditLog.Audit(new AuditEventTextToSpeech(TTSManager.Instance.ActiveEngine.Descriptor.Name));
                        Log.Debug("starting speechTimer...");
                        speechTimer.Start();
                    }
                }
            }
            else
            {
                Log.Debug("No text to speak!");
            }

            if (!retVal)
            {
                _speakState = SpeakState.Error;
                updateStatusField();
                _retrySentence = true;
            }
            else
            {
                _retrySentence = false;
            }

            return retVal;
        }

        /// <summary>
        /// Highlights text starting at the specified position
        /// and length
        /// </summary>
        /// <param name="pos">starting offset</param>
        /// <param name="length">number of chars to highlight</param>
        private void setCaretPosAndSelect(int pos, int length = 0)
        {
            textBox1.Focus();
            textBox1.Select(pos, length);

            Rectangle textBoxBounds = this.textBox1.Bounds;
            textBoxBounds = textBox1.Parent.RectangleToScreen(textBoxBounds);

            Point caretLocation = textBox1.GetPositionFromCharIndex(textBox1.SelectionStart);
            caretLocation = textBox1.Parent.PointToScreen(caretLocation);

            Log.Debug("textBounds.Top: " + textBoxBounds.Top +
                        ", caretLocation.Y : " + caretLocation.Y +
                        ", scrolllines*textheight: " + (ScrollThresholdLines * _textHeight));

            if ((textBoxBounds.Bottom - caretLocation.Y) <= ScrollThresholdLines * _textHeight)
            {
                Log.Debug("Scrolling down...");
                SendMessage(textBox1.Handle, EM_LINESCROLL, 0, ScrollNumberOfLines);
            }
            else
            {
                if (caretLocation.Y < 0)
                {
                    Log.Debug("caretLocation is negative. Calling scroll to caret");
                    textBox1.ScrollToCaret();
                }

                caretLocation = textBox1.GetPositionFromCharIndex(textBox1.SelectionStart);
                caretLocation = textBox1.Parent.PointToScreen(caretLocation);

                if (caretLocation.Y - textBoxBounds.Top <= ScrollThresholdLines * _textHeight)
                {
                    Log.Debug("Scrolling up...");
                    SendMessage(textBox1.Handle, EM_LINESCROLL, 0, -ScrollNumberOfLines);
                }
            }

            Log.Debug("setCaretPos.  SelectionStart: " + textBox1.SelectionStart);
        }

        /// <summary>
        /// Starting from the index, skips sentence and
        /// para terminators and returns index
        /// </summary>
        /// <param name="text">input text</param>
        /// <param name="index">starting offset</param>
        /// <returns>index offset</returns>
        private int skipSentenceAndParaTerminators(String text, int index)
        {
            while (index < text.Length &&
                    !isSentenceTerminator(text[index]) &&
                    !isParagraphTerminator(text[index]))
            {
                index++;
            }

            if (index < text.Length - 1 && isSentenceTerminator(text[index]))
            {
                index++;
            }

            return index;
        }

        /// <summary>
        /// starting at the specified index skips all
        /// sentence terminators.
        /// </summary>
        /// <param name="text">input text</param>
        /// <param name="index">starting index</param>
        /// <returns>index offset</returns>
        private int skipSentenceTerminators(string text, int index)
        {
            while (index < text.Length && isSentenceTerminator(text[index]))
            {
                index++;
            }

            return index;
        }

        /// <summary>
        /// Starting at index, skips all the white spaces
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="index">starting index</param>
        /// <returns>index offset</returns>
        private int skipWhiteSpaces(string text, int index)
        {
            while (index < text.Length && isWhiteSpace(text[index]))
            {
                index++;
            }

            return index;
        }

        /// <summary>
        /// Sends the next sentence to the tts engine
        /// </summary>
        /// <returns>sentence spoken</returns>
        private Sentence speakNextSentence()
        {
            _paused = false;

            Sentence sentence;
            if (_retrySentence && _lastSentenceSpoken != null)
            {
                sentence = _lastSentenceSpoken;
            }
            else
            {
                sentence = getNextSentenceToSpeak();
            }

            if (sentence != null)
            {
                setCaretPosAndSelect(sentence.Start, sentence.Length);
                Speaking = true;
                txtStatusBox.Invoke((MethodInvoker)delegate { txtStatusBox.Text = textBox1.SelectedText; });
                SendTextImmediate(textBox1.SelectedText.Trim());
                _lastSentenceSpoken = sentence;
                return sentence;
            }

            if (textBox1.Text.Length > 0)
            {
                setCaretPosAndSelect(textBox1.Text.Length);
                Speaking = false;
            }

            _lastSentenceSpoken = null;

            return _lastSentenceSpoken;
        }

        /// <summary>
        /// Timer tick method used when timers are used
        /// to gauge how long the speech tackes
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void speechTimer_Tick(object sender, EventArgs e)
        {
            speechTimer.Stop();

            // not really using an index now.  just need to get the event
            OnIndexReached(0);
        }

        /// <summary>
        /// Updates the status bar with the name of the file
        /// </summary>
        /// <param name="fileName">input file name</param>
        private void updateFileName(String fileName)
        {
            String name;
            try
            {
                name = Path.GetFileName(fileName);
            }
            catch
            {
                name = String.Empty;
            }

            Windows.SetText(lblChosenFile, name);
        }

        /// <summary>
        /// Update the mode field on the form based
        ///  on the mode setting
        /// </summary>
        private void updateModeField()
        {
            if (_formLoadComplete)
            {
                Windows.SetText(lblMode, _mode.ToString());
            }
        }

        /// <summary>
        /// Update the status bar on the form depending
        /// on the context
        /// </summary>
        private void updateStatusField()
        {
            if (_formLoadComplete)
            {
                String status = String.Empty;

                if (Speaking)
                {
                    switch (Mode)
                    {
                        case SpeechMode.Paragraph:
                            status = R.GetString("SpeakingPara");
                            break;

                        case SpeechMode.All:
                            status = R.GetString("SpeakingAll");
                            break;

                        case SpeechMode.Sentence:
                            status = R.GetString("SpeakingSentence");
                            break;

                        default:
                            status = R.GetString("Speaking");
                            break;
                    }
                }
                else if (_speakState == SpeakState.Error)
                {
                    status = R.GetString("SpeakError");
                }
                else if (_paused)
                {
                    status = R.GetString("LMPaused");
                }

                Windows.SetText(lblStatus, status);
            }
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
                positionForm(form);
            }
        }

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Disable mouse clicks on the text window
        /// </summary>
        public class TextBoxMouseDisabled : RichTextBox
        {
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    // WM_LBUTTONDOWN
                    case 0x0201:

                    // WM_LBUTTONUP
                    case 0x0202:

                    // WM_LBUTTONDBLCLK
                    case 0x0203:

                    // WM_RBUTTONDOWN
                    case 0x0204:

                    // WM_RBUTTONUP
                    case 0x0205:

                    // WM_RBUTTONDBLCLK
                    case 0x0206:
                        {
                            return;
                        }
                }

                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// Represents a paragraph of text. Contains
        /// the starting offset of the para in the text
        /// and length of the text.  ALso contains a
        /// list of sentences that comprise the para
        /// </summary>
        private class Paragraph
        {
            /// <summary>
            /// Initialize
            /// </summary>
            public Paragraph()
            {
                Start = 0;
                Length = 0;
                Sentences = new List<Sentence>();
            }

            /// <summary>
            /// Gets offset of length of para
            /// </summary>
            public int End
            {
                get { return Start + Length; }
            }

            /// <summary>
            /// Gets or sets the length
            /// </summary>
            public int Length
            {
                get;
                set;
            }

            /// <summary>
            /// Gets list of sentences
            /// </summary>
            public List<Sentence> Sentences { get; private set; }

            /// <summary>
            /// Gets or sets starting offset
            /// </summary>
            public int Start { get; set; }

            /// <summary>
            /// Checks if this is the first sentence
            /// </summary>
            /// <param name="sentence">sentence</param>
            /// <returns>true if is</returns>
            public bool isFirstSentence(Sentence sentence)
            {
                return Sentences.Any() && Sentences[0] == sentence;
            }

            /// <summary>
            /// Checks fi this is the last sentence
            /// </summary>
            /// <param name="sentence">sentence object</param>
            /// <returns>true if it is</returns>
            public bool isLastSentence(Sentence sentence)
            {
                return Sentences.Any() && Sentences[Sentences.Count - 1] == sentence;
            }
        }

        /// <summary>
        /// Represents a sentence. Stores the starting offset
        /// of the sentence in the text stream and the length
        /// of the text
        /// </summary>
        private class Sentence
        {
            /// <summary>
            /// Initialize
            /// </summary>
            /// <param name="parent">Parent paragraph</param>
            public Sentence(Paragraph parent)
            {
                Start = 0;
                Length = 0;
                Parent = parent;
            }

            /// <summary>
            /// Gets the offset of the end of the sentence
            /// </summary>
            public int End
            {
                get { return Start + Length; }
            }

            /// <summary>
            /// Gets or sets the length of the sentence
            /// </summary>
            public int Length { get; set; }

            /// <summary>
            /// Gets the parent paragraph object
            /// </summary>
            public Paragraph Parent { get; private set; }

            /// <summary>
            /// Gets or sets starting offset of the sentence
            /// </summary>
            public int Start { get; set; }

            /// <summary>
            /// Checks if all the chars in the sentence are
            /// valid alphanumeric chars.
            /// </summary>
            /// <param name="s">input string</param>
            /// <returns>true if the sentence is good.</returns>
            public bool IsValid(String s)
            {
                if (s.Length <= 2)
                {
                    foreach (char c in s)
                    {
                        if (Char.IsControl(c) || c < 0x20 || c > 127)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}