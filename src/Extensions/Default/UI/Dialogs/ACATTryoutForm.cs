///////////////////////////////////////////////////////////////////////////
// <copyright file="ACATryoutForm.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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

using ACAT.ACATResources;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Dialogs
{
    /// <summary>
    /// Enables the user get used to using the swtich trigger mechanism
    /// and interact with the UI.  A simple dialog that lets the enter a few words.
    /// Four letters are displayed, words are made up of combinations of these
    /// letters. The scanning speed can also be adjusted to suit the user's
    /// preference.
    /// A nice way to get the user to get started on ACAT
    /// </summary>
    [DescriptorAttribute("CE2553C4-8055-4CAC-BAE0-4C1A0E8E8774",
                        "ACATTryoutForm",
                        "A Tryout form to get started with ACAT")]
    public partial class ACATTryoutForm : Form, IDialogPanel
    {
        /// <summary>
        /// By how much to increase/decrease the stepping time
        /// </summary>
        private const int StepSize = 50;

        /// <summary>
        /// List of words to try.
        /// </summary>
        private readonly String[] words =
        {
            R.GetString("TryoutWord1"),
            R.GetString("TryoutWord2"),
            R.GetString("TryoutWord3"),
            R.GetString("TryoutWord4"),
            R.GetString("TryoutWord5"),
            R.GetString("TryoutWord6"),
            R.GetString("TryoutWord7"),
        };

        /// <summary>
        /// The DialogCommon object
        /// </summary>
        private DialogCommon _dialogCommon;

        /// <summary>
        /// Which word are we trying now?
        /// </summary>
        private int _index;

        /// <summary>
        /// Initializes an instance of hte class
        /// </summary>
        public ACATTryoutForm()
        {
            InitializeComponent();

            label1.Text = R.GetString("TryoutTypeThis") + ":";
            buttonSave.Text = R.GetString("Save");
            buttonSteppingTimeIncrease.Text = R.GetString("Faster");
            buttonSteppingTimeDecrease.Text = R.GetString("Slower");
            toolStripLabel1.Text = R.GetString("ScanSpeed");

            updateToolbar();

            textBoxEntry.TextChanged += TextBoxEntryOnTextChanged;

            setNextWord();

            Load += ACATTryoutForm_Load;
            FormClosing += ACATTryoutForm_FormClosing;
        }

        /// <summary>
        /// Gets the descriptor for this class
        /// </summary>
        public IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets the PanelCommon object
        /// </summary>
        public IPanelCommon PanelCommon { get { return _dialogCommon; } }

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
        /// Intitializes the class
        /// </summary>
        /// <param name="startupArg">startup param</param>
        /// <returns>true on success</returns>
        public bool Initialize(StartupArg startupArg)
        {
            _dialogCommon = new DialogCommon(this);
            return _dialogCommon.Initialize(startupArg);
        }

        /// <summary>
        /// Triggered when a widget is actuated.
        /// </summary>
        /// <param name="widget">Which one triggered?</param>
        public void OnButtonActuated(Widget widget)
        {
            bool quit = false;

            Log.Debug();

            Invoke(new MethodInvoker(delegate
            {
                switch (widget.Name)
                {
                    case "B1":
                        setText(textBoxEntry.Text + widget.Value);
                        break;

                    case "B2":
                        setText(textBoxEntry.Text + widget.Value);
                        break;

                    case "B3":
                        setText(textBoxEntry.Text + widget.Value);
                        break;

                    case "B4":
                        setText(textBoxEntry.Text + widget.Value);
                        break;

                    case "buttonBackspace": // delete last letter
                        if (textBoxEntry.Text.Length == 1)
                        {
                            setText(String.Empty);
                        }
                        else if (textBoxEntry.Text.Length > 1)
                        {
                            setText(textBoxEntry.Text.Substring(0, textBoxEntry.Text.Length - 1));
                        }

                        break;

                    case "buttonReset":
                        setText(String.Empty);
                        break;

                    case "buttonExit":
                        quit = true;
                        break;
                }

                if (quit)
                {
                    if (DialogUtils.Confirm(R.GetString("ExitQuestion")))
                    {
                        Context.AppQuit = true;
                        Windows.CloseForm(this);
                    }
                    else
                    {
                        textBoxEntry.Focus();
                    }
                }
            }));
        }

        /// <summary>
        /// Sets the text in the textbox and sets caret position
        /// </summary>
        /// <param name="text">text to set</param>
        private void setText(String text)
        {
            textBoxEntry.Text = text;
            textBoxEntry.SelectionStart = textBoxEntry.Text.Length;
            textBoxEntry.ScrollToCaret();
            textBoxEntry.Focus();
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
        private void ACATTryoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Context.AppQuit = true;
            _dialogCommon.OnClosing();
        }

        /// <summary>
        /// Form has been loaded. Start animation
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void ACATTryoutForm_Load(object sender, EventArgs e)
        {
            _dialogCommon.OnLoad();
            PanelCommon.AnimationManager.Start(PanelCommon.RootWidget);
        }

        /// <summary>
        /// Event handler to save settings
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            var prefs = ACATPreferences.Load();

            prefs.ScanTime = Common.AppPreferences.ScanTime;

            prefs.Save();
        }

        /// <summary>
        /// Make scanning slower
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonSteppingTimeDecrease_Click(object sender, EventArgs e)
        {
            PanelCommon.AnimationManager.Interrupt();

            Common.AppPreferences.ScanTime += StepSize;

            Common.AppPreferences.NotifyPreferencesChanged();

            updateToolbar();
        }

        /// <summary>
        /// Make scanning faster
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void buttonSteppingTimeIncrease_Click(object sender, EventArgs e)
        {
            PanelCommon.AnimationManager.Interrupt();

            Common.AppPreferences.ScanTime = Math.Max(50, Common.AppPreferences.ScanTime - StepSize);

            Common.AppPreferences.NotifyPreferencesChanged();

            updateToolbar();
        }

        /// <summary>
        /// Clears the text box
        /// </summary>
        private void clearTextBox()
        {
            
        }

        /// <summary>
        /// Asynchronously resumes animation
        /// </summary>
        private void resumeAnimationAsync()
        {
            Task.Run(() =>
            {
                Thread.Sleep(1000);

                PanelCommon.AnimationManager.Resume();
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
                PanelCommon.AnimationManager.Pause();

                Opacity = 0.75f;
                DialogUtils.Toast(R.GetString("Good"), 500);
                Opacity = 1.0f;

                setText(String.Empty);

                setNextWord();

                resumeAnimationAsync();
            }
        }

        /// <summary>
        /// Updates toolbar with current status
        /// </summary>
        private void updateToolbar()
        {
            labelSteppingTimeValue.Text = "(" + Common.AppPreferences.ScanTime + " " + R.GetString("Msecs") + ")";
        }
    }
}