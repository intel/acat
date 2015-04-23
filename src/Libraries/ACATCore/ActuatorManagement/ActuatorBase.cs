////////////////////////////////////////////////////////////////////////////
// <copyright file="ActuatorBase.cs" company="Intel Corporation">
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
#endregion

namespace ACAT.Lib.Core.ActuatorManagement
{
    /// <summary>
    /// Base class for all the actuators.  Actuators are input mechanisms
    /// to the application.  An actuator contains a list 
    /// of switches, each of which act as a trigger to drive the UI. For
    /// instance, a keyboard actuator will use input from the keyboard as 
    /// triggers.  Soft actuators can also be implemented that use sockets
    /// to send triggers to the UI.
    /// </summary>
    public abstract class ActuatorBase : IActuator
    {
        /// <summary>
        /// A list of switches defined for this actuator.  Each switch has a 
        /// name that is unique to the actuator
        /// </summary>
        private readonly Dictionary<String, IActuatorSwitch> _switches;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;
        
        /// <summary>
        /// Initializes a new instance of the ActuatorBase class
        /// </summary>
        protected ActuatorBase()
        {
            Enabled = false;
            _switches = new Dictionary<String, IActuatorSwitch>();
            actuatorState = State.Stopped;
        }

        /// <summary>
        /// Triggered when one of the switches in this actuator is engaged.
        /// </summary>
        public event SwitchActivated EvtSwitchActivated;

        /// <summary>
        /// Triggered when one of the switches in this actuator is disengaged
        /// </summary>
        public event SwitchDeactivated EvtSwitchDeactivated;

        /// <summary>
        /// Raised when one of the switches in this actuator is triggered (engaged
        /// followed by a disengaged)
        /// </summary>
        public event SwitchTriggered EvtSwitchTriggered;

        /// <summary>
        /// Gets the descriptor for the actuator class
        /// </summary>
        public virtual IDescriptor Descriptor
        {
            get { return DescriptorAttribute.GetDescriptor(GetType()); }
        }

        /// <summary>
        /// Gets or sets the name of the actuator
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the actuator is enabled or not.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets the list of switches that are a part of this actuator
        /// </summary>
        public ICollection<IActuatorSwitch> Switches
        {
            get { return _switches.Values; }
        }

        /// <summary>
        /// Gets or sets the current state of the actuator
        /// </summary>
        protected State actuatorState { get; set; }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Class factory to create a switch.  Override this in the 
        /// derived classes to enable creating switches that are specific to the
        /// actuator
        /// </summary>
        /// <returns>Switch object</returns>
        public abstract IActuatorSwitch CreateSwitch();

        /// <summary>
        /// Parses the XML node that contains all the info for this actuator
        /// </summary>
        /// <param name="actuatorNode">The xml fragment for the actuator</param>
        /// <returns>true on success, false otherwise</returns>
        public bool Load(XmlNode actuatorNode)
        {
            // enumerate the switches in this actuator and create
            // each switch object using the switch ClassFactory
            var switches = actuatorNode.SelectNodes("Switch");
            if (switches == null)
            {
                return false;
            }

            Log.Debug("Loading switches");
            foreach (XmlNode node in switches)
            {
                var name = XmlUtils.GetXMLAttrString(node, "name");
                if (String.IsNullOrEmpty(name))
                {
                    continue;
                }

                var actuatorSwitch = CreateSwitch();
                Log.Debug("name=" + name);
                if (!_switches.ContainsKey(name))
                {
                    if (actuatorSwitch.Load(node) && actuatorSwitch.Init())
                    {
                        Log.Debug("Adding switch " + actuatorSwitch.Name);
                        actuatorSwitch.Actuator = this;
                        _switches.Add(actuatorSwitch.Name, actuatorSwitch);
                    }
                }
                else
                {
                    Log.Error("Warning.  Switch " + actuatorSwitch.Name + " defined more than once");
                }
            }

            return true;
        }

        /// <summary>
        /// Allow derived classes to allocate resources
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Init()
        {
            return true;
        }

        /// <summary>
        /// Pauses actuator.  No events will be received from the actuator
        /// when paused
        /// </summary>
        public virtual void Pause()
        {
        }

        /// <summary>
        /// Resumes actuator.  Will start sending events
        /// </summary>
        public virtual void Resume()
        {
        }

        /// <summary>
        /// Helper function to trigger an event that a switch was engaged.  This
        /// function is called by the derived classes
        /// </summary>
        /// <param name="switchObj">Switch that caused the event</param>
        protected virtual void OnSwitchActivated(IActuatorSwitch switchObj)
        {
            switchObj.Action = SwitchAction.Down;
            EvtSwitchActivated(this, new ActuatorSwitchEventArgs(switchObj));
        }

        /// <summary>
        /// Helper function to trigger an event that a switch was disengaged.  This
        /// function is called by the derived classes
        /// </summary>
        /// <param name="switchObj">Switch that caused the event</param>
        protected virtual void OnSwitchDeactivated(IActuatorSwitch switchObj)
        {
            switchObj.Action = SwitchAction.Up;
            EvtSwitchDeactivated(this, new ActuatorSwitchEventArgs(switchObj));
        }

        /// <summary>
        /// Helper function to trigger an event that a switch was triggered.  This
        /// function is called by the derived classes
        /// </summary>
        /// <param name="switchObj">Switch that caused the event</param>
        protected virtual void OnSwitchTriggered(IActuatorSwitch switchObj)
        {
            switchObj.Action = SwitchAction.Trigger;
            EvtSwitchTriggered(this, new ActuatorSwitchEventArgs(switchObj));
        }

        /// <summary>
        /// Disposer. Release resources and cleanup.
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                Log.Debug();

                if (disposing)
                {
                    // dispose all managed resources.
                }

                // Release unmanaged resources. 
            }

            _disposed = true;
        }
    }
}
