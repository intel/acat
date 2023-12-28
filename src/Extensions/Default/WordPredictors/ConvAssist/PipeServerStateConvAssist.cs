////////////////////////////////////////////////////////////////////////////////////
///
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// PipeServerState.cs
//
// Interop functions into User32.dll
//
////////////////////////////////////////////////////////////////////////////////////

using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace ACAT.Extensions.Default.WordPredictors.ConvAssist
{
    public class PipeServerStateConvAssist
    {
        /// <summary>
        ///     The buffer size.
        /// </summary>
        public const int BufferSize = 8125;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeServerStateConvAssist"/> class.
        /// </summary>
        /// <param name="pipeServer">
        /// The pipe server instance.
        /// </param>
        /// <param name="token">
        /// A token referenced by and external cancellation token.
        /// </param>
        public PipeServerStateConvAssist(NamedPipeServerStream pipeServer, CancellationToken token)
            : this(pipeServer, new byte[BufferSize], token)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeServerStateConvAssist"/> class.
        /// </summary>
        /// <param name="pipeServer">
        /// The pipe server instance.
        /// </param>
        /// <param name="buffer">
        /// The byte buffer.
        /// </param>
        /// <param name="token">
        /// A token referenced by and external cancellation token.
        /// </param>
        public PipeServerStateConvAssist(NamedPipeServerStream pipeServer, byte[] buffer, CancellationToken token)
        {
            Buffer = buffer;
            ExternalCancellationToken = token;
            PipeServer = pipeServer;
            Message = new StringBuilder();
        }

        /// <summary>
        /// Gets the byte buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// The external cancellation token.
        /// </summary>
        public CancellationToken ExternalCancellationToken { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public StringBuilder Message { get; private set; }

        /// <summary>
        /// Gets the pipe server.
        /// </summary>
        public NamedPipeServerStream PipeServer { get; private set; }
    }
}