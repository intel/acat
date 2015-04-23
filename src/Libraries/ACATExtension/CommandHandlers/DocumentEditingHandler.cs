////////////////////////////////////////////////////////////////////////////
// <copyright file="DocumentEditingHandler.cs" company="Intel Corporation">
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
using System.Windows.Forms;
using ACAT.Lib.Core.AgentManagement;
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
    /// Takes care of document editing operations such as
    /// selecting text, clipboard operations, deleting
    /// words, characters etc.
    /// </summary>
    public class DocumentEditingHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public DocumentEditingHandler(String cmd)
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
                        case "CmdUndo":
                            context.TextAgent().Undo();
                            break;

                        case "CmdRedo":
                            context.TextAgent().Redo();
                            break;

                        case "CmdSelectModeToggle":
                            context.TextAgent().SetSelectMode(!context.TextAgent().GetSelectMode());
                            break;

                        case "CmdFind":
                            Context.AppAgentMgr.RunCommand("CmdFind", ref handled);
                            break;

                        case "CmdSelectAll":
                            context.TextAgent().SelectAll();
                            break;

                        case "CmdDeletePrevChar":
                            Context.AppAgentMgr.Keyboard.Send(Keys.Back);
                            break;

                        case "CmdDeleteNextChar":
                            Context.AppAgentMgr.Keyboard.Send(Keys.Delete);
                            break;

                        case "CmdDeletePrevWord":
                            Dispatcher.Scanner.TextController.UndoLastAction();
                            break;

                        case "CmdCut":
                            context.TextAgent().Cut();
                            turnOffSelectMode();  // TODO move to cursor scanner
                            break;

                        case "CmdCopy":
                            context.TextAgent().Copy();
                            turnOffSelectMode();
                            break;

                        case "CmdPaste":
                            context.TextAgent().Paste();
                            turnOffSelectMode();
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
        /// Turns of the select.
        /// </summary>
        private void turnOffSelectMode()
        {
            KeyStateTracker.ClearAll();

            try
            {
                using (AgentContext context = Context.AppAgentMgr.ActiveContext())
                {
                    context.TextAgent().SetSelectMode(false);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }
    }
}