
using System;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ACAT.Core.Utility
{
    /// <summary>
    /// JSON serialization utility for ACAT configuration files.
    /// Supports comments and trailing commas in JSON for user-friendly editing.
    /// 
    /// ⚠️ CRITICAL: Choose the correct serialization method based on your use case:
    /// 
    /// ✅ Use Serialize() for:
    ///    - ACAT configuration files (internal use)
    ///    - User preferences
    ///    - Application settings
    ///    - Any JSON that stays within ACAT
    ///    
    /// ✅ Use SerializeForInterop() for:
    ///    - External processes (ConvAssist, Python, etc.)
    ///    - Named pipes / Socket communication
    ///    - REST APIs / HTTP requests
    ///    - Any JSON sent to non-.NET applications
    ///    
    /// ⚠️ Why this matters:
    ///    Serialize() uses camelCase (messageType) - wrong for external apps expecting PascalCase (MessageType)
    ///    SerializeForInterop() preserves property names exactly as written in C# classes
    ///    
    /// 💡 When in doubt: Use SerializeForInterop() - it's safer for compatibility
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
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
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
        /// Serialization options for interop scenarios (named pipes, external apps)
        /// Uses PascalCase to match C# property names exactly - no transformation
        /// </summary>
        private static readonly JsonSerializerOptions _interopWriteOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = null,  // Use property names as-is (PascalCase)
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };

        /// <summary>
        /// Serializes an object to JSON string using camelCase property naming.
        /// 
        /// ⚠️ FOR INTERNAL ACAT CONFIGURATION FILES ONLY
        /// 
        /// This method converts property names to camelCase:
        ///   C# property: MessageType → JSON: "messageType"
        ///   C# property: PredictionType → JSON: "predictionType"
        ///   
        /// ❌ DO NOT USE FOR:
        ///   - External process communication (use SerializeForInterop instead)
        ///   - Named pipes to non-.NET apps
        ///   - Socket communication
        ///   - REST APIs
        ///   
        /// ✅ USE FOR:
        ///   - ACAT configuration files
        ///   - User preferences
        ///   - Internal settings that stay within ACAT
        ///   
        /// Example:
        ///   var config = new MyConfig { MaxRetries = 5 };
        ///   string json = JsonSerializer.Serialize(config);
        ///   // Result: { "maxRetries": 5 }
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
            TValue res = System.Text.Json.JsonSerializer.Deserialize<TValue>(json, _readOptions);
            Debug.Assert(res != null);
            if (res is not TValue)
            {
                throw new InvalidOperationException("Deserialization Failed.");
            }

            return res;
        }

        /// <summary>
        /// Serializes an object to JSON string preserving exact property names (PascalCase).
        /// 
        /// ✅ FOR EXTERNAL COMMUNICATION - ALWAYS SAFE TO USE
        /// 
        /// This method preserves property names exactly as written in C#:
        ///   C# property: MessageType → JSON: "MessageType"
        ///   C# property: PredictionType → JSON: "PredictionType"
        ///   
        /// ✅ USE FOR:
        ///   - Named pipes to external processes (Python, Node.js, etc.)
        ///   - Socket communication
        ///   - REST APIs / HTTP requests
        ///   - Any JSON sent outside of ACAT
        ///   - Interop scenarios where external code expects exact property names
        ///   
        /// ✅ ALSO SAFE FOR:
        ///   - Internal ACAT configuration (will work, just less conventional)
        ///   
        /// Example:
        ///   var message = new ConvAssistMessage { MessageType = 4, PredictionType = 1 };
        ///   string json = JsonSerializer.SerializeForInterop(message);
        ///   // Result: { "MessageType": 4, "PredictionType": 1 }
        ///   // Python/Node.js can access properties by their exact C# names
        ///   
        /// 💡 This is the ConvAssist bug fix - sentence predictions failed because
        ///    Python expected "MessageType" but received "messageType" from Serialize()
        /// </summary>
        public static string SerializeForInterop<TValue>(TValue message)
        {
            var res = System.Text.Json.JsonSerializer.Serialize(message, _interopWriteOptions);

            if (string.IsNullOrEmpty(res))
            {
                throw new InvalidOperationException("Serialization Failed.");
            }

            return res;
        }

        /// <summary>
        /// Deserializes JSON string to an object using case-insensitive property matching.
        /// Used for interop scenarios where property names might vary in casing.
        /// </summary>
        public static TValue DeserializeForInterop<TValue>(string json)
        {
            // Reuse _readOptions which already has PropertyNameCaseInsensitive = true
            return Deserialize<TValue>(json);
        }
    }
}