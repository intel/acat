////////////////////////////////////////////////////////////////////////////
// <copyright file="LaunchAppAgent.cs" company="Intel Corporation">
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
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.UserManagement;
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

namespace ACAT.Extensions.Default.FunctionalAgents.LaunchAppAgent
{
    /// <summary>
    /// Agent that allows the user to launch applications. The
    /// list of applications and the command line args are configurable
    /// through an external XML file.  The LaunchAppScanner reads the
    /// xml file, parses it and build the list of apps.  The list
    /// of apps is diplayed in the form and the user selects the app
    /// to launch
    /// </summary>
    [DescriptorAttribute("AC74FFEA-4B1C-4707-93E4-2D6BA98C9EA0",
                            "Launch App Agent",
                            "Allows the user to launch an application")]
    internal class LaunchAppAgent : FunctionalAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static LaunchAppSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "LaunchAppSettings.xml";

        /// <summary>
        /// The scanner that displays the list of applications
        /// </summary>
        private static LaunchAppScanner _launchAppScanner;

        /// <summary>
        /// AppInfo of the application selected by the user
        /// to launch
        /// </summary>
        private AppInfo _appToLaunchInfo;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public LaunchAppAgent()
        {
            LaunchAppSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = LaunchAppSettings.Load();
        }

        /// <summary>
        /// Invoked when the Functional agent is activated.  This is
        /// the entry point.
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Activate()
        {
            ExitCode = CompletionCode.ContextSwitch;
            _appToLaunchInfo = null;
            _launchAppScanner = Context.AppPanelManager.CreatePanel("LaunchAppScanner") as LaunchAppScanner;

            if (_launchAppScanner != null)
            {
                _launchAppScanner.FormClosing += _form_FormClosing;
                _launchAppScanner.EvtQuit += _launchAppScanner_EvtQuit;
                _launchAppScanner.EvtLaunchApp += _launchAppScanner_EvtLaunchApp;
                _launchAppScanner.EvtShowScanner += launchAppScanner_EvtShowScanner;

                Context.AppPanelManager.ShowDialog(_launchAppScanner);
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
            arg.Handled = true;

            switch (arg.Widget.SubClass)
            {
                case "PunctuationScanner":
                case "NumberScanner":
                    arg.Enabled = true;
                    break;

                default:
                    if (_launchAppScanner != null)
                    {
                        _launchAppScanner.CheckWidgetEnabled(arg);
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
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug("OnFocus: " + monitorInfo);

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
            if (_launchAppScanner != null)
            {
                _launchAppScanner.OnRunCommand(command, ref handled);
            }
        }

        /// <summary>
        /// Creates text control agent object
        /// </summary>
        /// <param name="handle">handle of target control</param>
        /// <param name="focusedElement">automaton element</param>
        /// <param name="handled">was this handled?</param>
        /// <returns>the text control object</returns>
        protected override TextControlAgentBase createEditControlTextInterface(
                                                        IntPtr handle,
                                                        AutomationElement focusedElement,
                                                        ref bool handled)
        {
            return new LaunchAppTextControlAgent(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Release resources and close
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_launchAppScanner != null)
            {
                _launchAppScanner.FormClosing -= _form_FormClosing;
                _launchAppScanner.EvtQuit -= _launchAppScanner_EvtQuit;
                _launchAppScanner.EvtLaunchApp -= _launchAppScanner_EvtLaunchApp;
                _launchAppScanner.EvtShowScanner -= launchAppScanner_EvtShowScanner;
            }

            _launchAppScanner = null;
        }

        /// <summary>
        /// Request came in to launch an app. Launch
        /// the specified application.  After launching,
        /// the scanner and the agent are both closed.
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="appinfo">which app to launch</param>
        private void _launchAppScanner_EvtLaunchApp(object sender, AppInfo appInfo)
        {
            _appToLaunchInfo = appInfo;

            launchProcess(_appToLaunchInfo);

            closeScanner();

            Close();
        }

        /// <summary>
        /// Quit the agent after confirming with the user
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="e">event args</param>
        private void _launchAppScanner_EvtQuit(object sender, EventArgs args)
        {
            if (confirm("Close?"))
            {
                quit();
            }
        }

        /// <summary>
        /// Set focus to the specified window
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
        /// Close the launchapp scanner
        /// </summary>
        private void closeScanner()
        {
            if (_launchAppScanner != null)
            {
                Windows.CloseForm(_launchAppScanner);

                _launchAppScanner = null;
            }
        }

        /// <summary>
        /// Get confirmation from the user
        /// </summary>
        /// <param name="prompt">prompt to display</param>
        /// <returns>true on yes</returns>
        private bool confirm(String prompt)
        {
            return DialogUtils.ConfirmScanner(PanelManager.Instance.GetCurrentForm(), prompt);
        }

        /// <summary>
        /// Event handler to display the alphabet scanner
        /// </summary>
        /// <param name="sender">event sender</param>
        /// <param name="eventArgs">event args</param>
        private void launchAppScanner_EvtShowScanner(object sender, EventArgs eventArgs)
        {
            if (_launchAppScanner != null)
            {
                var arg = new PanelRequestEventArgs(PanelClasses.AlphabetMinimal, WindowActivityMonitor.GetForegroundWindowInfo())
                {
                    TargetPanel = _launchAppScanner,
                    RequestArg = _launchAppScanner,
                    UseCurrentScreenAsParent = true
                };
                showPanel(this, arg);
            }
        }

        /// <summary>
        /// Launch the specified app
        /// </summary>
        /// <param name="info">info about the app</param>
        /// <returns>true on success</returns>
        private bool launchProcess(AppInfo info)
        {
            bool retVal = true;

            var startInfo = new ProcessStartInfo
            {
                FileName = info.Path,
                Arguments = normalizeCommandLine(info.CommandLine)
            };

            try
            {
                var process = Process.Start(startInfo);
                if (process == null)
                {
                    retVal = false;
                }
                else
                {
                    waitForProcessAndActivate(process);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Replace macros in the command line argument.
        /// </summary>
        /// <param name="commandLine">command line</param>
        /// <returns>normalized string</returns>
        private String normalizeCommandLine(String commandLine)
        {
            if (String.IsNullOrEmpty(commandLine))
            {
                return String.Empty;
            }

            commandLine = commandLine.ToLower().Trim();
            if (commandLine.Contains("@mydocuments"))
            {
                commandLine = commandLine.Replace("@mydocuments",
                                                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
            else if (commandLine.Contains("@mymusic"))
            {
                commandLine = commandLine.Replace("@mymusic",
                                                    Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            }
            else if (commandLine.Contains("@mypictures"))
            {
                commandLine = commandLine.Replace("@mypictures",
                                                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            }
            else if (commandLine.Contains("@myvideos"))
            {
                commandLine = commandLine.Replace("@myvideos",
                                                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            }

            return commandLine;
        }

        /// <summary>
        /// Quit the agent.  Close the scanner
        /// </summary>
        private void quit()
        {
            ExitCode = CompletionCode.None;
            closeScanner();
            Close();
        }

        /// <summary>
        /// Wait the the proc to start and set focus to its window
        /// </summary>
        /// <param name="process">which process?</param>
        private void waitForProcessAndActivate(Process process)
        {
            try
            {
                process.WaitForInputIdle(6000);
                var handle = process.MainWindowHandle;
                if (handle != IntPtr.Zero)
                {
                    activateWindow(handle);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }
    }
}