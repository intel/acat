////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Core.WordPredictorManagement.Interfaces;
using ACAT.Extension.UI;
using System;

namespace ACAT.Extension
{
    /// <summary>
    /// Base class for all horizontal strip scanners.  This
    /// is a scanner with a single row of buttons.
    /// The width of the scanner is dynamically
    /// computed depending on how many menu items are there
    /// </summary>
    [ClassDescriptor("954E418F-B206-457C-B7A5-7C32B4D85E76",
                    "WordPredictionSetModeScanner",
                    "Set the word prediction mode")]
    public partial class WordPredictionSetModeScanner : HorizontalStripScanner
    {
        /// <summary>
        /// The command dispatcher.  If the derived class as additional
        /// commands, just call Commands.Add on this object
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Current word prediction mode
        /// </summary>
        private readonly WordPredictionModes _mode;

        /// <summary>
        /// The root widget representing this scanner form
        /// </summary>
        //private Widget _rootWidget;

        /// <summary>
        /// ScannerCommon object for all the heavy lifting
        /// </summary>
        //private ScannerCommon _scannerCommon;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Scanner class</param>
        /// <param name="title">Title of the scanner</param>
        public WordPredictionSetModeScanner(String panelClass, String title) : base(panelClass, title)
        {
            InitializeComponent();
            _dispatcher = new Dispatcher(this);

            Text = title;

            _mode = Context.AppWordPredictionManager.ActiveWordPredictor.GetMode();
        }

        /// <summary>
        /// Called to check if the specified widget in arg should
        /// be enabled or not.  This function is called periodically
        /// because application context may change any time. Set
        /// the handled property in arg to true if this is handled.
        /// </summary>
        /// <param name="arg">argument</param>
        /// <returns>true on success</returns>
        public override bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            switch (arg.Command)
            {
                case "CmdTypingModeSentence":
                    arg.Enabled = _mode != WordPredictionModes.Sentence;
                    arg.Handled = true;
                    break;

                case "CmdTypingModePhrase":
                    arg.Enabled = _mode != WordPredictionModes.CannedPhrases;
                    arg.Handled = true;
                    break;

                case "CmdTypingModeShortHand":
                    arg.Enabled = _mode != WordPredictionModes.Shorthand;
                    arg.Handled = true;
                    break;

                default:
                    arg.Handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Invoked when the user actuates a button in
        /// the scanner form
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled here?</param>
        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            String prompt = "Switch typing mode to ";

            WordPredictionModes mode;

            if (e.SourceWidget.Value == "@CmdTypingModeSentence")
            {
                mode = WordPredictionModes.Sentence;
                prompt += "SENTENCE?";
            }
            else if (e.SourceWidget.Value == "@CmdTypingModePhrase")
            {
                mode = WordPredictionModes.CannedPhrases;
                prompt += "CANNED PHRASE?";
            }
            else if (e.SourceWidget.Value == "@CmdTypingModeShortHand")
            {
                mode = WordPredictionModes.Shorthand;
                prompt += "SHORTHAND?";
            }
            else
            {
                mode = WordPredictionModes.None;
                prompt = String.Empty;
            }

            if (!String.IsNullOrEmpty(prompt))
            {
                if (DialogUtils.ConfirmScanner(this, prompt))
                {
                    Context.AppWordPredictionManager.ActiveWordPredictor.SetMode(mode);
                    Windows.CloseAsync(this);
                }
            }

            handled = false;
        }
    }
}