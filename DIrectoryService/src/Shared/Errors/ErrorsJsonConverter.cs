using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shared.Errors;

public sealed class ErrorsJsonConverter : JsonConverter<Errors>
{
    public override Errors Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        List<Error>? errors = JsonSerializer.Deserialize<List<Error>>(ref reader, options);

        return new Errors(errors ?? []);
    }

    public override void Write(Utf8JsonWriter writer, Errors value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.ToList(), options);
    }
}
