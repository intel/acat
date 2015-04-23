////////////////////////////////////////////////////////////////////////////
// <copyright file="LectureManagerTextControlAgent.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.AgentManagement.TextInterface;
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

namespace ACAT.Extensions.Hawking.FunctionalAgents.LectureManager
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
        private readonly String[] _supportedFeatures =
        {
            "PreviousPara",
            "NextPara",
            "PreviousSentence",
            "NextSentence",
            "TopOfDoc",
            "EndOfDoc",
            "PreviousPage",
            "NextPage",
            "PreviousLine",
            "NextLine",
            "PreviousChar",
            "NextChar",
            "PreviousWord",
            "NextWord",
            "Home",
            "End"
        };

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>

        public override void CheckWidgetEnabled(CheckEnabledArgs arg)
        {
            checkWidgetEnabled(_supportedFeatures, arg);
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