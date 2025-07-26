using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Darkside.Logging.Datas.Models;
using Darkside.Logging.Application;
using Microsoft.Extensions.Configuration;
using Darkside.Logging.Application.Settings;

public class Program
{
    public static void Main(string[] args)
    {
        var settingsBuilder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddUserSecrets<Program>(optional: true, reloadOnChange: true);

        var config = settingsBuilder.Build();
        var settings = config.Get<AppSettings>();

        var builder = FunctionsApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.ConfigureFunctionsWebApplication();

        // Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
        // builder.Services
        //     .AddApplicationInsightsTelemetryWorkerService()
        //     .ConfigureFunctionsApplicationInsights();

        // Register all validators in the assembly
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // No changes needed here

        // Register MediatR
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register the validation pipeline behavior
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddDbContext<GPSDocumentGenieDataContext>(options => options.UseSqlServer(settings!.ConnectionStrings.DocumentGenie));
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateLogCommandHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(GetLogsQueryHandler).Assembly);
        });

        builder.Build().Run();
    }
}
