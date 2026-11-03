////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationConfigConverter.cs
//
// Converts legacy XML panel config files into standalone
// {panelName}.animation.json files consumable by AnimationConfigProvider.
//
// Usage:
//   var converter = new AnimationConfigConverter();
//
//   // Single file
//   AnimationConfigJson? config = converter.ConvertFile("path/to/Panel.xml");
//   if (config != null)
//       await converter.WriteAsync(config, outputDir);
//
//   // Batch – all panel XML files in a directory tree
//   var results = await converter.ConvertDirectoryAsync(inputDir, outputDir);
//
////////////////////////////////////////////////////////////////////////////

using ACAT.ConfigMigrationTool.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace ACAT.ConfigMigrationTool
{
    /// <summary>
    /// Converts panel XML config files that contain <c>&lt;Animations&gt;</c> elements
    /// into standalone <c>{panelName}.animation.json</c> files.
    ///
    /// The output format matches the <c>AnimationConfig</c> model used by
    /// <c>AnimationConfigProvider</c> in ACATCore, so the JSON files can be loaded
    /// directly at runtime without further transformation.
    /// </summary>
    public class AnimationConfigConverter
    {
        private static readonly JsonSerializerOptions _writeOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = null,           // use explicit [JsonPropertyName] attributes
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        // ---------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------

        /// <summary>
        /// Converts a single panel XML config file to an <see cref="AnimationConfigJson"/>.
        /// </summary>
        /// <param name="xmlFilePath">Full path to the panel XML config file.</param>
        /// <returns>
        ///   The converted config, or <c>null</c> if the file contains no
        ///   <c>&lt;Animations&gt;</c> element or cannot be parsed.
        /// </returns>
        public AnimationConfigJson? ConvertFile(string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                throw new ArgumentNullException(nameof(xmlFilePath));
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException("Panel config XML file not found.", xmlFilePath);

            string panelName = Path.GetFileNameWithoutExtension(xmlFilePath);

            XmlDocument doc = LoadXml(xmlFilePath);
            XmlNode? animationsNode = doc.SelectSingleNode("/ACAT/Animations");
            if (animationsNode == null)
                return null;   // file has no animations; skip silently

            return ConvertNode(panelName, animationsNode);
        }

        /// <summary>
        /// Converts a raw <c>&lt;Animations&gt;</c> <see cref="XmlNode"/> for the given
        /// panel name. Useful for unit tests and in-process callers.
        /// </summary>
        public AnimationConfigJson ConvertNode(string panelName, XmlNode animationsNode)
        {
            if (string.IsNullOrWhiteSpace(panelName)) throw new ArgumentNullException(nameof(panelName));
            if (animationsNode == null) throw new ArgumentNullException(nameof(animationsNode));

            var config = new AnimationConfigJson
            {
                PanelName = panelName,
                ScanStrategy = "auto"
            };

            foreach (XmlNode animNode in SelectChildElements(animationsNode, "Animation"))
            {
                var seq = ConvertSequence(animNode);
                if (seq != null)
                    config.Sequences.Add(seq);
            }

            return config;
        }

        /// <summary>
        /// Serialises an <see cref="AnimationConfigJson"/> and writes it to
        /// <c>{outputDir}/{config.PanelName}.animation.json</c>.
        /// </summary>
        /// <returns>The full path of the written file.</returns>
        public async Task<string> WriteAsync(AnimationConfigJson config, string outputDir)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(outputDir)) throw new ArgumentNullException(nameof(outputDir));

            Directory.CreateDirectory(outputDir);

            string fileName = config.PanelName + ".animation.json";
            string outputPath = Path.Combine(outputDir, fileName);

            string json = JsonSerializer.Serialize(config, _writeOptions);
            await File.WriteAllTextAsync(outputPath, json);
            return outputPath;
        }

        /// <summary>
        /// Converts all panel XML files in <paramref name="inputDir"/> (recursively) and
        /// writes <c>.animation.json</c> output files to <paramref name="outputDir"/>,
        /// preserving the relative sub-directory structure.
        /// </summary>
        /// <param name="dryRun">
        ///   When <c>true</c>, conversions are performed in-memory but no files are written.
        /// </param>
        /// <returns>A summary of the conversion run.</returns>
        public async Task<AnimationConversionResult> ConvertDirectoryAsync(
            string inputDir,
            string outputDir,
            bool dryRun = false)
        {
            if (!Directory.Exists(inputDir))
                throw new DirectoryNotFoundException($"Input directory not found: {inputDir}");

            var result = new AnimationConversionResult { DryRun = dryRun };

            string[] xmlFiles = Directory.GetFiles(inputDir, "*.xml", SearchOption.AllDirectories);
            result.TotalFiles = xmlFiles.Length;

            foreach (string xmlFile in xmlFiles)
            {
                try
                {
                    AnimationConfigJson? config = ConvertFile(xmlFile);

                    if (config == null || config.Sequences.Count == 0)
                    {
                        result.SkippedCount++;
                        continue;
                    }

                    if (!dryRun)
                    {
                        // Mirror the relative sub-directory structure
                        string relativePath = Path.GetRelativePath(inputDir, Path.GetDirectoryName(xmlFile)!);
                        string targetDir = relativePath == "."
                            ? outputDir
                            : Path.Combine(outputDir, relativePath);

                        string outputPath = await WriteAsync(config, targetDir);
                        result.SuccessfulFiles.Add(outputPath);
                    }
                    else
                    {
                        result.SuccessfulFiles.Add(xmlFile); // record source path in dry-run
                    }

                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.FailureCount++;
                    result.Errors.Add((xmlFile, ex.Message));
                }
            }

            return result;
        }

        // ---------------------------------------------------------------
        // Internal conversion helpers
        // ---------------------------------------------------------------

        private AnimationSequenceConfigJson? ConvertSequence(XmlNode animNode)
        {
            if (animNode == null) return null;

            var seq = new AnimationSequenceConfigJson
            {
                Name          = GetAttr(animNode, "name") ?? string.Empty,
                IsFirst       = ParseBool(GetAttr(animNode, "start"), false),
                AutoStart     = ParseBool(GetAttr(animNode, "autoStart"), true),
                Iterations    = GetAttr(animNode, "iterations") ?? "1",
                ScanTime      = NullIfEmpty(GetAttr(animNode, "scanTime")),
                FirstPauseTime = NullIfEmpty(GetAttr(animNode, "firstPauseTime")),
                OnEnter       = NullIfEmpty(GetAttr(animNode, "onEnter")),
                OnEnd         = NullIfEmpty(GetAttr(animNode, "onEnd"))
            };

            foreach (XmlNode widgetNode in SelectChildElements(animNode, "Widget"))
            {
                var widget = ConvertWidget(widgetNode);
                if (widget != null)
                    seq.Widgets.Add(widget);
            }

            return seq;
        }

        private static AnimationWidgetConfigJson? ConvertWidget(XmlNode widgetNode)
        {
            if (widgetNode == null) return null;

            return new AnimationWidgetConfigJson
            {
                Name      = GetAttr(widgetNode, "name") ?? string.Empty,
                PlayBeep  = ParseBool(GetAttr(widgetNode, "playBeep"), false),
                OnSelected = NullIfEmpty(GetAttr(widgetNode, "onSelect"))
            };
        }

        // ---------------------------------------------------------------
        // XML helpers
        // ---------------------------------------------------------------

        /// <summary>
        /// Loads an XML file, tolerating DTD declarations (common in ACAT panel configs).
        /// </summary>
        private static XmlDocument LoadXml(string path)
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,
                ValidationType = ValidationType.None,
                XmlResolver = null       // no network access for external entities
            };

            var doc = new XmlDocument { XmlResolver = null };
            using var reader = XmlReader.Create(path, settings);
            doc.Load(reader);
            return doc;
        }

        private static IEnumerable<XmlNode> SelectChildElements(XmlNode parent, string localName)
        {
            if (parent.ChildNodes == null) yield break;
            foreach (XmlNode child in parent.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element &&
                    string.Equals(child.LocalName, localName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return child;
                }
            }
        }

        private static string? GetAttr(XmlNode node, string name)
            => node.Attributes?[name]?.Value;

        private static bool ParseBool(string? value, bool defaultValue)
        {
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return bool.TryParse(value, out bool r) ? r : defaultValue;
        }

        private static string? NullIfEmpty(string? value)
            => string.IsNullOrEmpty(value) ? null : value;
    }

    // ---------------------------------------------------------------
    // Result type
    // ---------------------------------------------------------------

    /// <summary>
    /// Summary of an <see cref="AnimationConfigConverter.ConvertDirectoryAsync"/> run.
    /// </summary>
    public class AnimationConversionResult
    {
        public bool DryRun { get; init; }
        public int TotalFiles { get; set; }
        public int SuccessCount { get; set; }
        public int SkippedCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> SuccessfulFiles { get; } = new();
        public List<(string File, string Error)> Errors { get; } = new();

        public string GenerateReport()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== Animation Config Conversion Report ===");
            if (DryRun) sb.AppendLine("DRY RUN — no files were written");
            sb.AppendLine($"  XML files scanned : {TotalFiles}");
            sb.AppendLine($"  Converted         : {SuccessCount}");
            sb.AppendLine($"  Skipped (no anim) : {SkippedCount}");
            sb.AppendLine($"  Failed            : {FailureCount}");
            if (Errors.Count > 0)
            {
                sb.AppendLine("  Errors:");
                foreach (var (file, error) in Errors)
                    sb.AppendLine($"    {Path.GetFileName(file)}: {error}");
            }
            return sb.ToString();
        }
    }
}
