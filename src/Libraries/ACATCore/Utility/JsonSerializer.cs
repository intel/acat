
using System;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace ACAT.Core.Utility
{
    public static class JsonSerializer
    {
        public static string Serialize<TValue>(TValue message)
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            var res = System.Text.Json.JsonSerializer.Serialize(message, options);

            if (string.IsNullOrEmpty(res))
            {
                throw new InvalidOperationException("Serialization Failed.");
            }

            return res;
        }

        public static TValue Deserialize<TValue>(string json)
        {
            var res = System.Text.Json.JsonSerializer.Deserialize<TValue>(json);
            Debug.Assert(res != null);
            if (res is not TValue)
            {
                throw new InvalidOperationException("Deserialization Failed.");
            }

            return res;
        }
    }
}