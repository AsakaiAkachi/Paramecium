using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Paramecium.Utils
{
    // データのJSON形式でのインポートとエクスポートを行うためのクラス
    public static class JsonImportAndExport
    {
        private static JsonSerializerOptions _jsonSerializerOptions;

        static JsonImportAndExport()
        {
            _jsonSerializerOptions = new JsonSerializerOptions()
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
        }

        public static T? JsonImport<T>(string jsonText)
        {
            try
            {
                T? data = JsonSerializer.Deserialize<T>(jsonText, _jsonSerializerOptions);
                return data;
            }
            catch { return default; }
        }
        public static string JsonExport(object data)
        {
            return JsonSerializer.Serialize(data, _jsonSerializerOptions);
        }

        public static T? FileImport<T>(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
            T? data = JsonImport<T>(streamReader.ReadToEnd());
            streamReader.Close();
            return data;
        }
        public static void FileExport(string filePath, object data)
        {
            StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.UTF8);
            streamWriter.Write(JsonExport(data));
            streamWriter.Close();
        }
    }
}
