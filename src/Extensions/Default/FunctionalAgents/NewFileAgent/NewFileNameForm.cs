////////////////////////////////////////////////////////////////////////////
// <copyright file="NewFileNameForm.cs" company="Intel Corporation">
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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.ThemeManagement;
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Extensions.Hawking.FunctionalAgents.NewFile
{
    /// <summary>
    /// Form that lets the user enter a file name for a new
    /// file. As the user is entering characters, it checks
    /// if the file exists and visually indicates that the
    /// file exists.
    /// </summary>
    public partial class NewFileNameForm : Form
    {
        /// <summary>
        /// Is the filename entered a valid one?
        /// </summary>
        private bool _isValid;

        /// <summary>
        /// Ensures the file name dialog stays in focus
        /// </summary>
        private WindowActiveWatchdog _windowActiveWatchdog;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public NewFileNameForm()
        {
            InitializeComponent();
            CreateFileType = FileType.Text;
            FileToCreate = String.Empty;

            Load += NewFileNameForm_Load;
            FormClosing += NewFileNameForm_FormClosing;
            Context.AppPanelManager.EvtScannerShow += AppPanelManager_EvtScannerShow;
            textBoxFileName.KeyPress += textBoxFileName_KeyPress;

            var colorScheme = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(ColorSchemes.ScannerSchemeName);
            BackColor = colorScheme.Background;
            ForeColor = colorScheme.Foreground;
        }

        /// <summary>
        /// For the event indicating that the user is done
        /// entering the filename
        /// </summary>
        /// <param name="flag">prompt the user?</param>
        public delegate void DoneEvent(bool flag);

        /// <summary>
        /// Raised when the user is done
        /// </summary>
        public event DoneEvent EvtDone;

        /// <summary>
        /// What type of file to create?
        /// </summary>
        public enum FileType
        {
            Text,
            Word
        }

        /// <summary>
        /// Gets or sets the directory in which to create the file
        /// </summary>
        public String CreateFileDirectory { get; set; }

        /// <summary>
        /// Gets or sets the type of file to create
        /// </summary>
        public FileType CreateFileType { get; set; }

        /// <summary>
        /// Gets or sets whether the entered filename already exists
        /// </summary>
        public bool FileExists { get; set; }

        /// <summary>
        /// Gets the name of the file entered so far in
        /// the text box
        /// </summary>
        public String FileNameEntered
        {
            get { return Windows.GetText(textBoxFileName); }
        }

        /// <summary>
        /// Gets the name of the file the user entered
        /// </summary>
        public String FileToCreate { get; private set; }

        /// <summary>
        /// Clears the text box that has the file name
        /// </summary>
        public void ClearFileName()
        {
            Windows.SetText(textBoxFileName, String.Empty);
        }

        /// <summary>
        /// Checks if the filename entered is valid or not
        /// </summary>
        /// <returns>true if it is</returns>
        public bool ValidNameSpecified()
        {
            try
            {
                var fileName = (!String.IsNullOrEmpty(FileToCreate)) ? Path.GetFileName(FileToCreate) : String.Empty;

                return _isValid &&
                        !String.IsNullOrEmpty(FileToCreate) &&
                        fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                        !FileExists;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// Invoked when the companion scanner is shown. Dock
        /// this form to the scanner
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="arg">args</param>
        private void AppPanelManager_EvtScannerShow(object sender, ScannerShowEventArg arg)
        {
            if (arg.Scanner != this)
            {
                dockToScanner(arg.Scanner.Form);
            }
        }

        /// <summary>
        /// Dock this form to the companion scanner
        /// </summary>
        /// <param name="scanner">companion scanner</param>
        private void dockToScanner(Form scanner)
        {
            if (scanner is IScannerPanel)
            {
                Windows.DockWithScanner(this, scanner, Context.AppWindowPosition);
            }
        }

        /// <summary>
        /// Checks if the user entered valid characters for the file name
        /// </summary>
        /// <param name="fileName">name of the file</param>
        /// <returns>true if it is</returns>
        private bool hasValidCharacters(String fileName)
        {
            var invalidChars = new char[] { '\\', '/', ':', '*', '?', '<', '>', '|', '"' };
            return !String.IsNullOrEmpty(fileName) && fileName.IndexOfAny(invalidChars) < 0;
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void NewFileNameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_windowActiveWatchdog != null)
            {
                _windowActiveWatchdog.Dispose();
            }

            Context.AppPanelManager.EvtScannerShow -= new ScannerShow(AppPanelManager_EvtScannerShow);
        }

        /// <summary>
        /// Form is loaded. Initialize
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void NewFileNameForm_Load(object sender, EventArgs e)
        {
            _windowActiveWatchdog = new WindowActiveWatchdog(this);
            Shown += NewFileNameForm_Shown;
            textBoxFileName.TextChanged += new EventHandler(textBoxFileName_TextChanged);
        }

        /// <summary>
        /// Form is visible. Set focus to it.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void NewFileNameForm_Shown(object sender, EventArgs e)
        {
            Windows.SetForegroundWindow(this.Handle);
            Windows.ClickOnWindow(this);
        }

        /// <summary>
        /// Key press event handler
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void textBoxFileName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (EvtDone == null) return;

            int c = e.KeyChar;

            if (c == 13)
            {
                if (ValidNameSpecified())
                {
                    EvtDone.BeginInvoke(true, null, null);
                }
            }
            else if (c == 27)
            {
                EvtDone.BeginInvoke(false, null, null);
            }
        }

        /// <summary>
        /// The textbox containing the filename changed. Check
        /// if the name is valid. Depending on the type of file,
        /// automatically append the extension. Visually indicate
        /// if the file already exists.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void textBoxFileName_TextChanged(object sender, EventArgs e)
        {
            var text = textBoxFileName.Text.Trim();
            var displayFileName = text;

            FileExists = false;

            if (String.IsNullOrEmpty(text))
            {
                labelNameOfFile.Text = String.Empty;
                FileToCreate = String.Empty;
                return;
            }

            _isValid = hasValidCharacters(text);

            if (!_isValid)
            {
                return;
            }

            var fileName = Path.Combine(CreateFileDirectory, text);

            fileName = fileName.Trim();

            FileExists = File.Exists(fileName);

            if (!FileExists)
            {
                if (fileName.EndsWith("."))
                {
                    fileName = fileName.Substring(0, fileName.Length - 1);
                }

                switch (CreateFileType)
                {
                    case FileType.Word:
                        if (!fileName.ToLower().EndsWith(".docx") && !fileName.ToLower().EndsWith(".doc"))
                        {
                            FileExists = File.Exists(fileName + ".docx") || File.Exists(fileName + ".doc");
                            displayFileName = Path.GetFileName(fileName + ".docx");
                        }

                        break;

                    case FileType.Text:
                        if (!fileName.ToLower().EndsWith(".txt"))
                        {
                            FileExists = File.Exists(fileName + ".txt");
                            displayFileName = Path.GetFileName(fileName + ".txt");
                        }
                        else
                        {
                            FileExists = File.Exists(fileName.Substring(0, fileName.Length - 4));
                        }

                        break;
                }
            }

            labelNameOfFile.Text = displayFileName;

            FileToCreate = !FileExists ? Path.Combine(CreateFileDirectory, displayFileName) : String.Empty;
            Log.Debug("File: " + FileToCreate);
            labelNameOfFile.ForeColor = FileExists ? Color.Red : Color.Green;
            Windows.SetText(labelPrompt, FileExists ? "File already exists" : String.Empty);
        }
    }
}