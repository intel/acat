////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelClass.cs" company="Intel Corporation">
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
    /// Represents types of common scanner classes
    /// </summary>
    public class PanelClasses
    {
        public const String Alphabet = "Alphabet";
        public const String AlphabetMinimal = "AlphabetMinimal";
        public const String Cursor = "Cursor";
        public const String DialogContextMenu = "DialogContextMenu";
        public const String DialogDockContextMenu = "DialogDockContextMenu";
        public const String FileMenu = "FileMenu";
        public const String MenuContextMenu = "MenuContextMenu";
        public const String Mouse = "Mouse";
        public const String None = "None";
        public const String Number = "Number";
        public const String Punctuation = "Punctuation";
        public const String Unknown = "Unknown";
        public const String UnsupportedAppContextMenu = "UnsupportedAppContextMenu";

        /// <summary>
        /// The type of scanner
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