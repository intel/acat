////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// XmlAnimationConfigAdapter.cs
//
// Converts legacy XML <Animation> elements into AnimationConfig model objects.
// Enables migration without changing the 69 existing XML animation config files.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;

namespace ACAT.Core.AnimationManagement.Configuration
{
    /// <summary>
    /// Reads existing XML <c>&lt;Animations&gt;</c> / <c>&lt;Animation&gt;</c> elements
    /// (used by <see cref="Animation"/>) and converts them into the new
    /// <see cref="AnimationConfig"/> model.
    ///
    /// XML format:
    /// <code>
    /// &lt;Animations&gt;
    ///   &lt;Animation name="Row1" start="true" autoStart="true" scanTime="600" iterations="3"
    ///              firstPauseTime="0" onEnter="" onEnd=""&gt;
    ///     &lt;Widget name="Button1" onSelect="..." /&gt;
    ///     &lt;Widget name="Button2" onSelect="..." /&gt;
    ///   &lt;/Animation&gt;
    /// &lt;/Animations&gt;
    /// </code>
    /// </summary>
    public class XmlAnimationConfigAdapter
    {
        /// <summary>
        /// Converts the XML content of an <c>&lt;Animations&gt;</c> node into an
        /// <see cref="AnimationConfig"/> for the given panel.
        /// </summary>
        /// <param name="panelName">The name of the panel.</param>
        /// <param name="animationsNode">
        ///   An <see cref="XmlNode"/> representing the <c>&lt;Animations&gt;</c> root,
        ///   or any node whose children are <c>&lt;Animation&gt;</c> elements.
        /// </param>
        /// <returns>A populated <see cref="AnimationConfig"/>.</returns>
        public AnimationConfig Convert(string panelName, XmlNode animationsNode)
        {
            if (string.IsNullOrWhiteSpace(panelName)) throw new ArgumentNullException(nameof(panelName));
            if (animationsNode == null) throw new ArgumentNullException(nameof(animationsNode));

            var config = new AnimationConfig
            {
                PanelName = panelName,
                ScanStrategy = "auto"
            };

            foreach (XmlNode animNode in animationsNode.SelectNodes("Animation") ?? new EmptyXmlNodeList())
            {
                var seq = ConvertSequence(animNode);
                if (seq != null)
                    config.Sequences.Add(seq);
            }

            return config;
        }

        /// <summary>
        /// Converts a single <c>&lt;Animation&gt;</c> node into an
        /// <see cref="AnimationSequenceConfig"/>.
        /// </summary>
        public AnimationSequenceConfig ConvertSequence(XmlNode animNode)
        {
            if (animNode == null) return null;

            var seq = new AnimationSequenceConfig
            {
                Name = GetAttrString(animNode, "name"),
                IsFirst = GetAttrBool(animNode, "start", false),
                AutoStart = GetAttrBool(animNode, "autoStart", true),
                Iterations = GetAttrString(animNode, "iterations") ?? "1",
                ScanTime = GetAttrString(animNode, "scanTime"),
                FirstPauseTime = GetAttrString(animNode, "firstPauseTime"),
                OnEnter = GetAttrString(animNode, "onEnter"),
                OnEnd = GetAttrString(animNode, "onEnd")
            };

            foreach (XmlNode widgetNode in animNode.SelectNodes("Widget") ?? new EmptyXmlNodeList())
            {
                var widget = ConvertWidget(widgetNode);
                if (widget != null)
                    seq.Widgets.Add(widget);
            }

            return seq;
        }

        /// <summary>
        /// Converts a single <c>&lt;Widget&gt;</c> node into an
        /// <see cref="AnimationWidgetConfig"/>.
        /// </summary>
        public AnimationWidgetConfig ConvertWidget(XmlNode widgetNode)
        {
            if (widgetNode == null) return null;

            return new AnimationWidgetConfig
            {
                Name = GetAttrString(widgetNode, "name"),
                OnSelected = GetAttrString(widgetNode, "onSelect"),
                PlayBeep = GetAttrBool(widgetNode, "playBeep", false)
            };
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        private static string GetAttrString(XmlNode node, string attrName)
        {
            return node.Attributes?[attrName]?.Value;
        }

        private static bool GetAttrBool(XmlNode node, string attrName, bool defaultValue)
        {
            var val = GetAttrString(node, attrName);
            if (string.IsNullOrEmpty(val)) return defaultValue;
            return bool.TryParse(val, out bool result) ? result : defaultValue;
        }

        /// <summary>
        /// Empty XmlNodeList for safe iteration when SelectNodes returns null.
        /// </summary>
        private sealed class EmptyXmlNodeList : XmlNodeList
        {
            public override int Count => 0;
            public override XmlNode Item(int index) => null;
            public override System.Collections.IEnumerator GetEnumerator()
                => System.Array.Empty<XmlNode>().GetEnumerator();
        }
    }
}
