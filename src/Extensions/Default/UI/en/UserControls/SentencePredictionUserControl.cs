﻿////////////////////////////////////////////////////////////////////////////
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

using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.Audit;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.TTSManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.Widgets;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.UserControls
{
    [DescriptorAttribute("07E0D588-1E80-4A07-BC26-FA4C8BCF5589",
                    "SentencePredictionUserControl",
                    "User Control for Sentence Prediction")]
    public partial class SentencePredictionUserControl : UserControl, IUserControl
    {
        private UserControlKeyboardCommon _keyboardCommon;
        private UserControlWordPredictionCommon _userControlWordPredictionCommon;

        public SentencePredictionUserControl()
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

        public bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            _userControlWordPredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.Sentences });

            bool retVal = _keyboardCommon.Initialize();

            if (retVal)
            {
                retVal = _userControlWordPredictionCommon.Initialize(_keyboardCommon.RootWidget);
            }

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            Context.AppWordPredictionManager.ActiveWordPredictor.EvtModeChanged += ActiveWordPredictor_EvtModeChanged;

            setColorScheme();

            return retVal;
        }


        public void OnLoad()
        {
            _keyboardCommon.OnLoad();

            _userControlWordPredictionCommon.OnLoad();

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
                _userControlWordPredictionCommon.OnWidgetActuated(e, ref handled);
            }
            else
            {
                ttsAndLearn(e.SourceWidget.Value);
            }
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            if (EvtPlayerStateChanged != null)
            {
                EvtPlayerStateChanged(this, e);
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

        private void ActiveWordPredictor_EvtModeChanged(WordPredictionModes newMode)
        {
            setColorScheme();
        }

        private void setColorScheme()
        {
            List<Widget> widgets = new List<Widget>();
            _keyboardCommon.RootWidget.Finder.FindAllChildren(typeof(SentenceListItemWidget), widgets);

            if (Context.AppWordPredictionManager.ActiveWordPredictor.GetMode() == WordPredictionModes.CannedPhrases)
            {
                foreach (Widget widget in widgets)
                {
                    widget.Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.PhraseSchemeName);
                    widget.HighlightOff();
                }
            }
            else
            {

                foreach (Widget widget in widgets)
                {
                    widget.Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.ScannerButtonSchemeName);
                    widget.HighlightOff();
                }
            }   
        }
    }
}