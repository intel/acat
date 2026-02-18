////////////////////////////////////////////////////////////////////////////
//
// Copyright 2013-2019; 2023 Intel Corporation
// SPDX-License-Identifier: Apache-2.0
//
//
// JsonSchemaValidator.cs
//
// Validates JSON configuration files against JSON Schema definitions
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ACAT.Core.Configuration
{
    /// <summary>
    /// Validates JSON configuration files against JSON Schema definitions
    /// </summary>
    public class JsonSchemaValidator
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, JsonDocument> _schemas;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger instance (optional)</param>
        public JsonSchemaValidator(ILogger logger = null)
        {
            _logger = logger ?? Utility.LoggingConfiguration.CreateLogger<JsonSchemaValidator>();
            _schemas = new Dictionary<string, JsonDocument>();
        }

        /// <summary>
        /// Load a JSON schema from file
        /// </summary>
        /// <param name="schemaName">Name identifier for the schema</param>
        /// <param name="schemaFilePath">Path to the JSON schema file</param>
        /// <returns>True if schema loaded successfully</returns>
        public bool LoadSchema(string schemaName, string schemaFilePath)
        {
            try
            {
                if (string.IsNullOrEmpty(schemaName))
                {
                    _logger?.LogError("Schema name cannot be null or empty");
                    return false;
                }

                if (string.IsNullOrEmpty(schemaFilePath) || !File.Exists(schemaFilePath))
                {
                    _logger?.LogError("Schema file not found: {FilePath}", schemaFilePath);
                    return false;
                }

                string schemaContent = File.ReadAllText(schemaFilePath);
                JsonDocument schemaDoc = JsonDocument.Parse(schemaContent);
                
                _schemas[schemaName] = schemaDoc;
                _logger?.LogInformation("Loaded JSON schema: {SchemaName} from {FilePath}", schemaName, schemaFilePath);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading JSON schema: {SchemaName} from {FilePath}", schemaName, schemaFilePath);
                return false;
            }
        }

        /// <summary>
        /// Validate a JSON configuration file against a schema
        /// </summary>
        /// <param name="schemaName">Name of the schema to validate against</param>
        /// <param name="jsonFilePath">Path to JSON configuration file</param>
        /// <param name="errors">List of validation errors (if any)</param>
        /// <returns>True if validation passes</returns>
        public bool Validate(string schemaName, string jsonFilePath, out List<string> errors)
        {
            errors = new List<string>();

            try
            {
                if (!_schemas.ContainsKey(schemaName))
                {
                    errors.Add($"Schema not loaded: {schemaName}");
                    _logger?.LogError("Schema not loaded: {SchemaName}", schemaName);
                    return false;
                }

                if (string.IsNullOrEmpty(jsonFilePath) || !File.Exists(jsonFilePath))
                {
                    errors.Add($"Configuration file not found: {jsonFilePath}");
                    _logger?.LogError("Configuration file not found: {FilePath}", jsonFilePath);
                    return false;
                }

                string jsonContent = File.ReadAllText(jsonFilePath);
                
                // Basic validation: check if JSON is well-formed
                try
                {
                    using (JsonDocument jsonDoc = JsonDocument.Parse(jsonContent))
                    {
                        // Perform basic schema validation checks
                        JsonElement root = jsonDoc.RootElement;
                        JsonElement schema = _schemas[schemaName].RootElement;

                        bool isValid = ValidateElement(root, schema, "", errors);

                        if (isValid)
                        {
                            _logger?.LogDebug("Validation passed for {FilePath} against schema {SchemaName}", jsonFilePath, schemaName);
                        }
                        else
                        {
                            _logger?.LogWarning("Validation failed for {FilePath} against schema {SchemaName}. Errors: {ErrorCount}", 
                                jsonFilePath, schemaName, errors.Count);
                        }

                        return isValid;
                    }
                }
                catch (JsonException ex)
                {
                    errors.Add($"Invalid JSON format: {ex.Message}");
                    _logger?.LogError(ex, "JSON parsing error in {FilePath}", jsonFilePath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Validation error: {ex.Message}");
                _logger?.LogError(ex, "Error validating {FilePath} against schema {SchemaName}", jsonFilePath, schemaName);
                return false;
            }
        }

        /// <summary>
        /// Validate a JSON string against a schema
        /// </summary>
        /// <param name="schemaName">Name of the schema to validate against</param>
        /// <param name="jsonContent">JSON content as string</param>
        /// <param name="errors">List of validation errors (if any)</param>
        /// <returns>True if validation passes</returns>
        public bool ValidateContent(string schemaName, string jsonContent, out List<string> errors)
        {
            errors = new List<string>();

            try
            {
                if (!_schemas.ContainsKey(schemaName))
                {
                    errors.Add($"Schema not loaded: {schemaName}");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    errors.Add("JSON content is empty");
                    return false;
                }

                using (JsonDocument jsonDoc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = jsonDoc.RootElement;
                    JsonElement schema = _schemas[schemaName].RootElement;
                    
                    return ValidateElement(root, schema, "", errors);
                }
            }
            catch (JsonException ex)
            {
                errors.Add($"Invalid JSON format: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                errors.Add($"Validation error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validate a JSON element against a schema element (basic implementation)
        /// </summary>
        private bool ValidateElement(JsonElement element, JsonElement schema, string path, List<string> errors)
        {
            bool isValid = true;

            // Check for required properties
            if (schema.TryGetProperty("required", out JsonElement required) && required.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement requiredProp in required.EnumerateArray())
                {
                    string propName = requiredProp.GetString();
                    if (!element.TryGetProperty(propName, out _))
                    {
                        errors.Add($"Missing required property '{propName}' at {path}");
                        isValid = false;
                    }
                }
            }

            // Check property types
            if (schema.TryGetProperty("properties", out JsonElement properties))
            {
                foreach (JsonProperty schemaProp in properties.EnumerateObject())
                {
                    if (element.TryGetProperty(schemaProp.Name, out JsonElement elementProp))
                    {
                        if (schemaProp.Value.TryGetProperty("type", out JsonElement typeElement))
                        {
                            string expectedType = typeElement.GetString();
                            if (!ValidateType(elementProp, expectedType, $"{path}.{schemaProp.Name}", errors))
                            {
                                isValid = false;
                            }
                        }
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validate element type matches expected type
        /// </summary>
        private bool ValidateType(JsonElement element, string expectedType, string path, List<string> errors)
        {
            switch (expectedType.ToLower())
            {
                case "string":
                    if (element.ValueKind != JsonValueKind.String)
                    {
                        errors.Add($"Expected string at {path}, but got {element.ValueKind}");
                        return false;
                    }
                    break;
                case "number":
                case "integer":
                    if (element.ValueKind != JsonValueKind.Number)
                    {
                        errors.Add($"Expected number at {path}, but got {element.ValueKind}");
                        return false;
                    }
                    break;
                case "boolean":
                    if (element.ValueKind != JsonValueKind.True && element.ValueKind != JsonValueKind.False)
                    {
                        errors.Add($"Expected boolean at {path}, but got {element.ValueKind}");
                        return false;
                    }
                    break;
                case "array":
                    if (element.ValueKind != JsonValueKind.Array)
                    {
                        errors.Add($"Expected array at {path}, but got {element.ValueKind}");
                        return false;
                    }
                    break;
                case "object":
                    if (element.ValueKind != JsonValueKind.Object)
                    {
                        errors.Add($"Expected object at {path}, but got {element.ValueKind}");
                        return false;
                    }
                    break;
            }

            return true;
        }

        /// <summary>
        /// Dispose of loaded schemas
        /// </summary>
        public void Dispose()
        {
            foreach (var schema in _schemas.Values)
            {
                schema?.Dispose();
            }
            _schemas.Clear();
        }
    }
}
