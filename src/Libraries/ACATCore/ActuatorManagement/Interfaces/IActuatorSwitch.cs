////////////////////////////////////////////////////////////////////////////
// <copyright file="IActuatorSwitch.cs" company="Intel Corporation">
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Media;
using System.Xml;

#region SupressStyleCopWarnings

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

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Delegate for the event raised when the accept time for the switch elapses
    /// </summary>
    /// <param name="sender">sender of the event</param>
    /// <param name="e">event args</param>
    public delegate void AcceptTimeExpired(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Action of the switch
    /// </summary>
    public enum SwitchAction
    {
        Unknown,
        Down,
        Up,
        Trigger
    }

    /// <summary>
    /// Interface for an actuator switch.  All switches must derive from this interface.
    /// </summary>
    public interface IActuatorSwitch : IDisposable
    {
        /// <summary>
        /// Gets or sets the length of time the switch to stay engaged for it to be
        /// recognized as a valid trigger.  If the switch stays engaged for less than
        /// the AcceptTime, it is ignored.
        /// </summary>
        int AcceptTime { get; set; }

        /// <summary>
        /// Gets the timer that tracks the accept time for the switch
        /// </summary>
        Stopwatch AcceptTimer { get; }

        /// <summary>
        /// Gets or sets the switch action
        /// </summary>
        SwitchAction Action { get; set; }

        /// <summary>
        /// Gets or sets property that controls whether the switch should be
        /// actuated or not.
        /// </summary>
        bool Actuate { get; set; }

        /// <summary>
        /// Gets or sets the parent actuator that contains the switch object
        /// </summary>
        IActuator Actuator { get; set; }

        /// <summary>
        /// Gets or sets the audio player to play the beep
        /// </summary>
        SoundPlayer Audio { get; }

        /// <summary>
        /// Gets or sets the name of the WAV file containing the beep associated
        /// with the switch. Beep is sounded when the switch is trigerred
        /// </summary>
        String BeepFile { get; set; }

        /// <summary>
        /// Gets or sets the confidence level of activation.  For future use.
        /// </summary>
        int Confidence { get; set; }

        /// <summary>
        /// Gets the active state of the switch.  True if active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Name of the switch
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Gets or sets the source of the switch activation.Depends on the type of
        /// the switch.  For instance, for a keyboard switch, source
        /// would be "F5" for the F5 function key.
        /// </summary>
        String Source { get; set; }

        /// <summary>
        /// Gets or sets auxilliary data.  Opaque, can be set by the application.
        /// </summary>
        String Tag { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of switch event
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// Initialize the actuator switch
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();

        /// <summary>
        /// XML node that contains attributes for the switch
        /// </summary>
        /// <param name="xmlNode">The XML node that contains the Switch attributes</param>
        /// <returns>True on successful parse, false otherwise</returns>
        bool Load(XmlNode xmlNode);

        /// <summary>
        /// Records the fact that a switch down was detected.  Call this when the
        /// switch is engaged
        /// </summary>
        void RegisterSwitchDown();

        /// <summary>
        /// Records the fact the switch up was detected. Call this when the switch
        ///  is disengaged
        /// </summary>
        void RegisterSwitchUp();
    }
}