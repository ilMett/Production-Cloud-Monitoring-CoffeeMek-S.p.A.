using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PW2_Gruppo3.ApiService.Converters
{
    public class StringToBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();

                if (stringValue is null)
                {
                    return false;
                }

                if (bool.TryParse(stringValue, out bool boolValue))
                {
                    return boolValue;
                }

                // Se non è "True" o "False", prova a gestire "0" o "1"
                string normalizedString = stringValue.Trim().ToLowerInvariant();

                if (normalizedString == "0")
                    return false;

                if (normalizedString == "1")
                    return true;

                return false;
            }

            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                return reader.GetBoolean();

            // Se il tipo di token non è né stringa né bool, è un errore inaspettato.
            throw new JsonException($"Unexpected token type {reader.TokenType} for bool conversion.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
