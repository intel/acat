using ACAT.Extensions.WordPredictors.ConvAssist;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ACATExtension.Tests
{
    public class ConvAssistPipeTests
    {
        [Fact]
        public async Task WriteAsync_SerializesRequestsUntilTheirResponsesArrive()
        {
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var token = cancellation.Token;
            var pipeName = "ACATConvAssistTest-" + Guid.NewGuid().ToString("N");
            using var server = new NamedPipeServerConvAssist(pipeName, PipeDirection.InOut, string.Empty,
                NullLogger<NamedPipeServerConvAssist>.Instance);
            using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            try
            {
                var startup = server.CreatePipeServerAsync(false, token);
                await client.ConnectAsync(5000, token).ConfigureAwait(false);
                Assert.True(await startup.ConfigureAwait(false));

                var first = server.WriteAsync("first π", 2000);
                Assert.Equal("first π", await ReadFrameAsync(client, token).ConfigureAwait(false));
                var second = server.WriteAsync("second", 2000);
                var nextFrame = ReadFrameAsync(client, token);

                Assert.NotSame(nextFrame, await Task.WhenAny(nextFrame, Task.Delay(100, token)).ConfigureAwait(false));
                Assert.False(first.IsCompleted);
                Assert.False(second.IsCompleted);

                await WriteFrameAsync(client, "first response π", token).ConfigureAwait(false);
                Assert.Equal("first response π", await first.ConfigureAwait(false));
                Assert.Equal("second", await nextFrame.ConfigureAwait(false));

                await WriteFrameAsync(client, "second response", token).ConfigureAwait(false);
                Assert.Equal("second response", await second.ConfigureAwait(false));
            }
            finally
            {
                server.ClosePipeServer();
            }
        }

        [Fact]
        public async Task WriteAsync_ReturnsEmptyOnResponseTimeoutAndReleasesRequestGate()
        {
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var token = cancellation.Token;
            var pipeName = "ACATConvAssistTest-" + Guid.NewGuid().ToString("N");
            using var server = new NamedPipeServerConvAssist(pipeName, PipeDirection.InOut, string.Empty,
                NullLogger<NamedPipeServerConvAssist>.Instance);
            using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            try
            {
                var startup = server.CreatePipeServerAsync(false, token);
                await client.ConnectAsync(5000, token).ConfigureAwait(false);
                Assert.True(await startup.ConfigureAwait(false));

                var unanswered = server.WriteAsync("unanswered", 100);
                Assert.Equal("unanswered", await ReadFrameAsync(client, token).ConfigureAwait(false));
                Assert.Equal(string.Empty, await unanswered.ConfigureAwait(false));
                Assert.True(server.TaskFinished);

                var next = server.WriteAsync("next", 2000);
                Assert.Equal("next", await ReadFrameAsync(client, token).ConfigureAwait(false));
                await WriteFrameAsync(client, "next response", token).ConfigureAwait(false);
                Assert.Equal("next response", await next.ConfigureAwait(false));
            }
            finally
            {
                server.ClosePipeServer();
            }
        }

        private static async Task<string> ReadFrameAsync(Stream stream, CancellationToken token)
        {
            var prefix = new byte[sizeof(int)];
            await ReadExactlyAsync(stream, prefix, token).ConfigureAwait(false);
            var length = BitConverter.ToInt32(prefix, 0);
            Assert.InRange(length, 1, PipeServerStateConvAssist.BufferSize - sizeof(int));
            var payload = new byte[length];
            await ReadExactlyAsync(stream, payload, token).ConfigureAwait(false);
            return Encoding.UTF8.GetString(payload);
        }

        private static async Task ReadExactlyAsync(Stream stream, byte[] buffer, CancellationToken token)
        {
            var offset = 0;
            while (offset < buffer.Length)
            {
                var count = await stream.ReadAsync(buffer, offset, buffer.Length - offset, token).ConfigureAwait(false);
                if (count == 0)
                {
                    throw new EndOfStreamException();
                }
                offset += count;
            }
        }

        private static Task WriteFrameAsync(Stream stream, string message, CancellationToken token)
        {
            var payload = Encoding.UTF8.GetBytes(message);
            var frame = new byte[sizeof(int) + payload.Length];
            Buffer.BlockCopy(BitConverter.GetBytes(payload.Length), 0, frame, 0, sizeof(int));
            Buffer.BlockCopy(payload, 0, frame, sizeof(int), payload.Length);
            return stream.WriteAsync(frame, 0, frame.Length, token);
        }
    }
}
