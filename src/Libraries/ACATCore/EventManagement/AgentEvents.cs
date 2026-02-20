////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AgentEvents.cs
//
// Event types for agent notifications (context change).
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.EventManagement
{
    /// <summary>
    /// Published when an agent's context changes (e.g. the active application
    /// window that the agent is managing has changed).
    /// </summary>
    public class AgentContextChangedEvent : EventBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AgentContextChangedEvent"/>.
        /// </summary>
        /// <param name="agentName">The name of the agent whose context changed.</param>
        /// <param name="context">
        /// Application-defined context object describing the new context
        /// (may be <c>null</c>).
        /// </param>
        public AgentContextChangedEvent(string agentName, object context)
        {
            AgentName = agentName;
            Context = context;
        }

        /// <summary>
        /// Gets the name of the agent whose context changed.
        /// </summary>
        public string AgentName { get; }

        /// <summary>
        /// Gets the application-defined context object for the new context.
        /// </summary>
        public object Context { get; }
    }
}
