using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Tests.Infrastructure;

// Reads the API's JSON: camelCase, enums as strings (DT-00-R5)
public static class Json
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };
}
