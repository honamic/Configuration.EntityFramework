using Honamic.Configuration.EntityFramework.Sample;
using Honamic.Configuration.EntityFramework.Storages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

public interface IChangeOptionService
{
    Task Change();
}


public class ChangeOptionService : IChangeOptionService
{
    private readonly ILogger<TestService> _logger;
    private readonly IOptionsMonitor<SampleOptions> options;
    private readonly SettingStorage settingStorage;

    public ChangeOptionService(ILogger<TestService> logger,
        IOptionsMonitor<SampleOptions> options,
        SettingStorage settingStorage)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options;
        this.settingStorage = settingStorage;
    }

    public async Task Change()
    {
        var setting = await settingStorage.GetAsync("SampleOptions", null);

        var option = JsonSerializer.Deserialize<SampleOptions>(setting.Value);
        option.Title = $"Changed at {DateTime.Now:s}";
        settingStorage.AddOrUpdate(option, "SampleOptions", null);

        //setting.Value = null;
        //await settingStorage.UpdateAsync(setting);

    }
}