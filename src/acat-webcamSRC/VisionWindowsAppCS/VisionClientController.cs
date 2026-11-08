using System;
using System.Diagnostics;
using System.Threading;
using VisionContracts.Contracts;

namespace VisionWindowsAppCS
{
    internal sealed class VisionClientController : IDisposable
    {
        private readonly VisionHostClient hostClient;
        private readonly ManualResetEvent hostStoppedEvent;
        private bool shutdownRequested;
        private bool disposed;

        public event Action<string> MessageReceived;
        public event Action<VisionHostEvent> HostEventReceived;
        public event Action HostDisconnected;

        public VisionClientController()
        {
            hostClient = new VisionHostClient();
            hostClient.EventReceived += OnHostEventReceived;
            hostClient.Disconnected += OnHostDisconnected;
            hostStoppedEvent = new ManualResetEvent(false);
        }

        public void StartVision()
        {
            hostClient.EnsureConnected();
            hostClient.SendCommand(VisionHostCommandType.StartVision);
        }

        public void StopVision(string correlationId = null)
        {
            if (!hostClient.IsConnected)
            {
                throw new InvalidOperationException("Vision host is not connected.");
            }

            hostClient.SendCommand(VisionHostCommandType.StopVision, correlationId: correlationId);
        }

        public void SendNativeCommand(string command, double value)
        {
            hostClient.EnsureConnected();
            hostClient.SendCommand(VisionHostCommandType.NativeCommand, command, value);
        }

        public void ShowVideo()
        {
            hostClient.EnsureConnected();
            hostClient.SendCommand(VisionHostCommandType.ShowVideo);
        }

        public void HideVideo()
        {
            hostClient.EnsureConnected();
            hostClient.SendCommand(VisionHostCommandType.HideVideo);
        }

        public void ShutdownHost(int timeoutMs = 3000)
        {
            if (shutdownRequested)
            {
                return;
            }

            shutdownRequested = true;
            try
            {
                if (hostClient.IsConnected)
                {
                    hostStoppedEvent.Reset();
                    hostClient.SendCommand(VisionHostCommandType.ShutdownHost);
                    if (!hostStoppedEvent.WaitOne(timeoutMs))
                    {
                        Trace.WriteLine("[VisionClientController] Host shutdown ack timed out.");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[VisionClientController] ShutdownHost failure: " + ex);
            }
            finally
            {
                // The ack only means the host intends to exit; make sure it actually does.
                hostClient.TryTerminateOwnedHostProcess(2000);
            }
        }

        private void OnHostEventReceived(VisionHostEvent hostEvent)
        {
            if (hostEvent == null)
            {
                return;
            }

            if (hostEvent.EventType == VisionHostEventType.HostStopped)
            {
                hostStoppedEvent.Set();
            }

            HostEventReceived?.Invoke(hostEvent);
            switch (hostEvent.EventType)
            {
                case VisionHostEventType.NativeEvent:
                    MessageReceived?.Invoke(hostEvent.Payload ?? string.Empty);
                    break;
            }
        }

        private void OnHostDisconnected()
        {
            HostDisconnected?.Invoke();
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
                ShutdownHost();
            }
            catch { }

            hostClient.EventReceived -= OnHostEventReceived;
            hostClient.Disconnected -= OnHostDisconnected;
            hostClient.Dispose();
            hostStoppedEvent.Dispose();
        }
    }
}
