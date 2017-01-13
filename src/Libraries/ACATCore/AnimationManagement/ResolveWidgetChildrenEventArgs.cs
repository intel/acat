////////////////////////////////////////////////////////////////////////////
// <copyright file="ResolveWidgetChildrenEventArgs.cs" company="Intel Corporation">
//
// Copyright (c) 2013-2017 Intel Corporation 
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