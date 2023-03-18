using System.Text.Json;
using System.Text.Json.Serialization;

namespace VinylEye.Core.Helpers;


public static class JsonHelper
{
    public static JsonSerializerOptions DefaultJsonSerializerOptions => new()
    {
        NumberHandling =
            JsonNumberHandling.AllowReadingFromString |
            JsonNumberHandling.WriteAsString,
        WriteIndented = true
    };

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, DefaultJsonSerializerOptions);
    }

    public static T? DeSerialize<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains('{'))
            return default;
        try
        {
            return JsonSerializer.Deserialize<T>(value, DefaultJsonSerializerOptions);
        }
        catch
        {
            Console.WriteLine($"Can't deserialise: {value}");
            return default;
        }
    }
}