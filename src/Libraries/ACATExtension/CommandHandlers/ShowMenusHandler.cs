////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Displays the various menus
    /// </summary>
    public class ShowMenusHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public ShowMenusHandler(String cmd)
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
                case "CmdMainMenu":
                    mainMenuHandler();
                    break;

                case "CmdSettingsMenu":
                    settingsMenuHandler();
                    break;

                case "CmdToolsMenu":
                    toolsMenuHandler();
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Displays the main menu
        /// </summary>
        private void mainMenuHandler()
        {
            var panelClass = "MainMenu";

            if (Context.AppPanelManager.IsCurrentPanelClass(panelClass))
            {
                return;
            }

            Form form = Dispatcher.Scanner.Form;

            if (Windows.GetVisible(form))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    IPanel mainMenu = Context.AppPanelManager.CreatePanel(panelClass) as IPanel;
                    if (mainMenu != null)
                    {
                        Context.AppPanelManager.ShowDialog(form as IPanel, mainMenu);
                    }
                }));
            }
        }

        /// <summary>
        /// Displays the Settings menu
        /// </summary>
        private void settingsMenuHandler()
        {
            var panelClass = "SettingsMenu";

            if (Context.AppPanelManager.IsCurrentPanelClass(panelClass))
            {
                return;
            }

            Form settingsMenuForm = Context.AppPanelManager.CreatePanel(panelClass);
            if (settingsMenuForm != null)
            {
                Context.AppPanelManager.ShowDialog(settingsMenuForm as IPanel);
            }
        }

        /// <summary>
        /// Displays the Tools menu
        /// </summary>
        private void toolsMenuHandler()
        {
            var panelClass = "ToolsMenu";

            if (Context.AppPanelManager.IsCurrentPanelClass(panelClass))
            {
                return;
            }

            Form form = Dispatcher.Scanner.Form;
            if (Windows.GetVisible(form))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    IPanel mainMenu = Context.AppPanelManager.CreatePanel(panelClass) as IPanel;
                    if (mainMenu != null)
                    {
                        Context.AppPanelManager.ShowDialog(form as IPanel, mainMenu);
                    }
                }));
            }
        }
    }
}