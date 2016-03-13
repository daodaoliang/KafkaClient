using System;
using System.Linq;
using KafkaClient;
using log4net;
using Newtonsoft.Json;

namespace Publisher
{
    public class TestPublisher
    {
        public static readonly ILog Log = LogManager.GetLogger("TestPublisher");

        private static string TopicName;

        public static void Publish(XTask[] tasks, Action action)
        {
            var contentType = "application/vnd.kafka.json.v1+json";
            var accept = "application/vnd.kafka.v1+json, application/vnd.kafka+json, application/json";
            var data = JsonConvert.SerializeObject(new { records = tasks.Select(x => new { value = x }) });

            try
            {
                WebRequestHelper.Post(Config.HostName, $"topics/{TopicName}", data, contentType, accept);
                action();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        public static void Main(string[] args, char c='0')
        {
            TopicName = Config.TopicName;
            var numRecords = 50000000L;
            var recordSize = 100;
            try
            {
                TopicName = args[1];
                numRecords = long.Parse(args[1]);
                recordSize = int.Parse(args[2]);
            }
            catch (Exception)
            {
                Log.Warn($"Default values were used. \n\ttopicName={TopicName}\n\tnumRecords={numRecords}\n\trecordSize={recordSize}");
            }

            
            var tasksPerIteration = 1000;
            var iterationsCount = numRecords/tasksPerIteration;
            var payload = new string(Enumerable.Range(0, recordSize).Select(x => 'A').ToArray());
            var tasks = Enumerable.Range(0, tasksPerIteration).Select(x => new XTask { Description = payload }).ToArray();
            var stats = new Stats(numRecords, 5000);
            for (int i = 0; i < iterationsCount; i++)
            {
                var sendStart = DateTimeExtensions.CurrentTimeMillis();
                //task.Timestamp = DateTime.UtcNow;
                var cb = stats.NextCompletion(sendStart, payload.Length, stats).Times(tasksPerIteration);
                Publish(tasks, cb);
            }
            stats.PrintTotal();
        }
    }

    public static class DateTimeExtensions
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long CurrentTimeMillis()
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }
    }

    public static class Extensions
    {
        public static Action Times(this Action a, int count)
        {
            return () =>
            {
                for (int i = 0; i < count; i++)
                    a();
            };
        }
    }

    public class Stats
    {
        public static readonly ILog Log = LogManager.GetLogger("Stats");

        private readonly long _start;
        private readonly int[] _latencies;
        private readonly int _sampling;
        private readonly long _reportingInterval;

        private long _windowStart;
        private int _iteration;
        private int _index;
        private long _count;
        private long _bytes;
        private int _maxLatency;
        private long _totalLatency;
        private long _windowCount;
        private int _windowMaxLatency;
        private long _windowTotalLatency;
        private long _windowBytes;
        

        public Stats(long numRecords, int reportingInterval)
        {
            _start = DateTimeExtensions.CurrentTimeMillis();
            _windowStart = DateTimeExtensions.CurrentTimeMillis();
            _index = 0;
            _iteration = 0;
            _sampling = (int)(numRecords / Math.Min(numRecords, 500000));
            _latencies = new int[(int)(numRecords / _sampling) + 1];
            _index = 0;
            _maxLatency = 0;
            _totalLatency = 0;
            _windowCount = 0;
            _windowMaxLatency = 0;
            _windowTotalLatency = 0;
            _windowBytes = 0;
            _totalLatency = 0;
            _reportingInterval = reportingInterval;
        }

        public void Record(int iter, int latency, int bytes, long time)
        {
            _count++;
            _bytes += bytes;
            _totalLatency += latency;
            _maxLatency = Math.Max(_maxLatency, latency);
            _windowCount++;
            _windowBytes += bytes;
            _windowTotalLatency += latency;
            _windowMaxLatency = Math.Max(_windowMaxLatency, latency);
            if (iter % _sampling == 0)
            {
                _latencies[_index] = latency;
                _index++;
            }
            /* maybe report the recent perf */
            if (time - _windowStart >= _reportingInterval)
            {
                PrintWindow();
                NewWindow();
            }
        }

        public Action NextCompletion(long start, int bytes, Stats stats)
        {
            Action cb = () =>
            {
                var iter = _iteration;
                long now = DateTimeExtensions.CurrentTimeMillis();
                int latency = (int) (now - start);
                stats.Record(iter, latency, bytes, now);
            };
            //Callback cb = new PerfCallback(_iteration, start, bytes, stats);
            _iteration++;
            return cb;
        }

        public void PrintWindow()
        {
            var ellapsed = DateTimeExtensions.CurrentTimeMillis() - _windowStart;
            var recsPerSec = 1000.0 * _windowCount / ellapsed;
            var mbPerSec = 1000.0 * _windowBytes / ellapsed / (1024.0 * 1024.0);
            Log.Info($"{_windowCount} records sent, " +
                     $"{recsPerSec} records/sec ({mbPerSec} MB/sec), " +
                     $"{_windowTotalLatency/(double)_windowCount} ms avg latency, " +
                     $"{(double)_windowMaxLatency} max latency.\n");
        }

        public void NewWindow()
        {
            _windowStart = DateTimeExtensions.CurrentTimeMillis();
            _windowCount = 0;
            _windowMaxLatency = 0;
            _windowTotalLatency = 0;
            _windowBytes = 0;
        }

        public void PrintTotal()
        {
            var ellapsed = DateTimeExtensions.CurrentTimeMillis() - _start;
            var recsPerSec = 1000.0 * _count / ellapsed;
            var mbPerSec = 1000.0 * _bytes / ellapsed / (1024.0 * 1024.0);
            var percs = Percentiles(_latencies, _index, 0.5, 0.95, 0.99, 0.999);
            Log.Info($"{_count} records sent, " +
                     $"{recsPerSec} records/sec ({mbPerSec} MB/sec), " +
                     $"{_totalLatency / (double)_count} ms avg latency, " +
                     $"{(double)_maxLatency} ms max latency, " +
                     $"{percs[0]} ms 50th, " +
                     $"{percs[1]} ms 95th, " +
                     $"{percs[2]} ms 99th, " +
                     $"{percs[3]} ms 99.9th.\n");
        }

        private static int[] Percentiles(int[] latencies, int count, params double[] percentiles)
        {
            var size = Math.Min(count, latencies.Length);
            Array.Sort(latencies, 0, size);
            var values = new int[percentiles.Length];
            for (int i = 0; i < percentiles.Length; i++)
            {
                var index = (int)(percentiles[i] * size);
                values[i] = latencies[index];
            }
            return values;
        }
    }
}
