using System;
using java.util;
using org.apache.kafka.clients.consumer;
using org.apache.kafka.common.serialization;
using System.Collections.Generic;

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
            prop.put("bootstrap.servers", host);
            prop.put("group.id", "test3");
            prop.put("auto.offset.reset", "earliest");
            prop.put("enable.auto.commit", "true");
            prop.put("auto.commit.interval.ms", "1000");
            prop.put("socket.receive.buffer.bytes", (2*1024*1024).ToString());
            prop.put("fetch.message.max.bytes", (1024*1024).ToString());

            var c = new KafkaConsumer(prop, new ByteArrayDeserializer(), new ByteArrayDeserializer());
            
            var topics = new ArrayList(1);
            topics.add(topic);
            var time = DateTime.UtcNow;
            c.subscribe(topics);
            var bytes = 0;
            var i = count;
            var recordCount = 0;
            while (i > 0)
            {
                var r = c.poll(1000);
                var records = r.records(topic);
                for (var it = records.iterator(); it.hasNext() && i > 0; i--, recordCount++)
                {
                    var rec = (ConsumerRecord)it.next();
                    var b = (byte[]) rec.value();
                    bytes += b.Length;
                }
                Console.WriteLine(recordCount);
            }
            var mb = bytes / 1024.0 / 1024.0;
            var seconds = (DateTime.UtcNow - time).TotalSeconds;
            Console.WriteLine($"{mb / seconds} MB/sec");
            Console.WriteLine($"{count / seconds} Msg/sec");
        }
    }
}
