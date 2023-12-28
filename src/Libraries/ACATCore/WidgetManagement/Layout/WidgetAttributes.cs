////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ACAT.Lib.Core.WidgetManagement
{
    /// <summary>
    /// Represents a collection of WidgetAttribute objects.
    /// The attributes are read from the <WidgetAttributes> section
    /// of the scanner config file
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
        /// Gets the collection of widget attributes
        /// </summary>
        public ICollection<WidgetAttribute> Attributes
        {
            get { return _widgetAttributes.Values; }
        }

        /// <summary>
        /// Retrieves the WidgetAttribute object for the widget
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

        /// <summary>
        /// Checks if  the widget attribute for the specified
        /// widget name exists.
        /// </summary>
        /// <param name="name">name of the widget</param>
        /// <returns>true if it does</returns>
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
        /// Loads widget attribute collection from the XML file. The
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
                try
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

                        widgetAttribute.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("Error loading config file " + configFile + ", " + ex.ToString());
                    retVal = false;
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