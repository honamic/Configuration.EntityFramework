﻿using Honamic.Configuration.EntityFramework.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework;

internal sealed class EntityFrameworkConfigurationSource : IConfigurationSource, IEntityFrameworkConfiguration
{
    public static EntityFrameworkConfigurationSource? Current { get; private set; }

    internal IJosnConfigurationParser Parser { get; set; }

    public Dictionary<string, Type> InitializeTypes { get; set; }
    public Action<DbContextOptionsBuilder> DbContextOptionsBuilder { get; set; } = default!;
    public string? ApplicationName { get; set; }
    public string TableName { get; set; } = default!;
    public string? Schema { get; set; }
    public bool AutoCreateTable { get; set; }
    public Action? Reload { get; private set; }

    public EntityFrameworkConfigurationSource()
    {
        Parser = new JsonConfigurationParser();
        TableName = "AppSettings";
        InitializeTypes = new Dictionary<string, Type>();
    }

    public EntityFrameworkConfigurationSource(Action<DbContextOptionsBuilder> optionsAction) : this()
    {
        DbContextOptionsBuilder = optionsAction;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        Current = this;
        var provider = new EntityFrameworkConfigurationProvider(Current);
        Current.Reload = provider.Reload;
        return provider;
    }
}
