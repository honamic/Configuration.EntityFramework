
using Honamic.Extensions.Configuration.EntityFramework;

namespace Honamic.Configuration.EntityFramework;

internal static class EntityFrameworkConfiguration
{
    static EntityFrameworkConfiguration()
    {
        OptionTypes = new Dictionary<string, Type>();
        ApplicationName = null;
    }

    public static EntityFrameworkConfigurationProvider Current { get; internal set; }

    public static Dictionary<string, Type> OptionTypes { get; internal set; }

    public static string? ApplicationName { get; internal set; }
}
