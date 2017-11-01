using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chan.Net.JsonModel
{
    public class BoardListModel
    {
        public List<BoardInfo> boards;
    }

    public class BoardInfo
    {
        [JsonProperty("board")]
        public string ShortName { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("ws_board")]
        public bool Worksafe { get; set; }

        [JsonProperty("per_page")]
        public int ThreadsPerPage { get; set; }

        [JsonProperty("pages")]
        public int PagesCount { get; set; }

        [JsonProperty("max_filesize")]
        public int MaxFilesize { get; set; }

        [JsonProperty("max_webm_filesize")]
        public int MaxWebMFilesize { get; set; }

        [JsonProperty("max_comment_chars")]
        public int MaxCommentLength { get; set; }

        [JsonProperty("bump_limit")]
        public int BumpLimit { get; set; }

        [JsonProperty("cooldowns")]
        public CooldownInfo Cooldowns { get; set; }

        [JsonProperty("spoilers")]
        public bool SpoilersEnabled { get; set; }

        [JsonProperty("custom_spoilers")]
        public int CustomSpoilersCount { get; set; }

        [JsonProperty("user_ids")]
        public bool UserIdsEnabled { get; set; }

        [JsonProperty("code_tags")]
        public bool CodeTagsEnabled { get; set; }

        [JsonProperty("country_flags")]
        public bool CountryFlagsEnabled { get; set; }

        [JsonProperty("math_tags")]
        public bool MathTagsEnabled { get; set; }
    }

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