using System;
using System.Runtime.InteropServices;
using RdKafka.RdKafka;

namespace RdKafka
{
    [StructLayout(LayoutKind.Sequential)]
    public struct rd_kafka_message
    {
        public int err;
        public IntPtr rkt;
        public int partition;
        public IntPtr payload;
        public int len;
        public IntPtr key;
        public int key_len;
        public long offset;
        public IntPtr _private;
    }

    public class Internal
    {
        private const string DllName = "librdkafka";
        private const CallingConvention Convention = CallingConvention.Cdecl;

        [UnmanagedFunctionPointer(callingConvention: Convention)]
        public delegate void DeliveryReportCallback(IntPtr rk, ref rd_kafka_message rkmessage, IntPtr opaque);

        [UnmanagedFunctionPointer(callingConvention: Convention)]
        public delegate void OpaqueCallback();

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern IntPtr rd_kafka_conf_new();

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern void rd_kafka_conf_set(IntPtr conf, string name, string value, IntPtr errstr, int errstr_size);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern IntPtr rd_kafka_new(RdKafkaType type, IntPtr conf, IntPtr errstr, int errstr_size);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern void rd_kafka_brokers_add(IntPtr rk, string host);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern IntPtr rd_kafka_topic_conf_new();

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern IntPtr rd_kafka_topic_new(IntPtr rk, string topic, IntPtr conf);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern int rd_kafka_produce(IntPtr rkt, int partition, MsgFlags msgflags, byte[] payload, int len, byte[] key,
            int keylen, IntPtr msg_opaque);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern int rd_kafka_poll(IntPtr rk, int timeout_ms);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern int rd_kafka_outq_len(IntPtr rk);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern void rd_kafka_topic_destroy(IntPtr rkt);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern void rd_kafka_destroy(IntPtr rk);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern void rd_kafka_conf_set_dr_msg_cb(IntPtr conf, DeliveryReportCallback dr_cb);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern int rd_kafka_consume_start(IntPtr rkt, int partition, long offset);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern ErrorCode rd_kafka_last_error();

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern string rd_kafka_err2str(ErrorCode err);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern IntPtr rd_kafka_consume(IntPtr rkt, int partition, int timeout_ms);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern void rd_kafka_message_destroy(IntPtr rkmessage);

        [DllImport(DllName, CallingConvention = Convention)]
        public static extern void rd_kafka_consume_stop(IntPtr rkt, int partition);
    }
}
