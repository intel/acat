////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// MigrationBase.cs
//
// Abstract base class for configuration file version migrations.
// Derive from this class to implement a migration between two specific
// configuration versions.
//
////////////////////////////////////////////////////////////////////////////

using ACAT.Core.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ACAT.Core.Utility.Migrations
{
    /// <summary>
    /// Abstract base class for configuration migrations.
    /// Each derived class handles migration from one specific version to the next.
    /// </summary>
    public abstract class MigrationBase : IConfigurationMigration
    {
        /// <summary>
        /// Logger instance for migration operations
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The version this migration upgrades from
        /// </summary>
        public abstract ConfigurationVersion FromVersion { get; }

        /// <summary>
        /// The version this migration upgrades to
        /// </summary>
        public abstract ConfigurationVersion ToVersion { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Optional logger instance</param>
        protected MigrationBase(ILogger logger = null)
        {
            Logger = logger ?? LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Perform the migration. Override ApplyMigration to implement migration logic.
        /// Updates the version field in the result automatically.
        /// </summary>
        /// <param name="source">Source JSON element to migrate</param>
        /// <param name="result">Migrated JSON element</param>
        /// <param name="error">Error message if migration fails</param>
        /// <returns>True if migration succeeded, false otherwise</returns>
        public bool Migrate(JsonElement source, out JsonElement result, out string error)
        {
            Logger?.LogInformation("Applying migration: {FromVersion} -> {ToVersion}", FromVersion, ToVersion);

            try
            {
                bool success = ApplyMigration(source, out result, out error);
                if (success)
                {
                    Logger?.LogInformation("Migration completed: {FromVersion} -> {ToVersion}", FromVersion, ToVersion);
                }
                else
                {
                    Logger?.LogError("Migration failed: {FromVersion} -> {ToVersion}. Error: {Error}",
                        FromVersion, ToVersion, error);
                }
                return success;
            }
            catch (System.Exception ex)
            {
                error = ex.Message;
                result = source;
                Logger?.LogError(ex, "Unexpected error during migration: {FromVersion} -> {ToVersion}",
                    FromVersion, ToVersion);
                return false;
            }
        }

        /// <summary>
        /// Implement migration logic in derived classes.
        /// The result should have its "version" field updated to ToVersion.ToString().
        /// </summary>
        /// <param name="source">Source JSON element</param>
        /// <param name="result">Migrated JSON element</param>
        /// <param name="error">Error message if migration fails</param>
        /// <returns>True if migration succeeded, false otherwise</returns>
        protected abstract bool ApplyMigration(JsonElement source, out JsonElement result, out string error);

        /// <summary>
        /// Helper method to create a new JSON document with updated properties.
        /// Copies all properties from source, adds/updates specified properties,
        /// and sets the version field to ToVersion.
        /// </summary>
        /// <param name="source">Source JSON element</param>
        /// <param name="additionalProperties">Properties to add or update (key=name, value=JSON value string)</param>
        /// <returns>Updated JsonElement</returns>
        protected JsonElement BuildMigratedElement(JsonElement source,
            System.Collections.Generic.Dictionary<string, object> additionalProperties = null)
        {
            var writer = new System.Text.Json.Nodes.JsonObject();

            // Copy all existing properties
            foreach (JsonProperty property in source.EnumerateObject())
            {
                writer[property.Name] = System.Text.Json.Nodes.JsonNode.Parse(property.Value.GetRawText());
            }

            // Apply additional properties
            if (additionalProperties != null)
            {
                foreach (var kvp in additionalProperties)
                {
                    if (kvp.Value == null)
                    {
                        writer[kvp.Key] = null;
                    }
                    else if (kvp.Value is string s)
                    {
                        writer[kvp.Key] = s;
                    }
                    else if (kvp.Value is int i)
                    {
                        writer[kvp.Key] = i;
                    }
                    else if (kvp.Value is bool b)
                    {
                        writer[kvp.Key] = b;
                    }
                    else if (kvp.Value is double d)
                    {
                        writer[kvp.Key] = d;
                    }
                    else
                    {
                        writer[kvp.Key] = System.Text.Json.Nodes.JsonNode.Parse(
                            JsonSerializer.Serialize(kvp.Value));
                    }
                }
            }

            // Always update the version field to the target version
            writer["version"] = ToVersion.ToString();

            return JsonSerializer.Deserialize<JsonElement>(writer.ToJsonString());
        }
    }
}
