using System;

namespace RdKafka
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long) ((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }
    }

    public static class ActionExtensions
    {
        public static Action Times(this Action a, int count)
        {
            return () => { for (int i = 0; i < count; i++) a(); };
        }
    }

    public class Stats
    {
        private readonly Action<string> _print;

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

        public Stats(long numRecords, int reportingInterval, Action<string> print)
        {
            _print = print;
            _start = DateTimeExtensions.CurrentTimeMillis();
            _windowStart = DateTimeExtensions.CurrentTimeMillis();
            _index = 0;
            _iteration = 0;
            _sampling = (int) (numRecords/Math.Min(numRecords, 500000));
            _latencies = new int[(int) (numRecords/_sampling) + 1];
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
            if (iter%_sampling == 0)
            {
                _latencies[_index] = latency;
                _index++;
            }

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
            _iteration++;
            return cb;
        }

        public void PrintWindow()
        {
            var ellapsed = DateTimeExtensions.CurrentTimeMillis() - _windowStart;
            var recsPerSec = 1000.0*_windowCount/ellapsed;
            var mbPerSec = 1000.0*_windowBytes/ellapsed/(1024.0*1024.0);
            _print($"{_windowCount} records sent, " +
                   $"{recsPerSec:F1} records/sec ({mbPerSec:F1} MB/sec), " +
                   $"{_windowTotalLatency/(double) _windowCount:F1} ms avg latency, " +
                   $"{(double) _windowMaxLatency:F1} max latency.");
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
            var recsPerSec = 1000.0*_count/ellapsed;
            var mbPerSec = 1000.0*_bytes/ellapsed/(1024.0*1024.0);
            var percs = Percentiles(_latencies, _index, 0.5, 0.95, 0.99, 0.999);
            _print($"{_count} records sent, " +
                   $"{recsPerSec:F1} records/sec ({mbPerSec:F1} MB/sec), " +
                   $"{_totalLatency/(double) _count:F1} ms avg latency, " +
                   $"{(double) _maxLatency:F1} ms max latency, " +
                   $"{percs[0]} ms 50th, " +
                   $"{percs[1]} ms 95th, " +
                   $"{percs[2]} ms 99th, " +
                   $"{percs[3]} ms 99.9th.");
        }

        private static int[] Percentiles(int[] latencies, int count, params double[] percentiles)
        {
            var size = Math.Min(count, latencies.Length);
            Array.Sort(latencies, 0, size);
            var values = new int[percentiles.Length];
            for (int i = 0; i < percentiles.Length; i++)
            {
                var index = (int) (percentiles[i]*size);
                values[i] = latencies[index];
            }
            return values;
        }
    }
}
