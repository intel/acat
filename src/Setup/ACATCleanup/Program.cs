////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2016 Intel Corporation 
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

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ACATCleanup
{
    /// <summary>
    /// This program removes all the files from under the ACAT
    /// install directory (C:\Intel\ACAT by default).
    /// </summary>
    internal static class Program
    {
        private const String PresageProcessName = "presage_wcf_service_system_tray";

        /// <summary>
        /// Kills the presage process
        /// </summary>
        public static void KillPresage()
        {
            try
            {
                var processes = Process.GetProcessesByName(PresageProcessName);
                if (processes.Length > 0)
                {
                    processes[0].Kill();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// This is invoked from the ACAT uninstaller, meant to
        /// uninstall Presage and delete the contents of the
        /// ACAT Install dir optionally.
        /// If the arg is "blah123", it confirms if the user wants to
        /// delete the c:\intel\ACAT folder. If the user says YES,
        /// it makes a copy of this exe to the TEMP folder and spawns
        /// itself with the argument "blah".
        /// If the arg is "blah", it deletes all the contents of c:\intel\ACAT.
        /// The reason for doing all this is because if this EXE is run from
        /// C:\Intel\ACAT, it cannot delete the folder c:\Intel\ACAT because
        /// this process is still running.
        /// </summary>
        [STAThread]
        public static void Main(String[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                return;
            }

            args[0] = args[0].ToLower();

            if (args[0].StartsWith("uninstalllanguagepack"))
            {
                if (args.Length < 2)
                {
                    return;
                }

                var path = args[1];
                if (Directory.Exists(path))
                {
                    if (isPresageRunning())
                    {
                        KillPresage();
                    }

                    Directory.Delete(path, true);
                }

                Environment.Exit(0);
            }
            if (args[0].StartsWith("killpresage"))
            {
                if (isPresageRunning())
                {
                    KillPresage();
                }

                return;
            }
            else if (args[0].StartsWith("blahblah"))
            {
                String[] strings = args[0].Split(';');

                var installDir = String.Empty;
                if (strings.Length > 1)
                {
                    installDir = strings[1];
                }

                Thread.Sleep(3000);

                if (isPresageRunning())
                {
                    KillPresage();
                }

                //const string key = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Intel Corporation\\ACAT";
                //var installDir = (String)Registry.GetValue(key, "InstallDir", String.Empty);

                try
                {
                    if (!String.IsNullOrEmpty((installDir)))
                    {
                        var filePath = Process.GetCurrentProcess().MainModule.FileName;
                        var fileName = Path.Combine(installDir, Path.GetFileName(filePath));

                        if (directoryExists(installDir, "Vision") || directoryExists(installDir, "Logs") ||
                            directoryExists(installDir, "AuditLogs") ||
                            directoryExists(installDir, "Users") || File.Exists(fileName))
                        {
                            Directory.Delete(installDir, true);
                            Directory.Delete(installDir);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Could not locate ACAT install folder.  Please delete it manually", "ACAT Uninstall",
                             MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error Deleting folders " + ex.ToString());
                }

                RemovePresage();

                return;
            }
            else if (args[0] == "blah123")
            {
                Application.Run(new Form1());
            }
        }

        /// <summary>
        /// Uninstalls Presage
        /// </summary>
        public static void RemovePresage()
        {
            try
            {
                var presageInstallDir =
                    Registry.GetValue("HKEY_CURRENT_USER\\Software\\Presage", "", string.Empty).ToString();

                if (!String.IsNullOrEmpty(presageInstallDir))
                {
                    String presageUninstaller = Path.Combine(presageInstallDir, "Uninstall.exe");
                    if (File.Exists(presageUninstaller))
                    {
                        var form = new Form2();
                        form.ShowDialog();

                        var process = new Process();
                        var startInfo = new ProcessStartInfo { FileName = presageUninstaller };
                        process.StartInfo = startInfo;
                        process.Start();
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Checks if the dir exists
        /// </summary>
        /// <param name="rootdir">Root dir</param>
        /// <param name="subdir">sub folder under the root dir</param>
        /// <returns>true if it does</returns>
        private static bool directoryExists(String rootdir, String subdir)
        {
            return Directory.Exists(rootdir + "\\" + subdir);
        }

        /// <summary>
        /// Checks if the presage tray app is running
        /// </summary>
        /// <returns>true if it is</returns>
        private static bool isPresageRunning()
        {
            var pname = Process.GetProcessesByName(PresageProcessName);
            return pname.Length != 0;
        }
    }
}