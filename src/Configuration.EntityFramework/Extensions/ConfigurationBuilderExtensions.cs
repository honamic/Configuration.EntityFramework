using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Honamic.Configuration.EntityFramework.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSqlServerEntityFrameworkConfiguration(this IConfigurationBuilder builder, string SqlConnectionString)
    {
        return builder.AddEntityFrameworkConfiguration(opt =>
        {
            opt.DbContextOptionsBuilder = (builder) => builder.UseSqlServer(SqlConnectionString);
        });
    }


    public static IConfigurationBuilder AddEntityFrameworkConfiguration(this IConfigurationBuilder builder,
            Action<IEntityFrameworkConfiguration> optionsAction)
    {
        var configurationSource = new EntityFrameworkConfigurationSource();
      
        optionsAction.Invoke(configurationSource);

        if (configurationSource.ApplicationName?.Length > 100)
        {
            throw new ArgumentException("The value can be a maximum of 100 characters", nameof(configurationSource.ApplicationName));
        }

        if ( configurationSource.DbContextOptionsBuilder.ToString()=="")
        {
            throw new ArgumentException("The value must be configured.", nameof(configurationSource.DbContextOptionsBuilder));
        }

        return builder.Add(configurationSource);
    }
}
