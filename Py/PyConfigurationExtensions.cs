using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;

namespace NetCoreConfigTest.Py
{
    public static class PyConfigurationExtensions
    {
        public static IConfigurationBuilder AddPyFile(this IConfigurationBuilder builder, string path)
        {
            return AddPyFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddPyFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddPyFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddPyFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddPyFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
        }

        public static IConfigurationBuilder AddPyFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));

            return builder.AddPyFile(s =>
            {
                s.FileProvider = provider;
                s.Path = path;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ResolveFileProvider();
            });
        }

        public static IConfigurationBuilder AddPyFile(this IConfigurationBuilder builder, Action<PyConfigurationSource> configureSource) => builder.Add(configureSource);
    }
}
