using Newtonsoft.Json;
using System.Collections.Generic;

namespace Chan.Net.JsonModel
{
    public class BoardListModel
    {
        [JsonProperty("boards")]
        public List<BoardInfo> Boards;
    }
}
