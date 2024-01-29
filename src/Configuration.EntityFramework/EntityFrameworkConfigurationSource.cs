using Honamic.Configuration.EntityFramework.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework;

internal sealed class EntityFrameworkConfigurationSource : IConfigurationSource, IEntityFrameworkConfiguration
{
    public static EntityFrameworkConfigurationSource? Current { get; private set; }

    internal IJosnConfigurationParser Parser { get; set; }

    public Action<DbContextOptionsBuilder> DbContextOptionsBuilder { get; set; } = default!;
    public string? ApplicationName { get; set; }
    public string TableName { get; set; } = default!;
    public string? Schema { get; set; }

    public EntityFrameworkConfigurationSource()
    {
        Parser = new JsonConfigurationParser();
        TableName = "AppSettings";
    }

    public EntityFrameworkConfigurationSource(Action<DbContextOptionsBuilder> optionsAction) : this()
    {
        DbContextOptionsBuilder = optionsAction;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        Current = this;
        return new EntityFrameworkConfigurationProvider(Current);
    }
}
