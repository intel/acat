////////////////////////////////////////////////////////////////////////////
// <copyright file="KeyboardSwitch.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Xml;
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
    /// Represents a keyboard switch object. The user can drive the
    /// UI using the keyboard.  Each keyboard switch object encapsulates
    /// a short cut or hotkey such as Ctrl-T. When this key combination
    ///  is detected, the switch event is raised.
    /// </summary>
    internal class KeyboardSwitch : ActuatorSwitchBase
    {
        /// <summary>
        /// Has this object been disposed?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes the keyboardswitch object
        /// </summary>
        public KeyboardSwitch()
        {
        }

        /// <summary>
        /// Initialize the keyboard actuator object
        /// </summary>
        /// <param name="switchObj"></param>
        public KeyboardSwitch(IActuatorSwitch switchObj)
            : base(switchObj)
        {
        }

        /// <summary>
        /// The keyboard hotkey this switch represents (e.g F5).  This
        /// is the 'source' attribute of a keyboard switch in the xml file
        /// </summary>
        public String HotKey { get; set; }

        /// <summary>
        /// Perform initialization
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            bool retVal = true;

            try
            {
                // parse the 'source' attribute in the XML file and figure
                // out which keyboard key it represents
                //SwitchKey = (Keys)Enum.Parse(typeof(Keys), Source, true);
                HotKey = Source;
            }
            catch (Exception e)
            {
                Log.Error("Invalid source key specified " + Source);
                Log.Exception(e);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Load keyboard switch details from the xml fragment
        /// </summary>
        /// <param name="xmlNode">The XML node</param>
        /// <returns>true on success</returns>
        public override bool Load(XmlNode xmlNode)
        {
            base.Load(xmlNode);

            return true;
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">true to dispose managed resources</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    Log.Debug();

                    if (disposing)
                    {
                        // release managed resources
                        unInit();
                    }

                    // Release the native unmanaged resources
                    _disposed = true;
                }
                finally
                {
                    // Call Dispose on your base class.
                    base.Dispose(disposing);
                }
            }
        }

        /// <summary>
        /// De-allocate resources
        /// </summary>
        /// <returns></returns>
        private void unInit()
        {
        }
    }
}