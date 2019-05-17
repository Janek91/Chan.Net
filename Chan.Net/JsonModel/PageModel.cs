using Newtonsoft.Json;
using System.Collections.Generic;

namespace Chan.Net.JsonModel
{
    public class PageModel
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("threads")]
        public List<ThreadStarterModel> Threads { get; set; }
    }
}
