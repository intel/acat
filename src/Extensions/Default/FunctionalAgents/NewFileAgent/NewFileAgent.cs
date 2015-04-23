////////////////////////////////////////////////////////////////////////////
// <copyright file="NewFileAgent.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
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

namespace ACAT.Extensions.Hawking.FunctionalAgents.NewFile
{
    /// <summary>
    /// Functional agent that allows the user to create a new
    /// text file or a word document.  The file is created in a
    /// configurable folder. A check is made to see if the file
    /// already exists.  The agent then creates the file and quits
    /// so the user can start interacting with the application (notepad
    /// or ms word)
    /// </summary>
    [DescriptorAttribute("91390B32-8C7F-49DE-937E-E0BE3FF224F7", "New File Agent", "Agent for creating new files")]
    internal class NewFileAgent : FunctionalAgentBase
    {
        /// <summary>
        /// Is MS Word installed on the user's computer?
        /// </summary>
        private readonly bool _isWordInstalled;

        /// <summary>
        /// Which buttons in the alphabet scanner do we support?
        /// </summary>
        private readonly String[] _supportedFeatures =
        {
            "Back",
            "ShiftKey"
        };

        /// <summary>
        /// Contextual menu for the new file form
        /// </summary>
        private Form _newFileContextMenu;

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
        /// Initializes a new instance of the class.
        /// </summary>
        public NewFileAgent()
        {
            Name = "New File Agent";

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
                        _isWordInstalled = (wordObject.Version == "14.0");
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
            ExitCode = CompletionCode.ContextSwitch;
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
                showFileChoices();
            }

            return true;
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            switch (arg.Widget.SubClass)
            {
                case "ShowMainMenu":
                case "MouseScanner":
                case "CursorScanner":
                case "TabKey":
                case "AltKey":
                case "CtrlKey":
                case "ContextualMenu":
                case "FileBrowserToggle":
                    arg.Handled = true;
                    arg.Enabled = false;
                    break;

                case "Back":
                case "DeletePreviousWord":
                case "clearText":
                    arg.Handled = true;
                    arg.Enabled = _newFileNameForm != null && !String.IsNullOrEmpty(_newFileNameForm.FileNameEntered);
                    return;

                case "EnterKey":
                    arg.Handled = true;
                    arg.Enabled = _newFileNameForm != null && _newFileNameForm.ValidNameSpecified();
                    return;

                case "WordDoc":
                    arg.Enabled = _isWordInstalled;
                    arg.Handled = true;
                    break;

                case "TextDoc":
                    arg.Enabled = true;
                    arg.Handled = true;
                    break;
            }

            checkWidgetEnabled(_supportedFeatures, arg);
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug("OnFocus: " + monitorInfo.ToString());

            base.OnFocusChanged(monitorInfo, ref handled);
            Log.Debug("menuShown: " + _scannerShown + ", title: " + monitorInfo.Title);
            if (!_scannerShown && monitorInfo.Title == "NewFileNameForm")
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
                case "TextFile":

                    if (checkValidOrCreate(Common.AppPreferences.NewTextFileCreateFolder))
                    {
                        Context.AppPanelManager.CloseCurrentForm();
                        showNewFileDialogForTextFile();
                    }

                    break;

                case "WordDoc":
                    if (checkValidOrCreate(Common.AppPreferences.NewWordDocCreateFolder))
                    {
                        Context.AppPanelManager.CloseCurrentForm();
                        showNewFileDialogForWordDoc();
                    }

                    break;

                case "clearText":
                    if (_newFileNameForm != null && confirm("Clear File Name?"))
                    {
                        _newFileNameForm.ClearFileName();
                    }

                    break;

                case "exitFileTypeMenu":
                    ExitCode = CompletionCode.None;
                    Context.AppPanelManager.CloseCurrentForm();
                    handleQuit(false);
                    break;

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
        protected override TextControlAgentBase createEditControlTextInterface(IntPtr handle, AutomationElement focusedElement, ref bool handled)
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
                                    "Error", 
                                    "Could not create folder " + normalizedFolder);
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
                adoc.SaveAs(
                        ref f,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing,
                        ref missing);

                ((Microsoft.Office.Interop.Word._Document)adoc).Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdSaveChanges, ref missing, ref missing);
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
                confirmQuit = confirm("Exit without creating?");
                if (confirmQuit)
                {
                    ExitCode = CompletionCode.None;
                }
            }

            if (confirmQuit)
            {
                if (_newFileNameForm != null)
                {
                    _newFileNameForm.EvtDone -= _newFileNameForm_EvtDone;
                    Windows.CloseForm(_newFileNameForm);
                }

                _newFileContextMenu = null;
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
        private void showFileChoices()
        {
            _newFileContextMenu = Context.AppPanelManager.CreatePanel("NewFileContextMenu", "Create New");
            _scannerShown = false;
            Context.AppPanelManager.ShowDialog(_newFileContextMenu as IPanel);
        }

        /// <summary>
        /// Shows the new file form to create a text file
        /// </summary>
        private void showNewFileDialogForTextFile()
        {
            _newFileNameForm = new NewFileNameForm();
            _newFileNameForm.EvtDone += new NewFileNameForm.DoneEvent(_newFileNameForm_EvtDone);
            _newFileNameForm.CreateFileType = NewFileNameForm.FileType.Text;
            _newFileNameForm.CreateFileDirectory = SmartPath.ACATNormalizePath(Common.AppPreferences.NewTextFileCreateFolder);
            _newFileNameForm.Show();
        }

        /// <summary>
        /// Shows the new file form to create a word document
        /// </summary>
        private void showNewFileDialogForWordDoc()
        {
            _newFileNameForm = new NewFileNameForm();
            _newFileNameForm.EvtDone += new NewFileNameForm.DoneEvent(_newFileNameForm_EvtDone);
            _newFileNameForm.CreateFileType = NewFileNameForm.FileType.Word;
            _newFileNameForm.CreateFileDirectory = SmartPath.ACATNormalizePath(Common.AppPreferences.NewWordDocCreateFolder);
            _newFileNameForm.Show();
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
            String title = name + " - Microsoft Word";
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
    }
}