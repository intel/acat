////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.AgentManagement.TextInterface;
using System;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Maintains the active context of the currently active agent. Access
    /// the agent's text control agent ONLY through the agent context. Let's
    /// say the caller has bunch of text manipulations to carry out through
    /// the text control agent.  It is likely that a context switch may occur
    /// during this and text control agent that the caller has been using has
    /// been disposed.
    /// Accessing textcontrol agent through the AgentContextis safe as this
    /// class throws an exception if the text control agent is no longer valid.
    /// </summary>
    public class AgentContext : IDisposable
    {
        /// <summary>
        /// Has this been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The text control agent object
        /// </summary>
        private ITextControlAgent _textControlAgent;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="textControlAgent">currently active text control agent object</param>
        internal AgentContext(ITextControlAgent textControlAgent)
        {
            _textControlAgent = textControlAgent;
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Is the active text control agent still valid? Has it changed?
        /// Has it been disposed?
        /// </summary>
        /// <returns></returns>
        public bool IsDisposed()
        {
            return _disposed || _textControlAgent == null ||
                    _textControlAgent != AgentManager.Instance.TextControlAgent ||
                    _textControlAgent.IsDisposed();
        }

        /// <summary>
        /// Returns the text control agent object associated with this AgentContext
        /// </summary>
        /// <returns>Text control agent object</returns>
        public ITextControlAgent TextAgent()
        {
            if (IsDisposed())
            {
                throw new InvalidAgentContextException();
            }

            return _textControlAgent;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">Disposed yet?</param>
        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                _textControlAgent = null;
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Excpetion thrown if the agent context is invalid
    /// </summary>
    public class InvalidAgentContextException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public InvalidAgentContextException()
            : base("Context is no longer valid")
        {
        }
    }
}