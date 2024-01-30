using Honamic.Configuration.EntityFramework.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Honamic.Configuration.EntityFramework.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEntityFrameworkConfigurationService(this IServiceCollection services)
    {
        services.AddTransient<SettingStorage>(c =>
        {
            if (EntityFrameworkConfigurationSource.Current is null)
            {
                throw new Exception("Configure with IConfigurationBuilder.AddEntityFrameworkConfiguration");
            }

            var builder = new DbContextOptionsBuilder<EntityFrameworkConfigurationDbContext>();
            EntityFrameworkConfigurationSource.Current.DbContextOptionsBuilder(builder);
            var dbContext = new EntityFrameworkConfigurationDbContext(builder.Options);

            return new SettingStorage(dbContext);
        });

        return services;
    }
}
