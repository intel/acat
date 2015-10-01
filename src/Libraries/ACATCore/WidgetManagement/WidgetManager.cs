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
using System.IO;
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
    /// Caches the .NET types of all the widgets in the Core DLL as well
    /// as any in the Extensions\Widgets folder.  All widgets MUST derive
    /// from the Widget class.  The cached types are then used to create
    /// the Widget objects.
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
        private static List<String> _widgetTypeCollection;

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
            _widgetAttributes = new WidgetAttributes();
            _layout = new Layout();
            _rootWidget = new Widget(control);

            Log.Debug("control name is : " + control.Name);
            Log.Debug("_rootWidget.name is  : " + _rootWidget.Name);
        }

        /// <summary>
        /// Caches .NET types of the widgets.
        /// </summary>
        /// <param name="extensionDirs"></param>
        /// <returns></returns>
        public static bool Init(IEnumerable<String> extensionDirs)
        {
            if (_widgetTypeCollection == null)
            {
                loadWidgetTypeCollection(extensionDirs);
            }

            return true;
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
        /// Parses the xml config file for the panel and creates the layout
        /// and all the widgets for the panel.  These widgets are all
        /// descendents of the root widget which is the Panel itself.
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

                retVal = Layout.Load(configPath, _rootWidget);
                if (retVal)
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
        static private void loadWidgetTypeCollection(IEnumerable<String> extensionDirs)
        {
            var assembly = Assembly.GetExecutingAssembly();
            _widgetTypeCollection = new List<String>();
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (typeof(Widget).IsAssignableFrom(type))
                {
                    _widgetTypeCollection.Add(type.FullName);
                }
            }

            foreach (var dir in extensionDirs)
            {
                var targetDir = dir + "\\Widgets";
                load(targetDir);
#if abc
                targetDir = dir + "\\UI";
                load(targetDir);

                targetDir = dir + "\\Agents";
                load(targetDir);
#endif
            }
        }

        /// <summary>
        /// Walks the specified directory (rescursively)
        /// to look for dll's that may contain widgets
        /// </summary>
        /// <param name="dir">Directory to walk</param>
        /// <param name="resursive">Recursively search?</param>
        private static void load(String dir, bool resursive = true)
        {
            var walker = new DirectoryWalker(dir, "*.dll");
            Log.Debug("Walking dir " + dir);
            walker.Walk(new OnFileFoundDelegate(onFileFound));
        }

        /// Callback function for the directory walker that's invoked
        /// when a file is found.  Checks the file is a dll and handles
        /// loads widgets from the dll
        /// </summary>
        /// <param name="file">name of the file found</param>
        private static void onFileFound(String file)
        {
            String filePath = file.ToLower();
            String fileName = Path.GetFileName(filePath);
            String extension = Path.GetExtension(filePath);
            if (String.Compare(extension, ".dll", true) == 0)
            {
                onDllFound(filePath);
            }
        }

        /// <summary>
        /// Found a DLL.  Load the class Types of all the relevant classes
        /// from the DLL
        /// </summary>
        /// <param name="dllName">name of the dll</param>
        private static void onDllFound(String dllName)
        {
            try
            {
                Log.Debug("Found dll " + dllName);
                loadTypesFromAssembly(Assembly.LoadFile(dllName));
            }
            catch (Exception ex)
            {
                Log.Debug("Could get types from assembly " + dllName + ". Exception : " + ex);
                if (ex is ReflectionTypeLoadException)
                {
                    var typeLoadException = (ReflectionTypeLoadException)ex;
                    var exceptions = typeLoadException.LoaderExceptions;
                    foreach (var e in exceptions)
                    {
                        Log.Debug("Loader exception: " + e);
                    }
                }
            }
        }

        /// <summary>
        /// Loads relevant types from the assembly and caches them
        /// </summary>
        /// <param name="assembly">name of the assembly</param>
        /// <returns>true on success</returns>
        private static bool loadTypesFromAssembly(Assembly assembly)
        {
            bool retVal = true;

            if (assembly == null)
            {
                return false;
            }

            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(Widget).IsAssignableFrom(type) && !_widgetTypeCollection.Contains(type.FullName))
                    {
                        _widgetTypeCollection.Add(type.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
                retVal = false;
            }

            return retVal;
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