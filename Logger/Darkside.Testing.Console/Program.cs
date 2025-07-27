using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Darkside.Logging.Logger.Client;
using Darkside.Logging.Contracts.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Darkside.Logging.Datas.Models;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Enter the number of requests to send: ");
        if (!int.TryParse(Console.ReadLine(), out int numberOfRequests) || numberOfRequests <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a positive number.");
            return;
        }

        Console.Write("Enter the number of connections: ");
        if (!int.TryParse(Console.ReadLine(), out int numberOfConnections) || numberOfConnections <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a positive number.");
            return;
        }

        // Load the Service Bus connection string using ConfigurationBuilder
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var serviceBusConnectionString = configuration["ServiceBus:ConnectionString"];
        if (string.IsNullOrEmpty(serviceBusConnectionString))
        {
            Console.WriteLine("Service Bus connection string not found in secrets file.");
            return;
        }

        // Set up the database context
        var optionsBuilder = new DbContextOptionsBuilder<GPSDocumentGenieDataContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DarksideLogging"));

        var loggingClient = new LoggingClient(serviceBusConnectionString);
        var logLevels = new[] { "Info", "Warning", "Error" };
        var random = new Random();

        using var semaphore = new SemaphoreSlim(numberOfConnections);

        Console.WriteLine("Starting the test...");

        for (int i = 0; i < numberOfRequests; i++)
        {
            await semaphore.WaitAsync();

            _ = Task.Run(async () =>
            {
                try
                {
                    var logLevel = logLevels[random.Next(logLevels.Length)];
                    var logRequest = new AddLoggingRequest
                    {
                        Application = "Darkside Leasing",
                        LogLevel = logLevel,
                        Message = logLevel switch
                        {
                            "Info" => "The Force is strong with this one.",
                            "Warning" => "I find your lack of faith disturbing.",
                            "Error" => "The Death Star has been destroyed!",
                            _ => "There is a great disturbance in the force.  A new jedi has come to save the rebels.  We must destroy him.."
                        },
                        Exception = logLevel == "Error" ? "Darth Vader encountered an unexpected error while using the Force." : null,
                        CreatedBy = "DarksideLogger",
                        CreatedDate = DateTime.UtcNow,
                        UpdatedBy = "DarksideLogger",
                        UpdatedDate = DateTime.UtcNow,
                        Module = "StarWarsModule",
                        Properties = "{\"Key\":\"Value\"}"
                    };

                    try
                    {
                        await loggingClient.AddLoggingToServiceBusAsync(logRequest);
                        Console.WriteLine($"Log message with level '{logLevel}' successfully added to the Service Bus queue.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }

        Console.WriteLine("Test completed.");
    }
}
