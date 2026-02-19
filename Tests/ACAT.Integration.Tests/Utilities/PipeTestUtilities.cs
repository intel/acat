////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Utility.NamedPipe;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;

namespace ACAT.Integration.Tests.Utilities
{
    /// <summary>
    /// Helper utilities for writing integration tests that involve named-pipe
    /// communication between ACAT components.
    /// </summary>
    public static class PipeTestUtilities
    {
        /// <summary>
        /// Default timeout in milliseconds used when waiting for pipe events.
        /// </summary>
        public const int DefaultTimeoutMs = 3000;

        /// <summary>
        /// Creates a unique pipe name that is safe to use in a single test run.
        /// </summary>
        public static string CreateUniquePipeName(string prefix = "acat_test") =>
            $"{prefix}_{Guid.NewGuid():N}";

        /// <summary>
        /// Creates a <see cref="PipeServer"/> listening on <paramref name="pipeName"/>,
        /// starts it, and returns both the server and a <see cref="ManualResetEventSlim"/>
        /// that is set the first time a message arrives.
        /// </summary>
        /// <param name="pipeName">Name of the pipe to create.</param>
        /// <param name="receivedMessages">
        /// Collection that will be populated with every message received by the server.
        /// </param>
        /// <param name="messageEvent">
        /// Event that is signalled when the first message is received.
        /// </param>
        /// <returns>The started <see cref="PipeServer"/>.</returns>
        public static PipeServer CreateAndStartServer(
            string pipeName,
            IList<string> receivedMessages,
            ManualResetEventSlim messageEvent)
        {
            if (receivedMessages == null) throw new ArgumentNullException(nameof(receivedMessages));
            if (messageEvent == null) throw new ArgumentNullException(nameof(messageEvent));

            var server = new PipeServer(pipeName, PipeDirection.InOut);
            server.MessageReceived += (_, args) =>
            {
                if (args?.Message != null)
                {
                    receivedMessages.Add(args.Message);
                    messageEvent.Set();
                }
            };
            server.Start();
            return server;
        }

        /// <summary>
        /// Creates a <see cref="PipeClient"/>, connects it to an already-started server,
        /// and returns the client.
        /// </summary>
        /// <param name="pipeName">Name of the pipe to connect to.</param>
        /// <param name="timeoutMs">
        /// Connection timeout in milliseconds. Defaults to <see cref="DefaultTimeoutMs"/>.
        /// </param>
        /// <returns>The connected <see cref="PipeClient"/>.</returns>
        public static PipeClient ConnectClient(string pipeName, int timeoutMs = DefaultTimeoutMs)
        {
            var client = new PipeClient(pipeName, PipeDirection.InOut);
            client.Connect(timeoutMs);
            return client;
        }

        /// <summary>
        /// Waits for <paramref name="messageEvent"/> to be signalled within
        /// <paramref name="timeoutMs"/> milliseconds.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the event was signalled; <see langword="false"/>
        /// if the wait timed out.
        /// </returns>
        public static bool WaitForMessage(ManualResetEventSlim messageEvent, int timeoutMs = DefaultTimeoutMs) =>
            messageEvent.Wait(timeoutMs);

        /// <summary>
        /// Disposes a <see cref="PipeServer"/> without throwing, allowing tests to
        /// clean up even when the server is in a faulted state.
        /// </summary>
        public static void SafeDisposeServer(PipeServer server)
        {
            try
            {
                server?.Stop();
                server?.Dispose();
            }
            catch
            {
                // Best-effort cleanup.
            }
        }

        /// <summary>
        /// Disposes a <see cref="PipeClient"/> without throwing.
        /// </summary>
        public static void SafeDisposeClient(PipeClient client)
        {
            try
            {
                client?.Dispose();
            }
            catch
            {
                // Best-effort cleanup.
            }
        }
    }
}
