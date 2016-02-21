using System;
using Newtonsoft.Json;

namespace KafkaClient
{
    public class XTask
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public DateTime Timestamp { get; set; }
    }
}
