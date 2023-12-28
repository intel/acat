////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// Program.cs
//
// Main entry point into the program.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using ACAT.Lib.Extension;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ACAT.Applications.ACATWatch
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (FileUtils.CheckAppExistingInstance("ACATWatchMutex"))
            {
                return;
            }

            if (!AppCommon.LoadUserPreferences())
            {
                return;
            }

            CoreGlobals.AppId = "ACATWatcher";
            Common.AppPreferences.AppName = "ACAT Watcher";

            CoreGlobals.AppPreferences.DebugLogMessagesToFile = true;
            CoreGlobals.AppPreferences.DebugMessagesEnable = true;

            Log.SetupListeners();

            FileUtils.LogAssemblyInfo(Assembly.GetExecutingAssembly());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ACATWatchForm());

            Log.Info("**** Exit " + Common.AppPreferences.AppName + " " + DateTime.Now.ToString() + " ****");

            Log.Close();
        }
    }
}