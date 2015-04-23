////////////////////////////////////////////////////////////////////////////
// <copyright file="TalkWindowForm.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.TalkWindowManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WordPredictionManagement;
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

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// The talk window dialog. It lets the user communicate.
    /// User types in the text and presses ENTER to convert
    /// text to speech.  If word prediction learning is supported,
    /// also learns user writing style from the text entered in
    /// the talk window.
    /// </summary>
    [DescriptorAttribute("BF1C93D7-3962-4A52-A56D-668149D116AE", "TalkWindowForm", "Talk Window")]
    public partial class TalkWindowForm : TalkWindowBase
    {
        /// <summary>
        /// Thickness of the border around the form
        /// </summary>
        private const int BorderThickness = 2;

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
            dateTimeControl = labelDateTime;

            enableDateTimeDisplay = Common.AppPreferences.TalkWindowDisplayDateTimeEnable;
            displayDateFormat = Common.AppPreferences.TalkWindowDisplayDateFormat;
            displayTimeFormat = Common.AppPreferences.TalkWindowDisplayTimeFormat;

            ShowInTaskbar = false;
            MaximizeBox = false;
            Windows.ShowWindowBorder(this,
                                    Common.AppPreferences.TalkWindowShowBorder,
                                    Common.AppPreferences.TalkWindowShowTitleBar ? Text : String.Empty);

            if (Common.AppPreferences.TalkWindowShowTitleBar)
            {
                labelTalk.Text = String.Empty;
            }

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
                Invoke(new MethodInvoker(delegate()
                {
                    retVal = textBox.Text;
                }));

                return retVal;
            }

            set
            {
                Invoke(new MethodInvoker(delegate()
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
            Invoke(new MethodInvoker(delegate()
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
            Invoke(new MethodInvoker(delegate()
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
            Invoke(new MethodInvoker(delegate()
            {
                updateVolumeStatus();
            }));
        }

        /// <summary>
        /// Paint the border around the talk window
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void BorderPanel_Paint(object sender, PaintEventArgs e)
        {
            if (BorderPanel.BorderStyle == BorderStyle.FixedSingle)
            {
                const int thickness = BorderThickness;
                const int halfThickness = thickness / 2;
                using (var pen = new Pen(Color.Black, thickness))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        new Rectangle(halfThickness,
                        halfThickness,
                        BorderPanel.ClientSize.Width - thickness,
                        BorderPanel.ClientSize.Height - thickness));
                }
            }
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
        /// User clicked on the "Clear text" button in the talk window status bar
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void labelClearText_Click(object sender, EventArgs e)
        {
            Clear();
        }

        /// <summary>
        /// User clicked on the "Close" button in the talk indow
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void labelClose_Click(object sender, EventArgs e)
        {
            notifyRequestClose();
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
        /// Sets the intel logo on the talk window
        /// </summary>
        private void setIcon()
        {
            var intelIcon = FileUtils.GetImagePath("Intel-logo.png");
            if (File.Exists(intelIcon))
            {
                var image = Image.FromFile(FileUtils.GetImagePath("Intel-logo.png"));
                image = ImageUtils.ImageResize(image, lblIntelIcon.Width, lblIntelIcon.Height);
                lblIntelIcon.ImageAlign = ContentAlignment.MiddleCenter;
                lblIntelIcon.Image = image;
            }
            else
            {
                lblIntelIcon.Text = "&";
            }
        }

        /// <summary>
        /// Subscribes to the various events
        /// </summary>
        private void subscribeToEvents()
        {
            Load += TalkWindowForm_Load;
            FormClosing += TalkWindowForm_FormClosing;
            VisibleChanged += TalkWindowForm_VisibleChanged;
            BorderPanel.Paint += BorderPanel_Paint;
            TTSManager.Instance.ActiveEngine.EvtPropertyChanged += ActiveEngine_EvtPropertyChanged;
            TTSManager.Instance.EvtEngineChanged += Instance_EvtEngineChanged;
            labelSpeaker.Click += labelSpeaker_Click;
            textBox.KeyPress += textBox_KeyPress;
            trackBarVolume.MouseUp += trackBarVolume_MouseUp;
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

            TTSManager.Instance.ActiveEngine.EvtPropertyChanged -= new EventHandler(ActiveEngine_EvtPropertyChanged);
        }

        /// <summary>
        /// Called when loaded. Create timers, position the window
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
                    _acatFont = new Font(fontFamily, labelSpeaker.Font.Size);
                    labelSpeaker.Font = _acatFont;
                    labelClearText.Font = _acatFont;
                }
            }

            setFont(FontSize);
            updateVolumeStatus();
            CenterToScreen();
            _windowOverlapWatchdog = new WindowOverlapWatchdog(this);
            _windowActiveWatchdog = new WindowActiveWatchdog(this);
            textBox.SelectionStart = textBox.Text.Length;
            textBox.DeselectAll();
        }

        /// <summary>
        /// Visibility of the talk window changed
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
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Convert text to speech
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