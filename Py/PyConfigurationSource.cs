using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace NetCoreConfigTest.Py
{
    public class PyConfigurationSource : JsonConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new PyConfigurationProvider(this);
        }
    }
}
