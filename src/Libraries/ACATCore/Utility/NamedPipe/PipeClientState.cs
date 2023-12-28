////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.IO.Pipes;
using System.Text;

namespace ACAT.Lib.Core.Utility.NamedPipe
{
    internal class PipeClientState
    {
        private const int BufferSize = 8125;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeClientState"/> class.
        /// </summary>
        /// <param name="pipeServer">
        /// The pipe server instance.
        /// </param>
        public PipeClientState(NamedPipeClientStream pipeServer)
            : this(pipeServer, new byte[BufferSize])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeClientState"/> class.
        /// </summary>
        /// <param name="pipeServer">
        /// The pipe server instance.
        /// </param>
        /// <param name="buffer">
        /// The byte buffer.
        /// </param>
        public PipeClientState(NamedPipeClientStream pipeServer, byte[] buffer)
        {
            this.PipeClient = pipeServer;
            this.Buffer = buffer;
            this.Message = new StringBuilder();
        }

        /// <summary>
        ///  Gets the byte buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public StringBuilder Message { get; private set; }

        /// <summary>
        /// Gets the pipe server.
        /// </summary>
        public NamedPipeClientStream PipeClient { get; private set; }
    }
}