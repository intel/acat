namespace ACAT.Core.Utility
{
    public static class JsonSerializer
    {
        public static string Serialize<TValue>(TValue message)
        {
            return JsonSerializer.Serialize(message);
        }

        public static TValue Deserialize<TValue>(string json)
        {
            return JsonSerializer.Deserialize<TValue>(json);
        }
    }
}