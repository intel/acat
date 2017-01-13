////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

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

namespace ACAT.Applications.ACATApp
{
    /// <summary>
    /// Initializes the various modules in ACAT and activates the default scanner.
    /// </summary>
    internal static class Program
    {
        private static String _formName = String.Empty;

        /// <summary>
        /// Used for parsing the command line
        /// </summary>
        private enum ParseState
        {
            Next,
            Form,
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {
            if (AppCommon.OtherInstancesRunning())
            {
                return;
            }

            //Windows.TurnOffDPIAwareness();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            parseCommandLine(args);

            AppCommon.LoadGlobalSettings();

            //Set the active user/profile information
            AppCommon.SetUserName();
            AppCommon.SetProfileName();

            if (!AppCommon.CreateUserAndProfile())
            {
                return;
            }

            if (!AppCommon.LoadUserPreferences())
            {
                return;
            }

            Common.AppPreferences.AppName = "ACAT Tryout";

            Log.SetupListeners();

            if (!AppCommon.SetCulture())
            {
                return;
            }

            Splash splash = new Splash(2000);
            splash.Show();
            splash.Close();

            Context.PreInit();
            Common.PreInit();

            try
            {
                if (!Context.Init(Context.StartupFlags.Minimal | Context.StartupFlags.TextToSpeech))
                {
                    MessageBox.Show(R.GetString("ContextInitializationError"));
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("ContextInitializationErrorException", ex));
                return;
            }

            if (!Context.PostInit())
            {
                MessageBox.Show(Context.GetInitCompletionStatus(), R.GetString("InitializationError"));
                return;
            }

            Common.Init();

            var formName = String.IsNullOrEmpty(_formName) ? "ACATTryoutForm" : _formName;
            var form = PanelManager.Instance.CreatePanel(formName);
            if (form != null)
            {
                Context.AppPanelManager.Show(null, form as IPanel);
            }
            else
            {
                MessageBox.Show(String.Format(R.GetString("InvalidFormName"), formName), R.GetString("Error"));
                return;
            }

            try
            {
                Application.Run();


                AppCommon.ExitMessageShow();

                Context.Dispose();

                Common.Uninit();

                AppCommon.ExitMessageClose();

                Log.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            AppCommon.OnExit();
        }

        /// <summary>
        /// Parses the command line arguments. Format of the
        /// arguments are -option <option arg>
        /// </summary>
        /// <param name="args">Args to parse</param>
        private static void parseCommandLine(string[] args)
        {
            var parseState = ParseState.Next;

            for (int index = 0; index < args.Length; index++)
            {
                switch (args[index].ToLower().Trim())
                {
                    case "-form":
                    case "/form":
                    case "-f":
                    case "/f":
                        parseState = ParseState.Form;
                        break;
                }

                switch (parseState)
                {
                    case ParseState.Form:
                        args[index] = args[index].Trim();
                        if (!AppCommon.IsOption(args[index]))
                        {
                            _formName = args[index].Trim();
                        }

                        break;
                }
            }
        }
    }
}