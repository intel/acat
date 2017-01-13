////////////////////////////////////////////////////////////////////////////
// <copyright file="SwitchSetting.cs" company="Intel Corporation">
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


using ACAT.Lib.Core.CommandManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Represents the settings for a single switch of an
    /// actuator
    /// </summary>
    [Serializable]
    public class SwitchSetting
    {
        /// <summary>
        /// The command verb to be used for a switch that
        /// is used for selection
        /// </summary>
        public const String TriggerCommand = "@Trigger";

        /// <summary>
        /// Should the switch be actuated?  if set to 
        /// false, it will not trigger, but log an audit
        /// log event.
        /// </summary>
        public bool Actuate;

        /// <summary>
        /// Optional file that contains the beep sound that
        /// will be played when the switch is activated
        /// </summary>
        public String BeepFile;

        /// <summary>
        /// Command mapped to the switch
        /// </summary>
        public String Command;

        /// <summary>
        /// A user-friendly description of the switch
        /// </summary>
        public String Description;

        /// <summary>
        /// Is the switch enabled?
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// Minimum hold time for the switch.  If the switch
        /// is held for a perdiod less than this, the switch
        /// activation (trigger) is ignored.
        /// </summary>
        public String MinHoldTime;

        /// <summary>
        /// Name of the switch
        /// </summary>
        public String Name;

        /// <summary>
        /// Switch source
        /// </summary>
        public String Source;

        /// <summary>
        /// Initializes an instance fo the class
        /// </summary>
        public SwitchSetting()
        {
            Name = String.Empty;
            Source = String.Empty;
            Description = String.Empty;
            Enabled = false;
            BeepFile = String.Empty;
            Actuate = false;
            Command = String.Empty;
        }

        /// <summary>
        /// Initializes an instance fo the class
        /// </summary>
        /// <param name="name">Name of the switch</param>
        /// <param name="description">Description</param>
        /// <param name="commands">commands associated with the switch</param>
        /// <param name="minHoldTIme">minimum hold time</param>
        /// <param name="enabled">is it enabled or not</param>
        /// <param name="actuate">whether to actuate or not</param>
        public SwitchSetting(String name, 
                            String description, 
                            String command, 
                            String minHoldTIme = "@MinActuationHoldTime", 
                            bool enabled = true, 
            bool actuate = true)
        {
            Name = name;
            Description = description;
            Source = name;
            Enabled = enabled;
            BeepFile = String.Empty;
            MinHoldTime = minHoldTIme;
            Actuate = actuate;
            Command = command;
        }

        /// <summary>
        /// Configure this switch to act as a selector when
        /// triggered. When the switch is triggered, the item
        /// that is currently highlighted in the scanner is activated
        /// </summary>
        /// <param name="onOff">to trigger or not</param>
        public void ConfigureAsTriggerSwitch(bool onOff)
        {
            if (onOff)
            {
                if (!IsTriggerSwitch())
                {
                    Command = TriggerCommand;
                }
            }
            else if (IsTriggerSwitch())
            {
                Command = String.Empty;
            }
        }

        /// <summary>
        /// Returns true if the switch is configured as a 
        /// trigger
        /// </summary>
        /// <returns>true if it is</returns>
        public bool IsTriggerSwitch()
        {
            return Command == TriggerCommand;
        }
    }
}
