using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onebrc
{
    struct TemperatureStats
    {
        public double Min;
        public double Max;
        public double Sum;
        public int Count;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var measurementsDictionary = new ConcurrentDictionary<string, TemperatureStats>(Environment.ProcessorCount * 2, 1000000000);

            var lines = File.ReadLines(@"T:\1brc\1brc.data\1brc\mediumbig.txt").ToArray();
            var rangePartitioner = Partitioner.Create(0, lines.Length);

            Parallel.ForEach(rangePartitioner, range =>
            {
                var buffer = ArrayPool<char>.Shared.Rent(128); // Rent a buffer from the pool
                try
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        var line = lines[i];
                        var separatorIndex = line.IndexOf(';');
                        if (separatorIndex > 0 && separatorIndex < line.Length - 1)
                        {
                            var station = line.AsSpan(0, separatorIndex).ToString();
                            if (double.TryParse(line.AsSpan(separatorIndex + 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var temperature))
                            {
                                measurementsDictionary.AddOrUpdate(station,
                                    new TemperatureStats { Min = temperature, Max = temperature, Sum = temperature, Count = 1 },
                                    (_, old) => new TemperatureStats { Min = Math.Min(old.Min, temperature), Max = Math.Max(old.Max, temperature), Sum = old.Sum + temperature, Count = old.Count + 1 });
                            }
                        }
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(buffer); // Return the buffer to the pool
                }
            });

            var sortedMeasurementsDictionary = measurementsDictionary.OrderBy(kvp => kvp.Key);

            var sb = new StringBuilder(sortedMeasurementsDictionary.Count() * 50); // Estimate the capacity
            sb.Append("{");
            foreach (var kvp in sortedMeasurementsDictionary)
            {
                var station = kvp.Key;
                var data = kvp.Value;
                var mean = data.Sum / data.Count;
                sb.AppendFormat("{0}={1:F1}/{2:F1}/{3:F1}, ", station, data.Min, mean, data.Max);
            }
            if (sb.Length > 2)
            {
                sb.Length -= 2; // Remove last comma and space
            }
            sb.Append("}");

            Console.WriteLine(sb.ToString());
        }
    }
}