////////////////////////////////////////////////////////////////////////////
// <copyright file="FileBrowserAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.FileBrowserAgent
{
    /// <summary>
    /// This agent manages the file browser scanner that enables the
    /// user to manage files from favorite folders.  User can open
    /// or delete files.  The files are displayed in the file
    /// browser scanner as a list.  User can search based on
    /// a filter.
    /// Based on configuration, the agent will either open a file
    /// or return the name of the file that was selected and
    /// the caller can act on the file.
    /// </summary>
    [DescriptorAttribute("EC2EA972-934B-4EE0-A909-3EA0140AC738",
                        "File Browser",
                        "FileBrowserAgent",
                        "Open/delete files from favorite folders")]
    internal class FileBrowserAgent : FunctionalAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static FileBrowserSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "FileBrowserSettings.xml";

        /// <summary>
        /// The scanner object
        /// </summary>
        private static FileBrowserScanner _fileBrowserScanner;

        /// <summary>
        /// When the user selects a file, should it be
        /// launched or should the user be prompted?
        /// </summary>
        private bool _autoLaunchFile;

        /// <summary>
        /// To prevent reentrancy in the form_EvtDone event handler 
        /// </summary>
        private volatile bool _inDone;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FileBrowserAgent()
        {
            FileBrowserSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);

            Settings = FileBrowserSettings.Load();

            initProperties();
        }

        /// <summary>
        /// Gets or sets the action on the file. What should
        /// we do when the user selects.  Maps to the FileOperation
        /// enum
        /// </summary>
        public String Action { get; set; }

        /// <summary>
        /// Gets or sets the action verb that will be used in the
        /// confirmation box when the user selects a file
        /// </summary>
        public String ActionVerb { get; set; }

        /// <summary>
        /// Gets or sets whether the agent should open the file selected
        /// </summary>
        public bool AutoLaunchFile { get; set; }

        /// <summary>
        /// Gets or sets the file extensions to be excluded. Files
        /// with these extensions will not be selected
        /// </summary>
        public String[] ExcludeFileExtensions { get; set; }

        /// <summary>
        /// Gets or sets a set of folders from which to
        /// list files
        /// </summary>
        public String[] Folders { get; set; }

        /// <summary>
        /// Gets or sets the extensions to include. Only files with
        /// these extensions will be displayed. Wildcard * can
        /// be used for all files
        /// </summary>
        public String[] IncludeFileExtensions { get; set; }

        /// <summary>
        /// Gets the name of the file that the user selected
        /// </summary>
        public String SelectedFile { get; private set; }

        /// <summary>
        /// Invoked when the Functional agent is activated.  This is
        /// the entry point for this agent.  Show the scanner that
        /// will display the list of files from the preferred folders(s)
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Activate()
        {
            _autoLaunchFile = false;
            ExitCode = CompletionCode.ContextSwitch;

            _fileBrowserScanner = Context.AppPanelManager.CreatePanel("FileBrowserScanner") as FileBrowserScanner;
            if (_fileBrowserScanner != null)
            {
                IsClosing = false;

                subscribeToEvents();

                _fileBrowserScanner.Action = parseAction(Action);
                _fileBrowserScanner.IncludeFileExtensions = IncludeFileExtensions;
                _fileBrowserScanner.ExcludeFileExtensions = ExcludeFileExtensions;
                _fileBrowserScanner.Folders = Folders;
                _fileBrowserScanner.ActionVerb = ActionVerb;
                SelectedFile = String.Empty;

                IsActive = true;

                Context.AppPanelManager.ShowDialog(_fileBrowserScanner);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            arg.Handled = true;

            switch (arg.Command)
            {
                case "CmdPunctuationScanner":
                case "CmdNumberScanner":
                    arg.Enabled = true;
                    break;

                default:
                    if (_fileBrowserScanner != null)
                    {
                        _fileBrowserScanner.CheckCommandEnabled(arg);
                    }
                    if (!arg.Handled)
                    {
                        arg.Enabled = false;
                        arg.Handled = true;
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<FileBrowserSettings>();
        }

        /// <summary>
        /// Returns the settings for this agent
        /// </summary>
        /// <returns>settings object</returns>
        public override IPreferences GetPreferences()
        {
            return Settings;
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

            base.OnFocusChanged(monitorInfo, ref handled);

            handled = true;
        }

        /// <summary>
        /// A request came in to close the agent. We MUST
        /// quit if this call is ever made
        /// </summary>
        /// <returns>true on success</returns>
        public override bool OnRequestClose()
        {
            quit();
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
                    Log.Debug(command);
                    if (_fileBrowserScanner != null)
                    {
                        _fileBrowserScanner.OnRunCommand(command, ref handled);
                    }

                    break;
            }
        }

        /// <summary>
        /// Returns the text control interface object
        /// </summary>
        /// <param name="handle">handle to the edit control</param>
        /// <param name="focusedElement">automation element</param>
        /// <param name="handled">was this handled?</param>
        /// <returns>the text control agent object</returns>
        protected override TextControlAgentBase createEditControlTextInterface(
                                                    IntPtr handle,
                                                    AutomationElement focusedElement,
                                                    ref bool handled)
        {
            return new FileBrowserTextControlAgent(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Event handler for when the user selects a file. Depending
        /// on the configuration, either the file will be opened or the name
        /// of the file will be returned to the caller.
        /// The SelectedFile property will contain the name of the selected file.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _fileBrowserScanner_EvtFileOpen(object sender, EventArgs e)
        {
            _autoLaunchFile = AutoLaunchFile;
            SelectedFile = _fileBrowserScanner.SelectedFile;

            var extension = Path.GetExtension(SelectedFile);

            if (String.IsNullOrEmpty(FileUtils.GetFileAssociationForExtension(extension)))
            {
                if (!DialogUtils.ConfirmScanner(PanelManager.Instance.GetCurrentForm(),
                                R.GetString("NoProgramAssociatedWithFile")))
                {
                    return;
                }
            }

            closeScanner();

            if (_autoLaunchFile && !String.IsNullOrEmpty(SelectedFile.Trim()))
            {
                handleOpenFile(SelectedFile);
            }
            else
            {
                ExitCode = CompletionCode.None;
            }

            Close();
        }

        /// <summary>
        /// Event handler for when the file browser scanner requests for an
        /// alphabet scanner to be displayed.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void _fileBrowserScanner_EvtShowScanner(object sender, EventArgs eventArgs)
        {
            if (_fileBrowserScanner != null)
            {
                var arg = new PanelRequestEventArgs(PanelClasses.AlphabetMinimal, WindowActivityMonitor.GetForegroundWindowInfo())
                {
                    TargetPanel = _fileBrowserScanner,
                    RequestArg = _fileBrowserScanner,
                    UseCurrentScreenAsParent = true
                };
                showPanel(this, arg);
            }
        }

        /// <summary>
        /// We are done. Quit!
        /// </summary>
        /// <param name="flag">set to false to not quit</param>
        private void _form_EvtDone(bool flag)
        {
            if (_inDone)
            {
                return;
            }

            _inDone = true;

            if (!flag)
            {
                quitFileBrowser();
            }

            _inDone = false;
        }

        /// <summary>
        /// Form is closing. Cleanup
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            unsubscribeFromEvents();
            _fileBrowserScanner = null;

            // re-initialize for next time
            initProperties();
        }

        /// <summary>
        /// Activates the window represented by the handle and
        /// sets it as the foreground window
        /// </summary>
        /// <param name="handle"></param>
        private void activateWindow(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                User32Interop.BringWindowToTop(handle);
                User32Interop.SetFocus(handle);
                Thread.Sleep(1000);
                Windows.SetForegroundWindow(handle);
            }
        }

        /// <summary>
        /// For text files, checks if the file is already
        /// open in notepad. IF so, activates the window
        /// </summary>
        /// <param name="fileName">name of the file</param>
        /// <returns>true if window was found</returns>
        private bool activateWindowIfAlreadyOpen(String fileName)
        {
            String extension = Path.GetExtension(fileName);
            Log.Debug("Extension: [" + extension + "]");
            if (extension.Equals(".txt", StringComparison.InvariantCultureIgnoreCase) || String.IsNullOrEmpty(extension))
            {
                String name = Path.GetFileName(fileName);
                String title = name + " - " + R.GetString2("Notepad");
                Log.Debug("Finding window " + title);
                IntPtr h = User32Interop.FindWindow(null, title);
                Log.Debug("handle h = " + h);
                if (h != IntPtr.Zero)
                {
                    User32Interop.ShowWindowAsync(h, User32Interop.SW_RESTORE);
                    activateWindow(h);
                    return true;
                }

                Log.Debug("Could not find window");
            }

            return false;
        }

        /// <summary>
        /// Closes the file browser scanner
        /// </summary>
        private void closeScanner()
        {
            if (_fileBrowserScanner != null)
            {
                Windows.CloseForm(_fileBrowserScanner);
                _fileBrowserScanner = null;
            }
        }

        /// <summary>
        /// Handles file open operation.  Uses file association
        /// to determine which app opens the file.  Starts
        /// notepad for text files.
        /// </summary>
        /// <param name="fileName">Name of the file to open</param>
        private void handleOpenFile(String fileName)
        {
            try
            {
                String extension = Path.GetExtension(fileName);
                if (!activateWindowIfAlreadyOpen(fileName))
                {
                    if (String.IsNullOrEmpty(extension))
                    {
                        startWithNotepad(fileName);
                    }
                    else
                    {
                        Log.Debug("Starting process " + fileName);
                        var process = Process.Start(fileName);
                        waitForProcessAndActivate(process);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Cannot start process ", ex);
            }
        }

        /// <summary>
        /// Initializes the agent
        /// </summary>
        private void initProperties()
        {
            ActionVerb = "Open";
            IncludeFileExtensions = new String[] { };
            ExcludeFileExtensions = (!String.IsNullOrEmpty(Settings.ExcludeFileExtensions)) ?
                                        Settings.ExcludeFileExtensions.Split(';') : new String[] { };
            Folders = Settings.GetFavoriteFolders();
            AutoLaunchFile = false;
        }

        private FileBrowserScanner.FileOperation parseAction(String action)
        {
            FileBrowserScanner.FileOperation result;

            if (Enum.TryParse(action, true, out result))
            {
                return result;
            }

            return FileBrowserScanner.FileOperation.UserChoice;
        }

        /// <summary>
        /// Close all the forms
        /// </summary>
        private void quit()
        {
            IsClosing = true;
            IsActive = false;

            closeScanner();
            Close();
        }

        /// <summary>
        /// Gets confirmation from the user and quits
        /// </summary>
        private void quitFileBrowser()
        {
            if (_fileBrowserScanner != null)
            {
                if (DialogUtils.ConfirmScanner(PanelManager.Instance.GetCurrentForm(), R.GetString("CloseQ")))
                {
                    ExitCode = CompletionCode.None;
                    quit();
                }
            }
        }

        /// <summary>
        /// Launches the specified file in notepad
        /// </summary>
        /// <param name="fileName">name of the file</param>
        private void startWithNotepad(String fileName)
        {
            var startInfo = new ProcessStartInfo { FileName = "notepad.exe", Arguments = fileName };
            var process = Process.Start(startInfo);
            waitForProcessAndActivate(process);
        }

        /// <summary>
        /// Subscribes to events
        /// </summary>
        private void subscribeToEvents()
        {
            if (_fileBrowserScanner != null)
            {
                _fileBrowserScanner.FormClosing += _form_FormClosing;
                _fileBrowserScanner.EvtFileOpen += _fileBrowserScanner_EvtFileOpen;
                _fileBrowserScanner.EvtDone += _form_EvtDone;
                _fileBrowserScanner.EvtShowScanner += _fileBrowserScanner_EvtShowScanner;
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        private void unsubscribeFromEvents()
        {
            if (_fileBrowserScanner != null)
            {
                _fileBrowserScanner.FormClosing -= _form_FormClosing;
                _fileBrowserScanner.EvtFileOpen -= _fileBrowserScanner_EvtFileOpen;
                _fileBrowserScanner.EvtDone -= _form_EvtDone;
                _fileBrowserScanner.EvtShowScanner -= _fileBrowserScanner_EvtShowScanner;
            }
        }

        /// <summary>
        /// Waits for process to come up.  Checks the process
        /// mainhandle to get activated.
        /// </summary>
        /// <param name="process">the process object</param>
        private void waitForProcessAndActivate(Process process)
        {
            try
            {
                process.WaitForInputIdle(6000);

                IntPtr handle = process.MainWindowHandle;

                if (handle != IntPtr.Zero)
                {
                    activateWindow(handle);
                }
                else
                {
                    Log.Debug("Process.MainInputHandle is zero");
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}