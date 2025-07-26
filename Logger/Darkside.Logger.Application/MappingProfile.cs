using AutoMapper;
using Darkside.Logging.Application.Features.Logging.Commands;
using Darkside.Logging.Contracts.Requests;
using Darkside.Logging.Logger.API.Responses;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Darkside.Logging.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GetLoggingRequest, GetLogsQuery>();
            CreateMap<AddLoggingRequest, CreateLogCommand>();
        }
    }
}