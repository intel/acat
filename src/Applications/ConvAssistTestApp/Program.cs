////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Program.cs
//
// Main entry point into the application. Initializes ACAT and displays
// the main form.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PanelManagement;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.IO;
using System.Windows.Forms;

namespace ACAT.Applications.ConvAssistTestApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (AppCommon.OtherInstancesRunning())
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CoreGlobals.AppId = "ConvAssistTestApp";

            FileUtils.LogAssemblyInfo();

            AppCommon.LoadGlobalSettings();

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

            Common.AppPreferences.AppName = "ConvAssist Test App";

            if (!AppCommon.SetCulture())
            {
                return;
            }

            Log.SetupListeners();

            Splash splash = new Splash(2000);
            splash.Show();

            Context.PreInit();
            Common.PreInit();

            if (!Context.Init(Context.StartupFlags.Minimal |
                                Context.StartupFlags.TextToSpeech |
                                Context.StartupFlags.WordPrediction |
                                Context.StartupFlags.NoActuator |
                                Context.StartupFlags.DialogSense |
                                Context.StartupFlags.NoUI))
            {
                splash.Close();
                splash = null;

                ConfirmBoxSingleOption.ShowDialog(Context.GetInitCompletionStatus(), "OK");
                if (Context.IsInitFatal())
                {
                    return;
                }
            }

            splash?.Close();

            if (!Context.PostInit())
            {
                Context.Dispose();
                return;
            }

            Common.Init();

            new MainForm().ShowDialog();

            AppCommon.ExitMessageShow();

            Context.Dispose();

            Common.Uninit();

            AppCommon.ExitMessageClose();

            Log.Close();
        }
    }
}