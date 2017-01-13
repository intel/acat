////////////////////////////////////////////////////////////////////////////
// <copyright file="ToolsMenu.cs" company="Intel Corporation">
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
using ACAT.Lib.Extension;
using System;

namespace ACAT.Extensions.Default.UI.Menus
{
    /// <summary>
    /// Form for the Tools menu for the application. Commands
    /// are handled by the command dispatcher in the base class
    /// </summary>
    [DescriptorAttribute("ABF847DF-ECBC-4361-96E9-D2DD3D031D73",
                        "ToolsMenu",
                        "Tools AppMenu")]
    public partial class ToolsMenu : MenuPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Panel class of the scanner</param>
        /// <param name="panelTitle">title of the panel (not used)</param>
        public ToolsMenu(String panelClass, String panelTitle)
            : base(panelClass, R.GetString("Tools"))
        {
        }
    }
}