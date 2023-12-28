////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension.CommandHandlers;
using System;

namespace ACAT.Lib.Extension
{
    /// <summary>
    /// Contextual menu with icons and text. Use this
    /// as the base class for Contextual menus.
    /// </summary>
    [DescriptorAttribute("6307D870-D90E-45ED-8A7E-43A3BA97D868",
                        "MenuPanel",
                        "AppMenu with Icons and Text")]
    public partial class MenuPanel : MenuPanelBase
    {
        /// <summary>
        /// The command dispatcher.  If the derived class as additional
        /// commands, just call Commands.Add on this object
        /// </summary>
        protected Dispatcher commandDispatcher;

        /// <summary>
        /// Initializes a new instance of the class
        /// </summary>
        public MenuPanel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">The panel class of the conextual menu</param>
        /// <param name="panelTitle">title of the contextual</param>
        public MenuPanel(String panelClass, String panelTitle)
            : base(panelClass, panelTitle)
        {
            commandDispatcher = new Dispatcher(this);
        }

        /// <summary>
        /// Gets the command dispatcher object
        /// </summary>
        public override RunCommandDispatcher CommandDispatcher
        {
            get { return commandDispatcher; }
        }

        public override void OnResume()
        {
            //CoreGlobals.AppPreferences.ScannerScaleFactor = 25;

            base.OnResume();
        }

        protected override void OnLoad()
        {
            //CoreGlobals.AppPreferences.ScannerScaleFactor = 25;
            ScannerCommon.PositionSizeController.ScaleForm();

            base.OnLoad();
        }

        /// <summary>
        /// The dispatcher object.  The DefaultCommandDispatcher
        /// will take care of executing the commands
        /// </summary>
        public class Dispatcher : DefaultCommandDispatcher
        {
            public Dispatcher(IScannerPanel panel)
                : base(panel)
            {
            }
        }
    }
}