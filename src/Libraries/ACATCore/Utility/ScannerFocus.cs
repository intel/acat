////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility.NamedPipe;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Windows.Forms;

namespace ACAT.Lib.Core.Utility
{
    /// <summary>
    /// Communicates with ACATWatcher which is a separate process, to 
    /// set focus to a specified window. Setting focus is tricky. Doesn't
    /// always work if ACAT sets focus to one of its own windows. works 
    /// better if a different process does it, hence ACATWatcher
    /// </summary>
    public static class ScannerFocus
    {
        private static PipeClient pipeClient;

        private static bool pipeClientDisposed = false;

        public static void Connect(int numRetries = 5)
        {
            if (!isACATWatcherRunning())
            {
                Start();
            }

            for (int ii = 0; ii < numRetries; ii++)
            {
                try
                {
                    if (pipeClient == null || pipeClientDisposed)
                    {
                        pipeClient = new PipeClient("ACATWatch", PipeDirection.InOut);
                        pipeClientDisposed = false;
                        pipeClient.EvtServerDisconnected += PipeClient_EvtServerDisconnected;
                        Log.Debug("Connecting to pipe server...");
                        pipeClient.Connect();
                        Log.Debug("Successfully connected to pipe server");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    disposePipeClient();
                    Log.Debug("Could not communicate with ACATWatcher. Will retry" + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Simulates a mouse click on the specified control
        /// </summary>
        /// <param name="control">the control</param>
        public static void SetFocus(Control control)
        {
            try
            {
                if (pipeClient == null || pipeClientDisposed)
                {
                    Connect();
                }
                else
                {
                    Log.Debug("Already connected to ACATWatch");
                }
                pipeClient.Send(control.Handle.ToString());
                Log.Debug("Successfully sent window handle to ACATWatch");
            }
            catch (Exception ex)
            {
                Log.Debug("Could not communicate with ACATWatch. " + ex.ToString());
                try
                {
                    disposePipeClient();

                    if (control.Visible)
                    {
                        int xpos = control.Left + 5;
                        int ypos = control.Top + 5;

                        Point oldPos = Cursor.Position;
                        MouseUtils.ClickLeftMouseButton(xpos, ypos);
                        Cursor.Position = oldPos;
                    }
                }
                catch (Exception ex1)
                {
                    Log.Debug(ex1.ToString());
                }
            }

            return;
        }

        public static void Start()
        {
            if (isACATWatcherRunning())
            {
                Log.Debug("ACAT Watcher is already running!");

                return;
            }

            Log.Debug("Running ACAT Watcher...");
            FileUtils.Run("ACATWatch.exe", ProcessWindowStyle.Minimized);
        }

        public static void Stop()
        {
            if (isACATWatcherRunning())
            {
                Log.Debug("Watcher is running ");

                try
                {
                    if (pipeClient == null)
                    {
                        Connect();
                    }

                    if (pipeClient != null)
                    {
                        Log.Debug("Sending quit message");

                        pipeClient.Send("quit");
                        Log.Debug("Successfully sent quit message to ACATWatcher");
                    }
                    else
                    {
                        Log.Debug("pipeLcient is null");
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("Could not send quit message to ACATWatcher. " + ex.ToString());
                }
            }
        }

        private static void disposePipeClient()
        {
            if (pipeClient != null)
            {
                pipeClient.EvtServerDisconnected -= PipeClient_EvtServerDisconnected;
                pipeClient.Dispose();
                pipeClientDisposed = true;
            }
        }

        private static bool isACATWatcherRunning()
        {
            var processName = "ACATWatch";
            var processes = Process.GetProcesses();

            int count = processes.Count(process => String.Compare(process.ProcessName, processName, true) == 0);

            return count >= 1;
        }

        private static void PipeClient_EvtServerDisconnected(object sender, EventArgs e)
        {
            Log.Debug("ACATWatch server disconnected");
            disposePipeClient();
        }
    }
}