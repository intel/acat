////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// SentencePredictionUserControl.cs
//
// User control that displays the sentence predictions.  Also displays
// phrases in the "PHRASES" mode and converts the phrase to speech when
// selected
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Audit;
using ACAT.Core.PanelManagement;
using ACAT.Core.TTSManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictorManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extension.UI;
using ACAT.Extension.UI.UserControls;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("07E0D588-1E80-4A07-BC26-FA4C8BCF5589",
                    "SentencePredictionUserControl",
                    "User Control for Sentence Prediction")]
    public partial class SentencePredictionUserControl : KeyboardUserControl
    {
        private UserControlWordPredictionCommon _userControlWordPredictionCommon;

        public SentencePredictionUserControl()
        {
            InitializeComponent();
        }

        protected override bool HandleInitialize()
        {
            _userControlWordPredictionCommon = new UserControlWordPredictionCommon(this, _keybordUserControlCommon.TextController, _keybordUserControlCommon.ScannerPanel, new PredictionTypes[] { PredictionTypes.Sentences }, null);

            bool retVal = _userControlWordPredictionCommon.Initialize(_keybordUserControlCommon.RootWidget);
            return retVal;
        }

        public override void OnLoad()
        {
            base.OnLoad();

            _userControlWordPredictionCommon.OnLoad();
        }


        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() != WordPredictionModes.CannedPhrases)
            {
                _userControlWordPredictionCommon.OnWidgetActuated(e, ref handled);
            }
            else
            {
                ttsAndLearn(e.SourceWidget.Value);
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
                _logger.LogDebug("*** TTS *** : {Text}", text);
                TTSManager.Instance.ActiveEngine.Speak(text);
                _logger.LogDebug("*** TTS *** : sent text!");

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