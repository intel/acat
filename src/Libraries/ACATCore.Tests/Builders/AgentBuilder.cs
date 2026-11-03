////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// AgentBuilder.cs
//
// Fluent builder for constructing agent-related test data for ACAT tests.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace ACATCore.Tests.Builders
{
    /// <summary>
    /// Describes the configuration of an application agent for use in tests.
    /// This is a lightweight data-only representation of the information
    /// required to register or configure an agent in ACAT.
    /// </summary>
    public sealed class AgentDescriptor
    {
        /// <summary>Display name of the agent.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Unique identifier for the agent.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Fully-qualified type name of the agent implementation,
        /// used to resolve it at runtime via reflection.
        /// </summary>
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// Process names this agent handles (e.g., "notepad", "winword").
        /// </summary>
        public List<string> SupportedProcesses { get; set; } = new List<string>();

        /// <summary>Whether this agent is enabled.</summary>
        public bool IsEnabled { get; set; } = true;
    }

    /// <summary>
    /// Fluent builder for <see cref="AgentDescriptor"/> test data.
    /// </summary>
    public sealed class AgentBuilder
    {
        private string _name = "TestAgent";
        private Guid _id = Guid.NewGuid();
        private string _typeName = string.Empty;
        private readonly List<string> _supportedProcesses = new List<string>();
        private bool _isEnabled = true;

        /// <summary>Sets the agent name.</summary>
        public AgentBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>Sets the agent GUID.</summary>
        public AgentBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>Sets the fully-qualified type name of the agent.</summary>
        public AgentBuilder WithTypeName(string typeName)
        {
            _typeName = typeName;
            return this;
        }

        /// <summary>Adds a process name that this agent handles.</summary>
        public AgentBuilder WithSupportedProcess(string processName)
        {
            _supportedProcesses.Add(processName);
            return this;
        }

        /// <summary>Enables or disables the agent.</summary>
        public AgentBuilder WithEnabled(bool enabled)
        {
            _isEnabled = enabled;
            return this;
        }

        /// <summary>Builds the <see cref="AgentDescriptor"/>.</summary>
        public AgentDescriptor Build()
        {
            return new AgentDescriptor
            {
                Name = _name,
                Id = _id,
                TypeName = _typeName,
                SupportedProcesses = new List<string>(_supportedProcesses),
                IsEnabled = _isEnabled
            };
        }

        /// <summary>Returns a builder pre-configured for a generic app agent.</summary>
        public static AgentBuilder AsGenericAppAgent()
        {
            return new AgentBuilder()
                .WithName("GenericAppAgent")
                .WithTypeName("ACAT.Core.AgentManagement.GenericAppAgentBase")
                .WithEnabled(true);
        }

        /// <summary>Returns a builder pre-configured for a Notepad agent.</summary>
        public static AgentBuilder AsNotepadAgent()
        {
            return new AgentBuilder()
                .WithName("NotepadAgent")
                .WithTypeName("ACAT.Applications.NotepadAgent")
                .WithSupportedProcess("notepad")
                .WithEnabled(true);
        }
    }
}
