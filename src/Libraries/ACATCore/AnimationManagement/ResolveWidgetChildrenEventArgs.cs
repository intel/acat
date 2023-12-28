////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Lib.Core.WidgetManagement;
using System;
using System.Xml;

namespace ACAT.Lib.Core.AnimationManagement
{
    /// <summary>
    /// Event argument for the event raised to resolve
    /// widget references in an animation sequence
    /// </summary>
    public class ResolveWidgetChildrenEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="containerWidget">the parent widget</param>
        /// <param name="xmlNode">xml node to parse</param>
        public ResolveWidgetChildrenEventArgs(Widget rootWidget, Widget containerWidget, XmlNode xmlNode)
        {
            RootWidget = rootWidget;
            ContainerWidget = containerWidget;
            Node = xmlNode;
        }

        /// <summary>
        /// Gets the parent widget
        /// </summary>
        public Widget ContainerWidget { get; private set; }

        /// <summary>
        /// Gets the xml node
        /// </summary>
        public XmlNode Node { get; private set; }

        /// <summary>
        /// Gets root widget of the scanner
        /// </summary>
        public Widget RootWidget { get; private set; }
    }
}