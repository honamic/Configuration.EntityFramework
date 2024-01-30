using Honamic.Configuration.EntityFramework.Extensions;
using Honamic.Configuration.EntityFramework.Sample;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


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
        // opt.DbContextOptionsBuilder = (builder) => builder.UseInMemoryDatabase("inmem");
        opt.ApplicationName = null;
        opt.AutoCreateTable = true;
        opt.Schema = "basic";
        opt.TableName = "Options";
        opt.InitializeTypes = new Dictionary<string, Type>
        {
            {"SampleOptions",typeof(SampleOptions)}
        };
    });
})
.ConfigureServices((hostContext, services) =>
{
    Configuration = hostContext.Configuration;

    services.AddLogging(configure => configure.AddConsole().AddDebug());
    services.AddEntityFrameworkConfigurationService();
    services.AddTransient<ITestService, TestService>();
    services.AddTransient<IChangeOptionService, ChangeOptionService>();
    services.Configure<SampleOptions>(Configuration.GetSection("SampleOptions"));
});

var host = builder.Build();


//host.Run();

using (var serviceScope = host.Services.CreateScope())
{
    IServiceProvider services = serviceScope.ServiceProvider;
    var testService = services.GetRequiredService<ITestService>();
    var changeOptionService = services.GetRequiredService<IChangeOptionService>();

#if DEBUG
    var root = (IConfigurationRoot)Configuration;
    var debugView = root.GetDebugView();
#endif


    testService.Run();
    await changeOptionService.Change();
    //Configuration.GetReloadToken().RegisterChangeCallback(c =>
    //{
    //    Debugger.Break();
    //}, null);

    //var configRoot = Configuration as IConfigurationRoot;

    //configRoot.Reload();

    testService.Run();

}

Console.ReadKey();
