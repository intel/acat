////////////////////////////////////////////////////////////////////////////
// <copyright file="AppWindowManagementHandler.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
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

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Command handler to manipulate the foreground window
    /// size and position, as well as window state such as
    /// maximize, minimize, restore etc.
    /// </summary>
    public class AppWindowManagementHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public AppWindowManagementHandler(String cmd)
            : base(cmd)
        {
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="handled">set to true if the command was handled</param>
        /// <returns>true on success</returns>
        public override bool Execute(ref bool handled)
        {
            handled = true;

            switch (Command)
            {
                case "CmdMaximizeWindow":
                    if (isForegroundWindowSizeable())
                    {
                        Windows.MaximizeWindow(Windows.GetForegroundWindow());
                    }
                    break;

                case "CmdRestoreWindow":
                    if (isForegroundWindowSizeable())
                    {
                        Windows.RestoreWindow(Windows.GetForegroundWindow());
                    }
                    break;

                case "CmdCloseWindow":
                    var info = WindowActivityMonitor.GetForegroundWindowInfo();
                    WindowHighlight win = null;
                    if (info.FgHwnd != IntPtr.Zero)
                    {
                        win = new WindowHighlight(info.FgHwnd, Dispatcher.Scanner.Form);
                    }

                    if (DialogUtils.ConfirmScanner(null, "Close the highlighted window?"))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F4);
                    }

                    if (win != null)
                    {
                        win.Dispose();
                    }

                    break;

                case "CmdMoveWindow":
                    if (!isForegroundWindowSizeable())
                    {
                        break;
                    }

                    Dispatcher.Scanner.Form.Invoke(new MethodInvoker(delegate()
                    {
                        const int delay = 500;

                        Context.AppAgentMgr.Keyboard.Send(Keys.LMenu, Keys.Space);
                        Thread.Sleep(delay);
                        Context.AppAgentMgr.Keyboard.Send(Keys.M);

                        Form form = Context.AppPanelManager.CreatePanel("WindowMoveResizeScannerForm");
                        if (form != null)
                        {
                            form.Text = "Move Window";
                            Context.AppPanelManager.ShowDialog(form as IPanel);
                        }
                    }));
                    break;

                case "CmdSizeWindow":
                    if (!isForegroundWindowSizeable())
                    {
                        break;
                    }

                    Dispatcher.Scanner.Form.Invoke(new MethodInvoker(delegate()
                    {
                        int delay = 500;

                        Context.AppAgentMgr.Keyboard.Send(Keys.LMenu, Keys.Space);
                        Thread.Sleep(delay);
                        Context.AppAgentMgr.Keyboard.Send(Keys.S);

                        Form form = Context.AppPanelManager.CreatePanel("WindowMoveResizeScannerForm");
                        if (form != null)
                        {
                            form.Text = "Resize Window";
                            Context.AppPanelManager.ShowDialog(form as IPanel);
                        }
                    }));
                    break;

                case "CmdMinimizeWindow":
                    if (isForegroundWindowSizeable())
                    {
                        Windows.MinimizeWindow(User32Interop.GetForegroundWindow());
                    }
                    break;

                case "CmdMaxRestoreWindow":
                    if (!isForegroundWindowSizeable())
                    {
                        break;
                    }

                    IntPtr handle = User32Interop.GetForegroundWindow();
                    if (Windows.IsMaximized(handle))
                    {
                        Windows.RestoreWindow(handle);
                    }
                    else
                    {
                        Windows.MaximizeWindow(handle);
                    }

                    break;

                case "CmdThreeFourthMaximizeWindow":
                    if (!isForegroundWindowSizeable())
                    {
                        break;
                    }

                    Dispatcher.Scanner.Form.Invoke(new MethodInvoker(delegate()
                    {
                        Windows.SetForegroundWindowSizePercent(Context.AppWindowPosition,
                                                                Common.AppPreferences.WindowMaximizeSizePercent);
                    }));
                    break;

                case "CmdMaximizeThreeFourthToggle":
                    if (!isForegroundWindowSizeable())
                    {
                        break;
                    }

                    Windows.ToggleFgWindowMaximizeAndPartialMaximize(Context.AppWindowPosition,
                                                                Common.AppPreferences.WindowMaximizeSizePercent);
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Checks if the foreground window is sizeable
        /// </summary>
        /// <returns>true if it is</returns>
        private bool isForegroundWindowSizeable()
        {
            IntPtr fgHandle = Windows.GetForegroundWindow();
            if (fgHandle == IntPtr.Zero)
            {
                return false;
            }

            bool retVal = false;

            var window = AutomationElement.FromHandle(fgHandle);
            object objPattern;
            Log.Debug("controltype: " + window.Current.ControlType.ProgrammaticName);

            if (window.TryGetCurrentPattern(WindowPattern.Pattern, out objPattern))
            {
                var windowPattern = objPattern as WindowPattern;
                retVal = (windowPattern.Current.CanMinimize ||
                            windowPattern.Current.CanMaximize) &&
                            !windowPattern.Current.IsModal;

                Log.Debug("canminimize: " + windowPattern.Current.CanMinimize +
                            ", ismodal: " + windowPattern.Current.IsModal);
            }

            Log.Debug("returning " + retVal);

            return retVal;
        }
    }
}