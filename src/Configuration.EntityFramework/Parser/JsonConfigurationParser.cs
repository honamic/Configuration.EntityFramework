using Microsoft.Extensions.Configuration.Json;

namespace Honamic.Configuration.EntityFramework.Parser;
public sealed class JsonConfigurationParser : IJosnConfigurationParser
{
    public IDictionary<string, string> Parse(Stream stream)
    {
        return JsonStreamParser.Parse(stream);
    }

    private sealed class JsonStreamParser : JsonStreamConfigurationProvider
    {
        private JsonStreamParser(JsonStreamConfigurationSource source)
            : base(source)
        {
        }

        internal static IDictionary<string, string> Parse(Stream stream)
        {
            var provider = new JsonStreamParser(new JsonStreamConfigurationSource { Stream = stream });
            provider.Load();
            return provider.Data;
        }
    }
}