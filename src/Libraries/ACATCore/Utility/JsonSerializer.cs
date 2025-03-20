using System;

namespace ACAT.Lib.Core.Utility
{
    public static class JsonSerializer
    {
        public static string Serialize<TValue>(TValue message)
        {
            return System.Text.Json.JsonSerializer.Serialize<TValue>(message);

        }

        public static TValue Deserialize<TValue>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TValue>(json);
        }

    }
}
