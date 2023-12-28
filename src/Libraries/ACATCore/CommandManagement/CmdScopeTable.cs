////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ACAT.Lib.Core.CommandManagement
{
    /// <summary>
    /// Represents a presistent collection of commands
    /// and thier respective scopes
    /// </summary>
    [Serializable]
    public class CmdScopeTable : PreferencesBase
    {
        /// <summary>
        /// Name of the file for presistence
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String FilePath;

        /// <summary>
        /// List of commands and their scopes
        /// </summary>
        public List<CmdScopeMapEntry> CmdScopeMapEntries;

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public CmdScopeTable()
        {
            CmdScopeMapEntries = new List<CmdScopeMapEntry>();
        }

        /// <summary>
        /// Loads table from the FilePath file
        /// </summary>
        /// <returns>the object</returns>
        public static CmdScopeTable Load()
        {
            return PreferencesBase.Load<CmdScopeTable>(FilePath);
        }

        /// <summary>
        /// Adds the specified object to the list
        /// </summary>
        /// <param name="cmdScopeMapEntry">object to add</param>
        public void Add(CmdScopeMapEntry cmdScopeMapEntry)
        {
            CmdScopeMapEntries.Add(cmdScopeMapEntry);
        }

        /// <summary>
        /// Save the table to FilePath
        /// </summary>
        /// <returns>true if successful</returns>
        public override bool Save()
        {
            return Save(this, FilePath);
        }
    }
}