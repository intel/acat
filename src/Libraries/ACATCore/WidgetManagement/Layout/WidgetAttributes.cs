////////////////////////////////////////////////////////////////////////////
// <copyright file="WidgetAttributes.cs" company="Intel Corporation">
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
using System.IO;
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

#endregion SupressStyleCopWarnings

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Represents a collection of WidgetAttribute objects.  This
    /// contains attributes for the button key elements
    /// </summary>
    public class WidgetAttributes : IDisposable
    {
        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Maps the name of the widget to its widget attribute
        /// </summary>
        private Dictionary<String, WidgetAttribute> _widgetAttributes;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WidgetAttributes()
        {
            _widgetAttributes = new Dictionary<String, WidgetAttribute>();
        }

        /// <summary>
        /// Gets the collection of button attributes
        /// </summary>
        public ICollection<WidgetAttribute> Attributes
        {
            get { return _widgetAttributes.Values; }
        }

        /// <summary>
        /// Retrieves the WidgetAttribute object for the button
        /// with the specified name
        /// </summary>
        /// <param name="name">name of the button</param>
        /// <returns>its button attribute, null if none</returns>
        public WidgetAttribute this[String name]
        {
            get
            {
                WidgetAttribute retVal = null;
                _widgetAttributes.TryGetValue(name, out retVal);
                return retVal;
            }
        }

        public bool Contains(String name)
        {
            return this[name] != null;
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Load button attribute collection from the XML file. The
        /// xml fragment that represents this collection is:
        /// <WidgetAttributes>
        ///  <WidgetAttribute name="B1" label="-" value="@CmdSuffix"/>
        ///  <WidgetAttribute name="B2" label="a" value="a"/>
        ///  <WidgetAttribute name="B3" label="i" value="i"/>
        ///  <WidgetAttribute name="B4" label="c" value="c"/>
        ///      ..
        ///      ..
        /// </WidgetAttributes>
        /// </summary>
        /// <param name="configFile">Full path to the xml file</param>
        /// <returns>true on success</returns>
        public bool Load(String configFile)
        {
            bool retVal = true;

            var xmlDoc = new XmlDocument();

            Log.Debug("configFile=" + configFile);

            if (File.Exists(configFile))
            {
                xmlDoc.Load(configFile);

                XmlNodeList widgetAttributeNodes = xmlDoc.SelectNodes("/ACAT/WidgetAttributes/WidgetAttribute");
                if (_widgetAttributes == null)
                {
                    return false;
                }

                // load all the elements
                foreach (XmlNode node in widgetAttributeNodes)
                {
                    var widgetAttribute = WidgetAttribute.CreateWidgetAttribute(node);
                    if (!_widgetAttributes.ContainsKey(widgetAttribute.Name))
                    {
                        _widgetAttributes.Add(widgetAttribute.Name, widgetAttribute);
                    }
                }
            }
            else
            {
                Log.Debug("Could not load WidgetAttributes. File does not exist " + configFile);
                retVal = false;
            }

            return retVal;
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
                    foreach (WidgetAttribute attr in _widgetAttributes.Values)
                    {
                        attr.Dispose();
                    }

                    _widgetAttributes.Clear();
                    _widgetAttributes = null;
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }
    }
}