using Honamic.Configuration.EntityFramework.Parser;
using Honamic.Configuration.EntityFramework.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Honamic.Configuration.EntityFramework;

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
        OnReload();
    }

    public override void Load()
    {
        var builder = new DbContextOptionsBuilder<EntityFrameworkConfigurationDbContext>();

        _configurationSource.DbContextOptionsBuilder(builder);

        using var storage = new SettingStorage(new EntityFrameworkConfigurationDbContext(builder.Options));

        if (_configurationSource.AutoCreateTable)
        {
            storage.CreateSettingTable(_configurationSource.TableName, _configurationSource.Schema);
        }

        if (_configurationSource.InitializeTypes.Any())
        {
            storage.SeedDefaultOptions(_configurationSource.InitializeTypes, _configurationSource.ApplicationName);
        }

        Data = ConvertToConfigDictionary(storage.GetAll(_configurationSource.ApplicationName));
    }

    private Dictionary<string, string> ConvertToConfigDictionary(ICollection<SettingNameValue> result)
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

