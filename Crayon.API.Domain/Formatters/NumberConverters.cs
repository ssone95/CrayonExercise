using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Crayon.API.Domain.Formatters;

public abstract class CustomDecimalConverter<T> : JsonConverter<T>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert switch
        {
            { } when typeToConvert == typeof(decimal) => true,
            { } when typeToConvert == typeof(double) => true,
            _ => false
        };
    }
}

public class DoubleLimitDecimalPlacesConverter : CustomDecimalConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(value.ToString("F2", CultureInfo.InvariantCulture));
    }
}

