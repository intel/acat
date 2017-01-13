////////////////////////////////////////////////////////////////////////////
// <copyright file="FileChoiceMenu.cs" company="Intel Corporation">
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

using ACAT.ACATResources;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using ACAT.Lib.Extension;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.FunctionalAgents.NewFile
{
    /// <summary>
    /// AppMenu that presents the choices for the types of file to
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
            : base(panelClass, R.GetString("NewFileFileType"))
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

                case "@ExitFileTypeMenu":
                    Choice = String.Empty;
                    DialogResult = DialogResult.Cancel;
                    break;
            }
        }
    }
}