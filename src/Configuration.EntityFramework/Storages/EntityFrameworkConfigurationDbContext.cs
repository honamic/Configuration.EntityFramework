using Microsoft.EntityFrameworkCore;

namespace Honamic.Configuration.EntityFramework.Storages;

internal class EntityFrameworkConfigurationDbContext : DbContext
{
    public EntityFrameworkConfigurationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var settingTypeConfiguration = new SettingEntityTypeConfiguration(EntityFrameworkConfigurationSource.Current.TableName
                                                        , EntityFrameworkConfigurationSource.Current.Schema);
        modelBuilder.ApplyConfiguration(settingTypeConfiguration);
    }
}
