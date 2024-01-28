using Honamic.Configuration.EntityFramework.Sample;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

public interface ITestService
{
    void Run();
}


public class TestService : ITestService
{
    private readonly ILogger<TestService> _logger;
    private readonly IOptionsMonitor<SampleOptions> options;

    public TestService(ILogger<TestService> logger,
        IOptionsMonitor<SampleOptions> sepidarOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = sepidarOptions;
    }

    public void Run()
    {
        _logger.LogWarning("Start ...");

        _logger.LogInformation("timeout: " + options.CurrentValue.Timeout.ToString());
        _logger.LogInformation(JsonSerializer.Serialize(options.CurrentValue));

        _logger.LogWarning("End ...");
    }
}