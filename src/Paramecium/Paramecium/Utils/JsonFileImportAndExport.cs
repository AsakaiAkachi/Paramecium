using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paramecium.Utils
{
    public static class JsonFileImportAndExport
    {
        private static JsonSerializerOptions _jsonSerializerOptions;

        static JsonFileImportAndExport()
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
        }

        public static T? Import<T>(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
            T? data = JsonSerializer.Deserialize<T>(streamReader.ReadToEnd(), _jsonSerializerOptions);
            streamReader.Close();

            return data;
        }

        public static void Export(string filePath, object data)
        {
            StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8);
            streamWriter.Write(JsonSerializer.Serialize(data, _jsonSerializerOptions));
            streamWriter.Close();
        }
    }
}
