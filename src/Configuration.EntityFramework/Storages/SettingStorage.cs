using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Claims;
using System.Text.Json;

namespace Honamic.Configuration.EntityFramework.Storages;
public class SettingStorage : IDisposable
{
    private readonly DbContext dbContext;

    public SettingStorage(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public ICollection<SettingNameValue> GetAll(string? applicationName)
    {
        return dbContext.Set<Setting>()
                  .Where(option => option.Application == applicationName)
                  .Select(option => new SettingNameValue(option.SectionName, option.Value))
                  .ToList();
    }

    public void CreateSettingTable(string tableName, string? schema)
    {
        switch (dbContext.Database.ProviderName)
        {
            case "Microsoft.EntityFrameworkCore.SqlServer":

                schema = string.IsNullOrEmpty(schema) ? "dbo" : schema;

                var query = "SELECT count(*) value  FROM INFORMATION_SCHEMA.TABLES " +
                      $"WHERE TABLE_SCHEMA = '{schema}'  AND  TABLE_NAME = '{tableName}'";

                var exitTable = dbContext.Database.SqlQueryRaw<int>(query).FirstOrDefault();
                if (exitTable != 1)
                {
                    var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();
                    databaseCreator.CreateTables();
                }
                break;
            default:
                throw new NotImplementedException($"Table creation for {dbContext.Database.ProviderName} is not Supported");
        }

    }

    public void SeedDefaultOptions(Dictionary<string, Type> optionTypes, string? applicationName)
    {
        var optionDbSet = dbContext.Set<Setting>();

        var currentSettings = optionDbSet.ToList();

        foreach (var optionType in optionTypes)
        {
            var sectionName = optionType.Key;

            if (!currentSettings.Any(option => option.Application == applicationName
            && option.SectionName == sectionName))
            {
                var instance = GetInstance(optionType.Value);

                optionDbSet.Add(
                        new Setting
                        {
                            Application = applicationName,
                            SectionName = sectionName,
                            Value = SerializeSetting(instance),
                            CreatedOn = DateTimeOffset.Now,
                            CreatedBy = CurrentUserName(),
                        }
                    ); ;
            }
        }

        dbContext.SaveChanges();
    }

    public async Task<Setting?> GetAsync(string sectionName, string? applicationName)
    {
        return await dbContext.Set<Setting>().FirstOrDefaultAsync(option =>
                        option.Application == applicationName
                        && option.SectionName == sectionName);
    }

    public void AddOrUpdate<TOption>(TOption options, string sectionName, string? applicationName) where TOption : class
    {
        var currentSetting = dbContext.Set<Setting>().FirstOrDefault(option =>
                        option.Application == applicationName
                        && option.SectionName == sectionName);

        if (currentSetting == null)
        {
            dbContext.Set<Setting>().Add(
                    new Setting
                    {
                        Application = applicationName,
                        SectionName = sectionName,
                        Value = SerializeSetting(options),
                        CreatedOn = DateTimeOffset.Now,
                        CreatedBy = CurrentUserName(),
                    }
                );
        }
        else
        {
            currentSetting.Value = SerializeSetting(options);
            currentSetting.ModifiedOn = DateTimeOffset.Now;
            currentSetting.ModifiedBy = CurrentUserName();
        }

        dbContext.SaveChanges();

        Reload();
    }

    public async Task UpdateAsync(Setting updateSetting)
    {
        if (!string.IsNullOrEmpty(updateSetting.Value) && !IsValidJson(updateSetting.Value))
        {
            throw new ArgumentException("the value must be json", nameof(updateSetting.Value));
        }

        var currentSetting = await dbContext.Set<Setting>().FirstOrDefaultAsync(option =>
                        option.Id == updateSetting.Id);

        if (currentSetting == null)
        {
            throw new ArgumentException(nameof(updateSetting.Id));
        }
        else
        {
            currentSetting.Value = currentSetting.Value;
            currentSetting.SectionName = currentSetting.SectionName;
            currentSetting.Application = currentSetting.Application;
            currentSetting.ModifiedOn = DateTimeOffset.Now;
            currentSetting.ModifiedBy = CurrentUserName();
        }

        await dbContext.SaveChangesAsync();

        Reload();
    }

    public void Dispose()
    {
        dbContext.Dispose();
    }

    private static object? GetInstance(Type optionType)
    {
        if (optionType.GetConstructor(Type.EmptyTypes) == null)
        {
            throw new Exception("Only accept types that contain a parameterless constructor. " + optionType.FullName);
        }

        return Activator.CreateInstance(optionType);
    }

    private static string SerializeSetting(object? instance)
    {
        return JsonSerializer.Serialize(instance);
    }

    private static void Reload()
    {
        if (EntityFrameworkConfigurationSource.Current?.Reload != null)
        {
            EntityFrameworkConfigurationSource.Current.Reload();
        }
    }

    bool IsValidJson(string jsonString)
    {
        try
        {
            var jsonDocument = JsonDocument.Parse(jsonString);
            return false;
        }
        catch (Exception exc)
        {
            return false;
        }
    }

    private static string? CurrentUserName()
    {
        var userName = ClaimsPrincipal.Current?.Identity?.Name;

        if (!string.IsNullOrEmpty(userName) && userName.Length > 50)
        {
            userName = userName.Substring(0, 50);
        }

        return userName;
    }
}