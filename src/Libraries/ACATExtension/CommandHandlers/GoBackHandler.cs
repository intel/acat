////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Common;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace ACAT.Extension.CommandHandlers
{
    /// <summary>
    /// Handler for closing the currently active scanner.  Typically
    /// invoked when the user presses the "Back" button on the scanner.
    /// Closing the scanner will automatically display the parent scanner.
    /// If the scanner doesn't have a parent, displays the alphabet scanner
    /// </summary>
    public class GoBackHandler : RunCommandHandler
    {
        private readonly ILogger<GoBackHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        /// <param name="logger">Logger instance</param>
        public GoBackHandler(String cmd, ILogger<GoBackHandler> logger)
            : base(cmd)
        {
            _logger = logger;
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

            _logger.LogDebug("form: " + form.Name + ", hasParent: " + hasParent);

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