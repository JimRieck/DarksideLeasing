using Darkside.LeasingCalc.Core.Configuration;
using Darkside.LeasingCalc.Core.Repositories;
using Darkside.LeasingCalc.Core.Service;
using Darkside.LeasingCalc.Core.Validation;
using Darkside.LeasingCalc.Data.Models;
using Darkside.Logger.Client;
using Darkside.Logging.Logger.Client;
using DarkSideLeasing.App.Api.Settings;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

public class Program
{
    public static void Main(string[] args)
    {
        // 🔧 Load configuration and secrets
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>(optional: true, reloadOnChange: true);

        var config = configurationBuilder.Build();
        
        // 🔧 Get strongly typed settings (custom wrapper object if needed)
        var settings = config.Get<DarkSideLeasing.App.Api.Settings.Settings>();

        // 🛠 Build Functions Host
        var builder = FunctionsApplication.CreateBuilder(args);
        builder.Services.Configure<DarkSideLeasing.App.Api.Settings.Settings>(config);
        builder.ConfigureFunctionsWebApplication();

        // Optional default service config
        builder.AddServiceDefaults();

        // 💾 Register database context
        builder.Services.AddDbContext<DbContext, DarksideLeasingCalcDbContext>(
            options => options.UseSqlServer(settings.ConnectionStrings.DarksideLeasing),
            ServiceLifetime.Transient);

        // 🧠 Register services
        builder.Services.AddTransient<ILeaseCalculatorService, LeaseMilageCalculatorService>();
        builder.Services.AddTransient<ILeaseCalculatorRepository, LeaseCalculatorRepository>();
        builder.Services.AddTransient<ICarLeaseRepository, CarLeaseRepository>();
        builder.Services.AddTransient<IValidationService, ValidationService>();

        // 🔐 Register LoggingClientOptions and LoggingClient
        builder.Services.Configure<ServiceBus>(config.GetSection("ConnectionStrings")); // DocumentGenie inside
        builder.Services.AddTransient<ILoggingClient>(provider =>
        {
            var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ServiceBus>>();
            return new LoggingClient(settings.ServiceBus.ConnectionString);
        });

        // 🌩 Azure App Config, optional
        builder.Services.AddAzureAppConfiguration();

        // 🚀 Run
        builder.Build().Run();
    }
}
