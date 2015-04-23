////////////////////////////////////////////////////////////////////////////
// <copyright file="IActuator.cs" company="Intel Corporation">
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
using System.Xml;
using ACAT.Lib.Core.Utility;

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
#endregion

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Delegate for the event raised when a switch is engaged. This is the
    /// start of the switch activation event.  For instance, for a keyboard switch,
    /// this is equivalent to a keydown
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event argument</param>
    public delegate void SwitchActivated(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Delegate for the event raised when a switch is disengaged.  Ths is the end of 
    /// the switch activation event.  For instance, for a keyboard switch, this is 
    /// equivalent to a keyup
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event argument</param>
    public delegate void SwitchDeactivated(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Delegate for the event raised when a switch is triggered - engaged and disengaged.
    /// For instance, for a keyboard switch, this is equivalent to a KeyTriggered
    /// </summary>
    /// <param name="sender">sender object</param>
    /// <param name="e">event arguement</param>
    public delegate void SwitchTriggered(object sender, ActuatorSwitchEventArgs e);

    /// <summary>
    /// Represents the state of the actuator
    /// </summary>
    public enum State
    {
        Paused,
        Running,
        Stopped
    }

    /// <summary>
    /// Actutators must implement this interface.  An actuator contains one or more switches and
    /// raises events when the switches are actuated.
    /// </summary>
    public interface IActuator : IDisposable
    {
        /// <summary>
        /// Raised when one of the switches in this actuator is engaged
        /// </summary>
        event SwitchActivated EvtSwitchActivated;

        /// <summary>
        /// Raised when one of the switches in this actuator is disengaged
        /// </summary>
        event SwitchDeactivated EvtSwitchDeactivated;

        /// <summary>
        /// Raised when one of the switches in this actuator is triggered
        /// </summary>
        event SwitchTriggered EvtSwitchTriggered;
        
        /// <summary>
        /// Gets the ACAT descriptor
        /// </summary>
        IDescriptor Descriptor { get; }

        /// <summary>
        /// Indicates whether the acutator is enabled or not
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the actuator
        /// </summary>
        String Name { get; set; }
        /// <summary>
        /// Gets a collection of switches that are a part of this actuator
        /// </summary>
        ICollection<IActuatorSwitch> Switches { get; }

        /// <summary>
        /// Creates an actuator switch object
        /// </summary>
        /// <returns>Switch object</returns>
        IActuatorSwitch CreateSwitch();

        /// <summary>
        /// Initializes the actuator
        /// </summary>
        /// <returns>true on success</returns>
        bool Init();

        /// <summary>
        /// Parses the XML node that contains all the info for this actuator
        /// </summary>
        /// <param name="actuatorNode">The xml fragment for the actuator</param>
        /// <returns>true on success</returns>
        bool Load(XmlNode actuatorNode);
        /// <summary>
        /// Pauses the actuator.  No events will be raised from the acutator
        /// when paused
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes actuator.  Will resume raising events
        /// </summary>
        void Resume();
    }
}
