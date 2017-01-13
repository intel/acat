////////////////////////////////////////////////////////////////////////////
// <copyright file="MiscCommandHandler.cs" company="Intel Corporation">
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
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    public class MiscCommandHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public MiscCommandHandler(String cmd)
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
                case "CmdRestartScanning":
                    restartScanningHandler();
                    break;

                case "CmdSwitchLanguage":
                    switchLanguageHandler();
                    break;

                case "CmdExitAppWithConfirm":
                    if (DialogUtils.ConfirmScanner(null, R.GetString("QuitApplication")))
                    {
                        quitApplication();
                    }
                    break;

                case "CmdExitApp":
                    quitApplication();
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Quits the application
        /// </summary>
        private void quitApplication()
        {
            Context.AppQuit = true;
            PanelManager.Instance.CloseCurrentForm();
        }

        /// <summary>
        /// Restarts scanning sequence
        /// </summary>
        private void restartScanningHandler()
        {
            if (Dispatcher.Scanner.Form is IScannerPanel)
            {
                var scannerPanel = Dispatcher.Scanner.Form as IScannerPanel;
                scannerPanel.ScannerCommon.AnimationManager.Restart();
            }
        }

        /// <summary>
        /// Displays the dialog to switch the language
        /// </summary>
        private void switchLanguageHandler()
        {
            var form = Context.AppPanelManager.CreatePanel("SwitchLanguageScanner");
            if (form != null)
            {
                Context.AppPanelManager.ShowDialog(form as IPanel);
            }
        }
    }
}