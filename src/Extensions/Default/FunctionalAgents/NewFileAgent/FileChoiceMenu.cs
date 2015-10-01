////////////////////////////////////////////////////////////////////////////
// <copyright file="FileChoiceMenu.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;

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

namespace ACAT.Extensions.Default.FunctionalAgents.NewFile
{
    /// <summary>
    /// Menu that presents the choices for the types of file to
    /// create.
    /// </summary>
    [DescriptorAttribute("52BDBBA8-A855-42A1-AC3C-03945DAD3686",
                        "FileChoiceMenu",
                        "Create New")]
    public partial class FileChoiceMenu : MenuPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Panel class of the scanner</param>
        /// <param name="panelTitle">title of the panel (not used)</param>
        public FileChoiceMenu(String panelClass, String panelTitle)
            : base(panelClass, "File Type")
        {
        }

        /// <summary>
        /// Gets or sets the choice that user made
        /// </summary>
        public String Choice { get; set; }

        /// <summary>
        /// Called when a widget on the scanner is activated
        /// </summary>
        /// <param name="widget">widget activated</param>
        /// <param name="handled">true if handled</param>
        public override void OnWidgetActuated(Widget widget, ref bool handled)
        {
            handled = true;
            switch (widget.Value)
            {
                case "@TextFile":
                    Choice = "TextFile";
                    DialogResult = DialogResult.OK;
                    break;

                case "@WordDoc":
                    Choice = "WordDoc";
                    DialogResult = DialogResult.OK;
                    break;

                case "@exitFileTypeMenu":
                    Choice = String.Empty;
                    DialogResult = DialogResult.Cancel;
                    break;
            }
        }
    }
}