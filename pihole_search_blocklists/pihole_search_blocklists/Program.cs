using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace pihole_search_blocklists
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {{ "-f", "filePath" }};
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, switchMappings);

            var config = builder.Build();
            Console.WriteLine($"filepath: '{config["filePath"]}'");
        }
    }
}
