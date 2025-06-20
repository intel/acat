////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PhrasesUserControlBCI.cs
//
// User control that displays the sentence predictions.  Also displays
// phrases in the "PHRASES" mode and converts the phrase to speech when
// selected
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement;
using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictionManagement;
using ACAT.Extension;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.UI.UserControls
{
    [Descriptor("05011FAB-2725-4DCE-BFFE-5EA0F8E11F62",
        "SentencesUserControl",
        "User Control for Sentence Prediction BCI")]
    public partial class PhrasesUserControlBCI : UserControl, IUserControl
    {
        private static String _formConfigFilePath = "";
        private static UserControlConfigMapEntry _mapEntry;
        private UserControlKeyboardCommon _keyboardCommon;
        IScannerPanel _scanner;
        private UserControlWordPredictionCommon _sentencePredictionCommon;
        public PhrasesUserControlBCI()
        {
            InitializeComponent();
        }

        public event AnimationPlayerStateChanged EvtPlayerStateChanged;
        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the snchronization object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _keyboardCommon.SyncObj; }
        }

        public IUserControlCommon UserControlCommon
        {
            get
            {
                return _keyboardCommon;
            }
        }

        public static string getpathConfigFile()
        {
            try
            {
                if (_mapEntry != null)
                    _formConfigFilePath = _mapEntry.ConfigFileName;

            }
            catch (Exception)
            {

            }
            return _formConfigFilePath;
        }

        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _mapEntry = mapEntry;

            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            _sentencePredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.Sentences});

            _scanner = scanner;

            bool retVal = _keyboardCommon.Initialize();

            if (retVal)
            {
                retVal = _sentencePredictionCommon.Initialize(_keyboardCommon.RootWidget);
            }

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            return retVal;
        }

        public void OnLoad()
        {
            _keyboardCommon.OnLoad();

            _sentencePredictionCommon.OnLoad();

            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public void OnPause()
        {
            _keyboardCommon.OnPause();
        }

        public void OnResume()
        {
            _keyboardCommon.OnResume();
        }

        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)
            {
                _sentencePredictionCommon.OnWidgetActuated(e, ref handled);
            }
            else
            {
                ttsAndLearn(e.SourceWidget.Value);
            }

        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            EvtPlayerStateChanged?.Invoke(this, e);
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

        private void ttsAndLearn(String text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            textToSpeech(text);

            if (WordPredictionManager.Instance.ActiveWordPredictor.SupportsLearning)
            {
                WordPredictionManager.Instance.ActiveWordPredictor.Learn(text, WordPredictorMessageTypes.LearnCanned);
            }
        }
    }
}
