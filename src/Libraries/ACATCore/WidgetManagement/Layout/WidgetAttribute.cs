////////////////////////////////////////////////////////////////////////////
// <copyright file="WidgetAttribute.cs" company="Intel Corporation">
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using ACAT.Lib.Core.Interpreter;
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
    /// Holds attributes to a button widget such as the font to use,
    /// the name, value, whether it allows mouse clicks or not, etc.
    /// </summary>
    public class WidgetAttribute : IDisposable
    {
        /// <summary>
        /// The code to execute when the user clicks on the button
        /// </summary>
        public PCode OnMouseClick;

        /// <summary>
        /// Has this object been disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public WidgetAttribute()
        {
            FontName = CoreGlobals.AppPreferences.FontName;
            FontSize = CoreGlobals.AppPreferences.FontSize;
            FontBold = true;
            Name = String.Empty;
            Label = String.Empty;
            Value = String.Empty;
            Offset1 = -999;
            Offset2 = -999;
            Modifiers = null;
            MouseClickActuate = false;
            OnMouseClick = new PCode();
        }

        /// <summary>
        /// Alignment of text in the control
        /// </summary>
        public ContentAlignment? Alignment { get; set; }

        /// <summary>
        /// Whether to display the text as bold
        /// </summary>
        public bool FontBold { get; set; }

        /// <summary>
        /// The font to use to display this on the UI
        /// </summary>
        public String FontName { get; set; }

        /// <summary>
        /// The size of the font to use
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Is the value a virtual key (mapped to the Enum Keys)
        /// </summary>
        public bool IsVirtualKey { get; private set; }

        /// <summary>
        /// What to display on the screen
        /// </summary>
        public String Label { get; set; }

        /// <summary>
        /// Modifier keys such as Shift, Alt to send
        /// </summary>
        public ArrayList Modifiers { get; set; }

        /// <summary>
        /// Set this to true to enable actuation of widget with mouse clicks
        /// </summary>
        public bool MouseClickActuate { get; set; }

        /// <summary>
        /// The internal name of the button key.  This is the name
        /// given to the control in the form
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// If this is a two letter text that's displayed,
        /// offset of the 1st letter from the left margin of the bounding
        /// rectangle
        /// </summary>
        public int Offset1 { get; set; }

        /// <summary>
        /// If this is a two letter text that's displayed,
        /// offset of the 2nd letter from the left margin of the bounding
        /// rectangle
        /// </summary>
        public int Offset2 { get; set; }

        /// <summary>
        /// Tooltip help string
        /// </summary>
        public String ToolTip { get; set; }

        /// <summary>
        /// Internal string value.
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Class factory to create a WidgetAttribute object from
        /// the xml node.  The xml fragment (e.g.) is as follows
        ///   <WidgetAttribute name="B44" label="&lt;w" value="@unknown" fontname="Arial Narrow" fontsize="24"  offset2="16"/>
        /// </summary>
        /// <param name="node">the xml node</param>
        /// <returns>button attribute object</returns>
        public static WidgetAttribute CreateWidgetAttribute(XmlNode node)
        {
            var widgetAttribute = new WidgetAttribute();
            widgetAttribute.load(node);
            return widgetAttribute;
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
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Load settings from the XML node, extract the attributes
        /// and set the values of members in this object
        /// </summary>
        /// <param name="node">The xml node</param>
        private void load(XmlNode node)
        {
            Name = XmlUtils.GetXMLAttrString(node, "name");
            Label = XmlUtils.GetXMLAttrString(node, "label");
            Value = XmlUtils.GetXMLAttrString(node, "value");
            FontSize = XmlUtils.GetXMLAttrInt(node, "fontsize", FontSize);
            Offset1 = XmlUtils.GetXMLAttrInt(node, "offset1", Offset1);
            Offset2 = XmlUtils.GetXMLAttrInt(node, "offset2", Offset1);
            FontName = XmlUtils.GetXMLAttrString(node, "fontname", FontName);
            FontBold = XmlUtils.GetXMLAttrBool(node, "bold", FontBold);
            IsVirtualKey = XmlUtils.GetXMLAttrBool(node, "virtualkey", false);
            ToolTip = XmlUtils.GetXMLAttrString(node, "toolTip", String.Empty);
            MouseClickActuate = XmlUtils.GetXMLAttrBool(node, "mouseClickActuate", false);
            String onMouseClick = XmlUtils.GetXMLAttrString(node, "onMouseClick");
            if (!String.IsNullOrEmpty(onMouseClick))
            {
                var parser = new Parser();
                parser.Parse(onMouseClick, ref OnMouseClick);
            }

            parseModifiers(XmlUtils.GetXMLAttrString(node, "modifiers"));

            String align = XmlUtils.GetXMLAttrString(node, "align");

            Alignment = Enum.IsDefined(typeof(ContentAlignment), align) ?
                                (ContentAlignment)Enum.Parse(typeof(ContentAlignment), align) :
                                (ContentAlignment?)null;
        }

        /// <summary>
        /// Parse a string of modifiers into individual keys. The modifier
        /// string is a '+' separated array of keys.  Eg Ctrl+Alt. Parses
        /// this into array list of keys Keys.LControlKey, Keys.LMenu
        /// </summary>
        /// <param name="modifiers">Modifier string array</param>
        private void parseModifiers(String modifiers)
        {
            if (String.IsNullOrEmpty(modifiers))
            {
                return;
            }

            String[] array = modifiers.Split('+');
            if (array.Length > 0)
            {
                Modifiers = new ArrayList();
                foreach (string modifier in array)
                {
                    Keys key = Keys.None;
                    switch (modifier)
                    {
                        case "Shift":
                            key = Keys.LShiftKey;
                            break;

                        case "Ctrl":
                            key = Keys.LControlKey;
                            break;

                        case "Alt":
                            key = Keys.LMenu;
                            break;
                    }

                    if (key != Keys.None)
                    {
                        Modifiers.Add(key);
                    }
                    else
                    {
                        Log.Error("Invalid modifier " + modifier + " for widgetAttribute entry " + Name);
                        Modifiers = null;
                        break;
                    }
                }
            }
        }
    }
}