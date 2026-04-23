
using System.Text.Json;

namespace ACS.Helper
{
    public static class JsonElementExtensions
    {
        public static string? Str(this JsonElement el, string key)
        => el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString()
            : null;

        public static int Int(this JsonElement el, string key)
            => el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number
                ? v.GetInt32()
                : 0;

        public static int? IntN(this JsonElement el, string key)
            => el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number
                ? v.GetInt32()
                : null;

        public static long? LngN(this JsonElement el, string key)
            => el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number
                ? v.GetInt64()
                : null;

        public static double? DblN(this JsonElement el, string key)
            => el.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Number
                ? v.GetDouble()
                : null;

        public static bool? Bool(this JsonElement el, string key)
        {
            if (!el.TryGetProperty(key, out var v)) return null;
            return v.ValueKind switch
            {
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => null
            };
        }
    }
}
