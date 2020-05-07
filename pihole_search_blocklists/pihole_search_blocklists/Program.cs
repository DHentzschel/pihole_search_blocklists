using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
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
        }
    }
}
