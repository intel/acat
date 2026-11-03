////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// GetActiveAgentNameQuery.cs
//
// Sample CQRS query that retrieves the name of the currently active
// application agent from the AgentManager.
//
////////////////////////////////////////////////////////////////////////////

namespace ACAT.Core.Patterns.CQRS.Samples
{
    /// <summary>
    /// Query that requests the name of the currently active agent.
    /// Returns the agent name string, or an empty string when no agent is active.
    /// </summary>
    public class GetActiveAgentNameQuery : IQuery<string>
    {
    }
}
