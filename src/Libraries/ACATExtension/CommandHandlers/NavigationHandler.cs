////////////////////////////////////////////////////////////////////////////
// <copyright file="NavigationHandler.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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

using System;
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Handles document navigation functions to move the
    /// caret around the document, page up, page down, etc
    /// </summary>
    public class NavigationHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public NavigationHandler(String cmd)
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
            bool retVal = true;

            handled = true;

            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    switch (Command)
                    {
                        case "CmdPrevChar":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.PreviousCharacter);
                            break;

                        case "CmdNextChar":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.NextCharacter);
                            break;

                        case "CmdPrevLine":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.PreviousLine);
                            break;

                        case "CmdNextLine":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.NextLine);
                            break;

                        case "CmdPrevWord":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.PreviousWord);
                            break;

                        case "CmdNextWord":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.NextWord);
                            break;

                        case "CmdPrevPara":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.PreviousParagaph);
                            break;

                        case "CmdNextPara":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.NextParagraph);
                            break;

                        case "CmdPrevPage":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.PreviousPage);
                            break;

                        case "CmdNextPage":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.NextPage);
                            break;

                        case "CmdHome":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.Home);
                            break;

                        case "CmdEnd":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.End);
                            break;

                        case "CmdTopOfDoc":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.TopOfDocument);
                            break;

                        case "CmdEndOfDoc":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.EndOfDocument);
                            break;

                        default:
                            handled = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// If the sticky shift is turned on, turn on select mode
        /// </summary>
        private void turnOnSelectModeIfStickyShiftEnabled()
        {
            try
            {
                if (KeyStateTracker.IsStickyShiftOn())
                {
                    using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                    {
                        context.TextAgent().SetSelectMode(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}