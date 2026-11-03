////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ScannerBuilder.cs
//
// Fluent builder for constructing scanner-related test data for ACAT tests.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using System.Collections.Generic;

namespace ACATCore.Tests.Builders
{
    /// <summary>
    /// Describes a scanner configuration for use in tests.
    /// This is a lightweight data-only representation that mirrors the
    /// information required to configure a panel scanner.
    /// </summary>
    public sealed class ScannerDescriptor
    {
        /// <summary>The class name identifying the scanner type.</summary>
        public string PanelClass { get; set; } = string.Empty;

        /// <summary>The color scheme applied to the scanner.</summary>
        public string ColorScheme { get; set; } = "Default";

        /// <summary>Scan interval in milliseconds.</summary>
        public int ScanTimeMs { get; set; } = 1000;

        /// <summary>Whether row-column scanning is enabled.</summary>
        public bool RowColumnScanEnabled { get; set; } = true;

        /// <summary>
        /// The underlying <see cref="PanelConfigJson"/> for this scanner.
        /// </summary>
        public PanelConfigJson PanelConfig { get; set; }
    }

    /// <summary>
    /// Fluent builder for <see cref="ScannerDescriptor"/> test data.
    /// </summary>
    public sealed class ScannerBuilder
    {
        private string _panelClass = "AlphabetScanner";
        private string _colorScheme = "Default";
        private int _scanTimeMs = 1000;
        private bool _rowColumnScanEnabled = true;
        private PanelConfigJson _panelConfig;

        /// <summary>Sets the panel class name.</summary>
        public ScannerBuilder WithPanelClass(string panelClass)
        {
            _panelClass = panelClass;
            return this;
        }

        /// <summary>Sets the color scheme.</summary>
        public ScannerBuilder WithColorScheme(string colorScheme)
        {
            _colorScheme = colorScheme;
            return this;
        }

        /// <summary>Sets the scan interval in milliseconds.</summary>
        public ScannerBuilder WithScanTime(int scanTimeMs)
        {
            _scanTimeMs = scanTimeMs;
            return this;
        }

        /// <summary>Enables or disables row-column scanning.</summary>
        public ScannerBuilder WithRowColumnScan(bool enabled)
        {
            _rowColumnScanEnabled = enabled;
            return this;
        }

        /// <summary>Sets the panel configuration for this scanner.</summary>
        public ScannerBuilder WithPanelConfig(PanelConfigJson config)
        {
            _panelConfig = config;
            return this;
        }

        /// <summary>Builds the <see cref="ScannerDescriptor"/>.</summary>
        public ScannerDescriptor Build()
        {
            return new ScannerDescriptor
            {
                PanelClass = _panelClass,
                ColorScheme = _colorScheme,
                ScanTimeMs = _scanTimeMs,
                RowColumnScanEnabled = _rowColumnScanEnabled,
                PanelConfig = _panelConfig ?? new PanelConfigurationBuilder()
                    .WithColorScheme(_colorScheme)
                    .Build()
            };
        }

        /// <summary>Returns a builder pre-configured as a standard alphabet scanner.</summary>
        public static ScannerBuilder AsAlphabetScanner()
        {
            return new ScannerBuilder()
                .WithPanelClass("AlphabetScanner")
                .WithColorScheme("Default")
                .WithScanTime(1000)
                .WithRowColumnScan(true);
        }

        /// <summary>Returns a builder pre-configured as a cursor navigation scanner.</summary>
        public static ScannerBuilder AsCursorScanner()
        {
            return new ScannerBuilder()
                .WithPanelClass("CursorScanner")
                .WithColorScheme("Default")
                .WithScanTime(800)
                .WithRowColumnScan(false);
        }
    }
}
