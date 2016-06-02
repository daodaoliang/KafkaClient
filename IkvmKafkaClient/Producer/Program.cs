using System;
using System.Text;
using java.util;
using org.apache.kafka.clients.producer;
using org.apache.kafka.common.serialization;
using Exception = java.lang.Exception;

namespace Producer
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = "192.168.33.12:9092";
            var topic = "test2";
            var count = 50000000;
            var size = 100;

            var prop = new Properties();
            prop.setProperty("bootstrap.servers", "192.168.33.12:9092");
            prop.setProperty("acks", "1");
            prop.setProperty("buffer.memory", "67108864");
            prop.setProperty("batch.size", "8196");
            
            var producer = new KafkaProducer(prop, new ByteArraySerializer(), new ByteArraySerializer());
            var payload = new byte[size];
            for (int i = 0; i < size; i++)
                payload[i] = (byte)'a';

            var record = new ProducerRecord(topic, payload);
            var stats = new Stats(count, 5000, Console.WriteLine);

            for (int i = 0; i < count; i++)
            {
                //var payload = Encoding.UTF8.GetBytes(i.ToString());
                //var record = new ProducerRecord(topic, payload);
                var sendStart = DateTimeExtensions.CurrentTimeMillis();
                var cb = new StatsCallback { Action = stats.NextCompletion(sendStart, payload.Length, stats) };
                producer.send(record, cb);
            }
            producer.close();
            stats.PrintTotal();
        }
    }

    public class StatsCallback : Callback
    {
        public Action Action;

        public void onCompletion(RecordMetadata rm, Exception e)
        {
            Action();
        }
    }
}
