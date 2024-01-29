using Honamic.Configuration.EntityFramework.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework;

internal sealed class EntityFrameworkConfigurationSource : IConfigurationSource
{
    public readonly Action<DbContextOptionsBuilder> OptionsAction;
    public IJosnConfigurationParser Parser { get; set; }

    public static string? ApplicationName { get; internal set; }
    public static string TableName { get; internal set; } = "AppSettings";
    public static string? Schema { get; internal set; }


    public EntityFrameworkConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
    {
        OptionsAction = optionsAction;
        Parser = new JsonConfigurationParser();
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EntityFrameworkConfigurationProvider(this);
    }
}
