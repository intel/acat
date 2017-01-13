////////////////////////////////////////////////////////////////////////////
// <copyright file="MainMenu.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.UI.Menus
{
    /// <summary>
    /// Form for the main menu for the application. Commands
    /// are handled by the command dispatcher in the base class
    /// </summary>
    [DescriptorAttribute("148257A1-A8B7-4E75-93F0-56AFCD5B2A3E",
                        "MainMenu",
                        "Main AppMenu")]
    public partial class MainMenu : MenuPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Panel class of the scanner</param>
        /// <param name="panelTitle">title of the panel (not used)</param>
        public MainMenu(String panelClass, String panelTitle)
            : base(panelClass, R.GetString("MainMenu"))
        {
            // add commands that are not supported by the base class
            commandDispatcher.Commands.Add(new CommandHandler("Exit"));
        }

        /// <summary>
        /// User wants to quit the app. Confirm and exit
        /// </summary>
        private void confirmAndQuitApplication()
        {
            Invoke(new MethodInvoker(delegate
            {
                if (DialogUtils.ConfirmScanner(null, R.GetString("QuitApplication")))
                {
                    quitApplication();
                }
            }));
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        private void quitApplication()
        {
            Context.AppQuit = true;

            if (Owner != null)
            {
                Owner.Close();
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Command dispatcher to handles commands from
        /// the menu itmes in the main menu. Executes
        /// commands that are not supported by the base class
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">the command</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">was the command handled?</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as MainMenu;

                switch (Command)
                {
                    case "Exit":
                        form.confirmAndQuitApplication();
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }
        }
    }
}