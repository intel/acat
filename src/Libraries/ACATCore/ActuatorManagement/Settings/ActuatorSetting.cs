////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ActuatorSettings.cs
//
// Holds settings for an actuator and for all the switches that belong to
// the actuator
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Holds settings for an actuator and for all the
    /// switches that belong to the actuator
    /// </summary>
    [Serializable]
    public class ActuatorSetting
    {
        /// <summary>
        /// Description for the actuator
        /// </summary>
        public String Description;

        /// <summary>
        /// Is the actuator enabled or not?
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// ID of the actuator
        /// </summary>
        public Guid Id;

        public String ImageFileName;

        /// <summary>
        /// Name of the actuator
        /// </summary>
        public String Name;

        /// <summary>
        /// List of settings for the switches that belong to
        /// the actuator
        /// </summary>
        public List<SwitchSetting> SwitchSettings = new List<SwitchSetting>();

        /// <summary>
        /// Initializes an instance of the class
        /// </summary>
        public ActuatorSetting()
        {
            Name = String.Empty;
            Id = Guid.Empty;
            Enabled = false;
        }

        /// <summary>
        /// Initializes an instance of the class for an actuator
        ///
        /// </summary>
        /// <param name="name">name of the actuator</param>
        /// <param name="id">actuator id</param>
        /// <param name="enabled">is the actuator enabled?</param>
        /// <param name="switchSettings">settings for switches</param>
        public ActuatorSetting(String name, Guid id, String description = "", String imageFileName = "", bool enabled = true,
            IEnumerable<SwitchSetting> switchSettings = null)
        {
            Name = name;
            Id = id;
            Enabled = enabled;
            Description = description;
            ImageFileName = imageFileName;
            SwitchSettings = (switchSettings != null) ? switchSettings.ToList() : new List<SwitchSetting>();
        }

        /// <summary>
        /// Finds the settings for the switch indicated by
        /// switchName
        /// </summary>
        /// <param name="switchName">Name of the switch</param>
        /// <returns>Switch settings object, null if not found</returns>
        public SwitchSetting Find(String switchName)
        {
            return SwitchSettings.FirstOrDefault(switchSetting => String.Compare(switchSetting.Name, switchName, true) == 0);
        }
    }
}