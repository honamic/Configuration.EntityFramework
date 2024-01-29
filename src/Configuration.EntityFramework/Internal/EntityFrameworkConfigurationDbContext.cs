﻿using Microsoft.EntityFrameworkCore;

namespace Honamic.Configuration.EntityFramework.Internal;

internal class EntityFrameworkConfigurationDbContext : DbContext
{
    public EntityFrameworkConfigurationDbContext(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var settingTypeConfiguration = new SettingEntityTypeConfiguration(EntityFrameworkConfigurationSource.TableName
                                                        , EntityFrameworkConfigurationSource.Schema);
        modelBuilder.ApplyConfiguration(settingTypeConfiguration);
    }
}
