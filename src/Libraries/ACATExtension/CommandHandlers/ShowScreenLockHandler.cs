////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Displays the ACAT Screen Lock dialog.  This locks the display
    /// and the user has to enter a PIN to unlock the display.
    /// </summary>
    public class ShowScreenLockHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public ShowScreenLockHandler(String cmd)
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
            bool retVal = true;

            Form form = Dispatcher.Scanner.Form;
            form.Invoke(new MethodInvoker(delegate
            {
                if (DialogUtils.ConfirmScanner(form as IPanel, R.GetString("LockTheScreen")))
                {
                    var screenLockForm = PanelManager.Instance.CreatePanel("ScreenLockScanner", "Lock Screen");
                    if (screenLockForm != null)
                    {
                        WindowActivityMonitor.Pause();
                        Context.AppPanelManager.ShowDialog(screenLockForm as IPanel);
                        WindowActivityMonitor.Resume();
                    }
                    else
                    {
                        retVal = false;
                    }
                }
            }));

            return retVal;
        }
    }
}