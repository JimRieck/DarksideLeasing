using System.Text.Json;
using AutoMapper;
using Darkside.Logging.Application.Features.Logging.Commands;
using Darkside.Logging.Contracts.Requests;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Darkside.Logging.Logger.API;

public class AddLog
{
    private readonly ILogger<AddLog> _logger;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public AddLog(ILogger<AddLog> logger, IMediator mediator, IMapper mapper)
    {
        _logger = logger;
        _mediator = mediator;
        _mapper = mapper;
    }

    [Function(nameof(AddLog))]
    public async Task Run([ServiceBusTrigger("logging", Connection = "ServiceBusConnectionString")] string message,
    FunctionContext context)
    {
        try
        {
            var request = JsonSerializer.Deserialize<AddLoggingRequest>(message);
            if (request == null)
                throw new InvalidOperationException("Invalid log message payload");
            var command = _mapper.Map<CreateLogCommand>(request);
            var logId = await _mediator.Send(command);
            
            // No manual message completion needed for QueueTrigger
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process log message");
            // For QueueTrigger, failed messages are retried or dead-lettered automatically
        }
    }
}
