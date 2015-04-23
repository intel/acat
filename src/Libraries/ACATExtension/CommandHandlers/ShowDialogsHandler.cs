////////////////////////////////////////////////////////////////////////////
// <copyright file="ShowDialogsHandler.cs" company="Intel Corporation">
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
using System.Reflection;
using System.Windows.Forms;
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.PanelManagement.CommandDispatcher;
using ACAT.Lib.Core.Utility;

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

namespace ACAT.Lib.Extension.CommandHandlers
{
    /// <summary>
    /// Shows the various settings dialogs such as General,
    /// Scan, Voice etc.
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

                case "CmdShowMouseRadarSettings":
                    showDialog("MouseRadarSettingsForm");
                    break;

                case "CmdShowMouseGridSettings":
                    showDialog("MouseGridSettingsForm");
                    break;

                case "CmdShowVoiceSettings":
                    showDialog("VoiceSettingsForm");
                    break;

                case "CmdShowMuteScreenSettings":
                    showDialog("MuteScreenSettingsForm");
                    break;

                case "CmdShowDesignSettings":
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

            var version = ACATPreferences.ApplicationAssembly.GetName().Version.ToString();
            var versionInfo = "Version " + version;

            attributes = ACATPreferences.ApplicationAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            var copyrightInfo = (attributes.Length != 0) ? ((AssemblyCopyrightAttribute)attributes[0]).Copyright : String.Empty;

            DialogUtils.ShowAboutBox(parentForm, "AboutBoxLogo.png", appName, versionInfo, copyrightInfo, Attributions.GetAll());
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