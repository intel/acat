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

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.Vision.VisionTryout
{
    /// <summary>
    /// Entry point in to the program that lets the user
    /// tryout ACAT Vision. User can practice facial gestures
    /// to trigger
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Mutex to ensure that ACAT is not running
        /// </summary>
        private static Mutex _appMutex;

        /// <summary>
        /// Checks if multiple instances of this app ar running
        /// </summary>
        /// <returns>true if there are</returns>
        public static bool AreMultipleInstancesRunning()
        {
            var processName = Process.GetCurrentProcess().ProcessName;
            var processes = Process.GetProcesses();

            int count = processes.Count(process => String.Compare(process.ProcessName, processName, true) == 0);

            return count > 1;
        }

        /// <summary>
        /// Checks if the app is already running. The mutex
        /// is used to check this
        /// </summary>
        /// <param name="mutexName">name of the mutex</param>
        /// <returns>true if is</returns>
        public static bool CheckAppExistingInstance(String mutexName)
        {
            bool retVal = true;
            try
            {
                closeAppMutex();
                _appMutex = Mutex.OpenExisting(mutexName);
                closeAppMutex();
            }
            catch
            {
                //the specified mutex doesn't exist, we should create it
                _appMutex = new Mutex(true, mutexName);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Returns true if an instance of any of the ACAT apps is
        /// still running
        /// </summary>
        /// <returns>true if so</returns>
        public static bool IsACATRunning()
        {
            return CheckAppExistingInstance("ACATMutex");
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (AreMultipleInstancesRunning())
            {
                return;
            }

            if (IsACATRunning())
            {
                MessageBox.Show("Cannot run this app while an ACAT application is active.  Please exit the ACAT application and retry",
                    "Vision Tryout", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VisionTryoutForm());

            closeAppMutex();

            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Closes the application mutex
        /// </summary>
        private static void closeAppMutex()
        {
            if (_appMutex != null)
            {
                _appMutex.Close();
                _appMutex.Dispose();
                _appMutex = null;
            }
        }
    }
}
