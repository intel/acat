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

using ACAT.Core.AnimationManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.UserControlManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictionManagement;
using ACAT.Extension;
using ACAT.UserControls;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.UserControls
{
    [ClassDescriptor("531C7B62-A09F-4772-8C7E-915E2C0AD014",
                    "WordPredictionUserControl",
                    "User Control for Word Prediction")]
    /* NOTE: Put a slash at the start of this line to enable the designer view in Visual Studio. 
       // Remove the slash to run the code in the ACAT application. 
       // This allows you to edit the control in Visual Studio without having to run ACAT.
       // The designer view will not work when subclassing the GenericUserControl.
    public partial class WordPredictionUserControl : Form, IUserControl
    /*/
    public partial class WordPredictionUserControl : GenericUserControl, IUserControl
    //*/
    {
        private UserControlKeyboardCommon _keyboardCommon;
        private UserControlWordPredictionCommon _userControlWordPredictionCommon;

        public WordPredictionUserControl()
        {
            InitializeComponent();
        }

        public override bool Initialize(UserControlConfigMapEntry mapEntry, TextController textController, IScannerPanel scanner)
        {
            _keyboardCommon = new UserControlKeyboardCommon(this, mapEntry, textController, scanner);

            _userControlWordPredictionCommon = new UserControlWordPredictionCommon(this, textController, scanner, new PredictionTypes[] { PredictionTypes.Words });

            bool retVal = _keyboardCommon.Initialize();

            if (retVal)
            {
                retVal = _userControlWordPredictionCommon.Initialize(_keyboardCommon.RootWidget);
            }

            _keyboardCommon.AnimationManager.EvtPlayerStateChanged += AnimationManager_EvtPlayerStateChanged;

            return retVal;
        }

        public override void OnLoad()
        {
            _keyboardCommon.OnLoad();

            _userControlWordPredictionCommon.OnLoad();

            _keyboardCommon.AnimationManager.OnLoad(_keyboardCommon.RootWidget);
        }

        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            _userControlWordPredictionCommon.OnWidgetActuated(e, ref handled);
        }
    }
}