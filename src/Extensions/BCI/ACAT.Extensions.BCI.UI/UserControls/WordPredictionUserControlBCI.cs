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

using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement;
using ACAT.Core.UserControlManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictorManagement;
using ACAT.Extension.UI;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.BCI.UI.UserControls
{
    [ClassDescriptorAttribute("531C7B62-A09F-4772-8C7E-915E2C0AD015",
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
        public ClassDescriptorAttribute Descriptor
        {
            get { return ClassDescriptorAttribute.GetDescriptor(GetType()); }
        }

        public Guid Id => Descriptor.Id;

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

            _wordPredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.Words }, null);

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
            EvtPlayerStateChanged?.Invoke(this, e);
        }
    }
}
