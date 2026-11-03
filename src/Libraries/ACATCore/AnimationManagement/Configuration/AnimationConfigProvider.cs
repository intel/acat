////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
// AnimationConfigProvider.cs
//
// JSON loader implementation of IAnimationConfigProvider.
// Loads {panelName}.animation.json from the given config path.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.AnimationManagement.Interfaces;
using System;
using System.IO;
using System.Text.Json;

namespace ACAT.Core.AnimationManagement.Configuration
{
    /// <summary>
    /// Loads <see cref="AnimationConfig"/> from a JSON file named
    /// <c>{panelName}.animation.json</c> located in <c>configPath</c>.
    /// </summary>
    public class AnimationConfigProvider : IAnimationConfigProvider
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        /// <inheritdoc/>
        public AnimationConfig LoadForPanel(string panelName, string configPath)
        {
            if (string.IsNullOrWhiteSpace(panelName)) throw new ArgumentNullException(nameof(panelName));
            if (string.IsNullOrWhiteSpace(configPath)) throw new ArgumentNullException(nameof(configPath));

            var filePath = GetFilePath(panelName, configPath);
            if (!File.Exists(filePath)) return null;

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<AnimationConfig>(json, _options);
        }

        /// <inheritdoc/>
        public bool HasJsonConfig(string panelName, string configPath)
        {
            if (string.IsNullOrWhiteSpace(panelName) || string.IsNullOrWhiteSpace(configPath))
                return false;

            return File.Exists(GetFilePath(panelName, configPath));
        }

        private static string GetFilePath(string panelName, string configPath)
        {
            return Path.Combine(configPath, panelName + ".animation.json");
        }
    }
}
