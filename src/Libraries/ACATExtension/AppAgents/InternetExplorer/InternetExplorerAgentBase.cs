////////////////////////////////////////////////////////////////////////////
// <copyright file="InternetExplorerAgentBase.cs" company="Intel Corporation">
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
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
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

namespace ACAT.Lib.Extension.AppAgents.InternetExplorer
{
    /// <summary>
    /// Base class for the Application agent for Internet Explorer.
    /// </summary>
    public class InternetExplorerAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// Name of the IE Process
        /// </summary>
        private const String IEProcessName = "iexplore";

        /// <summary>
        /// Title for the IE contextual menu
        /// </summary>
        private const String ScannerTitle = "IExplorer";

        /// <summary>
        /// Tells us which control in the browser currently has focus
        /// </summary>
        private readonly IInternetExplorerElements _explorerElements;

        /// <summary>
        /// Which features does this agent support?  Widgets for
        /// these feature will be enabled
        /// </summary>
        private readonly String[] _supportedFeatures =
        {
            "OpenFile",
            "SaveFileAs",
            "Find",
            "ContextualMenu",
            "ZoomIn",
            "ZoomOut",
            "ZoomFit",
            "SelectMode",
            "SwitchAppWindow"
        };

        /// <summary>
        /// Which scanner type has been displayed?  Depends on
        /// which element in the browser currently has focus
        /// </summary>
        private ScannerType _scannerType = ScannerType.None;

        private bool _scannerShown;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public InternetExplorerAgentBase()
        {
            using (var browser = new WebBrowser())
            {
                var ver = browser.Version;
                if (ver.Major == 10)
                {
                    _explorerElements = new IE10Elements();
                }
                else
                {
                    _explorerElements = new IE8Elements();
                }
            }
        }

        /// <summary>
        /// Used to track which scanner has been displayed.
        /// This depends on which element in the browser
        /// currently has focus
        /// </summary>
        [Flags]
        private enum ScannerType
        {
            None = 0x0,
            Favorites = 0x1,
            Address = 0x2,
            Other = 0x4
        }

        /// <summary>
        /// This agent supports IE
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(IEProcessName) }; }
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
            //if (autoSwitchScanners)
            //{
            //    AgentManager.Instance.Keyboard.Send(Keys.F6);
            //}

            showPanel(this, new PanelRequestEventArgs("InternetExplorerContextMenu", ScannerTitle, monitorInfo));
        }

        /// <summary>
        /// Invoked when the foreground window focus changes. Display the
        /// scanner depending on the context. Also, if this is a new window that has
        /// come into focus, add its contents to the word prediction temporary batch model for more
        /// contextual prediction of words
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        public override void OnFocusChanged(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug();

            base.OnFocusChanged(monitorInfo, ref handled);

            if (autoSwitchScanners)
            {
                autoDisplayScanner(monitorInfo, ref handled);
            }
            else
            {
                if (!_scannerShown)
                {
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                    _scannerShown = true;
                }

                handled = true;
            }
        }

        /// <summary>
        /// Focus shifted to another window.  This agent is getting deactivated
        /// </summary>
        public override void OnFocusLost()
        {
            base.OnFocusLost();
            _scannerType = ScannerType.None;
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
                case "CmdThreeFourthMaximizeWindow":
                    Windows.SetForegroundWindowSizePercent(Context.AppWindowPosition, 75);
                    break;

                case "SwitchAppWindow":
                    DialogUtils.ShowTaskSwitcher(IEProcessName);
                    break;

                case "CmdZoomIn":
                    appTextInterface.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Add);
                    appTextInterface.Resume();
                    break;

                case "CmdZoomOut":
                    appTextInterface.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Subtract);
                    appTextInterface.Resume();
                    break;

                case "CmdZoomFit":
                    appTextInterface.Pause();
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D0);
                    appTextInterface.Resume();
                    break;

                case "CmdFind":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.F);
                    break;

                case "IEGoBackward":
                    AgentManager.Instance.Keyboard.Send(Keys.BrowserBack);
                    break;

                case "IEGoForward":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Right);
                    break;

                case "IEHomePage":
                    AgentManager.Instance.Keyboard.Send(Keys.BrowserHome);
                    break;

                case "IEWebSearch":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.E);
                    break;

                case "IEFavorites":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.I);
                    //AgentController.Instance.Send(Keys.BrowserFavorites);
                    break;

                case "IEHistory":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.H);
                    break;

                case "IEAddFavorites":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.D);
                    break;

                case "IERefreshPage":
                    AgentManager.Instance.Keyboard.Send(Keys.BrowserRefresh);
                    break;

                case "IEAddressBar":
                    AgentManager.Instance.Keyboard.Send(Keys.F4);
                    break;

                case "IEZoomMenu":
                    {
                        var monitorInfo = WindowActivityMonitor.GetForegroundWindowInfo();
                        var panelArg = new PanelRequestEventArgs("InternetExplorerZoomMenu",
                            "IExplorer", monitorInfo) { UseCurrentScreenAsParent = true };
                        showPanel(this, panelArg);
                    }

                    break;

                case "IEEmailLink":
                    _explorerElements.EmailPageAsLink();
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Creates the text control interface for the IE control that
        /// is currently in focused
        /// </summary>
        /// <param name="handle">Handle to the IE window</param>
        /// <param name="focusedElement">the focused element</param>
        /// <param name="handled">was this handled?</param>
        /// <returns>the text control object</returns>
        protected override TextControlAgentBase createEditControlTextInterface(IntPtr handle,
                                                                        AutomationElement focusedElement,
                                                                        ref bool handled)
        {
            if (_explorerElements.IsAddressWindow(focusedElement) ||
                _explorerElements.IsSearchControl(focusedElement) ||
                _explorerElements.IsFindControl(focusedElement))
            {
                return new IETextControlAgent(handle, focusedElement, ref handled);
            }

            return base.createEditControlTextInterface(handle, focusedElement, ref handled);
        }

        /// <summary>
        /// Displays the scanner that is appropriate for the element in focus
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        private void autoDisplayScanner(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            handled = true;
            if (_explorerElements.IsFavoritesWindow(monitorInfo.FocusedElement) ||
                            _explorerElements.IsHistoryWindow(monitorInfo.FocusedElement) ||
                            _explorerElements.IsFeedsWindow(monitorInfo.FocusedElement))
            {
                if (!_scannerType.HasFlag(ScannerType.Favorites))
                {
                    var args = new PanelRequestEventArgs(PanelClasses.MenuContextMenu, ScannerTitle, monitorInfo)
                    {
                        UseCurrentScreenAsParent = true
                    };

                    showPanel(this, args);
                    _scannerType = ScannerType.Favorites;
                }
            }
            else if (_explorerElements.IsSearchControl(monitorInfo.FocusedElement) ||
                _explorerElements.IsFindControl(monitorInfo.FocusedElement) ||
                    _explorerElements.IsAddressWindow(monitorInfo.FocusedElement))
            {
                if (!_scannerType.HasFlag(ScannerType.Address))
                {
                    showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                    _scannerType = ScannerType.Address;
                }
            }
            else if (!_scannerType.HasFlag(ScannerType.Other))
            {
                showPanel(this, new PanelRequestEventArgs("InternetExplorerContextMenu", ScannerTitle, monitorInfo));
                _scannerType = ScannerType.Other;
            }
        }
    }
}