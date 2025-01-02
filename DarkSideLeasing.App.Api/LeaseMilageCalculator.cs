using System.Net;
using Darkside.LeasingCalc.Contracts.Requests;
using Darkside.LeasingCalc.Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DarkSideLeasing.App.Api
{
    public class LeaseMilageCalculator
    {
        private readonly ILogger<LeaseMilageCalculator> _logger;
        private readonly ILeaseCalculatorService _leaseCalculatorService;

        public LeaseMilageCalculator(ILogger<LeaseMilageCalculator> logger, ILeaseCalculatorService leaseCalculatorService)
        {
            _logger = logger;
            _leaseCalculatorService = leaseCalculatorService;
        }

        [Function("LeaseMilageCalculator")]
        public async Task<HttpResponseData> CalculateDailyMileage([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData httpRequest)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var httpRequestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
            var apiRequest = JsonConvert.DeserializeObject<DailyMileageCalcRequest>(httpRequestBody);

            var serviceResponse = await _leaseCalculatorService.CalculateDailyMilage(apiRequest);


            var response = httpRequest.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            await response.WriteStringAsync(JsonConvert.SerializeObject(serviceResponse));

            return response;
        }
    }
}
