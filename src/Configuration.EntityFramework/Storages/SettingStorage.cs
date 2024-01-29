using Microsoft.EntityFrameworkCore;

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
}
