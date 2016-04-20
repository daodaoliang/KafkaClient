using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RdKafka;

namespace TestProducer
{
    public class Program
    {
        public const int RdKafkaPartitionUa = -1;

        public static void Main(string[] args)
        {
            string host;
            string topicName;
            int count;
            int size;
            if (args.Length < 4)
            {
                host = "192.168.33.12:9092";
                topicName = "test";
                count = 10000000;
                size = 100;
            }
            else
            {
                host = args[0];
                topicName = args[1];
                count = int.Parse(args[2]);
                size = int.Parse(args[3]);
            }

            var payload = Encoding.UTF8.GetBytes(string.Join("", Enumerable.Range(0, size).Select(x => 'a')));

            var partition = RdKafkaPartitionUa;
            var errsize = 0;
            var errstr = IntPtr.Zero;
            var conf = Internal.rd_kafka_conf_new();

            var stats = new Stats(count, 5000, Console.WriteLine);

            Internal.rd_kafka_conf_set_dr_msg_cb(conf, (IntPtr rk, ref rd_kafka_message msg, IntPtr opaque) =>
            {
                var gch = GCHandle.FromIntPtr(msg._private);
                var cb = (Internal.OpaqueCallback)gch.Target;
                gch.Free();
                cb();
            });

            Internal.rd_kafka_conf_set(conf, "batch.num.messages", "100", errstr, errsize);
            Internal.rd_kafka_conf_set(conf, "queue.buffering.max.ms", "100", errstr, errsize);
            Internal.rd_kafka_conf_set(conf, "queue.buffering.max.messages", "10000000", errstr, errsize);

            var kafka = Internal.rd_kafka_new(RdKafkaType.Producer, conf, errstr, errsize);
            Internal.rd_kafka_brokers_add(kafka, host);
            var topicConf = Internal.rd_kafka_topic_conf_new();
            var topic = Internal.rd_kafka_topic_new(kafka, topicName, topicConf);

            for (int i = 0; i < count; i++)
            {
                var sendStart = DateTimeExtensions.CurrentTimeMillis();
                Internal.OpaqueCallback cb = () => stats.NextCompletion(sendStart, size, stats)();
                var gch = GCHandle.Alloc(cb);
                if (Internal.rd_kafka_produce(topic, partition, MsgFlags.None, payload, size, null, 0, GCHandle.ToIntPtr(gch)) == -1)
                {
                    Internal.rd_kafka_poll(kafka, 10);
                    gch.Free();
                    continue;
                }
                Internal.rd_kafka_poll(kafka, 0);
            }

            while (Internal.rd_kafka_outq_len(kafka) > 0)
                Internal.rd_kafka_poll(kafka, 100);

            Internal.rd_kafka_topic_destroy(topic);
            Internal.rd_kafka_destroy(kafka);

            stats.PrintTotal();
        }
    }
}
