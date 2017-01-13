////////////////////////////////////////////////////////////////////////////
// <copyright file="CmdScopeTable.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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