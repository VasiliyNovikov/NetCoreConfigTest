using Microsoft.Extensions.Configuration;
using NetCoreConfigTest.Py;
using NetCoreConfigTest.Yaml;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            var svcBuilder = new ServiceCollection();

            var cfgBuilder = new ConfigurationBuilder()
                .SetBasePath(ProjectDirectory)
                .AddJsonFile("settings1.json", false, true)
                .AddJsonFile("settings2.json", false, true)
                .AddIniFile("settings3.ini", false, true)
                .AddYamlFile("settings4.yml", false, true)
                .AddPyFile("settings5.py", false, true);

            var config = cfgBuilder.Build();

            svcBuilder.AddSingleton<IConfiguration>(config);
            svcBuilder.AddOptions();
            svcBuilder.Configure<ConfigObj>(config.GetSection("object"));

            using (var services = svcBuilder.BuildServiceProvider())
            {
                var configSection = config.GetSection("key2");
                var objAccessor = services.GetService<IOptionsMonitor<ConfigObj>>();
                do
                {
                    //DumpConfig(config);
                    //DumpConfig(configSection);
                    DumpObj(objAccessor);
                    Console.WriteLine("______________________________________");
                } while (Console.ReadLine() != "q");
            }
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

        static void DumpObj(IOptionsMonitor<ConfigObj> objAccessor)
        {
            var obj = objAccessor.CurrentValue;
            Console.WriteLine($"IntProp: {obj.IntProp}");
            Console.WriteLine($"StrProp: {obj.StrProp}");
            Console.WriteLine($"ArrProp: [{String.Join(", ", obj.ArrayProp)}]");
        }

        class ConfigObj
        {
            public int IntProp { get; set; }
            public string StrProp { get; set; }
            public string[] ArrayProp { get; set; }
        }
    }
}
