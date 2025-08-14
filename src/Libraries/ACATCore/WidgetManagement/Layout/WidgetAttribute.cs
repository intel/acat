////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Interpreter;
using ACAT.Core.Utility;
using ACATResources;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace ACAT.Core.WidgetManagement.Layout
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
            //FontName = CoreGlobals.AppPreferences.FontName;
            //FontSize = CoreGlobals.AppPreferences.FontSize;
            //FontBold = true;
            //FontItalic = false;
            FontName = null;
            FontSize = 0;
            FontBold = false;
            FontItalic = false;
            Name = string.Empty;
            Label = string.Empty;
            Value = string.Empty;
            Modifiers = null;
            MouseClickActuate = true;
            OnMouseClick = new PCode();
        }

        /// <summary>
        /// Alignment of text in the control
        /// </summary>
        public ContentAlignment? Alignment { get; set; }

        public KeyValuePairs ExtendedAttributes { get; private set; }

        /// <summary>
        /// Whether to display the text as bold
        /// </summary>
        public bool FontBold { get; set; }

        /// <summary>
        /// Whether to display the text as italic
        /// </summary>
        public bool FontItalic { get; set; }

        /// <summary>
        /// The font to use to display this on the UI
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// The size of the font to use
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Is the value a virtual key (mapped to the Enum Keys)
        /// </summary>
        public bool IsVirtualKey { get; private set; }

        /// <summary>
        /// What to display on the control in the form
        /// </summary>
        public string Label { get; set; }

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
        public string Name { get; set; }

        /// <summary>
        /// Value when the Shift key is pressed
        /// </summary>
        public string ShiftValue { get; set; }

        /// <summary>
        /// Tooltip help string
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Internal string value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Class factory to create a WidgetAttribute object from
        /// the xml node.  The xml fragment (e.g.) is as follows
        ///   <WidgetAttribute name="B44" label="&lt;w" value="@CmdMainMenu" fontname="Arial Narrow" fontsize="24"/>
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
                Log.Verbose();

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

            Value = XmlUtils.GetXMLAttrString(node, "value");
            FontSize = XmlUtils.GetXMLAttrInt(node, "fontsize", FontSize);
            FontName = XmlUtils.GetXMLAttrString(node, "fontname", FontName);

            string label = XmlUtils.GetXMLAttrString(node, "label", "").Trim();

            if (FontName != null && !FontName.Contains("ACAT") && label != null && label.Length > 1)
            {
                Label = StringResources.ResourceManager.GetString(label) ?? label;
            }
            else
            {
                Label = label;
            }

            FontBold = XmlUtils.GetXMLAttrBool(node, "bold", FontBold);
            FontItalic = XmlUtils.GetXMLAttrBool(node, "italic", FontItalic);
            IsVirtualKey = XmlUtils.GetXMLAttrBool(node, "virtualkey", false);
            ToolTip = XmlUtils.GetXMLAttrString(node, "toolTip", string.Empty);
            ShiftValue = XmlUtils.GetXMLAttrString(node, "shiftValue", string.Empty);
            MouseClickActuate = XmlUtils.GetXMLAttrBool(node, "mouseClickActuate", true);
            string onMouseClick = XmlUtils.GetXMLAttrString(node, "onMouseClick");
            if (!string.IsNullOrEmpty(onMouseClick))
            {
                var parser = new Parser();
                parser.Parse(onMouseClick, ref OnMouseClick);
            }

            parseModifiers(XmlUtils.GetXMLAttrString(node, "modifiers"));

            string align = XmlUtils.GetXMLAttrString(node, "align");

            Alignment = Enum.IsDefined(typeof(ContentAlignment), align) ?
                                (ContentAlignment)Enum.Parse(typeof(ContentAlignment), align) :
                                null;

            ExtendedAttributes = new KeyValuePairs();

            var extendedAttr = XmlUtils.GetXMLAttrString(node, "extendedAttributes");
            if (!string.IsNullOrEmpty(extendedAttr))
            {
                ExtendedAttributes.Parse(extendedAttr);
            }
        }

        /// <summary>
        /// Parse a string of modifiers into individual keys. The modifier
        /// string is a '+' separated array of keys.  Eg Ctrl+Alt. Parses
        /// this into array list of keys Keys.LControlKey, Keys.LMenu
        /// </summary>
        /// <param name="modifiers">Modifier string array</param>
        private void parseModifiers(string modifiers)
        {
            if (string.IsNullOrEmpty(modifiers))
            {
                return;
            }

            string[] array = modifiers.Split('+');
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