////////////////////////////////////////////////////////////////////////////
// <copyright file="PanelRequestEventArgs.cs" company="Intel Corporation">
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
using System.Text;
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

namespace ACAT.Lib.Core.PanelManagement
{
    /// <summary>
    /// Represents event args for the event raised to
    /// request for a scanner to be activated
    /// </summary>
    public class PanelRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PanelRequestEventArgs()
        {
            init();
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">The scanner to be activted</param>
        /// <param name="monitorInfo">Contextual info about app window</param>
        public PanelRequestEventArgs(String panelClass, WindowActivityMonitorInfo monitorInfo)
        {
            init();

            PanelClass = panelClass;
            MonitorInfo = monitorInfo;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">The scanner to be activted</param>
        /// <param name="panelTitle">Title of the scanner </param>
        /// <param name="monitorInfo">Contextual info about app window</param>
        public PanelRequestEventArgs(String panelClass, String panelTitle, WindowActivityMonitorInfo monitorInfo)
            : this(panelClass, monitorInfo)
        {
            Title = panelTitle;
            TargetPanel = null;
        }

        /// <summary>
        /// Gets or sets active window information
        /// </summary>
        public WindowActivityMonitorInfo MonitorInfo { get; set; }

        /// <summary>
        /// Gets or sets the scanner class
        /// </summary>
        public String PanelClass { get; set; }

        /// <summary>
        /// Gets or sets User-defined arguments for the scanner
        /// </summary>
        public object RequestArg { get; set; }

        /// <summary>
        /// Gets or sets the dialog box or scanner for which this
        /// scanner is being activated.
        /// </summary>
        public object TargetPanel { get; set; }

        /// <summary>
        /// Gets or sets title of the scanner
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Gets or sets whether to use the current scanner as the parent
        /// for the scanner being activated
        /// </summary>
        public bool UseCurrentScreenAsParent { get; set; }

        /// <summary>
        /// Converts to a string representation for debugging
        /// purposes
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            String title = Title ?? "<null>";
            sb.Append("PanelRequestEventArgs: PanelClass: " + PanelClass +
                        ",  UseAsParent: " + UseCurrentScreenAsParent +
                        " Title: " + title);
            return sb.ToString();
        }

        /// <summary>
        /// Initializes class variables
        /// </summary>
        private void init()
        {
            PanelClass = PanelClasses.None;
            UseCurrentScreenAsParent = false;
            RequestArg = null;
            Title = "Default";
            TargetPanel = null;
        }
    }
}