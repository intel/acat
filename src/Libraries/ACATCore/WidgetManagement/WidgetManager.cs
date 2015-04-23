////////////////////////////////////////////////////////////////////////////
// <copyright file="WidgetManager.cs" company="Intel Corporation">
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
using System.Reflection;
using System.Windows.Forms;
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
    /// Parses the config file for the scanner and creates the two major elements
    /// from it:  The WidgetAttributes element and the Layout element.  The
    /// WidgetAttributes element contains extended attributes for all the
    /// widgets in the scanner.  The Layout element contains the parent-child relationship
    /// between widgets in the scanner.
    /// </summary>
    public class WidgetManager : IDisposable
    {
        /// <summary>
        /// Holds the types of all classes in the executing assembly that
        /// derive from the Widget class
        /// </summary>
        private static IEnumerable<String> _widgetTypeCollection;

        /// <summary>
        /// The layout object for the scanner
        /// </summary>
        private readonly Layout _layout;

        /// <summary>
        /// Represents the scanner Form
        /// </summary>
        private readonly Widget _rootWidget;

        /// <summary>
        /// WidgetAttributes collection for the scanner
        /// </summary>
        private readonly WidgetAttributes _widgetAttributes;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of WidgetManager
        /// </summary>
        public WidgetManager(Control control)
        {
            if (_widgetTypeCollection == null)
            {
                _widgetTypeCollection = loadWidgetTypeCollection();
            }

            _widgetAttributes = new WidgetAttributes();
            _layout = new Layout();
            _rootWidget = new Widget(control);

            Log.Debug("control name is : " + control.Name);
            Log.Debug("_rootWidget.name is  : " + _rootWidget.Name);
        }

        /// <summary>
        /// Gets the types of all classes in the executing assembly that
        /// derive from the Widget class
        /// </summary>
        public static IEnumerable<String> WidgetTypeCollection
        {
            get { return _widgetTypeCollection; }
        }

        /// <summary>
        /// Gets for the layout manager object for the scanner
        /// </summary>
        public Layout Layout
        {
            get { return _layout; }
        }

        /// <summary>
        /// Gets for the root widget. This is the scanner Form itself.
        /// </summary>
        public Widget RootWidget
        {
            get { return _rootWidget; }
        }

        /// <summary>
        /// Gets for the WidgetAttributes that was loaded from
        /// the config file
        /// </summary>
        public WidgetAttributes WidgetAttributes
        {
            get { return _widgetAttributes; }
        }

        /// <summary>
        /// Gets the .NET type of the widget with the specified
        /// widget type (may be partially spelled out)
        /// </summary>
        /// <param name="widgetTypeName">the partial widget type</param>
        /// <returns>Full type name, empty string if invalid</returns>
        public static Type GetWidgetType(String widgetTypeName)
        {
            String fullName = GetWidgetTypeFullName(widgetTypeName);
            Type retVal = null;

            try
            {
                if (!string.IsNullOrEmpty(fullName))
                {
                    retVal = Type.GetType(fullName);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Could not find widgettype " + widgetTypeName + ", exception: " + ex);
            }

            return retVal;
        }

        /// <summary>
        /// Gets the full type name of the widget.
        /// </summary>
        /// <param name="widgetTypeName">the partial widget type</param>
        /// <returns>Full type name, empty string if invalid</returns>
        public static String GetWidgetTypeFullName(String widgetTypeName)
        {
            foreach (String s in _widgetTypeCollection)
            {
                if (s.EndsWith("." + widgetTypeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return s;
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Call this to dispose off the objects
        /// </summary>
        ///
        public void Dispose()
        {
            Dispose(true);

            // Prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Parses the xml config file for the screen and creates the layout
        /// and all the widgets for the screen.  These widgets are all
        /// descendents of the root widget which is the screen itself.
        /// </summary>
        /// <param name="configPath">full path to the config file</param>
        /// <returns>true on success</returns>
        public bool Initialize(String configPath)
        {
            bool retVal;

            retVal = WidgetAttributes.Load(configPath);

            if (retVal)
            {
                Log.Debug("configPath: " + configPath);
                Log.IsNull("Layout for root widget ", Layout.RootWidget);

                if (retVal = Layout.Load(configPath, _rootWidget))
                {
                    retrieveAndSetWidgetAttribute(Layout.RootWidget);
                }
            }
            else
            {
                Log.Debug("Could not load WidgetAttributes from configFile [" + configPath + "]");
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
                    if (_rootWidget != null)
                    {
                        _rootWidget.Dispose();
                    }

                    if (_widgetAttributes != null)
                    {
                        _widgetAttributes.Dispose();
                    }
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Creates a list of types in the executing assembly that
        /// derive from the type "Widget"
        /// </summary>
        /// <returns>list of types</returns>
        private IEnumerable<String> loadWidgetTypeCollection()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var collection = new List<String>();
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                if (typeof(Widget).IsAssignableFrom(t))
                {
                    collection.Add(t.FullName);
                }
            }

            return collection;
        }

        /// <summary>
        /// Widgets have a whole bunch of extended attribtes that are contained
        /// in the WidgetAttributes node in the config file.  Retrieves and
        /// sets the widget attributes for all the widgets and recursively
        /// for all their descedents.
        /// </summary>
        /// <param name="widget"></param>
        private void retrieveAndSetWidgetAttribute(Widget widget)
        {
            Log.Debug("widget.Name=" + widget.Name);

            if (widget is IButtonWidget && WidgetAttributes.Contains(widget.Name))
            {
                IButtonWidget button = (IButtonWidget)widget;
                WidgetAttribute widgetAttribute = WidgetAttributes[widget.Name];
                button.SetWidgetAttribute(widgetAttribute);
            }

            foreach (Widget child in widget.Children)
            {
                retrieveAndSetWidgetAttribute(child);
            }
        }
    }
}