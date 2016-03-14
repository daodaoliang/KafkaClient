using System;
using System.Collections.Generic;
using System.Text;
using RdKafka;
using TestConfig = KafkaClient.Config;

namespace RdKafkaTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var producer = new Producer("localhost:9092"))
            using (var topic = producer.Topic("test"))
            {
                var data = Encoding.UTF8.GetBytes("Hello Kafka");
                var deliveryReport = topic.Produce(data).Result;
                Console.WriteLine($"Produced to Partition: {deliveryReport.Partition}, Offset: {deliveryReport.Offset}");
            }

            var config = new Config {GroupId = "test-consumer"};
            using (var consumer = new EventConsumer(config, "localhost:9092"))
            {
                consumer.OnMessage += (obj, msg) =>
                {
                    var text = Encoding.UTF8.GetString(msg.Payload, 0, msg.Payload.Length);
                    Console.WriteLine($"Topic: {msg.Topic} Partition: {msg.Partition} Offset: {msg.Offset} {text}");
                };
                
                consumer.Subscribe(new List<string> {"test"});
                consumer.Start();

                Console.ReadLine();
            }
        }
    }
}
