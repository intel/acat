using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace VisionHost
{
    internal sealed class NativeVisionBridge : IDisposable
    {
#if DEBUG
        private const string DllFilePath = "acat_gestures_dll_d.dll";
#else
        private const string DllFilePath = "acat_gestures_dll.dll";
#endif

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private delegate void VisionEventCallback([MarshalAs(UnmanagedType.LPStr)] string text);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private static extern void acatVision();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private static extern void SetVisionEventHandler(VisionEventCallback func);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern void visionCommand([MarshalAs(UnmanagedType.LPStr)] string command, double value);

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private static extern void showVideoWindow();

        [DllImport(DllFilePath, CallingConvention = CallingConvention.StdCall)]
        private static extern void hideVideoWindow();

        private readonly VisionEventCallback callback;
        private readonly object gate = new object();
        private Thread visionThread;
        private bool exitCommandSent;
        private bool disposed;

        public event Action<string> VisionEventReceived;

        public NativeVisionBridge()
        {
            callback = OnVisionEvent;
            SetVisionEventHandler(callback);
        }

        public bool IsRunning
        {
            get
            {
                lock (gate)
                {
                    return visionThread != null && visionThread.IsAlive;
                }
            }
        }

        public void StartVision()
        {
            lock (gate)
            {
                ThrowIfDisposed();
                if (visionThread != null && visionThread.IsAlive)
                {
                    return;
                }

                exitCommandSent = false;
                visionThread = new Thread(RunVision)
                {
                    IsBackground = true,
                    Name = "VisionHost.NativeVisionThread"
                };
                visionThread.Start();
            }
        }

        public void StopVision(int timeoutMs = 3000)
        {
            Thread threadToJoin;
            lock (gate)
            {
                ThrowIfDisposed();
                threadToJoin = visionThread;
            }

            if (threadToJoin == null)
            {
                return;
            }

            RequestExit();
            if (threadToJoin.IsAlive)
            {
                if (!threadToJoin.Join(timeoutMs))
                {
                    throw new TimeoutException("The native vision thread did not stop in time.");
                }
            }
        }

        public void SendNativeCommand(string command, double value)
        {
            ThrowIfDisposed();
            visionCommand(command, value);
        }

        public void ShowVideo()
        {
            ThrowIfDisposed();
            showVideoWindow();
        }

        public void HideVideo()
        {
            ThrowIfDisposed();
            hideVideoWindow();
        }

        private void RequestExit()
        {
            lock (gate)
            {
                if (exitCommandSent)
                {
                    return;
                }

                exitCommandSent = true;
            }

            try
            {
                visionCommand("action=EXITAPP", 0);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[VisionHost] Failed to send EXITAPP: " + ex);
            }
        }

        private void RunVision()
        {
            try
            {
                acatVision();
            }
            catch (Exception ex)
            {
                OnVisionEvent("gesture=EXCEPTION;reason=" + ex.Message);
            }
            finally
            {
                lock (gate)
                {
                    visionThread = null;
                }
            }
        }

        private void OnVisionEvent(string text)
        {
            VisionEventReceived?.Invoke(text ?? string.Empty);
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(NativeVisionBridge));
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            try
            {
                StopVision();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[VisionHost] Dispose stop failure: " + ex);
            }
        }
    }
}
