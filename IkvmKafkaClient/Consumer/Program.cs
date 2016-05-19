using System;
using java.util;
using org.apache.kafka.clients.consumer;
using org.apache.kafka.common.serialization;

namespace Consumer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = "192.168.33.12:9092";
            var topic = "test2";
            var count = 50000000;

            var prop = new Properties();
            prop.setProperty("bootstrap.servers", host);
            prop.setProperty("group.id", "testgroup");
            prop.setProperty("auto.offset.reset", "earliest");
            //prop.setProperty("enable.auto.commit", "true");
            //prop.setProperty("auto.commit.interval.ms", "1000");
            var c = new KafkaConsumer(prop, new ByteArrayDeserializer(), new ByteArrayDeserializer());

            var topics = new ArrayList(1);
            topics.add(topic);
            var time = DateTime.UtcNow;
            c.subscribe(topics);
            var bytes = 0;
            var i = count;
            while (i > 0)
            {
                var r = c.poll(1000);
                var records = r.records(topic);
                while (records.iterator().hasNext() && i > 0)
                {
                    var rec = records.iterator().next();
                    var s = (ConsumerRecord)rec;
                    var b = (byte[])s.value();
                    bytes += b.Length;
                    i--;
                }
            }
            var mb = bytes / 1024.0 / 1024.0;
            var seconds = (DateTime.UtcNow - time).TotalSeconds;
            Console.WriteLine($"{mb / seconds} MB/sec");
            Console.WriteLine($"{count / seconds} Msg/sec");
        }
    }
}
