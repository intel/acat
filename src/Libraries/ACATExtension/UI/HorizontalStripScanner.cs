////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.PanelManagement.Interfaces;
using ACAT.Core.Utility;
using ACAT.Core.WidgetManagement;
using ACAT.Extension.CommandHandlers;
using Microsoft.Extensions.Logging;
using System;

namespace ACAT.Extension
{
    /// <summary>
    /// This is a scanner with a single row of buttons.
    /// </summary>
    [ClassDescriptor("4287E55B-3364-46B5-A5B2-6C8BE3C57F1E",
                    "HorizontalStripScanner",
                    "Horizontal strip of buttons")]
    public partial class HorizontalStripScanner : HorizontalStripScannerBase
    {
        private readonly ILogger<HorizontalStripScanner> _logger;

        /// <summary>
        /// The command dispatcher.  If the derived class as additional
        /// commands, just call Commands.Add on this object
        /// </summary>
        protected Dispatcher commandDispatcher;

        //public event Action TextSubmitted;


        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">The panel class of the contextual menu</param>
        /// <param name="panelTitle">title of the contextual</param>
        /// <param name="logger">Logger instance</param>
        public HorizontalStripScanner(String panelClass, String panelTitle, ILogger<HorizontalStripScanner> logger = null)
            : base(panelClass, panelTitle)
        {
            _logger = logger ?? LogManager.GetLogger<HorizontalStripScanner>();
            commandDispatcher = new Dispatcher(this);
        }

        /// <summary>
        /// Gets the command dispatcher object
        /// </summary>
        public override RunCommandDispatcher CommandDispatcher
        {
            get { return commandDispatcher; }
        }

        /// <summary>
        /// Invoked when the user actuates a button in
        /// the scanner form
        /// </summary>
        /// <param name="widget">widget actuated</param>
        /// <param name="handled">was this handled here?</param>
        public override void OnWidgetActuated(WidgetActuatedEventArgs e, ref bool handled)
        {
            _logger.LogDebug("HorizontalStripScanner onWidgetActuated");
            Windows.CloseAsync(this);
            handled = false;
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