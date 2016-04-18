using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace RdKafka
{
    class Program
    {
        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr rd_kafka_conf_new();

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern void rd_kafka_conf_set(IntPtr conf, string name, string value, IntPtr errstr, int errstr_size);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr rd_kafka_new(int type, IntPtr conf, IntPtr errstr, int errstr_size);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern void rd_kafka_brokers_add(IntPtr rk, string host);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr rd_kafka_topic_conf_new();

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr rd_kafka_topic_new(IntPtr rk, string topic, IntPtr conf);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern int rd_kafka_produce(IntPtr rkt, int partition, int msgflags, byte[] payload, int len, byte[] key,
            int keylen, IntPtr msg_opaque);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern int rd_kafka_poll(IntPtr rk, int timeout_ms);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern int rd_kafka_outq_len(IntPtr rk);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern void rd_kafka_topic_destroy(IntPtr rkt);

        [DllImport("librdkafka", CallingConvention = CallingConvention.Cdecl)]
        static extern void rd_kafka_destroy(IntPtr rk);

        static void Main(string[] args)
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

            var partition = -1;
            var errsize = 0;
            var errstr = IntPtr.Zero;
            var conf = rd_kafka_conf_new();
            rd_kafka_conf_set(conf, "batch.num.messages", "100", errstr, errsize);
            rd_kafka_conf_set(conf, "queue.buffering.max.ms", "100", errstr, errsize);
            rd_kafka_conf_set(conf, "queue.buffering.max.messages", "10000000", errstr, errsize);

            var kafka = rd_kafka_new(0, conf, errstr, errsize);
            rd_kafka_brokers_add(kafka, host);
            var topicConf = rd_kafka_topic_conf_new();
            var topic = rd_kafka_topic_new(kafka, topicName, topicConf);

            var t = DateTime.UtcNow;
            var sent = 0;
            while (true)
            {
                if (rd_kafka_produce(topic, partition, 0, payload, size, null, 0, IntPtr.Zero) == -1)
                {
                    rd_kafka_poll(kafka, 10);
                    continue;
                }
                sent++;
                if (sent >= count)
                    break;

                rd_kafka_poll(kafka, 0);
            }

            while (rd_kafka_outq_len(kafka) > 0)
                rd_kafka_poll(kafka, 100);

            var seconds = (DateTime.UtcNow - t).TotalMilliseconds / 1000;
            Console.WriteLine($"{(count * size / (1024.0*1024))/seconds} MB/sec");

            rd_kafka_topic_destroy(topic);
            rd_kafka_destroy(kafka);
        }
    }
}
