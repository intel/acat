using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using VisionContracts.Contracts;

namespace VisionWindowsAppCS
{
    internal sealed class VisionHostClient : IDisposable
    {
        private readonly object gate = new object();
        private readonly object writeGate = new object();
        private NamedPipeClientStream pipe;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread readerThread;
        private Process hostProcess;
        private bool disposed;

        public event Action<VisionHostEvent> EventReceived;
        public event Action Disconnected;

        public bool IsConnected
        {
            get
            {
                lock (gate)
                {
                    return pipe != null && pipe.IsConnected;
                }
            }
        }

        public void EnsureConnected()
        {
            lock (gate)
            {
                ThrowIfDisposed();
                if (pipe != null && pipe.IsConnected)
                {
                    return;
                }

                StartHostProcess();
                ConnectPipe();
                StartReaderLoop();
            }
        }

        public void SendCommand(VisionHostCommandType commandType, string payload = "", double value = 0, string correlationId = null)
        {
            ThrowIfDisposed();

            var command = new VisionHostCommand
            {
                CommandType = commandType,
                Payload = payload ?? string.Empty,
                Value = value,
                CorrelationId = correlationId ?? Guid.NewGuid().ToString("N")
            };

            StreamWriter localWriter;
            lock (gate)
            {
                localWriter = writer;
            }

            if (localWriter == null)
            {
                throw new InvalidOperationException("Vision host pipe is not connected.");
            }

            lock (writeGate)
            {
                localWriter.WriteLine(VisionPipeProtocol.SerializeCommand(command));
            }
        }

        private void StartHostProcess()
        {
            if (hostProcess != null && !hostProcess.HasExited)
            {
                return;
            }

            var hostPath = ResolveHostPath();
            var startInfo = new ProcessStartInfo
            {
                FileName = hostPath,
                Arguments = "--parent-pid " + Process.GetCurrentProcess().Id,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            Trace.WriteLine("[VisionHostClient] Starting host: " + hostPath);
            hostProcess = Process.Start(startInfo);
            if (hostProcess == null)
            {
                throw new InvalidOperationException("Unable to start VisionHost process.");
            }
        }

        private static string ResolveHostPath()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configuration = baseDir.IndexOf("\\Release\\", StringComparison.OrdinalIgnoreCase) >= 0 ? "Release" : "Debug";

            var candidates = new[]
            {
                Path.Combine(baseDir, "VisionHost.exe"),
                Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "VisionHost", "bin", configuration, "VisionHost.exe")),
                Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "VisionHost", "bin", configuration, "VisionHost.exe")),
                Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "VisionHost", "bin", configuration, "VisionHost.exe"))
            };

            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException("VisionHost.exe was not found. Checked: " + string.Join(" | ", candidates), candidates[0]);
        }

        private void ConnectPipe()
        {
            // Asynchronous handle: closing the pipe must cancel a pending read instead
            // of blocking Dispose (closing a handle with a pending sync read hangs).
            pipe = new NamedPipeClientStream(".", VisionPipeSettings.PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);

            var connectTimeoutMs = 15000;
            var start = Environment.TickCount;
            while (true)
            {
                try
                {
                    pipe.Connect(1000);
                    break;
                }
                catch (TimeoutException)
                {
                    if (hostProcess != null && hostProcess.HasExited)
                    {
                        throw new InvalidOperationException("VisionHost exited before pipe connection could be established.");
                    }

                    var elapsed = unchecked(Environment.TickCount - start);
                    if (elapsed >= connectTimeoutMs)
                    {
                        throw;
                    }
                }
            }

            Trace.WriteLine("[VisionHostClient] Pipe connected.");
            reader = new StreamReader(pipe);
            writer = new StreamWriter(pipe) { AutoFlush = true };
        }

        private void StartReaderLoop()
        {
            if (readerThread != null && readerThread.IsAlive)
            {
                return;
            }

            readerThread = new Thread(ReadEvents)
            {
                IsBackground = true,
                Name = "VisionHostClient.Reader"
            };
            readerThread.Start();
        }

        private void ReadEvents()
        {
            try
            {
                while (!disposed)
                {
                    StreamReader localReader;
                    lock (gate)
                    {
                        localReader = reader;
                    }

                    if (localReader == null)
                    {
                        return;
                    }

                    string line = localReader.ReadLine();
                    if (line == null)
                    {
                        return;
                    }

                    VisionHostEvent hostEvent;
                    try
                    {
                        hostEvent = VisionPipeProtocol.DeserializeEvent(line);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("[VisionHostClient] Event parse failure: " + ex);
                        continue;
                    }

                    EventReceived?.Invoke(hostEvent);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (IOException ioEx)
            {
                Trace.WriteLine("[VisionHostClient] Pipe reader ended: " + ioEx.Message);
            }
            finally
            {
                if (!disposed)
                {
                    Disconnected?.Invoke();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(VisionHostClient));
            }
        }

        public bool TryTerminateOwnedHostProcess(int timeoutMs)
        {
            lock (gate)
            {
                if (hostProcess == null || hostProcess.HasExited)
                {
                    return true;
                }

                try
                {
                    if (hostProcess.WaitForExit(timeoutMs))
                    {
                        return true;
                    }

                    hostProcess.Kill();
                    hostProcess.WaitForExit(2000);
                    return true;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("[VisionHostClient] Unable to terminate owned host: " + ex);
                    return false;
                }
            }
        }

        public void Dispose()
        {
            disposed = true;

            // Terminate the host first: a dead host breaks the pipe and unblocks the
            // reader thread before any handle is closed on this thread.
            TryTerminateOwnedHostProcess(2000);

            Thread readerToJoin;
            lock (gate)
            {
                readerToJoin = readerThread;
                try { writer?.Dispose(); } catch { }
                try { reader?.Dispose(); } catch { }
                try { pipe?.Dispose(); } catch { }

                writer = null;
                reader = null;
                pipe = null;
            }

            if (readerToJoin != null && readerToJoin != Thread.CurrentThread)
            {
                readerToJoin.Join(1000);
            }

            lock (gate)
            {
                hostProcess?.Dispose();
                hostProcess = null;
            }
        }
    }
}
