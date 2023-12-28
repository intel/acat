////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ACAT.Lib.Core.AgentManagement
{
    /// <summary>
    /// Represents a list of preferred IApplicationAgent to use
    /// for a process.  The information comes from reading and parsing
    /// the PreferredAgents.xml located in the user directory.  The
    /// reason for this is there could be a conflict where a processes
    /// such as notepad could have multiple agents which are loaded
    /// from different folders. This config file tells ACAT which of
    /// those to use.
    ///
    /// Eg of PreferredAgents.xml file
    /// <ACAT>
    ///   <PreferredAgents>
    ///     <PreferredAgent agentId="EC2EA972-934B-4EE0-A909-3EA0140AC738"/>
    ///     <PreferredAgent agentId="E9B930AD-CB35-478C-BDA6-D7FC43349019"/>
    ///   </PreferredAgents>
    /// </ACAT>

    /// </summary>
    internal class PreferredAgents : IDisposable
    {
        /// <summary>
        /// Name of the preferences file
        /// </summary>
        private const String PreferredAgentsFileName = "PreferredAgents.xml";

        /// <summary>
        /// Table of preferred agents
        /// </summary>
        private readonly Hashtable _preferredAgents;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public PreferredAgents()
        {
            Log.Debug();
            _preferredAgents = new Hashtable();
        }

        /// <summary>
        /// Disposer
        /// </summary>
        public void Dispose()
        {
            _preferredAgents.Clear();
        }

        /// <summary>
        /// Looks up the list of preferred list and returns the
        /// agent corresponding to the category specified
        /// </summary>
        /// <param agentName>Category of the agent</param>
        /// <returns></returns>
        public IApplicationAgent GetPreferredAgentByCategory(String category)
        {
            foreach (IApplicationAgent agent in _preferredAgents.Values)
            {
                if (String.Compare(category, agent.Descriptor.Category, true) == 0)
                {
                    return agent;
                }
            }

            return null;
        }

        /// <summary>
        /// Looks up the list of preferred list and returns the
        /// agent corresponding to the name specified
        /// </summary>
        /// <param agentName>Name of the agent</param>
        /// <returns></returns>
        public IApplicationAgent GetPreferredAgentByName(String agentName)
        {
            foreach (IApplicationAgent agent in _preferredAgents.Values)
            {
                if (String.Compare(agentName, agent.Name, true) == 0)
                {
                    return agent;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the preferred agent for the specified process name
        /// </summary>
        /// <param name="processName">Name of the process</param>
        /// <returns>Agent object</returns>
        public IApplicationAgent GetPreferredAgentForProcess(String processName)
        {
            foreach (IApplicationAgent agent in _preferredAgents.Values)
            {
                foreach (var agentProcessInfo in agent.ProcessesSupported)
                {
                    if (String.Compare(processName, agentProcessInfo.Name, true) == 0)
                    {
                        return agent;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the preferred agent for the specified process
        /// </summary>
        /// <param name="process">Process object</param>
        /// <returns>Preferred agent object</returns>
        public IApplicationAgent GetPreferredAgentForProcess(Process process)
        {
            IApplicationAgent nullPathAgent = null;
            foreach (IApplicationAgent agent in _preferredAgents.Values)
            {
                foreach (var processInfo in agent.ProcessesSupported)
                {
                    if (String.Compare(process.ProcessName, processInfo.Name, true) == 0)
                    {
                        if (String.IsNullOrEmpty(processInfo.ExecutablePath))
                        {
                            nullPathAgent = agent;
                        }
                        else if (String.Compare(process.MainModule.FileName, processInfo.ExecutablePath, true) == 0)
                        {
                            return agent;
                        }
                    }
                }
            }

            return nullPathAgent;
        }

        /// <summary>
        /// Parses the PreferredAgents.xml file and loads the list of
        /// preferred agents specified in the file.  Populates the hashtable
        /// </summary>
        /// <param name="agentsTable">The table to populate</param>
        public void Load(Hashtable agentsTable)
        {
            String file = UserManagement.UserManager.GetFullPath(PreferredAgentsFileName);

            if (!File.Exists(file))
            {
                return;
            }

            try
            {
                var doc = new XmlDocument();

                doc.Load(file);

                XmlNodeList configNodes = doc.SelectNodes("/ACAT/PreferredAgents/PreferredAgent");

                if (configNodes == null)
                {
                    return;
                }

                // load each scheme from the config file
                foreach (XmlNode node in configNodes)
                {
                    var strGuid = XmlUtils.GetXMLAttrString(node, "agentId").Trim().ToLower();
                    Guid guid;
                    if (Guid.TryParse(strGuid, out guid))
                    {
                        if (agentsTable.ContainsKey(guid))
                        {
                            _preferredAgents.Add(guid, agentsTable[guid]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }
        }
    }
}