using Microsoft.Extensions.Configuration;
using NetCoreConfigTest.Py;
using NetCoreConfigTest.Yaml;
using System;
using System.IO;

namespace NetCoreConfigTest
{
    class Program
    {
        public static string ProjectDirectory
        {
            get
            {
                var asmLocation = typeof(Program).Assembly.Location;
                var asmDir = Path.GetDirectoryName(asmLocation);
                return Path.GetFullPath(Path.Combine(asmDir, "../../.."));
            }
        }

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(ProjectDirectory)
                .AddJsonFile("settings1.json", false, true)
                .AddJsonFile("settings2.json", false, true)
                .AddIniFile("settings3.ini", false, true)
                .AddYamlFile("settings4.yml", false, true)
                .AddPyFile("settings5.py", false, true);

            var config = builder.Build().GetSection("key2");
            do
            {
                DumpConfig(config);
                Console.WriteLine("______________________________________");
            }
            while (Console.ReadLine() != "q");
        }

        static void DumpConfig(IConfiguration config, string indent = "")
        {
            if (config is IConfigurationSection section)
            {
                Console.WriteLine($"{indent}{section.Key}: {section.Value}");
                indent += "  ";
            }

            foreach (var child in config.GetChildren())
                DumpConfig(child, indent);
        }
    }
}
