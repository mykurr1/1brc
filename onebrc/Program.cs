//1brc attempt one
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        const int Buffersize = 512; // Increase Buffersize later

        var measurementsDictionary = new Dictionary<string, List<double>>();

        using (var filesStream = File.OpenRead(@"T:\1brc\1brc.data\1brc\input.txt"))
        using (var streamReader = new StreamReader(filesStream, Encoding.UTF8, true, Buffersize))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var parts = line.Split(";");
                if (parts.Length == 2) // Ensure the line has correct format 
                {
                    var station = parts[0];
                    double measurement;
                    if (double.TryParse(parts[1], out measurement))
                    {
                        if (!measurementsDictionary.ContainsKey(station))
                        {
                            measurementsDictionary[station] = new List<double>();
                        }
                        measurementsDictionary[station].Add(measurement);
                    }
                }
            }
        }

        // Calculate and print min, mean, and max values per station
        foreach (var kvp in measurementsDictionary)
        {
            var station = kvp.Key;
            var measurements = kvp.Value;

            if (measurements.Any())
            {
                var min = measurements.Min();
                var mean = measurements.Average();
                var max = measurements.Max();

                Console.WriteLine($"Station: {station}");
                Console.WriteLine($"Min: {min}, Mean: {mean}, Max: {max}");
            }
            else
            {
                Console.WriteLine($"Station: {station} - No measurements available.");
            }

            Console.WriteLine();
        }
    }
}
