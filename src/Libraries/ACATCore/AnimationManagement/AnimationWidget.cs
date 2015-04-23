////////////////////////////////////////////////////////////////////////////
// <copyright file="AnimationWidget.cs" company="Intel Corporation">
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
using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;

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

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Represents a single animation widget that is a part
    /// of the animation sequence.  This class acts as a container class
    /// for the acutal ui widget.  The animation widget
    /// the PCodes associated with the animations. The remaining
    /// attributes such as the highlight color etc are in the UI
    ///  widget class.
    /// widget
    /// </summary>
    public class AnimationWidget : IDisposable
    {
        /// <summary>
        /// Additional amount of time to highlight this widget
        /// </summary>
        public int HesitateTime;

        /// <summary>
        /// Code to execute if the back button is selected
        /// </summary>
        public PCode OnBack;

        /// <summary>
        /// Code to execute when this widget is unhighlighted
        /// </summary>
        public PCode OnHighlightOff;

        /// <summary>
        /// Code to execute when this widget is highlighted
        /// </summary>
        public PCode OnHighlightOn;

        /// <summary>
        /// Code to execute when mouse is clicked on the widget
        /// </summary>
        public PCode OnMouseClick;

        /// <summary>
        /// Code to execute if this widget is selected
        /// </summary>
        public PCode OnSelect;

        /// <summary>
        /// Parser object to parse the script
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Has this object been disposed off yet?
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Hesitate string as read from the XML file
        /// </summary>
        private string _hesitateTimeVariableName;

        /// <summary>
        /// Initializes the animation widget object
        /// </summary>
        public AnimationWidget()
        {
            _parser = new Parser();
            OnSelect = new PCode();
            OnBack = new PCode();
            OnHighlightOn = new PCode();
            OnHighlightOff = new PCode();
            OnMouseClick = new PCode();
            HesitateTime = CoreGlobals.AppPreferences.HesitateTime;
            CoreGlobals.AppPreferences.EvtPreferencesChanged += AppPreferences_EvtPreferencesChanged;
        }

        /// <summary>
        /// Gets or sets the contained UI widget
        /// </summary>
        public Widget UIWidget { get; set; }

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
        /// Load attributes from XML file for a <Widget> node in an Animation node
        /// </summary>
        /// <param name="xmlNode">The input xml node</param>
        public void Load(XmlNode xmlNode)
        {
            var onSelect = XmlUtils.GetXMLAttrString(xmlNode, "onSelect");
            _parser.Parse(onSelect, ref OnSelect);

            var onBack = XmlUtils.GetXMLAttrString(xmlNode, "onBack");
            _parser.Parse(onBack, ref OnBack);

            var onHighlightOn = XmlUtils.GetXMLAttrString(xmlNode, "onHighlightOn");
            _parser.Parse(onHighlightOn, ref OnHighlightOn);

            var onHighlightOff = XmlUtils.GetXMLAttrString(xmlNode, "onHighlightOff");
            _parser.Parse(onHighlightOff, ref OnHighlightOff);

            var onMouseClick = XmlUtils.GetXMLAttrString(xmlNode, "onMouseClick");
            _parser.Parse(onMouseClick, ref OnMouseClick);

            _hesitateTimeVariableName = XmlUtils.GetXMLAttrString(xmlNode, "hesitateTime");
            HesitateTime = CoreGlobals.AppPreferences.ResolveVariableInt(_hesitateTimeVariableName, 0, 0);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose all managed resources.
                    CoreGlobals.AppPreferences.EvtPreferencesChanged -= AppPreferences_EvtPreferencesChanged;
                }

                // Release unmanaged resources.
            }

            _disposed = true;
        }

        /// <summary>
        /// Event triggered when preferences are changed
        /// </summary>
        private void AppPreferences_EvtPreferencesChanged()
        {
            HesitateTime = CoreGlobals.AppPreferences.ResolveVariableInt(_hesitateTimeVariableName, 0, 0);
        }
    }
}