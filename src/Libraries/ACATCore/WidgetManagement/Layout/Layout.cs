////////////////////////////////////////////////////////////////////////////
// <copyright file="Layout.cs" company="Intel Corporation">
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ACAT.Lib.Core.ThemeManagement;
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
    /// Reads screen layout information from the config file.  The layout
    /// information is essentially captures the parent-child hierarchy of
    /// controls in the UI and also contains the Widget class (.NET type)
    /// for each widget which will be used to instatiate the widget.
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// List of widgets that have the "contextual=true" attribute
        /// enabled. For these widgets, the enable/disable state will
        /// be determined at runtime depending on the context
        /// </summary>
        private readonly List<Widget> _contextualWidgets;

        /// <summary>
        /// Name of the color scheme for the widgets in this layout.  Is read
        /// from the config file
        /// </summary>
        private String _colorSchemeName;

        /// <summary>
        /// Name of color scheme for disabled elements
        /// </summary>
        private String _disabledButtonColorSchemeName;

        /// <summary>
        /// The screen form
        /// </summary>
        private Control _rootControl;

        /// <summary>
        /// Initializes an instance of the Layout class
        /// </summary>
        public Layout()
        {
            _colorSchemeName = String.Empty;
            _disabledButtonColorSchemeName = String.Empty;
            Colors = ColorSchemes.DefaultColorScheme;
            _contextualWidgets = new List<Widget>();
            ConfigFile = String.Empty;
        }

        /// <summary>
        /// Gets the color scheme to use for all the widgets in this layout.
        /// THis can be overriden for individual widgets
        /// </summary>
        public ColorScheme Colors { get; internal set; }

        /// <summary>
        /// Name of the layout config xml file
        /// </summary>
        public String ConfigFile { get; internal set; }

        /// <summary>
        /// Returns a list of widgets that have the "contextual" attribute set.
        /// For these widgets, the enable/disable state will
        /// be determined at runtime depending on the context
        /// </summary>
        public IEnumerable ContextualWidgets
        {
            get { return _contextualWidgets; }
        }

        /// <summary>
        /// Gets the color scheme to be used for disabled buttons in this
        /// layout.  Can be overriden for individual widgets
        /// </summary>
        public ColorScheme DisabledButtonColors { get; internal set; }

        /// <summary>
        /// The screen widget
        /// </summary>
        public Widget RootWidget { get; set; }

        /// <summary>
        /// Gets the global color schemes object
        /// </summary>
        /// <returns></returns>
        public static ColorSchemes GetColorSchemes()
        {
            return ThemeManager.Instance.ActiveTheme.Colors;
        }

        /// <summary>
        /// Using reflection, create an instance of the widget with the
        /// specified name.  After creating the widget, sets the Layout
        /// property to this object.
        /// </summary>
        /// <param name="classType">Type of the widget class</param>
        /// <param name="widgetName">Name of the widget</param>
        /// <returns>Created widget</returns>
        public Widget CreateWidget(Type classType, String widgetName)
        {
            Widget widget = null;
            try
            {
                Log.Debug("creating widget with name " + widgetName);

                widget = (Widget)Activator.CreateInstance(classType, widgetName);

                Log.IsNull("Widget created ", widget);

                if (widget != null)
                {
                    widget.SetLayout(this);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return widget;
        }

        /// <summary>
        /// Using reflection, create widget with the specified Type for the
        /// .NET Form control (eg a Button or a Label)
        /// </summary>
        /// <param name="classType">Class type of the widget</param>
        /// <param name="uiControl">The form control</param>
        /// <returns>created widget object</returns>
        public Widget CreateWidget(Type classType, Control uiControl)
        {
            Widget widget = null;
            try
            {
                Log.Debug("creating widget " + classType);

                widget = (Widget)Activator.CreateInstance(classType, uiControl);

                Log.IsNull("Widget created ", widget);

                if (widget != null)
                {
                    widget.SetLayout(this);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }

            return widget;
        }

        /// <summary>
        /// Loads the layout information for the specified screen.  The Layout
        /// element in the XML config file contains the parent child relationship
        /// between widgets starting with the form at the root.  Creates
        /// the root widget and all the child widgets. This gives the flexibility
        /// to group control in any manner, different from the parent-child
        /// relationship in the Form design.
        /// </summary>
        /// <param name="configFile">Full name of the xml file</param>
        /// <param name="root">The screen control to load layout for</param>
        /// <returns>Root widget for the screen</returns>
        public bool Load(String configFile, Widget rootWidget)
        {
            bool retVal = true;

            Log.Debug("configFile: " + configFile + ", rootWidget.name is " + rootWidget.Name);

            ConfigFile = configFile;

            var xmlDoc = new XmlDocument();

            if (File.Exists(configFile))
            {
                _rootControl = rootWidget.UIControl;

                xmlDoc.Load(configFile);

                XmlNode node = xmlDoc.SelectSingleNode("/ACAT/Layout");

                if (node != null)
                {
                    // retreive color attributes for the layout
                    getLayoutColors(node);

                    RootWidget = rootWidget;
                    RootWidget.SetLayout(this);

                    // load all the children and recursively their children.
                    loadChildren(RootWidget, node);
                }
                else
                {
                    retVal = false;
                    Log.Error("Could not find layout element in xml file " + configFile + ", screen: " + rootWidget.Name);
                }
            }
            else
            {
                Log.Error("Could not find config file " + configFile);
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// sets the name of the color scheme
        /// </summary>
        /// <param name="scheme"></param>
        public void SetColorScheme(String scheme)
        {
            _colorSchemeName = scheme;
        }

        /// <summary>
        /// Sets the name color scheme of disabled buttons in this layout
        /// </summary>
        /// <param name="scheme"></param>
        public void SetDisabledButtonColorScheme(String scheme)
        {
            _disabledButtonColorSchemeName = scheme;
        }

        /// <summary>
        /// Creates a widget by parsing the xml node, extracting
        /// widget information and then using it to create the widget
        /// object.
        /// </summary>
        /// <param name="node">Node that contains widget info</param>
        /// <returns>widget, null if error</returns>
        private Widget createWidget(XmlNode node)
        {
            String widgetClass = XmlUtils.GetXMLAttrString(node, "class");
            String widgetName = XmlUtils.GetXMLAttrString(node, "name");

            if (String.IsNullOrEmpty(widgetName) || String.IsNullOrEmpty(widgetClass))
            {
                return null;
            }

            // look in the form for the .NET control for this widget
            Control control = findControl(_rootControl, widgetName);

            Widget widget = null;

            try
            {
                Type widgetType = WidgetManager.GetWidgetType(widgetClass);
                if (widgetType != null)
                {
                    widget = (control != null) ?
                                CreateWidget(widgetType, control) :
                                CreateWidget(widgetType, widgetName);

                    if (widget != null)
                    {
                        widget.LayoutXmlNode = node;
                        widget.Load(node);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("Error creating widget " + widgetName, e);
                widget = null;
            }

            return widget;
        }

        /// <summary>
        /// Recursively looks for the .NET control of the specified
        /// name in the parent control and returns it
        /// </summary>
        /// <param name="parent">The .NET parent control</param>
        /// <param name="name">Name of the control</param>
        /// <returns>Control if found, null if not</returns>
        private Control findControl(Control parent, String name)
        {
            Control retVal = null;

            foreach (Control control in parent.Controls)
            {
                if (String.Compare(name, control.Name, true) == 0)
                {
                    retVal = control;
                    break;
                }

                retVal = findControl(control, name);
                if (retVal != null)
                {
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Parses the node and retrieves color attributes for the layout
        /// </summary>
        /// <param name="node">node to parse</param>
        private void getLayoutColors(XmlNode node)
        {
            String colorScheme = XmlUtils.GetXMLAttrString(node, "colorScheme");
            if (!String.IsNullOrEmpty(colorScheme))
            {
                _colorSchemeName = colorScheme;
            }

            Colors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(_colorSchemeName);

            colorScheme = XmlUtils.GetXMLAttrString(node, "disabledButtonColorScheme");
            if (!String.IsNullOrEmpty(colorScheme))
            {
                _disabledButtonColorSchemeName = colorScheme;
            }

            DisabledButtonColors = ThemeManager.Instance.ActiveTheme.Colors.GetColorScheme(_disabledButtonColorSchemeName);
        }

        /// <summary>
        /// Recursively loads all the child widgets for the specified
        /// root widget
        /// </summary>
        /// <param name="rootWidget">The root</param>
        /// <param name="node">xml node that contains layout info</param>
        private void loadChildren(Widget rootWidget, XmlNode node)
        {
            Log.Debug("rootWidget=" + rootWidget.Name);
            XmlNodeList widgetNodes = node.SelectNodes("Widget");
            if (widgetNodes == null)
            {
                return;
            }

            foreach (XmlNode childNode in widgetNodes)
            {
                Widget childWidget = createWidget(childNode);
                if (childWidget != null)
                {
                    rootWidget.AddChild(childWidget);

                    if (childWidget.EnabledState == Widget.EnabledStates.Contextual)
                    {
                        _contextualWidgets.Add(childWidget);
                    }

                    // drill down and load this one's children
                    loadChildren(childWidget, childNode);

                    childWidget.PostLoad();
                }
            }
        }
    }
}