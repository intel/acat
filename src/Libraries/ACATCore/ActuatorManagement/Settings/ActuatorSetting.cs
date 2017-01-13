////////////////////////////////////////////////////////////////////////////
// <copyright file="ActuatorSetting.cs" company="Intel Corporation">
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
        /// Is the actuator enabled or not?
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// ID of the actuator
        /// </summary>
        public Guid Id;

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
        public ActuatorSetting(String name, Guid id, bool enabled = true,
            IEnumerable<SwitchSetting> switchSettings = null)
        {
            Name = name;
            Id = id;
            Enabled = enabled;
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