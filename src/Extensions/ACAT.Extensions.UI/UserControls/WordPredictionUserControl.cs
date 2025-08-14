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

using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictorManagement;
using ACAT.Extension.UI;
using ACAT.Extension.UI.UserControls;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("531C7B62-A09F-4772-8C7E-915E2C0AD014",
                    "WordPredictionUserControl",
                    "User Control for Word Prediction")]
    public partial class WordPredictionUserControl : KeyboardUserControl
    {
        private UserControlWordPredictionCommon _userControlWordPredictionCommon;

        public WordPredictionUserControl()
        {
            InitializeComponent();
        }

        public override bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            base.Initialize(mapEntry, textController, scanner);

            _userControlWordPredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.Words });
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
            _userControlWordPredictionCommon.OnWidgetActuated(e, ref handled);
        }
    }
}