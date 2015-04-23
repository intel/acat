////////////////////////////////////////////////////////////////////////////
// <copyright file="WinsockCommon.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2015 Intel Corporation 
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
using System.Diagnostics.CodeAnalysis;
using ACAT.Lib.Core.ActuatorManagement;
using ACAT.Lib.Core.Utility;

#region SupressStyleCopWarnings

[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1126:PrefixCallsCorrectly",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1101:PrefixLocalCallsWithThis",
        Scope = "namespace",
        Justification = "Not needed. ACAT naming conventions takes care of this")]
[module: SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1121:UseBuiltInTypeAlias",
        Scope = "namespace",
        Justification = "Since they are just aliases, it doesn't really matter")]
[module: SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1200:UsingDirectivesMustBePlacedWithinNamespace",
        Scope = "namespace",
        Justification = "ACAT guidelines")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1309:FieldNamesMustNotBeginWithUnderscore",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private fields begin with an underscore")]
[module: SuppressMessage(
        "StyleCop.CSharp.NamingRules",
        "SA1300:ElementMustBeginWithUpperCaseLetter",
        Scope = "namespace",
        Justification = "ACAT guidelines. Private/Protected methods begin with lowercase")]

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.InputActuators
{
    /// <summary>
    /// A helper class for winsock based actuators.  Has methods to the parse
    /// data
    /// </summary>
    internal class WinsockCommon
    {
        public delegate IActuatorSwitch CreateSwitchDelegate(IActuatorSwitch s);

        /// <summary>
        /// Parses the action string and returns the corresponding
        /// enum
        /// </summary>
        /// <param name="action">String representation of gesture event</param>
        /// <returns>SwitchAction enum</returns>
        public static SwitchAction getSwitchAction(String action)
        {
            var retVal = SwitchAction.Unknown;
            try
            {
                retVal = (SwitchAction)Enum.Parse(typeof(SwitchAction), action, true);
            }
            catch (Exception e)
            {
                Log.Warn("Image switch, invalid action specified " + action);
                Log.Exception(e);
            }

            return retVal;
        }

        /// <summary>
        /// Parses the string sent over the tcp/ip connection and
        /// extracts information from it.  Then looks up
        /// the list of switches, matches the gesture with the
        /// switch and creates a clone of the switch object and
        /// returns the switch object
        /// Format of the packet is:
        ///    gesture=gesturetype;action=gestureevent;conf=confidence;time=timestamp;actuate=flag;tag=userdata
        /// where
        ///  gesturetype    is a string representing the gesture. This is used as
        ///                 the 'source' field in the actuator switch object
        ///  gestureevent   should be a valid value from the SwitchAction enum
        ///  confidence     Integer representing the confidence level, for future use
        ///  timestamp      Timestamp of when the switch event triggered (in ticks)
        ///  flag           true/false.  If false, the switch trigger event will be ignored
        ///  userdata       Any user data
        /// Eg
        ///    gesture=G1;action=trigger;conf=75;time=3244394443
        /// </summary>
        /// <param name="strData">Data</param>
        /// <param name="switches">Actuator switch collection</param>
        /// <param name="createSwitchDel">A delegate function that creates an actuator switch object</param>
        /// <returns>A clone of the matching switch object from the collection</returns>
        public static IActuatorSwitch parseAndGetSwitch(
                                            String strData, 
                                            ICollection<IActuatorSwitch> switches, 
                                            CreateSwitchDelegate createSwitchDel)
        {
            IActuatorSwitch actuatorSwitch = null;
            String gesture = String.Empty;
            var switchAction = SwitchAction.Unknown;
            String tag = String.Empty;
            int confidence = -1;
            long time = -1;
            bool actuate = true;

            var tokens = strData.Split(';');
            foreach (var token in tokens)
            {
                String[] nameValue = token.Split('=');
                if (nameValue.Length == 2)
                {
                    switch (nameValue[0])
                    {
                        case "gesture":  // G1, G2, ...
                            gesture = nameValue[1];
                            break;

                        case "action": // Up, Down, Trigger
                            switchAction = getSwitchAction(nameValue[1]);
                            break;

                        case "time":  // in Ticks
                            time = parseLong(nameValue[1]);
                            break;

                        case "confidence":  // integer 0 to 100
                            confidence = (int)parseLong(nameValue[1]);
                            break;

                        case "actuate":
                            actuate = String.Compare(nameValue[1], "true", true) == 0;
                            break;

                        case "tag":
                            tag = nameValue[1];
                            break;
                    }
                }
            }

            if (!String.IsNullOrEmpty(gesture) && switchAction != SwitchAction.Unknown)
            {
                actuatorSwitch = getSwitchForGesture(gesture, switches, createSwitchDel);
                actuatorSwitch.Action = switchAction;
                actuatorSwitch.Confidence = confidence;
                actuatorSwitch.Timestamp = time;
                actuatorSwitch.Actuate = actuate;
                actuatorSwitch.Tag = tag;
            }

            return actuatorSwitch;
        }

        /// <summary>
        /// Parses a long (as string) and returns value
        /// </summary>
        /// <param name="val">string representation</param>
        /// <returns>long value</returns>
        public static long parseLong(String val)
        {
            long retVal = -1;
            try
            {
                retVal = Convert.ToInt32(val);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return retVal;
        }

        /// <summary>
        /// Looks up the list of switches for a matching gesture and returns
        /// a clone of the matching switch object
        /// </summary>
        /// <param name="gesture">The gesture string</param>
        /// <param name="switches">Collection of switches for the actuator</param>
        /// <param name="createSwitchDel">Delegate that creates a clone of the switch</param>
        /// <returns></returns>
        private static IActuatorSwitch getSwitchForGesture(
                                            String gesture, 
                                            IEnumerable<IActuatorSwitch> switches, 
                                            CreateSwitchDelegate createSwitchDel)
        {
            foreach (var switchObj in switches)
            {
                var imageSwitch = switchObj;
                if (String.Compare(imageSwitch.Source, gesture, true) == 0)
                {
                    Log.Debug("Found switch object " + switchObj.Name + " for gesture" + gesture);
                    return createSwitchDel(switchObj);
                }
            }

            return null;
        }
    }
}