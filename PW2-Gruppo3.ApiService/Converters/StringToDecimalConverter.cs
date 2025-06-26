using PW2_Gruppo3.DataGenerator;
using PW2_Gruppo3.Models;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PW2_Gruppo3.ApiService.Converters
{
    public class StringToDecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();

                if (stringValue is null)
                    return 0m;

                string normalizedString = stringValue.Replace(',', '.');

                if (decimal.TryParse(normalizedString, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue))
                {
                    return decimalValue;
                }

                return 0m;

            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                // Se arriva come numero, leggilo direttamente come int
                return reader.GetDecimal();
            }

            // Se il tipo di token non è né stringa né numero, è un errore inaspettato.
            throw new JsonException($"Unexpected token type {reader.TokenType} for decimal conversion.");

        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
