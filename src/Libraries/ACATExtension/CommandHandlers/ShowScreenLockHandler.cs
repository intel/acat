////////////////////////////////////////////////////////////////////////////
// <copyright file="ShowScreenLockHandler.cs" company="Intel Corporation">
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

            Form form = Dispatcher.Scanner.Form;
            form.Invoke(new MethodInvoker(delegate
            {
                if (DialogUtils.ConfirmScanner(form as IPanel, R.GetString("LockTheScreen")))
                {
                    Form screenLockForm = Context.AppPanelManager.CreatePanel("ScreenLockForm");
                    if (screenLockForm != null)
                    {
                        WindowActivityMonitor.Pause();
                        Context.AppPanelManager.ShowDialog(form as IPanel, screenLockForm as IPanel);
                    }
                }
            }));

            return true;
        }
    }
}