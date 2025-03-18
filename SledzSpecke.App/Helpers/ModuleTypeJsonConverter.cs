using System.Text.Json;
using System.Text.Json.Serialization;
using SledzSpecke.App.Models.Enums;

namespace SledzSpecke.App.Helpers
{
    public class ModuleTypeJsonConverter : JsonConverter<ModuleType>
    {
        public override ModuleType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                return ModuleType.Specialistic;
            }

            return value.Equals("Basic", StringComparison.OrdinalIgnoreCase) ? ModuleType.Basic : ModuleType.Specialistic;
        }

        public override void Write(Utf8JsonWriter writer, ModuleType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}