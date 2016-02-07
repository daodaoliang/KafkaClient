using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KafkaClient
{
	public enum Format
	{
		Json,
		Avro,
		Binary
	}

	public class Consumer
	{
		private string host;
		private string name;
		private Format format;

		private string instanceId;
		private string baseUri;

		public Consumer(string host, string name, Format format, string offset = "smallest")
		{
			this.host = host;
			this.name = name;
			this.format = format;

			var contentType = "application/vnd.kafka.v1+json";
			var data = new Dictionary<string, string>
			{
				{"name", name},
				{"format", format.ToString().ToLower()},
				{"auto.offset.reset", offset}
			};
			var responseString = WebRequestHelper.Post(host, $"consumers/{name}", JsonConvert.SerializeObject(data), contentType);
			dynamic response = JsonConvert.DeserializeObject<dynamic>(responseString);
			instanceId = response.instance_id.ToObject<string>();
			baseUri = response.base_uri.ToObject<string>();
			Console.WriteLine(baseUri);
		}

		public string GetMessages(string topic)
		{
			var address = $"consumers/{name}/instances/{instanceId}/topics/{topic}";
			var accept = $"application/vnd.kafka.{format.ToString().ToLower()}.v1+json";
			var response = WebRequestHelper.Get(host, address, accept);
			return response;
		}

		~Consumer()
		{
			WebRequestHelper.Delete(baseUri);
		}

		public void Delete()
		{
			WebRequestHelper.Delete(baseUri);
		}
	}
}
