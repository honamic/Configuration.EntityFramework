using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework.Extensions;

public static class EntityFrameworkConfigurationExtensions
{
    public static string CreateSectionName(this Type type)
    {
        return $"{type.Assembly.GetName().Name},{type.FullName}";
    }

    public static string ToKeyName(this Appseting option)
    {
        return $"{option.Name}{ConfigurationPath.KeyDelimiter}{option.Key}";
    }
}
