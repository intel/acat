////////////////////////////////////////////////////////////////////////////
// <copyright file="DLLHostAgent.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Extensions.Base.AppAgents.DLLHostAgent
{
    /// <summary>
    /// Application agent for DLLHost.dll.  Windows uses this
    /// executable to load DLL's such as the Windows Photo Viewer.
    /// For now, it supports only Windows Photo Viewer but can be
    /// extended to support other services that DLLHost enables.
    /// </summary>
    [DescriptorAttribute("5D49B45B-0CD3-4058-BE2A-816A15600FA8", "DLL Host Agent", "App Agent DLL Host (eg Windows Photo Viewer)")]
    internal class DLLHostAgent : GenericAppAgentBase
    {
        /// <summary>
        /// Title of the contextual menu for the Photo viewer
        /// </summary>
        private const string PhotoViewerTitle = "Photo Viewer";

        /// <summary>
        /// Title of the windows photo viewer application window
        /// </summary>
        private const string WindowsPhotoViewerTitle = "windows photo viewer";

        /// <summary>
        /// Which features does this support?
        /// </summary>
        private readonly String[] _supportedFeatures = { "ContextualMenu" };

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
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            checkWidgetEnabled(_supportedFeatures, arg);
        }

        /// <summary>
        /// Invoked when required to display a contextual menu
        /// </summary>
        /// <param name="monitorInfo">info about foreground window</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            var title = monitorInfo.Title.ToLower();
            if (title.Contains(WindowsPhotoViewerTitle))
            {
                showPanel(this, new PanelRequestEventArgs("WindowsPhotoViewerContextMenu", monitorInfo));
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
            base.OnFocusChanged(monitorInfo, ref handled);

            // for now, we  handle only the photo viewer.
            var title = monitorInfo.Title.ToLower();
            if (title.Contains(WindowsPhotoViewerTitle))
            {
                showPanel(this, new PanelRequestEventArgs("WindowsPhotoViewerContextMenu", PhotoViewerTitle, monitorInfo));
                handled = true;
            }
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
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var panelArg = new PanelRequestEventArgs("PhotoViewerZoomMenu", PhotoViewerTitle, monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };
                        showPanel(this, panelArg);
                    }

                    break;

                case "PhotoViewerRotateMenu":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var panelArg = new PanelRequestEventArgs("PhotoViewerRotateMenu", PhotoViewerTitle, monitorInfo)
                        {
                            UseCurrentScreenAsParent = true
                        };

                        showPanel(this, panelArg);
                    }

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
    }
}