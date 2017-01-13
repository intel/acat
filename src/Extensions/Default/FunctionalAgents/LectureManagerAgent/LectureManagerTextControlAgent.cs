////////////////////////////////////////////////////////////////////////////
// <copyright file="LectureManagerTextControlAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.Utility;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.LectureManager
{
    /// <summary>
    /// Represents the text control object for the lecture manager.
    /// Handles navigation by sending hot key shortcuts to the lecture
    /// manager form
    /// </summary>
    public class LectureManagerTextControlAgent : TextControlAgentBase
    {
        /// <summary>
        /// Features supported by this agent
        /// </summary>
        private readonly String[] _supportedCommands =
        {
            "CmdPrevPara",
            "CmdNextPara",
            "CmdTopOfDoc",
            "CmdEndOfDoc",
            "CmdPrevPage",
            "CmdNextPage",
            "CmdPrevLine",
            "CmdNextLine",
            "CmdPrevChar",
            "CmdNextChar",
            "CmdPrevWord",
            "CmdNextWord",
            "CmdHome",
            "CmdEnd"
        };

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>

        public override void CheckCommandEnabled(CommandEnabledArg arg)
        {
            checkCommandEnabled(_supportedCommands, arg);
        }

        /// <summary>
        /// Handles text navigation
        /// </summary>
        /// <param name="gotoItem">how to navigate</param>
        /// <returns>true on success</returns>
        public override bool Goto(GoToItem gotoItem)
        {
            Log.Debug("GoToItem=" + gotoItem.ToString());

            switch (gotoItem)
            {
                case GoToItem.TopOfDocument:
                    Keyboard.Send(Keys.I);
                    break;

                case GoToItem.PreviousParagaph:
                    Keyboard.Send(Keys.M);
                    break;

                case GoToItem.NextParagraph:
                    Keyboard.Send(Keys.K);
                    break;

                case GoToItem.PreviousSentence:
                    Keyboard.Send(Keys.X);
                    break;

                case GoToItem.NextSentence:
                    Keyboard.Send(Keys.W);
                    break;

                default:
                    return base.Goto(gotoItem);
            }

            return true;
        }
    }
}