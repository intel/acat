////////////////////////////////////////////////////////////////////////////
// <copyright file="AppWindowManagementHandler.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Extensions;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Automation;
using System.Windows.Forms;

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

            IntPtr fgHandle = Windows.GetForegroundWindow();

            switch (Command)
            {
                case "CmdMaximizeWindow":
                    if (isWindowSizeable(fgHandle))
                    {
                        Windows.MaximizeWindow(fgHandle);
                    }
                    break;

                case "CmdRestoreWindow":
                    if (isWindowSizeable(fgHandle))
                    {
                        Windows.RestoreWindow(fgHandle);
                    }
                    break;

                case "CmdCloseWindow":
                    var info = WindowActivityMonitor.GetForegroundWindowInfo();
                    WindowHighlight win = null;
                    if (info.FgHwnd != IntPtr.Zero)
                    {
                        win = new WindowHighlight(info.FgHwnd, Dispatcher.Scanner.Form);
                    }

                    if (DialogUtils.ConfirmScanner(null, R.GetString("CloseHighlightedWindow")))
                    {
                        AgentManager.Instance.Keyboard.Send(Keys.LMenu, Keys.F4);
                    }

                    if (win != null)
                    {
                        win.Dispose();
                    }

                    break;

                case "CmdMoveWindow":
                    if (!isWindowSizeable(fgHandle))
                    {
                        break;
                    }

                    Dispatcher.Scanner.Form.Invoke(new MethodInvoker(delegate
                    {
                        Form form = Context.AppPanelManager.CreatePanel("WindowMoveResizeScannerForm");
                        if (form != null)
                        {
                            form.Text = R.GetString("MoveWindow");

                            var extension = form as IExtension;
                            if (extension != null)
                            {
                                extension.GetInvoker().SetValue("MoveWindow", true);
                            }

                            Context.AppPanelManager.ShowDialog(form as IPanel);
                        }
                    }));
                    break;

                case "CmdSizeWindow":
                    if (!isWindowSizeable(fgHandle))
                    {
                        break;
                    }

                    Dispatcher.Scanner.Form.Invoke(new MethodInvoker(delegate
                    {
                        Form form = Context.AppPanelManager.CreatePanel("WindowMoveResizeScannerForm");
                        if (form != null)
                        {
                            form.Text = R.GetString("ResizeWindow");

                            var extension = form as IExtension;
                            if (extension != null)
                            {
                                extension.GetInvoker().SetValue("ResizeWindow", true);
                            }

                            Context.AppPanelManager.ShowDialog(form as IPanel);
                        }
                    }));
                    break;

                case "CmdMinimizeWindow":
                    if (isWindowMinimizable(fgHandle))
                    {
                        Windows.MinimizeWindow(User32Interop.GetForegroundWindow());
                    }
                    break;

                case "CmdMaxRestoreWindow":
                    if (!isWindowMaximizable(fgHandle))
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

                case "CmdSnapWindow":
                    if (!isWindowSizeable(fgHandle))
                    {
                        break;
                    }

                    Dispatcher.Scanner.Form.Invoke(new MethodInvoker(delegate
                    {
                        Windows.SetForegroundWindowSizePercent(Context.AppWindowPosition,
                                                                Common.AppPreferences.WindowSnapSizePercent);
                    }));
                    break;

                case "CmdSnapWindowToggle":
                    if (!isWindowSizeable(fgHandle))
                    {
                        break;
                    }

                    Windows.ToggleSnapForegroundWindow(Context.AppWindowPosition,
                                                                Common.AppPreferences.WindowSnapSizePercent);
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Checks if the window has a TitleBar control and returns
        /// the element
        /// </summary>
        /// <param name="element">window automation element</param>
        /// <returns>the element</returns>
        private AutomationElement findTitleBar(AutomationElement element)
        {
            return AgentUtils.FindElementByAutomationId(element, "", ControlType.TitleBar, "TitleBar");
        }

        /// <summary>
        /// Checks if the window can be maximized
        /// </summary>
        /// <param name="fgHandle">window handle</param>
        /// <returns>true if it can</returns>
        private bool isWindowMaximizable(IntPtr fgHandle)
        {
            if (fgHandle == IntPtr.Zero)
            {
                return false;
            }

            var window = AutomationElement.FromHandle(fgHandle);

            if (!Windows.IsApplicationFrameHostProcessWindow(fgHandle))
            {
                object objPattern;
                Log.Debug("controltype: " + window.Current.ControlType.ProgrammaticName);

                bool retVal = false;
                if (window.TryGetCurrentPattern(WindowPattern.Pattern, out objPattern))
                {
                    var windowPattern = objPattern as WindowPattern;
                    retVal = (windowPattern.Current.CanMaximize) && !windowPattern.Current.IsModal;

                    Log.Debug("canmaximize: " + windowPattern.Current.CanMaximize + ", ismodal: " + windowPattern.Current.IsModal);
                }

                return retVal;
            }

            var titleBar = findTitleBar(window);

            if (titleBar == null)
            {
                return false;
            }

            var maximizeButton = AgentUtils.FindElementByAutomationId(titleBar, "", ControlType.Button, "Maximize");
            var restoreButton = AgentUtils.FindElementByAutomationId(titleBar, "", ControlType.Button, "Restore");

            return maximizeButton != null || restoreButton != null;
        }

        /// <summary>
        /// Checks if the window can be minimized
        /// </summary>
        /// <param name="fgHandle">window handle</param>
        /// <returns>true if it can</returns>
        private bool isWindowMinimizable(IntPtr fgHandle)
        {
            if (fgHandle == IntPtr.Zero)
            {
                return false;
            }

            var window = AutomationElement.FromHandle(fgHandle);

            if (!Windows.IsApplicationFrameHostProcessWindow(fgHandle))
            {
                object objPattern;
                Log.Debug("controltype: " + window.Current.ControlType.ProgrammaticName);

                bool retVal = false;
                if (window.TryGetCurrentPattern(WindowPattern.Pattern, out objPattern))
                {
                    var windowPattern = objPattern as WindowPattern;
                    retVal = (windowPattern.Current.CanMinimize) && !windowPattern.Current.IsModal;

                    Log.Debug("canminimize: " + windowPattern.Current.CanMinimize + ", ismodal: " + windowPattern.Current.IsModal);
                }

                return retVal;
            }

            var titleBar = findTitleBar(window);

            if (titleBar == null)
            {
                return false;
            }

            return AgentUtils.FindElementByAutomationId(titleBar, "", ControlType.Button, "Minimize") != null;
        }

        /// <summary>
        /// Checks if the window can be resized
        /// </summary>
        /// <param name="fgHandle">window handle</param>
        /// <returns>true if it can</returns>
        private bool isWindowSizeable(IntPtr fgHandle)
        {
            if (fgHandle == IntPtr.Zero)
            {
                return false;
            }

            bool retVal = isWindowMaximizable(fgHandle) || isWindowMinimizable(fgHandle);

            Log.Debug("returning " + retVal);

            return retVal;
        }
    }
}