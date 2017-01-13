////////////////////////////////////////////////////////////////////////////
// <copyright file="FoxitReaderBase.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.AppAgents.FoxitReader
{
    /// <summary>
    /// Base class application agent for the Foxit PDF reader.  Enables
    /// easy browsing of PDF files, page up, page down, zoom in,
    /// zoom out etc.
    /// </summary>
    public class FoxitReaderAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// Panel name of the contextual menu for the Foxit reader
        /// </summary>
        private const String ContextualMenuName = "FoxitReaderContextMenu";

        /// <summary>
        /// This one supports the Foxit reader
        /// </summary>
        private const String ReaderProcessName = "foxit reader";

        /// <summary>
        /// Features supported by this agent. Widgets that
        /// correspond to these features will be enabled
        /// </summary>
        private readonly String[] _supportedCommands =
        {
            "OpenFile",
            "SaveFile",
            "CmdFind",
            "CmdContextMenu",
            "CmdZoomIn",
            "CmdZoomOut",
            "CmdZoomFit",
            "CmdSelectModeToggle",
            "CmdSwitchApps"
        };

        /// <summary>
        /// Has the scanner been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// Gets a list of processes supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(ReaderProcessName), new AgentProcessInfo("foxitreader") }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            checkCommandEnabled(_supportedCommands, arg);
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            showPanel(this, new PanelRequestEventArgs(ContextualMenuName, "Foxit", monitorInfo));
        }

        /// <summary>
        /// Invoked when the foreground window focus changes. Depending on which
        /// element has focus in the acrobat reader window, display the appropriate
        /// scanner
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            if (autoSwitchScanners)
            {
                displayScanner(monitorInfo, ref handled);
            }
            else
            {
                base.OnFocusChanged(monitorInfo, ref handled);
                if (!_scannerShown)
                {
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                    _scannerShown = true;
                }

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
        /// Invoked to run a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="commandArg">Optional arguments for the command</param>
        /// <param name="handled">set this to true if handled</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "SwitchAppWindow":
                    DialogUtils.ShowTaskSwitcher(ReaderProcessName);
                    break;

                case "FoxitZoomMenu":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var panelArg = new PanelRequestEventArgs("FoxitReaderZoomMenu", "Foxit", monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };

                        showPanel(this, panelArg);
                    }

                    break;

                case "SaveFile":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F);
                    AgentManager.Instance.Keyboard.Send(Keys.A);
                    break;

                case "CmdZoomIn":
                    TextControlAgent.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Add);
                    TextControlAgent.Resume();
                    break;

                case "CmdZoomOut":
                    TextControlAgent.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Subtract);
                    TextControlAgent.Resume();
                    break;

                case "CmdZoomFit":
                    TextControlAgent.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D2);
                    TextControlAgent.Resume();
                    break;

                case "CmdFind":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.F);
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Displays the scanner appropriate for the element that is currently
        /// in focus in the Acrobat reader window
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        private void displayScanner(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            if (monitorInfo.FocusedElement.Current.ControlType.ProgrammaticName == "ControlType.Edit")
            {
                base.OnFocusChanged(monitorInfo, ref handled);
                showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
            }
            else
            {
                setTextInterface(new ReadOnlyTextControlAgent());
                showPanel(this, new PanelRequestEventArgs(ContextualMenuName, "Foxit", monitorInfo));
            }
            handled = true;
        }
    }
}