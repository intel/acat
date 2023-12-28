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
    /// Sends a function key (F1 to F12) to the keyboard
    /// buffer.  The Command field should be a string
    /// representation of the function key.  Eg "F1" for F1
    /// and so on
    /// </summary>
    public class FunctionKeyHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public FunctionKeyHandler(String cmd)
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
                Log.Exception(ex);
                retVal = false;
            }

            return retVal;
        }
    }
}