using Honamic.Configuration.EntityFramework;
using Honamic.Configuration.EntityFramework.Extensions;
using Honamic.Configuration.EntityFramework.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Honamic.Extensions.Configuration.EntityFramework;

public class EntityFrameworkConfigurationProvider : ConfigurationProvider
{
    Action<DbContextOptionsBuilder> OptionsAction { get; }

    public EntityFrameworkConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
    {
        OptionsAction = optionsAction;
    }

    public void Reload()
    {
        Load();
    }

    public override void Load()
    {
        var builder = new DbContextOptionsBuilder<EntityFrameworkConfigurationDbContext>();

        OptionsAction(builder);

        using (var dbContext = new EntityFrameworkConfigurationDbContext(builder.Options))
        {
            CreateAppSettingTable(dbContext);

            Data = dbContext.Set<Appseting>()
                .Where(option => option.Application == EntityFrameworkConfiguration.ApplicationName)
                .ToDictionary(option => option.ToKeyName(), option => option.Value);
        }
    }

    public void SeedDefaultOptions()
    {
        var builder = new DbContextOptionsBuilder<EntityFrameworkConfigurationDbContext>();

        OptionsAction(builder);

        using (var dbContext = new EntityFrameworkConfigurationDbContext(builder.Options))
        {
            var optionDbSet = dbContext.Set<Appseting>();

            var currentOptions = optionDbSet.ToList();

            string? CurrentUserName = ClaimsPrincipal.Current?.Identity?.Name;

            foreach (var type in EntityFrameworkConfiguration.OptionTypes)
            {
                if (type.Value.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new Exception("Only accept types that contain a parameterless constructor. " + type.Value.FullName);
                }
                else
                {
                    var instance = Activator.CreateInstance(type.Value);

                    var dic = instance.GetPropertiesValues();

                    var sectionName = type.Key;

                    foreach (var property in dic)
                    {
                        if (!currentOptions.Any(option =>
                           option.Application == EntityFrameworkConfiguration.ApplicationName
                        && option.Name == sectionName
                        && option.Key == property.Key))
                        {
                            optionDbSet.Add(
                                    new Appseting
                                    {
                                        Application = EntityFrameworkConfiguration.ApplicationName,
                                        Name = sectionName,
                                        Value = property.Value,
                                        Key = property.Key,
                                        CreatedOn = DateTimeOffset.Now,
                                        CreatedBy = CurrentUserName
                                    }
                                ); ;
                        }
                    }
                }
            }

            dbContext.SaveChanges();

            Data = optionDbSet.ToDictionary(option => option.ToKeyName(), option => option.Value);
        }
    }

    public void AddOrUpdateAsync<TOption>(TOption options) where TOption : class
    {
        var type = options.GetType();

        var dic = options.GetPropertiesValues();

        var sectionName = type.CreateSectionName();

        var builder = new DbContextOptionsBuilder<EntityFrameworkConfigurationDbContext>();

        OptionsAction(builder);

        using var dbContext = new EntityFrameworkConfigurationDbContext(builder.Options);


        var currentOptions = dbContext.Set<Appseting>().Where(option =>
                        option.Application == EntityFrameworkConfiguration.ApplicationName
                     && option.Name == sectionName).ToList();

        foreach (var property in dic)
        {
            var option = currentOptions.FirstOrDefault(option => option.Key == property.Key);

            if (option == null)
            {
                dbContext.Set<Appseting>().Add(
                        new Appseting
                        {
                            Application = EntityFrameworkConfiguration.ApplicationName,
                            Name = sectionName,
                            Value = property.Value,
                            Key = property.Key,
                            CreatedOn = DateTimeOffset.Now,
                        }
                    );
            }
            else
            {
                option.Value = property.Value;
                option.ModifiedOn = DateTimeOffset.Now;
            }
        }

        dbContext.SaveChanges();

        //reload settings
        Load();
    }

    private static void CreateAppSettingTable(EntityFrameworkConfigurationDbContext dbContext)
    {
        if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
        {
            var query = "SELECT count(*) value  FROM INFORMATION_SCHEMA.TABLES " +
                       "WHERE TABLE_SCHEMA = 'dbo'  AND  TABLE_NAME = 'AppSettings'";
            var exitTable = dbContext.Database.SqlQueryRaw<int>(query).FirstOrDefault();
            if (exitTable != 1)
            {
                var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();
                databaseCreator.CreateTables();
            }
        }
    }

}