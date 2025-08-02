using Darkside.Logging.Contracts.Requests;
using Darkside.Logging.Logger.API.Responses;

namespace Darkside.Logger.Client
{
    public interface ILoggingClient
    {
        Task BuildLogMessageAndSendAsync(string message, string logLevel);
        Task AddLoggingToServiceBusAsync(AddLoggingRequest entry);
        Task<RetrieveLogResponse> RetrieveLoggingAsync(GetLoggingRequest request, HttpClient httpClient);

    }
}
