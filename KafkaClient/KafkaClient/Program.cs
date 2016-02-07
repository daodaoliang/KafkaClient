using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace KafkaClient
{
	public class Program
	{
		private static string host = "192.168.190.129:8082";
		private const string topic = "JsonTestTopic";

//		public static int AddTopic(string name, Type schema)
//		{
//			var jsonSchemaGenerator = new JsonSchemaGenerator();
//			var sch = jsonSchemaGenerator.Generate(schema);
//			var writer = new StringWriter();
//			var jsonTextWriter = new JsonTextWriter(writer);
//			sch.WriteTo(jsonTextWriter);
//			Console.WriteLine(writer.ToString());
//			return 0;
//		}

		public static List<string> GetTopics()
		{
			return JsonConvert.DeserializeObject<List<string>>(WebRequestHelper.Get(host, "topics"));
		}

		public static void Publish(List<XTask> tasks)
		{
			var contentType = "application/vnd.kafka.json.v1+json";
			var accept = "application/vnd.kafka.v1+json, application/vnd.kafka+json, application/json";
			var data = JsonConvert.SerializeObject(new {records = tasks.Select(x => new {value = x})});

			Console.WriteLine(WebRequestHelper.Post(host, $"topics/{topic}", data, contentType, accept));
		}

		public static void Publish(XTask t)
		{
			Publish(new List<XTask> {t});
		}

		public static void Handle(XTask t)
		{
			// Do stuff
			Console.WriteLine($"Got {t.Name}: {t.Description}");
		}

		public static void Main(string[] args)
		{
			if (args.Length > 0)
				host = args[0];

			foreach (var t in GetTopics())
				Console.WriteLine(t);
			// cannot clear topic from here

			var consumer = new Consumer(host, "testJsonConsumer", Format.Json);
			for (int i = 0; i < 10; i++)
			{
				Publish(new XTask {Name = "Task #" + i, Description = ";;"});
				var recv = consumer.GetMessages(topic);
				var obj = JsonConvert.DeserializeObject<dynamic[]>(recv);
				foreach (var o in obj)
					Handle(o.value.ToObject<XTask>());
//				Console.WriteLine(recv);
			}
			consumer.Delete();
		}
	}
}
