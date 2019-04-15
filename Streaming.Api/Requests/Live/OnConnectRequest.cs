using Newtonsoft.Json;
using System;
using System.Net;

namespace Streaming.Api.Requests.Live
{
    public class OnConnectRequest
    {
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "ip")]
        public string Ip { get; set; }

        [JsonProperty(PropertyName = "vhost")]
        public string VHost { get; set; }

        [JsonProperty(PropertyName = "app")]
        public string App { get; set; }

        [JsonProperty(PropertyName = "tcUrl")]
        public Uri TcUrl { get; set; }

        [JsonProperty(PropertyName = "pageUrl")]
        public Uri PageUrl { get; set; }

        [JsonProperty(PropertyName = "stream_key")]
        public string StreamKey { get; set; }

        [JsonProperty(PropertyName = "httpUrl")]
        public Uri HttpUrl { get; set; }
    }
}
