////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// PreferencesBuilder.cs
//
// Fluent builder for constructing preferences-related test data for ACAT tests.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Text;

namespace ACATCore.Tests.Builders
{
    /// <summary>
    /// Fluent builder for XML-based preferences test data used in ACAT tests.
    /// Produces serialized preference XML strings suitable for writing to temp
    /// files and loading through <c>PreferencesBase.Load&lt;T&gt;</c>.
    /// </summary>
    public sealed class PreferencesBuilder
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private string _rootElement = "Preferences";

        /// <summary>
        /// Sets the XML root element name (defaults to <c>"Preferences"</c>).
        /// </summary>
        public PreferencesBuilder WithRootElement(string elementName)
        {
            _rootElement = elementName;
            return this;
        }

        /// <summary>
        /// Adds a simple string property to the preferences XML.
        /// </summary>
        public PreferencesBuilder WithProperty(string name, string value)
        {
            _properties[name] = value;
            return this;
        }

        /// <summary>
        /// Adds a boolean property to the preferences XML.
        /// </summary>
        public PreferencesBuilder WithProperty(string name, bool value)
        {
            _properties[name] = value ? "true" : "false";
            return this;
        }

        /// <summary>
        /// Adds an integer property to the preferences XML.
        /// </summary>
        public PreferencesBuilder WithProperty(string name, int value)
        {
            _properties[name] = value.ToString();
            return this;
        }

        /// <summary>
        /// Builds an XML string representing these preferences.
        /// </summary>
        public string BuildXml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine($"<{_rootElement}>");
            foreach (var kvp in _properties)
            {
                sb.AppendLine($"  <{kvp.Key}>{EscapeXmlValue(kvp.Value)}</{kvp.Key}>");
            }
            sb.AppendLine($"</{_rootElement}>");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a builder pre-populated with common default preference values.
        /// </summary>
        public static PreferencesBuilder WithDefaults()
        {
            return new PreferencesBuilder()
                .WithProperty("AutoSwitchScannerEnable", true)
                .WithProperty("ScanTime", 1000)
                .WithProperty("Language", "en");
        }

        private static string EscapeXmlValue(string value)
        {
            return value
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;");
        }
    }
}
