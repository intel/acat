////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.PreferencesManagement;
using System;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ACAT.Core.Utility
{
    /// <summary>*
    /// Contains system-wide preference settings.  Settings are serialized
    /// into a file for saving and loaded from the file.
    /// Be careful about renaming variables in this as the variable names
    /// are used as the setting names in the saved XML file.
    /// </summary>
    public class Preferences : PreferencesBase
    {
        [NonSerialized, XmlIgnore]
        public static Assembly ApplicationAssembly;

        [NonSerialized, XmlIgnore]
        public String AppName = "ACAT";

        /// <summary>
        /// Returns a string representation of the settings
        /// </summary>
        public override String toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Preferences: ");
            sb.Append(XmlUtils.XmlSerializeToString(this));
            return sb.ToString();
        }
    }
}