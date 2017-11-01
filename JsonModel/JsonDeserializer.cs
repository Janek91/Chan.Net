using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Chan.Net.JsonModel
{
    public static class JsonDeserializer
    {
        public static T Deserialize<T>(Stream json)
        {
            using (StreamReader sr = new StreamReader(json))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer ser = new JsonSerializer();

                return ser.Deserialize<T>(reader);
            }
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}