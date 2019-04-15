using Newtonsoft.Json;
using System;
using System.Net;

namespace Streaming.Api.Requests.Live
{
    public class OnUnpublishRequest
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

        [JsonProperty(PropertyName = "stream")]
        public Guid StreamId { get; set; }

        [JsonProperty(PropertyName = "param")]
        public string Params { get; set; }
    }
}
