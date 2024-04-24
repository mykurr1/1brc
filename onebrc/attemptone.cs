//One Billion Row challange attempt one - shit performance 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class attemptone
{
    static void start(string[] args)
    {
        const int Buffersize = 128; // Increase Buffersize later

        var measurementsDictionary = new Dictionary<string, List<double>>();

        using (var filesStream = File.OpenRead(@"T:\1brc\1brc.data\1brc\mediumbig.txt"))
        using (var streamReader = new StreamReader(filesStream, Encoding.UTF8, true, Buffersize))
        {
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                var parts = line.Split(';'); // Use ';' as separator
                if (parts.Length == 2) // Ensure the line has correct format
                {
                    var station = parts[0];
                    double temperature;
                    if (double.TryParse(parts[1], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out temperature))
                    {
                        if (!measurementsDictionary.ContainsKey(station))
                        {
                            measurementsDictionary[station] = new List<double>();
                        }
                        measurementsDictionary[station].Add(temperature);
                    }
                }
            }
        }

        // Sort dictionary by station name
        var sortedMeasurementsDictionary = measurementsDictionary.OrderBy(kvp => kvp.Key);

        // Calculate and print min, mean, and max values per station
        foreach (var kvp in sortedMeasurementsDictionary)
        {
            var station = kvp.Key;
            var temperatures = kvp.Value;

            if (temperatures.Any())
            {
                var min = double.MaxValue; // Initialize with a large value
                var max = temperatures.Max();
                var sum = 0.0;

                foreach (var temperature in temperatures)
                {
                    if (temperature < min)
                        min = temperature;
                    sum += temperature;
                }

                var mean = sum / temperatures.Count;

                Console.WriteLine($"Station: {station}");
                Console.WriteLine($"Min Temperature: {min:F2}°C, Mean Temperature: {mean:F2}°C, Max Temperature: {max:F2}°C");
            }
            else
            {
                Console.WriteLine($"Station: {station} - No temperature measurements available.");
            }

            Console.WriteLine();
        }
    }
}