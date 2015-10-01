////////////////////////////////////////////////////////////////////////////
// <copyright file="WindowsExplorerAgentBase.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Extension.AppAgents.WindowsExplorer
{
    /// <summary>
    /// Base class application agent for Windows Explorer. Enables
    /// navigation through files and folders in Explorer, create folders,
    /// clipboard operations, opening files etc.
    /// </summary>
    public class WindowsExplorerAgentBase : GenericAppAgentBase
    {
        /// <summary>
        /// If set to true, the agent will autoswitch the
        /// scanners depending on which element has focus.
        /// Eg: Alphabet scanner if an edit text window has focus,
        /// the contextual menu if the main document has focus
        /// </summary>
        protected bool autoSwitchScanners = true;

        /// <summary>
        /// Panel name of the contextual menu for Acrobat reader
        /// </summary>
        private const String ContextualMenuName = "WindowsExplorerContextMenu";

        /// <summary>
        /// This one supports Acrobat reader
        /// </summary>
        private const String ExplorerProcessName = "explorer";

        /// <summary>
        /// Features supported by this agent. Widgets that
        /// correspond to these features will be enabled
        /// </summary>
        private readonly String[] _supportedFeatures =
        {
            "ContextualMenu",
            "SwitchAppWindow"
        };

        /// <summary>
        /// Store the OS version
        /// </summary>
        private Windows.WindowsVersion _osVersion = Windows.GetOSVersion();

        /// <summary>
        /// Has the scanner been shown yet?
        /// </summary>
        private bool _scannerShown;

        /// <summary>
        /// Gets a list of processes supported by this agent
        /// </summary>
        public override IEnumerable<AgentProcessInfo> ProcessesSupported
        {
            get { return new[] { new AgentProcessInfo(ExplorerProcessName) }; }
        }

        /// <summary>
        /// Invoked to set the 'enabled' state of a widget.  This
        /// will depend on the current context.
        /// </summary>
        /// <param name="arg">contains info about the widget</param>
        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            if (arg.Widget.SubClass == "WindowsStartMenu" &&
                (_osVersion == Windows.WindowsVersion.Win8 || _osVersion == Windows.WindowsVersion.Win10))
            {
                arg.Enabled = false;
                arg.Handled = true;
            }
            else
            {
                checkWidgetEnabled(_supportedFeatures, arg);
            }
        }

        /// <summary>
        /// Displays the contextual menu
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        public override void OnContextMenuRequest(WindowActivityMonitorInfo monitorInfo)
        {
            // Do a panel switch to the main document
            //AgentManager.Instance.Keyboard.Send(Keys.F6);
            showPanel(this, new PanelRequestEventArgs(ContextualMenuName, "Explorer", monitorInfo));
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

            if (monitorInfo.IsNewWindow)
            {
                _scannerShown = false;
            }

            if (autoSwitchScanners)
            {
                if (!_scannerShown)
                {
                    displayScanner(monitorInfo, ref handled);
                }
                else
                {
                    handled = true;
                }
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
                    DialogUtils.ShowTaskSwitcher(ExplorerProcessName);
                    break;

                case "NavigateMenu":
                    showMenu("WindowsExplorerNavigateMenu", "Explorer Navigate");
                    break;

                case "FileOpsMenu":
                    showMenu("WindowsExplorerFileOpsMenu", "Explorer File Ops");
                    break;

                case "ClipboardMenu":
                    showMenu("WindowsExplorerClipboardMenu", "Explorer Clipboard");
                    break;

                case "PageUp":
                    AgentManager.Instance.Keyboard.Send(Keys.PageUp);
                    break;

                case "PageDown":
                    AgentManager.Instance.Keyboard.Send(Keys.PageDown);
                    break;

                case "Up":
                    AgentManager.Instance.Keyboard.Send(Keys.Up);
                    break;

                case "Down":
                    AgentManager.Instance.Keyboard.Send(Keys.Down);
                    break;

                case "ParentDir":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Up);
                    break;

                case "Back":
                    AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.Left);
                    break;

                case "Copy":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.C);
                    break;

                case "Cut":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.X);
                    break;

                case "Paste":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.V);
                    break;

                case "Rename":
                    AgentManager.Instance.Keyboard.Send(Keys.F2);
                    break;

                case "Delete":
                    AgentManager.Instance.Keyboard.Send(Keys.Delete);
                    break;

                case "NewFolder":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.LShiftKey, Keys.N);
                    break;

                case "RightClick":
                    AgentManager.Instance.Keyboard.Send(Keys.LShiftKey, Keys.F10);
                    break;

                case "Undo":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.Z);
                    break;

                case "AddressBar":
                    AgentManager.Instance.Keyboard.Send(Keys.F4);
                    break;

                case "NewWindow":
                    AgentManager.Instance.Keyboard.Send(Keys.LControlKey, Keys.N);
                    break;

                case "StartMenu":
                    AgentManager.Instance.Keyboard.Send(Keys.LWin);
                    break;

                case "Search":
                    AgentManager.Instance.Keyboard.Send(Keys.F3);
                    break;

                default:
                    base.OnRunCommand(command, commandArg, ref handled);
                    break;
            }
        }

        /// <summary>
        /// Gets the menu for the Explorer window in focus.  Checks if the
        /// file explorer is active and returns the contextual menu name for it
        /// </summary>
        /// <param name="monitorInfo">fg window info</param>
        /// <param name="panelClass">returns the panel class of the menu</param>
        /// <returns>true if it found a menu for the window</returns>
        protected bool getMenu(WindowActivityMonitorInfo monitorInfo, ref String panelClass)
        {
            AutomationElement windowElement = getWindowElement(monitorInfo.FgHwnd);

            bool retVal = true;
            if (windowElement != null)
            {
                if (isFileExplorer(windowElement))
                {
                    Log.Debug("KILLROY isFileExploer is TRUE ");
                    if (Windows.IsMinimized(monitorInfo.FgHwnd))
                    {
                        retVal = false;
                    }
                    else
                    {
                        panelClass = ContextualMenuName;
                    }
                }
                else
                {
                    Log.Debug("KILLROY isFileExploer is FALSE");
                    retVal = false;
                }
            }
            else
            {
                Log.Debug("KILLROY windowElement is NULL");

            }

            Log.Debug("KILLROY return from getmenu : " + retVal);
            return retVal;
        }

        /// <summary>
        /// Returns the automation element for the window handle
        /// </summary>
        /// <param name="hWndMain">window handle</param>
        /// <returns>automation element</returns>
        protected AutomationElement getWindowElement(IntPtr hWndMain)
        {
            AutomationElement retVal = null;
            try
            {
                retVal = AutomationElement.FromHandle(hWndMain);
            }
            catch
            {
                retVal = null;
            }
            return retVal;
        }

        /// <summary>
        /// Checks if the focused element is the desktop
        /// </summary>
        /// <param name="focusedElement">element in focus</param>
        /// <returns>true if it is</returns>
        protected bool isDesktopWindow(AutomationElement focusedElement)
        {
            Log.Debug();
            var walker = TreeWalker.ControlViewWalker;

            AutomationElement parent = focusedElement;

            while (parent != null)
            {
                Log.Debug("class: " + parent.Current.ClassName + ", ctrltype: " + parent.Current.ControlType.ProgrammaticName + ", name: " + parent.Current.Name);
                if (parent == AutomationElement.RootElement ||
                    (String.Compare(parent.Current.ClassName, "SysListView32", true) == 0 &&
                        String.Compare(parent.Current.ControlType.ProgrammaticName, "ControlType.List", true) == 0 &&
                        String.Compare(parent.Current.AutomationId, "1", true) == 0 &&
                        String.Compare(parent.Current.Name, "FolderView", true) == 0) ||
                    (String.Compare(parent.Current.ClassName, "Shell_TrayWnd", true) == 0 &&
                        String.Compare(parent.Current.ControlType.ProgrammaticName, "ControlType.Pane", true) == 0))
                {
                    Log.Debug("Returning true");
                    return true;
                }
                parent = walker.GetParent(parent);
            }
            Log.Debug("Returning false");
            return false;
        }

        /// <summary>
        /// Checks if the window element is the Windows Explorer window
        /// </summary>
        /// <param name="windowElement">element in focus</param>
        /// <returns>true if it is</returns>
        protected bool isFileExplorer(AutomationElement windowElement)
        {
            return (windowElement != null) ? (String.Compare(windowElement.Current.ClassName, "CabinetWClass") == 0 &&
                String.Compare(windowElement.Current.ControlType.ProgrammaticName, "ControlType.Window") == 0) : false;
        }

        /// <summary>
        /// Displays the scanner appropriate for the element that is currently
        /// in focus in the Windows explorer window
        /// </summary>
        /// <param name="monitorInfo">Foreground window info</param>
        /// <param name="handled">set to true if handled</param>
        private void displayScanner(WindowActivityMonitorInfo monitorInfo, ref bool handled)
        {
            Log.Debug("KILLROY Entered");
            if (monitorInfo.FocusedElement.Current.ControlType.ProgrammaticName == "ControlType.Edit")
            {
                Log.Debug("KILLROY controtype edit");

                base.OnFocusChanged(monitorInfo, ref handled);
                showPanel(this, new PanelRequestEventArgs(PanelClasses.Alphabet, monitorInfo));
                _scannerShown = true;
            }
            else// if (monitorInfo.IsNewWindow)
            {
                Log.Debug("KILLROY calling getmenu");

                String panel = PanelClasses.None;
                handled = getMenu(monitorInfo, ref panel);

                if (handled)
                {
                    showPanel(this, new PanelRequestEventArgs(panel, "Explorer", monitorInfo));
                    _scannerShown = true;
                }
            }
#if abc
            else
            {
                Log.Debug("KILLROY *** FALL THROUGH>  WILL NOT BE HANDLED");

                //handled = true;
            }
#endif
        }

        /// <summary>
        /// Dispays the menu indicated by the menu name
        /// </summary>
        /// <param name="menuName">name of the menu</param>
        /// <param name="title">Title of the menu</param>
        private void showMenu(String menuName, String title)
        {
            var panelArg = new PanelRequestEventArgs(menuName, title, WindowActivityMonitor.GetForegroundWindowInfo())
            {
                UseCurrentScreenAsParent = true
            };

            showPanel(this, panelArg);
        }
    }
}