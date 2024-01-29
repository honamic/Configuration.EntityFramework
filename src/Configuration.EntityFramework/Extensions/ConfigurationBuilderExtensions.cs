using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddEntityFrameworkConfiguration(this IConfigurationBuilder builder, string SqlConnectionString)
    {
        return builder.AddEntityFrameworkConfiguration(options => options.UseSqlServer(SqlConnectionString));
    }

    public static IConfigurationBuilder AddEntityFrameworkConfiguration(this IConfigurationBuilder builder,
            Action<DbContextOptionsBuilder> optionsAction,
            string? applicationName = null)
    {
        if (applicationName?.Length > 100)
        {
            throw new ArgumentException("The value can be a maximum of 100 characters", nameof(applicationName));
        }

        var configurationSource = new EntityFrameworkConfigurationSource(optionsAction);

        EntityFrameworkConfigurationSource.ApplicationName = applicationName;

        return builder.Add(configurationSource);
    }
}
