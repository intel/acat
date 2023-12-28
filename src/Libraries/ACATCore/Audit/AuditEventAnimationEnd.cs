////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace ACAT.Lib.Core.Audit
{
    /// <summary>
    /// Represents log entry to audit end of an animation sequence
    /// </summary>
    public class AuditEventAnimationEnd : AuditEventBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AuditEventAnimationEnd()
            : base("AnimationEnd")
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="rootWidget">root widget of the scanner</param>
        /// <param name="highlightWidgetName">name of the highlighted widget</param>
        /// <param name="highlightWidgetType">type of the highlighed widget</param>
        /// <param name="animationName">name of the animation sequence</param>
        public AuditEventAnimationEnd(String rootWidget,
                                    String highlightWidgetName,
                                    String highlightWidgetType,
                                    String animationName)
            : base("AnimationEnd")
        {
            RootWidgetName = rootWidget;
            HighlightWidgetName = highlightWidgetName;
            HighlightWidgetType = highlightWidgetType;
            AnimationName = animationName;
        }

        /// <summary>
        /// Gets or sets the animation sequence name
        /// </summary>
        public String AnimationName { get; set; }

        /// <summary>
        /// Gets or sets the highlighted widget name
        /// </summary>
        public String HighlightWidgetName { get; set; }

        /// <summary>
        /// Gets or sets the type of the highlighted widget
        /// </summary>
        public String HighlightWidgetType { get; set; }

        /// <summary>
        /// Gets or sets the root widget name for the scanner
        /// </summary>
        public String RootWidgetName { get; set; }

        /// <summary>
        /// Converts to string
        /// </summary>
        /// <returns>string representation</returns>
        protected override string toString()
        {
            return formatEventString(RootWidgetName, HighlightWidgetName, HighlightWidgetType, AnimationName);
        }
    }
}