using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace NetCoreConfigTest.Yaml
{
    class YamlConfigurationProvider : FileConfigurationProvider
    {
        public YamlConfigurationProvider(YamlConfigurationSource source) : base(source) { }

        public override void Load(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);
                var data = new Dictionary<string, string>();
                foreach (var (key, value) in yaml.SelectMany(d => ParseNode(d.RootNode)))
                    data[key] = value;
                Data = data;
            }
        }

        private static string CombineKeys<T>(string parent, T child)
        {
            return parent == null ? child.ToString() : $"{parent}:{child}";
        }

        private static IEnumerable<(string key, string value)> ParseNode(YamlNode node, string parentKey = null)
        {
            switch (node)
            {
                case YamlScalarNode scalar:
                    yield return (parentKey, scalar.Value);
                    break;
                case YamlSequenceNode sequence:
                    for (var i = 0; i < sequence.Children.Count; ++i)
                    {
                        var child = sequence.Children[i];
                        foreach (var (childKey, childValue) in ParseNode(child, CombineKeys(parentKey, i)))
                            yield return (childKey, childValue);
                    }
                    break;
                case YamlMappingNode mapping:
                    foreach (var kvp in mapping.Children)
                    {
                        var key = ((YamlScalarNode)kvp.Key).Value;
                        foreach (var (childKey, childValue) in ParseNode(kvp.Value, CombineKeys(parentKey, key)))
                            yield return (childKey, childValue);
                    }
                    break;
            }
        }
    }
}
