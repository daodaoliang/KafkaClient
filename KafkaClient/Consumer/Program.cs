using System;
using KafkaClient;
using log4net;
using Newtonsoft.Json;
using KafkaClient.Logging;

namespace Consumer
{
    public class Program
    {
        public static ILog Log = LogManager.GetLogger("Consumer");

        public static void Handle(XTask task)
        {
            Log.Info($"Got {task.Name} Latency: {(DateTime.Now - task.Timestamp).TotalSeconds}");
        }

        public static void Main(string[] args)
        {
            Log4NetConfigurator.Configure();

            var consumer = new Consumer(Config.HostName, "TestConsumer", "1", Format.Json);
            while (true)
            {
                var recv = consumer.GetMessages(Config.TopicName);
                var obj = JsonConvert.DeserializeObject<dynamic[]>(recv);
                foreach (var o in obj)
                    Handle(o.value.ToObject<XTask>());
            }
        }
    }
}
