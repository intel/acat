////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.Interpreter;
using ACAT.Lib.Core.Utility;
using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Xml;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Represents a single animation widget that is a part
    /// of the animation sequence.  This class acts as a container class
    /// for the acutal ui widget.  The animation widget
    /// the PCodes associated with the animations. The remaining
    /// attributes such as the highlight color etc are in the UI
    /// widget class.
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
            HesitateTime = CoreGlobals.AppPreferences.FirstPauseTime;
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
        /// Loads attributes from XML file for a <Widget> node in an Animation node
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

            _hesitateTimeVariableName = XmlUtils.GetXMLAttrString(xmlNode, "firstPauseTime");
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