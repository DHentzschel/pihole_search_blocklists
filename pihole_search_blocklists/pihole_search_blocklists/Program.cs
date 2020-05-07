using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace pihole_search_blocklists
{
    internal static class Program
    {
        private static void Main(string[] args)
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

            var lines = ReadFile(config["filePath"]);
            if (lines.Length == 0)
            {
                Console.WriteLine($"{nameof(lines)}.Length is 0");
            }
            var urls = MatchingUrls(lines, config);

            WriteFile(config["outputPath"], urls);
        }

        private static List<string> MatchingUrls(string[] lines, IConfigurationRoot configurationRoot)
        {
            var regex = new Regex(configurationRoot["regex"], RegexOptions.Compiled);
            var webClient = new WebClient();
            var result = new List<string>();

            foreach (var line in lines)
            {
                if (FileMatches(webClient, regex, line))
                {
                    Console.WriteLine($"Match found in file {line}");
                    result.Add(line);
                }
                else
                {
                    Console.WriteLine($"No match found in file {line}");
                }
            }
            return result;
        }

        private static bool FileMatches(WebClient webClient, Regex regex, string line)
        {
            try
            {
                var html = webClient.DownloadString(line);
                return regex.IsMatch(html);
            }
            catch (WebException e)
            {
                Console.Error.WriteLine("An exception occurred: " + e.Message + $"{Environment.NewLine}on URL: {line}");
            }
            return false;
        }

        public static bool IsValidUri(string uri)
            => Uri.TryCreate(uri, UriKind.Absolute, out Uri returnURL)
                && (returnURL.Scheme == Uri.UriSchemeHttp || returnURL.Scheme == Uri.UriSchemeHttps);

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
