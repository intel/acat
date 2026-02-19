////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Integration.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;

namespace ACAT.Integration.Tests.Tests
{
    /// <summary>
    /// Integration tests for named-pipe communication between ACAT components.
    /// </summary>
    [TestClass]
    public class NamedPipeCommunicationTests
    {
        [TestMethod]
        public void Pipe_ServerCanBeCreatedAndDisposed()
        {
            // Arrange
            string pipeName = PipeTestUtilities.CreateUniquePipeName();
            var messages = new List<string>();
            using var messageEvent = new ManualResetEventSlim(false);

            // Act & Assert – no exception should be thrown
            using var server = PipeTestUtilities.CreateAndStartServer(pipeName, messages, messageEvent);
            Assert.IsNotNull(server);
        }

        [TestMethod]
        public void Pipe_ClientCanConnectToServer()
        {
            // Arrange
            string pipeName = PipeTestUtilities.CreateUniquePipeName();
            var messages = new List<string>();
            using var messageEvent = new ManualResetEventSlim(false);

            using var server = PipeTestUtilities.CreateAndStartServer(pipeName, messages, messageEvent);

            // Act
            using var client = PipeTestUtilities.ConnectClient(pipeName);

            // Assert
            Assert.IsNotNull(client, "Client should connect to the server without error.");
        }

        [TestMethod]
        public void Pipe_ClientCanSendMessageToServer()
        {
            // Arrange
            const string expectedMessage = "hello_pipe";
            string pipeName = PipeTestUtilities.CreateUniquePipeName();
            var messages = new List<string>();
            using var messageEvent = new ManualResetEventSlim(false);

            using var server = PipeTestUtilities.CreateAndStartServer(pipeName, messages, messageEvent);
            using var client = PipeTestUtilities.ConnectClient(pipeName);

            // Act
            client.Send(expectedMessage);
            bool received = PipeTestUtilities.WaitForMessage(messageEvent);

            // Assert
            Assert.IsTrue(received, "Server should receive the message within the timeout period.");
            Assert.AreEqual(1, messages.Count, "Exactly one message should have been received.");
            Assert.AreEqual(expectedMessage, messages[0], "Received message content should match what was sent.");
        }

        [TestMethod]
        public void Pipe_MultipleMessagesCanBeSent()
        {
            // Arrange
            string pipeName = PipeTestUtilities.CreateUniquePipeName();
            var messages = new List<string>();
            // We need to wait for all 3 messages, so reset the event manually each time.
            using var allReceivedEvent = new ManualResetEventSlim(false);
            int receiveCount = 0;

            using var server = PipeTestUtilities.CreateAndStartServer(pipeName, messages, allReceivedEvent);
            // Override: attach an additional handler to count messages
            server.MessageReceived += (_, __) =>
            {
                if (Interlocked.Increment(ref receiveCount) >= 3)
                {
                    allReceivedEvent.Set();
                }
            };

            using var client = PipeTestUtilities.ConnectClient(pipeName);

            // Act
            client.Send("msg1");
            client.Send("msg2");
            client.Send("msg3");
            bool allReceived = PipeTestUtilities.WaitForMessage(allReceivedEvent, timeoutMs: 5000);

            // Assert
            Assert.IsTrue(allReceived, "All three messages should arrive within the timeout.");
            Assert.AreEqual(3, messages.Count, "Exactly 3 messages should have been received.");
            CollectionAssert.AreEquivalent(
                new[] { "msg1", "msg2", "msg3" },
                messages,
                "Received messages should match the sent content.");
        }

        [TestMethod]
        public void Pipe_ServerIsDisposedCleanly()
        {
            // Arrange
            string pipeName = PipeTestUtilities.CreateUniquePipeName();
            var messages = new List<string>();
            using var messageEvent = new ManualResetEventSlim(false);

            var server = PipeTestUtilities.CreateAndStartServer(pipeName, messages, messageEvent);

            // Act & Assert – disposal should not throw
            PipeTestUtilities.SafeDisposeServer(server);
        }

        [TestMethod]
        public void Pipe_UniqueNamesAreDistinct()
        {
            // Arrange & Act
            string name1 = PipeTestUtilities.CreateUniquePipeName("test");
            string name2 = PipeTestUtilities.CreateUniquePipeName("test");

            // Assert
            Assert.AreNotEqual(name1, name2,
                "Each call to CreateUniquePipeName should return a different value.");
        }
    }
}
