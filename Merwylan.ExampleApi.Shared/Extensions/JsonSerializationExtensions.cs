using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Merwylan.ExampleApi.Shared.Extensions
{
    public static class JsonSerializationExtensions
    {
        private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string? Serialize<T>(T obj)
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static string? SerializeCamelCase(this object? obj)
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj, _serializerSettings);
        }

        public static async Task<T> DeserializeAsync<T>(string path)
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task WriteJsonFileAsync(string json, string path)
        {
            await File.WriteAllTextAsync(path, json);
        }
    }
}
