////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowForm.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.TalkWindowManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ACAT.ACATResources;
using ACAT.Lib.Core.Extensions;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// The talk window dialog. It lets the user communicate.
    /// User types in the text and presses ENTER to convert
    /// text to speech.  If word prediction learning is supported,
    /// also learns user writing style from the text entered in
    /// the talk window.
    /// </summary>
    [DescriptorAttribute("BF1C93D7-3962-4A52-A56D-668149D116AE",
                        "TalkWindowForm",
                        "ACAT Talk Window")]
    public partial class TalkWindowForm : TalkWindowBase
    {
        /// <summary>
        /// Name of the bitmap to display in the top left
        /// corner of the talk window
        /// </summary>
        private const String _talkWindowIconBitmap = "talkwindowicon.png";

        /// <summary>
        /// Font to use for the speaker icon
        /// </summary>
        private Font _acatFont;

        /// <summary>
        /// Makes sure the talk window retains focus
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Makes sure that nothing overlaps the talk window
        /// </summary>
        private WindowOverlapWatchdog _windowOverlapWatchdog;

        /// <summary>
        /// Used to set the cursor positon when restoring talk window position
        /// </summary>
        private System.Timers.Timer cursorPosTimer;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public TalkWindowForm()
        {
            InitializeComponent();

            TalkWindowTextBox = textBox;

            enableDateTimeDisplay = Common.AppPreferences.TalkWindowDisplayDateTimeEnable;
            displayDateFormat = Common.AppPreferences.TalkWindowDisplayDateFormat;
            displayTimeFormat = Common.AppPreferences.TalkWindowDisplayTimeFormat;

            ShowInTaskbar = false;
            MaximizeBox = false;

            Windows.SetTopMost(this);

            subscribeToEvents();

            setColors();

            textBox.ContextMenu = new ContextMenu();

            setIcon();
        }

        /// <summary>
        /// Gets or sets the text in the talk window
        /// </summary>
        public override String TalkWindowText
        {
            get
            {
                var retVal = String.Empty;
                Invoke(new MethodInvoker(delegate
                {
                    retVal = textBox.Text;
                }));

                return retVal;
            }

            set
            {
                Invoke(new MethodInvoker(delegate
                {
                    textBox.Text = value;
                    if (String.IsNullOrEmpty(value))
                    {
                        textBox.SelectionStart = 0;
                        textBox.SelectionLength = 0;
                        textBox.ScrollToCaret();
                    }
                    else
                    {
                        // there is a strange problem with restoring text.  Windows
                        // reports that text has changed prematurely. The cursor pos
                        // it reports is somewhere in the middle of the text.  to
                        // work around this, let's set a timer and set the cursor
                        // position to the end of the sentence.
                        cursorPosTimer = new System.Timers.Timer { Interval = 100 };
                        cursorPosTimer.Elapsed += _cursorPosTimer_Elapsed;
                        cursorPosTimer.Start();
                    }
                }));
            }
        }

        /// <summary>
        /// Clear text in the talk window
        /// </summary>
        public override void Clear()
        {
            Invoke(new MethodInvoker(delegate 
            {
                    textBox.Text = String.Empty;
            }));
        }

        /// <summary>
        /// Talk window position changed
        /// </summary>
        public override void OnPositionChanged()
        {
            Windows.ClickOnWindow(this);
            User32Interop.SetForegroundWindow(Handle);
        }

        /// <summary>
        /// Displays the date/time on the form
        /// </summary>
        /// <param name="text">date/time string</param>
        protected override void displayDateTime(String text)
        {
            toolStripLabelDate.Text = text;
        }

        /// <summary>
        /// Called when form is closing
        /// </summary>
        /// <param name="e">event arg</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            notifyRequestClose();
            e.Cancel = false;
            base.OnFormClosing(e);
        }

        /// <summary>
        // Timer function to set the cursor position.
        // There is a strange problem with restoring text.  Windows
        // reports that text has changed prematurely. The cursor pos
        // it reports is somewhere in the middle of the text.  to
        // work around this, let's set a timer and set the cursor
        // position to the end of the sentence.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cursorPosTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cursorPosTimer.Stop();
            Invoke(new MethodInvoker(delegate
            {
                textBox.Select(textBox.Text.Length, 0);
                textBox.ScrollToCaret();
            }));
        }

        /// <summary>
        /// Some property related to the text to speech
        /// engine changed (eg volume). Update the volume
        /// level in the talk window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ActiveEngine_EvtPropertyChanged(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(delegate
            {
                updateVolumeStatus();
            }));
        }

        /// <summary>
        /// Gets the previous paragraph in the talk window.
        /// </summary>
        /// <returns>text of the previous paragraph</returns>
        private String getPreviousPara()
        {
            int index = textBox.SelectionStart;
            var text = textBox.Text;

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
        /// Activates the "Phrases" agent that allows the user to
        /// convert canned phrases to speech
        /// </summary>
        private async void handlePhraseSpeak()
        {
            notifyRequestClose();
            IApplicationAgent agent = Context.AppAgentMgr.GetAgentByCategory("PhraseSpeakAgent");
            if (agent != null)
            {
                if (agent is IExtension)
                {
                    var invoker = agent.GetInvoker();
                    invoker.SetValue("EnableSearch", true);
                    invoker.SetValue("PhraseListEdit", false);
                }

                await Context.AppAgentMgr.ActivateAgent(agent as IFunctionalAgent);
            }
        }

        /// <summary>
        /// The text to speech engine changed
        /// </summary>
        /// <param name="oldEngine">previous engine</param>
        /// <param name="newEngine">new engine</param>
        private void Instance_EvtEngineChanged(ITTSEngine oldEngine, ITTSEngine newEngine)
        {
            oldEngine.EvtPropertyChanged -= ActiveEngine_EvtPropertyChanged;
            newEngine.EvtPropertyChanged += ActiveEngine_EvtPropertyChanged;
        }

        /// <summary>
        /// User clicked on the speaker icon. Mute or unmute
        /// the text to speech
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void labelSpeaker_Click(object sender, EventArgs e)
        {
            ITTSEngine engine = TTSManager.Instance.ActiveEngine;
            if (engine.IsMuted())
            {
                engine.UnMute();
            }
            else
            {
                engine.Mute();
            }
        }

        /// <summary>
        /// Displays an optional icon in the top left corner
        /// of the talk window
        /// </summary>
        private void setIcon()
        {
            var talkWindowIcon = FileUtils.GetImagePath(_talkWindowIconBitmap);
            if (File.Exists(talkWindowIcon))
            {
                var image = Image.FromFile(FileUtils.GetImagePath(_talkWindowIconBitmap));
                image = ImageUtils.ImageResize(image, toolStripLabelIcon.Width, toolStripLabelIcon.Height);
                toolStripLabelIcon.ImageAlign = ContentAlignment.MiddleCenter;
                toolStripLabelIcon.Image = image;
            }
        }

        /// <summary>
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            Log.Debug("subscribing to formclosing");
            Load += TalkWindowForm_Load;
            FormClosing += TalkWindowForm_FormClosing;
            VisibleChanged += TalkWindowForm_VisibleChanged;
            TTSManager.Instance.ActiveEngine.EvtPropertyChanged += ActiveEngine_EvtPropertyChanged;
            TTSManager.Instance.EvtEngineChanged += Instance_EvtEngineChanged;
            labelSpeaker.Click += labelSpeaker_Click;
            textBox.KeyPress += textBox_KeyPress;
            trackBarVolume.MouseUp += trackBarVolume_MouseUp;
            toolStripButtonClear.Click += ToolStripButtonClearOnClick;
        }

        /// <summary>
        /// Called when closing. Release resources
        /// </summary>
        private void TalkWindowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_windowOverlapWatchdog != null)
            {
                _windowOverlapWatchdog.Dispose();
            }

            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
            }

            if (_acatFont != null)
            {
                _acatFont.Dispose();
            }

            TTSManager.Instance.ActiveEngine.EvtPropertyChanged -= ActiveEngine_EvtPropertyChanged;
        }

        /// <summary>
        /// Called when loaded. Creates timers, positions the window,
        /// initializes etc.
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        /// </summary>
        private void TalkWindowForm_Load(object sender, EventArgs e)
        {
            if (_acatFont == null)
            {
                var fontFamily = Fonts.Instance.GetFontFamily("ACAT Icon");
                if (fontFamily != null)
                {
                    _acatFont = new Font(fontFamily, 16);
                    labelSpeaker.Font = _acatFont;
                    toolStripButtonClear.Font = _acatFont;
                }
            }

            setFont(FontSize);
            updateVolumeStatus();
            CenterToScreen();
            Text = R.GetString("TalkWindow");
            _windowOverlapWatchdog = new WindowOverlapWatchdog(this);
            _windowActiveWatchdog = new WindowActiveWatchdog(this);
            textBox.SelectionStart = textBox.Text.Length;
            textBox.DeselectAll();
        }

        /// <summary>
        /// Visibility of the talk window changed.  Since the textbox
        /// is the only tabbable control, Windows selects the entire text
        /// because the text box received focus (as if the user tabbed
        /// into it).  So let's do an unselect.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void TalkWindowForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                textBox.Select(textBox.SelectionStart, 0);
            }
        }

        /// <summary>
        /// User pressed enter. Do a text to speech of the current
        /// current paragraph. if the user presses enter a second
        /// time, it respeaks the para.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    var para = getPreviousPara();

                    if (String.IsNullOrEmpty(textBox.Text.Trim()))
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
                        e.Handled = true;
                        textToSpeech(para);
                    }
                    else
                    {
                        ttsAndLearn(textToSpeak);
                    }
                }
                else if (e.KeyChar == '\t')
                {
                    e.Handled = true;
                    handlePhraseSpeak();
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
        /// User clicked on the "Clear" button on  the toolbar.  Clear
        /// the text in the window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void ToolStripButtonClearOnClick(object sender, EventArgs eventArgs)
        {
            Clear();
        }

        /// <summary>
        /// Mouse up event on the track bar that represents
        /// the volume
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void trackBarVolume_MouseUp(object sender, MouseEventArgs e)
        {
            textBox.Select();
            TTSManager.Instance.SetNormalizedVolume(trackBarVolume.Value);
        }

        /// <summary>
        /// Converts the current para to speech and optionally learns
        /// the text to refine the word prediction model
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
        /// Update the volume slider depending on the
        /// current volume level
        /// </summary>
        private void updateVolumeStatus()
        {
            int volume = TTSManager.Instance.GetNormalizedVolume().Value;

            if (!TTSManager.Instance.ActiveEngine.IsMuted())
            {
                trackBarVolume.Value = volume;
                labelSpeaker.Text = "F";
                labelSpeaker.ForeColor = Color.Black;
                labelVolumeLevel.Text = volume.ToString();
                labelVolumeLevel.ForeColor = Color.Black;
            }
            else
            {
                labelSpeaker.Text = "\u0511";
                labelSpeaker.ForeColor = Color.Red;
                labelVolumeLevel.Text = "M";
                labelVolumeLevel.ForeColor = Color.Red;
            }
        }
    }
}