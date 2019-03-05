////////////////////////////////////////////////////////////////////////////
// <copyright file="DualMonitorMenu.cs" company="Intel Corporation">
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

using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using ACAT.Lib.Extension.CommandHandlers;
using System;

namespace ACAT.Extensions.Default.UI.Menus
{
    /// <summary>
    /// Contextual menu that handles window position and size
    /// operations such as move window, size window, close window
    /// etc.  Command execution is handled by the base class
    /// </summary>
    [DescriptorAttribute("7A810F5D-06CB-47EE-A766-4574E3C2CDBC",
                        "DualMonitorMenu",
                        "Dual Monitor Menu")]
    public partial class DualMonitorMenu: MenuPanel
    {
        /// <summary>
        /// The command dispatcher object
        /// </summary>
        private readonly DefaultCommandDispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">the panel class of this scanner</param>
        /// <param name="panelTitle">The title of the menu</param>
        public DualMonitorMenu(String panelClass, String panelTitle) :
            base(panelClass, panelTitle)
        {
            _dispatcher = new DefaultCommandDispatcher(this);
        }

        /// <summary>
        /// Returns the command dispatcher object
        /// </summary>
        public override RunCommandDispatcher CommandDispatcher
        {
            get { return _dispatcher; }
        }
    }
}