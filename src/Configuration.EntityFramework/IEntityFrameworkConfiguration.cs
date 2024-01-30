using Microsoft.EntityFrameworkCore;

namespace Honamic.Configuration.EntityFramework;

public interface IEntityFrameworkConfiguration
{
    Action<DbContextOptionsBuilder> DbContextOptionsBuilder { get; set; }

    public string? ApplicationName { get; set; }

    public string TableName { get; set; }

    public string? Schema { get; set; }

    public bool AutoCreateTable { get; set; }

    public Dictionary<string, Type> InitializeTypes { get; set; }

}