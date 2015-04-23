////////////////////////////////////////////////////////////////////////////
// <copyright file="MainMenu.cs" company="Intel Corporation">
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
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;

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

namespace ACAT.Extensions.Default.UI.ContextMenus
{
    /// <summary>
    /// Form for the main menu for the application. Commands
    /// are handled by the command dispatcher in the base class
    /// </summary>
    [DescriptorAttribute("148257A1-A8B7-4E75-93F0-56AFCD5B2A3E", "MainMenu", "Main Menu")]
    public partial class MainMenu : ContextualMenu
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Panel class of the scanner</param>
        /// <param name="panelTitle">title of the panel (not used)</param>
        public MainMenu(String panelClass, String panelTitle)
            : base(panelClass, "Main Menu")
        {
            // add commands that are not supported by the base class
            commandDispatcher.Commands.Add(new CommandHandler("Exit"));
        }

        /// <summary>
        /// User wants to quit the app. Confirm and exit
        /// </summary>
        private void confirmAndQuitApplication()
        {
            Invoke(new MethodInvoker(delegate()
            {
                if (DialogUtils.ConfirmScanner(null, "Quit?"))
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