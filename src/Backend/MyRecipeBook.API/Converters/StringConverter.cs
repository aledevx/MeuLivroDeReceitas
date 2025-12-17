using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MyRecipeBook.API.Converters;

public partial class StringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Ponto de interrogação serve para não executar os métodos após o mesmo caso a função GetString() retorne um valor nulo
        var value = reader.GetString()?.Trim();

        if (value is null)
            return null;

        return RemoveExtraWhiteSpaces().Replace(value, " ");

    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
       writer.WriteStringValue(value);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex RemoveExtraWhiteSpaces();
}
