////////////////////////////////////////////////////////////////////////////
// <copyright file="FunctionKeyHandler.cs" company="Intel Corporation">
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