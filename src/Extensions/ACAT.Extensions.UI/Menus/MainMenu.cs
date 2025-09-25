////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MainMenu.cs
//
// Form for the main menu for the application. Some of the commands
// are handled by the command dispatcher in the base class
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.ActuatorManagement.Interfaces;
using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.PanelManagement.CommandDispatcher;
using ACAT.Core.Utility;
using ACAT.Extension;
using ACAT.Extension.UI;
using ACATResources;
using System;
using System.Windows.Forms;

namespace ACAT.Extensions.UI.Menus
{
    [ClassDescriptor("148257A1-A8B7-4E75-93F0-56AFCD5B2A3E",
                        "MainMenu",
                        "Main AppMenu")]
    public partial class MainMenu : MenuPanel
    {
        private readonly IActuator _calibrationSupporedActuator;
        private readonly bool _enableScanTimingConfigure = false;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="panelClass">Panel class of the scanner</param>
        /// <param name="panelTitle">title of the panel (not used)</param>
        public MainMenu(String panelClass, String panelTitle)
            : base(panelClass, StringResources.MainMenu)
        {
            InitializeComponent();

            // add commands that are not supported by the base class
            commandDispatcher.Commands.Add(new CommandHandler("Exit"));
            commandDispatcher.Commands.Add(new CommandHandler("CmdAdjustScanSpeed"));
            commandDispatcher.Commands.Add(new CommandHandler("CmdCalibrateActuator"));
            _enableScanTimingConfigure = Context.AppActuatorManager.CheckScanTimingConfigureEnable();

            _calibrationSupporedActuator = Context.AppActuatorManager.GetCalibrationSupportedActuator();

            Load += MainMenu_Load;
        }

        /// <summary>
        /// Invoked to check if a scanner button should be enabled.  Uses context
        /// to determine the 'enabled' state.
        /// </summary>
        /// <param name="arg">info about the scanner button</param>
        public override bool CheckCommandEnabled(CommandEnabledArg arg)
        {
            if (arg.Command == "CmdAdjustScanSpeed")
            {
                arg.Enabled = _enableScanTimingConfigure;
                arg.Handled = true;
            }
            else if (arg.Command == "CmdCalibrateActuator")
            {
                arg.Enabled = (_calibrationSupporedActuator != null);
                arg.Handled = true;
            }

            return true;
        }

        /// <summary>
        /// User wants to quit the app. Confirm and exit
        /// </summary>
        private void confirmAndQuitApplication()
        {
            Invoke(new MethodInvoker(delegate
            {
                if (DialogUtils.ConfirmScanner(null, StringResources.QuitApplication))
                {
                    quitApplication();
                }
            }));
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            CenterToScreen();
        }

        /// <summary>
        /// Quits the application.
        /// </summary>
        private void quitApplication()
        {
            //HACK for ACAT App
            bool quitApp = true;

            if ((this.Owner != null) &&
                (this.Owner.Owner != null))
            {
                // Just close myself, and don't set AppQuit to true
                quitApp = false;
            }

            Context.AppQuit = quitApp;

            Close();
            if (Owner != null)
            {
                Owner.Close();
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// Command dispatcher to handles commands from
        /// the menu itmes in the main menu. Executes
        /// commands that are not supported by the base class
        /// </summary>
        private class CommandHandler : RunCommandHandler
        {
            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="cmd">the command</param>
            public CommandHandler(String cmd)
                : base(cmd)
            {
            }

            /// <summary>
            /// Executes the command
            /// </summary>
            /// <param name="handled">was the command handled?</param>
            /// <returns>true on success</returns>
            public override bool Execute(ref bool handled)
            {
                handled = true;

                var form = Dispatcher.Scanner.Form as MainMenu;

                switch (Command)
                {
                    case "Exit":
                        form.confirmAndQuitApplication();
                        break;

                    case "CmdAdjustScanSpeed":
                        Context.AppActuatorManager.ShowScanTimingsConfigureDialog();
                        break;

                    case "CmdCalibrateActuator":
                        if (form._calibrationSupporedActuator != null)
                        {
                            Context.AppActuatorManager.RequestCalibration(form._calibrationSupporedActuator, RequestCalibrationReason.AppRequested);
                        }
                        break;

                    default:
                        handled = false;
                        break;
                }

                return true;
            }
        }
    }
}