////////////////////////////////////////////////////////////////////////////
// <copyright file="ShowAbbreviationsHandler.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using System;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Activates the Abbreivations functional agent to allow
    /// the user to manage abbreviations.
    /// </summary>
    public class ShowAbbreviationsHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public ShowAbbreviationsHandler(String cmd)
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

            if (!Context.AppAgentMgr.CanActivateFunctionalAgent())
            {
                return false;
            }

            handleAbbreviationSettings();

            return true;
        }

        /// <summary>
        /// Launches the abbreivations agent
        /// </summary>
        private async void handleAbbreviationSettings()
        {
            IApplicationAgent abbrAgent = Context.AppAgentMgr.GetAgentByCategory("AbbreviationsAgent");
            if (abbrAgent != null)
            {
                await Context.AppAgentMgr.ActivateAgent(abbrAgent as IFunctionalAgent);
            }
        }
    }
}