﻿using Microsoft.Extensions.Configuration;
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

        private static bool FileMatches(WebClient webClient, Regex regex, string line)
        {
            try
            {
                if (!IsValidUrl(line))
                {
                    Console.Error.WriteLine($"String is not a valid URL: {line}");
                    return false;
                }

                var html = webClient.DownloadString(line);
                return regex.IsMatch(html);
            }
            catch (WebException e)
            {
                Console.Error.WriteLine("An exception occurred: " + e.Message + $"{Environment.NewLine}on URL: {line}");
            }
            return false;
        }

        public static bool IsValidUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out Uri returnURL)
                && (returnURL.Scheme == Uri.UriSchemeHttp || returnURL.Scheme == Uri.UriSchemeHttps);

        private static List<string> MatchingUrls(string[] lines, IConfigurationRoot configurationRoot)
        {
            var regex = new Regex(configurationRoot["regex"], RegexOptions.Compiled);
            var webClient = new WebClient();
            var result = new List<string>();

            foreach (var line in lines)
            {
                var lineTrimmed = line.Trim();
                if (FileMatches(webClient, regex, lineTrimmed))
                {
                    Console.WriteLine($"Match found in file {lineTrimmed}");
                    result.Add(lineTrimmed);
                }
                else
                {
                    Console.WriteLine($"No match found in file {lineTrimmed}");
                }
            }
            return result;
        }

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
