using System;
using System.CodeDom;
using System.Threading.Tasks;
using KafkaClient;
using log4net;
using Newtonsoft.Json;
using KafkaClient.Logging;
using Metrics;

namespace Consumer
{
    public class ConsumerEntryPoint
    {
        public static readonly Meter Meter = Metric.Context("Consumer").Meter("HandleTask", Unit.Events);
        public static readonly Histogram Histogram = Metric.Context("Consumer").Histogram("Latency", Unit.Custom("ms"));

        public static ILog Log = LogManager.GetLogger("Consumer");

        public static readonly int ThreadCount = 4;

        public static void Handle(XTask task)
        {
            var ms = (DateTime.UtcNow - task.Timestamp).TotalMilliseconds;
            Log.Info($"Got {task.Name} Latency: {ms} ms.");
            Meter.Mark();
            Histogram.Update((long) ms);
        }

        public static void Consume(string instance)
        {
            var consumer = new Consumer(Config.HostName, Config.ConsumerName, instance, Format.Json);
            while (true)
            {
                string recv;
                try
                {
                    recv = consumer.GetMessages(Config.TopicName);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    continue;
                }

                try
                {
                    var obj = JsonConvert.DeserializeObject<dynamic[]>(recv);
                    foreach (var o in obj)
                        Handle(o.value.ToObject<XTask>());
                }
                catch (Exception e)
                {
                    Log.Error($"Error deserializing messages: {recv}");
                }
            }
        }

        public static void Main(string[] args)
        {
            Log4NetConfigurator.Configure();

            Metric.Config
                .WithHttpEndpoint("http://localhost:1234/metrics/")
                .WithAllCounters()
                .WithInternalMetrics()
                .WithReporting(config => config.WithConsoleReport(TimeSpan.FromSeconds(30)));

            var tasks = new Task[ThreadCount];
            for (int i = 0; i < ThreadCount; i++)
            {
                var i1 = i;
                tasks[i] = Task.Run(() => Consume(i1.ToString()));
            }
            Task.WaitAll(tasks);
        }
    }
}
