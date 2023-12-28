////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// VideoWindowFinder.cs
//
// Finds the video window by searching for the window by title and
// notifies event subscribers about this
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Threading;
using System.Windows.Forms;

namespace ACAT.Extensions.Default.Actuators.CameraActuator
{
    public class VideoWindowFinder : IDisposable
    {
        public volatile bool Done = false;
        public IntPtr VideoWindowHandle = IntPtr.Zero;

        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        private Thread _thread;

        public delegate void VideoWindowDisplayedDelegate(IntPtr handle);

        public event VideoWindowDisplayedDelegate EvtVideoWindowDisplayed;

        public event EventHandler EvtVideoWindowFindStart;

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        public void DockVideoWindow(Form form)
        {
            User32Interop.RECT rect;

            User32Interop.GetWindowRect(VideoWindowHandle, out rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            uint flags = (uint)User32Interop.SetWindowPosFlags.SWP_NOMOVE | (uint)User32Interop.SetWindowPosFlags.SWP_NOSIZE;
            User32Interop.SetWindowPos(VideoWindowHandle.ToInt32(),
                                        (int)User32Interop.HWND_TOPMOST, 0, 0, 0, 0,
                                        flags);

            User32Interop.SetWindowText(VideoWindowHandle, "Camera View");

            form.Invoke(new MethodInvoker(delegate
            {
                form.Left = width;
                form.Top = 0;
            }));
        }

        public void Start()
        {
            Done = false;
            _thread = new Thread(threadProc);// { IsBackground = true };
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        if (!Done)
                        {
                            Done = true;
                            try
                            {
                                if (_thread != null)
                                {
                                    _thread.Join(100);
                                    _thread = null;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }

                    // Release the native unmanaged resources

                    Log.Debug("Exiting dispose");
                    _disposed = true;
                }
                finally
                {
                }
            }
        }

        private void threadProc()
        {
            EvtVideoWindowFindStart?.Invoke(this, new EventArgs());

            while (!Done)
            {
                VideoWindowHandle = User32Interop.FindWindow("Main HighGUI class", "WebcamDLLGUI");
                if (VideoWindowHandle != IntPtr.Zero)
                {
                    Done = true;
                    break;
                }

                Thread.Sleep(30);
            }

            if (VideoWindowHandle != IntPtr.Zero)
            {
                EvtVideoWindowDisplayed?.Invoke(VideoWindowHandle);
            }
        }
    }
}