using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using VisionContracts.Contracts;

namespace VisionHost
{
    internal sealed class VisionHostServer : IDisposable
    {
        private readonly NativeVisionBridge nativeBridge;
        private readonly ManualResetEvent shutdownRequested;
        private readonly CancellationTokenSource cancellationSource;
        private readonly object writeLock;
        private NamedPipeServerStream currentServer;
        private StreamWriter currentWriter;

        public VisionHostServer(NativeVisionBridge nativeBridge)
        {
            this.nativeBridge = nativeBridge ?? throw new ArgumentNullException(nameof(nativeBridge));
            shutdownRequested = new ManualResetEvent(false);
            cancellationSource = new CancellationTokenSource();
            writeLock = new object();
            this.nativeBridge.VisionEventReceived += NativeBridgeOnVisionEventReceived;
        }

        public WaitHandle ShutdownSignal => shutdownRequested;

        public void Run()
        {
            while (!cancellationSource.IsCancellationRequested)
            {
                using (var server = CreateServer())
                {
                    currentServer = server;
                    try
                    {
                        server.WaitForConnection();
                        using (var reader = new StreamReader(server))
                        using (var writer = new StreamWriter(server) { AutoFlush = true })
                        {
                            currentWriter = writer;
                            SendEvent(new VisionHostEvent
                            {
                                EventType = VisionHostEventType.HostReady,
                                Payload = "VisionHost connected."
                            });

                            string line;
                            while (!cancellationSource.IsCancellationRequested && (line = reader.ReadLine()) != null)
                            {
                                ProcessCommand(line);
                            }
                        }
                    }
                    catch (IOException ioEx)
                    {
                        Trace.WriteLine("[VisionHost] Pipe IO error: " + ioEx);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("[VisionHost] Host loop failure: " + ex);
                    }
                    finally
                    {
                        currentWriter = null;
                        currentServer = null;
                    }
                }
            }
        }

        private static NamedPipeServerStream CreateServer()
        {
            return new NamedPipeServerStream(
                VisionPipeSettings.PipeName,
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
        }

        private void ProcessCommand(string serializedCommand)
        {
            VisionHostCommand command;
            try
            {
                command = VisionPipeProtocol.DeserializeCommand(serializedCommand);
            }
            catch (Exception ex)
            {
                SendError("Invalid command payload: " + ex.Message, null);
                return;
            }

            try
            {
                switch (command.CommandType)
                {
                    case VisionHostCommandType.StartVision:
                        nativeBridge.StartVision();
                        SendEvent(new VisionHostEvent
                        {
                            EventType = VisionHostEventType.HostReady,
                            Payload = "Vision started.",
                            CorrelationId = command.CorrelationId
                        });
                        break;

                    case VisionHostCommandType.StopVision:
                        nativeBridge.StopVision();
                        SendEvent(new VisionHostEvent
                        {
                            EventType = VisionHostEventType.VisionStopped,
                            Payload = "Vision stopped.",
                            CorrelationId = command.CorrelationId
                        });
                        break;

                    case VisionHostCommandType.ShutdownHost:
                        // Commit and ack before touching native code: a wedged native
                        // stop must not block the client's shutdown handshake.
                        cancellationSource.Cancel();
                        shutdownRequested.Set();
                        SendEvent(new VisionHostEvent
                        {
                            EventType = VisionHostEventType.HostStopped,
                            Payload = "Host shutting down.",
                            CorrelationId = command.CorrelationId
                        });
                        nativeBridge.StopVision();
                        break;

                    case VisionHostCommandType.NativeCommand:
                        nativeBridge.SendNativeCommand(command.Payload ?? string.Empty, command.Value);
                        break;

                    case VisionHostCommandType.ShowVideo:
                        nativeBridge.ShowVideo();
                        break;

                    case VisionHostCommandType.HideVideo:
                        nativeBridge.HideVideo();
                        break;
                }
            }
            catch (Exception ex)
            {
                SendError(ex.Message, command.CorrelationId);
            }
        }

        private void NativeBridgeOnVisionEventReceived(string nativeEvent)
        {
            SendEvent(new VisionHostEvent
            {
                EventType = VisionHostEventType.NativeEvent,
                Payload = nativeEvent
            });
        }

        private void SendError(string message, string correlationId)
        {
            SendEvent(new VisionHostEvent
            {
                EventType = VisionHostEventType.HostError,
                Payload = message,
                CorrelationId = correlationId
            });
        }

        private void SendEvent(VisionHostEvent hostEvent)
        {
            var writer = currentWriter;
            if (writer == null)
            {
                return;
            }

            string payload = VisionPipeProtocol.SerializeEvent(hostEvent);
            try
            {
                lock (writeLock)
                {
                    writer.WriteLine(payload);
                }
            }
            catch (IOException ex)
            {
                Trace.WriteLine("[VisionHost] Event write failed (client gone?): " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Dispose()
        {
            cancellationSource.Cancel();
            shutdownRequested.Set();
            nativeBridge.VisionEventReceived -= NativeBridgeOnVisionEventReceived;

            try
            {
                currentServer?.Dispose();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[VisionHost] Dispose pipe failure: " + ex);
            }

            shutdownRequested.Dispose();
            cancellationSource.Dispose();
        }
    }
}
