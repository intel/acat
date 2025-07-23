////////////////////////////////////////////////////////////////////////////
// <copyright file="AppInfo.cs" company="Intel Corporation">
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

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

namespace ACAT.Applications.ACATDashboard
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
        [NonSerialized, XmlIgnore]
        private const string Missing = "";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AppInfo()
        {
            Name = String.Empty;
            Path = String.Empty;
            CommandLine = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="appName">Friendly name of the app</param>
        /// <param name="path">full path to the exe</param>
        /// <param name="shortCutName">Name of the shortcut on desktop</param>
        /// <param name="commandLine">optional command line arg</param>
        /// <param name="description">user friendly description of the app</param>
        public AppInfo(String appName, 
                        String path, 
                        String shortCutName = Missing, 
                        String commandLine = Missing, 
                        String description = Missing,
                        String workingDirectory = Missing)
        {
            Name = appName;
            Path = path;
            ShortcutName = shortCutName;
            CommandLine = commandLine;
            Description = description;
            WorkingDirectory = workingDirectory;
        }

        /// <summary>
        /// Gets or sets the command line argument
        /// </summary>
        public String CommandLine { get; set; }

        /// <summary>
        /// Get or sets the description of the application
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets or sets friendly name of the app
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the full path to the exe
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// Gets or sets the name of the shortcut
        /// </summary>
        public String ShortcutName { get; set; }

        /// <summary>
        /// Gets or sets the working directory for the shortcut
        /// </summary>
        public String WorkingDirectory { get; set; }
    }
}