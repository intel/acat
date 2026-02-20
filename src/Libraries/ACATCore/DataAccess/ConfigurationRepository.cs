////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// ConfigurationRepository.cs
//
// Repository for JSON-based configuration objects.
// Uses System.Text.Json for serialization.
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;

namespace ACAT.Core.DataAccess
{
    /// <summary>
    /// Repository for JSON-based configuration entities.
    /// Uses <see cref="System.Text.Json.JsonSerializer"/> for serialization,
    /// consistent with the JSON configuration classes in ACAT.Core.Configuration.
    /// </summary>
    /// <typeparam name="T">
    /// Configuration type – must be a reference type with a public
    /// parameterless constructor.
    /// </typeparam>
    public class ConfigurationRepository<T> : RepositoryBase<T> where T : class, new()
    {
        private static readonly JsonSerializerOptions _readOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        private static readonly JsonSerializerOptions _writeOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationRepository{T}"/>.
        /// </summary>
        /// <param name="logger">Optional logger.</param>
        public ConfigurationRepository(ILogger logger = null) : base(logger) { }

        /// <summary>
        /// Loads a configuration entity from the JSON file at <paramref name="filePath"/>.
        /// Returns a default instance when the file is absent or unreadable.
        /// </summary>
        public override T Load(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogWarning("ConfigurationRepository.Load called with null/empty path");
                return null;
            }

            if (!File.Exists(filePath))
            {
                Logger.LogWarning("Configuration file not found: {FilePath} – returning defaults", filePath);
                return new T();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                T result = JsonSerializer.Deserialize<T>(json, _readOptions);

                if (result == null)
                {
                    Logger.LogWarning("Deserialization returned null for {FilePath} – returning defaults", filePath);
                    return new T();
                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ConfigurationRepository failed to load {FilePath} – returning defaults", filePath);
                return new T();
            }
        }

        /// <summary>
        /// Saves <paramref name="entity"/> as JSON to the file at <paramref name="filePath"/>.
        /// </summary>
        public override bool Save(T entity, string filePath)
        {
            if (entity == null)
            {
                Logger.LogError("ConfigurationRepository.Save: entity is null");
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogError("ConfigurationRepository.Save: filePath is null/empty");
                return false;
            }

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonSerializer.Serialize(entity, _writeOptions);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ConfigurationRepository failed to save to {FilePath}", filePath);
                return false;
            }
        }

        /// <inheritdoc/>
        public override T GetDefault() => new T();
    }
}
