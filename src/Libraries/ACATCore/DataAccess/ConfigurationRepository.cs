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
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        /// Asynchronously loads a configuration entity from the JSON file at
        /// <paramref name="filePath"/> using non-blocking file I/O.
        /// Returns a default instance when the file is absent or unreadable.
        /// </summary>
        public override async Task<T> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogWarning("ConfigurationRepository.LoadAsync called with null/empty path");
                return null;
            }

            if (!File.Exists(filePath))
            {
                Logger.LogWarning("Configuration file not found: {FilePath} – returning defaults", filePath);
                return new T();
            }

            try
            {
                string json;
                using (var reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    json = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                cancellationToken.ThrowIfCancellationRequested();

                // Note: JsonSerializer.DeserializeAsync is not available on .NET Framework 4.8.1
                // (it was introduced in .NET 5), so deserialization must remain synchronous here.
                T result = JsonSerializer.Deserialize<T>(json, _readOptions);

                if (result == null)
                {
                    Logger.LogWarning("Deserialization returned null for {FilePath} – returning defaults", filePath);
                    return new T();
                }

                return result;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ConfigurationRepository failed to load {FilePath} – returning defaults", filePath);
                return new T();
            }
        }

        /// <summary>
        /// Asynchronously saves <paramref name="entity"/> as JSON to the file at
        /// <paramref name="filePath"/> using non-blocking file I/O.
        /// </summary>
        public override async Task<bool> SaveAsync(T entity, string filePath, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                Logger.LogError("ConfigurationRepository.SaveAsync: entity is null");
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                Logger.LogError("ConfigurationRepository.SaveAsync: filePath is null/empty");
                return false;
            }

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Check cancellation before beginning the synchronous serialization step.
                // Note: JsonSerializer.SerializeAsync is not available on .NET Framework 4.8.1
                // (it was introduced in .NET 5), so serialization must remain synchronous here.
                cancellationToken.ThrowIfCancellationRequested();

                string json = JsonSerializer.Serialize(entity, _writeOptions);

                using (var writer = new StreamWriter(filePath, append: false, Encoding.UTF8))
                {
                    await writer.WriteAsync(json).ConfigureAwait(false);
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ConfigurationRepository failed to save to {FilePath}", filePath);
                return false;
            }
        }
    }
}
