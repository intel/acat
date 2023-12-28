////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using System;

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
                scannerPanel?.ScannerCommon?.AnimationManager?.Restart();
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