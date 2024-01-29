using Honamic.Configuration.EntityFramework.Extensions;
using Honamic.Configuration.EntityFramework.Sample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


IConfiguration Configuration = null;


var builder = new HostBuilder()
.ConfigureAppConfiguration((hostContext, builder) =>
{
    builder.AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

    var baseConfig = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

    var sqlConnectionString = baseConfig.GetConnectionString("AppContext");

    builder.AddEntityFrameworkConfiguration(opt =>
    {
        opt.DbContextOptionsBuilder = (builder) => builder.UseSqlServer(sqlConnectionString);
        opt.ApplicationName = null;
        //opt.Schema = null;
        //opt.TableName = null;
    });
})
.ConfigureServices((hostContext, services) =>
{
    Configuration = hostContext.Configuration;

    services.AddLogging(configure => configure.AddConsole().AddDebug());
    services.AddTransient<ITestService, TestService>();
    services.Configure<SampleOptions>(Configuration.GetSection("SampleOptions"));
});

var host = builder.Build();

// EntityFrameworkConfiguration.Current.SeedDefaultOptions();

//host.Run();

using (var serviceScope = host.Services.CreateScope())
{
    IServiceProvider services = serviceScope.ServiceProvider;
    var testService = services.GetRequiredService<ITestService>();

#if DEBUG
    var root = (IConfigurationRoot)Configuration;
    var debugView = root.GetDebugView();
#endif


    testService.Run();


}

Console.ReadKey();
