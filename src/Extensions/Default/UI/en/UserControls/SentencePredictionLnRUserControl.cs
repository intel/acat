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
    [DescriptorAttribute("C98E7824-A17E-4F96-B87F-9524474E2178",
                    "SentencePredictionLnRUserControl",
                    "User Control for Sentence Prediction")]
    public partial class SentencePredictionLnRUserControl : UserControl, IUserControl
    {
        private UserControlKeyboardCommon _keyboardCommon;
        private UserControlWordPredictionCommon _userControlWordPredictionCommon;

        public SentencePredictionLnRUserControl()
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

            _userControlWordPredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.LnRResponses});

            bool retVal = _keyboardCommon.Initialize();

            if (retVal)
            {
                retVal = _userControlWordPredictionCommon.Initialize(_keyboardCommon.RootWidget);
            }

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

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
            _userControlWordPredictionCommon.OnWidgetActuated(e, ref handled);
        }

        private void AnimationManager_EvtPlayerStateChanged(object sender, PlayerStateChangedEventArgs e)
        {
            if (EvtPlayerStateChanged != null)
            {
                EvtPlayerStateChanged(this, e);
            }
        }
    }
}