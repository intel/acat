////////////////////////////////////////////////////////////////////////////
// <copyright file="DLLHostAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PreferencesManagement;
using ACAT.Lib.Core.UserManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.DLLHostAgent
{
    /// <summary>
    /// Application agent for DLLHost.dll.  Windows uses this
    /// executable to load DLL's such as the Windows Photo Viewer.
    /// For now, it supports only Windows Photo Viewer but can be
    /// extended to support other services that DLLHost enables.
    /// </summary>
    [DescriptorAttribute("5D49B45B-0CD3-4058-BE2A-816A15600FA8",
                            "DLLHost Agent (Win7)",
                            "Manages interactions with Windows 7 apps such as the Photo Viewer")]
    internal class DLLHostAgent : GenericAppAgentBase
    {
        /// <summary>
        /// Settings for this agent
        /// </summary>
        internal static DLLHostAgentSettings Settings;

        /// <summary>
        /// Name of the settings file
        /// </summary>
        private const string SettingsFileName = "DLLHostAgentSettings.xml";

        /// <summary>
        /// Title of the contextual menu for the Photo viewer
        /// </summary>
        private readonly string _photoViewerTitle = R.GetString("PhotoViewer");

        /// <summary>
        /// Which features does this support?
        /// </summary>
        private readonly String[] _supportedCommands = { "CmdContextMenu" };

        /// <summary>
        /// Title of the windows photo viewer application window
        /// </summary>
        private readonly string WindowsPhotoViewerTitle = R.GetString("WindowsPhotoViewer").ToLower();

        /// <summary>
        /// Has the scanner been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public DLLHostAgent()
        {
            DLLHostAgentSettings.PreferencesFilePath = UserManager.GetFullPath(SettingsFileName);
            Settings = DLLHostAgentSettings.Load();
        }

        /// <summary>
        /// Which processes does this agent support?
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo("dllhost") }; }
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            checkCommandEnabled(_supportedCommands, arg);
        }

        /// <summary>
        /// Returns the default settings
        /// </summary>
        /// <returns>Default settings object</returns>
        public override IPreferences GetDefaultPreferences()
        {
            return PreferencesBase.LoadDefaults<DLLHostAgentSettings>();
        }

        /// <summary>
        /// Returns the settings for this agent
        /// </summary>
        /// <returns>The settings object</returns>
        public override IPreferences GetPreferences()
        {
            return Settings;
        }

        /// <summary>
        /// Invoked when required to display a contextual menu
        /// </summary>
        /// <param name="monitorInfo">info about foreground window</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            bool handled = false;
            displayScanner(monitorInfo, ref handled);
        }

        /// <summary>
        /// Invoked when the focus changes either in the active window or when the
        /// active window itself changes.
        /// </summary>
        /// <param name="monitorInfo">Info about focused element</param>
        /// <param name="handled">was this handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            if (!_scannerShown || monitorInfo.IsNewWindow)
            {
                if (Settings.AutoSwitchScannerEnable)
                {
                    displayScanner(monitorInfo, ref handled);
                    if (handled)
                    {
                        _scannerShown = true;
                    }
                }
                else
                {
                    base.OnFocusChanged(monitorInfo, ref handled);
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                    _scannerShown = true;
                }
            }
            else
            {
                handled = true;
            }
        }

        /// <summary>
        /// Focus shifted to another app.  This agent is
        /// getting deactivated.
        /// </summary>
        public override void OnFocusLost()
        {
            _scannerShown = false;
        }

        /// <summary>
        /// Invoked when the user selects something in the scanner that
        /// corresponds to a mapped functionality.  For now we only
        /// support photo viewer. Extend this to support other
        /// functionalities
        /// </summary>
        /// <param name="command">the command</param>
        /// <param name="commandArg">any optional arguments</param>
        /// <param name="handled">was it handled?</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;

            switch (command)
            {
                case "NextPhoto":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Right);
                    break;

                case "PreviousPhoto":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Left);
                    break;

                case "PhotoDelete":
                    Context.AppAgentMgr.Keyboard.Send(Keys.Delete);
                    break;

                case "PhotoViewerZoomMenu":
                    showPanel(this, new PanelRequestEventArgs("PhotoViewerZoomMenu",
                                                                _photoViewerTitle,
                                                                WindowActivityMonitor.GetForegroundWindowInfo(),
                                                                true));
                    break;

                case "PhotoViewerRotateMenu":
                    showPanel(this, new PanelRequestEventArgs("PhotoViewerRotateMenu",
                                                                _photoViewerTitle,
                                                                WindowActivityMonitor.GetForegroundWindowInfo(),
                                                                true));
                    break;

                case "CmdZoomIn":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Add);
                    break;

                case "CmdZoomOut":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Subtract);
                    break;

                case "CmdZoomFit":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D0);
                    break;

                case "RotateRight":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.OemPeriod);
                    break;

                case "RotateLeft":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Oemcomma);
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Displays scanner depending on which app has focus
        /// </summary>
        /// <param name="monitorInfo">window info</param>
        /// <param name="handled">was it handled?</param>
        private void displayScanner(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            var title = monitorInfo.Title.ToLower();
            if (title.Contains(WindowsPhotoViewerTitle))
            {
                showPanel(this, new PanelRequestEventArgs("WindowsPhotoViewerContextMenu", _photoViewerTitle, monitorInfo));
                handled = true;
            }
        }
    }
}