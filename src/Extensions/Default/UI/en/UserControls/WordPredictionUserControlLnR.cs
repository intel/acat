////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WordPredictionUserControl.cs
//
// User control that displays the next word prediction words
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AnimationManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserControlManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Core.WordPredictionManagement;
using ACAT.Lib.Extension;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.UserControls
{
    [DescriptorAttribute("D365D938-C247-4DC4-95E8-FABE8488204E",
                    "WordPredictionUserControlLnR",
                    "User Control for Word Prediction")]
    public partial class WordPredictionUserControlLnR : UserControl, IUserControl
    {
        private UserControlKeyboardCommon _keyboardCommon;
        private UserControlWordPredictionCommon _userControlWordPredictionCommon;

        public WordPredictionUserControlLnR()
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

            _userControlWordPredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.Words, PredictionTypes.Keywords });

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