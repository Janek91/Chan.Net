using Newtonsoft.Json;

namespace Chan.Net.JsonModel
{
    public class CooldownInfo
    {
        [JsonProperty("threads")]
        public int Thread { get; set; }

        [JsonProperty("replies")]
        public int Reply { get; set; }

        [JsonProperty("images")]
        public int Images { get; set; }
    }
}