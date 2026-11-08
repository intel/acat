using System;
using System.Diagnostics;

namespace VisionHost
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            WatchParentProcess(args);

            using (var bridge = new NativeVisionBridge())
            using (var hostServer = new VisionHostServer(bridge))
            {
                hostServer.Run();
            }

            // The native DLL may leave threads running; never let them keep a
            // headless helper process alive after shutdown.
            Environment.Exit(0);
        }

        // Exit with the launching client so a broken shutdown handshake can't leave a zombie host.
        private static void WatchParentProcess(string[] args)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (!string.Equals(args[i], "--parent-pid", StringComparison.OrdinalIgnoreCase)
                    || !int.TryParse(args[i + 1], out int parentPid))
                {
                    continue;
                }

                try
                {
                    var parent = Process.GetProcessById(parentPid);
                    parent.EnableRaisingEvents = true;
                    parent.Exited += (s, e) => Environment.Exit(0);
                    if (parent.HasExited)
                    {
                        Environment.Exit(0);
                    }
                }
                catch (ArgumentException)
                {
                    Environment.Exit(0);
                }

                return;
            }
        }
    }
}
