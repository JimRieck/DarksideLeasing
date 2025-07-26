using AutoMapper;
using Darkside.Logging.Contracts.Requests;
using Darkside.Logging.Logger.API.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Darkside.Logging.Logger.API
{
    public class RetrieveLog
    {
        private readonly ILogger<RetrieveLog> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public RetrieveLog(ILogger<RetrieveLog> logger, IMediator mediator, IMapper mapper)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
        }

        [Function("RetrieveLog")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            if (req.Body == null)
                return new BadRequestObjectResult("Request body is empty.");

            GetLoggingRequest? requestData;
            try
            {
                requestData = await JsonSerializer.DeserializeAsync<GetLoggingRequest>(req.Body);
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult("Malformed JSON payload.");
            }

            if (requestData == null)
                return new BadRequestObjectResult("Invalid log payload.");

            var query = _mapper.Map<GetLogsQuery>(requestData);
            var logs = await _mediator.Send(query);

            var response = new RetrieveLogResponse
            {
                Count = logs.Count,
                Logs = logs
            };

            return new OkObjectResult(response);
        }
    }
}
