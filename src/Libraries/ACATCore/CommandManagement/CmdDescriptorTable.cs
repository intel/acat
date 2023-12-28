////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ACAT.Lib.Core.CommandManagement
{
    /// <summary>
    /// Maintains a list of CommandDescriptor objects.
    /// </summary>
    [Serializable]
    public class CmdDescriptorTable : PreferencesBase
    {
        /// <summary>
        /// Name of the file to save/load from
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String FilePath;

        private readonly List<CmdDescriptor> _cmdDescriptors = new List<CmdDescriptor>();

        /// <summary>
        /// List of command descriptors
        /// </summary>
        public IEnumerable<CmdDescriptor> CmdDescriptors
        {
            get
            {
                return _cmdDescriptors;
            }
        }

        /// <summary>
        /// Loads the list from the file
        /// </summary>
        /// <returns>settings object</returns>
        public static CmdDescriptorTable Load()
        {
            return PreferencesBase.Load<CmdDescriptorTable>(FilePath);
        }

        /// <summary>
        /// Add the specified object to the list
        /// </summary>
        /// <param name="cmdDescriptor">Object to add</param>
        public void Add(CmdDescriptor cmdDescriptor)
        {
            _cmdDescriptors.Add(cmdDescriptor);
        }

        /// <summary>
        /// Returns the CommandDescriptor entry for the
        /// specified command, null if not found
        /// </summary>
        /// <param name="command">The command to look for</param>
        /// <returns></returns>
        public CmdDescriptor Get(string command)
        {
            return CmdDescriptors.FirstOrDefault(cmdDescriptor => String.Compare(command, cmdDescriptor.Command, true) == 0);
        }

        /// <summary>
        /// Returns a list of command descriptors for which "EnableSwitchMap" is
        /// equal to the 'enabled' parameter.
        /// </summary>
        /// <param name="enabled">Whether enabled or not</param>
        /// <returns>list of command descriptors </returns>
        public IEnumerable<CmdDescriptor> GetEnableSwitchMapCommands(bool enabled)
        {
            var cmdDescriptorList = new List<CmdDescriptor>();

            foreach (var cmdDescriptor in _cmdDescriptors)
            {
                if (cmdDescriptor.EnableSwitchMap == enabled)
                {
                    cmdDescriptorList.Add(cmdDescriptor);
                }
            }

            return cmdDescriptorList;
        }

        /// <summary>
        /// Saves the list to the file
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return (!String.IsNullOrEmpty(FilePath)) && Save(this, FilePath);
        }

        /// <summary>
        /// Sets the "EnableSwitchMap" flag for the specified
        /// commands
        /// </summary>
        /// <param name="commands">Array of commands</param>
        /// <param name="enable">value of EnableSwitchMap</param>
        public void SetEnableSwitchMap(String[] commands, bool enable)
        {
            foreach (var command in commands)
            {
                var cmdDescriptor = Get(command);
                if (cmdDescriptor != null)
                {
                    cmdDescriptor.EnableSwitchMap = enable;
                }
            }
        }
    }
}