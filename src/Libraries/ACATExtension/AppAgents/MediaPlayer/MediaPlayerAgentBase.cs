////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaPlayerAgentBase.cs" company="Intel Corporation">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
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

namespace ACAT.Lib.Extension.AppAgents.MediaPlayer
{
    /// <summary>
    /// Base class application agent for Windows Media Player
    /// to control playback of audio/video files.  Allows play,
    /// stop, pause, fwd, rewind etc.
    /// </summary>
    public class MediaPlayerAgentBase : GenericAppAgentBase
    {
        public const int WM_COMMAND = 0x111;

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
        private const String ContextualMenuName = "MediaPlayerContextMenu";

        private const String MenuTitle = "Media Player";

        /// <summary>
        /// This one supports the Foxit reader
        /// </summary>
        private const String PlayerProcessName = "wmplayer";

        /// <summary>
        /// Features supported by this agent. Widgets that
        /// correspond to these features will be enabled
        /// </summary>
        private readonly String[] _supportedFeatures =
        {
            "OpenFile",
            "ContextualMenu",
        };

        /// <summary>
        /// Has the scanner been shown yet?
        /// </summary>
        private bool _scannerShown;

        private IntPtr _windowHandle;

        /// <summary>
        /// Gets a list of processes supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(PlayerProcessName) }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            checkWidgetEnabled(_supportedFeatures, arg);
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            // Do a panel switch to the main document
            //AgentManager.Instance.Keyboard.Send(Keys.F6);
            showPanel(this, new PanelRequestEventArgs(ContextualMenuName, MenuTitle, monitorInfo));
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

            if (monitorInfo.Title == "Windows Media Player")
            {
                _windowHandle = monitorInfo.FgHwnd;

                if (!_scannerShown)
                {
                    showPanel(this,
                        new PanelRequestEventArgs((autoSwitchScanners) ? ContextualMenuName : PanelClasses.Alphabet,
                            MenuTitle, monitorInfo));
                    _scannerShown = true;
                }
                handled = true;
            }
            else
            {
                _windowHandle = IntPtr.Zero;
                handled = false;
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
        /// Executes the specified command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="commandArg">Optional arguments for the command</param>
        /// <param name="handled">set this to true if handled</param>
        public override void OnRunCommand(String command, object commandArg, ref bool handled)
        {
            handled = true;
            switch (command)
            {
                case "PlayPause":
                    sendMessage(0x00004978);
                    break;

                case "FastForward":
                    sendMessage(0x0000497D);
                    break;

                case "Rewind":
                    //sendMessage(0x0000497C);
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.B);
                    break;

                case "Stop":
                    sendMessage(0x00004979);
                    break;

                case "PreviousTrack":
                    sendMessage(0x0000497A);
                    break;

                case "NextTrack":
                    sendMessage(0x0000497B);
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Sends a windows message to the mediaplayer window
        /// </summary>
        /// <param name="messageCode">mesage to send</param>
        protected void sendMessage(int messageCode)
        {
            User32Interop.SendMessageIntInt(_windowHandle, WM_COMMAND, messageCode, 0x00000000);
        }
    }
}