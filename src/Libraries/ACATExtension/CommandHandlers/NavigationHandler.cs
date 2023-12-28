////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement;
using ACAT.Lib.Core.AgentManagement.TextInterface;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;

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

                        case "CmdPrevSentence":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.PreviousSentence);
                            break;

                        case "CmdNextSentence":
                            turnOnSelectModeIfStickyShiftEnabled();
                            context.TextAgent().Goto(GoToItem.NextSentence);
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
        /// If the sticky shift is turned on, turn on select mode. If select
        /// mode is on, text is automatically selected as the user moves
        /// the cursor.
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