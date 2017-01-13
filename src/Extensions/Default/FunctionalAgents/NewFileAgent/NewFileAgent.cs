////////////////////////////////////////////////////////////////////////////
// <copyright file="NewFileAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;

namespace ACAT.Extensions.Default.FunctionalAgents.NewFile
{
    /// <summary>
    /// Functional agent that allows the user to create a new
    /// text file or a word document.  The file is created in a
    /// configurable folder. A check is made to see if the file
    /// already exists.  The agent then creates the file and quits
    /// so the user can start interacting with the application (notepad
    /// or ms word)
    /// </summary>
    [DescriptorAttribute("91390B32-8C7F-49DE-937E-E0BE3FF224F7",
                        "File Creator",
                        "NewFileAgent",
                        "Create new text/Word documents")]
    internal class NewFileAgent : FunctionalAgentBase

    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static CreateFileSettings Settings;

        /// <summary>
        /// Is MS Word installed on the user's computer?
        /// </summary>
        private readonly bool _isWordInstalled;

        /// <summary>
        /// Which buttons in the alphabet scanner do we support?
        /// </summary>
        private readonly String[] _supportedCommands =
        {
            "Back",
            "CmdShiftKey"
        };

        /// <summary>
        /// Contextual menu for the new file form
        /// </summary>
        private FileChoiceMenu _fileChoiceMenu;

        /// <summary>
        /// The form that allows the user to enter the name of the file
        /// </summary>
        private NewFileNameForm _newFileNameForm;
        /// <summary>
        /// Determines whether the alphabet scanner has been
        /// shown or not
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "CreateFileSettings.xml";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public NewFileAgent()
        {
            Name = "New File Agent";

            CreateFileSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);

            if (!File.Exists(CreateFileSettings.PreferencesFilePath))
            {
                Settings = CreateFileSettings.Load();
                Settings.NewTextFileCreateFolder = Common.AppPreferences.NewTextFileCreateFolder;
                Settings.NewWordDocCreateFolder = Common.AppPreferences.NewWordDocCreateFolder;

                Settings.Save();
            }
            else
            {
                Settings = CreateFileSettings.Load();
            }

            try
            {
                // Check if word is installed on the user's computer and
                // is < MS Word 2013
                var type = Type.GetTypeFromProgID("Word.Application");
                if (type != null)
                {
                    dynamic wordObject = Activator.CreateInstance(type);
                    if (wordObject != null)
                    {
                        //_isWordInstalled = (wordObject.Version == "14.0");
                        _isWordInstalled = true;
                        wordObject.Quit(false);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a text file should be created
        /// </summary>
        public bool CreateTextFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a word doc should be created
        /// </summary>
        public bool CreateWordDoc { get; set; }

        /// <summary>
        /// Invoked when the Functional agent is activated.  This is
        /// the entry point into this agent.
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Activate()
        {
            IsClosing = false;
            ExitCode = CompletionCode.ContextSwitch;
            IsActive = true;

            if (CreateTextFile)
            {
                showNewFileDialogForTextFile();
            }
            else if (CreateWordDoc)
            {
                showNewFileDialogForWordDoc();
            }
            else
            {
                var choice = showFileChoices();
                switch (choice)
                {
                    case "TextFile":
                        if (checkValidOrCreate(Settings.NewTextFileCreateFolder))
                        {
                            showNewFileDialogForTextFile();
                        }
                        else
                        {
                            ExitCode = CompletionCode.None;
                            handleQuit(false);
                        }

                        break;

                    case "WordDoc":
                        if (checkValidOrCreate(Settings.NewWordDocCreateFolder))
                        {
                            showNewFileDialogForWordDoc();
                        }
                        else
                        {
                            ExitCode = CompletionCode.None;
                            handleQuit(false);
                        }

                        break;

                    default:
                        ExitCode = CompletionCode.None;
                        handleQuit(false);
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            switch (arg.Command)
            {
                case "CmdToolsMenu":
                case "CmdMainMenu":
                case "CmdMouseScanner":
                case "CmdCursorScanner":
                case "Tab":
                case "CmdAltKey":
                case "CmdCtrlKey":
                case "CmdContextMenu":
                case "CmdPrevPage":
                case "CmdNextPage":
                case "CmdFunctionKeyScanner":
                case "CmdWindowPosSizeMenu":
                case "CmdTalkWindowToggle":
                    arg.Handled = true;
                    arg.Enabled = false;
                    break;

                case "Back":
                case "CmdDeletePrevWord":
                    arg.Handled = true;
                    arg.Enabled = _newFileNameForm != null && !String.IsNullOrEmpty(_newFileNameForm.FileNameEntered);
                    return;

                case "CmdEnterKey":
                    arg.Handled = true;
                    arg.Enabled = _newFileNameForm != null && _newFileNameForm.ValidNameSpecified();
                    return;

                case "WordDoc":
                    arg.Enabled = _isWordInstalled;
                    arg.Handled = true;
                    break;

                case "TextFile":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
            }

            checkCommandEnabled(_supportedCommands, arg);
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            if (IsClosing)
            {
                Log.Debug("IsClosing is true.  Will not handle the focus change");
                return;
            }

            Log.Debug("OnFocus: " + monitorInfo.ToString());

            base.OnFocusChanged(monitorInfo, ref handled);
            Log.Debug("menuShown: " + _scannerShown + ", title: " + monitorInfo.Title);
            if (!_scannerShown && monitorInfo.Title == R.GetString("CreateNewFile"))
            {
                var arg = new PanelRequestEventArgs("Alphabet", monitorInfo);
                _scannerShown = true;
                showPanel(this, arg);
            }

            handled = true;
        }

        /// <summary>
        /// Invoked when the scanner is closed
        /// </summary>
        /// <param name="panelClass">name of scanner being closed</param>
        /// <param name="monitorInfo">foreground window info</param>
        public override void OnPanelClosed(String panelClass, WindowActivityMonitorInfo monitorInfo)
        {
            if (panelClass == PanelClasses.AlphabetMinimal)
            {
                if (_newFileNameForm != null)
                {
                    Windows.CloseForm(_newFileNameForm);
                }
            }
        }

        /// <summary>
        /// A request came in to close the agent. We MUST
        /// quit if this call is ever made
        /// </summary>
        /// <returns>true on success</returns>
        public override bool OnRequestClose()
        {
            handleQuit(false);
            return true;
        }

        /// <summary>
        /// Invoked when there is a request to run a command. This
        /// could as a result of the user activating a button on the
        /// scanner and there is a command associated with the button
        /// </summary>
        /// <param name="command">command to run</param>
        /// <param name="commandArg">any optional arguments</param>
        /// <param name="handled">was this handled?</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Creates and returns the text control object for this agent
        /// </summary>
        /// <param name="handle">handle of the edit control</param>
        /// <param name="focusedElement">automation element</param>
        /// <param name="handled">was this handled?</param>
        /// <returns>text control agent object</returns>
        protected override TextControlAgentBase createEditControlTextInterface(IntPtr handle,
                                                                        AutomationElement focusedElement,
                                                                        ref bool handled)
        {
            return new NewFileTextControlAgent(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Gets confirmation from the user
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>true on yes</returns>
        private static bool confirm(String prompt)
        {
            return DialogUtils.ConfirmScanner(PanelManager.Instance.GetCurrentPanel(), prompt);
        }

        /// <summary>
        /// Form is closing. quit
        /// </summary>
        /// <param name="flag">set false to quit</param>
        private void _newFileNameForm_EvtDone(bool flag)
        {
            if (flag)
            {
                handleDone();
            }
            else
            {
                handleQuit();
            }
        }

        /// <summary>
        /// Sets focus to the indicated window
        /// </summary>
        /// <param name="handle">handle to the window</param>
        private void activateWindow(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                User32Interop.SetFocus(handle);
                Thread.Sleep(1000);
                Windows.SetForegroundWindow(handle);
            }
        }

        /// <summary>
        /// Checks if the specified folder is valid
        /// Display error if is not valid
        /// </summary>
        /// <param name="folder">Name of folder</param>
        /// <returns>true on success</returns>
        private bool checkValidOrCreate(String folder)
        {
            var normalizedFolder = SmartPath.ACATNormalizePath(folder);
            if (Directory.Exists(normalizedFolder))
            {
                return true;
            }

            try
            {
                Directory.CreateDirectory(normalizedFolder);
                return true;
            }
            catch (Exception)
            {
                DialogUtils.ShowTimedDialog(
                                    Context.AppPanelManager.GetCurrentForm() as Form,
                                    R.GetString("Error"),
                                    String.Format(R.GetString("CouldNotCreateFolder"), normalizedFolder));
            }

            return false;
        }

        /// <summary>
        /// Creates a text file and launches notepad
        /// </summary>
        /// <param name="fileName">name of the text file</param>
        /// <returns>true on success</returns>
        private bool createAndLaunchNotepad(String fileName)
        {
            try
            {
                StreamWriter writer = File.CreateText(fileName);
                writer.Close();

                var startInfo = new ProcessStartInfo { FileName = "notepad.exe", Arguments = fileName };
                var process = Process.Start(startInfo);
                waitForProcessAndActivate(process);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a word doc and launches word
        /// </summary>
        /// <param name="fileName">name of the file to create</param>
        /// <returns>true on success</returns>
        private bool createAndLaunchWordDoc(String fileName)
        {
            bool retVal = true;
            bool comCreated = false;
            Microsoft.Office.Interop.Word.Application wordApp = null;
            try
            {
                try
                {
                    wordApp = (Microsoft.Office.Interop.Word.Application)Marshal.GetActiveObject("Word.Application");
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                    wordApp = null;
                }

                if (wordApp == null)
                {
                    wordApp = new Microsoft.Office.Interop.Word.Application();
                }
                else
                {
                    comCreated = true;
                }

                Microsoft.Office.Interop.Word.Document adoc = wordApp.Documents.Add();
                object missing = System.Reflection.Missing.Value;
                object f = fileName;
                adoc.SaveAs(ref f, ref missing, ref missing, ref missing, ref missing,ref missing,
                        ref missing, ref missing, ref missing, ref missing, ref missing,ref missing,
                        ref missing, ref missing, ref missing, ref missing);

                adoc.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges, ref missing, ref missing);
                if (comCreated)
                {
                    Marshal.ReleaseComObject(wordApp);
                }

                wordApp = null;
                Process.Start(fileName);
                waitForWordWindow(fileName);
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }
            finally
            {
                if (wordApp != null)
                {
                    Marshal.FinalReleaseComObject(wordApp);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Creates the indicated file and launches the app
        /// with the file so the user can start working on it
        /// </summary>
        /// <param name="fileName">name of the file to create</param>
        /// <returns>true on success</returns>
        private bool createFileAndLaunchApp(String fileName)
        {
            fileName = fileName.Trim();
            if (String.IsNullOrEmpty(fileName) ||
                File.Exists(fileName) ||
                fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
            {
                return false;
            }

            String extension = Path.GetExtension(fileName);
            bool retVal = false;
            if (extension.ToLower().Contains("txt"))
            {
                retVal = createAndLaunchNotepad(fileName);
            }
            else if (extension.ToLower().Contains("docx"))
            {
                retVal = createAndLaunchWordDoc(fileName);
            }

            return retVal;
        }

        /// <summary>
        /// User has entered the filename. Create the file,
        /// launch the app and quit
        /// </summary>
        private void handleDone()
        {
            var fileName = _newFileNameForm.FileToCreate.Trim();
            if (!String.IsNullOrEmpty(fileName))
            {
                var name = Path.GetFileName(fileName);
                if (confirm("Create file " + name + "?"))
                {
                    createFileAndLaunchApp(fileName);
                    handleQuit(false);
                }
            }
        }

        /// <summary>
        /// Handle quit by asking the user if there is a need
        /// to ask.  Close forms
        /// </summary>
        /// <param name="showPrompt">Should we prompt the user?</param>
        private void handleQuit(bool showPrompt = true)
        {
            bool confirmQuit = true;
            if (showPrompt)
            {
                confirmQuit = confirm(R.GetString("ExitWithoutCreating"));
                if (confirmQuit)
                {
                    ExitCode = CompletionCode.None;
                }
            }

            if (confirmQuit)
            {
                IsClosing = true;
                IsActive = false;

                if (_newFileNameForm != null)
                {
                    _newFileNameForm.EvtDone -= _newFileNameForm_EvtDone;
                    Windows.CloseForm(_newFileNameForm);
                }

                _fileChoiceMenu = null;
                _scannerShown = false;
                CreateTextFile = false;
                CreateWordDoc = false;

                Close();
            }
        }

        /// <summary>
        /// Displays a menu to let the user to select whether to
        /// create a text file or a word doc
        /// </summary>
        private String showFileChoices()
        {
            _fileChoiceMenu = Context.AppPanelManager.CreatePanel("NewFileChoiceMenu", R.GetString("CreateNew")) as FileChoiceMenu;
            _scannerShown = false;
            Context.AppPanelManager.ShowDialog(_fileChoiceMenu);

            return _fileChoiceMenu.Choice;
        }

        /// <summary>
        /// Shows the new file form to create a text file
        /// </summary>
        private void showNewFileDialogForTextFile()
        {
            _newFileNameForm = new NewFileNameForm();
            _newFileNameForm.EvtDone += _newFileNameForm_EvtDone;
            _newFileNameForm.CreateFileType = NewFileNameForm.FileType.Text;
            _newFileNameForm.CreateFileDirectory = SmartPath.ACATNormalizePath(Settings.NewTextFileCreateFolder);
            _newFileNameForm.ShowDialog();
        }

        /// <summary>
        /// Shows the new file form to create a word document
        /// </summary>
        private void showNewFileDialogForWordDoc()
        {
            _newFileNameForm = new NewFileNameForm();
            _newFileNameForm.EvtDone += _newFileNameForm_EvtDone;
            _newFileNameForm.CreateFileType = NewFileNameForm.FileType.Word;
            _newFileNameForm.CreateFileDirectory = SmartPath.ACATNormalizePath(Settings.NewWordDocCreateFolder);
            _newFileNameForm.ShowDialog();
        }

        /// <summary>
        /// Waits for the process to start and activates the window
        /// </summary>
        /// <param name="process">process to activate</param>
        private void waitForProcessAndActivate(Process process)
        {
            try
            {
                process.WaitForInputIdle(6000);
                int ii = 0;
                while (true)
                {
                    IntPtr handle = process.MainWindowHandle;
                    if (handle != IntPtr.Zero)
                    {
                        activateWindow(handle);
                        break;
                    }

                    Thread.Sleep(500);
                    ii++;
                    if (ii > 10)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        /// <summary>
        /// Waits for MS Word to start. Does this by
        /// checking if the window with the file name title
        /// was created
        /// </summary>
        /// <param name="fileName">name of the file loaded</param>
        private void waitForWordWindow(String fileName)
        {
            String name = Path.GetFileName(fileName);
            String title = name + " - " + R.GetString("MicrosoftWord");
            Log.Debug(title);
            for (int ii = 0; ii < 10; ii++)
            {
                IntPtr h = User32Interop.FindWindow(null, title);
                if (h != IntPtr.Zero)
                {
                    Log.Debug("window FOUND!!!");
                    break;
                }

                Log.Debug("window not found");
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<CreateFileSettings>();
        }

        /// <summary>
        /// Returns the settings for this agent
        /// </summary>
        /// <returns>settings object</returns>
        public override IPreferences GetPreferences()
        {
            return Settings;
        }

    }
}