using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.Configuration.EntityFramework.Extensions;

public static class ServiceCollectionConfigurationExtensions
{
    public static void ConfigureByFullNameSection<TOption>(this IServiceCollection services, IConfiguration configuration, bool registerForEFSource = true) where TOption : class, new()
    {
        var type = typeof(TOption);

        var sectionName = type.CreateSectionName();

        if (registerForEFSource)
        {
            EntityFrameworkConfiguration.OptionTypes.Add(sectionName, type);
        }

        services.Configure<TOption>(configuration.GetSection(sectionName));
    }

    public static void ConfigureByCustomSection<TOption>(this IServiceCollection services, IConfiguration configuration, string sectionName, bool registerForEFSource = true) where TOption : class, new()
    {
        var type = typeof(TOption);

        if (registerForEFSource)
        {
            EntityFrameworkConfiguration.OptionTypes.Add(sectionName, type);
        }

        services.Configure<TOption>(configuration.GetSection(sectionName));
    }
}