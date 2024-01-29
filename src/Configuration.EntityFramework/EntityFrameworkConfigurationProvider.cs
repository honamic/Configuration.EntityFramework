using Honamic.Configuration.EntityFramework;
using Honamic.Configuration.EntityFramework.Internal;
using Honamic.Configuration.EntityFramework.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Honamic.Extensions.Configuration.EntityFramework;

internal sealed class EntityFrameworkConfigurationProvider : ConfigurationProvider
{
    private EntityFrameworkConfigurationSource _configurationSource;


    public EntityFrameworkConfigurationProvider(EntityFrameworkConfigurationSource configurationSource)
    {
        if (configurationSource.Parser == null)
        {
            throw new ArgumentNullException(nameof(configurationSource.Parser));
        }

        _configurationSource = configurationSource;
    }

    public void Reload()
    {
        Load();
    }

    public override void Load()
    {
        var builder = new DbContextOptionsBuilder<EntityFrameworkConfigurationDbContext>();

        _configurationSource.OptionsAction(builder);

        using (var dbContext = new EntityFrameworkConfigurationDbContext(builder.Options))
        {
            List<(string Name, string? JsonValue)> settings = dbContext.Set<Setting>()
                .Where(option => option.Application == EntityFrameworkConfigurationSource.ApplicationName)
                .Select(option => new { option.Name, option.Value })
                .ToList()
                 .Select(option => (option.Name, option.Value))
                 .ToList();

            Data = ToConfigDictionary(settings, _configurationSource.Parser);
        }
    }

    internal static Dictionary<string, string> ToConfigDictionary(List<(string Name, string? JsonValue)> result,
               IJosnConfigurationParser parser)
    {
        return result
            .Where(item => !string.IsNullOrEmpty(item.JsonValue))
            .SelectMany(c => ConvertToConfig(c, parser))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
    }

    internal static IEnumerable<KeyValuePair<string, string>> ConvertToConfig(
           (string Name, string? JsonValue) kvPair,
           IJosnConfigurationParser parser)
    {

        byte[] byteArray = Encoding.UTF8.GetBytes(kvPair.JsonValue ?? "");
        using MemoryStream stream = new MemoryStream(byteArray);
        return parser.Parse(stream)
              .Select(
                    pair =>
                    {
                        var key = $"{kvPair.Name}:{pair.Key}".Trim(':');

                        return new KeyValuePair<string, string>(key, pair.Value);
                    });
    }
}

