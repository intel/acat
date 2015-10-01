////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelClasses.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents types of common scanner types
    /// </summary>
    public class PanelClasses
    {
        /// <summary>
        /// The main Alphabet scanner
        /// </summary>
        public const String Alphabet = "Alphabet";

        /// <summary>
        /// The alphabet scanner without word prediction
        /// </summary>
        public const String AlphabetMinimal = "AlphabetMinimal";

        /// <summary>
        /// The Cursor navigation scanner
        /// </summary>
        public const String Cursor = "Cursor";

        /// <summary>
        /// Contexutal menus to interact with application dialogs. E.g. the
        /// Find dialog box when the user presses Ctrl-F in Notepad
        /// </summary>
        public const String DialogContextMenu = "DialogContextMenu";

        /// <summary>
        /// Scanner for function keys F1 through F12
        /// </summary>
        public const String FunctionKey = "FunctionKey";

        /// <summary>
        /// Contexutal menu to interact with application menus (E.g user right
        /// clicks in an application window and Windows displays a
        /// menu.
        /// </summary>
        public const String MenuContextMenu = "MenuContextMenu";

        /// <summary>
        /// The Mouse navigation scanner
        /// </summary>
        public const String Mouse = "Mouse";

        /// <summary>
        /// For uninitialized panels
        /// </summary>
        public const String None = "None";

        /// <summary>
        /// The Numbers scanner (equivalent to the numeric keypad)
        /// </summary>
        public const String Number = "Number";

        /// <summary>
        /// Scanner to enter Punctuations.
        /// </summary>
        public const String Punctuation = "Punctuation";

        /// <summary>
        /// The category of scanner
        /// </summary>
        public enum PanelCategory
        {
            Unknown = 0,
            Scanner = 1,
            Dialog = 2,
            ContextualMenu = 3,
            Menu = 4
        }
    }
}