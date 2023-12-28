////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ACATResources;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Shows the various settings dialogs such as General,
    /// Scan, Text-to-speech, Mouse etc.  These dialogs enable
    /// the user to configure ACAT.
    /// </summary>
    public class ShowDialogsHandler : RunCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="cmd">The command to be executed</param>
        public ShowDialogsHandler(String cmd)
            : base(cmd)
        {
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="handled">set to true if the command was handled</param>
        /// <returns>true on success</returns>
        public override bool Execute(ref bool handled)
        {
            handled = true;

            Form form = Dispatcher.Scanner.Form;

            switch (Command)
            {
                case "CmdShowGeneralSettings":
                    showDialog("GeneralSettingsForm");
                    break;

                case "CmdShowScanSettings":
                    showDialog("ScannerSettingsForm");
                    break;

                case "CmdShowWordPredictionSettings":
                    showDialog("WordPredictionSettingsForm");
                    break;

                case "CmdShowMouseGridSettings":
                    showDialog("MouseGridSettingsForm");
                    break;

                case "CmdShowVoiceSettings":
                    showDialog("TextToSpeechSettingsForm");
                    break;

                case "CmdShowScreenLockSettings":
                    showDialog("ScreenLockSettingsForm");
                    break;

                case "CmdResizeRepositionScanner":
                    showDialog("ResizeScannerForm");
                    break;

                case "CmdShowAboutBox":
                    showAboutBox(form);
                    break;

                default:
                    handled = false;
                    break;
            }

            return true;
        }

        /// <summary>
        /// Displays the about box
        /// </summary>
        /// <param name="parentForm">scanner form</param>
        private void showAboutBox(Form parentForm)
        {
            object[] attributes = ACATPreferences.ApplicationAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var appName = (attributes.Length != 0) ? ((AssemblyTitleAttribute)attributes[0]).Title : String.Empty;

            var version = ACATPreferences.ApplicationAssembly.GetName().Version.Major + "." + ACATPreferences.ApplicationAssembly.GetName().Version.Minor;
            var versionInfo = String.Format(R.GetString("Version"), version);

            attributes = ACATPreferences.ApplicationAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            var copyrightInfo = (attributes.Length != 0) ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : String.Empty;

            attributes = ACATPreferences.ApplicationAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);

            var companyName = (attributes.Length != 0) ? ((AssemblyCompanyAttribute)attributes[0]).Company : String.Empty;

            DialogUtils.ShowAboutBox(parentForm, appName, versionInfo, companyName, copyrightInfo, Attributions.GetAll());
        }

        /// <summary>
        /// Displays the dialog that has the specified name
        /// </summary>
        /// <param name="name">name of the dialog</param>
        private void showDialog(String name)
        {
            try
            {
                Form dlg = Context.AppPanelManager.CreatePanel(name);
                if (dlg != null)
                {
                    Context.AppPanelManager.ShowDialog((IPanel)dlg);
                }
            }
            catch (Exception e)
            {
                Log.Debug("Error creating dialog of type " + name + ". Exception: " + e);
            }
        }
    }
}