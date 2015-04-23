////////////////////////////////////////////////////////////////////////////
// <copyright file="AppInfo.cs" company="Intel Corporation">
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

namespace ACAT.Extensions.Hawking.FunctionalAgents.LaunchApp
{
    /// <summary>
    /// Serializable class that represents info about the application
    /// to launch.  Includes name of the exe, command line arg, friendly name
    /// etc.
    /// </summary>
    [Serializable]
    public class AppInfo
    {
        /// <summary>
        /// To indicate a missing parameter
        /// </summary>
        [NonSerialized]
        private const string Missing = "";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AppInfo()
        {
            Name = String.Empty;
            Path = String.Empty;
            CommandLine = String.Empty;
            Action = LaunchAction.StartNew;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="appName">Friendly name of the app</param>
        /// <param name="path">full path to the exe</param>
        /// <param name="commandLine">optional command line arg</param>
        /// <param name="action">start a new instance or switch to existing</param>
        public AppInfo(String appName, String path, String commandLine = Missing, LaunchAction action = LaunchAction.StartNew)
        {
            Name = appName;
            Path = path;
            CommandLine = commandLine;
            Action = action;
        }

        /// <summary>
        /// How to launch?
        /// </summary>
        public enum LaunchAction
        {
            /// <summary>
            /// No action
            /// </summary>
            None,

            /// <summary>
            /// Starts a new instance of the application
            /// </summary>
            StartNew,

            /// <summary>
            /// Switches to an existing instance of the application
            /// </summary>
            SwitchTo
        }

        /// <summary>
        /// Gets or sets the launch action
        /// </summary>
        public LaunchAction Action { get; set; }

        /// <summary>
        /// Gets or sets the command line argument
        /// </summary>
        public String CommandLine { get; set; }

        /// <summary>
        /// Gets or sets friendly name of the app
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the full path to the exe
        /// </summary>
        public String Path { get; set; }
    }
}