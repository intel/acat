////////////////////////////////////////////////////////////////////////////
// <copyright file="AgentContext.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.AgentManagement.TextInterface;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

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