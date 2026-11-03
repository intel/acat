////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AgentManagerExtensions.cs
//
// Extension methods for IAgentManager that provide CQRS-based wrappers
// around agent query operations.  Each method resolves the registered
// IQueryHandler from the DI container when available and falls back to
// calling the IAgentManager method directly when running outside a
// DI-configured environment (e.g., unit tests or legacy entry points).
//
// Usage – call-site migration:
//
//   Before:
//       string name = Context.AppAgentMgr.GetCurrentAgentName();
//
//   After:
//       string name = Context.AppAgentMgr.GetCurrentAgentNameViaQuery();
//
// The extension method body can be simplified to the CQRS-only path once
// every call site has been migrated and legacy fallback is no longer needed.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement;
using ACAT.Core.PanelManagement;
using ACAT.Core.Patterns.CQRS.Samples;

namespace ACAT.Core.Patterns.CQRS
{
    /// <summary>
    /// Extension methods for <see cref="IAgentManager"/> that route agent
    /// queries through the CQRS <see cref="IQueryHandler{TQuery,TResult}"/>
    /// infrastructure when available, falling back to direct
    /// <see cref="IAgentManager"/> calls when a DI container is not configured.
    /// </summary>
    public static class AgentManagerExtensions
    {
        /// <summary>
        /// Returns the name of the currently active agent by dispatching a
        /// <see cref="GetActiveAgentNameQuery"/> through the registered
        /// <see cref="IQueryHandler{TQuery,TResult}"/>.
        /// </summary>
        /// <remarks>
        /// When <see cref="Context.ServiceProvider"/> is configured the call
        /// is routed through the DI-registered handler so that the CQRS
        /// boundary is respected.  When no provider is available the method
        /// falls back to <see cref="IAgentManager.GetCurrentAgentName"/> and
        /// normalises a <see langword="null"/> return to
        /// <see cref="string.Empty"/>.
        /// </remarks>
        /// <param name="agentManager">The agent manager to query.</param>
        /// <returns>
        /// The name of the active agent, or <see cref="string.Empty"/> when
        /// no agent is currently active.
        /// </returns>
        public static string GetCurrentAgentNameViaQuery(this IAgentManager agentManager)
        {
            var handler = Context.ServiceProvider
                ?.GetService(typeof(IQueryHandler<GetActiveAgentNameQuery, string>))
                    as IQueryHandler<GetActiveAgentNameQuery, string>;

            if (handler != null)
            {
                return handler.Handle(new GetActiveAgentNameQuery());
            }

            return agentManager.GetCurrentAgentName() ?? string.Empty;
        }
    }
}
