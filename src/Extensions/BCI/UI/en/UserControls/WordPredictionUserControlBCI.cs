////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// WordPredictionUserControlBCI.cs
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
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.UI.UserControls
{
    [DescriptorAttribute("531C7B62-A09F-4772-8C7E-915E2C0AD015",
            "WordPredictionUserControl",
            "User Control for Word Prediction BCI")]

    public partial class WordPredictionUserControlBCI : UserControl, IUserControl
    {
        private static String _formConfigFilePath = "";
        private static UserControlConfigMapEntry _mapEntry;
        private UserControlKeyboardCommon _keyboardCommon;
        IScannerPanel _scanner;
        private UserControlWordPredictionCommon _wordPredictionCommon;
        public WordPredictionUserControlBCI()
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

            _wordPredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.Words });

            _scanner = scanner;

            bool retVal = _keyboardCommon.Initialize();

            if (retVal)
            {
                retVal = _wordPredictionCommon.Initialize(_keyboardCommon.RootWidget);
            }

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            return retVal;
        }

        public void OnLoad()
        {
            _keyboardCommon.OnLoad();

            _wordPredictionCommon.OnLoad();

            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public void OnPause()
        {

        }

        public void OnResume()
        {

        }
        public void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            _wordPredictionCommon.OnWidgetActuated(e, ref handled);

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
