using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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
                  .Select(option => new SettingNameValue(option.Name, option.Value))
                  .ToList();
    }

    public void Dispose()
    {
        dbContext.Dispose();
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
}
