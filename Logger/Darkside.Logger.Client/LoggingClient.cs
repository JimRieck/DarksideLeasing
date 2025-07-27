using Azure.Messaging.ServiceBus;
using Darkside.Logging.Contracts.Requests;
using Darkside.Logging.Logger.API.Responses;
using System.Text;
using System.Text.Json;

namespace Darkside.Logging.Logger.Client
{
    public class LoggingClient
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        public LoggingClient(string connectionString, string queueName = "logging")
        {
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(queueName);
        }

        public async Task AddLoggingToServiceBusAsync(AddLoggingRequest entry)
        {
            var json = JsonSerializer.Serialize(entry);
            var message = new ServiceBusMessage(json)
            {
                ContentType = "application/json",
                MessageId = $"{ entry.TenantId?.ToString()}-{entry.Application}-{Guid.NewGuid().ToString().Substring(0,8)}"
            };

            // Optional: Add application-specific metadata for filtering
            message.ApplicationProperties.Add("LogLevel", entry.LogLevel);
            message.ApplicationProperties.Add("Application", entry.Application);

            await _sender.SendMessageAsync(message);
        }

        public async Task<RetrieveLogResponse> RetrieveLoggingAsync(GetLoggingRequest request, HttpClient httpClient)
        {
            var serializedRequest = JsonSerializer.Serialize(request);
            var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var apiResponse = await httpClient.PostAsync("api/AddLog", content);
            if (apiResponse.IsSuccessStatusCode)
            {
                var responseContent = await apiResponse.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<RetrieveLogResponse>(responseContent);
                return result ?? new RetrieveLogResponse();
            }
            else
            {
                // Handle error response
                return null;
            }
        }
    }
}
