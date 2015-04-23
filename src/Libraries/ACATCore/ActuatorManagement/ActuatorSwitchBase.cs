////////////////////////////////////////////////////////////////////////////
// <copyright file="ActuatorSwitchBase.cs" company="Intel Corporation">
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
    /// Represents the attributes of a switch object. Note that
    /// the acutal action of the switch is handled in the Actuators.
    /// The Switch objects just enacapusate the attributes.
    /// This is the base class for all the actuator switches.
    /// </summary>
    public class ActuatorSwitchBase : IActuatorSwitch
    {
        /// <summary>
        /// Used for synchronization
        /// </summary>
        private readonly Object _lockObj = new object();

        /// <summary>
        /// Timer to track switch trigger timings
        /// </summary>
        private TimerQueue _timer;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Play beep when switch is triggered
        /// </summary>
        private SoundPlayer _audio;

        /// <summary>
        /// Timer to track the accept time.  If the switch
        /// stays engaged for less then AcceptTime, the 
        /// switch is ignored.
        /// </summary>
        private Stopwatch _acceptTimer;

        /// <summary>
        /// Is this enabled or not
        /// </summary>
        private Boolean _isActive;

        /// <summary>
        /// Initializes the Switch object
        /// </summary>
        public ActuatorSwitchBase()
        {
            _isActive = false;
            Name = String.Empty;
            Source = String.Empty;
            _acceptTimer = new Stopwatch();
            Actuate = true;
        }

        /// <summary>
        /// Initializes the switch object from the passed switch object
        /// </summary>
        /// <param name="switchObj">Source switch object</param>
        public ActuatorSwitchBase(IActuatorSwitch switchObj)
        {
            Name = switchObj.Name;
            Source = switchObj.Source;
            _isActive = switchObj.IsActive;
            AcceptTime = switchObj.AcceptTime;
            Confidence = switchObj.Confidence;
            Timestamp = switchObj.Timestamp;
            Action = switchObj.Action;
            BeepFile = switchObj.BeepFile;
            Audio = switchObj.Audio;
            Actuator = switchObj.Actuator;
            _acceptTimer = switchObj.AcceptTimer;
            Actuate = switchObj.Actuate;
            Tag = switchObj.Tag;
        }

        public Stopwatch AcceptTimer
        {
            get
            {
                return _acceptTimer;
            }
        }

        /// <summary>
        /// Gets or sets the name of the switch
        /// </summary>
        public String Name 
        { 
            get; 
            internal set; 
        }

        /// <summary>
        /// Gets or sets the source of the switch tht caused the trigger.  
        /// Depends on the type of the switch.  For instance, for 
        /// a keyboard switch, source would be "F5" for the F5 function key.
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// Whether to actuate the switch or not
        /// </summary>
        public bool Actuate { get; set; }

        /// <summary>
        /// Gets or sets the length of time (msecs) the switch to stay engaged for it to be 
        /// recognized as a trigger. 
        /// </summary>
        public int AcceptTime { get; set; }

        /// <summary>
        /// Gets or sets the Switch action
        /// </summary>
        public SwitchAction Action { get; set; }

        /// <summary>
        /// Gets or sets the time when the switch was triggered.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the confidence level (unused now)
        /// </summary>
        public int Confidence { get; set; }

        /// <summary>
        /// Gets or sets name of the beep file associated wit this switch
        /// </summary>
        public String BeepFile { get; set; }

        /// <summary>
        /// Gets the audio player to play the beep
        /// </summary>
        public SoundPlayer Audio
        {
            get 
            { 
                return _audio; 
            }
           
            private set 
            { 
                _audio = value; 
            }
        }

        /// <summary>
        /// Gets or sets the active state of the switch.
        /// </summary>
        public Boolean IsActive
        {
            get
            {
                lock (_lockObj)
                {
                    return _isActive;
                }
            }

            private set
            {
                lock (_lockObj)
                {
                    _isActive = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent Actuator of this switch
        /// </summary>
        public IActuator Actuator { get; set; }

        /// <summary>
        /// Gets or sets opaque tag
        /// </summary>
        public String Tag { get; set; }

        /// <summary>
        /// Reads switch attributes from the XML file. 
        /// </summary>
        /// <param name="xmlNode">The XML node that contains the Switch attributes</param>
        /// <returns>True on successful parse, false otherwise</returns>
        public virtual bool Load(XmlNode xmlNode)
        {
            // name of the actuator
            Name = XmlUtils.GetXMLAttrString(xmlNode, "name");

            // read and set the remaining attributes
            bool enabled = XmlUtils.GetXMLAttrBool(xmlNode, "enabled", true);
            if (!enabled)
            {
                return false;
            }

            Actuate = XmlUtils.GetXMLAttrBool(xmlNode, "actuate", true);

            Source = XmlUtils.GetXMLAttrString(xmlNode, "source");

            String value = XmlUtils.GetXMLAttrString(xmlNode, "acceptTime");
            AcceptTime = CoreGlobals.AppPreferences.ResolveVariableInt(value, CoreGlobals.AppPreferences.AcceptTime, 0);

            BeepFile = XmlUtils.GetXMLAttrString(xmlNode, "beepFile");
            if (!String.IsNullOrEmpty(BeepFile))
            {
                _audio = new SoundPlayer(FileUtils.GetSoundPath(BeepFile));
            }

            return true;
        }

        /// <summary>
        /// Override this to do init for derived classes
        /// </summary>
        /// <returns>true on success</returns>
        public virtual bool Init()
        {
            return true;
        }

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
        /// Records the fact that a switch down was detected.  Starts
        /// a timer to track the AcceptTime
        /// </summary>
        public void RegisterSwitchDown()
        {
            IsActive = true;
        }

        /// <summary>
        /// Records the fact the swtich up was dected. Stos the
        /// accept timer
        /// </summary>
        public void RegisterSwitchUp()
        {
            IsActive = false;
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
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    if (_acceptTimer != null)
                    {
                        _acceptTimer.Stop();
                        _acceptTimer = null;
                    }
                    
                    if (_audio != null)
                    {
                        _audio.Dispose();
                        _audio = null;
                    }
                }

                // Release unmanaged resources. 
            }

            _disposed = true;
        }
    }
}
