////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AgentManagement.Interfaces;
using ACAT.Core.PanelManagement;
using ACAT.Core.Utility;
using ACAT.Core.Utility.TypeLoader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ACAT.Core.AgentManagement.Agents
{
    /// <summary>
    /// Maintains a cache of application agent objects.  The cache is
    /// populated by doing a directory walk of a base directory, dynamically
    /// loading DLL's that have IApplicationAgent objects, instantiating
    /// the objects and storing them in a table.
    /// Each application agent supports one or more processes (like notepad,
    /// ms word etc)
    /// </summary>
    internal class AgentsCache : IDisposable
    {
        private readonly ILogger<AgentsCache> _logger;

        /// <summary>
        /// Adhoc agents are those that can be added at runtime. Adhoc
        /// agents are attached to a window handle and activated when that
        /// window becomes active.
        /// </summary>
        private readonly Hashtable _adhocAgentTable;

        /// <summary>
        /// A list of application agents
        /// </summary>
        private readonly List<IApplicationAgent> _agentCache;

        private readonly TypeLoader<IApplicationAgent> _agentTypeLoader = new();

        /// <summary>
        /// Used to look up agents by agent id
        /// </summary>
        private readonly Hashtable _agentLookupTableById;

        /// <summary>
        /// Used to lookup agents by process name
        /// </summary>
        private readonly Hashtable _agentLookupTableByProcessName;

        /// <summary>
        /// If there are conflicts, which agent to use.  This is a
        /// list of preferred agents.
        /// </summary>
        private readonly PreferredAgents _preferredAgents;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AgentsCache(ILogger<AgentsCache> logger = null)
        {
            _logger = logger ?? LoggingConfiguration.CreateLogger<AgentsCache>();
            _logger?.LogTrace("AgentsCache created");

            _agentCache = new List<IApplicationAgent>();
            _agentLookupTableByProcessName = new Hashtable();
            _agentLookupTableById = new Hashtable();
            _adhocAgentTable = new Hashtable();
            _preferredAgents = new PreferredAgents();
        }

        /// <summary>
        /// Delegate for the event that's raised when an agent is detected
        /// and added to the cache
        /// </summary>
        /// <param name="agent"></param>
        public delegate void AgentAdded(IApplicationAgent agent);

        /// <summary>
        /// Raised when an agent is added
        /// </summary>
        public event AgentAdded EvtAgentAdded;

        /// <summary>
        /// Add the specified agent to the adhoc table.  When the window
        /// identified by 'handle' is activated, the agent is also
        /// activated.  Useful to add agents dynamically at runtime.
        /// </summary>
        /// <param name="handle">An application window handle</param>
        /// <param name="agent">Agent that supports the application</param>
        public void AddAgent(IntPtr handle, IApplicationAgent agent)
        {
            _adhocAgentTable[handle] = agent;
            EvtAgentAdded?.Invoke(agent);
        }

        /// <summary>
        /// Adds agent by the .NET class type
        /// </summary>
        /// <param name="type">The class Type of the agent</param>
        public void AddAgentByType(Type type)
        {
            addAgent(type);
            //populateLookupTableByProcess();
        }

        /// <summary>
        /// Disposer
        /// </summary>
        public void Dispose()
        {
            foreach (var agent in _agentCache)
            {
                agent.Dispose();
            }

            _agentCache.Clear();

            foreach (IApplicationAgent agent in _adhocAgentTable.Values)
            {
                agent.Dispose();
            }

            _adhocAgentTable.Clear();
            _preferredAgents.Dispose();
        }

        /// <summary>
        /// Retrieves adhoc agent that supports the window identified
        /// by the handle.  Use AddAgent to add an agent to the adhoc
        /// table
        /// </summary>
        /// <param name="handle">window handle</param>
        /// <returns>agent object, null if not found</returns>
        public IApplicationAgent GetAgent(IntPtr handle)
        {
            return (IApplicationAgent)_adhocAgentTable[handle];
        }

        public IntPtr GetHandleByAgentName(string agentName)
        {
            foreach (DictionaryEntry entry in _adhocAgentTable)
            {
                var agent = (IApplicationAgent)entry.Value;
                if (string.Compare(agent.Name, agentName, true) == 0)
                {
                    return (IntPtr)entry.Key;
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Returns the preferred agent that supports the process identified by
        /// the specified process name.
        /// </summary>
        /// <param name="processName">Name of the process</param>
        /// <returns>Agent object, null if not found</returns>
        public IApplicationAgent GetAgent(string processName)
        {
            // check if there is a configured preferred agent for the process
            var applicationAgent = _preferredAgents.GetPreferredAgentForProcess(processName);
            if (applicationAgent == null)
            {
                // Return the first agent from the list of supported agents.
                var supportedAgents = (List<IApplicationAgent>)_agentLookupTableByProcessName[processName.ToLower()];
                if (supportedAgents != null && supportedAgents.Count > 0)
                {
                    applicationAgent = supportedAgents[0];
                }
            }

            return applicationAgent;
        }

        /// <summary>
        /// Returns the preferred agent for the specified process
        /// </summary>
        /// <param name="process">Process</param>
        /// <returns>Agent object</returns>
        public IApplicationAgent GetAgent(Process process)
        {
            IApplicationAgent retVal = _preferredAgents.GetPreferredAgentForProcess(process);

            if (retVal != null)
            {
                return retVal;
            }

            // check if there is a preferred agent.  If not
            // look up the cache for the supported agent
            var supportedAgents = (List<IApplicationAgent>)_agentLookupTableByProcessName[process.ProcessName.ToLower()];

            if (supportedAgents != null)
            {
                foreach (var agent in supportedAgents)
                {
                    foreach (var processInfo in agent.ProcessesSupported)
                    {
                        if (string.IsNullOrEmpty(processInfo.ExecutablePath))
                        {
                            retVal = agent;
                        }
                        else if (string.Compare(process.MainModule.FileName, processInfo.ExecutablePath, true) == 0)
                        {
                            return agent;
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Returns the agent object identified by the category
        /// </summary>
        /// <param name="category">Category to lookup</param>
        /// <returns>Application agent object</returns>
        public IApplicationAgent GetAgentByCategory(string category)
        {
            _logger?.LogDebug("GetAgentByCategory called for: {Category}. AgentCache count: {Count}", category, _agentCache.Count);

            var retVal = _preferredAgents.GetPreferredAgentByCategory(category);
            if (retVal == null)
            {
                _logger?.LogDebug("No preferred agent found for category {Category}, searching in cache", category);
                foreach (var agent in _agentCache)
                {
                    if (string.Compare(category, agent.Descriptor.Category, true) == 0)
                    {
                        retVal = agent;
                        break;
                    }
                }
            }

            if (retVal == null)
            {
                _logger?.LogWarning("No agent found for category: {Category}", category);
            }

            return retVal;
        }

        /// <summary>
        /// Returns the agent object identified by the name
        /// </summary>
        /// <param name="name">Name to lookup</param>
        /// <returns>Application agent object</returns>
        public IApplicationAgent GetAgentByName(string name)
        {
            _logger?.LogDebug("GetAgentByName called for: {Name}. AgentCache count: {Count}", name, _agentCache.Count);

            var retVal = _preferredAgents.GetPreferredAgentByName(name);
            if (retVal == null)
            {
                _logger?.LogDebug("No preferred agent found for {Name}, searching in cache", name);
                foreach (var agent in _agentCache)
                {
                    if (string.Compare(name, agent.Name, true) == 0)
                    {
                        retVal = agent;
                        break;
                    }
                }
            }
            else
            {
                _logger?.LogDebug("Found preferred agent for {Name}: {AgentName}", name, retVal.Name);
            }

            if (retVal == null)
            {
                _logger?.LogWarning("No agent found for name: {Name}", name);
            }

            return retVal;
        }

        /// <summary>
        /// Returns a list of agent objects
        /// </summary>
        /// <returns>list of agent objects</returns>
        public IEnumerable<object> GetExtensions()
        {
            return _agentCache;
        }

        /// <summary>
        /// Walks the list of diretories and loads agent DLL's and
        /// populates the cache
        /// </summary>
        /// <param name="extensionDirs">List of directories to walk</param>
        /// <returns>true on success</returns>
        public bool Init(IEnumerable<string> extensionDirs)
        {
            _logger?.LogTrace("Initializing AgentsCache");

            loadCache(extensionDirs);

            _logger?.LogDebug("Loaded {Count} agents into cache before loading preferred agents", _agentCache.Count);
            _logger?.LogDebug("AgentLookupTableById has {Count} entries", _agentLookupTableById.Count);

            _preferredAgents.Load(_agentLookupTableById);

            _logger?.LogDebug("Done loading cache with {Count} total agents", _agentCache.Count);

            populateLookupTableByProcess();

            return true;
        }

        /// <summary>
        /// Removes a previously added adhoc agent
        /// </summary>
        /// <param name="handle">Handle of the window supported by the agent</param>
        /// <param name="dispose">Set this to true to dispose the agent object as well</param>
        public void RemoveAgent(IntPtr handle, bool dispose = true)
        {
            if (_adhocAgentTable.Contains(handle))
            {
                var agent = (IApplicationAgent)_adhocAgentTable[handle];
                _adhocAgentTable.Remove(handle);

                if (dispose && agent != null)
                {
                    agent.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates an instance of the specified type and adds
        /// it to the agents cache.  Raises event that an agent
        /// was added
        /// </summary>
        /// <param name="agentType"></param>
        private void addAgent(Type agentType)
        {
            try
            {
                IApplicationAgent agent = null;

                // Try to create instance using DI container first (if available)
                if (Context.ServiceProvider != null)
                {
                    try
                    {
                        agent = ActivatorUtilities.CreateInstance(Context.ServiceProvider, agentType) as IApplicationAgent;
                        _logger?.LogDebug("Adding agent {AgentType} with dependency injection", agentType.FullName);
                    }
                    catch
                    {
                        // DI creation failed, will try parameterless constructor
                    }
                }

                // Fall back to parameterless constructor
                if (agent == null)
                {
                    agent = (IApplicationAgent)Activator.CreateInstance(agentType);
                    _logger?.LogDebug("Adding agent {AgentType} (parameterless constructor)", agentType.FullName);
                }

                _agentCache.Add(agent);
                _agentLookupTableById.Add(agent.Descriptor.Id, agent);

                updateProcessLookupTable(agent);

                EvtAgentAdded?.Invoke(agent);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Could not load agent {AgentType}", agentType);
            }
        }

        /// <summary>
        /// Walks the specified directories, looks for DLL's that
        /// have IApplicationAgent classes
        /// </summary>
        /// <param name="extensionDirs"></param>
        private void loadCache(IEnumerable<string> extensionDirs)
        {
            //TODO: Fix this hack
            foreach (string dir in extensionDirs)
            {
                _logger?.LogDebug("Loading agents from directory: {Directory}", dir);
                loadAgentsFromDir(dir, "ACAT.Extensions.AppAgents.*.dll");
                loadAgentsFromDir(dir, "ACAT.Extensions.FunctionalAgents.*.dll");
            }
            _logger?.LogDebug("TypeLoader has {Count} loaded types", _agentTypeLoader.LoadedTypes.Count);
            foreach (var agent in _agentTypeLoader.LoadedTypes)
            {
                addAgent(agent.Value);
            }
        }

        /// <summary>
        /// Walks the specified directory tree
        /// </summary>
        /// <param name="path">Directory path</param>
        private void loadAgentsFromDir(string path, string filter)
        {
            // Recursively look for ACAT Agents in Extensions/Agents direrctory
            var walker = new DirectoryWalker(path, filter);
            walker.Walk(new OnFileFoundDelegate(onAgentFound));
        }

        /// <summary>
        /// Callback function for the directory walker.  Called
        /// when a dll is found
        /// </summary>
        /// <param name="dllFileName"></param>
        private void onAgentFound(string agentName)
        {
            try
            {
                _agentTypeLoader.LoadFromAssembly(agentName);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading agent from {AgentName}", agentName);
            }

        }

        /// <summary>
        /// Goes through the agentCache and populates the reverse lookup table
        /// that maps a process name to a list of agents that support it
        /// </summary>
        private void populateLookupTableByProcess()
        {
            // now create a reverse lookup table that maps a process
            // name to a list of agents that support it
            foreach (var agent in _agentCache)
            {
                updateProcessLookupTable(agent);
            }
        }

        private void updateProcessLookupTable(IApplicationAgent agent)
        {
            foreach (var processInfo in agent.ProcessesSupported)
            {
                if (!string.IsNullOrEmpty(processInfo.Name))
                {
                    List<IApplicationAgent> supportedAgents;
                    var processName = processInfo.Name.ToLower();
                    if (!_agentLookupTableByProcessName.ContainsKey(processName))
                    {
                        supportedAgents = new List<IApplicationAgent>();
                        _agentLookupTableByProcessName.Add(processName, supportedAgents);
                    }
                    else
                    {
                        supportedAgents = (List<IApplicationAgent>)_agentLookupTableByProcessName[processName];
                    }

                    bool found = false;
                    foreach (var supportedAgent in supportedAgents)
                    {
                        if (supportedAgent == agent)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        supportedAgents.Add(agent);
                    }
                }
            }
        }
    }
}