using Azure.Core;
using Darkside.LeasingCalc.Contracts.Helpers;
using Darkside.LeasingCalc.Contracts.Requests;
using Darkside.LeasingCalc.Contracts.Response;
using Darkside.LeasingCalc.Core.Repositories;
using Darkside.LeasingCalc.Core.Validation;
using Darkside.LeasingCalc.Data.Models;
using Darkside.Logger.Client;

namespace Darkside.LeasingCalc.Core.Service;

public class LeaseMilageCalculatorService : ILeaseCalculatorService
{
    private readonly ILeaseCalculatorRepository _leaseCalculatorRepository;
    private readonly ICarLeaseRepository _carLeaseRepository;
    private readonly IValidationService _validationService;
    private readonly ILoggingClient _loggingClient;

    public LeaseMilageCalculatorService(ILeaseCalculatorRepository leaseCalculatorRepository, ICarLeaseRepository carLeaseRepository, IValidationService validationService, ILoggingClient loggingClient)
    {
        _leaseCalculatorRepository = leaseCalculatorRepository;
        _carLeaseRepository = carLeaseRepository;
        _validationService = validationService;
        _loggingClient = loggingClient;
    }

    public async Task<DailyMileageCalcResponse> CalculateDailyMilage(DailyMileageCalcRequest request)
    {
        await _loggingClient.BuildLogMessageAndSendAsync($"Calculating Daily Mileage for Car Number: {request.CarNumber}", "Information");
        var response = new DailyMileageCalcResponse() {Success = true, DailyMilageDetails = new List<DailyMileageDetails>() };

        await _loggingClient.BuildLogMessageAndSendAsync($"Validating Request: {request.CarNumber}", "Information");
        var validationResult = _validationService.IsValid(new DailyMilageCalcRequestValidator(), request);
        if (!validationResult.IsValid)
        {
            await _loggingClient.BuildLogMessageAndSendAsync($"Request is invalid: {request.CarNumber}", "Information");
            response = new DailyMileageCalcResponse()
            {
                Success = false,
                Message = $"Validation Errors: {_validationService.GetValidationErrors(validationResult)}"
            };
        }
        else
        {
            await _loggingClient.BuildLogMessageAndSendAsync($"Searching for CarNumber: {request.CarNumber}", "Information");
            var (foundCar, car) = await FindCarNumber(request);
            var carLeaseId = new Guid();
            if (!foundCar)
            {
                await _loggingClient.BuildLogMessageAndSendAsync($"Car was found!  will add it now", "Information");
                carLeaseId = car.Id;
                await _loggingClient.BuildLogMessageAndSendAsync($"Calculating daily milage for # {request.CarNumber} ", "Information");
                var mileageDetails = await CalculateDailyMiles(request);
                await _loggingClient.BuildLogMessageAndSendAsync($"Saving car number:{request.CarNumber}", "Information");
                await SaveMilageEstimates(mileageDetails, carLeaseId);
            }
            else
            {
                await _loggingClient.BuildLogMessageAndSendAsync($"Car was found!  will fetch its details now", "Information");
            }

            var carLeaseDetails = await _carLeaseRepository.GetByIdAsync(car.Id);
            response.TotalYears = carLeaseDetails.TotalYears.Value;
            response.CarNumber = carLeaseDetails.CarNumber;
            response.CustomerName = carLeaseDetails.CustomerName;
            response.StartDateTime = carLeaseDetails.StartDate.Value;
            response.StartingMileage = carLeaseDetails.StartingMileage.Value;
            response.YearlyMiles = (int)carLeaseDetails.YearlyMiles.Value;

            await _loggingClient.BuildLogMessageAndSendAsync($"Searching for car leasing details.", "Information");
            var carCalcDetails = await _leaseCalculatorRepository.SearchAsync(p => p.CarLeaseId == carLeaseId);

            foreach (var calcDetail in carCalcDetails)
            {
                await _loggingClient.BuildLogMessageAndSendAsync($"Processing calculation details for CarLeaseId: {carLeaseId}", "Information");
                response.DailyMilageDetails.Add(new DailyMileageDetails()
                {
                    ExpectedMileage = calcDetail.ExpectedMilage.Value,
                    MileageDifference = calcDetail.MilageDifference.Value,
                    ActualMilage = calcDetail.ActualMileage.Value,
                    MilesDriven = calcDetail.MilesDriven.Value,
                    ExpectedMilesDriven = calcDetail.ExpectedMileDriven.Value,
                    IsDeleted = false,
                    CreatedBy = "system",
                    CarLeaseId = carLeaseId,
                    UpdatedBy = "system",
                    MileageDate = calcDetail.MilageDate.Value,
                    UpdatedDate = DateTime.Now
                });
            }
        }

        return response;
    }

    private async Task SaveMilageEstimates(DailyMileageCalcResponse mileageDetails, Guid carLeaseId)
    {
        try
        {
            await _loggingClient.BuildLogMessageAndSendAsync($"Saving mileage estimates for CarLeaseId: {carLeaseId}", "Information");
            foreach (var detail in mileageDetails.DailyMilageDetails)
            {
                var newModel = detail.ToModel(carLeaseId);
                await _leaseCalculatorRepository.InsertAsync(newModel);
                await _loggingClient.BuildLogMessageAndSendAsync($"Inserted mileage detail for CarLeaseId: {carLeaseId}", "Information");
            }
        }
        catch (Exception e)
        {
            await _loggingClient.BuildLogMessageAndSendAsync($"Saving mileage estimates for CarLeaseId:  {carLeaseId} failed, {e.Message}", "Error");
            throw;
        }
    }

    private async Task<DailyMileageCalcResponse> CalculateDailyMiles(DailyMileageCalcRequest request)
    {
        try
        {
            //Calculation Here
            await _loggingClient.BuildLogMessageAndSendAsync($"Calculating Daily miles for Car Number: {request.CarNumber}", "Information");
            var response = new DailyMileageCalcResponse() { DailyMilageDetails = new List<DailyMileageDetails>() };
            var leaseEndDate = request.StartDate.AddYears(request.TotalYears);
            var totalMilesForLease = request.YearlyMiles * request.TotalYears;
            var totalDaysForLease = request.TotalYears * 365;
            var milesPerDay = totalMilesForLease / totalDaysForLease;
            var currentDay = request.StartDate;
            var previousDayMiles = request.StartingMileage;
            var previousDaysMilageDifference = 0;
            var rnd = new Random();
            await _loggingClient.BuildLogMessageAndSendAsync($"Calculating Take Days: {request.CarNumber} with start date {request.StartDate}", "Information");
            var takeDays = TakeDaysHelper.CalculateTakeDays(request.StartDate);
            for (var loopCounter = 0; loopCounter < takeDays; loopCounter++)
            {
                //Expected Miles - Previous Day's mileage + miles Per Day - done
                //Miles Driven - Random number from 1-100.
                //Total Miles on Car - Previous Day's mileage + miles driven
                //Mileage Difference Total Miles on Car - Expected miles.

                var details = new DailyMileageDetails();
                details.MileageDate = currentDay;
                details.ExpectedMilesDriven = milesPerDay;
                details.MilesDriven = rnd.Next(100);
                details.ExpectedMileage = previousDayMiles + milesPerDay;
                details.ActualMilage = previousDayMiles + details.MilesDriven;
                details.MileageDifference = previousDaysMilageDifference + (details.ActualMilage - details.ExpectedMileage); 

                previousDaysMilageDifference = details.MileageDifference;
                response.DailyMilageDetails.Add(details);
                currentDay = currentDay.AddDays(1);
                previousDayMiles += details.MilesDriven;
            }

            return response;
        }
        catch (Exception e)
        {
            await _loggingClient.BuildLogMessageAndSendAsync($"Calculating Daily Mileage failed for Car Number: {request.CarNumber}, {e.Message}", "Error");
            throw;
        }
    }

    private async Task<(bool, CarLease)> FindCarNumber(DailyMileageCalcRequest carInfo)
    {
        var returnedCar = new CarLease();
        var carFound = false;
        await _loggingClient.BuildLogMessageAndSendAsync($"Searching for car number : {carInfo.CarNumber}", "Information");
        //TODO: Save Data here.  be sure to get the Car Lease ID.
        var car = await _carLeaseRepository.GetCarByCarNumber(carInfo.CarNumber);
        var carLeaseId = Guid.Empty;
        if (car != null)
        {
            await _loggingClient.BuildLogMessageAndSendAsync($"car # {car.CarNumber} was found", "Information");
            carLeaseId = car.Id;
            carFound = true;
            returnedCar = car;
        }
        else
        {
            await _loggingClient.BuildLogMessageAndSendAsync($"car # {carInfo.CarNumber} was found", "Information");
            var newCar = await _carLeaseRepository.InsertAsync(carInfo.ToModel());
            await _loggingClient.BuildLogMessageAndSendAsync($"car # {carInfo.CarNumber} was saved.", "Information");

            carLeaseId = newCar.Id;
            carFound = false;
            returnedCar = newCar;
        }

        return (carFound, returnedCar);
    }
}