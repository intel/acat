////////////////////////////////////////////////////////////////////////////
// <copyright file="OutlookAgentKeyLoggerTextInterface.cs" company="Intel Corporation">
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

using System.Diagnostics.CodeAnalysis;
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

namespace ACAT.Lib.Extension.AppAgents.Outlook
{
    /// <summary>
    /// Key logger text interface that can be used to switch
    /// on/off learning, abbr expansion, spell check and
    /// smart punctuations
    /// </summary>
    public class OutlookAgentKeyLoggerTextInterface : KeyLogTextControlAgent
    {
        private bool _enableLearn;

        private bool _enableSmartPunctuations;
        private bool _enableSpellCheck;
        private bool _expandAbbreviations;

        /// <summary>
        /// Instantiates a new instance of the class
        /// </summary>
        /// <param name="enableLearn">enable learning?</param>
        /// <param name="expandAbbreviations">expand abbreviations?</param>
        /// <param name="enableSpellCheck">enable spellcheck?</param>
        /// <param name="enableSmartPunctuations">enable smart punctuations?</param>
        public OutlookAgentKeyLoggerTextInterface(bool enableLearn = true, 
                                                bool expandAbbreviations = true, 
                                                bool enableSpellCheck = true, 
                                                bool enableSmartPunctuations = true)
        {
            Log.Debug();

            _enableLearn = enableLearn;
            _expandAbbreviations = expandAbbreviations;
            _enableSpellCheck = enableSpellCheck;
            _enableSmartPunctuations = enableSmartPunctuations;
        }

        /// <summary>
        /// Enables/disables smart punctuations
        /// </summary>
        /// <returns>whether to enable or not</returns>
        public override bool EnableSmartPunctuations()
        {
            return _enableSmartPunctuations;
        }

        /// <summary>
        /// Enables/Disables abbreviations expansion
        /// </summary>
        /// <returns>whether to expand abbr or not</returns>
        public override bool ExpandAbbreviations()
        {
            return _expandAbbreviations;
        }

        /// <summary>
        /// Add the current sentence to the word prediction model
        /// </summary>
        public override void OnSentenceTerminator()
        {
            if (_enableLearn)
            {
                learn();
            }
        }

        /// <summary>
        /// Enable/Disable spellchecking
        /// </summary>
        /// <returns>true to enable or not</returns>
        public override bool SupportsSpellCheck()
        {
            return !_enableSpellCheck;
        }
    }
}