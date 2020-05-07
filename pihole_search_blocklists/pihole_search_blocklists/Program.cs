using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace pihole_search_blocklists
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "-f", "filePath" },
                { "-r", "regex" },
                { "-o", "outputPath" }
            };
            if (args.Length < 3 || switchMappings.Any(p => !args.Contains(p.Key)))
            {
                Console.Error.WriteLine("Error: Please type in all required arguments: -f (filePath), -r (regex) and -o (outputPath)");
                return;
            }
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, switchMappings);

            var config = builder.Build();
        private static string[] ReadFile(string filePath)
        {
            try
            {
                Console.WriteLine($"Reading file {filePath}");
                return File.ReadAllLines(filePath);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            return new string[0];
        }

        private static void WriteFile(string filePath, List<string> contents)
        {
            try
            {
                var stringBuilder = new StringBuilder();
                foreach (var line in contents)
                {
                    stringBuilder.Append(line);
                    stringBuilder.Append(Environment.NewLine);
                }
                Console.WriteLine($"Writing matching urls to file {filePath}");
                File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
    }
}
