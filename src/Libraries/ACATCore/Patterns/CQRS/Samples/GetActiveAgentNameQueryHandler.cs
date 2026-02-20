////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// GetActiveAgentNameQueryHandler.cs
//
// Sample CQRS query handler that retrieves the name of the currently
// active application agent without causing any side effects.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Handles <see cref="GetActiveAgentNameQuery"/> by delegating to
    /// <see cref="IAgentManager.GetCurrentAgentName"/>.
    /// </summary>
    public class GetActiveAgentNameQueryHandler
        : IQueryHandler<GetActiveAgentNameQuery, string>
    {
        private readonly IAgentManager _agentManager;

        /// <summary>
        /// Initialises a new <see cref="GetActiveAgentNameQueryHandler"/>.
        /// </summary>
        /// <param name="agentManager">The agent manager to query.</param>
        public GetActiveAgentNameQueryHandler(IAgentManager agentManager)
        {
            _agentManager = agentManager;
        }

        /// <inheritdoc />
        public string Handle(GetActiveAgentNameQuery query)
        {
            return _agentManager.GetCurrentAgentName() ?? string.Empty;
        }
    }
}
