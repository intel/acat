
using System;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// JSON serialization utility for ACAT configuration files.
    /// Supports comments and trailing commas in JSON for user-friendly editing.
    /// </summary>
    public static class JsonSerializer
    {
        /// <summary>
        /// Default serialization options for writing JSON
        /// </summary>
        private static readonly JsonSerializerOptions _writeOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Default deserialization options for reading JSON
        /// Supports comments and trailing commas for user-friendly manual editing
        /// </summary>
        private static readonly JsonSerializerOptions _readOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,  // Allow // and /* */ comments
            AllowTrailingCommas = true,                       // Allow trailing commas
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Serializes an object to JSON string
        /// </summary>
        public static string Serialize<TValue>(TValue message)
        {
            var res = System.Text.Json.JsonSerializer.Serialize(message, _writeOptions);

            if (string.IsNullOrEmpty(res))
            {
                throw new InvalidOperationException("Serialization Failed.");
            }

            return res;
        }

        /// <summary>
        /// Deserializes JSON string to an object.
        /// Supports comments (//, /* */) and trailing commas for user convenience.
        /// Note: Comments are not preserved when re-serializing.
        /// </summary>
        public static TValue Deserialize<TValue>(string json)
        {
            var res = System.Text.Json.JsonSerializer.Deserialize<TValue>(json, _readOptions);
            Debug.Assert(res != null);
            if (res is not TValue)
            {
                throw new InvalidOperationException("Deserialization Failed.");
            }

            return res;
        }
    }
}