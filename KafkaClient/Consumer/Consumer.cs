using System;
using System.Collections.Generic;
using KafkaClient;
using log4net;
using Newtonsoft.Json;

namespace Consumer
{
    public enum Format
    {
        Json,
        Avro,
        Binary
    }

    public class Consumer
    {
        private readonly ILog log = LogManager.GetLogger("Consumer");

        private string host;
        private string groupName;
        private Format format;

        private string instanceId;
        private string baseUri;

        public Consumer(string host, string groupName, string instanceId, Format format, string offset = "smallest")
        {
            this.host = host;
            this.groupName = groupName;
            this.instanceId = instanceId;
            this.format = format;

            var contentType = "application/vnd.kafka.v1+json";
            var data = new Dictionary<string, string>
            {
                {"name", instanceId},
                {"format", format.ToString().ToLower()},
                {"auto.offset.reset", offset}
            };
            try
            {
                var responseString = WebRequestHelper.Post(host, $"consumers/{groupName}", JsonConvert.SerializeObject(data),
                    contentType);
                dynamic response = JsonConvert.DeserializeObject<dynamic>(responseString);
                this.instanceId = response.instance_id.ToObject<string>();
                baseUri = response.base_uri.ToObject<string>();
            }
            catch (Exception e)
            {
                // it means, that consumer with this name already exists
                log.Warn(e.Message);
                baseUri = $"{host}/consumers/{groupName}/instances/{instanceId}";
            }
        }

        public string GetMessages(string topic)
        {
            var address = $"consumers/{groupName}/instances/{instanceId}/topics/{topic}";
            var accept = $"application/vnd.kafka.{format.ToString().ToLower()}.v1+json";
            var response = WebRequestHelper.Get(host, address, accept);
            return response;
        }

        public void Delete()
        {
            WebRequestHelper.Delete(baseUri);
        }
    }
}
