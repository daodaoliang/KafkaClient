using System;
using System.Runtime.InteropServices;
using RdKafka;
using RdKafka.RdKafka;

namespace TestConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string host;
            string topicName;
            int count;
            if (args.Length < 3)
            {
                host = "192.168.33.12:9092";
                topicName = "test2";
                count = 1000000;
            }
            else
            {
                host = args[0];
                topicName = args[1];
                count = int.Parse(args[2]);
            }

            var totalMessagesRead = 0L;
            var totalBytesRead = 0L;
            var conf = Internal.rd_kafka_conf_new();
            var kafka = Internal.rd_kafka_new(RdKafkaType.Consumer, conf, IntPtr.Zero, 0);
            Internal.rd_kafka_brokers_add(kafka, host);
            var topic_conf = Internal.rd_kafka_topic_conf_new();
            var topic = Internal.rd_kafka_topic_new(kafka, topicName, topic_conf);

            IntPtr metaPtr;
            var err = Internal.rd_kafka_metadata(kafka, false, topic, out metaPtr, 5000);
            if (err != ErrorCode.NoError)
            {
                Console.WriteLine($"Failed to aquire metadata: {Internal.rd_kafka_err2str(Internal.rd_kafka_last_error())}");
                return;
            }

            int partition_cnt =
                Marshal.PtrToStructure<rd_kafka_metadata_topic>(
                    Marshal.PtrToStructure<rd_kafka_metadata>(metaPtr).topics).partition_cnt;
            var start_offset = 0;
            for (int i = 0; i < partition_cnt; i++)
            {
                if (Internal.rd_kafka_consume_start(topic, i, start_offset) == -1)
                {
                    err = Internal.rd_kafka_last_error();
                    Console.WriteLine($"Failed to start consuming: {Internal.rd_kafka_err2str(err)}");
                    if (err == ErrorCode._INVALID_ARG)
                        Console.WriteLine("%% Broker based offset storage requires a group.id, add: -X group.id=yourGroup");
                    return;
                }
            }

            var s = DateTime.UtcNow;
            var recv = 0;
            var bytes = 0;
            while (recv < count)
            {
                Internal.rd_kafka_poll(kafka, 0);
                for (int i = 0; i < partition_cnt; i++)
                {
                    var msgPtr = Internal.rd_kafka_consume(topic, i, 1000);
                    if (msgPtr == IntPtr.Zero) 
                        continue;
                    var msg = Marshal.PtrToStructure<rd_kafka_message>(msgPtr);
                    recv++;
                    bytes += msg.len;
                    //Console.WriteLine($"{recv}: {msg.len}");
                    Internal.rd_kafka_message_destroy(msgPtr);
                }
            }
            var mb = bytes/1024.0/1024;
            var elapsed = (DateTime.UtcNow - s).TotalMilliseconds/1000;
            
            Console.WriteLine($"{(float)(mb/elapsed)} MB/sec");

            for (int i = 0; i < partition_cnt; i++)
                Internal.rd_kafka_consume_stop(topic, i);
            while (Internal.rd_kafka_outq_len(kafka) > 0)
                Internal.rd_kafka_poll(kafka, 10);

            Internal.rd_kafka_topic_destroy(topic);
            Internal.rd_kafka_destroy(kafka);
        }
    }
}
