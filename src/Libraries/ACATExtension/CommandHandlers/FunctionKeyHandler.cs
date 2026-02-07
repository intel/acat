////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Forms;

namespace ACAT.Extension.CommandHandlers
{
    /// <summary>
    /// Sends a function key (F1 to F12) to the keyboard
    /// buffer.  The Command field should be a string
    /// representation of the function key.  Eg "F1" for F1
    /// and so on
    /// </summary>
    public class FunctionKeyHandler : RunCommandHandler
    {
        private readonly ILogger<FunctionKeyHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        /// <param name="logger">Logger instance</param>
        public FunctionKeyHandler(String cmd, ILogger<FunctionKeyHandler> logger)
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
            bool retVal;

            handled = true;

            try
            {
                retVal = sendFunctionKey(Command);
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        protected bool sendFunctionKey(String keyString)
        {
            bool retVal = true;
            try
            {
                Keys key = (Keys)Enum.Parse(typeof(Keys), keyString, true);
                Context.AppAgentMgr.Keyboard.Send(KeyStateTracker.GetExtendedKeys(), key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                retVal = false;
            }

            return retVal;
        }
    }
}