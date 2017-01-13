////////////////////////////////////////////////////////////////////////////
// <copyright file="GoBackHandler.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Handler for closing the currently active scanner.  Typically
    /// invoked when the user presses the "Back" button on the scanner.
    /// Closing the scanner will automatically display the parent scanner.
    /// If the scanner doesn't have a parent, displays the alphabet scanner
    /// </summary>
    public class GoBackHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public GoBackHandler(String cmd)
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

            // close the form.  If the form doesn't have
            // a parent, just activate the default scanner

            bool hasParent = form.Owner != null;

            Log.Debug("form: " + form.Name + ", hasParent: " + hasParent);

            Windows.CloseForm(form);
            if (!hasParent)
            {
                IPanel panel = Context.AppPanelManager.CreatePanel(PanelClasses.Alphabet) as IPanel;
                Context.AppPanelManager.Show(panel);
            }

            return true;
        }
    }
}