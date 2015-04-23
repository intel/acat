////////////////////////////////////////////////////////////////////////////
// <copyright file="ACATGettingStartedForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// Lets the user get used to using the swtich trigger mechanism
    /// and interact with the UI.
    /// A simple dialog that lets the enter a few words.
    /// Four letters are displayed, words are made up of these
    /// letters.
    /// </summary>
    [DescriptorAttribute("CE2553C4-8055-4CAC-BAE0-4C1A0E8E8774", "ACATGettingStartedForm",
                            "Getting started with ACAT")]
    public partial class ACATGettingStartedForm : Form, IDialogPanel
    {
        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private readonly DialogCommon _dialogCommon;

        /// <summary>
        /// List of words to try.
        /// </summary>
        private readonly String[] words = { "tea", "eat", "ate", "tab", "bet", "bat", "beet" };

        /// <summary>
        /// Which word are we trying now?
        /// </summary>
        private int _index;

        /// <summary>
        /// Initializes an instance of hte class
        /// </summary>
        public ACATGettingStartedForm()
        {
            InitializeComponent();

            textBoxEntry.TextChanged += TextBoxEntryOnTextChanged;
            _dialogCommon = new DialogCommon(this);

            if (!_dialogCommon.Initialize())
            {
                return;
            }

            setNextWord();

            Load += AsterLaunchpad_Load;
            FormClosing += AsterLaunchpad_FormClosing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the sync object
        /// </summary>
        public SyncLock SyncObj
        {
            get { return _dialogCommon.SyncObj; }
        }

        /// <summary>
        /// Gets window styles
        /// </summary>
        protected override CreateParams CreateParams
        {
            get { return DialogCommon.SetFormStyles(base.CreateParams); }
        }

        /// <summary>
        /// Triggered when a widget is actuated.
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            bool quit = false;

            Log.Debug();

            Invoke(new MethodInvoker(delegate()
            {
                switch (widget.Name)
                {
                    case "B1":
                        textBoxEntry.Text = textBoxEntry.Text + widget.Value;
                        break;

                    case "B2":
                        textBoxEntry.Text = textBoxEntry.Text + widget.Value;
                        break;

                    case "B3":
                        textBoxEntry.Text = textBoxEntry.Text + widget.Value;
                        break;

                    case "B4":
                        textBoxEntry.Text = textBoxEntry.Text + widget.Value;
                        break;

                    case "buttonBackspace": // delete last letter
                        if (textBoxEntry.Text.Length == 1)
                        {
                            textBoxEntry.Text = string.Empty;
                        }
                        else if (textBoxEntry.Text.Length > 1)
                        {
                            textBoxEntry.Text = textBoxEntry.Text.Substring(0, textBoxEntry.Text.Length - 1);
                        }

                        break;

                    case "buttonReset":
                        clearTextBox();
                        break;

                    case "buttonExit":
                        quit = true;
                        break;
                }

                if (quit)
                {
                    if (DialogUtils.Confirm("Exit?"))
                    {
                        Context.AppQuit = true;
                        Windows.CloseForm(this);
                    }
                }
            }));
        }

        /// <summary>
        /// Pauses animation
        /// </summary>
        public void OnPause()
        {
            _dialogCommon.OnPause();
            Windows.SetVisible(this, false);
        }

        /// <summary>
        /// Resumes animation
        /// </summary>
        public void OnResume()
        {
            _dialogCommon.OnResume();
            Windows.SetTopMost(this);
            Windows.SetVisible(this, true);
        }

        /// <summary>
        /// Runs the specified command
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <param name="handled">was it handled?</param>
        public void OnRunCommand(string command, ref bool handled)
        {
            switch (command)
            {
                default:
                    handled = false;
                    break;
            }
        }

        /// <summary>
        /// Window proc.
        /// </summary>
        /// <param name="m">window message</param>
        protected override void WndProc(ref Message m)
        {
            _dialogCommon.HandleWndProc(m);
            base.WndProc(ref m);
        }

        /// <summary>
        /// Form is closing. Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AsterLaunchpad_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Start animation
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void AsterLaunchpad_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();
            _dialogCommon.GetAnimationManager().Start(_dialogCommon.GetRootWidget());
        }

        /// <summary>
        /// Clears the text box
        /// </summary>
        private void clearTextBox()
        {
            textBoxEntry.Text = String.Empty;
        }

        /// <summary>
        /// Asynchronously resumes animation
        /// </summary>
        private void resumeAnimationAsync()
        {
            Task.Run(() =>
            {
                Thread.Sleep(1000);

                _dialogCommon.GetAnimationManager().Resume();
            });
        }

        /// <summary>
        /// Picks the next word from the list and displays
        /// it as the prompt for the user
        /// </summary>
        private void setNextWord()
        {
            labelTryWord.Text = words[_index];
            _index = (_index + 1) % words.Length;
        }

        /// <summary>
        /// Something changed in the text box. Check if the user
        /// typed the word successfuly, if so, clear the word and
        /// restart animation
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event arg</param>
        private void TextBoxEntryOnTextChanged(object sender, EventArgs eventArgs)
        {
            var str = textBoxEntry.Text.Trim();
            if (string.Compare(str, labelTryWord.Text, true) == 0)
            {
                _dialogCommon.GetAnimationManager().Pause();

                Opacity = 0.75f;
                DialogUtils.Toast("Good.", 500);
                Opacity = 1.0f;

                clearTextBox();

                setNextWord();

                resumeAnimationAsync();
            }
        }
    }
}