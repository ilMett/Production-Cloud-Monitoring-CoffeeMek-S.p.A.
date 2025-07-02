using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PW2_Gruppo3.ApiService.Converters
{
    public class StringToIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Controlla se il token corrente nel JSON è una stringa
            if (reader.TokenType == JsonTokenType.String)
            {
                // Ottiene il valore della stringa. GetString() restituirà null se il valore è JSON null.
                string? stringValue = reader.GetString();

                // Tenta di parsare la stringa in un intero.
                // Usiamo CultureInfo.InvariantCulture per assicurarci che la conversione non dipenda dalla cultura locale (es. separatori di migliaia)
                if (int.TryParse(stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                {
                    return intValue; // Se la conversione ha successo, restituisce il valore intero
                }

                // Se la stringa è null, vuota, o non un intero valido
                // 1. Restituire un valore di default (es. 0)
                // 2. Lanciare un'eccezione JsonException con un messaggio più specifico
                // 3. Loggare un avvertimento

                // Opzione 1: Restituire 0 (valore di default per int, robusto per dati sporchi)
                return 0;

                // Opzione 2: Lanciare un'eccezione (più rigoroso)
                // throw new JsonException($"Cannot convert '{stringValue}' to int. Invalid format or null.");
            }

            //  la parte seguente direi che non ci serve, visto che ogni token del nostro messaggio json è una string, giusto?
            // Se il token non è una stringa (es. è già un numero JSON),
            // System.Text.Json di solito lo gestisce correttamente.
            // Puoi lanciare un'eccezione se ti aspetti SOLO stringhe.
            if (reader.TokenType == JsonTokenType.Number)
            {
                // Se arriva come numero, leggilo direttamente come int
                return reader.GetInt32();
            }

            // Se il tipo di token non è né stringa né numero, è un errore inaspettato.
            throw new JsonException($"Unexpected token type {reader.TokenType} for int conversion.");
        }


        //  QUI RE-SERIALIZZA ROBA, QUINDI DIREI CHE NON CI INTERESSA, se non che è richiesto dall'implementazione di JsonConverter

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            // Quando si serializza un int, lo scriviamo come stringa.
            // Questo è utile se il DataGenerator si aspetta indietro stringhe (anche se di solito non è il caso per i response API).
            // Se le tue API di risposta devono avere int come int, puoi rimuovere questo converter.
            // Per il tuo caso d'uso (deserializzazione input), il metodo Write è meno critico.
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
