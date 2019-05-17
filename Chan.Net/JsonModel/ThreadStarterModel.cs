using Newtonsoft.Json;
using System.Collections.Generic;

namespace Chan.Net.JsonModel
{
    public class ThreadStarterModel
    {
        [JsonProperty("no")]
        public int No { get; set; }

        [JsonProperty("sticky")]
        public int Sticky { get; set; }

        [JsonProperty("closed")]
        public int Closed { get; set; }

        [JsonProperty("now")]
        public string Now { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("com")]
        public string Com { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("ext")]
        public string Ext { get; set; }

        [JsonProperty("w")]
        public int W { get; set; }

        [JsonProperty("h")]
        public int H { get; set; }

        [JsonProperty("tn_w")]
        public int TnW { get; set; }

        [JsonProperty("tn_h")]
        public int TnH { get; set; }

        [JsonProperty("tim")]
        public object Tim { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("md5")]
        public string MD5 { get; set; }

        [JsonProperty("fsize")]
        public int Fsize { get; set; }

        [JsonProperty("resto")]
        public int Resto { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("semantic_url")]
        public string SemanticUrl { get; set; }

        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        [JsonProperty("replies")]
        public int Replies { get; set; }

        [JsonProperty("images")]
        public int Images { get; set; }

        [JsonProperty("last_modified")]
        public int LastModified { get; set; }

        [JsonProperty("bumplimit")]
        public int? Bumplimit { get; set; }

        [JsonProperty("imagelimit")]
        public int? Imagelimit { get; set; }

        [JsonProperty("last_replies")]
        public List<LastReplyModel> LastReplies { get; set; }

        [JsonProperty("omitted_posts")]
        public int? OmittedPosts { get; set; }

        [JsonProperty("omitted_images")]
        public int? OmittedImages { get; set; }

        [JsonProperty("m_img")]
        public int? MImg { get; set; }

        [JsonProperty("trip")]
        public string Trip { get; set; }
    }
}
