using Newtonsoft.Json;
using System.IO;

namespace Chan.Net.JsonModel
{
    public static class JsonDeserializer
    {
        public static T Deserialize<T>(Stream json)
        {
            using (var sr = new StreamReader(json))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var ser = new JsonSerializer();

                return ser.Deserialize<T>(reader);
            }
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
