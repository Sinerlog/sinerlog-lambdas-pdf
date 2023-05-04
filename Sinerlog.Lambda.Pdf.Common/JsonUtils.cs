using System.Text.Json;
using System.Text.Json.Serialization;


namespace Sinerlog.FullCommerce.Common
{
    public static class JsonUtils
    {
        public static JsonSerializerOptions GetOptions(bool usePreserve)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
               
                
            };

            if (usePreserve)
                options.ReferenceHandler = ReferenceHandler.Preserve;

            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new DateTimeConverterUsingDateTimeParse());

            return options;
        }

        public static T Deserialize<T>(string content, bool usePreserve = true)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(content, GetOptions(usePreserve));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool TryDeserialize<T>(string content, bool usePreserve = true)
        {
            try
            {
                JsonSerializer.Deserialize<T>(content, GetOptions(usePreserve));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static T Deserialize<T>(this object property)
        {
            var element = (JsonElement)property;
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string Serialize(object content, bool usePreserve = false)
        {
            return JsonSerializer.Serialize(content, GetOptions(usePreserve));
        }
    }

    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }


}