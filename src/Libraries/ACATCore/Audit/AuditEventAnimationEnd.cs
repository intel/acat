////////////////////////////////////////////////////////////////////////////
// <copyright file="AnimationEndEvent.cs" company="Intel Corporation">
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

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Represents log entry to audit end of an animation sequence
    /// </summary>
    public class AuditEventAnimationEnd : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventAnimationEnd()
            : base("AnimationEnd")
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="highlightWidgetName">name of the highlighted widget</param>
        /// <param name="highlightWidgetType">type of the highlighed widget</param>
        /// <param name="animationName">name of the animation sequence</param>
        public AuditEventAnimationEnd(String rootWidget,
                                    String highlightWidgetName,
                                    String highlightWidgetType,
                                    String animationName)
            : base("AnimationEnd")
        {
            RootWidgetName = rootWidget;
            HighlightWidgetName = highlightWidgetName;
            HighlightWidgetType = highlightWidgetType;
            AnimationName = animationName;
        }

        /// <summary>
        /// Gets or sets the animation sequence name
        /// </summary>
        public String AnimationName { get; set; }

        /// <summary>
        /// Gets or sets the highlighted widget name
        /// </summary>
        public String HighlightWidgetName { get; set; }

        /// <summary>
        /// Gets or sets the type of the highlighted widget
        /// </summary>
        public String HighlightWidgetType { get; set; }

        /// <summary>
        /// Gets or sets the root widget name for the scanner
        /// </summary>
        public String RootWidgetName { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(RootWidgetName, HighlightWidgetName, HighlightWidgetType, AnimationName);
        }
    }
}