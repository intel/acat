////////////////////////////////////////////////////////////////////////////
// <copyright file="SettingsMenu.cs" company="Intel Corporation">
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
    /// Form for the settings menu. All the commands
    /// associated with the menu items are handled by the base class.
    /// </summary>
    [DescriptorAttribute("5FE98886-D943-4E27-8380-BBB42D95D21E",
                        "SettingsMenu",
                        "Settings AppMenu")]
    public partial class SettingsMenu : MenuPanel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">the panel class</param>
        /// <param name="panelTitle">not used</param>
        public SettingsMenu(String panelClass, String panelTitle)
            : base(panelClass, R.GetString("Settings"))
        {
        }
    }
}