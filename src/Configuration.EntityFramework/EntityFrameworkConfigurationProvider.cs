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

        using var dbContext = new EntityFrameworkConfigurationDbContext(builder.Options);

        var settings = dbContext.Set<Setting>()
             .Where(option => option.Application == EntityFrameworkConfigurationSource.ApplicationName)
             .Select(option => new SettingNameValue(option.Name, option.Value))
             .ToList();

        Data = ConvertToConfigDictionary(settings);
    }

    private Dictionary<string, string> ConvertToConfigDictionary(List<SettingNameValue> result)
    {
        return result
            .Where(item => !string.IsNullOrEmpty(item.JsonValue))
            .SelectMany(c => ConvertToConfig(c, _configurationSource.Parser))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value, StringComparer.OrdinalIgnoreCase);
    }

    internal static IEnumerable<KeyValuePair<string, string>> ConvertToConfig(SettingNameValue nameValue,
           IJosnConfigurationParser parser)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(nameValue.JsonValue ?? "");
        using MemoryStream stream = new MemoryStream(byteArray);
        return parser.Parse(stream)
              .Select(
                    pair =>
                    {
                        var key = $"{nameValue.Name}:{pair.Key}".Trim(':');
                        return new KeyValuePair<string, string>(key, pair.Value);
                    });
    }
}

