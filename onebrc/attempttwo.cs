//1BRC second attempt - using multithreading 
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

            using (var reader = new StreamReader("input.txt"))
            {
                var lines = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                Parallel.For(0, lines.Count, i =>
                {
                    var line = lines[i];
                    var separatorIndex = line.IndexOf(';');
                    if (separatorIndex > 0 && separatorIndex < line.Length - 1)
                    {
                        var station = line.AsSpan(0, separatorIndex).ToString();
                        if (double.TryParse(line.AsSpan(separatorIndex + 1), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var temperature))
                        {
                            var newStats = new TemperatureStats { Min = temperature, Max = temperature, Sum = temperature, Count = 1 };
                            if (measurementsDictionary.TryGetValue(station, out var oldStats))
                            {
                                var updatedStats = new TemperatureStats { Min = Math.Min(oldStats.Min, temperature), Max = Math.Max(oldStats.Max, temperature), Sum = oldStats.Sum + temperature, Count = oldStats.Count + 1 };
                                measurementsDictionary.TryUpdate(station, updatedStats, oldStats);
                            }
                            else
                            {
                                measurementsDictionary.TryAdd(station, newStats);
                            }
                        }
                    }
                });
            }

            var sortedMeasurementsDictionary = measurementsDictionary.OrderBy(kvp => kvp.Key);

            using (var writer = new StreamWriter("output.txt"))
            {
                var output = new List<string> {"{"};
                foreach (var kvp in sortedMeasurementsDictionary)
                {
                    var station = kvp.Key;
                    var data = kvp.Value;
                    var mean = data.Sum / data.Count;
                    output.Add($"{station}={data.Min:F1}/{mean:F1}/{data.Max:F1}, ");
                }
                output[output.Count - 1] = output.Last().TrimEnd(',', ' '); // Remove last comma and space
                output.Add("}");
                writer.Write(string.Join("", output));
            }
        }
    }
}